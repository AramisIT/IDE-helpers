using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AramisIDE.Utils;

namespace AramisIDE
    {
    public partial class ImageViewer : Form
        {
        public ImageViewer()
            {
            InitializeComponent();
            }

        private void ImageViewer_Load(object sender, EventArgs e)
            {
            try
                {
                base64StringTextBox.Text = Clipboard.GetText() ?? string.Empty;
                }
            catch
                {
                }
            }

        private void button1_Click(object sender, EventArgs e)
            {
            pictureBox.Image = base64StringTextBox.Text.ToBitmap(Convert.ToInt32(widthTextBox.Text),
                Convert.ToInt32(heightTextBox.Text));
            }

        private void button2_Click(object sender, EventArgs e)
            {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            saveFileDialog.Title = "Save an Image File";
            saveFileDialog.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (saveFileDialog.FileName != "")
                {
                // Saves the Image via a FileStream created by the OpenFile method.  
                System.IO.FileStream fs =
                    (System.IO.FileStream)saveFileDialog.OpenFile();
                // Saves the Image in the appropriate ImageFormat based upon the  
                // File type selected in the dialog box.  
                // NOTE that the FilterIndex property is one-based.  
                switch (saveFileDialog.FilterIndex)
                    {
                    case 1:
                        pictureBox.Image.Save(fs,
                            System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;

                    case 2:
                        pictureBox.Image.Save(fs,
                            System.Drawing.Imaging.ImageFormat.Bmp);
                        break;

                    case 3:
                        pictureBox.Image.Save(fs,
                            System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                    }

                fs.Close();
                }
            }
        }
    }
