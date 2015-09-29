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
        private string frameNum;

        public ConverterForm(MainForm mainForm, string[] args)
        {
            InitializeComponent();

            _arguments = args;
            _owner = mainForm;
        }

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
                if (string.IsNullOrWhiteSpace(_owner.boxCropTo.Text) && string.IsNullOrWhiteSpace(_owner.boxCropFrom.Text))
                {
                    if (args.Data.Contains("frame"))
                    {
                        frameNum = Regex.Match(args.Data, @"\d+").Value;
                        timeLeft = (Double.Parse(frameNum) / (duration * 25)) * 100;
                        if (timeLeft <= 100)
                            textBoxOutput.Invoke((Action)(() => textBoxOutput.Text = ("\n" + String.Format("{0:0}", timeLeft) + "%")));
                        else
                            textBoxOutput.Invoke((Action)(() => textBoxOutput.Text = ("\nFinalising video...")));
                    }
                }

                else if (string.IsNullOrWhiteSpace(_owner.boxCropTo.Text) && !string.IsNullOrWhiteSpace(_owner.boxCropFrom.Text))
                {
                    trim = duration - TimeSpan.Parse(_owner.boxCropFrom.Text).TotalSeconds;
                    trim = Math.Abs(trim);
                    if (args.Data.Contains("frame"))
                    {
                        frameNum = Regex.Match(args.Data, @"\d+").Value;
                        timeLeft = (Double.Parse(frameNum) / (trim * 25)) * 100;
                        if (timeLeft <= 100)
                            textBoxOutput.Invoke((Action)(() => textBoxOutput.Text = ("\n" + String.Format("{0:0}", timeLeft) + "%")));
                        else
                            textBoxOutput.Invoke((Action)(() => textBoxOutput.Text = ("\nFinalising video...")));
                    }
                }

                else if (!string.IsNullOrWhiteSpace(_owner.boxCropTo.Text) && string.IsNullOrWhiteSpace(_owner.boxCropFrom.Text))
                {
                    trim = TimeSpan.Parse(_owner.boxCropTo.Text).TotalSeconds;
                    trim = Math.Abs(trim);
                    if (args.Data.Contains("frame"))
                    {
                        frameNum = Regex.Match(args.Data, @"\d+").Value;
                        timeLeft = (Double.Parse(frameNum) / (trim * 25)) * 100;
                        if (timeLeft <= 100)
                            textBoxOutput.Invoke((Action)(() => textBoxOutput.Text = ("\n" + String.Format("{0:0}", timeLeft) + "%")));
                        else
                            textBoxOutput.Invoke((Action)(() => textBoxOutput.Text = ("\nFinalising video...")));
                    }
                }

                else
                {
                    trim = (TimeSpan.Parse(_owner.boxCropTo.Text) - TimeSpan.Parse(_owner.boxCropFrom.Text)).TotalSeconds;
                    trim = Math.Abs(trim);
                    if (args.Data.Contains("frame"))
                    {
                        frameNum = Regex.Match(args.Data, @"\d+").Value;
                        timeLeft = (Double.Parse(frameNum) / (trim * 25)) * 100;
                        if (timeLeft <= 100)
                            textBoxOutput.Invoke((Action)(() => textBoxOutput.Text = ("\n" + String.Format("{0:0}", timeLeft) + "%")));
                        else
                            textBoxOutput.Invoke((Action)(() => textBoxOutput.Text = ("\nFinalising video...")));
                    }
                }
            }
        }

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

            /*if (_multipass)
                for (int i = 0; i < _arguments.Length; i++)
                    textBoxOutput.AppendText(string.Format("\nArguments for pass {0}: {1}", i + 1, _arguments[i]));
            else
                textBoxOutput.AppendText("\nArguments: " + argument);*/

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
                                                                                  if (_panic) return; //This should stop that one exception when closing the converter
                                                                                  //textBoxOutput.Text = ("\n--- The video conversion is done ---");
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
                if (_panic) return; //This should stop that one exception when closing the converter
                textBoxOutput.AppendText("\n--- Back-end done converting video ---");

                currentPass++;
                if (currentPass < passes && !_cancelMultipass)
                {
                    //textBoxOutput.AppendText(string.Format("\n--- ENTERING PASS {0} ---", currentPass + 1));

                    MultiPass(arguments); //Sort of recursion going on here, be careful with stack overflows and shit
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
                    textBoxOutput.AppendText("\nPlease check that your: \n(1) Subtitle file is correct \nYour video is working"
                    + " outside the application \n(3) Your subtitles are not longer than the video.");
                }
                pictureBox.BackgroundImage = Properties.Resources.cross;

                if (process.ExitCode == -1073741819) //This error keeps happening for me if I set threads to anything above 1, might happen for other people too
                    MessageBox.Show("FFmpeg crashed because of a thread error. Please try again.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
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

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            _cancelMultipass = true;

            if (!_ended || _panic) //Prevent stack overflow
            {
                if (!_ffmpegProcess.Process.HasExited)
                    _ffmpegProcess.Process.Kill();
            }
            else
                Close();
        }

        private void ConverterForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _panic = true; //Shut down while avoiding exceptions
            buttonCancel_Click(sender, e);
        }

        private void ConverterForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _ffmpegProcess.Process.Dispose();
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            Process.Start(_owner.textBoxOut.Text); //Play result video
        }
    }
}
