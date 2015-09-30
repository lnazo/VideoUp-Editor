namespace VideoUp
{
    partial class SubtitleForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubtitleForm));
            this.infoBox = new System.Windows.Forms.TextBox();
            this.endSubMsg = new System.Windows.Forms.Label();
            this.startSubTime = new System.Windows.Forms.Button();
            this.startSubMsg = new System.Windows.Forms.Label();
            this.endSubTime = new System.Windows.Forms.Button();
            this.endSubBox = new System.Windows.Forms.TextBox();
            this.startSubBox = new System.Windows.Forms.TextBox();
            this.saveSub = new System.Windows.Forms.Button();
            this.enterSubtitle = new System.Windows.Forms.Button();
            this.axWindowsMediaPlayer2 = new AxWMPLib.AxWindowsMediaPlayer();
            this.subtitleGridView = new System.Windows.Forms.DataGridView();
            this.num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.start = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.end = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.text = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.subtitleMsg = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.subtitleGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // infoBox
            // 
            this.infoBox.Location = new System.Drawing.Point(424, 90);
            this.infoBox.Multiline = true;
            this.infoBox.Name = "infoBox";
            this.infoBox.Size = new System.Drawing.Size(453, 107);
            this.infoBox.TabIndex = 43;
            // 
            // endSubMsg
            // 
            this.endSubMsg.AutoSize = true;
            this.endSubMsg.BackColor = System.Drawing.Color.Red;
            this.endSubMsg.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.endSubMsg.Location = new System.Drawing.Point(685, 64);
            this.endSubMsg.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.endSubMsg.Name = "endSubMsg";
            this.endSubMsg.Size = new System.Drawing.Size(193, 13);
            this.endSubMsg.TabIndex = 49;
            this.endSubMsg.Text = "End time must be greater than start time";
            this.endSubMsg.Visible = false;
            // 
            // startSubTime
            // 
            this.startSubTime.Enabled = false;
            this.startSubTime.Location = new System.Drawing.Point(424, 13);
            this.startSubTime.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.startSubTime.Name = "startSubTime";
            this.startSubTime.Size = new System.Drawing.Size(112, 30);
            this.startSubTime.TabIndex = 44;
            this.startSubTime.Text = "Set Start Time";
            this.startSubTime.UseVisualStyleBackColor = true;
            this.startSubTime.Click += new System.EventHandler(this.startSubTime_Click);
            // 
            // startSubMsg
            // 
            this.startSubMsg.AutoSize = true;
            this.startSubMsg.BackColor = System.Drawing.Color.Red;
            this.startSubMsg.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.startSubMsg.Location = new System.Drawing.Point(685, 25);
            this.startSubMsg.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.startSubMsg.Name = "startSubMsg";
            this.startSubMsg.Size = new System.Drawing.Size(179, 13);
            this.startSubMsg.TabIndex = 48;
            this.startSubMsg.Text = "Start time must be less than end time";
            this.startSubMsg.Visible = false;
            // 
            // endSubTime
            // 
            this.endSubTime.Enabled = false;
            this.endSubTime.Location = new System.Drawing.Point(424, 53);
            this.endSubTime.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.endSubTime.Name = "endSubTime";
            this.endSubTime.Size = new System.Drawing.Size(112, 30);
            this.endSubTime.TabIndex = 45;
            this.endSubTime.Text = "Set End Time";
            this.endSubTime.UseVisualStyleBackColor = true;
            this.endSubTime.Click += new System.EventHandler(this.endSubTime_Click);
            // 
            // endSubBox
            // 
            this.endSubBox.Enabled = false;
            this.endSubBox.Location = new System.Drawing.Point(556, 57);
            this.endSubBox.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.endSubBox.Name = "endSubBox";
            this.endSubBox.Size = new System.Drawing.Size(116, 20);
            this.endSubBox.TabIndex = 47;
            // 
            // startSubBox
            // 
            this.startSubBox.Enabled = false;
            this.startSubBox.Location = new System.Drawing.Point(556, 17);
            this.startSubBox.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.startSubBox.Name = "startSubBox";
            this.startSubBox.Size = new System.Drawing.Size(116, 20);
            this.startSubBox.TabIndex = 46;
            // 
            // saveSub
            // 
            this.saveSub.Location = new System.Drawing.Point(12, 521);
            this.saveSub.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.saveSub.Name = "saveSub";
            this.saveSub.Size = new System.Drawing.Size(112, 30);
            this.saveSub.TabIndex = 50;
            this.saveSub.Text = "Save Subtitle";
            this.saveSub.UseVisualStyleBackColor = true;
            this.saveSub.Click += new System.EventHandler(this.saveSub_Click);
            // 
            // enterSubtitle
            // 
            this.enterSubtitle.Location = new System.Drawing.Point(583, 204);
            this.enterSubtitle.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.enterSubtitle.Name = "enterSubtitle";
            this.enterSubtitle.Size = new System.Drawing.Size(112, 30);
            this.enterSubtitle.TabIndex = 51;
            this.enterSubtitle.Text = "Enter Subtitle";
            this.enterSubtitle.UseVisualStyleBackColor = true;
            this.enterSubtitle.Click += new System.EventHandler(this.enterSubtitle_Click);
            // 
            // axWindowsMediaPlayer2
            // 
            this.axWindowsMediaPlayer2.AllowDrop = true;
            this.axWindowsMediaPlayer2.Enabled = true;
            this.axWindowsMediaPlayer2.Location = new System.Drawing.Point(12, 13);
            this.axWindowsMediaPlayer2.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.axWindowsMediaPlayer2.Name = "axWindowsMediaPlayer2";
            this.axWindowsMediaPlayer2.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWindowsMediaPlayer2.OcxState")));
            this.axWindowsMediaPlayer2.Size = new System.Drawing.Size(394, 249);
            this.axWindowsMediaPlayer2.TabIndex = 42;
            // 
            // subtitleGridView
            // 
            this.subtitleGridView.AllowUserToAddRows = false;
            this.subtitleGridView.AllowUserToResizeColumns = false;
            this.subtitleGridView.AllowUserToResizeRows = false;
            this.subtitleGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.subtitleGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.subtitleGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.num,
            this.start,
            this.end,
            this.text});
            this.subtitleGridView.Location = new System.Drawing.Point(12, 279);
            this.subtitleGridView.Name = "subtitleGridView";
            this.subtitleGridView.Size = new System.Drawing.Size(865, 235);
            this.subtitleGridView.TabIndex = 52;
            this.subtitleGridView.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.subtitleGridView_RowsRemoved);
            // 
            // num
            // 
            this.num.FillWeight = 33.57664F;
            this.num.HeaderText = "Number";
            this.num.Name = "num";
            this.num.ReadOnly = true;
            // 
            // start
            // 
            this.start.FillWeight = 96.39494F;
            this.start.HeaderText = "Start time";
            this.start.Name = "start";
            this.start.ReadOnly = true;
            // 
            // end
            // 
            this.end.FillWeight = 111.7462F;
            this.end.HeaderText = "End time";
            this.end.Name = "end";
            this.end.ReadOnly = true;
            // 
            // text
            // 
            this.text.FillWeight = 158.2823F;
            this.text.HeaderText = "Text";
            this.text.Name = "text";
            // 
            // subtitleMsg
            // 
            this.subtitleMsg.AutoSize = true;
            this.subtitleMsg.BackColor = System.Drawing.Color.Red;
            this.subtitleMsg.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.subtitleMsg.Location = new System.Drawing.Point(511, 249);
            this.subtitleMsg.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.subtitleMsg.Name = "subtitleMsg";
            this.subtitleMsg.Size = new System.Drawing.Size(250, 13);
            this.subtitleMsg.TabIndex = 53;
            this.subtitleMsg.Text = "Check that start time, end time or subtitle not empty.";
            this.subtitleMsg.Visible = false;
            // 
            // SubtitleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(889, 563);
            this.Controls.Add(this.subtitleMsg);
            this.Controls.Add(this.subtitleGridView);
            this.Controls.Add(this.enterSubtitle);
            this.Controls.Add(this.saveSub);
            this.Controls.Add(this.endSubMsg);
            this.Controls.Add(this.startSubTime);
            this.Controls.Add(this.startSubMsg);
            this.Controls.Add(this.endSubTime);
            this.Controls.Add(this.endSubBox);
            this.Controls.Add(this.startSubBox);
            this.Controls.Add(this.infoBox);
            this.Controls.Add(this.axWindowsMediaPlayer2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SubtitleForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create Subtitles";
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.subtitleGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer2;
        private System.Windows.Forms.TextBox infoBox;
        private System.Windows.Forms.Label endSubMsg;
        private System.Windows.Forms.Button startSubTime;
        private System.Windows.Forms.Label startSubMsg;
        private System.Windows.Forms.Button endSubTime;
        private System.Windows.Forms.TextBox endSubBox;
        private System.Windows.Forms.TextBox startSubBox;
        private System.Windows.Forms.Button saveSub;
        private System.Windows.Forms.Button enterSubtitle;
        private System.Windows.Forms.DataGridView subtitleGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn num;
        private System.Windows.Forms.DataGridViewTextBoxColumn start;
        private System.Windows.Forms.DataGridViewTextBoxColumn end;
        private System.Windows.Forms.DataGridViewTextBoxColumn text;
        private System.Windows.Forms.Label subtitleMsg;
    }
}