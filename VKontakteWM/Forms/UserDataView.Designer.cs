using System.Windows.Forms;
using System;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Properties;
using System.Drawing;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Components;
using AnchorStyles = Galssoft.VKontakteWM.Components.UI.AnchorStyles;
using HorizontalAlignment = Galssoft.VKontakteWM.Components.UI.HorizontalAlignment;

namespace Galssoft.VKontakteWM.Forms
{
    partial class UserDataView
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
            buttonClearPass = new UIButton(ButtonStyle.AlphaChannel);
            buttonClearCache = new UIButton(ButtonStyle.AlphaChannel);
            lblName = new UILabel(Resources.LoginView_Caption_Login);
            mainMenu = new System.Windows.Forms.MainMenu();
            menuItemBack = new System.Windows.Forms.MenuItem();
            menuEmpty = new System.Windows.Forms.MenuItem();

            userDataKineticListView = new UserDataKineticListView();
            this.header = new GraphicsImage();
            this.logo = new GraphicsImage();
            this.headerText = new UILabel();
            this.headerShadowText = new UILabel();

            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.Add(this.menuItemBack);
            this.mainMenu.MenuItems.Add(this.menuEmpty);
            // 
            // menuItemBack
            // 
            this.menuItemBack.Text = Resources.UploadPhoto_Designer_menuItemBack_Text;
            this.menuItemBack.Click += new System.EventHandler(menuItemBack_Click);

            this.menuEmpty.Text = "";
            this.menuEmpty.Enabled = false;

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
            this.headerText.Text = Resources.UserDataView_Text;
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
            this.headerShadowText.Text = Resources.UserDataView_Text;
            this.headerShadowText.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            //
            // userDataKineticListView
            //
            this.userDataKineticListView.Anchor = System.Windows.Forms.AnchorStyles.Left
                                            | System.Windows.Forms.AnchorStyles.Right
                                            | System.Windows.Forms.AnchorStyles.Top
                                            | System.Windows.Forms.AnchorStyles.Bottom;
            this.userDataKineticListView.Location = new System.Drawing.Point(0, 36);
            this.userDataKineticListView.BackColor = Color.White;
            this.userDataKineticListView.ScrollAction = KineticControlBase.KineticControlScrollAction.ScrollingForTime;
            this.userDataKineticListView.Name = "userDataKineticListView";
            this.userDataKineticListView.Size = new System.Drawing.Size(240, 232);
            this.userDataKineticListView.BackgroundIImage = MasterForm.SkinManager.GetImage("List-background");
            this.userDataKineticListView.ContentUpShadow = MasterForm.SkinManager.GetImage("ContentUpShadow");
            this.userDataKineticListView.ContentDownShadow = MasterForm.SkinManager.GetImage("ContentDownShadow");
            this.userDataKineticListView.ShowContentShadows = true;
            this.userDataKineticListView.OutsideDownShadow = MasterForm.SkinManager.GetImage("ContentUpShadow");
            this.userDataKineticListView.OutsideUpShadow = MasterForm.SkinManager.GetImage("ContentDownShadow");
            this.userDataKineticListView.ShowInnerShadows = true;
            this.userDataKineticListView.TabIndex = 2;
            //
            // lblName
            //
            lblName.Name = "lblName";
            lblName.Location = new Point(30, 40);
            lblName.Size = new Size(150, 10);
            lblName.Font = FontCache.CreateFont("Calibri", 18, FontStyle.Regular, true);
            // 
            // buttonClearPass
            // 
            buttonClearPass.TransparentButton = MasterForm.SkinManager.GetImage("ButtonOther");
            buttonClearPass.TransparentButtonPressed = MasterForm.SkinManager.GetImage("ButtonOtherPressed");
            buttonClearPass.Location = new Point(80, 100);
            buttonClearPass.Size = new Size(100, 25);
            buttonClearPass.Text = Resources.UserDataView_Button_ClearPass;
            buttonClearPass.Name = "buttonClearPass";
            buttonClearPass.Click += new EventHandler(buttonClearPass_Click);
            buttonClearPass.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            // 
            // buttonClearCache
            // 
            buttonClearCache.TransparentButton = MasterForm.SkinManager.GetImage("ButtonOther");
            buttonClearCache.TransparentButtonPressed = MasterForm.SkinManager.GetImage("ButtonOtherPressed");
            buttonClearCache.Location = new Point(80, 150);
            buttonClearCache.Size = new Size(100, 25);
            buttonClearCache.Text = Resources.UserDataView_Button_ClearCache;
            buttonClearCache.Name = "buttonClearCache";
            buttonClearCache.Click += new EventHandler(buttonClearCache_Click);
            buttonClearCache.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            // 
            // UserDataView
            // 
            this.Size = new Size(240, 268);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = Color.White;
            this.AutoScroll = true;
            //this.Canvas.Children.Add(buttonClearPass);
            //this.Canvas.Children.Add(buttonClearCache);
            //this.Canvas.Children.Add(lblName);

            this.Controls.Add(userDataKineticListView);
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

        private UIButton buttonClearPass;
        private UIButton buttonClearCache;
        private UILabel lblName;
        private System.Windows.Forms.MainMenu mainMenu;
        private System.Windows.Forms.MenuItem menuItemBack;
        private System.Windows.Forms.MenuItem menuEmpty;

        private GraphicsImage header;
        private GraphicsImage logo;
        private UILabel headerText;
        private UILabel headerShadowText;

        private UserDataKineticListView userDataKineticListView;

        #endregion
    }
}
