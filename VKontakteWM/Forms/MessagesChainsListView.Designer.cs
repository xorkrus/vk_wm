using System;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Properties;
using AnchorStyles = Galssoft.VKontakteWM.Components.UI.AnchorStyles;
using ToolBar = Galssoft.VKontakteWM.Components.UI.CompoundControls.ToolBar;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;

namespace Galssoft.VKontakteWM.Forms
{
    partial class MessagesChainsListView
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
            this.toolBar = new BottomToolBar();

            btnSentMessage = new UIButton(ButtonStyle.AlphaChannel);
            btnSentMessageText = new UILabel();
            btnSentMessageShadowText = new UILabel();
            btnRefresh = new UIButton(ButtonStyle.AlphaChannel);
            header = new GraphicsImage();
            logo = new GraphicsImage();

            this.klvMessagesChainsList = new MessagesChainsListKineticListView();

            this.SuspendLayout();

            this.Size = new Size(240, 268);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.Name = "MessagesChainsListView";
            this.Resize += new EventHandler(MessagesChainsListView_Resize);
            this.ForeColor = Color.White;

            this.header.Name = "Head";
            this.header.Location = new Point(0, 0);
            this.header.Stretch = true;
            this.header.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            this.logo.Name = "Logo";
            this.logo.Location = new Point(6, 7);
            this.logo.Stretch = false;
            this.logo.Anchor = AnchorStyles.Left | AnchorStyles.Top;

            //
            //btnRefresh
            //
            btnRefresh.TransparentButton = MasterForm.SkinManager.GetImage("RefreshButton");
            btnRefresh.TransparentButtonPressed = MasterForm.SkinManager.GetImage("RefreshButtonPressed");
            btnRefresh.Location = new Point(211, 5);
            btnRefresh.Size = new Size(24, 24);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefresh.Click += new EventHandler(BtnRefreshClick);

            //
            //btnSentMessage
            //
            btnSentMessage.TransparentButton = MasterForm.SkinManager.GetImage("MessagesNew");
            btnSentMessage.TransparentButtonPressed = MasterForm.SkinManager.GetImage("MessagesNewPressed");
            btnSentMessage.Location = new Point(60, 5);
            btnSentMessage.Size = new Size(121, 25);
            btnSentMessage.Name = "btnSentMessage";
            btnSentMessage.Click += new EventHandler(BtnSentMessageClick);

            //
            // btnSentMessageText
            //
            btnSentMessageText.Name = "btnSentMessageText";
            btnSentMessageText.Location = btnSentMessage.Location;
            btnSentMessageText.Font = FontCache.CreateFont("Tahoma", 12, FontStyle.Bold, false);
            btnSentMessageText.ForeColor = Color.White;
            btnSentMessageText.VerticalTextAlignment = Galssoft.VKontakteWM.Components.UI.VerticalAlignment.Center;
            btnSentMessageText.HorizontalTextAlignment = Galssoft.VKontakteWM.Components.UI.HorizontalAlignment.Center;
            btnSentMessageText.Text = Resources.NewMessage;

            //
            // btnSentMessageShadowText
            //
            btnSentMessageShadowText.Name = "btnSentMessageShadowText";
            btnSentMessageShadowText.Location = btnSentMessage.Location;
            btnSentMessageShadowText.Font = FontCache.CreateFont("Tahoma", 12, FontStyle.Bold, false);
            btnSentMessageShadowText.ForeColor = Color.FromArgb(0, 0, 0);
            btnSentMessageShadowText.VerticalTextAlignment = Galssoft.VKontakteWM.Components.UI.VerticalAlignment.Center;
            btnSentMessageShadowText.HorizontalTextAlignment = Galssoft.VKontakteWM.Components.UI.HorizontalAlignment.Center;
            btnSentMessageShadowText.Text = Resources.NewMessage;

            //
            //klvStatusUpdatesList
            //
            klvMessagesChainsList.Anchor =  System.Windows.Forms.AnchorStyles.Left
                                            | System.Windows.Forms.AnchorStyles.Right
                                            | System.Windows.Forms.AnchorStyles.Top
                                            | System.Windows.Forms.AnchorStyles.Bottom;
            klvMessagesChainsList.Location = new System.Drawing.Point(0, 36);
            klvMessagesChainsList.BackColor = Color.White;
            klvMessagesChainsList.BackgroundIImage = MasterForm.SkinManager.GetImage("List-background");
            klvMessagesChainsList.ContentUpShadow = MasterForm.SkinManager.GetImage("ContentUpShadow");
            klvMessagesChainsList.ContentDownShadow = MasterForm.SkinManager.GetImage("ContentDownShadow");
            klvMessagesChainsList.ShowContentShadows = true;
            klvMessagesChainsList.OutsideDownShadow = MasterForm.SkinManager.GetImage("ContentUpShadow");
            klvMessagesChainsList.OutsideUpShadow = MasterForm.SkinManager.GetImage("ContentDownShadow");
            klvMessagesChainsList.ShowInnerShadows = true;
            klvMessagesChainsList.ScrollAction = KineticControlBase.KineticControlScrollAction.ScrollingForTime;
            klvMessagesChainsList.Name = "klvMessagesChainsList";
            klvMessagesChainsList.Size = new System.Drawing.Size(240, 193);

            //
            // toolBar
            //
            toolBar.ToolbarButtonNews.Click += new EventHandler(ButtonNewsClick);
            toolBar.ToolbarButtonFriends.Click += new EventHandler(ButtonFriendsClick);
            toolBar.ToolbarButtonPhotos.Click += new EventHandler(ButtonPhotosClick);
            toolBar.ToolbarButtonExtras.Click += new EventHandler(ButtonExtrasClick);

            toolBar.miUserData.Click += new EventHandler(MiUserDataClick);
            toolBar.miSettings.Click += new EventHandler(MiSettingsClick);
            toolBar.miAbout.Click += new EventHandler(MiAboutClick);
            toolBar.miExit.Click += new EventHandler(MiExitClick);
                                 
            this.Canvas.Children.Add(toolBar);
            this.Canvas.Children.Add(header);
            this.Canvas.Children.Add(logo);
            this.Canvas.Children.Add(btnSentMessage);
            this.Canvas.Children.Add(btnSentMessageShadowText);
            this.Canvas.Children.Add(btnSentMessageText);
            this.Canvas.Children.Add(btnRefresh);
            this.Controls.Add(klvMessagesChainsList);

            this.Canvas.RecalcDPIScaling();

            this.ResumeLayout(false);

            this.header.AlphaChannelImage = MasterForm.SkinManager.GetImage("Header");
            this.logo.AlphaChannelImage = MasterForm.SkinManager.GetImage("HeaderLogo");

            btnSentMessageShadowText.Location = new Point(
                btnSentMessageShadowText.Location.X,
                btnSentMessageShadowText.Location.Y - 1
                );

            btnSentMessageText.Size = btnSentMessageShadowText.Size = btnSentMessage.Size;
        } 

        #endregion

        private BottomToolBar toolBar;

        private UIButton btnSentMessage;
        private UILabel btnSentMessageText;
        private UILabel btnSentMessageShadowText;

        private UIButton btnRefresh;
        private GraphicsImage header;
        private GraphicsImage logo;

        private MessagesChainsListKineticListView klvMessagesChainsList;
    }
}
