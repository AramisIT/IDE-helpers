using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AramisIDE.Utils;

namespace AramisIDE.Actions
    {
    class NewCatalog : BaseAction
        {
        public NewCatalog()
            {
            string tableName = string.Empty;
            var clipboard = FromClipboard();
            if (clipboard.EndsWith("*"))
                {
                tableName = clipboard.Substring(0, clipboard.Length - 1).Trim();
                }

            var guid = new GuidGenerator('A').NewGuid;
            var doc = string.Format(Properties.Resources.CatalogTemplate, guid, tableName);
            ToClipboard(doc);
            }
        }
    }
