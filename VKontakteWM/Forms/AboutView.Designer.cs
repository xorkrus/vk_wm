using System.Windows.Forms;
using System;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Properties;
using System.Drawing;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Components;
using AnchorStyles = Galssoft.VKontakteWM.Components.UI.AnchorStyles;
using HorizontalAlignment = Galssoft.VKontakteWM.Components.UI.HorizontalAlignment;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Components.Common.Configuration;

namespace Galssoft.VKontakteWM.Forms
{
    partial class AboutView
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
            this.menuItemBack = new System.Windows.Forms.MenuItem();
            this.menuItemActions = new System.Windows.Forms.MenuItem();
            this.menuItemAdd = new System.Windows.Forms.MenuItem();
            this.menuItemSupport = new System.Windows.Forms.MenuItem();

            //this.header = new GraphicsImage();
            this.giLogo = new GraphicsImage();
            this.giGlassoftLogo = new ClickableGraphicsImage();

            //this.lblTitle = new UILabel(Resources.AboutView_lblApplicationTitle_Text);
            this.lblVersion = new UILabel();
            this.lblVersionShadow = new UILabel();
            //this.lblMadeBy = new UILabel(Resources.AboutView_MadeBy);
            //this.lblButtonSubstitue = new UILabel(Resources.ApplicationUpdate_Message_AlreadyInstall);
            this.lblButtonSubstitue = new UILabel();

            //this.lblVersionDescription = new MLLabel();

            //this.buttonCheckOrUploadUpdate = new UploadPhotoControl();

            this.SuspendLayout();

            //
            // AboutForm
            //
            this.Name = "AboutForm";
            this.Size = new Size(240, 268);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
            this.Resize += new EventHandler(FormResize);

            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.Add(this.menuItemBack);
            this.mainMenu.MenuItems.Add(this.menuItemAdd);

            ////
            ////Header
            ////
            //this.header.Name = "header";
            //this.header.Location = new Point(0, 0);
            //this.header.Stretch = true;
            //this.header.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            //
            // giLogo
            //
            this.giLogo.Name = "giLogo";
            this.giLogo.Location = new Point(98, 45);

            //
            // giGlassoftLogo
            //
            this.giGlassoftLogo.Name = "giGlassoftLogo";
            this.giGlassoftLogo.Location = new Point(108, 230);
            this.giGlassoftLogo.Click += new EventHandler(GlassoftLogo_Click);

            // 
            // menuItemBack
            // 
            this.menuItemBack.Text = Resources.AboutView_Button_Back;
            this.menuItemBack.Click += new EventHandler(MenuItemBackClick);

            // 
            // menuItemAdd
            // 
            this.menuItemAdd.Text = Resources.AboutView_MenuItem_Add;
            this.menuItemAdd.MenuItems.Add(menuItemSupport);
            this.menuItemAdd.MenuItems.Add(menuItemActions);

            // 
            // menuItemSupport
            // 
            this.menuItemSupport.Text = Resources.AboutView_MenuItem_Support;
            this.menuItemSupport.Click += new EventHandler(GlassoftLogo_Click);

            // 
            // menuItemActions
            // 
            this.menuItemActions.Text = Resources.AboutView_CheckUpdateButton;
            this.menuItemActions.Click += new EventHandler(menuItemActions_Check_Click);
            //this.menuItemActions.Enabled = false;

            //// 
            ////upcUploadPhoto
            ////
            //buttonCheckOrUploadUpdate.Location = new System.Drawing.Point(5, 95);
            //buttonCheckOrUploadUpdate.Size = new System.Drawing.Size(225, 32);
            //buttonCheckOrUploadUpdate.Text = Resources.AboutView_CheckUpdateButton;
            //buttonCheckOrUploadUpdate.Name = "buttonCheckUpdate";
            //buttonCheckOrUploadUpdate.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            //buttonCheckOrUploadUpdate.Font = FontCache.CreateFont("Tahoma", 14, FontStyle.Regular);
            //buttonCheckOrUploadUpdate.PressedFont = FontCache.CreateFont("Tahoma", 14, FontStyle.Regular);
            //buttonCheckOrUploadUpdate.FontColor = Color.FromArgb(129, 129, 129);
            //buttonCheckOrUploadUpdate.FontColorShadow = Color.FromArgb(223, 223, 223);
            //buttonCheckOrUploadUpdate.DropShadow = true;
            //buttonCheckOrUploadUpdate.Click += new EventHandler(buttonCheckUpdate_Click);

            //lblTitle.Name = "lblTitle";
            //lblTitle.Location = new Point(15, 50);
            //lblTitle.Size = new Size(215, 15);
            //lblTitle.HorizontalTextAlignment = HorizontalAlignment.Center;
            //lblTitle.VerticalAlignment = VerticalAlignment.Center;
            //lblTitle.Font = FontCache.CreateFont("Tahoma", 14, FontStyle.Bold);
            //lblTitle.ForeColor = Color.FromArgb(50, 50, 50);
            //lblTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            lblVersion.Name = "lblVersion";
            lblVersion.Location = new Point(15, 125);
            lblVersion.Size = new Size(215, 15);
            lblVersion.HorizontalTextAlignment = HorizontalAlignment.Center;
            lblVersion.Font = FontCache.CreateFont("Tahoma", 20, FontStyle.Bold);
            lblVersion.ForeColor = Color.White;
            lblVersion.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            lblVersionShadow.Name = "lblVersionShadow";
            lblVersionShadow.Location = new Point(15, 125);
            lblVersionShadow.Size = new Size(215, 15);
            lblVersionShadow.HorizontalTextAlignment = HorizontalAlignment.Center;
            lblVersionShadow.Font = FontCache.CreateFont("Tahoma", 20, FontStyle.Bold);
            lblVersionShadow.ForeColor = Color.Black;
            lblVersionShadow.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            lblButtonSubstitue.Name = "lblButtonSubstitue";
            lblButtonSubstitue.Location = new Point(15, 145);
            lblButtonSubstitue.Size = new Size(215, 30);
            lblButtonSubstitue.HorizontalTextAlignment = HorizontalAlignment.Center;
            lblButtonSubstitue.VerticalAlignment = VerticalAlignment.Center;
            lblButtonSubstitue.Font = FontCache.CreateFont("Tahoma", 14, FontStyle.Regular);
            lblButtonSubstitue.ForeColor = Color.White;
            lblButtonSubstitue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblButtonSubstitue.Visible = false;

            //lblMadeBy.Name = "lblMadeBy";
            //lblMadeBy.Location = new Point(15, 235);
            //lblMadeBy.Size = new Size(105, 18);
            //lblMadeBy.HorizontalTextAlignment = HorizontalAlignment.Right;
            //lblMadeBy.Font = FontCache.CreateFont("Tahoma", 14, FontStyle.Regular);
            //lblMadeBy.ForeColor = Color.FromArgb(69, 68, 68);
            //lblMadeBy.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            //lblVersionDescription.Name = "lblVersionDescription";
            //lblVersionDescription.Location = new Point(15, 140);
            //lblVersionDescription.Size = new Size(215, 80);
            //lblVersionDescription.Font = FontCache.CreateFont("Tahoma", 14, FontStyle.Regular);
            //lblVersionDescription.FontColor = Color.FromArgb(69, 68, 68);
            //lblVersionDescription.VerticalTextAlignment = VerticalAlignment.Top;
            //lblVersionDescription.HorizontalTextAlignment = HorizontalAlignment.Left;
            //lblVersionDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;

            //this.Canvas.Children.Add(header);
            this.Canvas.Children.Add(giLogo);
            //this.Canvas.Children.Add(lblTitle);
            this.Canvas.Children.Add(lblVersionShadow);
            this.Canvas.Children.Add(lblVersion);
            this.Canvas.Children.Add(lblButtonSubstitue);
            //this.Canvas.Children.Add(lblMadeBy);
            this.Canvas.Children.Add(giGlassoftLogo);
            //this.Canvas.Children.Add(buttonCheckOrUploadUpdate);
            //this.Canvas.Children.Add(lblVersionDescription);

            this.Canvas.RecalcDPIScaling();
            this.ResumeLayout(false);

            //this.header.AlphaChannelImage = MasterForm.SkinManager.GetImage("bdTop");
            this.giLogo.AlphaChannelImage = MasterForm.SkinManager.GetImage("AboutLogo");
            this.giGlassoftLogo.AlphaChannelImage = MasterForm.SkinManager.GetImage("LogoGalsSoft");

            lblVersionShadow.Top += UISettings.CalcPix(1);

            //mainMenu = new System.Windows.Forms.MainMenu();
            //menuItemBack = new System.Windows.Forms.MenuItem();
            //menuItemUpdate = new System.Windows.Forms.MenuItem();

            //this.buttonUpdate = new UIButton(ButtonStyle.AlphaChannel);
            //this.btnBack = new UIButton(ButtonStyle.AlphaChannel);
            //this.btnFotoUpload = new UIButton(ButtonStyle.AlphaChannel);
            //this.lblVersion = new UILabel(Resources.LoginView_Caption_Login);
            ////this.lblNewVersion = new UILabel(Resources.LoginView_Caption_Login);
            ////this.lblNewVersionDescription = new UILabel(Resources.LoginView_Caption_Login);
            ////this.lblNewVersion = new UILabel();
            ////this.lblNewVersionDescription = new UILabel();            
            //lblNewVersion = new UILabel();
            //lblNewVerDescription = new UILabel();
            //lblTitle = new UILabel();
            //lblVersionIsMandatory = new UILabel();
            //giLogo = new GraphicsImage();

            //// 
            //// AboutForm
            //// 
            //this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            //this.Name = "AboutForm";
            ////this.ClientSize = new System.Drawing.Size(240, 268);
            //this.Size = new System.Drawing.Size(Screen.PrimaryScreen.Bounds.Width, 268);

            //// 
            //// mainMenu
            //// 
            //this.mainMenu.MenuItems.Add(this.menuItemBack);
            //this.mainMenu.MenuItems.Add(this.menuItemUpdate);

            //// 
            //// menuItemBack
            //// 
            //this.menuItemBack.Text = Resources.UploadPhoto_Designer_menuItemBack_Text;
            //this.menuItemBack.Click += new System.EventHandler(menuItemBack_Click);

            //// 
            //// menuItemUpdate
            //// 
            //this.menuItemUpdate.Text = Resources.StatusUpdatesList_Designer_btnRefresh_Text;
            //this.menuItemUpdate.Click += new System.EventHandler(menuItemUpdate_Click);

            //// 
            //// lblPassword
            //// 

            //giLogo.Location = new System.Drawing.Point(this.Width / 2 - 100, 40);
            //giLogo.Name = "giLogo";
            //giLogo.Size = new System.Drawing.Size(89, 101);
            //giLogo.Anchor = AnchorStyles.Top;

            //lblTitle.Name = "lbl3";
            //lblTitle.Location = new Point(0, 105);
            //lblTitle.Size = new Size(this.Width, 20);
            //lblTitle.Text = Resources.AboutView_lblApplicationTitle_Text;
            //lblTitle.HorizontalTextAlignment = HorizontalAlignment.Center;
            //lblTitle.Font = FontCache.CreateFont("Comic Sans", 16, FontStyle.Bold, true);
            //lblTitle.Anchor = AnchorStyles.Top;

            //lblVersion.Name = "lblPassword";
            //lblVersion.Location = new Point(10, 120);
            //lblVersion.Size = new Size(this.Width - 20, 20);
            //lblVersion.HorizontalTextAlignment = HorizontalAlignment.Right;
            //lblVersion.Anchor = AnchorStyles.Top;

            //lblNewVersion.Name = "lbl1";
            //lblNewVersion.Location = new Point(10, 135);
            //lblNewVersion.Size = new Size(this.Width - 20, 20);
            //lblNewVersion.Anchor = AnchorStyles.Top;

            //lblNewVerDescription.Name = "lbl2";
            //lblNewVerDescription.Location = new Point(20, 155);
            //lblNewVerDescription.Size = new Size(this.Width - 30, 50);
            //lblNewVerDescription.Multiline = true;
            //lblNewVerDescription.HorizontalTextAlignment = HorizontalAlignment.Stretch;
            //lblNewVerDescription.Anchor = AnchorStyles.Top;

            //lblVersionIsMandatory.Name = "lbl4";
            //lblVersionIsMandatory.Location = new Point(10, 200);
            //lblVersionIsMandatory.Size = new Size(this.Width - 20, 20);
            //lblVersionIsMandatory.Text = Resources.AboutView_lblVersionIsMandatory_Text;
            //lblVersionIsMandatory.Anchor = AnchorStyles.Top;
            //lblVersionIsMandatory.Visible = false;

            //this.Canvas.Children.Add(buttonUpdate);
            //this.Canvas.Children.Add(btnBack);
            //this.Canvas.Children.Add(lblVersion);
            ////this.Canvas.Children.Add(btnFotoUpload);

            //Canvas.Children.Add(lblNewVersion);
            //Canvas.Children.Add(lblNewVerDescription);
            //Canvas.Children.Add(lblTitle);
            //Canvas.Children.Add(lblVersionIsMandatory);
            //Canvas.Children.Add(giLogo);


            ////this.Canvas.Children.Add(lblNewVersion);
            ////this.Canvas.Children.Add(lblNewVersionDescription);

            //this.ResumeLayout(false);
        }

        //void btnFotoUpload_Click(object sender, EventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion

        private System.Windows.Forms.MainMenu mainMenu;
        private System.Windows.Forms.MenuItem menuItemBack;
        private System.Windows.Forms.MenuItem menuItemAdd;
        private System.Windows.Forms.MenuItem menuItemActions;
        private System.Windows.Forms.MenuItem menuItemSupport;

        //private GraphicsImage header;
        private GraphicsImage giLogo;

        private ClickableGraphicsImage giGlassoftLogo;

        //private UILabel lblTitle;
        private UILabel lblVersion;
        private UILabel lblVersionShadow;
        //private UILabel lblMadeBy;
        private UILabel lblButtonSubstitue;

        //private UploadPhotoControl buttonCheckOrUploadUpdate;

        //private MLLabel lblVersionDescription;        
    }
}
