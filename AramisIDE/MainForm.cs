using AramisIDE.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AramisIDE
    {
    public partial class MainForm : Form
        {
        public MainForm()
            {
            InitializeComponent();
            new TrayMenu(this);
            new HotKeysManager(this);
            }

        private void MainForm_Load(object sender, EventArgs e)
            {

            }

        private bool showing;

        internal void ShowForm()
            {
            showing = true;
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            showing = false;
            }

        private void hide()
            {
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            }

        private void MainForm_Resize(object sender, EventArgs e)
            {
            if (showing) return;

            if (ShowInTaskbar)
                {
                hide();
                }
            }
        }
    }
