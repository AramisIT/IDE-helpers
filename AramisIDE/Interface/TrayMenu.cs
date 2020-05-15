using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;
using AramisIDE.Actions;
using AramisIDE.SolutionUpdating;
using AramisIDE.SourceCodeHelper;
using ScreenBrightness;

namespace AramisIDE.Interface
    {
    class TrayMenu
        {
        enum MenuItems
            {
            CopyIpAddress,
            MakeCatalog,
            MakeDocument,
            //MakeCatalogGuid,
            //MakeDocumentGuid,
            StartDateAndFinishDate,
            CreateRestoreScript,
            //ShowGrayImageFromBase64String,
            Quit
            }

        private readonly List<string> menuDescriptions;

        private static string getWirelessAddressCommand()
            {
            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211
                    && !networkInterface.Description.ToLower().Contains("virtual")
                    && networkInterface.OperationalStatus == OperationalStatus.Up
                    )
                    {
                    foreach (UnicastIPAddressInformation ip in networkInterface.GetIPProperties().UnicastAddresses)
                        {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                            return ip.Address.ToString();
                            }
                        }
                    }
                }

            return null;
            }

        private NotifyIcon trayIcon;
        private List<SolutionDetails> solutions;
        private SortedDictionary<string, string> passwords;
        private Action quitApplication;
        private string wirelessIdAddress;
        private int menuCommandsIndexOffset;
        private MenuItem copyAddressMenuItem;
        private Action<Action> executeInMainThread;

        public TrayMenu(List<SolutionDetails> solutions, SortedDictionary<string, string> passwords, Action quitApplication,
                Action<Action> executeInMainThread)
            {
            menuDescriptions = getBaseMenuCommands();

            this.solutions = solutions;
            this.passwords = passwords;
            this.quitApplication = quitApplication;
            this.executeInMainThread = executeInMainThread;
            createTrayIcon();
            }

        private List<string> getBaseMenuCommands()
            {
            var result = new List<string>() {
        "Make catalog Win-Alt-C",
        "Make document Win-Alt-L",
        //"Make catalog guid",
        //"Make document guid",
        "StartDate and FinishDate",
        "Create restore script",
        //"Show gray image",
        "Quit" };

            result.Insert(0, getCopyWirelessAddressCommand());

            //if (!string.IsNullOrEmpty(wirelessIdAddress))
            //    {
            //    result.Insert(0, string.Format("Copy {0}", wirelessIdAddress));
            //    }
            //else
            //    {
            //    menuCommandsIndexOffset = 1;
            //    }
            return result;
            }

        private string getCopyWirelessAddressCommand()
            {
            wirelessIdAddress = getWirelessAddressCommand();
            return string.IsNullOrEmpty(wirelessIdAddress) ? "< No wireless >" : string.Format("Copy {0}", wirelessIdAddress);
            }

        private void createTrayIcon()
            {
            trayIcon = new NotifyIcon();
            trayIcon.Icon = Properties.Resources.ProgramIcon;
            trayIcon.ContextMenu = createNotifyMenu();
            trayIcon.Visible = true;
            trayIcon.MouseClick += trayIcon_MouseClick;
            }

        private Form brightnessForm;
        private bool showMenuOnLeftClick;

        private void showMenu()
            {
            System.Reflection.MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            mi.Invoke(trayIcon, null);
            }

        private void trayIcon_MouseClick(object sender, MouseEventArgs e)
            {
            if (e.Button == MouseButtons.Left)
                {
                if (showMenuOnLeftClick)
                    {
                    showMenu();
                    return;
                    }
                if (brightnessForm != null && !brightnessForm.IsDisposed)
                    {
                    brightnessForm.Close();
                    }
                else
                    {
                    var brightnessForm = new BrightnessForm();
                    var initException = brightnessForm.Init();
                    if (initException == null)
                        {
                        brightnessForm = brightnessForm;
                        ((BrightnessForm)brightnessForm).ShowEditor();
                        }
                    else
                        {
                        showMenuOnLeftClick = true;
                        showMenu();
                        }
                    }
                }
            }

        private ContextMenu createNotifyMenu()
            {
            var menu = new ContextMenu();

            addHelpersItems(menu);

            var ipCheckerTimer = new Timer() { Interval = 1000 * 10 };
            ipCheckerTimer.Tick += ipCheckerTimer_Tick;
            ipCheckerTimer.Start();

            addUpdateSolutionsItems(menu);
            addCopyToClipboardItems(menu);

            return menu;
            }

        void ipCheckerTimer_Tick(object sender, EventArgs e)
            {
            executeInMainThread(() => copyAddressMenuItem.Text = getCopyWirelessAddressCommand());
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

            menu.MenuItems.Add("Hide migrations script", (sender, e) =>
                {
                Clipboard.SetText(HideMigrationsReader.Script);
                });

            // menu.MenuItems.Add("Update hot keys", (sender, e) => new HotKeysManager(MainForm.Instance as MainForm));
            }

        private void addHelpersItems(ContextMenu menu)
            {
            var isCopyAddressMenuItem = true;
            foreach (var item in menuDescriptions)
                {
                var newItem = menu.MenuItems.Add(item);
                if (isCopyAddressMenuItem)
                    {
                    isCopyAddressMenuItem = false;
                    copyAddressMenuItem = newItem;

                    newItem.Click += (sender, e) =>
                        {
                            Clipboard.SetText(getWirelessAddressCommand() ?? "<connection isn't detected!>");
                        };
                    }
                else
                    {
                    newItem.Click += (sender, e) =>
                        {
                            MenuItem menuItem = sender as MenuItem;
                            if (menuItem != null)
                                {
                                int itemIndex = menuDescriptions.IndexOf(menuItem.Text);
                                if (itemIndex >= 0)
                                    {
                                    onMenuItemClick((MenuItems)(menuCommandsIndexOffset + itemIndex));
                                    }
                                }
                        };
                    }
                }
            }

        private void onMenuItemClick(MenuItems menuItem)
            {
            switch (menuItem)
                {
                case MenuItems.CopyIpAddress:
                    Clipboard.SetText(wirelessIdAddress);
                    return;

                case MenuItems.Quit:
                    closeProgram();
                    return;

                case MenuItems.CreateRestoreScript:
                    new RestoreScriptCreator();
                    return;

                //case MenuItems.ShowGrayImageFromBase64String:
                //    new ImageViewer().Show();
                //    return;

                //case MenuItems.MakeCatalogGuid:
                //    new CatalogGuid();
                //    return;

                //case MenuItems.MakeDocumentGuid:
                //    new DocumentGuid();
                //    return;

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

        private void closeProgram()
            {
            if (quitApplication != null)
                {
                trayIcon.Visible = false;
                quitApplication();
                }
            }
        }
    }
