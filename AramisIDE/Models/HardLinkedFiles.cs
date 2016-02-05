using System;
using AramisIDE.SolutionUpdating;
using System.Collections.Generic;

namespace AramisIDE.Models
    {
    public class HardLinkedFiles
        {
        private string pathPrefix;

        public Dictionary<string, string> SubPaths { get; private set; }

        public HardLinkedFiles(string directory = "")
            {
            this.pathPrefix = directory.EndsWith("\\", StringComparison.OrdinalIgnoreCase) ? directory : directory + "\\";
            SubPaths = new Dictionary<string, string>(new IgnoreCaseStringEqualityComparer());
            }

        public void AddFile(string filePath)
            {
            if (string.IsNullOrEmpty(filePath) || SubPaths.ContainsKey(filePath)) return;

            SubPaths.Add(filePath,pathPrefix + filePath);
            }
        }
    }
