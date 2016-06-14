using System;
using AramisIDE.Interface;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace AramisIDE
    {
    internal class AramisIDEApplicationContext : ApplicationContext
        {
        private List<SolutionDetails> solutions;
        private SortedDictionary<string, string> passwords;
        private SynchronizationContext mainSynchronizationContext;

        public AramisIDEApplicationContext()
            {
            solutions = new SolutionsReader().ReadSolutions();
            passwords = new PasswordsReader().ReadPasswords();
            new TrayMenu(solutions, passwords, base.ExitThreadCore, executeInMainThread);
            updateHotKeys();
            }

        private void updateHotKeys()
            {
            try
                {
                new HotKeysManager();
                }
            catch { }
            }

        internal void OnApplicationIdle(object sender, System.EventArgs e)
            {
            Application.Idle -= OnApplicationIdle;

            mainSynchronizationContext = SynchronizationContext.Current;

            new Thread(() =>
            {
                while (true)
                    {
                    executeInMainThread(updateHotKeys);
                    Thread.Sleep(5000);
                    }
            }) { IsBackground = true }.Start();
            }

        private void executeInMainThread(Action action)
            {
            if (action == null) return;

            mainSynchronizationContext.Send((obj) => action(), null);
            }
        }
    }
