using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AramisIDE.Utils;

namespace AramisIDE.Actions
    {
    class NewDocument : BaseAction
        {
        public NewDocument()
            {
            string tableName = string.Empty;
            var clipboard = FromClipboard();
            if (clipboard.EndsWith("*"))
                {
                tableName = clipboard.Substring(0, clipboard.Length - 1).Trim();
                }

            var guid = new GuidGenerator('0').NewGuid;
            var doc = string.Format(Properties.Resources.DocumentTemplate, guid, tableName);
            ToClipboard(doc);
            }
        }
    }
