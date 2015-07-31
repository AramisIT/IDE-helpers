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

            //new HttpFileUploader(@"http://localhost:50257/upload/SaveFile").UploadFile(@"C:\Users\Denis\AppData\Roaming\Aramis .NET\Greenhouse.AramisUTK.10.9.1.3_mssqlserver2008\GreenHouse.exe");

            ////new HttpFileUploader(@"http://localhost:50257/upload/SaveFile").UploadFile(@"X:\My plans\test.txt");
            //return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
            }
        }
    }
