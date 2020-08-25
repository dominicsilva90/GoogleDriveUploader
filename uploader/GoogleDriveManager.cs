using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using HeyRed.Mime;
using System.IO;
using System.Threading;

namespace uploader
{
    public class GoogleDriveManager
    {
        private string[] Scopes = { DriveService.Scope.Drive };
        private string ApplicationName = "Quick Uploader";
        public DriveService Service { get; set; }
        public GoogleDriveManager()
        {
            Service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GetCredentials(),
                ApplicationName = ApplicationName,
            });
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
        public Google.Apis.Drive.v3.Data.File UploadFile(string uploadFilePath, string description)
        {
            if (File.Exists(uploadFilePath))
            {
                Google.Apis.Drive.v3.Data.File body = new Google.Apis.Drive.v3.Data.File();
                string mimeType = GetMimeType(uploadFilePath);
                body.Name = Path.GetFileName(uploadFilePath);
                body.Description = description;
                body.MimeType = mimeType;
                byte[] byteArray = System.IO.File.ReadAllBytes(uploadFilePath);
                MemoryStream stream = new MemoryStream(byteArray);
                try
                {
                    FilesResource.CreateMediaUpload request = Service.Files.Create(body, stream, mimeType);
                    request.SupportsTeamDrives = true;
                    request.Upload();
                    return request.ResponseBody;

                }
                catch
                {
                    return new Google.Apis.Drive.v3.Data.File();
                }
            }
            else
            {
                return new Google.Apis.Drive.v3.Data.File();
            }

        }

        private string GetMimeType(string filePath)
        {
            return MimeGuesser.GuessMimeType(filePath);
        }


    }
}
