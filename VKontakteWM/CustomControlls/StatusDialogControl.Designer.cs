using System;
using System.Drawing;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Forms;

namespace Galssoft.VKontakteWM.CustomControlls
{
    partial class StatusDialogControl
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
            this.buttonOk = new UIButton(ButtonStyle.AlphaChannel);
            this.buttonReset = new UIButton(ButtonStyle.AlphaChannel);
            this.buttonCancel = new UIButton(ButtonStyle.AlphaChannel);
            this.textBoxStatus = new UITextControl();

            this.SuspendLayout();
            // 
            // textBoxStatus
            // 
            this.textBoxStatus.Location = new Point(8, 0);
            this.textBoxStatus.Size = new Size(100, 25);
            this.textBoxStatus.Name = "textBoxStatus";
            this.textBoxStatus.BackColor = Color.White;
            this.textBoxStatus.Font = FontCache.CreateFont("Calibri", 18, FontStyle.Regular, true);//12
            //TODO через Init
            this.textBoxStatus.SetImages(MasterForm.SkinManager.GetImage("UITextBoxLeftBorder"), MasterForm.SkinManager.GetImage("UITextBoxRightBorder"),
                MasterForm.SkinManager.GetImage("UITextBoxTopBorder"), MasterForm.SkinManager.GetImage("UITextBoxBottomBorder"));
            this.textBoxStatus.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            // 
            // buttonOk
            // 
            buttonOk.Location = new Point(8, 20);
            buttonOk.Size = new Size(100, 25);
            buttonOk.Text = "ОК";
            buttonOk.Click += new EventHandler(buttonOk_Click);
            // 
            // buttonReset
            // 
            buttonReset.Location = new Point(8, 40);
            buttonReset.Size = new Size(100, 25);
            buttonReset.Text = "Сбросить";
            buttonReset.Click += new EventHandler(buttonReset_Click);
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(8, 60);
            buttonCancel.Size = new Size(150, 25);
            buttonCancel.Text = "Отмена";
            buttonCancel.Click += new EventHandler(buttonCancel_Click);
            // 
            // StatusDialogControl
            // 
            this.Size = new Size(240, 268);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Canvas.Children.Add(this.buttonOk);
            this.Canvas.Children.Add(this.buttonReset);
            this.Canvas.Children.Add(this.buttonCancel);
            this.Canvas.Children.Add(textBoxStatus);
            this.Name = "StatusDialogControl";
            this.Canvas.RecalcDPIScaling();
            this.ResumeLayout(false);
        }

        #endregion

        private UIButton buttonOk;
        private UIButton buttonReset;
        private UIButton buttonCancel;
        private UITextControl textBoxStatus;
    }
}
