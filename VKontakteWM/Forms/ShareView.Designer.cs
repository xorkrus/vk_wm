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
    partial class ShareView
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

            //btnStatusUpdates = new UIButton(ButtonStyle.AlphaChannel);
            //btnPhotoCommentsUpdates = new UIButton(ButtonStyle.AlphaChannel);
            btnRefresh = new UIButton(ButtonStyle.AlphaChannel);

            //btnPhotoCommentsUpdatesText = new UILabel();
            //btnPhotoCommentsUpdatesShadowText = new UILabel();
            //btnStatusUpdatesText = new UILabel();
            //btnStatusUpdatesShadowText = new UILabel();

            header = new GraphicsImage();
            logo = new GraphicsImage();
            buttonBack = new GraphicsImage();

            this.headerText = new UILabel();
            this.headerShadowText = new UILabel();

            this.klvStatusUpdatesList = new StatusHistoryKineticList();
            //this.klvImagesList = new ImagesKineticListView();

            //this.contexMenu = new ContextMenu();
            //this.menuItemSendMessage = new MenuItem();

            this.wbwiChangeStatus = new WideButtonWithIcon(WideButtonWithIconType.Write);
            this.wbwiSnapPhoto = new WideButtonWithIcon(WideButtonWithIconType.Photo);
            this.wbwiLoadImage = new WideButtonWithIcon(WideButtonWithIconType.Open);

            this.SuspendLayout();

            this.header.Name = "header";
            this.header.Location = new Point(0, 0);
            this.header.Stretch = true;
            this.header.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            this.logo.Name = "logo";
            this.logo.Location = new Point(6, 7);
            this.logo.Stretch = false;
            this.logo.Anchor = AnchorStyles.Left | AnchorStyles.Top;

            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Location = new Point(0, 124);
            //this.buttonBack.Size = new Size(240, 105);
            this.buttonBack.Stretch = true;
            this.buttonBack.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;

            //
            // headerText
            //
            this.headerText.Name = "headerText";
            this.headerText.Location = new Point(0, 0);
            this.headerText.Size = new Size(240, 36);
            this.headerText.Font = FontCache.CreateFont("Tahoma", 14, FontStyle.Bold);
            this.headerText.ForeColor = Color.White;
            this.headerText.VerticalTextAlignment = Galssoft.VKontakteWM.Components.UI.VerticalAlignment.Center;
            this.headerText.HorizontalTextAlignment = Galssoft.VKontakteWM.Components.UI.HorizontalAlignment.Center;
            this.headerText.Text = Resources.ShareView_Header;
            this.headerText.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            //
            // headerShadowText
            //
            this.headerShadowText.Name = "headerShadowText";
            this.headerShadowText.Location = new Point(0, 0);
            this.headerShadowText.Size = new Size(240, 36);
            this.headerShadowText.Font = FontCache.CreateFont("Tahoma", 14, FontStyle.Bold);
            this.headerShadowText.ForeColor = Color.Black;
            this.headerShadowText.VerticalTextAlignment = Galssoft.VKontakteWM.Components.UI.VerticalAlignment.Center;
            this.headerShadowText.HorizontalTextAlignment = Galssoft.VKontakteWM.Components.UI.HorizontalAlignment.Center;
            this.headerShadowText.Text = Resources.ShareView_Header;
            this.headerShadowText.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            this.Size = new Size(240, 268);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.Name = "StatusUpdatesListView";
            this.ForeColor = Color.White;
            //this.Resize += new EventHandler(StatusUpdatesListViewResize);

            toolBar.ToolbarButtonNews.Click += new EventHandler(ToolbarButtonNewsClick);
            toolBar.ToolbarButtonMessages.Click += new EventHandler(ToolbarButtonMessagesClick);
            toolBar.ToolbarButtonFriends.Click += new EventHandler(ToolbarButtonFriendsClick);
            toolBar.ToolbarButtonExtras.Click += new EventHandler(ToolbarButtonExtrasClick);

            // 
            // upcChangeStatus
            //
            this.wbwiChangeStatus.Location = new System.Drawing.Point(5, 124 + 8);
            this.wbwiChangeStatus.Size = new System.Drawing.Size(231, 25);
            this.wbwiChangeStatus.Text = Resources.ShareView_Button_Write;
            this.wbwiChangeStatus.Font = FontCache.CreateFont("Arial", 12, FontStyle.Bold, true);
            this.wbwiChangeStatus.PressedFont = FontCache.CreateFont("Arial", 12, FontStyle.Bold, true);
            this.wbwiChangeStatus.FontColor = Color.FromArgb(102, 102, 102);
            this.wbwiChangeStatus.PressedFontColor = Color.FromArgb(102, 102, 102);
            this.wbwiChangeStatus.FontColorShadow = Color.FromArgb(223, 223, 223);
            //this.wbwiChangeStatus.FontColorShadow = Color.White;
            this.wbwiChangeStatus.DropShadow = false;
            this.wbwiChangeStatus.Name = "upcChangeStatus";
            this.wbwiChangeStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left;
            this.wbwiChangeStatus.Click += new EventHandler(wbwiChangeStatus_Click);

            // 
            // upcChangeStatus
            //
            this.wbwiSnapPhoto.Location = new System.Drawing.Point(5, 124 + 8 + 24 + 8);
            this.wbwiSnapPhoto.Size = new System.Drawing.Size(231, 25);
            this.wbwiSnapPhoto.Text = Resources.ShareView_Button_Photo;
            this.wbwiSnapPhoto.Font = FontCache.CreateFont("Arial", 12, FontStyle.Bold, true);
            this.wbwiSnapPhoto.PressedFont = FontCache.CreateFont("Arial", 12, FontStyle.Bold, true);
            this.wbwiSnapPhoto.FontColor = Color.FromArgb(102, 102, 102);
            this.wbwiSnapPhoto.PressedFontColor = Color.FromArgb(102, 102, 102);
            this.wbwiSnapPhoto.FontColorShadow = Color.FromArgb(223, 223, 223);
            //this.wbwiSnapPhoto.FontColorShadow = Color.White;
            this.wbwiSnapPhoto.DropShadow = false;
            this.wbwiSnapPhoto.Name = "upcSnapPhoto";
            this.wbwiSnapPhoto.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left;
            this.wbwiSnapPhoto.Click += new EventHandler(wbwiSnapPhoto_Click);

            // 
            // upcChangeStatus
            //
            this.wbwiLoadImage.Location = new System.Drawing.Point(5, 124 + 8 + 24 + 8 + 24 + 8);
            this.wbwiLoadImage.Size = new System.Drawing.Size(231, 25);
            this.wbwiLoadImage.Text = Resources.ShareView_Button_Open;
            this.wbwiLoadImage.Font = FontCache.CreateFont("Arial", 12, FontStyle.Bold, true);
            this.wbwiLoadImage.PressedFont = FontCache.CreateFont("Arial", 12, FontStyle.Bold, true);
            this.wbwiLoadImage.FontColor = Color.FromArgb(102, 102, 102);
            this.wbwiLoadImage.PressedFontColor = Color.FromArgb(102, 102, 102);
            this.wbwiLoadImage.FontColorShadow = Color.FromArgb(223, 223, 223);
            //this.wbwiLoadImage.FontColorShadow = Color.White;
            this.wbwiLoadImage.DropShadow = false;
            this.wbwiLoadImage.Name = "upcLoadImage";
            this.wbwiLoadImage.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left;
            this.wbwiLoadImage.Click += new EventHandler(wbwiLoadImage_Click);

            //this.upcChangeStatus.Click += new EventHandler(upcUploadPhotoMobile_Click);

            ////
            ////btnStatusUpdates
            ////
            //btnStatusUpdates.TransparentButton = MasterForm.SkinManager.GetImage("NewsFriendSelected");
            //btnStatusUpdates.TransparentButtonPressed = MasterForm.SkinManager.GetImage("NewsFriendSelected");
            //btnStatusUpdates.Size = new Size(82, 25);
            //btnStatusUpdates.Location = new Point((int)((this.Width - 2 * btnStatusUpdates.Width) / 2), (int)((this.header.Height - btnStatusUpdates.Height) / 2));
            //btnStatusUpdates.Name = "btnStatusUpdates";
            //btnStatusUpdates.Click += new EventHandler(BtnStatusUpdatesClick);

            ////
            ////btnStatusUpdatesText
            ////
            //btnStatusUpdatesText.Name = "btnStatusUpdatesText";
            //btnStatusUpdatesText.Location = btnStatusUpdates.Location;
            //btnStatusUpdatesText.Font = FontCache.CreateFont("Tahoma", 10, FontStyle.Bold, false);
            //btnStatusUpdatesText.ForeColor = Color.White;
            //btnStatusUpdatesText.VerticalTextAlignment = Galssoft.VKontakteWM.Components.UI.VerticalAlignment.Center;
            //btnStatusUpdatesText.HorizontalTextAlignment = Galssoft.VKontakteWM.Components.UI.HorizontalAlignment.Center;
            //btnStatusUpdatesText.Text = Resources.StatusUpdatesList_Designer_btnStatusUpdates_Text;

            ////
            ////btnStatusUpdatesShadowText
            ////
            //btnStatusUpdatesShadowText.Name = "btnStatusUpdatesShadowText";
            //btnStatusUpdatesShadowText.Location = btnStatusUpdates.Location;
            //btnStatusUpdatesShadowText.Font = FontCache.CreateFont("Tahoma", 10, FontStyle.Bold, false);
            //btnStatusUpdatesShadowText.ForeColor = Color.FromArgb(120, 120, 120);
            //btnStatusUpdatesShadowText.VerticalTextAlignment = Galssoft.VKontakteWM.Components.UI.VerticalAlignment.Center;
            //btnStatusUpdatesShadowText.HorizontalTextAlignment = Galssoft.VKontakteWM.Components.UI.HorizontalAlignment.Center;
            //btnStatusUpdatesShadowText.Text = Resources.StatusUpdatesList_Designer_btnStatusUpdates_Text;

            ////
            ////btnPhotoCommentsUpdates
            ////
            //btnPhotoCommentsUpdates.TransparentButton = MasterForm.SkinManager.GetImage("NewsComment");
            //btnPhotoCommentsUpdates.TransparentButtonPressed = MasterForm.SkinManager.GetImage("NewsComment");
            //btnPhotoCommentsUpdates.Size = new Size(82, 25);
            //btnPhotoCommentsUpdates.Location = new Point((int)((this.Width - 2 * btnStatusUpdates.Width) / 2) + btnPhotoCommentsUpdates.Width, (int)((this.header.Height - btnPhotoCommentsUpdates.Height) / 2));
            //btnPhotoCommentsUpdates.Name = "btnPhotoCommentsUpdates";
            //btnPhotoCommentsUpdates.Click += new EventHandler(BtnPhotoCommentsUpdatesClick);

            ////
            ////btnPhotoCommentsUpdatesText
            ////
            //btnPhotoCommentsUpdatesText.Name = "btnPhotoCommentsUpdatesText";
            //btnPhotoCommentsUpdatesText.Location = btnPhotoCommentsUpdates.Location;
            //btnPhotoCommentsUpdatesText.Font = FontCache.CreateFont("Tahoma", 10, FontStyle.Bold, false);
            //btnPhotoCommentsUpdatesText.ForeColor = Color.White;
            //btnPhotoCommentsUpdatesText.VerticalTextAlignment = Galssoft.VKontakteWM.Components.UI.VerticalAlignment.Center;
            //btnPhotoCommentsUpdatesText.HorizontalTextAlignment = Galssoft.VKontakteWM.Components.UI.HorizontalAlignment.Center;
            //btnPhotoCommentsUpdatesText.Text = Resources.StatusUpdatesList_Designer_btnPhotoCommentsUpdates_Text;

            ////
            ////btnPhotoCommentsUpdatesShadowText
            ////
            //btnPhotoCommentsUpdatesShadowText.Name = "btnPhotoCommentsUpdatesShadowText";
            //btnPhotoCommentsUpdatesShadowText.Location = btnPhotoCommentsUpdates.Location;
            //btnPhotoCommentsUpdatesShadowText.Font = FontCache.CreateFont("Tahoma", 10, FontStyle.Bold, false);
            //btnPhotoCommentsUpdatesShadowText.ForeColor = Color.FromArgb(120, 120, 120);
            //btnPhotoCommentsUpdatesShadowText.VerticalTextAlignment = Galssoft.VKontakteWM.Components.UI.VerticalAlignment.Center;
            //btnPhotoCommentsUpdatesShadowText.HorizontalTextAlignment = Galssoft.VKontakteWM.Components.UI.HorizontalAlignment.Center;
            //btnPhotoCommentsUpdatesShadowText.Text = Resources.StatusUpdatesList_Designer_btnPhotoCommentsUpdates_Text;

            //
            //btnRefresh
            //
            btnRefresh.Location = new Point(211, 5);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefresh.Click += new EventHandler(btnRefresh_Click);
            //btnRefresh.Click += new EventHandler(BtnRefreshClick);

            ////
            //// contextMenu
            ////
            //contexMenu.MenuItems.Add(this.menuItemSendMessage);

            ////
            //// menuItemSendMessage
            ////
            //menuItemSendMessage.Text = Resources.ContextMenu_SendMessade;
            //menuItemSendMessage.Click += new EventHandler(menuItemSendMessage_Click);

            //
            // klvStatusUpdatesList
            //
            klvStatusUpdatesList.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom;
            klvStatusUpdatesList.Location = new System.Drawing.Point(0, 36);
            klvStatusUpdatesList.BackColor = Color.White;
            klvStatusUpdatesList.BackgroundIImage = MasterForm.SkinManager.GetImage("List-background");
            klvStatusUpdatesList.ContentUpShadow = MasterForm.SkinManager.GetImage("ContentUpShadow");
            klvStatusUpdatesList.ContentDownShadow = MasterForm.SkinManager.GetImage("ContentDownShadow");
            klvStatusUpdatesList.ShowContentShadows = true;
            klvStatusUpdatesList.OutsideDownShadow = MasterForm.SkinManager.GetImage("ContentUpShadow");
            klvStatusUpdatesList.OutsideUpShadow = MasterForm.SkinManager.GetImage("ContentDownShadow");
            klvStatusUpdatesList.ShowInnerShadows = true;
            klvStatusUpdatesList.ShowInnerBottomShadowToplevel = true;
            klvStatusUpdatesList.ScrollAction = KineticControlBase.KineticControlScrollAction.ScrollingForTime;
            klvStatusUpdatesList.Name = "klvStatusUpdatesList";
            klvStatusUpdatesList.Size = new System.Drawing.Size(240, 88);

            toolBar.miUserData.Click += new EventHandler(MiUserDataClick);
            toolBar.miSettings.Click += new EventHandler(MiSettingsClick);
            toolBar.miAbout.Click += new EventHandler(MiAboutClick);
            toolBar.miExit.Click += new EventHandler(MiExitClick);

            //klvStatusUpdatesList.ReturnLongPress += new EventHandler<ListViewLongPressEventArgs>(klvStatusUpdatesList_ReturnLongPress);

            ////
            ////klvImagesList
            ////
            //klvImagesList.Anchor = System.Windows.Forms.AnchorStyles.Left
            //                               | System.Windows.Forms.AnchorStyles.Right
            //                               | System.Windows.Forms.AnchorStyles.Bottom;
            //klvImagesList.Location = new System.Drawing.Point(0, 163);
            //klvImagesList.BackgroundIImage = MasterForm.SkinManager.GetImage("FilmStrip");
            //klvImagesList.Name = "klvImagesList";
            //klvImagesList.Size = new System.Drawing.Size(240, 65);
            //klvImagesList.Select += new ItemSelectedEvent(klvImagesList_Select);

            ////
            //// toolBar
            ////
            //toolBar.ToolbarButtonMessages.Click += new EventHandler(ButtonMessagesClick);
            //toolBar.ToolbarButtonFriends.Click += new EventHandler(ButtonFriendsClick);
            //toolBar.ToolbarButtonPhotos.Click += new EventHandler(ButtonPhotosClick);
            //toolBar.ToolbarButtonExtras.Click += new EventHandler(ButtonExtrasClick);

            this.Canvas.Children.Add(header);
            this.Canvas.Children.Add(logo);

            this.Canvas.Children.Add(headerShadowText);
            this.Canvas.Children.Add(headerText);

            this.Canvas.Children.Add(toolBar);

            this.Canvas.Children.Add(buttonBack);

            this.Canvas.Children.Add(wbwiChangeStatus);
            this.Canvas.Children.Add(wbwiSnapPhoto);
            this.Canvas.Children.Add(wbwiLoadImage);

            //this.Canvas.Children.Add(btnStatusUpdates);
            //this.Canvas.Children.Add(btnPhotoCommentsUpdates);
            this.Canvas.Children.Add(btnRefresh);
            //this.Canvas.Children.Add(btnPhotoCommentsUpdatesShadowText);
            //this.Canvas.Children.Add(btnPhotoCommentsUpdatesText);
            //this.Canvas.Children.Add(btnStatusUpdatesShadowText);
            //this.Canvas.Children.Add(btnStatusUpdatesText);

            this.Controls.Add(klvStatusUpdatesList);

            //this.Controls.Add(klvImagesList);

            this.Canvas.RecalcDPIScaling();
            this.ResumeLayout(false);

            this.headerShadowText.Location = new Point(this.headerShadowText.Location.X, this.headerShadowText.Location.Y - UISettings.CalcPix(1));

            //this.FirstRenderComplete += new EventHandler(MainView_FirstRenderComplete);
            this.header.AlphaChannelImage = MasterForm.SkinManager.GetImage("Header");
            this.logo.AlphaChannelImage = MasterForm.SkinManager.GetImage("HeaderLogo");
            this.buttonBack.AlphaChannelImage = MasterForm.SkinManager.GetImage("ButtonBackground");

            this.btnRefresh.TransparentButton = MasterForm.SkinManager.GetImage("RefreshButton");
            this.btnRefresh.TransparentButtonPressed = MasterForm.SkinManager.GetImage("RefreshButtonPressed");

            //btnStatusUpdatesShadowText.Location = new Point(
            //    btnStatusUpdatesShadowText.Location.X - 1,
            //    btnStatusUpdatesShadowText.Location.Y - 1
            //    );

            //btnPhotoCommentsUpdatesShadowText.Location = new Point(
            //    btnPhotoCommentsUpdatesShadowText.Location.X - 1,
            //    btnPhotoCommentsUpdatesShadowText.Location.Y - 1
            //    );

            //btnPhotoCommentsUpdatesText.Size = btnPhotoCommentsUpdatesShadowText.Size = btnPhotoCommentsUpdates.Size;
            //btnStatusUpdatesText.Size = btnStatusUpdatesShadowText.Size = btnStatusUpdates.Size;  


        }

        #endregion

        private BottomToolBar toolBar;

        //private UIButton btnStatusUpdates;
        //private UIButton btnPhotoCommentsUpdates;
        private UIButton btnRefresh;

        //private UILabel btnStatusUpdatesText;
        //private UILabel btnStatusUpdatesShadowText;
        //private UILabel btnPhotoCommentsUpdatesText;
        //private UILabel btnPhotoCommentsUpdatesShadowText;

        private GraphicsImage header;
        private GraphicsImage logo;
        private GraphicsImage buttonBack;

        private StatusHistoryKineticList klvStatusUpdatesList;
        //private ImagesKineticListView klvImagesList;

        //private ContextMenu contexMenu;
        //private MenuItem menuItemSendMessage;  

        private WideButtonWithIcon wbwiChangeStatus;
        private WideButtonWithIcon wbwiSnapPhoto;
        private WideButtonWithIcon wbwiLoadImage;

        private UILabel headerText;
        private UILabel headerShadowText;
    }
}
