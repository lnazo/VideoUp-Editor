using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VideoUp
{
    class FFmpeg
    {
        public string FFmpegPath = Path.Combine(Environment.CurrentDirectory, "ffmpeg/ffmpeg.exe");
        public Process Process;

        /// <summary>
        /// Fetches and uses the FFmpeg command constructed in the MainForm.cs class
        /// </summary>
        /// <param name="argument">the object that contains the FFmpeg command.>/param>
        public FFmpeg(string argument)
        {
            Process = new Process();

            ProcessStartInfo info = new ProcessStartInfo(FFmpegPath);
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.UseShellExecute = false; // Required to redirect IO streams
            info.CreateNoWindow = true; // Hide console
            info.Arguments = argument;

            Process.StartInfo = info;
            Process.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Starts the process
        /// </summary>
        public void Start()
        {
            Process.Start();
            Process.BeginErrorReadLine();
            Process.BeginOutputReadLine();
        }
    }
}
