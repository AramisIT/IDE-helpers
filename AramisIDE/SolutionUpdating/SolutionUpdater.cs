using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using AramisIDE.Models;

namespace AramisIDE.SolutionUpdating
    {
    public class SolutionUpdater
        {
        private SolutionDetails solutionDetails;
        private Guid id;
        private Thread updateThread;

        public SolutionUpdater(SolutionDetails solutionDetails)
            {
            id = Guid.NewGuid();
            this.solutionDetails = solutionDetails;
            }

        public void Update()
            {
            if (startWebApplication())
                {
                startUpdating();
                }
            }

        private bool startWebApplication()
            {
            var url = string.Format(@"{0}/sa/StartUpdate?id={1}",
             solutionDetails.UpdateUrl.TrimEnd(new[] { '/' }),
             id);
            try
                {
                Process.Start(url);
                }
            catch (Exception exp)
                {
                Trace.WriteLine(string.Format("An error on starting browser occurred: {0}", exp));
                return false;
                }

            return true;
            }

        private void startUpdating()
            {
            updateThread = new Thread(performUpdating) { IsBackground = false };
            updateThread.Start();
            }

        private void performUpdating()
            {
            if (!authorize()) return;

            List<UploadingFile> tasks = getUploadingTasks();
            if (tasks == null | tasks.Count == 0) return;

            }

        private List<UploadingFile> getUploadingTasks()
            {
            var result = new List<UploadingFile>();

            foreach (var filesGroup in solutionDetails.FilesGroups)
                {
                var filesList = buildFilesList(filesGroup.Files, filesGroup.Path);

                if (filesGroup.CopyAll)
                    {
                    var filesWithExceptions = new Dictionary<string, bool>();
                    findAllFiles(filesWithExceptions, filesGroup.Path, filesList);

                    filesList = filesWithExceptions;
                    }

                addFiles(result, filesList, filesGroup.Type.ToString());
                }

            return result;
            }

        private void addFiles(List<UploadingFile> resultFiles, Dictionary<string, bool> filesList, string groupName)
            {
            foreach (var kvp in filesList)
                {
                resultFiles.Add(new UploadingFile()
                    {
                        FullPath = kvp.Key,
                        IsCommon = kvp.Value,
                        FileSize = new FileInfo(kvp.Key).Length,
                        GroupName = groupName,
                        Id = resultFiles.Count + 1
                    });
                }
            }

        private void findAllFiles(Dictionary<string, bool> result, string path, Dictionary<string, bool> exceptions)
            {
            var currentDirInfo = new DirectoryInfo(path);

            foreach (var fileInfo in currentDirInfo.GetFiles())
                {
                if (exceptions.ContainsKey(fileInfo.FullName)) continue;

                result.Add(fileInfo.FullName, false);
                }

            foreach (var dirInfo in currentDirInfo.GetDirectories())
                {
                findAllFiles(result, dirInfo.FullName, exceptions);
                }
            }

        private Dictionary<string, bool> buildFilesList(List<FileDetails> fileslist, string path)
            {
            var files = new Dictionary<string, bool>(new IgnoreCaseStringEqualityComparer());

            foreach (var fileDetails in fileslist)
                {
                var fullName = string.Format(@"{0}\{1}", path, fileDetails.SubPath);
                if (File.Exists(fullName))
                    {
                    files.Add(fullName, fileDetails.IsCommon);
                    }
                }

            return files;
            }

        private bool authorize()
            {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            const int AUTHORISATION_TIME_LIMIT_SEC = 60 * 2;
            while (stopwatch.Elapsed.TotalSeconds < AUTHORISATION_TIME_LIMIT_SEC)
                {
                Thread.Sleep(1000);

                if (userAuthorized())
                    {
                    return true;
                    }
                }

            return false;
            }

        private bool userAuthorized()
            {
            var authorizeUrl = string.Format("{0}/sa/UserAuthorized", solutionDetails.UpdateUrl);
            var request = WebRequest.Create(authorizeUrl);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            byte[] byteArray = Encoding.ASCII.GetBytes(id.ToString());
            request.ContentLength = byteArray.Length;

            WebResponse response = null;
            try
                {
                using (var sourceStream = request.GetRequestStream())
                    {
                    sourceStream.Write(byteArray, 0, byteArray.Length);
                    sourceStream.Close();
                    }

                response = request.GetResponse();
                }
            catch (Exception exp)
                {
                Trace.WriteLine(
                    string.Format(@"Error on authorization check: {0}. Error type is ""{1}""", exp, exp.GetType().Name));
                return false;
                }

            Trace.WriteLine(((HttpWebResponse)response).StatusDescription);

            using (var resultStream = response.GetResponseStream())
                {
                using (var reader = new StreamReader(resultStream))
                    {
                    string responseFromServer = reader.ReadToEnd();
                    const string successfulResult = "OK";

                    return successfulResult.Equals(responseFromServer);
                    }
                }
            }
        }
    }
