using System.Windows.Forms;
using System;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.WebBrowser;
using Galssoft.VKontakteWM.Properties;
using System.Drawing;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Components;
using AnchorStyles = Galssoft.VKontakteWM.Components.UI.AnchorStyles;
using HorizontalAlignment = Galssoft.VKontakteWM.Components.UI.HorizontalAlignment;

namespace Galssoft.VKontakteWM.Forms
{
    partial class WebBrowserView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            mainMenu = new System.Windows.Forms.MainMenu();
            menuItemBack = new System.Windows.Forms.MenuItem();
            menuItemToFriends = new System.Windows.Forms.MenuItem();
            webBrowser = new WebBrowserEx();

            this.SuspendLayout();

            // 
            // webBrowser
            // 
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Anchor = System.Windows.Forms.AnchorStyles.Left
                                | System.Windows.Forms.AnchorStyles.Right
                                | System.Windows.Forms.AnchorStyles.Top
                                | System.Windows.Forms.AnchorStyles.Bottom;
            this.webBrowser.Location = new System.Drawing.Point(0, 0);
            this.webBrowser.Size = new System.Drawing.Size(240, 268);
            this.webBrowser.Dock = DockStyle.Fill;
            this.webBrowser.ShowWaitCursorWhileLoading = true;
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.Add(this.menuItemBack);
            this.mainMenu.MenuItems.Add(this.menuItemToFriends);

            // 
            // menuItemBack
            // 
            this.menuItemBack.Text = Resources.WebBrowser_menuItemBack_Text;
            this.menuItemBack.Click += new System.EventHandler(menuItemBack_Click);

            // 
            // menuItemToFriends
            // 
            this.menuItemToFriends.Text = Resources.WebBrowser_menuItemToApp_Text;
            this.menuItemToFriends.Click += new System.EventHandler(menuItemToApp_Click);

            // 
            // SettingView
            // 
            this.Size = new Size(240, 268);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = Color.White;
            this.AutoScroll = true;

            this.Controls.Add(webBrowser);

            this.Canvas.RecalcDPIScaling();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.MainMenu mainMenu;
        private System.Windows.Forms.MenuItem menuItemBack;
        private System.Windows.Forms.MenuItem menuItemToFriends;
        private WebBrowserEx webBrowser;

        #endregion
    }
}
