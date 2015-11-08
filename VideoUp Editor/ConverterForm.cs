using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace VideoUp
{
    public partial class ConverterForm:Form
    {
        private string[] _arguments;
        private FFmpeg _ffmpegProcess;

        private Timer _timer;
        private bool _ended;
        private bool _panic;

        int currentPass = 0;
        private bool _multipass;
        private bool _cancelMultipass;

        private MainForm _owner;
        private double trim, timeLeft, duration;
        private double timeLeftTemp = 0;
        static int wholenum = 0;
        private string frameNum;
        private delegate void EventHandle();

        /// <summary>
        /// Fetches the information about a video file from the MainForm.cs class
        /// </summary>
        /// <param name="mainForm">the object that opens this class.>/param>
        public ConverterForm(MainForm mainForm, string[] args)
        {
            InitializeComponent();

            // FFMpeg command arguments from MainForm.class
            _arguments = args;
            _owner = mainForm;
        }

        /// <summary>
        /// Handles the video conversion on screen
        /// </summary>
        private void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs args)
        {
            if (args.Data != null)
            {
                if (args.Data.Contains("Duration"))
                {
                    duration = TimeSpan.Parse(args.Data.Substring(11, 11)).TotalSeconds;
                }
            }

            if (args.Data != null)
            {
                //!string.IsNullOrWhiteSpace(_owner.boxCropTo.Text)
                //textBoxOutput.Invoke((Action)(() => textBoxOutput.AppendText("\n" + args.Data)));

                // if there's no start or end time, convert the entire video
                if (string.IsNullOrWhiteSpace(_owner.boxCropTo.Text) && string.IsNullOrWhiteSpace(_owner.boxCropFrom.Text))
                {
                    if (args.Data.Contains("frame"))
                    {
                        frameNum = Regex.Match(args.Data, @"\d+").Value;
                        timeLeft = (Double.Parse(frameNum) / (duration * 25)) * 100;
                        if (timeLeftTemp < timeLeft)
                        {
                            timeLeftTemp = timeLeft;
                            if (wholenum != (int)timeLeft)
                            {
                                this.Invoke(new Action(() => this.progressBar1.Value = wholenum));
                                wholenum = (int)timeLeft;
                            }
                            textBoxOutput.Invoke((Action)(() => textBoxOutput.Text = ("\n" + String.Format("{0:0}", timeLeft) + "%")));
                        }
                        else
                            textBoxOutput.Invoke((Action)(() => textBoxOutput.Text = ("\nFinalising video...")));
                        
                    }
                }

                // if there's no end time, convert until the end of the video
                else if (string.IsNullOrWhiteSpace(_owner.boxCropTo.Text) && !string.IsNullOrWhiteSpace(_owner.boxCropFrom.Text))
                {
                    trim = duration - TimeSpan.Parse(_owner.boxCropFrom.Text).TotalSeconds;
                    trim = Math.Abs(trim);
                    if (args.Data.Contains("frame"))
                    {
                        frameNum = Regex.Match(args.Data, @"\d+").Value;
                        timeLeft = (Double.Parse(frameNum) / (trim * 25)) * 100;
                        if (timeLeftTemp < timeLeft)
                        {
                            timeLeftTemp = timeLeft;
                            if (wholenum != (int)timeLeft)
                            {
                                this.Invoke(new Action(() => this.progressBar1.Value = wholenum));
                                wholenum = (int)timeLeft;
                            }
                            textBoxOutput.Invoke((Action)(() => textBoxOutput.Text = ("\n" + String.Format("{0:0}", timeLeft) + "%")));
                        }
                        else
                            textBoxOutput.Invoke((Action)(() => textBoxOutput.Text = ("\nFinalising video...")));
                    }
                }

                // if there's no start time, convert from the beginning of the video
                else if (!string.IsNullOrWhiteSpace(_owner.boxCropTo.Text) && string.IsNullOrWhiteSpace(_owner.boxCropFrom.Text))
                {
                    trim = TimeSpan.Parse(_owner.boxCropTo.Text).TotalSeconds;
                    trim = Math.Abs(trim);
                    if (args.Data.Contains("frame"))
                    {
                        frameNum = Regex.Match(args.Data, @"\d+").Value;
                        timeLeft = (Double.Parse(frameNum) / (trim * 25)) * 100;
                        if (timeLeftTemp < timeLeft)
                        {
                            timeLeftTemp = timeLeft;
                            if (wholenum != (int)timeLeft)
                            {
                                this.Invoke(new Action(() => this.progressBar1.Value = wholenum));
                                wholenum = (int)timeLeft;
                            }
                            textBoxOutput.Invoke((Action)(() => textBoxOutput.Text = ("\n" + String.Format("{0:0}", timeLeft) + "%")));
                        }
                        else
                            textBoxOutput.Invoke((Action)(() => textBoxOutput.Text = ("\nFinalising video...")));
                    }
                }

                // if there's both a start and end time
                else
                {
                    trim = (TimeSpan.Parse(_owner.boxCropTo.Text) - TimeSpan.Parse(_owner.boxCropFrom.Text)).TotalSeconds;
                    trim = Math.Abs(trim);
                    if (args.Data.Contains("frame"))
                    {
                        frameNum = Regex.Match(args.Data, @"\d+").Value;
                        timeLeft = (Double.Parse(frameNum) / (trim * 25)) * 100;
                        if (timeLeftTemp < timeLeft)
                        {
                            timeLeftTemp = timeLeft;
                            if (wholenum != (int)timeLeft)
                            {  
                                this.Invoke(new Action(() => this.progressBar1.Value = wholenum));
                                wholenum = (int)timeLeft;
                            }
                            textBoxOutput.Invoke((Action)(() => textBoxOutput.Text = ("\n" + String.Format("{0:0}", timeLeft) + "%")));
                        }
                        else
                        {
                            textBoxOutput.Invoke((Action)(() => textBoxOutput.Text = ("\nFinalising video...")));
                            //progressBar1.Increment(100);
                        }                   
                    }
                }
            }
        }

        /// <summary>
        /// Initiates the conversion process
        /// </summary>
        private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs args)
        {
            //if (args.Data != null)
                //textBoxOutput.Invoke((Action)(() => textBoxOutput.AppendText("\n" + args.Data)));
        }

        private void ConverterForm_Load(object sender, EventArgs e)
        {
            textBoxOutput.Text = ("Conversion has begun. Please wait...");

            string argument = null;
           _multipass = true;
            if (_arguments.Length == 1)
            {
                _multipass = false;
                argument = _arguments[0];
            }

            if (_multipass)
                MultiPass(_arguments);
            else
                SinglePass(argument);
        }

        private void SinglePass(string argument)
        {
            _ffmpegProcess = new FFmpeg(argument);

            _ffmpegProcess.Process.ErrorDataReceived += ProcessOnErrorDataReceived;
            _ffmpegProcess.Process.OutputDataReceived += ProcessOnOutputDataReceived;
            _ffmpegProcess.Process.Exited += (o, args) => textBoxOutput.Invoke((Action)(() =>
                                                                              {
                                                                                  if (_panic) return;
                                                                                  buttonCancel.Enabled = false;

                                                                                  _timer = new Timer();
                                                                                  _timer.Interval = 500;
                                                                                  _timer.Tick += Exited;
                                                                                  _timer.Start();
                                                                              }));

            _ffmpegProcess.Start();
        }

        private void MultiPass(string[] arguments)
        {
            int passes = arguments.Length;

            _ffmpegProcess = new FFmpeg(arguments[currentPass]);

            _ffmpegProcess.Process.ErrorDataReceived += ProcessOnErrorDataReceived;
            _ffmpegProcess.Process.OutputDataReceived += ProcessOnOutputDataReceived;
            _ffmpegProcess.Process.Exited += (o, args) => textBoxOutput.Invoke((Action)(() =>
            {
                if (_panic) return;
                textBoxOutput.AppendText("\n--- Back-end done converting video ---");

                currentPass++;
                if (currentPass < passes && !_cancelMultipass)
                {
                    MultiPass(arguments);
                    return;
                }

                buttonCancel.Enabled = false;

                _timer = new Timer();
                _timer.Interval = 500;
                _timer.Tick += Exited;
                _timer.Start();
            }));

            _ffmpegProcess.Start();
        }

        /// <summary>
        /// Handles what happens at the end of a video conversion; successful or not.
        /// </summary>
        private void Exited(object sender, EventArgs eventArgs)
        {
            _timer.Stop();

            var process = _ffmpegProcess.Process;

            if (process.ExitCode != 0)
            {
                if (_cancelMultipass)
                    textBoxOutput.AppendText("\n\nConversion cancelled.");
                else
                {
                    textBoxOutput.AppendText(string.Format("\n\nOops. An error occurred with the code {0}.", process.ExitCode));
                    textBoxOutput.AppendText("\nPlease check that your: \n(1) Subtitle file is correct \n(2)Your video is working"
                    + " outside the application \n(3) Your subtitles are not longer than the video.");
                }
                pictureBox.BackgroundImage = Properties.Resources.cross;

                if (process.ExitCode == -1073741819)
                    MessageBox.Show("FFmpeg crashed because of a thread error. Please try again.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                progressBar1.Visible = false;
                textBoxOutput.AppendText("\n\nVideo is ready.");
                pictureBox.BackgroundImage = Properties.Resources.tick;

                buttonPlay.Enabled = true;
                //_owner.MkvMerge();
                //_owner.manageFiles();
            }

            buttonCancel.Text = "Close";
            buttonCancel.Enabled = true;
            _ended = true;
        }

        /// <summary>
        /// Stops conversion if the user cancels
        /// </summary>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            _cancelMultipass = true;

            // prevents stack overflow
            if (!_ended || _panic)
            {
                if (!_ffmpegProcess.Process.HasExited)
                    _ffmpegProcess.Process.Kill();
            }
            else
                Close();
        }

        private void ConverterForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // shuts down to avoid exceptions
            _panic = true;
            buttonCancel_Click(sender, e);
        }

        /// <summary>
        /// Disposes the ffmpeg process when this form is closed
        /// </summary>
        private void ConverterForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _ffmpegProcess.Process.Dispose();
        }

        /// <summary>
        /// Plays the video if the user chooses to do so after conversion
        /// </summary>
        private void buttonPlay_Click(object sender, EventArgs e)
        {
            // plays result video
            Process.Start(_owner.textBoxOut.Text);
        }
    }
}
