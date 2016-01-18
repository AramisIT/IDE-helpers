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
        public static Form Instance { get; private set; }

        public MainForm()
            {
            InitializeComponent();
            solutions = new SolutionsReader().ReadSolutions();
            passwords = new PasswordsReader().ReadPasswords();
            new TrayMenu(this, solutions, passwords);
            Instance = this;
            updateHotKeys();
            }

        private void MainForm_Load(object sender, EventArgs e)
            {

            }

        private bool showing;
        private List<SolutionDetails> solutions;
        private SortedDictionary<string, string> passwords;

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

        private void button1_Click(object sender, EventArgs e)
            {
            hide();
            }

        private void hotKeysUpdater_Tick(object sender, EventArgs e)
            {
            if (InvokeRequired)
                {
                Invoke(new Action(updateHotKeys));
                }
            else
                {
                updateHotKeys();
                }
            }

        private void updateHotKeys()
            {
            try
                {
                new HotKeysManager(this);
                }
            catch { }
            }
        }
    }
