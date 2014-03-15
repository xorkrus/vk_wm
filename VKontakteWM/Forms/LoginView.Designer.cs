using System;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Properties;
using Microsoft.WindowsCE.Forms;
using AnchorStyles = Galssoft.VKontakteWM.Components.UI.AnchorStyles;
using Galssoft.VKontakteWM.Components.Common.Configuration;

namespace Galssoft.VKontakteWM.Forms
{
    partial class LoginView
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
            mainMenu = new System.Windows.Forms.MainMenu();
            menuItemLogin = new System.Windows.Forms.MenuItem();
            menuItemExit = new System.Windows.Forms.MenuItem();
            this.textBoxLogin = new UITextControl();
            this.textBoxPass = new UITextControl();
            this.btnRemember = new UIButton(ButtonStyle.AlphaChannel);
            this.lblRemember = new UILabel();
            this.loginLogo = new GraphicsImage();

            this.SuspendLayout();

            this.Name = "LoginForm";
            this.Size = new Size(240, 268);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.Dock = DockStyle.Fill;
            this.Resize += new EventHandler(FormResize);
            this.AutoScroll = true;
            this.Click += new EventHandler(LoginViewClick);

            // 
            // mainMenu1
            // 
            this.mainMenu.MenuItems.Add(this.menuItemExit);
            this.mainMenu.MenuItems.Add(this.menuItemLogin);

            // 
            // menuItemLogin
            // 
            this.menuItemLogin.Text = Resources.LoginView_Button_Login;
            this.menuItemLogin.Click += new System.EventHandler(BtnEnterClick);

            // 
            // menuItemExit
            // 
            this.menuItemExit.Text = Resources.LoginView_Exit;
            this.menuItemExit.Click += new System.EventHandler(ButtonCancelClick);

            //
            // loginLogo
            //
            this.loginLogo.Location = new Point(48, 23);

            //
            // btnRemember
            //
            btnRemember.TransparentButton = MasterForm.SkinManager.GetImage("RememberBoxChecked");
            btnRemember.TransparentButtonPressed = MasterForm.SkinManager.GetImage("RememberBoxCheckedPressed");
            btnRemember.Size = new Size(25, 25);
            btnRemember.Location = new Point(20, 153);
            btnRemember.Name = "btnRememberChecked";
            btnRemember.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            btnRemember.Click += new EventHandler(BtnRememberClick);

            //
            // lblRemember
            //
            lblRemember.Size = new Size(200, 14);
            lblRemember.Location = new Point(53, 157);
            lblRemember.ForeColor = Color.White;
            lblRemember.Font = FontCache.CreateFont("Tahoma", 14, FontStyle.Regular);
            lblRemember.Text = Resources.LoginView_Remember;
            lblRemember.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            lblRemember.VerticalTextAlignment = Galssoft.VKontakteWM.Components.UI.VerticalAlignment.Center;
            lblRemember.HorizontalTextAlignment = Galssoft.VKontakteWM.Components.UI.HorizontalAlignment.Left;

            //
            // textBoxLogin
            //
            this.textBoxLogin.Location = new Point(20, 71);
            this.textBoxLogin.Name = "textBoxLogin";
            this.textBoxLogin.BackColor = Color.White;
            this.textBoxLogin.Font = FontCache.CreateFont("Calibri", 14, FontStyle.Regular, true);
            this.textBoxLogin.SetImages(MasterForm.SkinManager.GetImage("UITextBoxLeftBorder"), MasterForm.SkinManager.GetImage("UITextBoxRightBorder"), MasterForm.SkinManager.GetImage("UITextBoxTopBorder"), MasterForm.SkinManager.GetImage("UITextBoxBottomBorder"));
            this.textBoxLogin.Size = new System.Drawing.Size(200, 31);
            this.textBoxLogin.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            this.textBoxLogin.TextColor = Color.DarkGray;
            this.textBoxLogin.MouseUp += new MouseEventHandler(TextBoxLoginMouseUp);

            //
            // textBoxPass
            //
            this.textBoxPass.Location = new Point(20, 110);
            this.textBoxPass.Name = "textBoxPass";
            this.textBoxPass.BackColor = Color.White;
            this.textBoxPass.Font = FontCache.CreateFont("Calibri", 14, FontStyle.Regular, true);
            this.textBoxPass.SetImages(MasterForm.SkinManager.GetImage("UITextBoxLeftBorder"), MasterForm.SkinManager.GetImage("UITextBoxRightBorder"), MasterForm.SkinManager.GetImage("UITextBoxTopBorder"), MasterForm.SkinManager.GetImage("UITextBoxBottomBorder"));
            this.textBoxPass.Size = new System.Drawing.Size(200, 31);
            this.textBoxPass.PasswordChar = '*';
            this.textBoxPass.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            this.textBoxPass.TextColor = Color.DarkGray;
            this.textBoxPass.MouseUp += new MouseEventHandler(TextBoxPassMouseUp);

            this.Canvas.Children.Add(this.loginLogo);
            this.Canvas.Children.Add(this.textBoxPass);
            this.Canvas.Children.Add(this.textBoxLogin);
            this.Canvas.Children.Add(this.btnRemember);
            this.Canvas.Children.Add(this.lblRemember);

            this.Canvas.RecalcDPIScaling();
            this.ResumeLayout(false);

            this.loginLogo.AlphaChannelImage = MasterForm.SkinManager.GetImage("LoginLogo");
        }

        #endregion

        private System.Windows.Forms.MainMenu mainMenu;
        private System.Windows.Forms.MenuItem menuItemLogin;
        private System.Windows.Forms.MenuItem menuItemExit;
        private UITextControl textBoxLogin;
        private UITextControl textBoxPass;
        private GraphicsImage loginLogo;
        private UIButton btnRemember;
        private UILabel lblRemember;
    }
}
