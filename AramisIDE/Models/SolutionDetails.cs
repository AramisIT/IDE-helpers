using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AramisIDE.Models;

namespace AramisIDE
    {
    public class SolutionDetails
        {
        public string Name { get; set; }

        public string UpdateUrl { get; set; }

        public List<FilesGroup> FilesGroups { get; private set; }

        public SolutionDetails()
            {
            FilesGroups = new List<FilesGroup>();
            }

        public override string ToString()
            {
            return Name;
            }
        }
    }
