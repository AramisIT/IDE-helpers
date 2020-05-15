using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AramisIDE
    {
    public class UpdateLogger
        {
        private static UpdateLogger instance;
        private static volatile object locker = new object();

        public static UpdateLogger Instance
            {
            get
                {
                if (instance == null)
                    {
                    lock (locker)
                        {
                        if (instance == null)
                            {
                            instance = new UpdateLogger();
                            }
                        }
                    }
                return instance;
                }
            }

        private UpdateLogger()
            {

            }

        private StringBuilder stringBuilder = new StringBuilder();

        public void Append(string message, string header = null)
            {
            lock (locker)
                {
                stringBuilder.AppendLine(DateTime.Now.ToString("HH:mm:ss") + " "
                                         + (header ?? string.Empty)
                                         + (string.IsNullOrEmpty(header) ? "" : " - ")
                                         + message);
                }
            }

        public void Reset()
            {
            lock (locker)
                {
                stringBuilder.Clear();
                }
            }

        public override string ToString()
            {
            lock (locker)
                {
                return stringBuilder.ToString();
                }
            }
        }
    }
