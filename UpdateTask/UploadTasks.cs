using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using AramisIDE.SolutionUpdating;

namespace UpdateTask
    {
    [XmlRoot("Root")]
    [XmlInclude(typeof(UploadTask))]
    public class UploadTasks
        {
        [XmlArray("Files")]
        [XmlArrayItem("UploadTask")]
        public List<UploadTask> Files { get; set; }
        }

    [XmlType("UploadTask")]
    public class UploadTask
        {
        [XmlText]
        public Guid Id { get; set; }

        [XmlAttribute]
        public bool FatClientFile { get; set; }
        }
    }
