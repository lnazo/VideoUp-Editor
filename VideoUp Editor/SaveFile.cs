using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoUp
{
    class SaveFile
    {
        /// <summary>
        /// Opens DropIt from the navigation bar
        /// </summary>
        /*private void saveToolStripMenuItem_Click(object sender, EventArgs e)
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
        }*/
    }
}
