using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AramisIDE.SolutionUpdating;

namespace AramisIDE.Models
    {
    public class FilesGroup
        {
        public bool CopyAll { get; set; }

        public string Path { get; set; }

        public FilesGroupTypes Type { get; set; }

        public List<FileDetails> Files { get; private set; }

        public FilesGroup()
            {
            Files = new List<FileDetails>();
            IgnoreFilesExtensions = new HashSet<string>(new IgnoreCaseStringEqualityComparer());
            }

        public override string ToString()
            {
            return Type.ToString();
            }

        public string BuildFullFilePath(string subPath)
            {
            return string.Format(@"{0}\{1}", Path, subPath);
            }

        public HashSet<string> IgnoreFilesExtensions { get; private set; }

        public HardLinkedFiles HardLinkedFiles { get; set; }
        }
    }
