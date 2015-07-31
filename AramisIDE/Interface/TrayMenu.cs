using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AramisIDE.Actions;
using AramisIDE.SolutionUpdating;

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

        public TrayMenu(MainForm mainForm, List<SolutionDetails> solutions)
            {
            this.mainForm = mainForm;
            this.solutions = solutions;
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

            var firstSolution = true;
            foreach (var solutionDetails in solutions)
                {
                var newItem = menu.MenuItems.Add(string.Format(@"Update ""{0}""", solutionDetails.Name));
                newItem.Click += (sender, e) => new SolutionUpdater(solutionDetails).Update();
                if (firstSolution)
                    {
                    newItem.BarBreak = true;
                    firstSolution = false;
                    }
                }

            return menu;
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
