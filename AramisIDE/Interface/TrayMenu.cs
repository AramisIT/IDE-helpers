using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AramisIDE.Actions;

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
            Quit
            }

        private List<string> menuDescriptions = new List<string>() { 
        "Make catalog Win-Alt-C", 
        "Make document Win-Alt-L", 
        "Make catalog guid", 
        "Make document guid", 
        "StartDate and FinishDate",
        "Show properties",
        "Quit" };

        private MainForm mainForm;
        private NotifyIcon trayIcon;

        public TrayMenu(MainForm mainForm)
            {
            this.mainForm = mainForm;
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
