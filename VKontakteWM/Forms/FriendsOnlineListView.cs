using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
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

namespace Galssoft.VKontakteWM.Forms
{
    public partial class FriendsOnlineListView : UIViewBase, IView<List<FriendListViewItem>>
    {
        public FriendsOnlineListView()
        {
            InitializeComponent();
        }

        public void Load()
        {
            klvFriendsOnlineList.DataSource = Model;

            using (new WaitWrapper(false))
            {
                ViewData["IsRefresh"] = "false";

                OnViewStateChanged("LoadList");
            }
        }

        #region IView Members

        public string Title
        {
            get { return Resources.FriendsList_View_Title; }
        }

        public MainMenu ViewMenu
        {
            get { return null; }
        }

        public void OnBeforeActivate()
        {
            //
        }

        public void OnAfterDeactivate()
        {
            //
        }

        public void OnAfterActivate()
        {
            (new InputPanel()).Enabled = false;

            toolBar.SelectButton(toolBar.ToolbarButtonFriends);
        }

        public Bitmap CreateScreenShot()
        {
            return null;
        }

        public TransitionType GetTransition(IView from)
        {
            return TransitionType.Basic;
        }

        public new ViewDataDictionary<List<FriendListViewItem>> ViewData { get; set; }

        public new List<FriendListViewItem> Model { get; set; }

        #endregion

        #region Обработчики нажатий на кнопки формы

        #region BottomToolBar

        // Переход на форму профиля
        private void ButtonPhotosClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToPhotos");
            }
        }
        // Переход на форму новостей
        private void ButtonNewsClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToMain");
            }
        }
        // Переход на форму сообщений
        private void ButtonMessagesClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToMessages");
            }
        }
        // Переход на форму опций
        private void ButtonExtrasClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToExtras");
            }
        }

        #endregion

        #region Кнопки формы

        // обновление данных на форме
        private void BtnRefreshClick(object sender, EventArgs e)
        {
            klvFriendsOnlineList.DataSource = Model;

            using (new WaitWrapper(false))
            {
                ViewData["IsRefresh"] = "true";

                OnViewStateChanged("LoadList");
            }
        }

        void BtnFriendsAllClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToFriends");
            }
        }

        void BtnFriendsNewClick(object sender, EventArgs e)
        {
            /*
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToFriendsNew");
            }
            */
        }

        #endregion

        #endregion

        protected override void OnUpdateView(string key)
        {
            #region LoadListResponse

            if (key == "LoadListResponse")
            {
                klvFriendsOnlineList.Clear();

                klvFriendsOnlineList.DataSource = Model;

                klvFriendsOnlineList.Reload();
            }

            #endregion

            #region ReloadListResponse

            if (key == "ReloadListResponse")
            {
                klvFriendsOnlineList.Reload();
            }

            #endregion

            #region LoadListResponseNegative

            if (key == "LoadListResponseNegative")
            {
                DialogControl.ShowQuery((string)ViewData["LoadListResponseMessage"], DialogButtons.OK);
                //MessageBox.Show((string)ViewData["LoadListResponseMessage"]);
            }

            #endregion
        }

        private void OnFriendAvatarLoad(object sender, EventArgs e)
        {
            AfterLoadFriendAvatarEventArgs ee = (AfterLoadFriendAvatarEventArgs)e;

            string friendAvatarPath = ee.ImageFileName;

            if (friendAvatarPath != "")
            {
                foreach (FriendListViewItem item in klvFriendsOnlineList.Items)
                {
                    if (item.Avatar == friendAvatarPath)
                    {
                        item.IsAvatarLoaded = true;
                        klvFriendsOnlineList.UpdateUserPhoto(item);
                    }
                }

                if (ee.ImageLast)
                {
                    using (new WaitWrapper(false))
                    {
                        ViewData["IsRefresh"] = "false";

                        OnViewStateChanged("LoadList");
                    }
                }
            }
        }
    }
}
