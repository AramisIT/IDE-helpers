using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using AramisIDE.Models;
using AramisIDE.SolutionUpdating;
using AramisIDE.SourceCodeHelper;

namespace AramisIDE
    {
    public class SolutionsReader
        {
        public static readonly string APPLICATION_PATH = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private static readonly XAttribute EMPTY_ATTRIBUTE = new XAttribute("Empty", string.Empty);
        private static readonly XAttribute FALSE_ATTRIBUTE = new XAttribute("Empty", "False");

        public List<SolutionDetails> ReadSolutions()
            {
            var result = new List<SolutionDetails>();
            XDocument document = null;

            try
                {
                var xmlString = File.ReadAllText(string.Format(@"{0}\Solutions.xml", APPLICATION_PATH));
                document = XDocument.Parse(xmlString);

                new PredefinedStoredObjectsUpdater((document.Root.Attribute("predefined-stored-objects-path") ?? EMPTY_ATTRIBUTE).Value,
                    (document.Root.Attribute("connection-string") ?? EMPTY_ATTRIBUTE).Value);


                document.Root.Elements("Solution").ToList().ForEach(solution =>
                    {
                        try
                            {
                            var solutionDetails = new SolutionDetails()
                                {
                                    Name = solution.Attribute("Name").Value,
                                    UpdateUrl = solution.Attribute("Url").Value.TrimEnd(new[] { '/' })
                                };

                            if (solution.Attribute("Login") != null)
                                {
                                solutionDetails.UserName = solution.Attribute("Login").Value;
                                if (solution.Attribute("Password") != null)
                                    {
                                    solutionDetails.Password = solution.Attribute("Password").Value;
                                    }
                                }

                            solution.Elements("FilesGroup").ToList().ForEach(fileGroupXml =>
                                {
                                    var filesGroup = new FilesGroup()
                                        {
                                            CopyAll = bool.Parse((fileGroupXml.Attribute("CopyAll") ?? FALSE_ATTRIBUTE).Value),
                                            Path = (fileGroupXml.Attribute("Path") ?? EMPTY_ATTRIBUTE).Value.Trim(new[] { '\\' }),
                                            Type =
                                            (FilesGroupTypes)
                                                Enum.Parse(typeof(FilesGroupTypes),
                                                    (fileGroupXml.Attribute("Type") ?? EMPTY_ATTRIBUTE).Value)
                                        };

                                    solutionDetails.FilesGroups.Add(filesGroup);

                                    fileGroupXml.Elements().ToList().ForEach(pathXml =>
                                        {
                                            var nodeTypeName = pathXml.Name.LocalName;
                                            var filePath = new FileDetails()
                                            {
                                                SubPath = pathXml.Value.Trim(),
                                                IsRef = nodeTypeName.Equals("Reference", StringComparison.InvariantCultureIgnoreCase),
                                                IsCommon = bool.Parse((pathXml.Attribute("Common") ?? FALSE_ATTRIBUTE).Value),
                                                FullPath = (pathXml.Attribute("Path") ?? EMPTY_ATTRIBUTE).Value
                                            };

                                            if (!filePath.IsRef)
                                                {
                                                filePath.FullPath = filesGroup.BuildFullFilePath(filePath.SubPath);
                                                }

                                            if (File.Exists(filePath.FullPath))
                                                {
                                                filesGroup.Files.Add(filePath);
                                                }
                                        });
                                });

                            solutionDetails.CheckFilesDetails();
                            result.Add(solutionDetails);
                            }
                        catch { }
                    });
                }
            catch
                {
                return new List<SolutionDetails>();
                }

            return result;
            }
        }
    }
