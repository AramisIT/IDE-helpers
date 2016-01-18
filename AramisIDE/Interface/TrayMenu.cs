using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AramisIDE.Actions;
using AramisIDE.SolutionUpdating;
using AramisIDE.SourceCodeHelper;

namespace AramisIDE.Interface
    {
    class TrayMenu
        {
        enum MenuItems
            {
            MakeCatalog,
            MakeDocument,
            MakeCatalogGuid,
            MakeDocumentGuid,
            StartDateAndFinishDate,
            ShowProperties,
            CreateRestoreScript,
            Quit
            }

        private List<string> menuDescriptions = new List<string>() { 
        "Make catalog Win-Alt-C", 
        "Make document Win-Alt-L", 
        "Make catalog guid", 
        "Make document guid", 
        "StartDate and FinishDate",
        "Show properties",
        "Create restore script",
        "Quit" };

        private MainForm mainForm;
        private NotifyIcon trayIcon;
        private List<SolutionDetails> solutions;
        private SortedDictionary<string, string> passwords;

        public TrayMenu(MainForm mainForm, List<SolutionDetails> solutions, SortedDictionary<string, string> passwords)
            {
            this.mainForm = mainForm;
            this.solutions = solutions;
            this.passwords = passwords;
            createTrayIcon();
            }

        private void createTrayIcon()
            {
            trayIcon = new NotifyIcon();
            trayIcon.Icon = Properties.Resources.ProgramIcon;
            trayIcon.ContextMenu = createNotifyMenu();
            trayIcon.Visible = true;
            }

        private ContextMenu createNotifyMenu()
            {
            var menu = new ContextMenu();

            addHelpersItems(menu);

            addUpdateSolutionsItems(menu);

            addCopyToClipboardItems(menu);

            return menu;
            }

        private void addCopyToClipboardItems(ContextMenu menu)
            {
            var firstSolution = true;
            foreach (var kvp in passwords)
                {
                var newItem = menu.MenuItems.Add(kvp.Key);
                newItem.Tag = kvp.Value;

                newItem.Click += (sender, e) => copyToClipBoard((sender as MenuItem).Tag.ToString());

                if (firstSolution)
                    {
                    newItem.BarBreak = true;
                    firstSolution = false;
                    }
                }
            }

        private void copyToClipBoard(string text)
            {
            if (string.IsNullOrEmpty(text)) return;

            try
                {
                Clipboard.SetText(text);
                }
            catch (Exception exp)
                {
                Trace.WriteLine(exp.Message);
                }
            }

        private void addUpdateSolutionsItems(ContextMenu menu)
            {
            var firstSolution = true;
            foreach (var solutionDetails in solutions)
                {
                var newItem = menu.MenuItems.Add(string.Format(@"Update ""{0}""", solutionDetails.Name));
                newItem.Click += (sender, e) =>
                    {
                        var updater = new SolutionUpdater(solutionDetails);
                        if (!updater.CanStartUpdating())
                            {
                            MessageBox.Show("Last update isn't complated!",
                                string.Format(@"Update ""{0}""", solutionDetails.Name), MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                            return;
                            }

                        updater.Update();
                    };
                if (firstSolution)
                    {
                    newItem.BarBreak = true;
                    firstSolution = false;
                    }
                }

            if (solutions.Count > 0)
                {
                menu.MenuItems.Add("-");
                menu.MenuItems.Add("Log  >>  clipboard", (sender, e) =>
                    {
                        var message = UpdateLogger.Instance.ToString();
                        if (string.IsNullOrEmpty(message))
                            {
                            MessageBox.Show("Log hasn't data!");
                            return;
                            }
                        try
                            {
                            Clipboard.SetText(message);
                            }
                        catch
                            {
                            MessageBox.Show("Can't copy to clipboard!\r\n\r\n" + message);
                            }
                    });
                }

            menu.MenuItems.Add("Sql stored objects  >> source code Win-Shift-S", (sender, e) =>
                {
                    new PredefinedStoredObjectsUpdater().Update();
                });
           // menu.MenuItems.Add("Update hot keys", (sender, e) => new HotKeysManager(MainForm.Instance as MainForm));
            }

        private void addHelpersItems(ContextMenu menu)
            {
            foreach (var item in menuDescriptions)
                {
                var newItem = menu.MenuItems.Add(item);
                newItem.Click += (sender, e) =>
                    {
                        MenuItem menuItem = sender as MenuItem;
                        if (menuItem != null)
                            {
                            int itemIndex = menuDescriptions.IndexOf(menuItem.Text);
                            if (itemIndex >= 0)
                                {
                                onMenuItemClick((MenuItems)itemIndex);
                                }
                            }
                    };
                }
            }

        private void onMenuItemClick(MenuItems menuItem)
            {
            switch (menuItem)
                {
                case MenuItems.Quit:
                    CloseProgram();
                    return;

                case MenuItems.ShowProperties:
                    mainForm.ShowForm();
                    return;

                case MenuItems.CreateRestoreScript:
                    new RestoreScriptCreator();
                    return;

                case MenuItems.MakeCatalogGuid:
                    new CatalogGuid();
                    return;

                case MenuItems.MakeDocumentGuid:
                    new DocumentGuid();
                    return;

                case MenuItems.StartDateAndFinishDate:
                    new CurrentDayPeriod();
                    return;

                case MenuItems.MakeDocument:
                    new NewDocument();
                    return;

                case MenuItems.MakeCatalog:
                    new NewCatalog();
                    return;
                }
            }

        private void CloseProgram()
            {
            trayIcon.Visible = false;
            mainForm.Close();
            }
        }
    }
