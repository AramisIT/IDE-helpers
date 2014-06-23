using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AramisIDE.Actions;
using AramisIDE.Utils;

namespace AramisIDE
    {
    class HotKeysManager
        {
        private MainForm mainForm;
        private KeyboardHook keyboardHook;

        public HotKeysManager(MainForm mainForm)
            {
            this.mainForm = mainForm;

            keyboardHook = new KeyboardHook();
            createHotKeys();
            }

        private void createHotKeys()
            {
            keyboardHook.RegisterHotKey(ModifierKeys.Win | ModifierKeys.Alt, Keys.C);
            keyboardHook.RegisterHotKey(ModifierKeys.Win | ModifierKeys.Alt, Keys.L);
            keyboardHook.KeyPressed += keyboardHook_KeyPressed;
            }

        void keyboardHook_KeyPressed(object sender, KeyPressedEventArgs e)
            {
            switch (e.Key)
                {
                case Keys.C:
                    new NewCatalog();
                    break;

                case Keys.L:
                    new NewDocument();
                    break;
                }
            }
        }
    }
