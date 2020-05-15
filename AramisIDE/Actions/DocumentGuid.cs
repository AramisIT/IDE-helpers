using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AramisIDE.Utils;

namespace AramisIDE.Actions
    {
    class DocumentGuid : BaseAction
        {
        public DocumentGuid()
            {
            ToClipboard(new GuidGenerator('0'));
            }
        }
    }
