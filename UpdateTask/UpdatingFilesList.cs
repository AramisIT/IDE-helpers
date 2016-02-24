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

        [XmlIgnore]
        public string StringView
            {
            get
                {
                var result = new StringBuilder();
                foreach (var uploadingFile in Files)
                    {
                    result.AppendLine(string.Format("{0} ({1})", uploadingFile.FilePath, uploadingFile.Group));
                    }
                return result.ToString();
                }
            }
        }
    }
