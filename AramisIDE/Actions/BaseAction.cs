using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AramisIDE.Actions
    {
    abstract class BaseAction
        {
        protected void ToClipboard(object objectValue)
            {
            if (objectValue != null)
                {
                Clipboard.SetText(objectValue.ToString());
                }
            }

        protected string FromClipboard()
            {
            return Clipboard.GetText() ?? string.Empty;
            }
        }
    }
