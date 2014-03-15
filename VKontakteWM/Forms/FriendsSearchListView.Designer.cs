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
    partial class FriendsSearchListView
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
            mainMenu = new MainMenu();
            menuItemBack = new MenuItem();
            menuItemSelect = new MenuItem();
            header = new GraphicsImage();
            logo = new GraphicsImage();
            filter = new FilterControl(195, 25);
            this.klvFriendsList = new FriendsListKineticListView();

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
            // this
            //
            this.Size = new Size(240, 268);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.Resize += new EventHandler(FriendsListView_Resize);
            this.Name = "FriendsListView";

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
            klvFriendsList.OutsideUpShadow = MasterForm.SkinManager.GetImage("ContentDownShadow");
            klvFriendsList.OutsideDownShadow = MasterForm.SkinManager.GetImage("ContentUpShadow");
            klvFriendsList.ShowInnerShadows = true;
            klvFriendsList.ScrollAction = KineticControlBase.KineticControlScrollAction.ScrollingForTime;
            klvFriendsList.Name = "klvStatusUpdatesList";
            klvFriendsList.Size = new System.Drawing.Size(240, 232);
            klvFriendsList.MouseUp += new MouseEventHandler(KlvFriendsListClick);

            //
            // filter
            //
            filter.Location = new Point(39, 5);
            filter._filter.TextBox.TextChanged += new EventHandler(TextChange);
            filter._filter.TextBox.GotFocus += new EventHandler(TextBoxGotFocus);
            filter._filter.TextBox.LostFocus += new EventHandler(TextBoxLostFocus);

            //
            // mainMenu
            //
            mainMenu.MenuItems.Add(menuItemBack);
            mainMenu.MenuItems.Add(menuItemSelect);

            //
            // menuItemBack
            //
            menuItemBack.Text = Resources.FriendsSearchListViewMenuItemBack;
            menuItemBack.Click += new EventHandler(MenuItemBackClick);

            //
            // menuItemSelect
            //
            menuItemSelect.Text = Resources.FriendsSearchListViewMenuItemSelect;
            menuItemSelect.Click += new EventHandler(MenuItemSelectClick);

            this.Canvas.Children.Add(header);
            this.Canvas.Children.Add(logo);
            this.Controls.Add(klvFriendsList);
            this.Controls.Add(filter);

            this.Canvas.RecalcDPIScaling();

            this.ResumeLayout(false);
            this.header.AlphaChannelImage = MasterForm.SkinManager.GetImage("Header");
            this.logo.AlphaChannelImage = MasterForm.SkinManager.GetImage("HeaderLogo");
        }

        #endregion

        private FilterControl filter;
        private GraphicsImage header;
        private GraphicsImage logo;
        private FriendsListKineticListView klvFriendsList;
        private MainMenu mainMenu;
        private MenuItem menuItemBack;
        private MenuItem menuItemSelect;
    }
}
