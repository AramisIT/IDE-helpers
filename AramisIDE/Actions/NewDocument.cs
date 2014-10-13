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
            if (clipboard.Length > 100 || String.IsNullOrEmpty(clipboard) || clipboard.Contains(" "))
                {
                tableName = "ITEM_NAME";
                }
            else
                {
                tableName = clipboard;
                }

            var guid = new GuidGenerator('0').NewGuid;
            var doc = string.Format(Properties.Resources.DocumentTemplate, guid, tableName);
            ToClipboard(doc);
            }
        }
    }
