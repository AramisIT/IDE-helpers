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
        public static string ToXmlString<T>(T tasks) where T : class
            {
            if (tasks == null) return string.Empty;

            var serializer = new XmlSerializer(typeof(T));
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

        public static T ToObjectFromXmlString<T>(string tasksXml) where T : class
            {
            if (string.IsNullOrEmpty(tasksXml))
                {
                return null;
                }

            try
                {
                var serializer = new XmlSerializer(typeof(T));

                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(tasksXml ?? "")))
                    {
                    var result = (T)serializer.Deserialize(stream);

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
