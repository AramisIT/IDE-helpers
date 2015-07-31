using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using AramisIDE.Models;

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

                document.Root.Elements("Solution").ToList().ForEach(solution =>
                    {
                        var solutionDetails = new SolutionDetails()
                            {
                                Name = solution.Attribute("Name").Value,
                                UpdateUrl = solution.Attribute("Url").Value.TrimEnd(new[] { '/' })
                            };

                        result.Add(solutionDetails);

                        solution.Elements("FilesGroup").ToList().ForEach(fileGroupXml =>
                            {
                                var filesGroup = new FilesGroup()
                                    {
                                        CopyAll = bool.Parse((fileGroupXml.Attribute("CopyAll") ?? FALSE_ATTRIBUTE).Value),
                                        Path = (fileGroupXml.Attribute("Path") ?? EMPTY_ATTRIBUTE).Value.Trim(new[] { '\\' }),
                                        Type = (FilesGroupTypes)Enum.Parse(typeof(FilesGroupTypes), (fileGroupXml.Attribute("Type") ?? EMPTY_ATTRIBUTE).Value)
                                    };

                                solutionDetails.FilesGroups.Add(filesGroup);

                                fileGroupXml.Elements("Path").ToList().ForEach(pathXml =>
                                {
                                    var filePath = new FileDetails()
                                    {
                                        IsCommon = bool.Parse((pathXml.Attribute("Common") ?? FALSE_ATTRIBUTE).Value),
                                        SubPath = pathXml.Value.Trim()
                                    };

                                    if (!string.IsNullOrEmpty(filePath.SubPath))
                                        {
                                        filesGroup.Files.Add(filePath);
                                        }
                                });
                            });
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
