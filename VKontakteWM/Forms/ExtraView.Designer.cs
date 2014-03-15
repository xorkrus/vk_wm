using System.Windows.Forms;
using System;
using Galssoft.VKontakteWM.ApplicationLogic;
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
    partial class ExtraView
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
            this.toolBar = new BottomToolBar();
            extraKineticListView = new ExtraKineticListView();
            this.header = new GraphicsImage();
            this.logo = new GraphicsImage();
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
            this.headerText.Size = new Size(headerText.Width, headerText.Height);
            this.headerText.Font = FontCache.CreateFont("Tahoma", 12, FontStyle.Bold, true);
            this.headerText.ForeColor = Color.White;
            this.headerText.VerticalTextAlignment = VerticalAlignment.Center;
            this.headerText.HorizontalTextAlignment = HorizontalAlignment.Center;
            //FIXME
            this.headerText.Text = Resources.ExtraView_Text;
            //this.headerText.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            //
            //HeaderShadowText
            //
            this.headerShadowText.Name = "HeadShadowText";
            this.headerShadowText.Location = new Point(0, 0);
            this.headerShadowText.Size = new Size(headerText.Width, headerText.Height);
            this.headerShadowText.Font = FontCache.CreateFont("Tahoma", 12, FontStyle.Bold, true);
            this.headerShadowText.ForeColor = Color.FromArgb(80, 80, 80);
            this.headerShadowText.VerticalTextAlignment = VerticalAlignment.Center;
            this.headerShadowText.HorizontalTextAlignment = HorizontalAlignment.Center;
            //FIXME
            this.headerShadowText.Text = Resources.ExtraView_Text;
            //this.headerShadowText.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            //
            // extraKineticListView
            //
            this.extraKineticListView.Anchor = System.Windows.Forms.AnchorStyles.Left
                                            | System.Windows.Forms.AnchorStyles.Right
                                            | System.Windows.Forms.AnchorStyles.Top
                                            | System.Windows.Forms.AnchorStyles.Bottom;
            this.extraKineticListView.Location = new System.Drawing.Point(0, 36);
            this.extraKineticListView.BackColor = Color.White;
            this.extraKineticListView.ScrollAction = KineticControlBase.KineticControlScrollAction.ScrollingForTime;
            this.extraKineticListView.Name = "extraKineticListView";
            this.extraKineticListView.Size = new System.Drawing.Size(240, 193);
            this.extraKineticListView.BackgroundIImage = MasterForm.SkinManager.GetImage("List-background");
            this.extraKineticListView.ContentUpShadow = MasterForm.SkinManager.GetImage("ContentUpShadow");
            this.extraKineticListView.ContentDownShadow = MasterForm.SkinManager.GetImage("ContentDownShadow");
            this.extraKineticListView.ShowContentShadows = true;
            this.extraKineticListView.TabIndex = 2;

            //
            // toolBar
            //
            toolBar.ToolbarButtonNews.Click += new EventHandler(ButtonNewsClick);
            toolBar.ToolbarButtonMessages.Click += new EventHandler(ButtonMessagesClick);
            toolBar.ToolbarButtonFriends.Click += new EventHandler(ButtonFriendsClick);
            toolBar.ToolbarButtonPhotos.Click += new EventHandler(ButtonPhotosClick);                                   

            this.Canvas.Children.Add(toolBar);            
            this.Canvas.Children.Add(header);
            this.Canvas.Children.Add(logo);
            this.Canvas.Children.Add(headerShadowText);
            this.Canvas.Children.Add(headerText);

            this.Controls.Add(extraKineticListView);

            // 
            // ExtraView
            // 
            this.Size = new Size(240, 268);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = Color.White;
            this.AutoScroll = true;
            this.Resize += new EventHandler(ExtraViewResize);

            this.Canvas.RecalcDPIScaling();
            this.ResumeLayout(false);

            this.headerShadowText.Location = new Point(this.headerShadowText.Location.X - 1,
                                           this.headerShadowText.Location.Y - 1);
            this.header.AlphaChannelImage = MasterForm.SkinManager.GetImage("Header");
            this.logo.AlphaChannelImage = MasterForm.SkinManager.GetImage("HeaderLogo");
        }

        private BottomToolBar toolBar;

        private GraphicsImage header;
        private GraphicsImage logo;
        private UILabel headerText;
        private UILabel headerShadowText;

        private ExtraKineticListView extraKineticListView;
        #endregion
    }
}
