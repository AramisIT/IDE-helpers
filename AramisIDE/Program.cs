using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AramisIDE.Utils.FileUploader;

namespace AramisIDE
    {
    static class Program
        {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
            {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var context = new AramisIDEApplicationContext();
            Application.Idle += context.OnApplicationIdle;
            Application.Run(context);
            }
        }
    }
