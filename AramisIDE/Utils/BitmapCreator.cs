using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace AramisIDE.Utils
    {
    public static class BitmapCreator
        {
        public static unsafe Bitmap ToBitmap(this string base64String, int width, int height)
            {
            byte[] dataSource = Convert.FromBase64String(base64String);

            var bitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            ColorPalette ncp = bitmap.Palette;
            for (int i = 0; i < 256; i++)
                ncp.Entries[i] = Color.FromArgb(255, i, i, i);
            bitmap.Palette = ncp;

            var rect = new Rectangle(0, 0, width, height);
            BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, bitmap.PixelFormat);

            unsafe
                {
                byte* dst = (byte*)bmpData.Scan0.ToPointer();
                var bitmapOffset = bmpData.Stride - bmpData.Width;
                int sourceIndex = 0;
                unchecked
                    {
                    for (int y = 0; y < height; y++)
                        {
                        for (int x = 0; x < width; x++, dst++, sourceIndex += 1)
                            {
                            int luminance = dataSource[sourceIndex] & 0xFF;
                            *dst = (byte)(luminance);
                            }
                        dst += bitmapOffset;
                        }
                    }
                }

            bitmap.UnlockBits(bmpData);

            return bitmap;
            }
        }
    }
