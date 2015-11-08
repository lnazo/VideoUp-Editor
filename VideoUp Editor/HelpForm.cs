using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VideoUp
{
    public partial class HelpForm : Form
    {
        /// <summary>
        /// Fetches the information about a video file from the MainForm.cs class
        /// </summary>
        /// <param name="mainForm">the object that opens this class.>/param>
        /// <param name="video">the help video file selected by the user.>/param>
        public HelpForm(MainForm mainFormm, string video)
        {
            InitializeComponent();
            if (video != null)
                helpMediaPlayer.URL = video;
        }

        /// <summary>
        /// Help video with regards to opening and saving a video
        /// </summary>
        private void openSaveButton_Click(object sender, EventArgs e)
        {
            helpMediaPlayer.URL = @"Help\\Open Save video.mp4";
        }

        /// <summary>
        /// Help video with regards to trimming a video
        /// </summary>
        private void trimButton_Click(object sender, EventArgs e)
        {
            helpMediaPlayer.URL = @"Help\\Trim video.mp4";
        }

        /// <summary>
        /// Help video with regards to adding a subtitle to a video
        /// </summary>
        private void subtitleButton_Click(object sender, EventArgs e)
        {
            helpMediaPlayer.URL = @"Help\\Subtitle video.mp4";
        }

        /// <summary>
        /// Help video with regards to adding information to a video
        /// </summary>
        private void infoButton_Click(object sender, EventArgs e)
        {
            helpMediaPlayer.URL = @"Help\\info video.mp4";
        }

        /// <summary>
        /// Help video with regards to uploading a video
        /// </summary>
        private void uploadButton_Click(object sender, EventArgs e)
        {
            helpMediaPlayer.URL = @"Help\\Upload video.mp4";
        }
    }
}
