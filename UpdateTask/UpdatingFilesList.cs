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

        public void Add(UploadingFile file)
            {
            Files.Add(file);
            }

        public UpdatingFilesList GetCopyWithRelativePath()
            {
            var result = new UpdatingFilesList() { UpdateId = UpdateId, RestartAllDesktopClients = RestartAllDesktopClients };

            foreach (var file in Files)
                {
                result.Files.Add(file.GetCopyWithRelativePath());
                }
            return result;
            }

        [XmlAttribute]
        public bool RestartAllDesktopClients { get; set; }
        }
    }
