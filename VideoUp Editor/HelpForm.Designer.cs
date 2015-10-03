namespace VideoUp
{
    partial class HelpForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HelpForm));
            this.helpMediaPlayer = new AxWMPLib.AxWindowsMediaPlayer();
            this.uploadButton = new System.Windows.Forms.Button();
            this.infoButton = new System.Windows.Forms.Button();
            this.subtitleButton = new System.Windows.Forms.Button();
            this.trimButton = new System.Windows.Forms.Button();
            this.openSaveButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.helpMediaPlayer)).BeginInit();
            this.SuspendLayout();
            // 
            // helpMediaPlayer
            // 
            this.helpMediaPlayer.Enabled = true;
            this.helpMediaPlayer.Location = new System.Drawing.Point(214, 22);
            this.helpMediaPlayer.Name = "helpMediaPlayer";
            this.helpMediaPlayer.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("helpMediaPlayer.OcxState")));
            this.helpMediaPlayer.Size = new System.Drawing.Size(474, 309);
            this.helpMediaPlayer.TabIndex = 19;
            // 
            // uploadButton
            // 
            this.uploadButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.uploadButton.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.uploadButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.uploadButton.Image = global::VideoUp.Properties.Resources.upload_help;
            this.uploadButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uploadButton.Location = new System.Drawing.Point(29, 289);
            this.uploadButton.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.uploadButton.Name = "uploadButton";
            this.uploadButton.Size = new System.Drawing.Size(147, 42);
            this.uploadButton.TabIndex = 18;
            this.uploadButton.Text = "Video Upload";
            this.uploadButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.uploadButton.UseVisualStyleBackColor = true;
            this.uploadButton.Click += new System.EventHandler(this.uploadButton_Click);
            // 
            // infoButton
            // 
            this.infoButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.infoButton.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.infoButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.infoButton.Image = global::VideoUp.Properties.Resources.info_help;
            this.infoButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.infoButton.Location = new System.Drawing.Point(29, 223);
            this.infoButton.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.infoButton.Name = "infoButton";
            this.infoButton.Size = new System.Drawing.Size(147, 42);
            this.infoButton.TabIndex = 17;
            this.infoButton.Text = "Video Info";
            this.infoButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.infoButton.UseVisualStyleBackColor = true;
            this.infoButton.Click += new System.EventHandler(this.infoButton_Click);
            // 
            // subtitleButton
            // 
            this.subtitleButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.subtitleButton.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.subtitleButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.subtitleButton.Image = global::VideoUp.Properties.Resources.subtitle_help;
            this.subtitleButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.subtitleButton.Location = new System.Drawing.Point(29, 153);
            this.subtitleButton.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.subtitleButton.Name = "subtitleButton";
            this.subtitleButton.Size = new System.Drawing.Size(147, 42);
            this.subtitleButton.TabIndex = 16;
            this.subtitleButton.Text = "Subtitles";
            this.subtitleButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.subtitleButton.UseVisualStyleBackColor = true;
            this.subtitleButton.Click += new System.EventHandler(this.subtitleButton_Click);
            // 
            // trimButton
            // 
            this.trimButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.trimButton.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.trimButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.trimButton.Image = global::VideoUp.Properties.Resources.trim_help;
            this.trimButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.trimButton.Location = new System.Drawing.Point(29, 86);
            this.trimButton.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.trimButton.Name = "trimButton";
            this.trimButton.Size = new System.Drawing.Size(147, 42);
            this.trimButton.TabIndex = 15;
            this.trimButton.Text = "Trim";
            this.trimButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.trimButton.UseVisualStyleBackColor = true;
            this.trimButton.Click += new System.EventHandler(this.trimButton_Click);
            // 
            // openSaveButton
            // 
            this.openSaveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.openSaveButton.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.openSaveButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.openSaveButton.Image = global::VideoUp.Properties.Resources.opensave_help;
            this.openSaveButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.openSaveButton.Location = new System.Drawing.Point(29, 22);
            this.openSaveButton.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.openSaveButton.Name = "openSaveButton";
            this.openSaveButton.Size = new System.Drawing.Size(147, 42);
            this.openSaveButton.TabIndex = 14;
            this.openSaveButton.Text = "Open and Save";
            this.openSaveButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.openSaveButton.UseVisualStyleBackColor = true;
            this.openSaveButton.Click += new System.EventHandler(this.openSaveButton_Click);
            // 
            // HelpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(44)))), ((int)(((byte)(63)))));
            this.ClientSize = new System.Drawing.Size(721, 364);
            this.Controls.Add(this.helpMediaPlayer);
            this.Controls.Add(this.uploadButton);
            this.Controls.Add(this.infoButton);
            this.Controls.Add(this.subtitleButton);
            this.Controls.Add(this.trimButton);
            this.Controls.Add(this.openSaveButton);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "HelpForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Help";
            ((System.ComponentModel.ISupportInitialize)(this.helpMediaPlayer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button openSaveButton;
        private System.Windows.Forms.Button trimButton;
        private System.Windows.Forms.Button subtitleButton;
        private System.Windows.Forms.Button infoButton;
        private System.Windows.Forms.Button uploadButton;
        private AxWMPLib.AxWindowsMediaPlayer helpMediaPlayer;
    }
}