namespace ScreenBrightness
    {
    partial class BrightnessForm
        {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
            {
            if (disposing && (components != null))
                {
                components.Dispose();
                }
            base.Dispose(disposing);
            }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
            {
            this.trackBar = new ScreenBrightness.TrackbarWithoutBorder();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBar
            // 
            this.trackBar.LargeChange = 1;
            this.trackBar.Location = new System.Drawing.Point(36, 4);
            this.trackBar.Maximum = 100;
            this.trackBar.Name = "trackBar";
            this.trackBar.Size = new System.Drawing.Size(217, 45);
            this.trackBar.TabIndex = 0;
            this.trackBar.TickFrequency = 10;
            this.trackBar.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::AramisIDE.Properties.Resources.brightness;
            this.pictureBox1.Location = new System.Drawing.Point(1, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(34, 32);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // BrightnessForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(258, 40);
            this.ControlBox = false;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.trackBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BrightnessForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TopMost = true;
            this.Deactivate += new System.EventHandler(this.BrightnessForm_Deactivate);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

            }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private TrackbarWithoutBorder trackBar;
        }
    }

