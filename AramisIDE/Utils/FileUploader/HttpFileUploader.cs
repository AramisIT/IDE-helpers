using System;
using System.Net;
using System.Text;

namespace AramisIDE.Utils.FileUploader
    {
    class HttpFileUploader
        {
        private string userName;
        private string password;

        public HttpFileUploader(string userName, string password)
            {
            this.userName = userName;
            this.password = password;
            }

        public string Url { get; set; }

        public string UploadFile(string fullFileName)
            {
            try
                {
                var client = new WebClient();

                //client.Credentials = new NetworkCredential(userName, password);
                client.Credentials = CredentialCache.DefaultCredentials;

                byte[] response = null;
                //response = client.UploadData(url, new byte[] { 65, 65, 65 });
                response = client.UploadFile(Url, "POST", fullFileName);

                var reply = System.Text.Encoding.UTF8.GetString(response);
                client.Dispose();
                return reply;
                }
            catch (Exception err)
                {
                UpdateLogger.Instance.Append("An exception has been throwed!");
                return err.Message;
                }
            }
        }
    }
