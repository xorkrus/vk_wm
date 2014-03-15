using System;
using System.Drawing;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.UI.Controls;

namespace Galssoft.VKontakteWM.CustomControlls
{
    partial class DialogControl
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
            ReleaseResources();
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new UIButton(ButtonStyle.AlphaChannel);
            this.button2 = new UIButton(ButtonStyle.AlphaChannel);
            this.button3 = new UIButton(ButtonStyle.AlphaChannel);


            this.SuspendLayout();

            // 
            // DialogControl
            // 
            this.Size = new Size(240, 268);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Name = "DialogControl";
            // 
            // button1
            // 
            button1.Location = new Point(8, 0);
            button1.Size = new Size(70, 24);
            button1.Click += new EventHandler(button1_Click);
            // 
            // button2
            // 
            button2.Location = new Point(8, 0);
            button2.Size = new Size(70, 24);
            button2.Click += new EventHandler(button2_Click);
            // 
            // button3
            // 
            button3.Location = new Point(8, 0);
            button3.Size = new Size(70, 24);
            button3.Click += new EventHandler(button3_Click);

            this.Canvas.Children.Add(this.button1);
            this.Canvas.Children.Add(this.button2);
            this.Canvas.Children.Add(this.button3);

            this.Canvas.RecalcDPIScaling();
            this.ResumeLayout(false);
        }

        #endregion

        private UIButton button1;
        private UIButton button2;
        private UIButton button3;
    }
}