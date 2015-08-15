using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using AramisIDE.Models;
using AramisIDE.Utils;
using AramisIDE.Utils.FileUploader;
using UpdateTask;
using ThreadState = System.Threading.ThreadState;

namespace AramisIDE.SolutionUpdating
    {
    public class SolutionUpdater
        {
        private SolutionDetails solutionDetails;
        private Guid id;

        private static object threadLocker = new object();
        private static Thread _updateThread;

        private static Thread updateThread
            {
            get
                {
                lock (threadLocker)
                    {
                    return _updateThread;
                    }
                }

            set
                {
                lock (threadLocker)
                    {
                    _updateThread = value;
                    }
                }
            }

        public SolutionUpdater(SolutionDetails solutionDetails)
            {
            id = Guid.NewGuid();
            this.solutionDetails = solutionDetails;
            }

        public bool CanStartUpdating()
            {
            return updateThread == null
                   || updateThread.ThreadState == ThreadState.Stopped;
            }

        public bool Update()
            {
            var askUserForCloseAllDesktopSessions = (Control.ModifierKeys & Keys.Control) != 0;

            if (askUserForCloseAllDesktopSessions)
                {
                var dialogResult = MessageBox.Show(string.Format("Завершить все начатые сеансы принудительно?", solutionDetails.Name),
                    string.Format(@"Обновление ""{0}""", solutionDetails.Name), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                switch (dialogResult)
                    {
                    case DialogResult.Cancel:
                        return true;

                    case DialogResult.Yes:
                        restartAllDesktopClients = true;
                        break;
                    }
                }

            if (!checkInternetAccess())
                {
                MessageBox.Show("Нет интернет доступа!");
                return false;
                }

            if (!startWebApplication()) return false;

            return startUpdating();
            }

        private bool checkInternetAccess()
            {
            var authorizeUrl = string.Format("{0}/sa/ApplicationIsOk", solutionDetails.UpdateUrl);

            var result = new WebClientHelper(authorizeUrl)
                 .PerformPostRequest();

            return result.Equals(SUCCESSFUL_RESULT);
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

        private bool startUpdating()
            {
            updateThread = new Thread(performUpdating) { IsBackground = false };
            updateThread.Start();

            return true;
            }

        private void performUpdating()
            {
            if (!authorize()) return;

            UpdatingFilesList tasks = getUploadingTasks();
            if (tasks == null || tasks.Files.Count == 0) return;

            tasks.RestartAllDesktopClients = restartAllDesktopClients;
            if (!uploadTasksList(tasks)) return;

            uploadFilesToWebServer(tasks);
            }

        private bool uploadFilesToWebServer(UpdatingFilesList tasks)
            {
            var totalFiles = tasks.Files.Count;
            var uploaded = 0;

            var fileUploader = new HttpFileUploader(solutionDetails.UserName, solutionDetails.Password);

            UploadingFile lastTask = tasks.Files.Count > 0 ? tasks.Files.Last() : null;
            foreach (var task in tasks.Files)
                {
                fileUploader.Url = string.Format("{0}/sa/UploadFile?id={1}", solutionDetails.UpdateUrl, task.Id);
                var result = fileUploader.UploadFile(task.FullPath);

                if (!SUCCESSFUL_RESULT.Equals(result))
                    {
                    return false;
                    }
                else
                    {
                    uploaded++;
                    Trace.WriteLine(string.Format(@"Uploaded ""{0}"". Left to upload: {1}", task.FilePath, (totalFiles - uploaded)));
                    }

                if (task == lastTask)
                    {
                    Trace.WriteLine("All tasks were uploaded!");
                    }
                }

            return true;
            }

        private bool uploadTasksList(UpdatingFilesList tasks)
            {
            var tasksXml = XmlConvertor.ToXmlString(tasks);

            var url = string.Format("{0}/sa/TaskReceiver", solutionDetails.UpdateUrl);

            var result = new WebClientHelper(url).AddContent(tasksXml).PerformPostRequest();

            return result.Equals(SUCCESSFUL_RESULT);
            }

        private UpdatingFilesList getUploadingTasks()
            {
            var result = new UpdatingFilesList() { UpdateId = id };

            foreach (var filesGroup in solutionDetails.FilesGroups)
                {
                var filesList = new Dictionary<string, FileDetails>();
                filesGroup.Files.ForEach(fileDetails => filesList.Add(fileDetails.FullPath, fileDetails));

                if (filesGroup.CopyAll)
                    {
                    var filesWithExceptions = new Dictionary<string, FileDetails>();

                    findAllFiles(filesWithExceptions, filesGroup.Path, filesList);

                    filesList = filesWithExceptions;

                    filesGroup.Files.ForEach(fileDetails =>
                        {
                            if (fileDetails.IsRef)
                                {
                                filesList.Add(fileDetails.FullPath, fileDetails);
                                }
                        });
                    }

                addFiles(result, filesList, filesGroup.Type.ToString());
                }

            return result;
            }

        private void addFiles(UpdatingFilesList resultFiles, Dictionary<string, FileDetails> filesList, string groupName)
            {
            foreach (var kvp in filesList)
                {
                var fileDetails = kvp.Value;
                var isCommon = fileDetails.IsCommon;
                var isDesktop = isCommon || groupName.Equals("DesktopBin");
                var fileInfo = new FileInfo(kvp.Key);

                var uploadingFile = new UploadingFile()
                    {
                        FullPath = kvp.Key,
                        IsDesktop = isDesktop,
                        IsWebSystem = isCommon || !isDesktop,
                        FileSize = fileInfo.Length,
                        Group = (FilesGroupTypes)Enum.Parse(typeof(FilesGroupTypes), groupName),
                        ModifiedTime = fileInfo.LastWriteTime,
                        FilePath = fileDetails.SubPath
                    }.CreateId();
                uploadingFile.SetFilePath(fileDetails.SubPath);

                resultFiles.Add(uploadingFile);
                }
            }

        private void findAllFiles(Dictionary<string, FileDetails> result, string path, Dictionary<string, FileDetails> exceptions)
            {
            var currentDirInfo = new DirectoryInfo(path);

            foreach (var fileInfo in currentDirInfo.GetFiles())
                {
                if (exceptions.ContainsKey(fileInfo.FullName)) continue;

                result.Add(fileInfo.FullName, new FileDetails()
                    {
                        FullPath = fileInfo.FullName
                    });
                }

            foreach (var dirInfo in currentDirInfo.GetDirectories())
                {
                findAllFiles(result, dirInfo.FullName, exceptions);
                }
            }

        private bool authorize()
            {
            Thread.Sleep(10 * 1000);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            const int AUTHORISATION_TIME_LIMIT_SEC = 60 * 2;
            while (stopwatch.Elapsed.TotalSeconds < AUTHORISATION_TIME_LIMIT_SEC)
                {
                if (userAuthorized())
                    {
                    return true;
                    }

                Thread.Sleep(1000);
                }

            return false;
            }

        const string SUCCESSFUL_RESULT = "OK";
        private bool restartAllDesktopClients;

        private bool userAuthorized()
            {
            var authorizeUrl = string.Format("{0}/sa/UserAuthorized", solutionDetails.UpdateUrl);

            var result = new WebClientHelper(authorizeUrl)
                 .AddContent(id)
                 .PerformPostRequest();

            return result.Equals(SUCCESSFUL_RESULT);
            }
        }
    }
