using AramisIDE.Models;
using AramisIDE.Utils;
using AramisIDE.Utils.FileUploader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
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
            tasks = getUploadingTasks();
            if (tasks == null || tasks.Files.Count == 0) return false;

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

        private UpdateLogger log = UpdateLogger.Instance;

        private void performUpdating()
            {
            if (!authorize()) return;

            log.Reset();
            if (!uploadTasksList(tasks)) return;
            log.Append("Tasks have been uploaded: ");
            tasks.Files.ForEach(file => log.Append(file.FilePath, "File"));

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
                    UpdateLogger.Instance.Append(result, "Can't upload the file");
                    log.Append(task.FilePath, "Aborted!");
                    return false;
                    }
                else
                    {
                    uploaded++;
                    Trace.WriteLine(string.Format(@"Uploaded ""{0}"". Left to upload: {1}", task.FilePath, (totalFiles - uploaded)));
                    }

                log.Append(task.FilePath, "File uploaded");

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

            var webBinFiles = new Dictionary<string, UploadingFile>();

            foreach (var filesGroup in solutionDetails.FilesGroups)
                {
                var filesList = new Dictionary<string, FileDetails>();

                if (filesGroup.CopyAll)
                    {
                    findAllFiles(filesList, filesGroup.Path, filesGroup);
                    }
                else
                    {
                    foreach (var fileDetails in filesGroup.Files)
                        {
                        filesList.Add(fileDetails.FullPath, fileDetails);
                        }
                    }

                var firstIndex = result.Files.Count;
                addFiles(result.Files, filesList, filesGroup);

                if (filesGroup.Type == FilesGroupTypes.WebBin)
                    {
                    for (int index = firstIndex; index < result.Files.Count; index += 1)
                        {
                        var file = result.Files[index];
                        webBinFiles.Add(file.FilePath, file);
                        }
                    }
                }

            if (!commonFilesAreEqual(result.Files, webBinFiles)) return null;

            foreach (var fileInfo in result.Files)
                {
                if (string.IsNullOrEmpty(fileInfo.Hash))
                    {
                    fileInfo.Hash = getFileHash(fileInfo.FullPath);
                    }
                }

            return result;
            }

        private bool commonFilesAreEqual(List<UploadingFile> updatingFilesList, Dictionary<string, UploadingFile> webBinFiles)
            {
            if (solutionDetails.DesktopBin != null && solutionDetails.DesktopBin.Files.Count > 0)
                {
                foreach (var fileDetails in solutionDetails.DesktopBin.Files)
                    {
                    if (!fileDetails.IsCommon) continue;

                    var fileSubPath = fileDetails.SubPath;

                    UploadingFile webApplicationFile;
                    if (!webBinFiles.TryGetValue(fileSubPath, out webApplicationFile))
                        {
                        MessageBox.Show(string.Format(@"File ""{0}"" not found in web solution!", fileSubPath));
                        return false;
                        }

                    var desktopFile = findDesktopFile(updatingFilesList, fileSubPath);
                    webApplicationFile.Id = desktopFile.Id;

                    webApplicationFile.Hash = getFileHash(webApplicationFile.FullPath);
                    desktopFile.Hash = getFileHash(desktopFile.FullPath);

                    if (!string.Equals(desktopFile.Hash, webApplicationFile.Hash))
                        {
                        var webIsNewer = new FileInfo(desktopFile.FullPath).LastWriteTime <
                                         new FileInfo(webApplicationFile.FullPath).LastWriteTime;

                        var additionalMessage = (webIsNewer ? "Web" : "Desktop") + " file is newer!";
                        MessageBox.Show(string.Format(@"File ""{0}"" is defferent for desktop and web!

{1}", fileSubPath, additionalMessage));

                        return false;
                        }
                    }
                }

            return true;
            }

        private UploadingFile findDesktopFile(List<UploadingFile> updatingFilesList, string subPath)
            {
            foreach (var uploadingFile in updatingFilesList)
                {
                if (uploadingFile.IsDesktop
                    && uploadingFile.FilePath.Equals(subPath, StringComparison.OrdinalIgnoreCase))
                    {
                    return uploadingFile;
                    }
                }

            return null;
            }

        private string getFileHash(string filePath)
            {
            try
                {
                return FileHashGenerator.GetFileHash(filePath, FileHashGenerator.HashType.SHA512);
                }
            catch (Exception exp)
                {
                var error = ("Updating.GetFileModificationDate() : Ошибка при попытке получения хешкода файла\r\n" + exp.Message);
                MessageBox.Show(error);
                throw exp;
                }
            }

        private void addFiles(List<UploadingFile> files, Dictionary<string, FileDetails> filesList, FilesGroup filesGroup)
            {
            foreach (var kvp in filesList)
                {
                var fileDetails = kvp.Value;
                var isDesktop = filesGroup.Type == FilesGroupTypes.DesktopBin;
                var fileInfo = new FileInfo(kvp.Key);

                var uploadingFile = new UploadingFile()
                    {
                        FullPath = kvp.Key,
                        IsDesktop = isDesktop,
                        FileSize = fileInfo.Length,
                        Group = (FilesGroupTypes)Enum.Parse(typeof(FilesGroupTypes), filesGroup.Type.ToString()),
                        ModifiedTime = fileInfo.LastWriteTime,
                        FilePath = fileDetails.SubPath
                    }.CreateId();
                uploadingFile.SetFilePath(fileDetails.SubPath);

                files.Add(uploadingFile);
                }
            }

        private void findAllFiles(Dictionary<string, FileDetails> result, string path, FilesGroup filesGroup)
            {
            var currentDirInfo = new DirectoryInfo(path);

            foreach (var fileInfo in currentDirInfo.GetFiles())
                {
                if (filesGroup.IgnoreFilesExtensions != null
                    && filesGroup.IgnoreFilesExtensions.Contains(Path.GetExtension(fileInfo.FullName))) continue;

                var subPath = fileInfo.FullName.Substring(filesGroup.Path.Length + 1);
                string fullPath;
                if (filesGroup.HardLinkedFiles == null ||
                    !filesGroup.HardLinkedFiles.SubPaths.TryGetValue(subPath, out fullPath))
                    {
                    fullPath = fileInfo.FullName;
                    }

                result.Add(fileInfo.FullName, new FileDetails()
                    {
                        FullPath = fullPath,
                        SubPath = subPath
                    });
                }

            foreach (var dirInfo in currentDirInfo.GetDirectories())
                {
                findAllFiles(result, dirInfo.FullName, filesGroup);
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
        private UpdatingFilesList tasks;

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
