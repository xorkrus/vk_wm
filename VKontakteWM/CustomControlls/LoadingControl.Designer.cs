using System;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.UI.Controls;

namespace Galssoft.VKontakteWM.CustomControls
{
    partial class LoadingControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new UIButton(ButtonStyle.AlphaChannel);
            this.progressBar = new VKontakteWM.Components.UI.Controls.ExtendedProgressBar();

            this.SuspendLayout();

            // 
            // DialogControl
            // 
            this.Size = new Size(240, 268);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Name = "LoadingControl";
            // 
            // button1
            // 
            button1.Location = new Point(8, 0);
            button1.Size = new Size(70, 24);
            button1.Click += new EventHandler(button1_Click);
            //
            // ProgressBar
            //
            progressBar.Location = new Point(UISettings.CalcPix(22), UISettings.CalcPix(122));
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(UISettings.CalcPix(98), UISettings.CalcPix(8));
            progressBar.Maximum = 100;

            this.Canvas.Children.Add(this.button1);
            this.Canvas.Children.Add(this.progressBar);
            this.Canvas.RecalcDPIScaling();
            this.ResumeLayout(false);
        }

        #endregion

        private VKontakteWM.Components.UI.Controls.ExtendedProgressBar progressBar;
        private UIButton button1;
    }
}
