using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AramisIDE.Actions;
using AramisIDE.SourceCodeHelper;
using AramisIDE.Utils;

namespace AramisIDE
    {
    class HotKeysManager
        {
        private KeyboardHook keyboardHook;

        public HotKeysManager()
            {
            keyboardHook = new KeyboardHook();
            createHotKeys();
            }

        private void createHotKeys()
            {
            keyboardHook.RegisterHotKey(ModifierKeys.Win | ModifierKeys.Alt, Keys.C);
            keyboardHook.RegisterHotKey(ModifierKeys.Win | ModifierKeys.Alt, Keys.L);
            keyboardHook.RegisterHotKey(ModifierKeys.Win | ModifierKeys.Shift, Keys.S);

            keyboardHook.RegisterHotKey(ModifierKeys.Win | ModifierKeys.Alt, Keys.O);
            keyboardHook.RegisterHotKey(ModifierKeys.Win | ModifierKeys.Alt, Keys.P);

            keyboardHook.KeyPressed += keyboardHook_KeyPressed;
            }

        void keyboardHook_KeyPressed(object sender, KeyPressedEventArgs e)
            {
            switch (e.Key)
                {
                case Keys.O:
                    DisplayBrightness.ReduceBrightness();
                    break;

                case Keys.P:
                    DisplayBrightness.IncreaseBrightness();
                    break;

                case Keys.C:
                    new NewCatalog();
                    break;

                case Keys.L:
                    new NewDocument();
                    break;

                case Keys.S:
                    if ((e.Modifier & ModifierKeys.Shift) == 0) return;

                    new PredefinedStoredObjectsUpdater().Update();
                    break;
                }
            }
        }
    }
