﻿using Google.Apis.YouTube.Samples;
using System;
using System.Drawing;
using System.Globalization;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;
using System.Collections;

namespace VideoUp
{
    public partial class MainForm : Form
    {
        private UploadVideo uploadV;

        private Timer check;
        private string subtitleFile;
        private string subFile = "";

        private string _template;
        private string _templateArguments;

        private TimeSpan startTimes = new TimeSpan(00, 00, 00);
        private TimeSpan endTimes = new TimeSpan(10, 10, 10);
        private TimeSpan startTimesSub = new TimeSpan(00, 00, 00);
        private TimeSpan endTimesSub = new TimeSpan(10, 10, 10);

        private string _autoOutput;
        private string _autoTitle;
        private string _autoArguments;
        private bool _argumentError;

        public Size AssumedInputSize;

        /// <summary>
        /// Handles the cropping rectangle shown to the user
        /// </summary>
        public RectangleF CroppingRectangle
        {
            get { return _croppingRectangle; }
            set
            {
                _croppingRectangle = value;
                if (_croppingRectangle == CropForm.FullCrop)
                    labelCrop.Text = "Don't crop";
                else
                    labelCrop.Text = string.Format(CultureInfo.InvariantCulture, "X:{0:0%} Y:{1:0%} W:{2:0%} H:{3:0%}",
                                                   _croppingRectangle.X,
                                                   _croppingRectangle.Y,
                                                   _croppingRectangle.Width,
                                                   _croppingRectangle.Height);
            }
        }
        private RectangleF _croppingRectangle;

        /// <summary>
        /// Starts the form with the following ffmpeg command placeholders
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            CroppingRectangle = new RectangleF(0, 0, 1, 1);

            AllowDrop = true;
            DragEnter += HandleDragEnter;
            DragDrop += HandleDragDrop;

            // default arguments
            //_templateArguments = "{0} -vcodec libx264 -preset fast -crf 32 -b:v {1}K {2} {3} {11} {6} {7} {8} {9} {10}";
            _templateArguments = "{0} -codec:v libx264 -preset fast -crf 25 -b:v {1}K {2} {3} {11} {6} {7} {8} {9} {10}";
            //{0} is '-an' if no audio, otherwise blank
            //{1} is bitrate in kb/s
            //{2} is '-vf scale=WIDTH:HEIGHT' if set otherwise blank
            //{3} is '-filter:v "crop=out_w:out_h:x:y"' if set otherwise blank
            //{6} is '-metadata title="TITLE"' when specifying a title, otherwise blank
            //{7} is '-metadata author="AUTHOR"' when specifying an author, otherwise blank
            //{8} is '-metadata year="YEAR"' when specifying a date, otherwise blank
            //{9} is '-metadata description="DESCRIPTION"' when specifying a description, otherwise blank
            //{10} is '-metadata copyright="© DCCT"' when setting the copyright
            //{11} is 'subtitle' when including the subtitle

            // selected arguments by user
            _template = "{2} -i \"{0}\" {3} {4} {5} -f avi -y \"{1}\"";
            //{0} is input file
            //{1} is output file
            //{2} is TIME if seek enabled otherwise blank
            //{3} is TIME if to enabled otherwise blank
            //{4} is extra arguments
            //{5} is '-pass X' if 2-pass enabled, otherwise blank
        }

        /// <summary>
        /// Loads the form
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            resBox.SelectedIndex = 1;
            uploadV = new UploadVideo();
            trackThreads_Scroll(sender, e);
        }

        /// <summary>
        /// Opens the video file specified by the user
        /// </summary>
        private void buttonBrowseIn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.ValidateNames = true;
                dialog.Filter = "Video files (*.mxf, *.wmv, *.avi, *.flv, *.mkv, *.mov, *.mp4, *.mpg) | *.mxf; *.wmv; *.avi; *.flv; *.mkv; *.mov; *.mp4; *.mpg";

                if (dialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
                    SetFile(dialog.FileName);
            }
        }

        /// <summary>
        /// Sets the path of the directory where the video file is
        /// </summary>
        /// <param name="path">the directory of the video file.>/param>
        private void SetFile(string path)
        {
            textBoxIn.Text = path;
           
            string fullPath = Path.GetDirectoryName(path);
            axWindowsMediaPlayer1.URL = @textBoxIn.Text.Replace(@"\\", @"\");

            if (!textBoxIn.Text.Equals(""))
            {
                startTime.Enabled = true;
                endTime.Enabled = true;
            }

            string name = Path.GetFileNameWithoutExtension(path);
            if (boxMetadataTitle.Text == _autoTitle || boxMetadataTitle.Text == "")
                boxMetadataTitle.Text = _autoTitle = name;
            if (textBoxOut.Text == _autoOutput || textBoxOut.Text == "")
                textBoxOut.Text = _autoOutput = Path.Combine(fullPath, name + ".avi");
            //textBox2.Text = textBoxOut.Text.Substring(textBoxOut.Text.LastIndexOf(@"\") + 1);
        }

        /// <summary>
        /// Handles drag and drop
        /// </summary>
        private void HandleDragEnter(object sender, DragEventArgs e)
        {
            bool dataPresent = e.Data.GetDataPresent(DataFormats.FileDrop);
            e.Effect = dataPresent ? DragDropEffects.Copy : DragDropEffects.None;
        }

        /// <summary>
        /// Handles drag and drop
        /// </summary>
        private void HandleDragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files) SetFile(file);
        }

        /// <summary>
        /// Sets the path of the directory where the video file will be saved
        /// </summary>
        private void buttonBrowseOut_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.OverwritePrompt = true;
                dialog.ValidateNames = true;
                dialog.Filter = "avi file (*.avi) | *.avi";

                if (dialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
                    textBoxOut.Text = dialog.FileName;
            }
        }

        /// <summary>
        /// Starts the conversion process
        /// </summary>
        private void buttonGo_Click(object sender, EventArgs e)
        {
            string result = Convert();
            if (!string.IsNullOrWhiteSpace(result))
                MessageBox.Show(result, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        char[] invalidChars = Path.GetInvalidPathChars();

        /// <summary>
        /// Starts the conversion process, and checks if the necessary information is at hand
        /// </summary>
        private string Convert()
        {
            string input = textBoxIn.Text;
            string output = textBoxOut.Text;

            if (string.IsNullOrWhiteSpace(input))
                return "No input file!";
            if (string.IsNullOrWhiteSpace(output))
                return "No output file!";

            if (invalidChars.Any(input.Contains))
                return "Input path contains invalid characters!\nInvalid characters: " + string.Join(" ", invalidChars);
            if (invalidChars.Any(output.Contains))
                return "Output path contains invalid characters!\nInvalid characters: " + string.Join(" ", invalidChars);

            if (!File.Exists(input))
                return "Input file doesn't exist!";

            if (input == output)
                return "Input and output files are the same!";

            float startSeconds, endSeconds;

            try
            {
                startSeconds = ParseTime(boxCropFrom.Text);
            }
            catch (ArgumentException)
            {
                return "Invalid start crop time!";
            }
            try
            {
                endSeconds = ParseTime(boxCropTo.Text);
            }
            catch (ArgumentException)
            {
                return "Invalid end crop time!";
            }

            string options = textBoxArguments.Text;
            try
            {
                if (options.Trim() == "" || _argumentError)
                    options = GenerateArguments();
            }
            catch (ArgumentException e)
            {
                return e.Message;
            }

            string start = "";
            string end = "";

            if (startSeconds != 0.0)
            {
                start = "-ss " + startSeconds.ToString(CultureInfo.InvariantCulture); //Convert comma to dot
            }

            float duration = 0;

            if (endSeconds != 0.0)
            {
                duration = endSeconds - startSeconds;
                if (duration <= 0)
                    return "Video is 0 or less seconds long!";

                end = "-to " + duration.ToString(CultureInfo.InvariantCulture); //Convert comma to dot
            }

            string[] arguments;
            if (!checkBox2Pass.Checked)
                arguments = new[] { string.Format(_template, input, output, start, end, options, "", "") };
            else
            {
                int passes = 2;

                arguments = new string[passes];
                for (int i = 0; i < passes; i++)
                    arguments[i] = string.Format(_template, input, output, start, end, options, "-pass " + (i + 1));
            }

            var form = new ConverterForm(this, arguments);
            form.ShowDialog();

            return null;
        }

        /// <summary>
        /// Adds subtitles to the application as a separate track
        /// </summary>
        /*public void MkvMerge()
        {
            string inText = "\"" + textBoxOut.Text + "\"";
            string outText = "\"" + textBoxOut.Text + "\"";
            subtitleFile = "\"" + subtitleFile + "\"";
            int dot = outText.LastIndexOf(@".");
            outText = outText.Insert(dot, "sub");

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/c FFmpeg\\mkvmerge -o " + outText + " " + inText + " " + subtitleFile;
            process.StartInfo = startInfo;
            process.Start();
        }*/

        /// <summary>
        /// Calls DropIt to do file management
        /// </summary>
        public void manageFiles()
        {
            string text = textBoxOut.Text.Substring(0, textBoxOut.Text.LastIndexOf(@"\"));
            text = "\"" + text + "\"";

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "Dropit\\DropIt.exe";
            process.StartInfo = startInfo;
            process.Start();

            /*System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = text,
                UseShellExecute = true,
                Verb = "open"
            });*/
        }

        /// <summary>
        /// Checks trim time selected
        /// </summary>
        /// <param name="text">the trim time that has been set.>/param>
        public static float ParseTime(string text)
        {

            if (!string.IsNullOrWhiteSpace(text))
            {
                if (text.Contains(":"))
                {
                    TimeSpan time;
                    if (!TimeSpan.TryParse(MakeParseFriendly(text), CultureInfo.InvariantCulture, out time))
                        throw new ArgumentException("Invalid time!");
                    return (float)time.TotalSeconds;
                }
                else
                {
                    float time;
                    if (!float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out time))
                        throw new ArgumentException("Invalid time!");
                    return time;
                }
            }
            return 0.0f;
        }

        /// <summary>
        /// Updates arguments of FFmpeg command
        /// </summary>
        private void UpdateArguments(object sender, EventArgs e)
        {
            /*try
            {
                string arguments = GenerateArguments();
                if (arguments != _autoArguments || _argumentError)
                {
                    textBoxArguments.Text = _autoArguments = arguments;
                    _argumentError = false;
                }
            }
            catch (ArgumentException argExc)
            {
                textBoxArguments.Text = "ERROR: " + argExc.Message;
                _argumentError = true;
            }*/
        }

        /// <summary>
        /// Generates the FFmpeg commands based on user input
        /// </summary>
        private string GenerateArguments()
        {
            int width = 0;
            int height = 0;
            
            string[] res = resBox.SelectedItem.ToString().Split('x');

            if (!string.IsNullOrWhiteSpace(res[0]) || !string.IsNullOrWhiteSpace(res[1]))
            {
                if (!int.TryParse(res[0], out width))
                    throw new ArgumentException("Invalid width");
                if (!int.TryParse(res[1], out height))
                    throw new ArgumentException("Invalid height");
            }

            if ((!string.IsNullOrWhiteSpace(res[0]) && string.IsNullOrWhiteSpace(res[1])) ||
                (string.IsNullOrWhiteSpace(res[0]) && !string.IsNullOrWhiteSpace(res[1])))
                throw new ArgumentException("One of the width/height fields isn't filled in! Either fill none of them, or both of them!");
            
            string sizeCrop = "";
            if (_croppingRectangle != CropForm.FullCrop)
            {
                int assumedWidth = width;
                int assumedHeight = height;
                if (width == -1 || height == -1)
                    throw new ArgumentException("Sorry, but you can't crop while using -1 in one of the resolution fields.");
                if (width == 0 || height == 0) 
                {
                    assumedWidth = AssumedInputSize.Width;
                    assumedHeight = AssumedInputSize.Height;

                    if (assumedWidth == 0 || assumedHeight == 0)
                        throw new ArgumentException("For some reason you've cropped without generating a preview image.");
                }

                int cropX = (int)(assumedWidth * _croppingRectangle.X);
                int cropY = (int)(assumedHeight * _croppingRectangle.Y);
                int cropW = (int)(assumedWidth * _croppingRectangle.Width);
                int cropH = (int)(assumedHeight * _croppingRectangle.Height);

                sizeCrop = string.Format("-filter:v crop=\"{0}:{1}:{2}:{3}\"", cropW, cropH, cropX, cropY);
            }

            float startSeconds = ParseTime(boxCropFrom.Text);

            float duration = 0;

            float endSeconds = ParseTime(boxCropTo.Text);
            if (endSeconds != 0.0)
            {
                duration = endSeconds - startSeconds;
                if (duration <= 0)
                    throw new ArgumentException("Video is 0 or less seconds long!");
            }

            float limit = 0;
            string limitTo = "";

            string size = "";
            if (width != 0 && height != 0)
                size = string.Format("-vf scale={0}:{1}", width, height);

            int bitrate = 900;
            if (duration != 0 && limit != 0)
            {
                bitrate = (int)(8192 * limit / duration);
            }

            int threads = trackThreads.Value;

            // below is the metadata information
            string metadataTitle = "";
            if (!string.IsNullOrWhiteSpace(boxMetadataTitle.Text))
                metadataTitle = string.Format("-metadata title=\"{0}\"", boxMetadataTitle.Text.Replace("\"", "\\\""));
            vidNameUpload.Text = boxMetadataTitle.Text.Substring(boxMetadataTitle.Text.LastIndexOf(@"\") + 1);

            string metadataAuthor = "";
            if (!string.IsNullOrWhiteSpace(boxMetadataAuthor.Text))
                metadataAuthor = string.Format("-metadata author=\"{0}\"", boxMetadataAuthor.Text);

            string metadataYear = "";
            string theDate = dateTimeMetadata.Value.ToString("yyyy-MM-dd");
            if (!string.IsNullOrWhiteSpace(theDate))
                metadataYear = string.Format("-metadata date=\"{0}\"", theDate);

            string metadataDesc = "";
            if (!string.IsNullOrWhiteSpace(boxMetadataDesc.Text))
                metadataDesc = string.Format("-metadata comment=\"{0}\"", boxMetadataDesc.Text);
            textBox4.Text = boxMetadataDesc.Text.Substring(boxMetadataDesc.Text.LastIndexOf(@"\") + 1);

            string metadataCopy = "-metadata copyright=\"(c) DCCT\"";

            if (!string.IsNullOrWhiteSpace(subFile))
            {
                subFile = string.Format("-vf \"subtitles='{0}'\"", subFile);
                subFile = @subFile.Replace(@"\", @"\\");
                subFile = subFile.Insert(17, "\\");
                //Console.WriteLine(subFile);
            }

            string audioEnabled = boxAudio.Checked ? "" : "-an";
            return string.Format(_templateArguments, audioEnabled, bitrate, size, sizeCrop, threads, limitTo, subFile, metadataTitle, metadataAuthor, metadataYear, metadataDesc, metadataCopy);
        }

        /// <summary>
        /// Checks the trim time
        /// </summary>
        /// <param name="text">checks the trim time.>/param>
        private static string MakeParseFriendly(string text)
        {
            string pattern = @"^[0-5][0-9]:[0-5][0-9](\.[0-9]+)?$";
            Regex regex = new Regex(pattern, RegexOptions.Singleline);
            if (regex.IsMatch(text))
                return "00:" + text;
            return text;
        }

        /// <summary>
        /// Checks threads involved
        /// </summary>
        private void trackThreads_Scroll(object sender, EventArgs e)
        {
            labelThreads.Text = trackThreads.Value.ToString();
            UpdateArguments(sender, e);
        }

        /// <summary>
        /// Opens the crop form
        /// </summary>
        private void buttonOpenCrop_Click(object sender, EventArgs e)
        {
            new CropForm(this).ShowDialog();
            UpdateArguments(sender, e);
        }

        /// <summary>
        /// Throws error if the format of the video is not playable by windows media player
        /// </summary>
        private void axWindowsMediaPlayer1_MediaError(object sender, AxWMPLib._WMPOCXEvents_MediaErrorEvent e)
        {
            MessageBox.Show("The current format is not working. Please try again.");
        }

        /// <summary>
        /// Opens the video to be uploaded
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            uploadButton.Enabled = true;
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.ValidateNames = true;
                dialog.Filter = "Video files (*.wmv, *.avi, *.flv, *.mkv, *.mov, *.mp4, *.mpg) | *.wmv; *.avi; *.flv; *.mkv; *.mov; *.mp4; *.mpg";

                if (dialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
                    SetFile1(dialog.FileName);
            }
        }

        /// <summary>
        /// Sets the path of the directory where the video file is
        /// </summary>
        /// <param name="path">the directory of the video file.>/param>
        private void SetFile1(string path)
        {
            //textBox2.Text = path.Substring(path.LastIndexOf(@"\") + 1);
            textBox2.Text = path;
        }

        private void buttonSubBrowse_Click_1(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.ValidateNames = true;
                dialog.Filter = "Subtitle file (*.srt) | *.srt";

                if (dialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
                    SetbuttonSubBrowse(dialog.FileName);
            }
        }

        /// <summary>
        /// Sets the path of the directory where the video file is
        /// </summary>
        /// <param name="path">the directory of the video file.>/param>
        private void SetbuttonSubBrowse(string path)
        {
            subtitleFile = path;
            textBox1.Text = path.Substring(path.LastIndexOf(@"\") + 1);
            subFile = path;
        }

        /// <summary>
        /// Starts the uploading process
        /// </summary>
        private void uploadButton_Click(object sender, EventArgs e)
        {
            string title, desc, path;
            title = vidNameUpload.Text;
            desc = textBox4.Text;
            path = textBox2.Text;

            // checks if all information is there
            if (string.IsNullOrWhiteSpace(title))
                MessageBox.Show("Enter a name for the video");
            if (string.IsNullOrWhiteSpace(desc))
                MessageBox.Show("Enter a description for the video");
            if (string.IsNullOrWhiteSpace(path))
                MessageBox.Show("Select a video to upload");

            if (invalidChars.Any(path.Contains))
                MessageBox.Show("Video directory has invalid characters: " + string.Join(" ", invalidChars));

            if (!File.Exists(path))
                MessageBox.Show("No video has been selected for upload");

            uploadV.passValues(title, desc, path);
            uploadV.startUpload();
        }

        /// <summary>
        /// Sets the start time for trimming
        /// </summary>
        private void startTime_Click(object sender, EventArgs e)
        {
            if (!(axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsStopped))
            {
                startTimes = TimeSpan.Parse(axWindowsMediaPlayer1.Ctlcontrols.currentPositionString.Insert(0, "00:"));

                if (startTimes < endTimes)
                {
                    startTimeValid.Visible = false;
                    startTimeBox.Text = startTimes.ToString();
                    boxCropFrom.Text = startTimes.ToString();
                }

                else if (startTimes == endTimes)
                    startTimeValid.Visible = true;

                else
                    startTimeValid.Visible = true;
            }
        }

        /// <summary>
        /// Sets the end time for trimming
        /// </summary>
        private void endTime_Click(object sender, EventArgs e)
        {
            if (!(axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsStopped))
            {
                endTimes = TimeSpan.Parse(axWindowsMediaPlayer1.Ctlcontrols.currentPositionString.Insert(0, "00:"));

                if (endTimes > startTimes)
                {
                    endTimeValid.Visible = false;
                    endTimeBox.Text = endTimes.ToString();
                    boxCropTo.Text = endTimes.ToString();
                }

                else if (endTimes == startTimes)
                    endTimeValid.Visible = true;

                else
                    endTimeValid.Visible = true;
            }
        }

        /// <summary>
        /// Opens DropIt from the navigation bar
        /// </summary>
        private void fileManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //string text = textBoxOut.Text.Substring(0, textBoxOut.Text.LastIndexOf(@"\"));
            //text = "\"" + text + "\"";

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "Dropit\\DropIt.exe";
            process.StartInfo = startInfo;
            process.Start();
        }

        /// <summary>
        /// Exits the application
        /// </summary>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Opens DropIt from the navigation bar
        /// </summary>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "VideoUp File|*.vidup";
            saveFileDialog1.Title = "Save VideoUp Project";

            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string name = saveFileDialog1.FileName;
                string content = textBoxIn.Text + "\n" + textBoxOut.Text + "\n" +
                boxMetadataTitle.Text + "\n" + boxMetadataAuthor.Text + "\n" + startTimeBox.Text + "\n" + endTimeBox.Text
                + "\n" + resBox.Text + "\n" + boxCropFrom.Text + "\n" + boxCropTo.Text + "\n" + textBox1.Text + "\n"
                + dateTimeMetadata.Text + "\n" + boxMetadataDesc.Text + "\n" + vidNameUpload.Text + "\n" + textBox2.Text + "\n"
                + textBox4.Text;
                File.WriteAllText(name, content);
                MessageBox.Show("Project Saved");
            }
            //SaveFile(project);
        }

        /// <summary>
        /// Opens the project file
        /// </summary>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Open VideoUp File";
            theDialog.Filter = "VideoUp File|*.vidup";

            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamReader sr = new
                System.IO.StreamReader(theDialog.FileName);
                textBoxIn.Text = sr.ReadLine();
                axWindowsMediaPlayer1.URL = @textBoxIn.Text.Replace(@"\\", @"\");
                //axWindowsMediaPlayer1.Ctlcontrols.stop();

                textBoxOut.Text = sr.ReadLine();
                boxMetadataTitle.Text = sr.ReadLine();
                boxMetadataAuthor.Text = sr.ReadLine();
                startTimeBox.Text = sr.ReadLine();
                endTimeBox.Text = sr.ReadLine();
                resBox.Text = sr.ReadLine();
                boxCropFrom.Text = sr.ReadLine();
                boxCropTo.Text = sr.ReadLine();
                buttonSubBrowse.Enabled = true;
                textBox1.Text = sr.ReadLine();

                dateTimeMetadata.Text = sr.ReadLine();
                boxMetadataDesc.Text = sr.ReadLine();
                vidNameUpload.Text = sr.ReadLine();
                textBox2.Text = sr.ReadLine();
                textBox4.Text = sr.ReadLine();

                startTime.Enabled = true;
                endTime.Enabled = true;
                radioSubNone.Checked = false;
                radioSubExternal.Checked = true;

                sr.Close();
            }
        }

        /// <summary>
        /// Clears the current project
        /// </summary>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.currentPlaylist.clear();

            textBoxIn.Text = "";
            textBoxOut.Text = "";
            boxMetadataTitle.Text = "";
            boxMetadataAuthor.Text = "";
            startTimeBox.Text = "";
            endTimeBox.Text = "";
            resBox.Text = "";
            boxCropFrom.Text = "";
            boxCropTo.Text = "";

            textBox1.Text = "";
            buttonSubBrowse.Enabled = false;
            textBox1.Enabled = false;
            vidNameUpload.Text = "";
            textBox2.Text = "";
            textBox4.Text = "";

            dateTimeMetadata.Text = "";
            boxMetadataDesc.Text = "";
            //infoBox.Text = "";
        }

        /// <summary>
        /// Opens the user manual
        /// </summary>
        private void documentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // check if it works on any PC
            Process.Start(@"https://docs.google.com/document/d/113OsjPjqSicUW_3b3dAk0sk5gadAvkKaEBrUbZu9lEU/edit?usp=sharing");
        }

        /// <summary>
        /// Opens the DCCT website
        /// </summary>
        private void dCCTWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.dcct.org.za/");
        }

        /// <summary>
        /// Opens project webpage
        /// </summary>
        private void projectWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://people.cs.uct.ac.za/~nzxlub001/");
        }

        /// <summary>
        /// Opens the About Us page
        /// </summary>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string info = "Copyright © 2015 \nCS Honours Project (Monty & Luba) \nDeveloped for the DCCT"
            + "\n\nReference tools and software: \nFFmpeg \nMkvMerge \nDropIt \nWebMConverter";

            MessageBox.Show(info, "VideoUp Application",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Opens the subtitle interface
        /// </summary>
        private void createSubtitlesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new SubtitleForm(this, @textBoxIn.Text.Replace(@"\\", @"\"));
            form.ShowDialog();
        }

        /// <summary>
        /// Checks if a subtitle file is going to be added or not
        /// </summary>
        private void radioSubNone_CheckedChanged(object sender, EventArgs e)
        {
            if (radioSubNone.Checked)
            {
                textBox1.Text = "";
                radioSubExternal.Checked = false;
                buttonSubBrowse.Enabled = false;
            }

            else
            {
                radioSubNone.Checked = false;
                radioSubExternal.Checked = true;
                buttonSubBrowse.Enabled = true;
            }
        }

        /// <summary>
        /// Opens the Help page
        /// </summary>
        private void helpButton_Click(object sender, EventArgs e)
        {
            var form = new HelpForm(this, null);
            form.ShowDialog();
        }

        private void trimClickHere_Click(object sender, EventArgs e)
        {
            var form = new HelpForm(this, @"Help\\Trim video.mp4");
            form.ShowDialog();
        }

        private void infoClickHere_Click(object sender, EventArgs e)
        {
            var form = new HelpForm(this, @"Help\\Info video.mp4");
            form.ShowDialog();
        }

        private void subtitleClickHere_Click(object sender, EventArgs e)
        {
            var form = new HelpForm(this, @"Help\\Subtitle video.mp4");
            form.ShowDialog();
        }

        private void uploadClickHere_Click(object sender, EventArgs e)
        {
            var form = new HelpForm(this, @"Help\\Upload video.mp4");
            form.ShowDialog();
        }
    }
}
