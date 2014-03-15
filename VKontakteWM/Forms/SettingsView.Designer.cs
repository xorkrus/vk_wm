using System.Windows.Forms;
using System;
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
    partial class SettingsView
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
            menuItemBack = new System.Windows.Forms.MenuItem();
            menuItemSave = new System.Windows.Forms.MenuItem();
            settingsKineticControl = new SettingsKineticControl();
            this.header = new GraphicsImage();
            this.logo = new GraphicsImage();
            this.headerText = new UILabel();
            this.headerShadowText = new UILabel();

            this.SuspendLayout();

            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.Add(this.menuItemBack);
            this.mainMenu.MenuItems.Add(this.menuItemSave);
            
            // 
            // menuItemBack
            // 
            this.menuItemBack.Text = Resources.Settings_View_Back;
            this.menuItemBack.Click += new System.EventHandler(MenuItemBackClick);

            // 
            // menuItemSave
            // 
            this.menuItemSave.Text = Resources.Settings_View_Save;
            this.menuItemSave.Click += new System.EventHandler(MenuItemSaveClick);

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
            this.headerText.Size = new Size(headerText.Width, headerText.Height);
            this.headerText.Font = FontCache.CreateFont("Tahoma", 14, FontStyle.Bold, true);
            this.headerText.ForeColor = Color.White;
            this.headerText.VerticalTextAlignment = VerticalAlignment.Center;
            this.headerText.HorizontalTextAlignment = HorizontalAlignment.Center;
            //FIXME
            this.headerText.Text = Resources.SettingsView_Text;
            //this.headerText.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            //
            //HeaderShadowText
            //
            this.headerShadowText.Name = "HeadShadowText";
            this.headerShadowText.Location = new Point(0, 0);
            this.headerShadowText.Size = new Size(headerText.Width, headerText.Height);
            this.headerShadowText.Font = FontCache.CreateFont("Tahoma", 14, FontStyle.Bold, true);
            this.headerShadowText.ForeColor = Color.FromArgb(0, 0, 0);
            this.headerShadowText.VerticalTextAlignment = VerticalAlignment.Center;
            this.headerShadowText.HorizontalTextAlignment = HorizontalAlignment.Center;
            //FIXME
            this.headerShadowText.Text = Resources.SettingsView_Text;
            //this.headerShadowText.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            //
            // settingsKineticControl
            //
            this.settingsKineticControl.Anchor = System.Windows.Forms.AnchorStyles.Left
                                            | System.Windows.Forms.AnchorStyles.Right
                                            | System.Windows.Forms.AnchorStyles.Top
                                            | System.Windows.Forms.AnchorStyles.Bottom;
            this.settingsKineticControl.Location = new System.Drawing.Point(0, 36);
            this.settingsKineticControl.BackColor = Color.White;
            this.settingsKineticControl.BackgroundIImage = MasterForm.SkinManager.GetImage("List-background");
            this.settingsKineticControl.ContentUpShadow = MasterForm.SkinManager.GetImage("ContentUpShadow");
            this.settingsKineticControl.ContentDownShadow = MasterForm.SkinManager.GetImage("ContentDownShadow");
            this.settingsKineticControl.ShowContentShadows = true;
            this.settingsKineticControl.OutsideDownShadow = MasterForm.SkinManager.GetImage("ContentUpShadow");
            this.settingsKineticControl.OutsideUpShadow = MasterForm.SkinManager.GetImage("ContentDownShadow");
            this.settingsKineticControl.ShowInnerShadows = true;
            this.settingsKineticControl.ScrollAction = KineticControlBase.KineticControlScrollAction.ScrollingForTime;
            this.settingsKineticControl.Name = "settingsKineticControl";
            this.settingsKineticControl.Size = new System.Drawing.Size(240, 232);
            this.settingsKineticControl.TabIndex = 2;

            // 
            // SettingView
            // 
            this.Size = new Size(240, 268);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = Color.White;
            this.AutoScroll = true;
            this.Resize += new EventHandler(SettingsViewResize);

            this.Controls.Add(settingsKineticControl);
            this.Canvas.Children.Add(header);
            this.Canvas.Children.Add(logo);
            this.Canvas.Children.Add(headerShadowText);
            this.Canvas.Children.Add(headerText);
            this.Canvas.RecalcDPIScaling();
            this.ResumeLayout(false);

            this.headerShadowText.Location = new Point(this.headerShadowText.Location.X,
                                           this.headerShadowText.Location.Y - 1);
            this.header.AlphaChannelImage = MasterForm.SkinManager.GetImage("Header");
            this.logo.AlphaChannelImage = MasterForm.SkinManager.GetImage("HeaderLogo");
        }

        private System.Windows.Forms.MainMenu mainMenu;
        private System.Windows.Forms.MenuItem menuItemBack;
        private System.Windows.Forms.MenuItem menuItemSave;
        private SettingsKineticControl settingsKineticControl;

        private GraphicsImage header;
        private GraphicsImage logo;
        private UILabel headerText;
        private UILabel headerShadowText;

        #endregion
    }
}
