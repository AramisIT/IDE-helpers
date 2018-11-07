using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AramisIDE
    {
    class HideMigrationsReader
        {
        private static string script = null;

        public static string Script
            {
            get
                {
                if (script == null)
                    {
                    script = readScript();
                    }
                return script;
                }
            }

        private static string readScript()
            {
            var fileName = string.Format(@"{0}\HideMigrations.txt", PasswordsReader.APPLICATION_PATH);
            if (!File.Exists(fileName)) return string.Empty;

            return File.ReadAllText(fileName);
            }
        }
    }
