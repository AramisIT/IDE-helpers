using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using AramisIDE.Models;
using AramisIDE.SolutionUpdating;
using AramisIDE.SourceCodeHelper;

namespace AramisIDE
    {
    public class SolutionsReader
        {
        public static readonly string APPLICATION_PATH = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string AramisSqlUser { get; private set; }

        private static readonly XAttribute EMPTY_ATTRIBUTE = new XAttribute("Empty", string.Empty);

        private readonly List<string> webRootFiles = new List<string> { "packages.config", "Global.asax" };

        public List<SolutionDetails> ReadSolutions()
            {
            var result = new List<SolutionDetails>();

            var xmlFileName = string.Format(@"{0}\Solutions.xml", APPLICATION_PATH);
            try
                {
                using (XmlReader reader = XmlReader.Create(xmlFileName, new XmlReaderSettings() { IgnoreComments = true }))
                    {
                    var xmlDocument = new XmlDocument();
                    xmlDocument.Load(reader);

                    using (var nodeReader = new XmlNodeReader(xmlDocument))
                        {
                        nodeReader.MoveToContent();
                        XDocument document = XDocument.Load(nodeReader);

                        AramisSqlUser = (document.Root.Attribute("aramis-user-name") ?? EMPTY_ATTRIBUTE).Value;

                        new PredefinedStoredObjectsUpdater(
                            (document.Root.Attribute("predefined-stored-objects-path") ?? EMPTY_ATTRIBUTE).Value,
                            (document.Root.Attribute("connection-string") ?? EMPTY_ATTRIBUTE).Value);


                        var hardLinkedFilesDictionary = new Dictionary<FilesGroupTypes, HardLinkedFiles>();
                        readXmlNodes(document.Root, "References", node => readReferences(node, hardLinkedFilesDictionary));
                        readXmlNodes(document.Root, "Solution", node => readSolutions(node, result, hardLinkedFilesDictionary));
                        }
                    }
                }
            catch (Exception exp)
                {
                Trace.WriteLine(string.Format(@"Can't read xml configurations of solutions:
{0}", exp.Message));
                return new List<SolutionDetails>();
                }

            return result;
            }

        private void readReferences(XElement referenceNode, Dictionary<FilesGroupTypes, HardLinkedFiles> hardLinkedFilesDictionary)
            {
            var hardLinkedFiles = new HardLinkedFiles(referenceNode.Attribute("Directory").Value);

            var filesGroupType = (FilesGroupTypes)Enum.Parse(typeof(FilesGroupTypes), referenceNode.Attribute("Type").Value, true);
            hardLinkedFilesDictionary.Add(filesGroupType, hardLinkedFiles);

            foreach (var pathNode in referenceNode.Elements("Path"))
                {
                hardLinkedFiles.AddFile(pathNode.Value);
                }
            }

        private void readXmlNodes(XElement xElement, string nodeName, Action<XElement> processNodeAction)
            {
            foreach (var node in xElement.Elements(nodeName))
                {
                try
                    {
                    processNodeAction(node);
                    }
                catch (Exception exp)
                    {
                    Trace.WriteLine(string.Format(@"Can't read ""{0}"" xml node: {1}", nodeName, exp.Message));
                    }
                }
            }

        private void readSolutions(XElement solution, List<SolutionDetails> result,
            Dictionary<FilesGroupTypes, HardLinkedFiles> hardLinkedFilesByGroupType)
            {
            var solutionDetails = new SolutionDetails()
                {
                Name = solution.Attribute("Name").Value,
                UpdateUrl = solution.Attribute("Url").Value.TrimEnd(new[] { '/' }),
                WebRootDirectory = solution.Attribute("Directory").Value
                };

            if (solution.Attribute("Login") != null)
                {
                solutionDetails.UserName = solution.Attribute("Login").Value;
                if (solution.Attribute("Password") != null)
                    {
                    solutionDetails.Password = solution.Attribute("Password").Value;
                    }
                }

            var hasDesktopInterface = solution.Attribute("DesktopDirectory") != null;
            if (hasDesktopInterface)
                {
                var desktopFilesGroup = new FilesGroup()
                    {
                    Type = FilesGroupTypes.DesktopBin,
                    CopyAll = false,
                    Path = solution.Attribute("DesktopDirectory").Value
                    };
                readDesktopFiles(solution.Elements(), desktopFilesGroup, true);
                solutionDetails.FilesGroups.Add(desktopFilesGroup);
                }

            addWebFilesGroups(solutionDetails.FilesGroups, solutionDetails.WebRootDirectory, hardLinkedFilesByGroupType,
                solutionDetails.DirectoriesToIgnore);
            solutionDetails.CheckFilesDetails();
            result.Add(solutionDetails);
            }

        private void addWebFilesGroups(List<FilesGroup> filesGroups, string webDirectoryPath,
            Dictionary<FilesGroupTypes, HardLinkedFiles> hardLinkedFilesByGroupType, HashSet<string> directoriesToIgnore)
            {
            // root can be like "x:\" or "x:\Projects\MyWebApp" or "x:\Projects\MyWebApp\"
            var pathPrefix = webDirectoryPath.EndsWith("\\")
                ? webDirectoryPath
                : webDirectoryPath + "\\";
            foreach (var groupType in new[]{ FilesGroupTypes.WebScripts,
                                    FilesGroupTypes.WebContent,
                                    FilesGroupTypes.WebViews,
                                    FilesGroupTypes.WebBin})
                {
                var filesGroup = new FilesGroup()
                    {
                    Type = groupType,
                    CopyAll = true,
                    Path = pathPrefix + DefaultSubdirectories.DirectoriesNames[groupType]
                    };
                filesGroups.Add(filesGroup);

                if (groupType == FilesGroupTypes.WebContent)
                    {
                    const string imagesFolder = "Images";
                    const string storageFolder = "Storage";
                    const string thumbnailsFolder = "AramisModelsImages";

                    directoriesToIgnore.Add(Path.Combine(filesGroup.Path, imagesFolder, storageFolder));
                    directoriesToIgnore.Add(Path.Combine(filesGroup.Path, imagesFolder, thumbnailsFolder));
                    }

                HardLinkedFiles hardLinkedFiles;
                if (hardLinkedFilesByGroupType.TryGetValue(groupType, out hardLinkedFiles))
                    {
                    filesGroup.HardLinkedFiles = hardLinkedFiles;
                    if (groupType == FilesGroupTypes.WebScripts)
                        {
                        filesGroup.IgnoreFilesSuffixes.Add(".intellisense.js", false);
                        filesGroup.IgnoreFilesSuffixes.Add("jquery.validate-vsdoc.js", false);
                        }
                    }
                else if (groupType == FilesGroupTypes.WebBin)
                    {
                    filesGroup.IgnoreFilesSuffixes.Add(".xml", false);
                    filesGroup.IgnoreFilesSuffixes.Add(".pdb", false);
                    }
                }

            var rootDir = new FilesGroup()
                {
                Type = FilesGroupTypes.WebRoot,
                CopyAll = false,
                Path = webDirectoryPath,
                DirectoriesToAdd = new List<string>() { "fonts" }
                };
            webRootFiles.ForEach(subPath =>
                {
                    var fullPath = rootDir.BuildFullFilePath(subPath);
                    if (File.Exists(fullPath))
                        {
                        rootDir.Files.Add(new FileDetails()
                            {
                            SubPath = subPath,
                            FullPath = fullPath
                            });
                        }
                });

            filesGroups.Add(rootDir);
            }

        private void readDesktopFiles(IEnumerable<XElement> nodes, FilesGroup desktopFilesGroup, bool isCommonFiles)
            {
            foreach (var node in nodes)
                {
                if (node.Name.ToString().Equals("Path", StringComparison.OrdinalIgnoreCase))
                    {
                    var filePath = new FileDetails()
                        {
                        SubPath = node.Value.Trim(),
                        IsCommon = isCommonFiles,
                        };

                    filePath.FullPath = desktopFilesGroup.BuildFullFilePath(filePath.SubPath);
                    if (File.Exists(filePath.FullPath))
                        {
                        desktopFilesGroup.Files.Add(filePath);
                        }
                    }
                else if (node.Name.ToString().Equals("DesktopOnlyFiles", StringComparison.OrdinalIgnoreCase))
                    {
                    readDesktopFiles(node.Elements("Path"), desktopFilesGroup, false);
                    }
                }
            }
        }
    }
