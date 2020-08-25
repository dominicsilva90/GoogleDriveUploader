using System;
using System.Drawing;
using System.Windows.Forms;

namespace uploader
{
    public partial class Form1 : Form
    {

        public GoogleDriveManager GoogleDriveManager = new GoogleDriveManager();
        public Form1()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK) // Test result.
            {
                filePathTxt.Text = openFileDialog1.FileName;
                resultLabel.ForeColor = Color.Black;
                resultLabel.Text = "File selected";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string pageToken = null;
            do
            {
                resultLabel.Text = "Uploading...";
                browseBtn.Enabled = false;
                var response = GoogleDriveManager.UploadFile(filePathTxt.Text, "Uploaded from EZUploader");
                resultLabel.ForeColor = Color.ForestGreen;
                resultLabel.Text = response.Name + " uploaded.";
            }
            while (pageToken != null);
            filePathTxt.Clear();
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(filePathTxt.Text))
            {
                uploadBtn.Enabled = false;
                browseBtn.Enabled = true;
            }
            else
            {
                uploadBtn.Enabled = true;
            }
        }
    }
}
