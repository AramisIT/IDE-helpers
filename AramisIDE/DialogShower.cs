using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AramisIDE
    {
    class DialogShower : IDisposable
        {
        private Form form;
        private volatile bool disposed;

        public DialogShower()
            {
            form = new Form() { TopMost = true, Top = -1000, Left = -1000, ShowInTaskbar = false };
            form.StartPosition = FormStartPosition.Manual;
            form.Show();
            }

        public void Dispose()
            {
            if (disposed) return;

            form.Hide();
            form.Dispose();

            disposed = true;
            }
        }
    }
