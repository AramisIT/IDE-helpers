using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace AramisIDE.SolutionUpdating
    {
    public enum FilesGroupTypes
        {
        WebRoot,
        WebViews,
        WebScripts,
        WebContent,
        WebBin,
        DesktopBin
        }

    public class DefaultSubdirectories
        {
        public static readonly Dictionary<FilesGroupTypes, string> DirectoriesNames = new Dictionary<FilesGroupTypes, string>()
            {
            {FilesGroupTypes.WebScripts, "Scripts"},
            {FilesGroupTypes.WebContent, "Content"},
            {FilesGroupTypes.WebViews, "Views"},
            {FilesGroupTypes.WebBin, "bin"},
            {FilesGroupTypes.WebRoot, ""}
            };
        }

    [XmlType("UploadingFile")]
    public class UploadingFile
        {
        [XmlAttribute("Size")]
        public long FileSize { get; set; }

        [XmlIgnore]
        public string FullPath { get; set; }

        [XmlText]
        public string Hash { get; set; }

        [XmlAttribute]
        public string FilePath { get; set; }

        [XmlAttribute]
        public Guid Id { get; set; }

        [XmlAttribute]
        public bool IsDesktop { get; set; }

        [XmlAttribute]
        public DateTime ModifiedTime { get; set; }

        [XmlAttribute]
        public FilesGroupTypes Group { get; set; }

        private Guid getNewGuid(FilesGroupTypes group)
            {
            string idPrefix = "0";
            switch (group)
                {
                case FilesGroupTypes.WebRoot:
                    idPrefix = "9";
                    break;

                case FilesGroupTypes.WebScripts:
                    idPrefix = "B";
                    break;

                case FilesGroupTypes.WebContent:
                    idPrefix = "C";
                    break;

                case FilesGroupTypes.WebViews:
                    idPrefix = "A";
                    break;

                case FilesGroupTypes.WebBin:
                    idPrefix = "F";
                    break;

                case FilesGroupTypes.DesktopBin:
                    idPrefix = "D";
                    break;
                }

            Guid guid;

            while (!(guid = Guid.NewGuid()).ToString().StartsWith(idPrefix, StringComparison.InvariantCultureIgnoreCase)) { }

            return guid;
            }

        public UploadingFile CreateId()
            {
            Id = getNewGuid(Group);

            return this;
            }

        public override string ToString()
            {
            return Path.GetFileName(FilePath);
            }

        public void SetFilePath(string filePath)
            {
            FilePath = (filePath ?? string.Empty).Trim();

            if (!string.IsNullOrEmpty(FilePath)) return;

            switch (Group)
                {
                case FilesGroupTypes.WebBin:
                case FilesGroupTypes.DesktopBin:
                case FilesGroupTypes.WebRoot:
                    FilePath = Path.GetFileName(FullPath);
                    break;

                case FilesGroupTypes.WebScripts:
                    FilePath = getRelativePath(FullPath, "Scripts");
                    break;

                case FilesGroupTypes.WebViews:
                    FilePath = getRelativePath(FullPath, "Views");
                    break;

                case FilesGroupTypes.WebContent:
                    FilePath = getRelativePath(FullPath, "Content");
                    break;
                }
            }

        private string getRelativePath(string filePath, string folderName)
            {
            var suffix = "\\" + folderName + "\\";
            var pos = filePath.LastIndexOf(suffix);
            if (pos > 0)
                {
                return filePath.Substring(pos + suffix.Length);
                }

            return filePath;
            }
        }
    }
