using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AramisIDE.Models
    {
    public enum FilesGroupTypes
        {
        WebViews,
        WebScripts,
        WebContent,
        WebBin,
        DesktopBin
        }

    public class FilesGroup
        {
        public bool CopyAll { get; set; }

        public string Path { get; set; }

        public FilesGroupTypes Type { get; set; }

        public List<FileDetails> Files { get; private set; }

        public FilesGroup()
            {
            Files = new List<FileDetails>();
            }

        public override string ToString()
            {
            return Type.ToString();
            }
        }
    }
