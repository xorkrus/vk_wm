using System;
using System.Drawing;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.Properties;

namespace Galssoft.VKontakteWM.CustomControlls
{
    partial class UpdateInfoDialogControl
    {
        /// <summary> 
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс  следует удалить; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Обязательный метод для поддержки конструктора - не изменяйте 
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonUpdate = new UIButton(ButtonStyle.AlphaChannel);
            this.buttonSkip = new UIButton(ButtonStyle.AlphaChannel);
            this.buttonCancel = new UIButton(ButtonStyle.AlphaChannel);

            // 
            // buttonUpdate
            // 
            buttonUpdate.Location = new Point(8, 20);
            buttonUpdate.Size = new Size(100, 25);
            buttonUpdate.Text = Resources.UpdateInfoDialogControl_Buttons_Update;
            buttonUpdate.Click += new EventHandler(buttonUpdate_Click);
            buttonUpdate.Font = FontCache.CreateFont("Tahoma", 11, FontStyle.Bold);
            buttonUpdate.FontColor = Color.Black;
            buttonUpdate.PressedFontColor = Color.White;

            // 
            // buttonSkip
            // 
            buttonSkip.Location = new Point(8, 40);
            buttonSkip.Size = new Size(100, 25);
            buttonSkip.Text = Resources.UpdateInfoDialogControl_Buttons_Skip;
            buttonSkip.Click += new EventHandler(buttonSkip_Click);
            buttonSkip.Font = FontCache.CreateFont("Tahoma", 11, FontStyle.Bold);
            buttonSkip.FontColor = Color.Black;
            buttonSkip.PressedFontColor = Color.White;

            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(8, 60);
            buttonCancel.Size = new Size(100, 25);
            buttonCancel.Text = string.Empty;
            buttonCancel.Click += new EventHandler(buttonCancel_Click);
            buttonCancel.Font = FontCache.CreateFont("Tahoma", 11, FontStyle.Bold);
            buttonCancel.FontColor = Color.Black;
            buttonCancel.PressedFontColor = Color.White;

            // 
            // PasswordRestoreDialogControl
            // 
            this.Size = new Size(240, 268);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Name = "UpdateInfoDialogControl";

            this.Canvas.Children.Add(this.buttonUpdate);
            this.Canvas.Children.Add(this.buttonSkip);
            this.Canvas.Children.Add(this.buttonCancel);

            this.Canvas.RecalcDPIScaling();

            this.ResumeLayout(false);
        }

        #endregion

        private UIButton buttonUpdate;
        private UIButton buttonSkip;
        private UIButton buttonCancel;
    }
}
