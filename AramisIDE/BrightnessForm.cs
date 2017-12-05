using AramisIDE.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ScreenBrightness
    {
    public partial class BrightnessForm : Form
        {
        private List<byte> brightnessOptions;

        public BrightnessForm()
            {
            InitializeComponent();
            }

        public Exception Init()
            {
            try
                {
                Exception exp;
                brightnessOptions = DisplayBrightness.GetBrightnessLevels(out exp).ToList(); //get the level array for this system
                if (exp != null)
                    {
                    return exp;
                    }
                if (brightnessOptions.Count > 0) //"WmiMonitorBrightness" is not supported by the system
                    {
                    trackBar.TickFrequency = brightnessOptions.Count / 10; //adjust the trackbar ticks according the number of possible brightness levels
                    trackBar.Maximum = brightnessOptions.Count - 1;
                    check_brightness();
                    }
                }
            catch (Exception exp)
                {
                return exp;
                }

            return null;
            }

        private void check_brightness()
            {
            int iBrightness = DisplayBrightness.GetBrightness(); //get the actual value of brightness
            int i = brightnessOptions.IndexOf((byte)iBrightness);
            if (i < 0) i = 1;
            trackBar.Value = i;
            }

        private void trackBar1_Scroll(object sender, EventArgs e)
            {
            DisplayBrightness.SetBrightness(brightnessOptions[trackBar.Value]);
            }

        internal void ShowEditor()
            {
            Point p = new Point(MousePosition.X, MousePosition.Y);
            Rectangle r = Screen.GetBounds(p);
            if (p.X > r.Width / 2)
                {
                if (p.X + 140 > r.Width)
                    this.Left = r.Width - 275;
                else
                    this.Left = p.X - 140;
                }
            else
                this.Left = p.X;

            if (p.Y > r.Height / 2)
                this.Top = p.Y - 60;
            else
                this.Top = p.Y;

            check_brightness();

            this.Show();
            this.Activate();
            }

        private void BrightnessForm_Deactivate(object sender, EventArgs e)
            {
            Close();
            }
        }

    class TrackbarWithoutBorder : TrackBar
        {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public extern static int SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        private static int MakeParam(int loWord, int hiWord)
            {
            return (hiWord << 16) | (loWord & 0xffff);
            }

        protected override void OnGotFocus(EventArgs e)
            {
            base.OnGotFocus(e);
            SendMessage(this.Handle, 0x0128, MakeParam(1, 0x1), 0);
            }
        }
    }
