﻿using System;
using System.IO;
using System.Windows.Forms;

namespace VideoUp
{
    public partial class SubtitleForm : Form
    {
        private TimeSpan startTimesSub = new TimeSpan(00, 00, 00);
        private TimeSpan endTimesSub = new TimeSpan(10, 10, 10);
        private int subtitleCount = 1;

        public SubtitleForm(MainForm mainForm, string video)
        {
            InitializeComponent();
            axWindowsMediaPlayer2.URL = video;
        }

        private void SubtitleForm_Load(object sender, EventArgs e)
        {
            
        }

        private void startSubTime_Click(object sender, EventArgs e)
        {
            if (!(axWindowsMediaPlayer2.playState == WMPLib.WMPPlayState.wmppsStopped))
            {
                startTimesSub = TimeSpan.Parse(axWindowsMediaPlayer2.Ctlcontrols.currentPositionString.Insert(0, "00:"));
                if (startTimesSub < endTimesSub)
                {
                    startSubMsg.Visible = false;
                    startSubBox.Text = startTimesSub.ToString();
                }

                else if (startTimesSub == endTimesSub)
                    startSubMsg.Visible = true;

                else
                    startSubMsg.Visible = true;
            }
        }

        private void endSubTime_Click(object sender, EventArgs e)
        {
            if (!(axWindowsMediaPlayer2.playState == WMPLib.WMPPlayState.wmppsStopped))
            {
                endTimesSub = TimeSpan.Parse(axWindowsMediaPlayer2.Ctlcontrols.currentPositionString.Insert(0, "00:"));
                if (endTimesSub > startTimesSub)
                {
                    endSubMsg.Visible = false;
                    endSubBox.Text = endTimesSub.ToString();
                }

                else if (endTimesSub == startTimesSub)
                    endSubMsg.Visible = true;

                else
                    endSubMsg.Visible = true;
            }
        }

        private void enterSubtitle_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(startSubBox.Text) && !string.IsNullOrWhiteSpace(endSubBox.Text) && !string.IsNullOrWhiteSpace(infoBox.Text))
            {
                subtitleGridView.Rows.Add(subtitleCount.ToString(), startSubBox.Text, endSubBox.Text, infoBox.Text);

                infoBox.Text = "";
                startSubBox.Text = "";
                endSubBox.Text = "";
                startTimesSub = new TimeSpan(0, 0, 0);
                endTimesSub = new TimeSpan(10, 10, 10);
                subtitleCount++;
            }
        }

        private void saveSub_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Subtitle File|*.srt";
            saveFileDialog1.Title = "Save subtitle file";

            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string name = saveFileDialog1.FileName;
                int track = 0;
                int num;
                StreamWriter sW = new StreamWriter(name);

                for (int rows = 0; rows < subtitleGridView.Rows.Count; rows++)
                {
                    for (int col = 0; col < subtitleGridView.Rows[rows].Cells.Count; col++)
                    {
                        if (subtitleGridView.Rows[rows].Cells[col].Value != null)
                        {
                            if (int.TryParse(subtitleGridView.Rows[rows].Cells[col].Value.ToString(), out num))
                                sW.WriteLine(subtitleGridView.Rows[rows].Cells[col].Value);

                            else if (track == 0)
                            //else if (subtitleGridView.Rows[rows].Cells[col].Value.ToString().Contains("00") && track == 0)
                            {
                                sW.Write(subtitleGridView.Rows[rows].Cells[col].Value + ",000 --> ");
                                track++;
                            }

                            else if (track == 1)
                            {
                                sW.Write(subtitleGridView.Rows[rows].Cells[col].Value + ",000");
                                sW.Write("\n");
                                track++;
                            }

                            else
                            {
                                sW.WriteLine(subtitleGridView.Rows[rows].Cells[col].Value);
                                sW.Write("\n");
                                track = 0;
                            }
                        }
                    }
                }

                sW.Close();
                MessageBox.Show("Subtitle file saved.");
            }
        }
    }
}
