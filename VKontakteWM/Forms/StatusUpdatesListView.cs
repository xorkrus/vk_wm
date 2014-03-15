using System;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Common;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.Configuration;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Components.UI.Transitions;
using Galssoft.VKontakteWM.Components.UI.Wrappers;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Properties;
using Microsoft.WindowsCE.Forms;
using Galssoft.VKontakteWM.ApplicationLogic;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;

namespace Galssoft.VKontakteWM.Forms
{
    public partial class StatusUpdatesListView : UIViewBase, IView<NewsItems>
    {
        public StatusUpdatesListView()
        {     
            InitializeComponent();
        }

        public void Load()
        {
            //
        }

        void StatusUpdatesListView_FirstRenderComplete(object sender, EventArgs e)
        {
            ViewData["MainViewThis"] = this;
            OnViewStateChanged("AutoUpdate");
            OnViewStateChanged("CheckAutoLogin");
        }

        #region IView Members

        public string Title
        {
            get { return Resources.StatusUpdatesList_View_Title; }
        }

        public MainMenu ViewMenu
        {
            get { return null; }
        }

        public void OnBeforeActivate()
        {
            //RenewMessageImage();

            //klvImagesList.ReloadUserPhotos();

            klvStatusUpdatesList.SelectedIndex = -1;
            klvImagesList.SelectedIndex = -1;
           
            //ViewData["IsRefresh"] = false;

            

            if (ViewData["FirstActivate"] == null)
            {
                ViewData["FirstActivate"] = string.Empty;

                if (Configuration.DataRenewType == DataRenewVariants.UpdateAlways)
                {
                    ViewData["IsRefresh"] = true;

                    OnViewStateChanged("GetViewData");
                }
            }

            //else
            //{
            //    ViewData["IsRefresh"] = false;
            //}

            //OnViewStateChanged("ListActualization"); // проверка "актуальности" списка (не изменился ли пользователь?)            
        }

        public void OnActivate()
        {
            //
        }

        public void OnAfterDeactivate()
        {
            klvImagesList.ReleaseUserPhotos();

            ViewData["FirstActivate"] = null;
        }

        public void OnAfterActivate()
        {
            (new InputPanel()).Enabled = false;

            toolBar.SelectButton(toolBar.ToolbarButtonNews);

            klvImagesList.ReloadUserPhotos();

            //// перенес проверку обновления сюда чтобы не зависало...
            //if (ViewData["StatusUpdatesListView"] == null)
            //{
            //    ViewData["StatusUpdatesListView"] = this;
            //    OnViewStateChanged("AutoUpdate");
            //}
        }

        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    base.OnPaint(e);

        //    if (ViewData["GetViewDataResponseMessage"] != null && (string)ViewData["IsFirstStart"] == "1")
        //    {
        //        DialogControl.ShowQuery((string)ViewData["GetViewDataResponseMessage"], DialogButtons.OK);

        //        // ViewData["IsFirstStart"] = "0"; // т.к. выставляем в другом месте, то попробуем очищать сообщение

        //        //ViewData["GetViewDataResponseMessage"] = null; // т.к. вызывается много раз, то надо что-то делать, чтобы не зависало на диалогах

        //        //ViewData["IsFirstStart"] = "0"; // это не правильно конечно, но как иначе х/з
        //    }
        //}

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
        
        public new ViewDataDictionary<NewsItems> ViewData { get; set; }
        
        public new NewsItems Model { get; set; }

        #endregion

        protected override void OnUpdateView(string key)
        {
            #region LoadListRespons

            if (key == "LoadListResponse")
            {
                klvStatusUpdatesList.Clear();
                klvImagesList.Clear();

                klvStatusUpdatesList.DataSource = Model.Statuses;
                klvImagesList.DataSource = Model.Photos;

                klvStatusUpdatesList.Reload();
                klvImagesList.Reload();
            }

            #endregion

            #region ReloadListResponse

            if (key == "ReloadListResponse")
            {
                klvStatusUpdatesList.Reload();
                klvImagesList.Reload();
            }

            #endregion

            #region RefreshListResponse

            if (key == "RefreshListResponse")
            {
                klvStatusUpdatesList.Refresh();
                klvImagesList.Refresh();
            }

            #endregion

            #region LoadListResponseNegative

            if (key == "GetViewDataResponseNegative")
            {
                DialogControl.ShowQuery((string)ViewData["GetViewDataResponseMessage"], DialogButtons.OK);

                //if (!((string)ViewData["IsFirstStart"]).Equals("1"))
                //{
                //    DialogControl.ShowQuery((string)ViewData["GetViewDataResponseMessage"], DialogButtons.OK);
                //}
            }

            #endregion

            if (key == "GoToLogin")
            {
                OnViewStateChanged("GoGoToLogin");
            }

            //RenewMessageImage();
        }

        #region кнопочки

        #region менюха
        
        private void ButtonMessagesClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToMessages");
            }
        }

        private void ButtonFriendsClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToFriends");
            }
        }

        private void ButtonPhotosClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToPhotos");
            }
        }
        
        private void ButtonExtrasClick(object sender, EventArgs e)
        {
            toolBar.SelectButton(toolBar.ToolbarButtonExtras);
            toolBar.contextMenu.Show(this, new Point(Width, Height - toolBar.GetCurrentShift()));
            toolBar.SelectButton(toolBar.ToolbarButtonNews);
        }

        #endregion

        #region формочка

        void BtnRefreshClick(object sender, EventArgs e)
        {
            ViewData["IsRefresh"] = true;

            OnViewStateChanged("GetViewData");            
        }

        void BtnPhotoCommentsUpdatesClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToPhotoComments");
            }
        }

        static void BtnStatusUpdatesClick(object sender, EventArgs e)
        {
            //ничего
        }

        #endregion

        private void menuItemSendMessage_Click(object sender, EventArgs e)
        {
            using (new WaitWrapper())
            {
                OnViewStateChanged("GoToSendMessage");
            }
        }

        // выделение айтема в нижнем списке изображений
        void klvImagesList_Select(object item, object view)
        {
            if (item != null)
            {
                ViewData["Uid"] = klvImagesList.SelectedItem.UserID;
                ViewData["PhotoID"] = klvImagesList.SelectedItem.Uid;
                ViewData["LargePhotoURL"] = klvImagesList.SelectedItem.LargePhotoURL;

                using (new WaitWrapper(false))
                {
                    OnViewStateChanged("GoDetailedView");
                }
            }
        }

        private void klvStatusUpdatesList_ReturnLongPress(object sender, ListViewLongPressEventArgs e)
        {
            if (e.ItemData != null)
            {
                //klvStatusUpdatesList.ShowSelectedItem = true;
                klvStatusUpdatesList.Refresh();

                ViewData["Uid"] = klvStatusUpdatesList.SelectedItem.UserID;

                contexMenu.Show((Control)sender, new Point(e.ClickCoordinates.X, e.ClickCoordinates.Y));

                klvStatusUpdatesList.SelectedIndex = -1;
                klvStatusUpdatesList.Refresh();
            }
        }

        private void klvStatusUpdatesList_MouseUp(object sender, MouseEventArgs e)
        {
            if (klvStatusUpdatesList.SelectedIndex > -1)
            {
                //klvStatusUpdatesList.ShowSelectedItem = true;
                klvStatusUpdatesList.Refresh();

                ViewData["Uid"] = klvStatusUpdatesList.SelectedItem.UserID;

                contexMenu.Show((Control)sender, new Point(e.X, e.Y));

                klvStatusUpdatesList.SelectedIndex = -1;
                klvStatusUpdatesList.Refresh();
            }
        }

        #endregion

        private void OnSetOnlineMode(object sender, EventArgs e)
        {
            //
        }

        private void OnSetOfflineMode(object sender, EventArgs e)
        {
            //
        }

        void StatusUpdatesListViewResize(object sender, EventArgs e)
        {
            btnStatusUpdates.Location = new Point((int) ((Width - 2*btnStatusUpdates.Width)/2),
                                                  (int) ((header.Height - btnStatusUpdates.Height)/2));
            btnPhotoCommentsUpdates.Location =
                new Point((int) ((Width - 2*btnStatusUpdates.Width)/2) + btnPhotoCommentsUpdates.Width,
                          (int) ((header.Height - btnPhotoCommentsUpdates.Height)/2));

            btnStatusUpdatesText.Location = btnStatusUpdates.Location;
            btnPhotoCommentsUpdatesText.Location = btnPhotoCommentsUpdates.Location;

            btnStatusUpdatesShadowText.Location = new Point(
                btnStatusUpdates.Location.X,
                btnStatusUpdates.Location.Y - 1
                );

            btnPhotoCommentsUpdatesShadowText.Location = new Point(
                btnPhotoCommentsUpdates.Location.X,
                btnPhotoCommentsUpdates.Location.Y - 1
                );

            btnPhotoCommentsUpdatesText.Size = btnPhotoCommentsUpdatesShadowText.Size = btnPhotoCommentsUpdates.Size;
            btnStatusUpdatesText.Size = btnStatusUpdatesShadowText.Size = btnStatusUpdates.Size;
        }

        private void OnFriendAvatarLoad(object sender, EventArgs e)
        {
            AfterLoadFriendAvatarEventArgs ee = (AfterLoadFriendAvatarEventArgs)e;

            string friendAvatarPath = ee.ImageFileName;

            if (!string.IsNullOrEmpty(friendAvatarPath))
            {
                foreach (var val in klvImagesList.Items)
                {
                    if (val.Photo == friendAvatarPath)
                    {
                        val.IsPhotoLoaded = true;
                        klvImagesList.UpdateUserPhoto(val);

                        OnViewStateChanged("RefreshList");
                    }
                }

                if (ee.ImageLast)
                {
                    if (ViewData["StatusUpdatesListView"] == null)
                    {
                        ViewData["StatusUpdatesListView"] = this;
                        OnViewStateChanged("AutoUpdate");
                    }
                }
            }
        }

        private void RenewMessageImage()
        {
            if (Globals.BaseLogic.IsNewMessages())
            {
                toolBar.ToolbarButtonMessages.TransparentButton = MasterForm.SkinManager.GetImage("TBButtonMessagesNew1");
                toolBar.ToolbarButtonMessages.TransparentButtonPressed = MasterForm.SkinManager.GetImage("TBButtonMessagesNew3");
                toolBar.ToolbarButtonMessages.TransparentButtonSelected = MasterForm.SkinManager.GetImage("TBButtonMessagesNew2");
            }
            else
            {
                toolBar.ToolbarButtonMessages.TransparentButton = MasterForm.SkinManager.GetImage("TBButtonMessages1");
                toolBar.ToolbarButtonMessages.TransparentButtonPressed = MasterForm.SkinManager.GetImage("TBButtonMessages3");
                toolBar.ToolbarButtonMessages.TransparentButtonSelected = MasterForm.SkinManager.GetImage("TBButtonMessages2");
            }
        }

        #region Обработчики выбора пунктов контекстного меню

        private static void MiExitClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                if (SystemConfiguration.DeleteOnExit)
                    Globals.BaseLogic.IDataLogic.ClearPass();
                Application.Exit();
            }
        }

        private static void MiAboutClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                MasterForm.Navigate<AboutController>();
            }  
        }

        private static void MiSettingsClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                Configuration.LoadConfigSettings();
                MasterForm.Navigate<SettingsController>();
            }
        }

        static void MiUserDataClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                MasterForm.Navigate<UserDataController>();
            }
        }

        #endregion
    }
}
