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
        public HelpForm(MainForm mainFormm, string video)
        {
            InitializeComponent();
            if (video != null)
                helpMediaPlayer.URL = video;
        }

        private void openSaveButton_Click(object sender, EventArgs e)
        {
            helpMediaPlayer.URL = @"Help\\Open Save video.mp4";
        }

        private void trimButton_Click(object sender, EventArgs e)
        {
            helpMediaPlayer.URL = @"Help\\Trim video.mp4";
        }

        private void subtitleButton_Click(object sender, EventArgs e)
        {
            helpMediaPlayer.URL = @"Help\\Subtitle video.mp4";
        }

        private void infoButton_Click(object sender, EventArgs e)
        {
            helpMediaPlayer.URL = @"Help\\info video.mp4";
        }

        private void uploadButton_Click(object sender, EventArgs e)
        {
            helpMediaPlayer.URL = @"Help\\Upload video.mp4";
        }
    }
}
