using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.ApplicationLogic;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Properties;
using AnchorStyles = Galssoft.VKontakteWM.Components.UI.AnchorStyles;
using ToolBar = Galssoft.VKontakteWM.Components.UI.CompoundControls.ToolBar;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;
using HorizontalAlignment = Galssoft.VKontakteWM.Components.UI.HorizontalAlignment;

namespace Galssoft.VKontakteWM.Forms
{
    partial class SendCommentView
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
            header = new GraphicsImage();
            logo = new GraphicsImage();
            tbxComment = new TextBox();
            mainMenu = new MainMenu();
            menuItemBack = new MenuItem();
            menuItemSend = new MenuItem();
            this.contextMenu = new ContextMenu();
            this.miCut = new MenuItem();
            this.miCopy = new MenuItem();
            this.miPaste = new MenuItem();
            this.headerText = new UILabel();
            this.headerShadowText = new UILabel();

            this.SuspendLayout();

            //
            // header
            //
            this.header.Name = "Head";
            this.header.Location = new Point(0, 0);
            this.header.Stretch = true;
            this.header.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            //
            // logo
            //
            this.logo.Name = "Logo";
            this.logo.Location = new Point(6, 7);
            this.logo.Stretch = false;
            this.logo.Anchor = AnchorStyles.Left | AnchorStyles.Top;

            //
            //HeaderText
            //
            this.headerText.Name = "HeadText";
            this.headerText.Location = new Point(0, 0);
            this.headerText.Size = new Size(240, 36);
            this.headerText.Font = FontCache.CreateFont("Tahoma", 14, FontStyle.Bold);
            this.headerText.ForeColor = Color.White;
            this.headerText.VerticalTextAlignment = VerticalAlignment.Center;
            this.headerText.HorizontalTextAlignment = HorizontalAlignment.Center;
            this.headerText.Text = Resources.SendCommentTitle;
            this.headerText.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            //
            //HeaderShadowText
            //
            this.headerShadowText.Name = "HeadShadowText";
            this.headerShadowText.Location = new Point(0, 0);
            this.headerShadowText.Size = new Size(240, 36);
            this.headerShadowText.Font = FontCache.CreateFont("Tahoma", 14, FontStyle.Bold);
            this.headerShadowText.ForeColor = Color.FromArgb(0, 0, 0);
            this.headerShadowText.VerticalTextAlignment = VerticalAlignment.Center;
            this.headerShadowText.HorizontalTextAlignment = HorizontalAlignment.Center;
            this.headerShadowText.Text = Resources.SendCommentTitle;
            this.headerShadowText.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            //
            // this
            //
            this.Size = new Size(240, 268);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.Name = "SendCommentView";

            //
            // mainMenu
            //
            mainMenu.MenuItems.Add(menuItemBack);
            mainMenu.MenuItems.Add(menuItemSend);

            //
            // menuItemBack
            //
            menuItemBack.Text = Resources.FriendsSearchListViewMenuItemBack;
            menuItemBack.Click += new EventHandler(MenuItemBackClick);

            //
            // menuItemSend
            //
            menuItemSend.Text = Resources.SendCommentSend;
            menuItemSend.Click += new EventHandler(MenuItemSendClick);

            //
            // contextMenu
            //
            this.contextMenu.MenuItems.Add(this.miCut);
            this.contextMenu.MenuItems.Add(this.miCopy);
            this.contextMenu.MenuItems.Add(this.miPaste);
            this.contextMenu.Popup += new EventHandler(ContextMenuPopup);

            //
            // miCut
            //
            this.miCut.Text = Resources.CutToClipboard;
            this.miCut.Click += new EventHandler(MiCutClick);

            //
            // miCopy
            //
            this.miCopy.Text = Resources.CopyToClipboard;
            this.miCopy.Click += new EventHandler(MiCopyClick);

            //
            // miPaste
            //
            this.miPaste.Text = Resources.PasteFromClipboard;
            this.miPaste.Click += new EventHandler(MiPasteClick);

            //
            // tbxComment
            //
            tbxComment.Location = new Point(0, 36);
            tbxComment.Size = new Size(240, 232);
            tbxComment.Multiline = true;
            tbxComment.Font = new System.Drawing.Font("Tahoma", 12, FontStyle.Regular);
            tbxComment.BorderStyle = BorderStyle.None;
            tbxComment.WordWrap = true;
            tbxComment.ScrollBars = ScrollBars.Vertical;
            tbxComment.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left |
                                   System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top;
            tbxComment.ContextMenu = contextMenu;

            this.Canvas.Children.Add(header);
            this.Canvas.Children.Add(logo);
            this.Canvas.Children.Add(headerShadowText);
            this.Canvas.Children.Add(headerText);
            this.Controls.Add(tbxComment);

            this.Canvas.RecalcDPIScaling();
            this.ResumeLayout(false);

            this.headerShadowText.Location = new Point(this.headerShadowText.Location.X,
                                                       this.headerShadowText.Location.Y - UISettings.CalcPix(1));

            this.header.AlphaChannelImage = MasterForm.SkinManager.GetImage("Header");
            this.logo.AlphaChannelImage = MasterForm.SkinManager.GetImage("HeaderLogo");
        }

        #endregion

        private GraphicsImage header;
        private GraphicsImage logo;
        private UILabel headerText;
        private UILabel headerShadowText;

        private TextBox tbxComment;
        private MainMenu mainMenu;
        private MenuItem menuItemBack;
        private MenuItem menuItemSend;

        private ContextMenu contextMenu;
        private MenuItem miCut;
        private MenuItem miCopy;
        private MenuItem miPaste;
    }
}
