using System.Runtime.InteropServices;
using Galssoft.VKontakteWM.Components.Common.Configuration;
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

namespace Galssoft.VKontakteWM.Forms
{
    partial class ChangeStatusView
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
            mainMenu = new MainMenu();
            menuItemCancel = new MenuItem();
            menuItemChange = new MenuItem();
            this.contextMenu = new ContextMenu();
            this.miCut = new MenuItem();
            this.miCopy = new MenuItem();
            this.miPaste = new MenuItem();
            tbxStatusText = new TextBox();
            header = new GraphicsImage();
            logo = new GraphicsImage();
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
            this.headerText.Text = Resources.ChangeStatusView_Title;
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
            this.headerShadowText.Text = Resources.ChangeStatusView_Title;
            this.headerShadowText.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            //
            // tbxStatusText
            //
            tbxStatusText.Location = new Point(5, 36);
            tbxStatusText.Size = new Size(235, 232);
            tbxStatusText.Multiline = true;
            tbxStatusText.Font = new System.Drawing.Font("Tahoma", 12, FontStyle.Regular);
            tbxStatusText.ScrollBars = ScrollBars.Vertical;
            tbxStatusText.WordWrap = true;
            tbxStatusText.BorderStyle = BorderStyle.None;
            tbxStatusText.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left |
                                   System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top;
            tbxStatusText.ContextMenu = this.contextMenu;

            //
            // mainMenu
            //
            mainMenu.MenuItems.Add(menuItemCancel);
            mainMenu.MenuItems.Add(menuItemChange);
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
            // ChangeStatusView
            //
            this.Size = new Size(240, 268);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = Color.White;
            this.AutoScroll = true;

            this.Controls.Add(tbxStatusText);
            this.Canvas.Children.Add(header);
            this.Canvas.Children.Add(logo);
            this.Canvas.Children.Add(headerShadowText);
            this.Canvas.Children.Add(headerText);
            this.Canvas.RecalcDPIScaling();
            this.ResumeLayout(false);

            this.headerShadowText.Location = new Point(this.headerShadowText.Location.X,
                this.headerShadowText.Location.Y - UISettings.CalcPix(1));

            this.header.AlphaChannelImage = MasterForm.SkinManager.GetImage("Header");
            this.logo.AlphaChannelImage = MasterForm.SkinManager.GetImage("HeaderLogo");
        }

        private MainMenu mainMenu;
        private MenuItem menuItemCancel;
        private MenuItem menuItemChange;

        private ContextMenu contextMenu;
        private MenuItem miCut;
        private MenuItem miCopy;
        private MenuItem miPaste;

        private TextBox tbxStatusText;

        private GraphicsImage header;
        private GraphicsImage logo;
        private UILabel headerText;
        private UILabel headerShadowText;

        #endregion
    }
}
