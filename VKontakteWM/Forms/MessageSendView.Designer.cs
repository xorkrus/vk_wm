using System;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Properties;
using Microsoft.WindowsCE.Forms;
using AnchorStyles = Galssoft.VKontakteWM.Components.UI.AnchorStyles;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.Common.Configuration;

namespace Galssoft.VKontakteWM.Forms
{
    partial class MessageSendView
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
            this.mainMenu = new System.Windows.Forms.MainMenu();
            this.menuItemSend = new System.Windows.Forms.MenuItem();
            this.menuItemCancel = new System.Windows.Forms.MenuItem();
            this.contextMenu = new ContextMenu();
            this.miCut = new MenuItem();
            this.miCopy = new MenuItem();
            this.miPaste = new MenuItem();
            this.tbxMessageData = new TextBox();
            this.headerText = new UILabel();
            this.headerShadowText = new UILabel();
            this.header = new GraphicsImage();
            this.logo = new GraphicsImage();
            this.friendsLogo = new UIButton(ButtonStyle.AlphaChannel);

            this.SuspendLayout();

            //
            // header
            //
            this.header.Name = "header";
            this.header.Location = new Point(0, 0);
            this.header.Stretch = true;
            this.header.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            //
            // logo
            //
            this.logo.Name = "logo";
            this.logo.Location = new Point(6, 7);
            this.logo.Stretch = false;
            this.logo.Anchor = AnchorStyles.Left | AnchorStyles.Top;

            //
            // friendLogo
            //
            this.friendsLogo.Name = "friendLogo";
            //this.friendsLogo.Location = new Point(210, 4);
            this.friendsLogo.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.friendsLogo.Click += new EventHandler(FriendsLogoClick);

            //
            // headerText
            //
            this.headerText.Name = "headerText";
            this.headerText.Location = new Point(0, 0);
            this.headerText.Size = new Size(240, 36);
            this.headerText.Font = FontCache.CreateFont("Tahoma", 14, FontStyle.Bold);
            this.headerText.ForeColor = Color.White;
            this.headerText.VerticalTextAlignment = VerticalAlignment.Center;
            this.headerText.HorizontalTextAlignment = Galssoft.VKontakteWM.Components.UI.HorizontalAlignment.Center;
            this.headerText.Text = Resources.ChangeCommentView_Title;
            this.headerText.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            //
            // headerShadowText
            //
            this.headerShadowText.Name = "headerShadowText";
            this.headerShadowText.Location = new Point(0, 0);
            this.headerShadowText.Size = new Size(240, 36);
            this.headerShadowText.Font = FontCache.CreateFont("Tahoma", 14, FontStyle.Bold);
            this.headerShadowText.ForeColor = Color.Black;
            this.headerShadowText.VerticalTextAlignment = VerticalAlignment.Center;
            this.headerShadowText.HorizontalTextAlignment = Galssoft.VKontakteWM.Components.UI.HorizontalAlignment.Center;
            this.headerShadowText.Text = Resources.ChangeCommentView_Title;
            this.headerShadowText.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            //
            // tbxCommentText
            //
            tbxMessageData.Name = "tbxCommentText";
            tbxMessageData.Location = new Point(0, 36);
            tbxMessageData.Size = new Size(240, 232);
            tbxMessageData.Multiline = true;            
            tbxMessageData.Font = new System.Drawing.Font("Tahoma", 12, FontStyle.Regular);
            tbxMessageData.BorderStyle = BorderStyle.None;
            tbxMessageData.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top;
            tbxMessageData.ContextMenu = this.contextMenu;
            tbxMessageData.ScrollBars = ScrollBars.Vertical;
            tbxMessageData.WordWrap = true;

            // 
            // mainMenu1
            // 
            this.mainMenu.MenuItems.Add(this.menuItemCancel);
            this.mainMenu.MenuItems.Add(this.menuItemSend);

            // 
            // menuItemSend
            // 
            this.menuItemSend.Text = Resources.SendCommentSend;
            this.menuItemSend.Click += new System.EventHandler(ButtonSendClick);

            // 
            // menuItemCancel
            // 
            this.menuItemCancel.Text = Resources.MessagesList_Designer_menuItemCancel_Text;
            this.menuItemCancel.Click += new System.EventHandler(ButtonCancelClick);

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
            // this
            //
            this.Size = new Size(240, 268);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = Color.White;
            this.GotFocus += new EventHandler(MessageSendViewGotFocus);
            this.AutoScroll = true;

            this.Canvas.Children.Add(header);
            this.Canvas.Children.Add(logo);
            //this.Canvas.Children.Add(friendsLogo);
            this.Canvas.Children.Add(headerShadowText);
            this.Canvas.Children.Add(headerText);

            this.Controls.Add(tbxMessageData);            

            this.Canvas.RecalcDPIScaling();
            this.ResumeLayout(false);

            this.headerShadowText.Location = new Point(this.headerShadowText.Location.X, this.headerShadowText.Location.Y - UISettings.CalcPix(1));

            this.header.AlphaChannelImage = MasterForm.SkinManager.GetImage("Header");
            this.logo.AlphaChannelImage = MasterForm.SkinManager.GetImage("HeaderLogo");
            this.friendsLogo.TransparentButton = MasterForm.SkinManager.GetImage("FriendsLogo");
            this.friendsLogo.TransparentButtonPressed = MasterForm.SkinManager.GetImage("FriendsLogoPressed");
        }

        #endregion

        private MainMenu mainMenu;
        private MenuItem menuItemCancel;
        private MenuItem menuItemSend;
        private TextBox tbxMessageData;

        private ContextMenu contextMenu;
        private MenuItem miCut;
        private MenuItem miCopy;
        private MenuItem miPaste;

        private GraphicsImage header;
        private GraphicsImage logo;
        private UIButton friendsLogo;

        private UILabel headerText;
        private UILabel headerShadowText;
    }
}
