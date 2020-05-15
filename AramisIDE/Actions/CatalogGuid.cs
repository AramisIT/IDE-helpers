using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AramisIDE.Utils;

namespace AramisIDE.Actions
    {
    class CatalogGuid : BaseAction
        {
        public CatalogGuid()
            {
            ToClipboard(new GuidGenerator('A'));
            }
        }
    }
