using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace AramisIDE.Utils.FileUploader
    {
    class HttpFileUploader
        {
        private string url;

        public HttpFileUploader(string url)
            {
            this.url = url;
            }

        public void UploadFile(string fullFileName)
            {
            try
                {
                var client = new WebClient();
                client.Credentials = CredentialCache.DefaultCredentials;
                byte[] response = null;
                //response = client.UploadData(url, new byte[] { 65, 65, 65 });
                response = client.UploadFile(url, "PUT", fullFileName);
                Trace.WriteLine(response);

                client.Dispose();
                }
            catch (Exception err)
                {
                MessageBox.Show(err.Message);
                }
            }
        }
    }
