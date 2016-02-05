using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using AramisIDE.SolutionUpdating;

namespace UpdateTask
    {
    [XmlRoot("Root")]
    [XmlInclude(typeof(UploadingFile))]
    public class UpdatingFilesList
        {
        [XmlArray("Files")]
        [XmlArrayItem("UploadingFile")]
        public List<UploadingFile> Files = new List<UploadingFile>();

        [XmlAttribute]
        public Guid UpdateId { get; set; }

        [XmlAttribute]
        public bool ReadyToUpdateSqlStructure { get; set; }

        [XmlAttribute]
        public int UpdateDocumentId { get; set; }
        }
    }
