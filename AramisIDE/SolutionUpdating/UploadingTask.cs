using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AramisIDE.SolutionUpdating
    {
    public class UploadingFile
        {
        public string FullPath { get; set; }

        public long FileSize { get; set; }

        public int Id { get; set; }

        public bool IsCommon { get; set; }

        public string GroupName { get; set; }

        public override string ToString()
            {
            return Path.GetFileName(FullPath);
            }
        }
    }
