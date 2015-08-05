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
            var passwords = File.ReadAllLines(string.Format(@"{0}\Passwords.txt", APPLICATION_PATH));
            foreach (var passwordLine in passwords)
                {
                if (string.IsNullOrEmpty(passwordLine.Trim())) continue;

                var parts = passwordLine.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2) continue;

                var description = parts[0].Trim();
                var password = parts[1];

                if (!result.ContainsKey(description))
                    {
                    result.Add(description, password);
                    }
                }

            return result;
            }
        }
    }
