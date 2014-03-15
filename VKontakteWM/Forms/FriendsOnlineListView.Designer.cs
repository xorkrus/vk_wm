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
    partial class FriendsOnlineListView
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
            btnFriendsAll = new UIButton(ButtonStyle.AlphaChannel);
            btnFriendsNew = new UIButton(ButtonStyle.AlphaChannel);

            this.klvFriendsOnlineList = new FriendsListKineticListView();

            this.SuspendLayout();

            this.Size = new Size(240, 268);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.Name = "FriendsListView";

            //
            //btnRefresh
            //
            btnRefresh.TransparentButton = MasterForm.SkinManager.GetImage("ButtonOther");
            btnRefresh.TransparentButtonPressed = MasterForm.SkinManager.GetImage("ButtonOtherPressed");
            btnRefresh.Location = new Point(0, 0);
            btnRefresh.Size = new Size(80, 25);
            btnRefresh.Text = Resources.FriendsList_Designer_btnRefresh_Text;
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnRefresh.Click += new EventHandler(BtnRefreshClick);

            //
            //btnFriendsAll
            //
            btnFriendsAll.TransparentButton = MasterForm.SkinManager.GetImage("ButtonOther");
            btnFriendsAll.TransparentButtonPressed = MasterForm.SkinManager.GetImage("ButtonOtherPressed");
            btnFriendsAll.Location = new Point(80, 0);
            btnFriendsAll.Size = new Size(80, 25);
            btnFriendsAll.Text = Resources.FriendsList_Designer_btnFriendsAll_Text;
            btnFriendsAll.Name = "btnFriendsOnline";
            btnFriendsAll.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnFriendsAll.Click += new EventHandler(BtnFriendsAllClick);

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

            //
            //klvFriendsList
            //
            klvFriendsOnlineList.Anchor = System.Windows.Forms.AnchorStyles.Left
                                    | System.Windows.Forms.AnchorStyles.Right
                                    | System.Windows.Forms.AnchorStyles.Top
                                    | System.Windows.Forms.AnchorStyles.Bottom;
            klvFriendsOnlineList.Location = new System.Drawing.Point(0, 25);
            klvFriendsOnlineList.BackColor = Color.White;
            klvFriendsOnlineList.ScrollAction = KineticControlBase.KineticControlScrollAction.ScrollingForTime;
            klvFriendsOnlineList.Name = "klvStatusUpdatesList";
            klvFriendsOnlineList.Size = new System.Drawing.Size(240, 203);

            //
            // toolBar
            //
            toolBar.ToolbarButtonExtras.Click += new EventHandler(ButtonExtrasClick);
            toolBar.ToolbarButtonPhotos.Click += new EventHandler(ButtonPhotosClick);
            toolBar.ToolbarButtonNews.Click += new EventHandler(ButtonNewsClick);
            toolBar.ToolbarButtonMessages.Click += new EventHandler(ButtonMessagesClick);

            this.Canvas.Children.Add(toolBar);
            this.Canvas.Children.Add(btnRefresh);
            this.Canvas.Children.Add(btnFriendsAll);
            this.Canvas.Children.Add(btnFriendsNew);
            this.Controls.Add(klvFriendsOnlineList);

            this.Canvas.RecalcDPIScaling();

            this.ResumeLayout(false);
        }

        //void klvFriendsList_MouseDown(object sender, MouseEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion

        private BottomToolBar toolBar;

        private UIButton btnRefresh;
        private UIButton btnFriendsAll;
        private UIButton btnFriendsNew;

        private FriendsListKineticListView klvFriendsOnlineList;
    }
}

