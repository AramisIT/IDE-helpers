using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AramisIDE.Utils
    {
    public class DisplayBrightness
        {
        public static int GetBrightness()
            {
            //define scope (namespace)
            System.Management.ManagementScope s = new System.Management.ManagementScope("root\\WMI");

            //define query
            System.Management.SelectQuery q = new System.Management.SelectQuery("WmiMonitorBrightness");

            //output current brightness
            System.Management.ManagementObjectSearcher mos = new System.Management.ManagementObjectSearcher(s, q);

            System.Management.ManagementObjectCollection moc = mos.Get();

            //store result
            byte curBrightness = 0;
            foreach (System.Management.ManagementObject o in moc)
                {
                curBrightness = (byte)o.GetPropertyValue("CurrentBrightness");
                break; //only work on the first object
                }

            moc.Dispose();
            mos.Dispose();

            return (int)curBrightness;
            }

        //array of valid brightness values in percent
        public static byte[] GetBrightnessLevels(out Exception exception)
            {
            //define scope (namespace)
            System.Management.ManagementScope s = new System.Management.ManagementScope("root\\WMI");

            //define query
            System.Management.SelectQuery q = new System.Management.SelectQuery("WmiMonitorBrightness");

            //output current brightness
            System.Management.ManagementObjectSearcher mos = new System.Management.ManagementObjectSearcher(s, q);
            byte[] BrightnessLevels = new byte[0];
            exception = null;

            try
                {
                System.Management.ManagementObjectCollection moc = mos.Get();

                //store result


                foreach (System.Management.ManagementObject o in moc)
                    {
                    BrightnessLevels = (byte[])o.GetPropertyValue("Level");
                    break; //only work on the first object
                    }

                moc.Dispose();
                mos.Dispose();

                }
            catch (Exception exp)
                {
                exception = exp;// new Exception("Sorry, Your System does not support this brightness control...");
                }

            return BrightnessLevels;
            }

        public static void SetBrightness(byte targetBrightness)
            {
            //define scope (namespace)
            System.Management.ManagementScope s = new System.Management.ManagementScope("root\\WMI");

            //define query
            System.Management.SelectQuery q = new System.Management.SelectQuery("WmiMonitorBrightnessMethods");

            //output current brightness
            System.Management.ManagementObjectSearcher mos = new System.Management.ManagementObjectSearcher(s, q);

            System.Management.ManagementObjectCollection moc = mos.Get();

            foreach (System.Management.ManagementObject o in moc)
                {
                o.InvokeMethod("WmiSetBrightness", new Object[] { UInt32.MaxValue, targetBrightness }); //note the reversed order - won't work otherwise!
                break; //only work on the first object
                }

            moc.Dispose();
            mos.Dispose();
            }

        public static void IncreaseBrightness()
            {
            changeBrightnessLevel(true);
            }

        private static void changeBrightnessLevel(bool increase)
            {
            Exception exp;
            var levels = GetBrightnessLevels(out exp).ToList();
            byte level = (byte)GetBrightness();
            var currentIndex = levels.IndexOf(level);
            if (currentIndex == (increase ? levels.Count - 1 : 0)) return;

            SetBrightness(levels[currentIndex + (increase ? 1 : -1)]);
            }

        public static void ReduceBrightness()
            {
            changeBrightnessLevel(false);
            }
        }
    }
