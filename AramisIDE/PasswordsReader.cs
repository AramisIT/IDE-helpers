using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AramisIDE
    {
    class PasswordsReader
        {
        public static readonly string APPLICATION_PATH = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);


        internal SortedDictionary<string, string> ReadPasswords()
            {
            var result = new SortedDictionary<string, string>();
            var fileName = string.Format(@"{0}\Passwords.txt", APPLICATION_PATH);
            if (!File.Exists(fileName)) return result;

            var passwords = File.ReadAllLines(fileName);
            foreach (var passwordLine in passwords)
                {
                if (string.IsNullOrEmpty(passwordLine.Trim())) continue;

                var separatorPos = passwordLine.IndexOf(';');
                if (separatorPos <= 0 || passwordLine.Length == (separatorPos + 1)) continue;

                var description = passwordLine.Substring(0, separatorPos).Trim();
                var password = passwordLine.Substring(separatorPos + 1);

                if (!result.ContainsKey(description))
                    {
                    result.Add(description, password);
                    }
                }

            return result;
            }
        }
    }
