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
    partial class PhotoCommentsUpdatesListView
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
            //this.toolBar = new ToolBar();
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

            this.klvPhotoCommentsUpdatesList = new PhotoCommentsUpdatesListKineticListView();

            this.contexMenu = new ContextMenu();
            this.menuItemSendMessage = new MenuItem();
            this.menuItemViewComments = new MenuItem();
            this.menuItemSendComment = new MenuItem();

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
            this.Name = "PhotoCommentsUpdatesListView";
            this.Resize += new EventHandler(PhotoCommentsUpdatesListViewResize);

            //
            //btnStatusUpdates
            //
            btnStatusUpdates.TransparentButton = MasterForm.SkinManager.GetImage("NewsFriend");
            btnStatusUpdates.TransparentButtonPressed = MasterForm.SkinManager.GetImage("NewsFriendSelected");
            btnStatusUpdates.TransparentButtonSelected = MasterForm.SkinManager.GetImage("NewsFriendSelected");
            btnStatusUpdates.Size = new Size(82, 25);
            btnStatusUpdates.Location = new Point((int)((this.Width - 2 * btnStatusUpdates.Width) / 2), 5);
            btnStatusUpdates.Name = "btnStatusUpdates";
            btnStatusUpdates.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnStatusUpdates.Click += new EventHandler(btnStatusUpdates_Click);

            //
            //btnStatusUpdatesText
            //
            btnStatusUpdatesText.Name = "btnStatusUpdatesText";
            btnStatusUpdatesText.Location = btnStatusUpdates.Location;
            btnStatusUpdatesText.Font = FontCache.CreateFont("Tahoma", 12, FontStyle.Bold, false);
            btnStatusUpdatesText.ForeColor = Color.White;
            btnStatusUpdatesText.VerticalTextAlignment = Galssoft.VKontakteWM.Components.UI.VerticalAlignment.Center;
            btnStatusUpdatesText.HorizontalTextAlignment = Galssoft.VKontakteWM.Components.UI.HorizontalAlignment.Center;
            btnStatusUpdatesText.Anchor = AnchorStyles.Top | AnchorStyles.Left;
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
            btnStatusUpdatesShadowText.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnStatusUpdatesShadowText.Text = Resources.StatusUpdatesList_Designer_btnStatusUpdates_Text;

            //
            //btnPhotoCommentsUpdates
            //
            btnPhotoCommentsUpdates.TransparentButton = MasterForm.SkinManager.GetImage("NewsCommentSelected");
            btnPhotoCommentsUpdates.TransparentButtonPressed = MasterForm.SkinManager.GetImage("NewsCommentSelected");
            btnPhotoCommentsUpdates.Size = new Size(82, 25);
            btnPhotoCommentsUpdates.Location = new Point((int)((this.Width - 2 * btnStatusUpdates.Width) / 2) + btnPhotoCommentsUpdates.Width, 5);
            btnPhotoCommentsUpdates.Name = "btnPhotoCommentsUpdates";
            btnPhotoCommentsUpdates.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPhotoCommentsUpdates.Click += new EventHandler(btnPhotoCommentsUpdates_Click);

            //
            //btnPhotoCommentsUpdatesText
            //
            btnPhotoCommentsUpdatesText.Name = "btnPhotoCommentsUpdatesText";
            btnPhotoCommentsUpdatesText.Location = btnPhotoCommentsUpdates.Location;
            btnPhotoCommentsUpdatesText.Font = FontCache.CreateFont("Tahoma", 12, FontStyle.Bold, false);
            btnPhotoCommentsUpdatesText.ForeColor = Color.White;
            btnPhotoCommentsUpdatesText.VerticalTextAlignment = Galssoft.VKontakteWM.Components.UI.VerticalAlignment.Center;
            btnPhotoCommentsUpdatesText.HorizontalTextAlignment = Galssoft.VKontakteWM.Components.UI.HorizontalAlignment.Center;
            btnPhotoCommentsUpdatesText.Anchor = AnchorStyles.Top | AnchorStyles.Right;
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
            btnPhotoCommentsUpdatesShadowText.Anchor = AnchorStyles.Top | AnchorStyles.Right;
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
            btnRefresh.Click += new EventHandler(btnRefresh_Click);

            //
            // contextMenu
            //
            contexMenu.MenuItems.Add(this.menuItemViewComments);
            contexMenu.MenuItems.Add(this.menuItemSendComment);
            contexMenu.MenuItems.Add(this.menuItemSendMessage);

            //
            // menuItemSendMessage
            //
            menuItemSendMessage.Text = Resources.ContextMenu_SendMessade;
            menuItemSendMessage.Click += new EventHandler(menuItemSendMessage_Click);

            //
            // menuItemViewComments
            //
            menuItemViewComments.Text = Resources.ContextMenu_ViewComments;
            menuItemViewComments.Click += new EventHandler(menuItemViewComments_Click);

            //
            // menuItemSendComment
            //
            menuItemSendComment.Text = Resources.ContextMenu_SendComment;
            menuItemSendComment.Click += new EventHandler(menuItemSendComment_Click);

            //
            //klvPhotoCommentsUpdatesList
            //
            klvPhotoCommentsUpdatesList.Anchor = System.Windows.Forms.AnchorStyles.Left
                                           | System.Windows.Forms.AnchorStyles.Right
                                           | System.Windows.Forms.AnchorStyles.Top
                                           | System.Windows.Forms.AnchorStyles.Bottom;
            klvPhotoCommentsUpdatesList.Location = new System.Drawing.Point(0, 36);
            klvPhotoCommentsUpdatesList.BackColor = Color.White;
            klvPhotoCommentsUpdatesList.BackgroundIImage = MasterForm.SkinManager.GetImage("List-background");
            klvPhotoCommentsUpdatesList.ContentUpShadow = MasterForm.SkinManager.GetImage("ContentUpShadow");
            klvPhotoCommentsUpdatesList.ContentDownShadow = MasterForm.SkinManager.GetImage("ContentDownShadow");
            klvPhotoCommentsUpdatesList.ShowContentShadows = true;
            klvPhotoCommentsUpdatesList.OutsideDownShadow = MasterForm.SkinManager.GetImage("ContentUpShadow");
            klvPhotoCommentsUpdatesList.OutsideUpShadow = MasterForm.SkinManager.GetImage("ContentDownShadow");
            klvPhotoCommentsUpdatesList.ShowInnerShadows = true;
            klvPhotoCommentsUpdatesList.ScrollAction = KineticControlBase.KineticControlScrollAction.ScrollingForTime;
            klvPhotoCommentsUpdatesList.Name = "klvPhotoCommentsUpdatesList";
            klvPhotoCommentsUpdatesList.Size = new System.Drawing.Size(240, 193);
            klvPhotoCommentsUpdatesList.ReturnLongPress += new EventHandler<ListViewLongPressEventArgs>(klvPhotoCommentsUpdatesList_ReturnLongPress);
            klvPhotoCommentsUpdatesList.MouseUp += new MouseEventHandler(klvPhotoCommentsUpdatesList_MouseUp);

            //
            // toolBar
            //
            toolBar.ToolbarButtonMessages.Click += new EventHandler(ToolbarButtonMessages_Click);
            toolBar.ToolbarButtonFriends.Click += new EventHandler(ToolbarButtonFriends_Click);
            toolBar.ToolbarButtonPhotos.Click += new EventHandler(ToolbarButtonPhotos_Click);
            toolBar.ToolbarButtonExtras.Click += new EventHandler(ToolbarButtonExtras_Click);

            toolBar.miUserData.Click += new EventHandler(MiUserDataClick);
            toolBar.miSettings.Click += new EventHandler(MiSettingsClick);
            toolBar.miAbout.Click += new EventHandler(MiAboutClick);
            toolBar.miExit.Click += new EventHandler(MiExitClick);

            this.Canvas.Children.Add(header);
            this.Canvas.Children.Add(logo);
            this.Canvas.Children.Add(toolBar);
            this.Canvas.Children.Add(btnStatusUpdates);
            this.Canvas.Children.Add(btnPhotoCommentsUpdates);
            this.Canvas.Children.Add(btnPhotoCommentsUpdatesShadowText);
            this.Canvas.Children.Add(btnPhotoCommentsUpdatesText);
            this.Canvas.Children.Add(btnStatusUpdatesShadowText);
            this.Canvas.Children.Add(btnStatusUpdatesText);
            this.Canvas.Children.Add(btnRefresh);
            this.Controls.Add(klvPhotoCommentsUpdatesList);

            this.Canvas.RecalcDPIScaling();

            this.ResumeLayout(false);

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

        private PhotoCommentsUpdatesListKineticListView klvPhotoCommentsUpdatesList;

        private ContextMenu contexMenu;
        private MenuItem menuItemSendMessage;
        private MenuItem menuItemViewComments;
        private MenuItem menuItemSendComment; 
    }
}
