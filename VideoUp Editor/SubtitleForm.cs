using System;
using System.IO;
using System.Windows.Forms;

namespace VideoUp
{
    public partial class SubtitleForm : Form
    {
        private TimeSpan startTimesSub = new TimeSpan(00, 00, 00);
        private TimeSpan endTimesSub = new TimeSpan(10, 10, 10);
        private int subtitleCount = 1;

        /// <summary>
        /// Fetches the information about a video file from the MainForm.cs class
        /// </summary>
        /// <param name="mainForm">the object that opens this class.>/param>
        /// <param name="video">the video file from the Main.cs class.>/param>
        public SubtitleForm(MainForm mainForm, string video)
        {
            InitializeComponent();
            axWindowsMediaPlayer2.URL = video;
            Load += new EventHandler(SubtitleForm_Load);
        }

        /// <summary>
        /// Opens the video file in the media player when this class loads
        /// </summary>
        private void SubtitleForm_Load(object sender, EventArgs e)
        {
            if (!(axWindowsMediaPlayer2.playState == WMPLib.WMPPlayState.wmppsUndefined))
            {
                startSubTime.Enabled = true;
                endSubTime.Enabled = true;
            }
        }

        /// <summary>
        /// Sets the start time for the subtitle
        /// </summary>
        private void startSubTime_Click(object sender, EventArgs e)
        {
            if (!(axWindowsMediaPlayer2.playState == WMPLib.WMPPlayState.wmppsStopped))
            {
                startTimesSub = TimeSpan.Parse(axWindowsMediaPlayer2.Ctlcontrols.currentPositionString.Insert(0, "00:"));
                // checks if the start time is less than the end time
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

        /// <summary>
        /// Sets the end time for the subtitle
        /// </summary>
        private void endSubTime_Click(object sender, EventArgs e)
        {
            if (!(axWindowsMediaPlayer2.playState == WMPLib.WMPPlayState.wmppsStopped))
            {
                endTimesSub = TimeSpan.Parse(axWindowsMediaPlayer2.Ctlcontrols.currentPositionString.Insert(0, "00:"));
                // checks if the end time is greater than the start time
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

        /// <summary>
        /// Adds the subtitle to the subtitle grid, and clears the subtitle box
        /// </summary>
        private void enterSubtitle_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(startSubBox.Text) && !string.IsNullOrWhiteSpace(endSubBox.Text) && !string.IsNullOrWhiteSpace(infoBox.Text))
            {
                // adds the start time, end tme, and subtitle into the subtitle grid
                subtitleGridView.Rows.Add(subtitleCount.ToString(), startSubBox.Text, endSubBox.Text, infoBox.Text);
                subtitleMsg.Visible = false;

                infoBox.Text = "";
                startSubBox.Text = "";
                endSubBox.Text = "";
                startTimesSub = new TimeSpan(0, 0, 0);
                endTimesSub = new TimeSpan(10, 10, 10);
                subtitleCount++;
            }

            else
                subtitleMsg.Visible = true;
        }

        /// <summary>
        /// Checks when rows have been removed from the subtitle grid, and calls the updateSubtitles() method
        /// </summary>
        private void subtitleGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            updateSubtitles();
        }

        /// <summary>
        /// Updates the subtitle grid when a subtitle has been added or removed
        /// </summary>
        private void updateSubtitles()
        {
            // if there's only one subtitle in the grid, don't do anything to it
            if (subtitleGridView.RowCount == 1)
            {
                if (subtitleGridView.Rows[0].Cells[0].Value != null)
                {
                    subtitleGridView.Rows[0].Cells[0].Value = 1;
                    subtitleCount = 2;
                }
            }

            else
            {
                for (int rows = 1; rows < subtitleGridView.Rows.Count; rows++)
                {
                    if (subtitleGridView.Rows[rows].Cells[0].Value != null)
                    {
                        // if a subtitle is added, make sure the index reflects this
                        if (int.Parse(subtitleGridView.Rows[0].Cells[0].Value.ToString()) != 1)
                        {
                            for (int row = 0; row < subtitleGridView.Rows.Count; row++)
                            {
                                if (subtitleGridView.Rows[row].Cells[0].Value != null)
                                {
                                    subtitleGridView.Rows[row].Cells[0].Value = (int.Parse((subtitleGridView.Rows[row].Cells[0].Value).ToString()) - 1).ToString();
                                }
                                subtitleCount = int.Parse((subtitleGridView.Rows[row].Cells[0].Value).ToString());
                            }
                        }

                        // if a subtitle is removed, make sure the index is updated
                        if (int.Parse(subtitleGridView.Rows[rows].Cells[0].Value.ToString()) - int.Parse(subtitleGridView.Rows[rows - 1].Cells[0].Value.ToString()) != 1)
                        {
                            subtitleGridView.Rows[rows].Cells[0].Value = (int.Parse((subtitleGridView.Rows[rows].Cells[0].Value).ToString()) - 1).ToString();
                            if (subtitleCount > 0)
                                subtitleCount--;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Saves a subtitle to a file specified by the user
        /// </summary>
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
