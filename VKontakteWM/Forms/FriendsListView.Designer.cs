using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.ApplicationLogic;
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
    partial class FriendsListView
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

            btnRefresh = new UIButton(ButtonStyle.AlphaChannel);
            btnFriendsOnline = new UIButton(ButtonStyle.AlphaChannel);
            btnFriendsNew = new UIButton(ButtonStyle.AlphaChannel);
            header = new GraphicsImage();
            logo = new GraphicsImage();
            filter = new FilterControl(163, 25);

            this.klvFriendsList = new FriendsListKineticListView();

            this.contexMenu = new ContextMenu();
            this.menuItemSendMessage = new MenuItem();
            this.menuItemSaveNumber = new MenuItem();
            this.menuItemCallNumber = new MenuItem();
            this.menuItemSendSms = new MenuItem();
            cancelMenuItem = new MenuItem();

            this.SuspendLayout();

            //
            //cancelMenuItem
            //
            cancelMenuItem.Text = Resources.AboutView_Button_Back;
            cancelMenuItem.Click +=new EventHandler(CancelMenuItemClick);

            //
            // filterMenu
            //
            filterMenu = new MainMenu();
            filterMenu.MenuItems.Add(cancelMenuItem);

            //
            // header
            //)
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
            // this
            //
            this.Size = new Size(240, 268);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.Resize += new EventHandler(FriendsListView_Resize);
            this.Name = "FriendsListView";

            //
            // contextMenu
            //
            contexMenu.MenuItems.Add(this.menuItemSendMessage);
            contexMenu.MenuItems.Add(this.menuItemSaveNumber);
            contexMenu.MenuItems.Add(this.menuItemCallNumber);
            //contexMenu.MenuItems.Add(this.menuItemSendSms);
            //
            // menuItemSendMessage
            //
            menuItemSendMessage.Text = Resources.ContextMenu_SendMessade;
            menuItemSendMessage.Click += new EventHandler(menuItemSendMessage_Click);
            //
            // menuItemSaveNumber
            //
            menuItemSaveNumber.Text = Resources.ContextMenu_SaveNumber;
            menuItemSaveNumber.Click += new EventHandler(menuItemSaveNumber_Click);
            //
            // menuItemCallNumber
            //
            menuItemCallNumber.Text = Resources.ContextMenu_CallNumber;
            menuItemCallNumber.Click += new EventHandler(menuItemCallNumber_Click);
            //
            // menuItemSendSms
            //
            menuItemSendSms.Text = Resources.ContextMenu_SendSms;
            menuItemSendSms.Click += new EventHandler(menuItemSendSms_Click);

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

            /*
            //
            //btnFriendsOnline
            //
            btnFriendsOnline.TransparentButton = MasterForm.SkinManager.GetImage("ButtonOther");
            btnFriendsOnline.TransparentButtonPressed = MasterForm.SkinManager.GetImage("ButtonOtherPressed");
            btnFriendsOnline.Location = new Point(80, 0);
            btnFriendsOnline.Size = new Size(80, 25);
            btnFriendsOnline.Text = Resources.FriendsList_Designer_btnFriendsOnline_Text;
            btnFriendsOnline.Name = "btnFriendsOnline";
            btnFriendsOnline.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnFriendsOnline.Click += new EventHandler(BtnFriendsOnlineClick);
          
            //
            //btnFriendsNew
            //
            btnFriendsNew.TransparentButton = MasterForm.SkinManager.GetImage("ButtonOther");
            btnFriendsNew.TransparentButtonPressed = MasterForm.SkinManager.GetImage("ButtonOtherPressed");
            btnFriendsNew.Location = new Point(160, 0);
            btnFriendsNew.Size = new Size(80, 25);
            btnFriendsNew.Text = Resources.FriendsList_Designer_btnFriendsNew_Text;
            btnFriendsNew.Name = "btnFriendsNew";
            btnFriendsNew.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnFriendsNew.Click += new EventHandler(BtnFriendsNewClick);
            */

            //
            //klvFriendsList
            //
            klvFriendsList.Anchor = System.Windows.Forms.AnchorStyles.Left
                                    | System.Windows.Forms.AnchorStyles.Right
                                    | System.Windows.Forms.AnchorStyles.Top
                                    | System.Windows.Forms.AnchorStyles.Bottom;
            klvFriendsList.Location = new System.Drawing.Point(0, 36);
            klvFriendsList.BackColor = Color.White;
            klvFriendsList.BackgroundIImage = MasterForm.SkinManager.GetImage("List-background");
            klvFriendsList.ContentUpShadow = MasterForm.SkinManager.GetImage("ContentUpShadow");
            klvFriendsList.ContentDownShadow = MasterForm.SkinManager.GetImage("ContentDownShadow");
            klvFriendsList.ShowContentShadows = true;
            klvFriendsList.OutsideDownShadow = MasterForm.SkinManager.GetImage("ContentUpShadow");
            klvFriendsList.OutsideUpShadow = MasterForm.SkinManager.GetImage("ContentDownShadow");
            klvFriendsList.ShowInnerShadows = true;
            klvFriendsList.ScrollAction = KineticControlBase.KineticControlScrollAction.ScrollingForTime;
            klvFriendsList.Name = "klvStatusUpdatesList";
            klvFriendsList.Size = new System.Drawing.Size(240, 193);
            klvFriendsList.ReturnLongPress += new EventHandler<ListViewLongPressEventArgs>(KlvFriendsListReturnLongPress);
            //klvFriendsList.Click += new EventHandler(KlvFriendsListClick);
            klvFriendsList.MouseUp += new MouseEventHandler(KlvFriendsListClick);

            //
            // toolBar
            //
            toolBar.ToolbarButtonNews.Click += new EventHandler(ButtonNewsClick);
            toolBar.ToolbarButtonMessages.Click += new EventHandler(ButtonMessagesClick);
            toolBar.ToolbarButtonPhotos.Click += new EventHandler(ButtonPhotosClick);
            toolBar.ToolbarButtonExtras.Click += new EventHandler(ButtonExtrasClick);   
            
            //
            // filter
            //
            filter.Location = new Point(39, 5);
            filter._filter.TextBox.TextChanged += new EventHandler(TextChange);
            filter._filter.TextBox.GotFocus += new EventHandler(TextBoxGotFocus);
            filter._filter.TextBox.LostFocus += new EventHandler(TextBoxLostFocus);
            filter.inputPanel.EnabledChanged +=new EventHandler(InputPanelEnabledChanged);

            toolBar.miUserData.Click += new EventHandler(MiUserDataClick);
            toolBar.miSettings.Click += new EventHandler(MiSettingsClick);
            toolBar.miAbout.Click += new EventHandler(MiAboutClick);
            toolBar.miExit.Click += new EventHandler(MiExitClick);

            this.Canvas.Children.Add(header);
            this.Canvas.Children.Add(logo);
            this.Canvas.Children.Add(toolBar);
            this.Canvas.Children.Add(btnRefresh);
            this.Canvas.Children.Add(btnFriendsOnline);
            this.Canvas.Children.Add(btnFriendsNew);
            this.Controls.Add(klvFriendsList);
            this.Controls.Add(filter);

            this.Canvas.RecalcDPIScaling();

            this.ResumeLayout(false);
            this.header.AlphaChannelImage = MasterForm.SkinManager.GetImage("Header");
            this.logo.AlphaChannelImage = MasterForm.SkinManager.GetImage("HeaderLogo");
        }

        #endregion

        private BottomToolBar toolBar;

        private UIButton btnRefresh;
        private UIButton btnFriendsOnline;
        private UIButton btnFriendsNew;

        private FilterControl filter;

        private GraphicsImage header;
        private GraphicsImage logo;

        private FriendsListKineticListView klvFriendsList;

        private ContextMenu contexMenu;
        private MenuItem menuItemSendMessage;
        private MenuItem menuItemSaveNumber;
        private MenuItem menuItemCallNumber;
        private MenuItem menuItemSendSms;
        private MenuItem cancelMenuItem;
        private MainMenu filterMenu;
    }
}
