﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AramisIDE.Models
    {
    public class FileDetails
        {
        public string SubPath { get; set; }

        public bool IsCommon { get; set; }

        public override string ToString()
            {
            return SubPath;
            }
        }
    }
