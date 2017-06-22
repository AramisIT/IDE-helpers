using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AramisIDE.Actions
    {
    class RestoreScriptCreator
        {
        private string bufferData;

        public RestoreScriptCreator()
            {
            bufferData = Clipboard.GetText().Trim();
            if (!string.IsNullOrEmpty(bufferData) && bufferData.Length >= 3)
                {
                createScript();
                }
            }

        private void createScript()
            {
            var databaseName = "";
            var path = "";

            if (bufferData.Substring(1, 2) == @":\")
                {
                var parts = bufferData.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                    {
                    databaseName = parts[parts.Length - 1];
                    path = bufferData.Substring(0, bufferData.Length - databaseName.Length).Trim();
                    }
                else
                    {
                    databaseName = "DATABASE_NAME";
                    path = bufferData;
                    }
                }
            else
                {
                databaseName = bufferData;
                path = "<path to back up>";
                }

            Clipboard.SetText(string.Format(AramisIDE.Properties.Resources.RestoreSqlCommand, databaseName, path, SolutionsReader.AramisSqlUser));
            }
        }
    }
