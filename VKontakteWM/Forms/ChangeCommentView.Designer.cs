using System.Runtime.InteropServices;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.Controls;
using System.Windows.Forms;
using System.Drawing;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Properties;
using System;
using AnchorStyles = Galssoft.VKontakteWM.Components.UI.AnchorStyles;
using HorizontalAlignment = Galssoft.VKontakteWM.Components.UI.HorizontalAlignment;
using Galssoft.VKontakteWM.Components.Common.Configuration;

namespace Galssoft.VKontakteWM.Forms
{
    partial class ChangeCommentView
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
            this.mainMenu = new MainMenu();
            this.menuItemCancel = new MenuItem();
            this.menuItemChange = new MenuItem();
            this.buttonClearText = new UIButton(ButtonStyle.AlphaChannel);
            this.tbxCommentText = new TextBox();
            this.header = new GraphicsImage();
            this.headerText = new UILabel();
            this.headerShadowText = new UILabel();
            this.SuspendLayout();

            //
            // header
            //
            this.header.Name = "header";
            this.header.Location = new Point(0, 0);
            this.header.Stretch = true;
            this.header.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            //
            // headerText
            //
            this.headerText.Name = "headerText";
            this.headerText.Location = new Point(0, 0);
            this.headerText.Size = new Size(240, 36);
            this.headerText.Font = FontCache.CreateFont("Tahoma", 19, FontStyle.Regular);
            this.headerText.ForeColor = Color.White;
            this.headerText.VerticalTextAlignment = VerticalAlignment.Center;
            this.headerText.HorizontalTextAlignment = HorizontalAlignment.Center;
            this.headerText.Text = Resources.ChangeCommentView_Title;
            this.headerText.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            //
            // headerShadowText
            //
            this.headerShadowText.Name = "headerShadowText";
            this.headerShadowText.Location = new Point(0, 0);
            this.headerShadowText.Size = new Size(240, 36);
            this.headerShadowText.Font = FontCache.CreateFont("Tahoma", 19, FontStyle.Regular);
            this.headerShadowText.ForeColor = Color.FromArgb(180, 180, 180);
            this.headerShadowText.VerticalTextAlignment = VerticalAlignment.Center;
            this.headerShadowText.HorizontalTextAlignment = HorizontalAlignment.Center;
            this.headerShadowText.Text = Resources.ChangeCommentView_Title;
            this.headerShadowText.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            //
            // buttonClearText
            //
            buttonClearText.Location = new Point(194, 0);
            buttonClearText.AutoSize = true;
            buttonClearText.Name = "buttonClearText";
            buttonClearText.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            buttonClearText.Click += new EventHandler(ButtonClearTextClick);

            //
            // tbxCommentText
            //
            tbxCommentText.Name = "tbxCommentText";
            tbxCommentText.Location = new Point(5, 40);
            tbxCommentText.Size = new Size(235, 228);
            tbxCommentText.Multiline = true;
            tbxCommentText.MaxLength = 250;
            tbxCommentText.Font = new System.Drawing.Font("Tahoma", 16, FontStyle.Regular);
            tbxCommentText.BorderStyle = BorderStyle.None;
            tbxCommentText.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top;

            //
            // mainMenu
            //
            mainMenu.MenuItems.Add(menuItemChange);
            mainMenu.MenuItems.Add(menuItemCancel);

            //
            // menuItemChange
            //
            menuItemChange.Text = Resources.ChangeStatusView_SetStatus;
            menuItemChange.Click += new EventHandler(MenuItemChangeClick);

            //
            // menuItemCancel
            //
            menuItemCancel.Text = Resources.ChangeStatusView_Cancel;
            menuItemCancel.Click += new EventHandler(MenuItemCancelClick);

            //
            // ChangeCommentView
            //
            this.Size = new Size(240, 268);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = Color.White;
            this.AutoScroll = true;

            this.Controls.Add(tbxCommentText);
            this.Canvas.Children.Add(header);
            this.Canvas.Children.Add(headerShadowText);
            this.Canvas.Children.Add(headerText);
            this.Canvas.Children.Add(buttonClearText);
            this.Canvas.RecalcDPIScaling();

            this.ResumeLayout(false);

            this.headerShadowText.Location = new Point(this.headerShadowText.Left - UISettings.CalcPix(1), this.headerShadowText.Top - UISettings.CalcPix(1));
            this.header.AlphaChannelImage = MasterForm.SkinManager.GetImage("bdTop");
        }

        #endregion

        private MainMenu mainMenu;
        private MenuItem menuItemCancel;
        private MenuItem menuItemChange;
        private TextBox tbxCommentText;
        private UIButton buttonClearText;

        private GraphicsImage header;
        private UILabel headerText;
        private UILabel headerShadowText;
    }
}
