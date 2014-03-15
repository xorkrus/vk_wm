using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.Common.ResponseClasses;
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
    public partial class FriendsSearchListView : UIViewBase, IView<List<FriendListViewItem>>
    {
        public FriendsSearchListView()
        {
            InitializeComponent();
        }

        public void Load()
        {
            ViewData["IsRefresh"] = "false";
            OnViewStateChanged("LoadList");
        }

        #region IView Members

        public string Title
        {
            get { return Resources.FriendsSearchListViewTitle; }
        }

        public MainMenu ViewMenu
        {
            get { return mainMenu; }
        }

        public void OnBeforeActivate()
        {
            filter._filter.TextColor = Color.DarkGray;
            filter.FilterText = Resources.FilterText;
            filter._filter.Focus(false);

            if (Configuration.DataRenewType == DataRenewVariants.UpdateAlways)
            {
                ViewData["IsRefresh"] = true;

                OnViewStateChanged("LoadList");
            }
            else
            {
                ViewData["IsRefresh"] = false;

                OnViewStateChanged("LoadList");
            }

            klvFriendsList.ReloadUserPhotos();

            klvFriendsList.SelectedIndex = -1;
        }

        public void OnAfterDeactivate()
        {
            klvFriendsList.ReleaseUserPhotos();
        }

        public void OnAfterActivate()
        {
            (new InputPanel()).Enabled = false;
        }

        public Bitmap CreateScreenShot()
        {
            var bitmap = new Bitmap(Width, Height);
            var rect = new Rectangle(0, 0, Width, Height);

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

        public new ViewDataDictionary<List<FriendListViewItem>> ViewData { get; set; }

        public new List<FriendListViewItem> Model { get; set; }

        #endregion

        #region Обработчики нажатий на кнопки формы

        // обновление данных на форме
        private void BtnRefreshClick(object sender, EventArgs e)
        {
            klvFriendsList.DataSource = Model;

            using (new WaitWrapper(false))
            {
                ViewData["IsRefresh"] = "true";

                OnViewStateChanged("LoadList");
            }
        }

        #endregion

        private void KlvFriendsListClick(object sender, MouseEventArgs e)
        {
            if (klvFriendsList.SelectedIndex > -1)
            {
                klvFriendsList.ShowSelectedItem = true;
                klvFriendsList.Refresh();

                ViewData["Uid"] = klvFriendsList.SelectedItem.Uid;
            }
        }

        protected override void OnUpdateView(string key)
        {
            if (key == "ResetState")
            {
                klvFriendsList.Clear();

                klvFriendsList.DataSource = (List<FriendListViewItem>)ViewData["OriginalModel"];

                klvFriendsList.Reload();
            }

            #region LoadListResponse

            if (key == "LoadListResponse")
            {
                klvFriendsList.Clear();

                klvFriendsList.DataSource = Model;

                klvFriendsList.Reload();
            }

            #endregion

            #region ReloadListResponse

            if (key == "ReloadListResponse")
            {
                klvFriendsList.Reload();
            }

            #endregion

            #region RefreshListResponse

            if (key == "RefreshListResponse")
            {
                klvFriendsList.Refresh();
            }

            #endregion

            #region LoadListResponseNegative

            if (key == "LoadListResponseNegative")
            {
                DialogControl.ShowQuery((string)ViewData["LoadListResponseMessage"], DialogButtons.OK);
            }

            #endregion

            if (key == "GoToLogin")
            {
                OnViewStateChanged("GoGoToLogin");
            }
        }

        void FriendsListView_Resize(object sender, EventArgs e)
        {

        }

        void TextBoxLostFocus(object sender, EventArgs e)
        {

        }

        void TextBoxGotFocus(object sender, EventArgs e)
        {

        }

        void MenuItemSelectClick(object sender, EventArgs e)
        {
            if (klvFriendsList.SelectedIndex == -1)
                DialogControl.ShowQuery(Resources.FriendSearchView_DontChoice, DialogButtons.OK);
            else
                OnViewStateChanged("UserChoise");
        }

        void MenuItemBackClick(object sender, EventArgs e)
        {
            OnViewStateChanged("GoBack");
        }

        void TextChange(object sender, EventArgs e)
        {
            if (filter.FilterText != Resources.FilterText)
            {
                if (!(filter.FilterText == string.Empty &&
                    ((FriendsListResponse)ViewData["OriginalFrendListResponse"]).Users.Count == Model.Count))
                {
                    filter.ClearButtonVisibleChange(filter.FilterText != string.Empty);
                    ViewData["FilterString"] = filter.FilterText;
                    OnViewStateChanged("FilterFriendList");
                }
            }
            else
            {
                filter.ClearButtonVisibleChange(false);
                ViewData["FilterString"] = string.Empty;
                OnViewStateChanged("FilterFriendList");
            }
        }
    }
}
