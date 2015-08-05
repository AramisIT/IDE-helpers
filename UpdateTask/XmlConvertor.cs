using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace UpdateTask
    {
    public class XmlConvertor
        {
        public static string ToXmlString(UpdatingFilesList tasks)
            {
            var serializer = new XmlSerializer(typeof(UpdatingFilesList));
            using (var stream = new MemoryStream())
                {
                serializer.Serialize(stream, tasks);
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream))
                    {
                    string text = reader.ReadToEnd();

                    return text;
                    }
                }
            }

        public static UpdatingFilesList ToListFromXmlString(string tasksXml)
            {
            try
                {
                var serializer = new XmlSerializer(typeof(UpdatingFilesList));

                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(tasksXml ?? "")))
                    {
                    var result = serializer.Deserialize(stream) as UpdatingFilesList;

                    return result;
                    }
                }
            catch (Exception exp)
                {
                Trace.WriteLine(exp.Message);

                return null;
                }
            }
        }
    }
