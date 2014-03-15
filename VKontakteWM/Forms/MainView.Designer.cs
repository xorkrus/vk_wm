using System;
using System.Drawing;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Properties;
using AnchorStyles = Galssoft.VKontakteWM.Components.UI.AnchorStyles;
using ToolBar = Galssoft.VKontakteWM.Components.UI.CompoundControls.ToolBar;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;

namespace Galssoft.VKontakteWM.Forms
{
    partial class MainView
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
            this.buttonRefresh = new UIButton(ButtonStyle.AlphaChannel);
            this.toolBar = new BottomToolBar();
            this.pictureAvatar = new GraphicsImage();
            this.labelName = new UILabel();
            this.status = new StatusControl();            
            this.timerRefreshEvent = new System.Windows.Forms.Timer();
            this.eventsListViewFriends = new EventsKineticListView();

            this.SuspendLayout();

            //this.labelFirstName = new UILabel();
            //this.labelLastName = new UILabel();

            //this.labelFriendsTitle = new UILabel();
            //this.labelMessagesTitle = new UILabel();
            //this.labelCommentsTitle = new UILabel();
            //this.labelFriendsNewsTitle = new UILabel();
            //this.labelFriendsPhotosTitle = new UILabel();
            //this.labelWallMessagesTitle = new UILabel();
            //this.labelFriends = new UILabel();
            //this.labelMessages = new UILabel();
            //this.labelComments = new UILabel();
            //this.labelFriendsNews = new UILabel();
            //this.labelFriendsPhotos = new UILabel();
            //this.labelWallMessages = new UILabel();

            //this.pictureMessages = new GraphicsImage();
            //this.pictureComments = new GraphicsImage();
            //this.pictureFriends = new GraphicsImage();
            //this.pictureFriendsNews = new GraphicsImage();
            //this.pictureFriendsPhotos = new GraphicsImage();
            //this.pictureWallMessages = new GraphicsImage();

            //this.labelBirthday = new UILabel();
            //this.labelSex = new UILabel();
            //this.labelTown = new UILabel();
            //this.labelMobilePhone = new UILabel();

            //this.buttonStartNotificator = new UIButton(ButtonStyle.AlphaChannel);
            //this.buttonStopNotificator = new UIButton(ButtonStyle.AlphaChannel);

            this.buttonUploadPhoto = new UIButton(ButtonStyle.AlphaChannel);

            //
            // this
            //
            this.Size = new Size(240, 268);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = Color.White;
            this.Resize += new EventHandler(FormResize);            
           
            // 
            // pictureAvatar
            //             
            this.pictureAvatar.Location = new System.Drawing.Point(5, 41);
            this.pictureAvatar.Name = "pictureAvatar";
            this.pictureAvatar.Size = new System.Drawing.Size(50, 50);
            this.pictureAvatar.Anchor = AnchorStyles.Top | AnchorStyles.Left;            

            //// 
            //// labelFirstName
            //// 
            //this.labelFirstName.Location = new System.Drawing.Point(110, 5);
            //this.labelFirstName.Name = "labelFirstName";
            //this.labelFirstName.Size = new System.Drawing.Size(150, 20);
            //// 
            //// labelLastName
            //// 
            //this.labelLastName.Location = new System.Drawing.Point(110, 20);
            //this.labelLastName.Name = "labelLastName";
            //this.labelLastName.Size = new System.Drawing.Size(75, 20);

            // 
            // labelName
            // 
            this.labelName.Location = new System.Drawing.Point(62, 38);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(175, 18);
            this.labelName.Font = FontCache.CreateFont("Tahoma", 13, FontStyle.Bold);
            this.labelName.BackColor = Color.White;
            this.labelName.HorizontalTextAlignment = HorizontalAlignment.Stretch;
            this.labelName.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            //// 
            //// labelBirthday
            //// 
            //this.labelBirthday.Location = new System.Drawing.Point(110, 35);
            //this.labelBirthday.Name = "labelBirthday";
            //this.labelBirthday.Size = new System.Drawing.Size(100, 20);
            //this.labelBirthday.Text = "День рожд.: ";
            //// 
            //// labelSex
            //// 
            //this.labelSex.Location = new System.Drawing.Point(110, 50);
            //this.labelSex.Name = "labelSex";
            //this.labelSex.Size = new System.Drawing.Size(60, 20);
            //this.labelSex.Text = "Пол: ";
            //// 
            //// labelTown
            //// 
            //this.labelTown.Location = new System.Drawing.Point(110, 65);
            //this.labelTown.Name = "labelTown";
            //this.labelTown.Size = new System.Drawing.Size(60, 20);
            //this.labelTown.Text = "Город: ";
            //// 
            //// labelMobilePhone
            //// 
            //this.labelMobilePhone.Location = new System.Drawing.Point(110, 80);
            //this.labelMobilePhone.Name = "labelMobilePhone";
            //this.labelMobilePhone.Size = new System.Drawing.Size(100, 20);
            //this.labelMobilePhone.Text = "моб: ";

            // 
            // textBoxStatus
            // 
            this.status.Location = new System.Drawing.Point(55, 59);
            this.status.Text = string.Empty;
            this.status.TextSubstitute = Resources.StatusControl_TextSubstitute;
            this.status.Size = new Size(180, 32);
            this.status.Font = FontCache.CreateFont("Tahoma", 11, FontStyle.Regular, true);
            this.status.FontColor = Color.FromArgb(102, 80, 51);
            this.status.FontColorUnactive = Color.FromArgb(179, 168, 153);
            this.status.FontColorInvert = Color.White;            
            this.status.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left;
            this.status.Click += new EventHandler(StatusClick);

            #region комменты

            //#region labelMessages

            //this.pictureMessages.Location = new System.Drawing.Point(30, 132);
            //this.pictureMessages.Name = "pictureMessages";
            //this.pictureMessages.Size = new System.Drawing.Size(20, 20);
            //this.pictureMessages.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            //this.pictureMessages.AlphaChannelImage = MasterForm.SkinManager.GetImage("MessageGrey");

            //this.labelMessages.Location = new System.Drawing.Point(30, 130);
            //this.labelMessages.Name = "labelMessages";
            //this.labelMessages.Size = new System.Drawing.Size(34, 20);
            //this.labelMessages.Text = "0";

            //this.labelMessagesTitle.Location = new System.Drawing.Point(50, 130);
            //this.labelMessagesTitle.Name = "labelMessagesTitle";
            //this.labelMessagesTitle.Size = new System.Drawing.Size(150, 20);
            //this.labelMessagesTitle.Text = "Личных сообщений";

            //#endregion

            //#region labelComments

            //this.pictureComments.Location = new System.Drawing.Point(30, 147);
            //this.pictureComments.Name = "pictureComments";
            //this.pictureComments.Size = new System.Drawing.Size(20, 20);
            //this.pictureComments.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            //this.pictureComments.AlphaChannelImage = MasterForm.SkinManager.GetImage("MessageGrey");

            //this.labelComments.Location = new System.Drawing.Point(30, 145);
            //this.labelComments.Name = "labelComments";
            //this.labelComments.Size = new System.Drawing.Size(34, 20);
            //this.labelComments.Text = "0";

            //this.labelCommentsTitle.Location = new System.Drawing.Point(50, 145);
            //this.labelCommentsTitle.Name = "labelCommentsTitle";
            //this.labelCommentsTitle.Size = new System.Drawing.Size(150, 20);
            //this.labelCommentsTitle.Text = "Коментариев";

            //#endregion

            //#region labelFriends

            //this.pictureFriends.Location = new System.Drawing.Point(30, 162);
            //this.pictureFriends.Name = "pictureFriends";
            //this.pictureFriends.Size = new System.Drawing.Size(20, 20);
            //this.pictureFriends.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            //this.pictureFriends.AlphaChannelImage = MasterForm.SkinManager.GetImage("MessageGrey");

            //this.labelFriends.Location = new System.Drawing.Point(30, 160);
            //this.labelFriends.Name = "labelFriends";
            //this.labelFriends.Size = new System.Drawing.Size(34, 20);
            //this.labelFriends.Text = "0";

            //this.labelFriendsTitle.Location = new System.Drawing.Point(50, 160);
            //this.labelFriendsTitle.Name = "labelFriendsTitle";
            //this.labelFriendsTitle.Size = new System.Drawing.Size(150, 20);
            //this.labelFriendsTitle.Text = "Приглашений дружить";

            //#endregion

            //#region labelFriendsNews

            //this.pictureFriendsNews.Location = new System.Drawing.Point(30, 177);
            //this.pictureFriendsNews.Name = "pictureFriendsNews";
            //this.pictureFriendsNews.Size = new System.Drawing.Size(20, 20);
            //this.pictureFriendsNews.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            //this.pictureFriendsNews.AlphaChannelImage = MasterForm.SkinManager.GetImage("MessageGrey");

            //this.labelFriendsNews.Location = new System.Drawing.Point(30, 175);
            //this.labelFriendsNews.Name = "labelFriendsNews";
            //this.labelFriendsNews.Size = new System.Drawing.Size(34, 20);
            //this.labelFriendsNews.Text = "0";

            //this.labelFriendsNewsTitle.Location = new System.Drawing.Point(50, 175);
            //this.labelFriendsNewsTitle.Name = "labelFriendsNewsTitle";
            //this.labelFriendsNewsTitle.Size = new System.Drawing.Size(150, 20);
            //this.labelFriendsNewsTitle.Text = "Новостей";

            //#endregion

            //#region labelFriendsPhotos

            //this.pictureFriendsPhotos.Location = new System.Drawing.Point(30, 192);
            //this.pictureFriendsPhotos.Name = "pictureFriendsPhotos";
            //this.pictureFriendsPhotos.Size = new System.Drawing.Size(20, 20);
            //this.pictureFriendsPhotos.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            //this.pictureFriendsPhotos.AlphaChannelImage = MasterForm.SkinManager.GetImage("MessageGrey");

            //this.labelFriendsPhotos.Location = new System.Drawing.Point(30, 190);
            //this.labelFriendsPhotos.Name = "labelFriendsPhotos";
            //this.labelFriendsPhotos.Size = new System.Drawing.Size(34, 20);
            //this.labelFriendsPhotos.Text = "0";

            //this.labelFriendsPhotosTitle.Location = new System.Drawing.Point(50, 190);
            //this.labelFriendsPhotosTitle.Name = "labelFriendsPhotosTitle";
            //this.labelFriendsPhotosTitle.Size = new System.Drawing.Size(150, 20);
            //this.labelFriendsPhotosTitle.Text = "Фотографий друзей";

            //#endregion

            //#region labelWallMessages

            //this.pictureWallMessages.Location = new System.Drawing.Point(30, 207);
            //this.pictureWallMessages.Name = "pictureWallMessages";
            //this.pictureWallMessages.Size = new System.Drawing.Size(20, 20);
            //this.pictureWallMessages.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            //this.pictureWallMessages.AlphaChannelImage = MasterForm.SkinManager.GetImage("MessageGrey");

            //this.labelWallMessages.Location = new System.Drawing.Point(30, 205);
            //this.labelWallMessages.Name = "labelWallMessages";
            //this.labelWallMessages.Size = new System.Drawing.Size(34, 20);
            //this.labelWallMessages.Text = "0";

            //this.labelWallMessagesTitle.Location = new System.Drawing.Point(50, 205);
            //this.labelWallMessagesTitle.Name = "labelWallMessagesTitle";
            //this.labelWallMessagesTitle.Size = new System.Drawing.Size(150, 20);
            //this.labelWallMessagesTitle.Text = "Сообщений на стене";

            //#endregion

            #endregion

            // 
            // buttonRefresh
            //             
            buttonRefresh.Location = new Point(194, 0);            
            buttonRefresh.Name = "buttonRefresh";
            buttonRefresh.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            buttonRefresh.Click += new System.EventHandler(buttonRefresh_Click);
                      
            //
            //buttonUploadPhoto
            //
            buttonUploadPhoto.Location = new Point(0, 0);
            buttonUploadPhoto.Name = "buttonUploadPhoto";
            buttonUploadPhoto.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonUploadPhoto.Click += new EventHandler(buttonUploadPhoto_Click);

            //
            // eventsListViewFriends
            //
            this.eventsListViewFriends.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom;
            this.eventsListViewFriends.Location = new System.Drawing.Point(1, 95);
            this.eventsListViewFriends.BackColor = Color.White;
            this.eventsListViewFriends.ScrollAction = KineticControlBase.KineticControlScrollAction.ScrollingForTime;
            this.eventsListViewFriends.Name = "eventsListViewFriends";

            //FIXME
            if (UISettings.ScreenDPI == 128)
            {
                this.eventsListViewFriends.Size = new System.Drawing.Size(233, 133);
            }
            else
            {
                this.eventsListViewFriends.Size = new System.Drawing.Size(233, 134);
            }

            //this.eventsListViewFriends.TabIndex = 2;

            //
            // timerRefreshEvent
            //
            this.timerRefreshEvent.Interval = 5000;
            this.timerRefreshEvent.Tick += new EventHandler(timerRefreshEvent_Tick);
            this.timerRefreshEvent.Enabled = true;

            //
            // toolBar
            //
            toolBar.ToolbarButtonExtras.Click += new EventHandler(ButtonExtrasClick);
            toolBar.ToolbarButtonFriends.Click += new EventHandler(ButtonFriendsClick);
            toolBar.ToolbarButtonNews.Click += new EventHandler(ButtonPhotosClick);
            toolBar.ToolbarButtonMessages.Click += new EventHandler(ButtonMessagesClick);
            
            //buttonMain.Click += new EventHandler(buttonMain_Click);
            // 

            // 
            // MainView
            // 
            this.Canvas.Children.Add(buttonRefresh);
            //this.Canvas.Children.Add(buttonStartNotificator);
            //this.Canvas.Children.Add(buttonStopNotificator);
            this.Canvas.Children.Add(buttonUploadPhoto);
            this.Canvas.Children.Add(toolBar);
            this.Canvas.Children.Add(pictureAvatar);
            //this.Canvas.Children.Add(labelFirstName);
            //this.Canvas.Children.Add(labelLastName);
            this.Canvas.Children.Add(labelName);
            this.Canvas.Children.Add(status);

            //this.Canvas.Children.Add(pictureMessages);
            //this.Canvas.Children.Add(pictureComments);
            //this.Canvas.Children.Add(pictureFriends);
            //this.Canvas.Children.Add(pictureFriendsPhotos);
            //this.Canvas.Children.Add(pictureFriendsNews);
            //this.Canvas.Children.Add(pictureWallMessages);
            //this.Canvas.Children.Add(labelFriendsTitle);
            //this.Canvas.Children.Add(labelMessagesTitle);
            //this.Canvas.Children.Add(labelCommentsTitle);
            //this.Canvas.Children.Add(labelFriendsNewsTitle);
            //this.Canvas.Children.Add(labelFriendsPhotosTitle);
            //this.Canvas.Children.Add(labelWallMessagesTitle);
            //this.Canvas.Children.Add(labelFriends);
            //this.Canvas.Children.Add(labelMessages);
            //this.Canvas.Children.Add(labelComments);
            //this.Canvas.Children.Add(labelFriendsNews);
            //this.Canvas.Children.Add(labelFriendsPhotos);
            //this.Canvas.Children.Add(labelWallMessages);

            //this.Canvas.Children.Add(labelBirthday);
            //this.Canvas.Children.Add(labelSex);
            //this.Canvas.Children.Add(labelTown);
            //this.Canvas.Children.Add(labelMobilePhone);

            this.Controls.Add(eventsListViewFriends);

            this.Canvas.RecalcDPIScaling();
            this.ResumeLayout(false);

            this.FirstRenderComplete += new EventHandler(MainView_FirstRenderComplete);

            buttonRefresh.TransparentButton = MasterForm.SkinManager.GetImage("ButtonOther");
            buttonRefresh.TransparentButtonPressed = MasterForm.SkinManager.GetImage("ButtonOtherPressed");
            buttonUploadPhoto.TransparentButton = MasterForm.SkinManager.GetImage("ButtonOther");
            buttonUploadPhoto.TransparentButtonPressed = MasterForm.SkinManager.GetImage("ButtonOtherPressed");

            buttonRefresh.Text = "Обновить"; // del
            buttonRefresh.Size = new Size(UISettings.CalcPix(46), UISettings.CalcPix(37)); // del
            buttonUploadPhoto.Text = "Загр. фото"; // del
            buttonUploadPhoto.Size = new Size(UISettings.CalcPix(46), UISettings.CalcPix(37)); // del            
        }

        #endregion

        private UIButton buttonRefresh;
        private UIButton buttonUploadPhoto;

        private BottomToolBar toolBar;

        private GraphicsImage pictureAvatar;
        
        //private UILabel labelFirstName;  
        //private UILabel labelLastName;

        private UILabel labelName;
        private StatusControl status;

        private EventsKineticListView eventsListViewFriends;

        //private UILabel labelMessages;
        //private UILabel labelMessagesTitle;
        //private UILabel labelComments;
        //private UILabel labelCommentsTitle;
        //private UILabel labelFriends;
        //private UILabel labelFriendsTitle;
        //private UILabel labelFriendsNews;
        //private UILabel labelFriendsNewsTitle;
        //private UILabel labelFriendsPhotos;
        //private UILabel labelFriendsPhotosTitle;
        //private UILabel labelWallMessages;
        //private UILabel labelWallMessagesTitle;

        private System.Windows.Forms.Timer timerRefreshEvent;

        //private UILabel labelBirthday;
        //private UILabel labelSex;
        //private UILabel labelTown;
        //private UILabel labelMobilePhone;
            
        //private UIButton buttonStartNotificator;
        //private UIButton buttonStopNotificator;

        //private UIButton buttonUploadPhoto;

        //private GraphicsImage pictureMessages;
        //private GraphicsImage pictureComments;
        //private GraphicsImage pictureFriends;
        //private GraphicsImage pictureFriendsPhotos;
        //private GraphicsImage pictureFriendsNews;
        //private GraphicsImage pictureWallMessages;

        
    }
}

