using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.Transitions;
using Galssoft.VKontakteWM.Components.UI.Wrappers;
using Galssoft.VKontakteWM.Properties;
using Microsoft.WindowsCE.Forms;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.ApplicationLogic;
using System.Collections.Generic;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;
using System.Runtime.InteropServices;

namespace Galssoft.VKontakteWM.Forms
{
    public partial class MainView : UIViewBase, IView<List<EventButton>>
    {
        private InputPanel _sipPanel;

        public MainView()
        {
            _sipPanel = new InputPanel();

            InitializeComponent();
        }

        public void Load()
        {           
            OnUpdateView("RefreshUserInfo");
            OnUpdateView("RefreshAvatarFromCache");
            OnUpdateView("RefreshEventsInfo");
        }

        private void FormResize(object sender, EventArgs e)
        {
            //центрируем лого, когда будет
        }

        void MainView_FirstRenderComplete(object sender, EventArgs e)
        {
            ViewData["MainViewThis"] = this;

            OnViewStateChanged("AutoUpdate");
            OnViewStateChanged("CheckAutoLogin");                       
        }

        void timerRefreshEvent_Tick(object sender, EventArgs e)
        {
            OnViewStateChanged("CheckNewEvents");
        }

        #region IView Members

        public string Title
        {
            get { return Resources.MainView_Text; }
        }

        public MainMenu ViewMenu
        {
            get { return null; }
        }

        public void OnBeforeActivate()
        {
            _sipPanel.Enabled = false;
        }

        public void OnAfterDeactivate()
        {
            eventsListViewFriends.SelectedIndex = -1;

            IImage image = MasterForm.SkinManager.GetImage("ImageNull");

            if (!pictureAvatar.AlphaChannelImage.Equals(image))
            {
                IImage newIImage = pictureAvatar.AlphaChannelImage;

                pictureAvatar.AlphaChannelImage = image;

                Marshal.FinalReleaseComObject(newIImage);
            }
        }

        public void OnAfterActivate()
        {
            _sipPanel.Enabled = false;

            //выделение кнопки toolbar
            toolBar.SelectButton(toolBar.ToolbarButtonNews);

            OnUpdateView("RefreshAvatarFromCache");
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (ViewData["GetError"] != null && (string)ViewData["IsFirstStart"] == "1")
            {
                DialogControl.ShowQuery((string)ViewData["GetError"], DialogButtons.OK);               
            }
        }

        public Bitmap CreateScreenShot()
        {
            Bitmap bitmap = new Bitmap(Width, Height);
            Rectangle rect = new Rectangle(0, 0, Width, Height);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                IntPtr gMemPtr = g.GetHdc();

                using (Gdi gMem = Gdi.FromHdc(gMemPtr, Rectangle.Empty))
                {
                    DrawBackground(gMem, rect);

                    if (Canvas != null)
                    {
                        Canvas.Render(gMem, rect);
                    }

                    foreach (Control control in Controls)
                    {
                        if (control is KineticControlBase)
                        {
                            ((KineticControlBase)control).DrawRender(gMem);
                        }
                    }
                }

                g.ReleaseHdc(gMemPtr);
            }

            return bitmap;
        }

        public TransitionType GetTransition(IView from)
        {
            return TransitionType.Basic;
        }

        public new ViewDataDictionary<List<EventButton>> ViewData { get; set; }

        public new List<EventButton> Model { get; set; }

        #endregion

        protected override void OnUpdateView(string key)
        {
            if (key == "NotificatorStartFail")
            {
                var error = (string)ViewData["NotificatorStartError"];

                DialogControl.ShowQuery(error, DialogButtons.OK);                
            }

            if (key == "RefreshUserInfo")
            {
                labelName.Text = (string)ViewData["UserName"];

                status.Text = (string)ViewData["UserStatus"];
                status.OnInvalidate();
            }

            if (key == "RefreshAvatarFromCache")
            {
                pictureAvatar.OnInvalidate();

                if (!string.IsNullOrEmpty((string)ViewData["AvatarPath"]))
                {
                    IImage newIImage = null;
                    Size outSize = Size.Empty;

                    ImageHelper.LoadImageFromFile((string)ViewData["AvatarPath"], out newIImage, out outSize);                    

                    //FIXME
                    int paWidth = UISettings.CalcPix(50);
                    int paHeight = UISettings.CalcPix(50);
                    int paLeft = UISettings.CalcPix(5);
                    int paTop = UISettings.CalcPix(41);

                    pictureAvatar.Size = outSize;
                    pictureAvatar.Location = new Point(paLeft + (paWidth - outSize.Width) / 2, paTop + (paHeight - outSize.Height) / 2);

                    pictureAvatar.AlphaChannelImage = newIImage;
                }
            }

            if (key == "GetFail")
            {
                var error = (string)ViewData["GetError"];

                if (!((string)ViewData["IsFirstStart"]).Equals("1"))
                {
                    DialogControl.ShowQuery(error, DialogButtons.OK);
                }
            }

            if (key == "RefreshEventsInfo")
            {
                eventsListViewFriends.DataSource = Model;
                eventsListViewFriends.Reload();

                #region (пока убрал комментарии)

                //labelMessages.Text = (string)ViewData["MessagesCount"];
                //if (!string.IsNullOrEmpty(labelMessages.Text) && !labelMessages.Text.Equals("0"))
                //{
                //    pictureMessages.AlphaChannelImage = MasterForm.SkinManager.GetImage("MessageYellow");
                //}
                //else
                //{
                //    pictureMessages.AlphaChannelImage = MasterForm.SkinManager.GetImage("MessageGrey");

                //    labelMessages.Text = "0";
                //}
                //pictureMessages.OnInvalidate();



                //labelComments.Text = (string)ViewData["CommentsCount"];
                //if (!string.IsNullOrEmpty(labelComments.Text) && !labelComments.Text.Equals("0"))
                //{
                //    pictureComments.AlphaChannelImage = MasterForm.SkinManager.GetImage("MessageYellow");
                //}
                //else
                //{
                //    pictureComments.AlphaChannelImage = MasterForm.SkinManager.GetImage("MessageGrey");

                //    labelComments.Text = "0";
                //}
                //pictureComments.OnInvalidate();                



                //labelFriends.Text = (string)ViewData["FriendsCount"];
                //if (!string.IsNullOrEmpty(labelFriends.Text) && !labelFriends.Text.Equals("0"))
                //{
                //    pictureFriends.AlphaChannelImage = MasterForm.SkinManager.GetImage("MessageYellow");
                //}
                //else
                //{
                //    pictureFriends.AlphaChannelImage = MasterForm.SkinManager.GetImage("MessageGrey");

                //    labelFriends.Text = "0";
                //}
                //pictureFriends.OnInvalidate();                



                //labelFriendsPhotos.Text = (string)ViewData["FriendsPhotosCount"];
                //if (!string.IsNullOrEmpty(labelFriendsPhotos.Text) && !labelFriendsPhotos.Text.Equals("0"))
                //{                   
                //    if (labelFriendsPhotos.Text.Equals("100"))
                //    {
                //        pictureFriendsPhotos.AlphaChannelImage = MasterForm.SkinManager.GetImage("MessageGrey");

                //        labelFriendsPhotos.Text = "-";
                //    }
                //    else
                //    {
                //        pictureFriendsPhotos.AlphaChannelImage = MasterForm.SkinManager.GetImage("MessageYellow");
                //    }
                //}
                //else
                //{
                //    pictureFriendsPhotos.AlphaChannelImage = MasterForm.SkinManager.GetImage("MessageGrey");

                //    labelFriendsPhotos.Text = "0";
                //}
                //pictureFriendsPhotos.OnInvalidate();



                //labelFriendsNews.Text = (string)ViewData["FriendsNewsCount"];
                //if (!string.IsNullOrEmpty(labelFriendsNews.Text) && !labelFriendsNews.Text.Equals("0"))
                //{
                //    pictureFriendsNews.AlphaChannelImage = MasterForm.SkinManager.GetImage("MessageYellow");

                //    if (labelFriendsNews.Text.Equals("50"))
                //    {
                //        labelFriendsNews.Text = "?";
                //        labelFriendsNewsTitle.Text = "Больше 50 новостей";                       
                //    }
                //    else
                //    {
                //        labelFriendsNewsTitle.Text = "Новостей";                        
                //    }                    
                //}
                //else
                //{
                //    pictureFriendsNews.AlphaChannelImage = MasterForm.SkinManager.GetImage("MessageGrey");

                //    labelFriendsNews.Text = "0";
                //}
                //pictureFriendsNews.OnInvalidate();



                //labelWallMessages.Text = (string)ViewData["WallCount"];
                //if (!string.IsNullOrEmpty(labelWallMessages.Text) && !labelWallMessages.Text.Equals("0"))
                //{
                //    pictureWallMessages.AlphaChannelImage = MasterForm.SkinManager.GetImage("MessageYellow");

                //    if (labelWallMessages.Text.Equals("50"))
                //    {
                //        labelWallMessages.Text = "?";
                //        labelWallMessagesTitle.Text = "Больше 50 сообщений на стене";
                //    }
                //    else
                //    {
                //        labelWallMessagesTitle.Text = "Сообщений на стене";
                //    }
                //}
                //else
                //{
                //    pictureWallMessages.AlphaChannelImage = MasterForm.SkinManager.GetImage("MessageGrey");

                //    labelWallMessages.Text = "0";
                //}
                //pictureWallMessages.OnInvalidate();

                #endregion
            }

            if (key == "RefreshStatusBox")
            {
                status.Text = (string) ViewData["CurrentStatus"];

                status.OnInvalidate();
            }



            //// Что то пришло от контроллера
            //if (key == "GetProfileFail")
            //{
            //    string loginError = (string)ViewData["GetProfileError"];
            //    MessageBox.Show(loginError);
            //}

            //if (key == "GetProfileResponse")
            //{
            //    //labelFirstName.Text = (string)ViewData["FirstName"];
            //    //labelLastName.Text = (string)ViewData["LastName"];
            //    status.Text = (string) ViewData["Status"];
            //    //labelBirthday.Text = (string)ViewData["Birthday"];
            //    //labelSex.Text = (string)ViewData["Sex"];
            //    //labelTown.Text = (string)ViewData["Town"];
            //    //labelMobilePhone.Text = (string)ViewData["MobilePhone"];
            //    status.OnInvalidate();
            //}

            //if (key == "RefreshStatusBox")
            //{
            //    status.Text = (string) ViewData["Status"];
            //    status.OnInvalidate();
            //}

            //if (key == "GetEventsResponse")
            //{
            //    //labelMessages.Text = (string)ViewData["MessagesCount"];
            //    //labelComments.Text = (string)ViewData["CommentsCount"];
            //    //labelFriends.Text = (string)ViewData["FriendsCount"];
            //    //if ((string)ViewData["FriendsNewsCount"] != "-1")
            //    //    labelFriendsNews.Text = (string)ViewData["FriendsNewsCount"];
            //    //if ((string)ViewData["FriendsPhotosCount"] != "-1")
            //    //    labelFriendsPhotos.Text = (string)ViewData["FriendsPhotosCount"];
            //    //if ((string)ViewData["WallCount"] != "-1")
            //    //    labelWallMessages.Text = (string)ViewData["WallCount"];
            //}            
        }

        #region кнопочки

        #region меню

        private void ButtonPhotosClick(object sender, EventArgs e)
        {
            OnViewStateChanged("GoToPhotos");
        }

        private void ButtonFriendsClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToFriends");
            }
        }

        private void ButtonMessagesClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToMessages");
            }
        }

        private void ButtonExtrasClick(object sender, EventArgs e)
        {
            OnViewStateChanged("GoToExtras");
        }

        #endregion

        //на форме

        void StatusClick(object sender, EventArgs e)
        {
            OnViewStateChanged("ChangeStatus");
        }

        void buttonRefresh_Click(object sender, System.EventArgs e)
        {
            ViewData["IsFirstStart"] = "0";
            ViewData["IsRefreshMainViewData"] = "1";

            OnViewStateChanged("GetMainViewData");
        }

        private void buttonUploadPhoto_Click(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToUploadPhoto");
            }
        }

        //private void buttonChangeAvatar_Click(object sender, EventArgs e)
        //{
        //    OnViewStateChanged("ChangeAvatar");
        //}

        //private void buttonStartNotificator_Click(object sender, EventArgs e)
        //{
        //    OnViewStateChanged("StartNotificator");
        //}

        //private void buttonStopNotificator_Click(object sender, EventArgs e)
        //{
        //    OnViewStateChanged("StopNotificator");
        //}

        #endregion

        private void OnSetOnlineMode(object sender, EventArgs e)
        {
            //
        }

        private void OnSetOfflineMode(object sender, EventArgs e)
        {
            //
        }

    }
}
