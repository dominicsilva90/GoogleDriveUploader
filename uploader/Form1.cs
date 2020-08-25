using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using HeyRed.Mime;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
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
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string pageToken = null;
            do
            {
                var response = GoogleDriveManager.UploadFile(textBox1.Text, "TEST");
                resultLabel.ForeColor = Color.ForestGreen;
                resultLabel.Text = response.Name + " uploaded.";
            }
            while (pageToken != null);
            textBox1.Clear();
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                button2.Enabled = false;
            }
            else
            {
                button2.Enabled = true;
            }
        }
    }
}
