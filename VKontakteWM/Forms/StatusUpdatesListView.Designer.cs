using System;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Properties;
using AnchorStyles = Galssoft.VKontakteWM.Components.UI.AnchorStyles;
using ToolBar = Galssoft.VKontakteWM.Components.UI.CompoundControls.ToolBar;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;

namespace Galssoft.VKontakteWM.Forms
{
    partial class StatusUpdatesListView
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

            btnStatusUpdates = new UIButton(ButtonStyle.AlphaChannel);
            btnPhotoCommentsUpdates = new UIButton(ButtonStyle.AlphaChannel);
            btnRefresh = new UIButton(ButtonStyle.AlphaChannel);
            btnPhotoCommentsUpdatesText = new UILabel();
            btnPhotoCommentsUpdatesShadowText = new UILabel();
            btnStatusUpdatesText = new UILabel();
            btnStatusUpdatesShadowText = new UILabel();
            header = new GraphicsImage();
            logo = new GraphicsImage();

            this.klvStatusUpdatesList = new StatusUpdatesListKineticListView();
            this.klvImagesList = new ImagesKineticListView();

            this.contexMenu = new ContextMenu();
            this.menuItemSendMessage = new MenuItem();

            this.SuspendLayout();

            this.header.Name = "Head";
            this.header.Location = new Point(0, 0);
            this.header.Stretch = true;
            this.header.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            this.logo.Name = "Logo";
            this.logo.Location = new Point(6, 7);
            this.logo.Stretch = false;
            this.logo.Anchor = AnchorStyles.Left | AnchorStyles.Top;

            this.Size = new Size(240, 268);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.Name = "StatusUpdatesListView";
            this.Resize += new EventHandler(StatusUpdatesListViewResize);
            this.ForeColor = Color.White;

            
            //
            //btnStatusUpdates
            //
            btnStatusUpdates.TransparentButton = MasterForm.SkinManager.GetImage("NewsFriendSelected");
            btnStatusUpdates.TransparentButtonPressed = MasterForm.SkinManager.GetImage("NewsFriendSelected");
            btnStatusUpdates.Size = new Size(82, 25);
            btnStatusUpdates.Location = new Point((int)((this.Width - 2 * btnStatusUpdates.Width) / 2), 
                (int)((this.header.Height - btnStatusUpdates.Height) / 2));
            btnStatusUpdates.Name = "btnStatusUpdates";
            btnStatusUpdates.Click += new EventHandler(BtnStatusUpdatesClick);

            //
            //btnStatusUpdatesText
            //
            btnStatusUpdatesText.Name = "btnStatusUpdatesText";
            btnStatusUpdatesText.Location = btnStatusUpdates.Location;
            btnStatusUpdatesText.Font = FontCache.CreateFont("Tahoma", 12, FontStyle.Bold, false);
            btnStatusUpdatesText.ForeColor = Color.White;
            btnStatusUpdatesText.VerticalTextAlignment = Galssoft.VKontakteWM.Components.UI.VerticalAlignment.Center;
            btnStatusUpdatesText.HorizontalTextAlignment = Galssoft.VKontakteWM.Components.UI.HorizontalAlignment.Center;
            btnStatusUpdatesText.Text = Resources.StatusUpdatesList_Designer_btnStatusUpdates_Text;

            //
            //btnStatusUpdatesShadowText
            //
            btnStatusUpdatesShadowText.Name = "btnStatusUpdatesShadowText";
            btnStatusUpdatesShadowText.Location = btnStatusUpdates.Location;
            btnStatusUpdatesShadowText.Font = FontCache.CreateFont("Tahoma", 12, FontStyle.Bold, false);
            btnStatusUpdatesShadowText.ForeColor = Color.FromArgb(0, 0, 0);
            btnStatusUpdatesShadowText.VerticalTextAlignment = Galssoft.VKontakteWM.Components.UI.VerticalAlignment.Center;
            btnStatusUpdatesShadowText.HorizontalTextAlignment = Galssoft.VKontakteWM.Components.UI.HorizontalAlignment.Center;
            btnStatusUpdatesShadowText.Text = Resources.StatusUpdatesList_Designer_btnStatusUpdates_Text;

            //
            //btnPhotoCommentsUpdates
            //
            btnPhotoCommentsUpdates.TransparentButton = MasterForm.SkinManager.GetImage("NewsComment");
            btnPhotoCommentsUpdates.TransparentButtonPressed = MasterForm.SkinManager.GetImage("NewsCommentSelected");
            btnPhotoCommentsUpdates.TransparentButtonSelected = MasterForm.SkinManager.GetImage("NewsCommentSelected");
            btnPhotoCommentsUpdates.Size = new Size(82, 25);
            btnPhotoCommentsUpdates.Location = new Point((int)((this.Width - 2 * btnStatusUpdates.Width) / 2) + btnPhotoCommentsUpdates.Width, 
                (int)((this.header.Height - btnPhotoCommentsUpdates.Height) / 2));
            btnPhotoCommentsUpdates.Name = "btnPhotoCommentsUpdates";
            btnPhotoCommentsUpdates.Click += new EventHandler(BtnPhotoCommentsUpdatesClick);

            //
            //btnPhotoCommentsUpdatesText
            //
            btnPhotoCommentsUpdatesText.Name = "btnPhotoCommentsUpdatesText";
            btnPhotoCommentsUpdatesText.Location = btnPhotoCommentsUpdates.Location;
            btnPhotoCommentsUpdatesText.Font = FontCache.CreateFont("Tahoma", 12, FontStyle.Bold, false);
            btnPhotoCommentsUpdatesText.ForeColor = Color.White;
            btnPhotoCommentsUpdatesText.VerticalTextAlignment = Galssoft.VKontakteWM.Components.UI.VerticalAlignment.Center;
            btnPhotoCommentsUpdatesText.HorizontalTextAlignment = Galssoft.VKontakteWM.Components.UI.HorizontalAlignment.Center;
            btnPhotoCommentsUpdatesText.Text = Resources.StatusUpdatesList_Designer_btnPhotoCommentsUpdates_Text;

            //
            //btnPhotoCommentsUpdatesShadowText
            //
            btnPhotoCommentsUpdatesShadowText.Name = "btnPhotoCommentsUpdatesShadowText";
            btnPhotoCommentsUpdatesShadowText.Location = btnPhotoCommentsUpdates.Location;
            btnPhotoCommentsUpdatesShadowText.Font = FontCache.CreateFont("Tahoma", 12, FontStyle.Bold, false);
            btnPhotoCommentsUpdatesShadowText.ForeColor = Color.FromArgb(0, 0, 0);
            btnPhotoCommentsUpdatesShadowText.VerticalTextAlignment = Galssoft.VKontakteWM.Components.UI.VerticalAlignment.Center;
            btnPhotoCommentsUpdatesShadowText.HorizontalTextAlignment = Galssoft.VKontakteWM.Components.UI.HorizontalAlignment.Center;
            btnPhotoCommentsUpdatesShadowText.Text = Resources.StatusUpdatesList_Designer_btnPhotoCommentsUpdates_Text;

            //
            //btnRefresh
            //
            btnRefresh.TransparentButton = MasterForm.SkinManager.GetImage("RefreshButton");
            btnRefresh.TransparentButtonPressed = MasterForm.SkinManager.GetImage("RefreshButtonPressed");
            btnRefresh.Location = new Point(211, 5);
            btnRefresh.Size = new Size(25, 25);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefresh.Click += new EventHandler(BtnRefreshClick);

            //
            // contextMenu
            //
            contexMenu.MenuItems.Add(this.menuItemSendMessage);

            //
            // menuItemSendMessage
            //
            menuItemSendMessage.Text = Resources.ContextMenu_SendMessade;
            menuItemSendMessage.Click += new EventHandler(menuItemSendMessage_Click);

            //
            //klvStatusUpdatesList
            //
            klvStatusUpdatesList.Anchor =  System.Windows.Forms.AnchorStyles.Left
                                           | System.Windows.Forms.AnchorStyles.Right
                                           | System.Windows.Forms.AnchorStyles.Top
                                           | System.Windows.Forms.AnchorStyles.Bottom;
            klvStatusUpdatesList.Location = new System.Drawing.Point(0, 36);
            klvStatusUpdatesList.BackColor = Color.White;
            klvStatusUpdatesList.BackgroundIImage = MasterForm.SkinManager.GetImage("List-background");
            klvStatusUpdatesList.ContentUpShadow = MasterForm.SkinManager.GetImage("ContentUpShadow");
            klvStatusUpdatesList.ContentDownShadow = MasterForm.SkinManager.GetImage("ContentDownShadow");
            klvStatusUpdatesList.ShowContentShadows = true;
            klvStatusUpdatesList.OutsideDownShadow = MasterForm.SkinManager.GetImage("ContentUpShadow");
            klvStatusUpdatesList.OutsideUpShadow = MasterForm.SkinManager.GetImage("ContentDownShadow");
            klvStatusUpdatesList.ShowInnerShadows = true;
            klvStatusUpdatesList.ScrollAction = KineticControlBase.KineticControlScrollAction.ScrollingForTime;
            klvStatusUpdatesList.Name = "klvStatusUpdatesList";
            klvStatusUpdatesList.Size = new System.Drawing.Size(240, 128);
            klvStatusUpdatesList.ShowInnerBottomShadowToplevel = true;
            klvStatusUpdatesList.ReturnLongPress += new EventHandler<ListViewLongPressEventArgs>(klvStatusUpdatesList_ReturnLongPress);
            klvStatusUpdatesList.MouseUp += new MouseEventHandler(klvStatusUpdatesList_MouseUp);

            //
            //klvImagesList
            //
            klvImagesList.Anchor =  System.Windows.Forms.AnchorStyles.Left
                                           | System.Windows.Forms.AnchorStyles.Right
                                           | System.Windows.Forms.AnchorStyles.Bottom;
            klvImagesList.Location = new System.Drawing.Point(0, 164);
            klvImagesList.BackgroundIImage = MasterForm.SkinManager.GetImage("FilmStrip");
            klvImagesList.Name = "klvImagesList";
            klvImagesList.Size = new System.Drawing.Size(240, 65);
            klvImagesList.Select += new ItemSelectedEvent(klvImagesList_Select);

            //
            // toolBar
            //
            toolBar.ToolbarButtonMessages.Click += new EventHandler(ButtonMessagesClick);
            toolBar.ToolbarButtonFriends.Click += new EventHandler(ButtonFriendsClick);
            toolBar.ToolbarButtonPhotos.Click += new EventHandler(ButtonPhotosClick);
            toolBar.ToolbarButtonExtras.Click += new EventHandler(ButtonExtrasClick);

            toolBar.miUserData.Click += new EventHandler(MiUserDataClick);
            toolBar.miSettings.Click += new EventHandler(MiSettingsClick);
            toolBar.miAbout.Click += new EventHandler(MiAboutClick);
            toolBar.miExit.Click += new EventHandler(MiExitClick);

            this.Canvas.Children.Add(header);
            this.Canvas.Children.Add(logo);
            this.Canvas.Children.Add(toolBar);
            this.Canvas.Children.Add(btnStatusUpdates);
            this.Canvas.Children.Add(btnPhotoCommentsUpdates);
            this.Canvas.Children.Add(btnRefresh);
            this.Canvas.Children.Add(btnPhotoCommentsUpdatesShadowText);
            this.Canvas.Children.Add(btnPhotoCommentsUpdatesText);
            this.Canvas.Children.Add(btnStatusUpdatesShadowText);
            this.Canvas.Children.Add(btnStatusUpdatesText);
            this.Controls.Add(klvStatusUpdatesList);
            this.Controls.Add(klvImagesList);

            this.Canvas.RecalcDPIScaling();
            this.ResumeLayout(false);

            this.FirstRenderComplete += new EventHandler(StatusUpdatesListView_FirstRenderComplete);
            this.header.AlphaChannelImage = MasterForm.SkinManager.GetImage("Header");
            this.logo.AlphaChannelImage = MasterForm.SkinManager.GetImage("HeaderLogo");

            btnStatusUpdatesShadowText.Location = new Point(
                btnStatusUpdatesShadowText.Location.X,
                btnStatusUpdatesShadowText.Location.Y - 1
                );

            btnPhotoCommentsUpdatesShadowText.Location = new Point(
                btnPhotoCommentsUpdatesShadowText.Location.X,
                btnPhotoCommentsUpdatesShadowText.Location.Y - 1
                );

            btnPhotoCommentsUpdatesText.Size = btnPhotoCommentsUpdatesShadowText.Size = btnPhotoCommentsUpdates.Size;
            btnStatusUpdatesText.Size = btnStatusUpdatesShadowText.Size = btnStatusUpdates.Size;    
        }

        #endregion

        private BottomToolBar toolBar;

        private UIButton btnStatusUpdates;
        private UIButton btnPhotoCommentsUpdates;
        private UIButton btnRefresh;

        private UILabel btnStatusUpdatesText;
        private UILabel btnStatusUpdatesShadowText;
        private UILabel btnPhotoCommentsUpdatesText;
        private UILabel btnPhotoCommentsUpdatesShadowText;

        private GraphicsImage header;
        private GraphicsImage logo;

        private StatusUpdatesListKineticListView klvStatusUpdatesList;
        private ImagesKineticListView klvImagesList;

        private ContextMenu contexMenu;
        private MenuItem menuItemSendMessage;   
    }
}
