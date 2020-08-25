using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using HeyRed.Mime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace uploader
{
    public partial class Form1 : Form
    {
        static string[] Scopes = { DriveService.Scope.Drive };
        static string ApplicationName = "Quick Uploader";
        public DriveService service { get; set; }
        public Form1()
        {
            InitializeComponent();
            service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GetCredentials(),
                ApplicationName = ApplicationName,
            });
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
                var response = UploadFile(service, textBox1.Text, "TEST");
                resultLabel.ForeColor = Color.ForestGreen;
                resultLabel.Text = response.Name + " uploaded.";
            }
            while (pageToken != null);
            textBox1.Clear();
        }
        private UserCredential GetCredentials()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }
            return credential;
        }
        private Google.Apis.Drive.v3.Data.File UploadFile(DriveService service, string uploadFile, string description)
        {
            if (File.Exists(uploadFile))
            {
                Google.Apis.Drive.v3.Data.File body = new Google.Apis.Drive.v3.Data.File();
                string mimeType = GetMimeType(uploadFile);
                body.Name = Path.GetFileName(uploadFile);
                body.Description = description;
                body.MimeType = mimeType;
                byte[] byteArray = System.IO.File.ReadAllBytes(uploadFile);
                MemoryStream stream = new MemoryStream(byteArray);
                try
                {
                    FilesResource.CreateMediaUpload request = service.Files.Create(body, stream, mimeType);
                    request.SupportsTeamDrives = true;
                    request.Upload();
                    return request.ResponseBody;

                }
                catch (Exception e)
                {
                    resultLabel.Text = e.Message;
                    return new Google.Apis.Drive.v3.Data.File();
                }
            }
            else
            {
                resultLabel.Text = "File does not exist";
                return new Google.Apis.Drive.v3.Data.File();
            }

        }

        private string GetMimeType(string fileName)
        {
            return MimeGuesser.GuessMimeType(fileName);
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

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
