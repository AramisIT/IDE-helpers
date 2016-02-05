using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AramisIDE.Models;
using AramisIDE.SolutionUpdating;

namespace AramisIDE
    {
    public class SolutionDetails
        {
        public string Name { get; set; }

        public string UpdateUrl { get; set; }

        public string WebRootDirectory { get; set; }

        public List<FilesGroup> FilesGroups { get; private set; }

        public SolutionDetails()
            {
            FilesGroups = new List<FilesGroup>();
            }

        public override string ToString()
            {
            return string.Format("{0}     {1}", Name, WebRootDirectory);
            }

        public string UserName { get; set; }

        public string Password { get; set; }

        public FilesGroup WebBin { get; private set; }

        public FilesGroup DesktopBin { get; private set; }

        public bool CheckFilesDetails()
            {
            foreach (var filesGroup in FilesGroups)
                {
                if (filesGroup.Type == FilesGroupTypes.WebBin)
                    {
                    WebBin = filesGroup;
                    }
                else if (filesGroup.Type == FilesGroupTypes.DesktopBin)
                    {
                    DesktopBin = filesGroup;
                    }
                }

            if (WebBin == null) return false;
            if (DesktopBin == null) return true;

            for (var fileIndex = DesktopBin.Files.Count - 1; fileIndex >= 0; fileIndex -= 1)
                {
                var desktopFile = DesktopBin.Files[fileIndex];
                FileDetails webFile;

                if (!findTheFileWithSamePath(WebBin, desktopFile, out webFile)) continue;
                if (!webFile.IsCommon && !desktopFile.IsCommon) continue;

                DesktopBin.Files.RemoveAt(fileIndex);
                webFile.IsCommon = true;
                }

            return true;
            }

        private bool findTheFileWithSamePath(FilesGroup filesGroup, FileDetails fileToMatch, out FileDetails matchedFile)
            {
            foreach (var file in filesGroup.Files)
                {
                if (file.SubPath.Equals(fileToMatch.SubPath))
                    {
                    matchedFile = file;
                    return true;
                    }
                }
            matchedFile = null;
            return false;
            }
        }
    }
