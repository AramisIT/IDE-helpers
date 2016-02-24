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

            if (uploadFilesToWebServer(tasks))
                {
                //new WebClientHelper(string.Format("{0}/sa/UpdateSolution?id={1}", solutionDetails.UpdateUrl, tasks.UpdateId)).PerformPostRequest();
                }
            }

        private bool uploadFilesToWebServer(UpdatingFilesList tasks)
            {
            var totalFiles = tasks.Files.Count;
            var uploaded = 0;

            var fileUploader = new HttpFileUploader(solutionDetails.UserName, solutionDetails.Password);

            UploadingFile lastTask = tasks.Files.Count > 0 ? tasks.Files.Last() : null;
            var maxUploadingAttemptCount = 10;
            var fatClientsFiles = tasks.Files[0].IsDesktop;

            foreach (var task in tasks.Files)
                {
                var checkIfFatClientsOnly = fatClientsFiles && !task.IsDesktop;
                if (checkIfFatClientsOnly)
                    {
                    if (isFatClientsOnly())
                        {
                        Trace.WriteLine("All tasks were uploaded!");
                        return true;
                        }

                    fatClientsFiles = false;
                    }

                fileUploader.Url = string.Format("{0}/sa/UploadFile?id={1}", solutionDetails.UpdateUrl, task.Id);

                var attemptIndex = 0;
                string uploadResult = null;
                while (attemptIndex < maxUploadingAttemptCount)
                    {
                    uploadResult = fileUploader.UploadFile(task.FullPath);

                    if (SUCCESSFUL_RESULT.Equals(uploadResult))
                        {
                        uploaded++;
                        Trace.WriteLine(string.Format(@"Uploaded ""{0}"". Left to upload: {1}", task.FilePath, (totalFiles - uploaded)));
                        break;
                        }
                    else
                        {
                        attemptIndex += 1;
                        Thread.Sleep(attemptIndex * 100);
                        }
                    }

                if (attemptIndex == maxUploadingAttemptCount)
                    {
                    UpdateLogger.Instance.Append(uploadResult, "Can't upload the file");
                    log.Append(task.FilePath, "Aborted!");
                    return false;
                    }

                log.Append(task.FilePath, "File uploaded");

                if (task == lastTask)
                    {
                    Trace.WriteLine("All tasks were uploaded!");
                    }
                }

            Trace.WriteLine("UploadFilesToWebServer: quit");
            return true;
            }

        private bool isFatClientsOnly()
            {
            var url = string.Format("{0}/sa/IsFatClientsOnly", solutionDetails.UpdateUrl);

            var result = new WebClientHelper(url).PerformPostRequest();

            return Convert.ToBoolean(result);
            }

        private bool uploadTasksList(UpdatingFilesList tasks)
            {
            var tasksXml = XmlConvertor.ToXmlString(tasks);

            var url = string.Format("{0}/sa/TaskReceiver", solutionDetails.UpdateUrl);

            var result = new WebClientHelper(url).AddContent(tasksXml).PerformPostRequest();

            try { Clipboard.SetText(result); }
            catch { }

            var resultUploadTasks = XmlConvertor.ToObjectFromXmlString<UploadTasks>(result);

            if (resultUploadTasks != null && resultUploadTasks.Files != null && resultUploadTasks.Files.Count > 0)
                {
                var uploadTasksDictionary = new Func<UploadTasks, Dictionary<Guid, bool>>(uploadTasks =>
                {
                    var dictionary = new Dictionary<Guid, bool>();
                    uploadTasks.Files.ForEach(uploadTask => dictionary.Add(uploadTask.Id, uploadTask.FatClientFile));
                    return dictionary;
                })(resultUploadTasks);

                var source = tasks.Files.ToList();
                tasks.Files.Clear();

                // updating uploading files set
                foreach (var uploadingFile in source)
                    {
                    bool fatClientFile;
                    // if file wasn't changed we shouldn't upload it
                    if (!uploadTasksDictionary.TryGetValue(uploadingFile.Id, out fatClientFile)) continue;

                    uploadTasksDictionary.Remove(uploadingFile.Id);
                    uploadingFile.IsDesktop = fatClientFile;

                    if (fatClientFile)
                        {
                        // optimizing in case fat clients only - fat clients files must be uploaded first
                        tasks.Files.Insert(0, uploadingFile);
                        }
                    else
                        {
                        tasks.Files.Add(uploadingFile);
                        }
                    }
                return true;
                }
            else
                {
                return false;
                }
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
                    if (filesGroup.DirectoriesToAdd != null)
                        {
                        foreach (var directoryName in filesGroup.DirectoriesToAdd)
                            {
                            findAllFiles(filesList, solutionDetails.WebRootDirectory + "\\" + directoryName, filesGroup);
                            }
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
                    var fileSubPath = fileDetails.SubPath;
                    UploadingFile webApplicationFile;
                    if (!webBinFiles.TryGetValue(fileSubPath, out webApplicationFile))
                        {
                        if (!fileDetails.IsCommon) continue;
                        MessageBox.Show(string.Format(@"File ""{0}"" not found in web solution!", fileSubPath));
                        return false;
                        }

                    var desktopFile = findDesktopFile(updatingFilesList, fileSubPath);
                    webApplicationFile.Id = desktopFile.Id;
                    desktopFile.Hash = getFileHash(desktopFile.FullPath);

                    if (!fileDetails.IsCommon)
                        {
                        webApplicationFile.FullPath = desktopFile.FullPath;
                        webApplicationFile.Hash = desktopFile.Hash;
                        }
                    else
                        {
                        webApplicationFile.Hash = getFileHash(webApplicationFile.FullPath);
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
                var skipFile = false;
                foreach (var kvp in filesGroup.IgnoreFilesSuffixes)
                    {
                    var fileSuffix = kvp.Key;
                    var checkWholeFileName = kvp.Value;
                    if (checkWholeFileName)
                        {
                        if (Path.GetFileName(fileInfo.FullName).Equals(fileSuffix))
                            {
                            skipFile = true;
                            break;
                            }
                        }
                    else if (fileInfo.FullName.EndsWith(fileSuffix, StringComparison.InvariantCultureIgnoreCase))
                        {
                        skipFile = true;
                        break;
                        }
                    }
                if (skipFile) continue;

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
            Thread.Sleep(2 * 1000);

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
