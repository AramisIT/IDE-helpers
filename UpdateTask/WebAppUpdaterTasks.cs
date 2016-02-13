using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using AramisIDE.SolutionUpdating;

namespace UpdateTask
    {
    [XmlRoot("Root")]
    [XmlInclude(typeof(UpdateFileTaskDetails))]
    public class WebAppUpdaterTasks
        {
        [XmlArray("FilesToAdd")]
        [XmlArrayItem("UpdateFileTaskDetails")]
        public List<UpdateFileTaskDetails> FilesToAdd { get; set; }

        [XmlArray("FilesToRewrite")]
        [XmlArrayItem("UpdateFileTaskDetails")]
        public List<UpdateFileTaskDetails> FilesToRewrite { get; set; }

        [XmlArray("FilesToRemove")]
        public List<string> FilesToRemove { get; set; }

        [XmlArray("DirectoriesToAdd")]
        public List<string> DirectoriesToAdd { get; set; }

        [XmlArray("DirectoriesToRemove")]
        public List<string> DirectoriesToRemove { get; set; }

        [XmlAttribute]
        public Guid UpdateId { get; set; }

        [XmlAttribute]
        public bool ReadyToUpdateSqlStructure { get; set; }
        }

    [XmlType("UpdateFileTaskDetails")]
    public class UpdateFileTaskDetails
        {
        [XmlText]
        public string FullPath { get; set; }

        [XmlAttribute]
        public Guid Id { get; set; }
        }
    }
