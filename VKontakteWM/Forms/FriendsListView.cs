using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Common;
using Galssoft.VKontakteWM.Components.Cache;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.Common.ResponseClasses;
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
    public partial class FriendsListView : UIViewBase, IView<List<FriendListViewItem>>
    {
        public FriendsListView()
        {
            InitializeComponent();
        }

        public void Load()
        {            
            //ViewData["IsRefresh"] = "false";
            //OnViewStateChanged("LoadList");
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
            //klvFriendsList.ReloadUserPhotos();

            //if (needUpdate())
            //{
            //    ViewData["IsRefresh"] = "true";

            //    OnViewStateChanged("LoadList");
            //}
            //else
            //{
            //    if (ViewData["IsRefresh"] == null)
            //    {
            //        ViewData["IsRefresh"] = "false";

            //        OnViewStateChanged("LoadList");
            //    }
            //    else
            //    {
            //        OnViewStateChanged("ListActualization");
            //    }
            //}

            if (ViewData["IsRefresh"] == null) // первый старт
            {
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
            }
            else
            {
                if (Configuration.DataRenewType == DataRenewVariants.UpdateAlways)
                {
                    ViewData["IsRefresh"] = true;

                    OnViewStateChanged("LoadList");
                }
                else
                {
                    OnViewStateChanged("ListActualization");
                }
            }

            filter._filter.TextColor = Color.DarkGray;
            filter.FilterText = Resources.FilterText;
            filter._filter.Focus(false);

            //RenewMessageImage();
        }

        public void OnAfterDeactivate()
        {
            klvFriendsList.ReleaseUserPhotos();
        }

        public void OnAfterActivate()
        {
            (new InputPanel()).Enabled = false;

            toolBar.SelectButton(toolBar.ToolbarButtonFriends);

            klvFriendsList.ReloadUserPhotos();
        }

        public void OnActivate()
        {
            //
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

        public new ViewDataDictionary<List<FriendListViewItem>> ViewData { get; set; }

        public new List<FriendListViewItem> Model { get; set; }

        #endregion

        #region Обработчики нажатий на кнопки формы

        #region BottomToolBar

        private void ButtonNewsClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToNews");
            }
        }

        private void ButtonMessagesClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToMessages");
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
            toolBar.SelectButton(toolBar.ToolbarButtonFriends);
        }

        #endregion

        #region Кнопки формы

        // обновление данных на форме
        private void BtnRefreshClick(object sender, EventArgs e)
        {
            klvFriendsList.DataSource = Model;

            using (new WaitWrapper(false))
            {
                ViewData["IsRefresh"] = true;

                filter._filter.Text = string.Empty;
                filter._filter.Focus(false);

                OnViewStateChanged("LoadList");

                if (filter.FilterText == string.Empty)
                {
                    filter._filter.TextColor = Color.DarkGray;
                    filter.FilterText = Resources.FilterText;
                }
            }
        }

        void BtnFriendsOnlineClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToFriendsOnline");
            }
        }

        void BtnFriendsNewClick(object sender, EventArgs e)
        {

        }

        #endregion

        #region контекстное

        private void menuItemSendMessage_Click(object sender, EventArgs e)
        {
            using (new WaitWrapper())
            {
                OnViewStateChanged("GoToSendMessage");
            }
        }

        private void menuItemSaveNumber_Click(object sender, EventArgs e)
        {
            OnViewStateChanged("SaveNumber");
        }

        private void menuItemCallNumber_Click(object sender, EventArgs e)
        {
            OnViewStateChanged("CallNumber");
        }

        private void menuItemSendSms_Click(object sender, EventArgs e)
        {
            using (new WaitWrapper())
            {

            }
        }
        

        #endregion

        private void KlvFriendsListReturnLongPress(object sender, ListViewLongPressEventArgs e)
        {
            if (e.ItemData != null)
            {
                //klvFriendsList.ShowSelectedItem = true;
                klvFriendsList.Refresh();

                ViewData["Uid"] = klvFriendsList.SelectedItem.Uid;

                if (!string.IsNullOrEmpty(klvFriendsList.SelectedItem.Telephone))
                {
                    menuItemSaveNumber.Enabled = true;
                    menuItemCallNumber.Enabled = true;
                    menuItemSendSms.Enabled = true;

                    // имя
                    if (!string.IsNullOrEmpty(klvFriendsList.SelectedItem.FirstName))
                    {
                        ViewData["ABFirstName"] = klvFriendsList.SelectedItem.FirstName;
                    }
                    else
                    {
                        ViewData["ABFirstName"] = null;
                    }

                    // фамилия
                    if (!string.IsNullOrEmpty(klvFriendsList.SelectedItem.LastName))
                    {
                        ViewData["ABLastName"] = klvFriendsList.SelectedItem.LastName;
                    }
                    else
                    {
                        ViewData["ABLastName"] = null;
                    }

                    // телефон
                    if (!string.IsNullOrEmpty(klvFriendsList.SelectedItem.Telephone))
                    {
                        ViewData["ABTelephone"] = klvFriendsList.SelectedItem.Telephone;
                    }
                    else
                    {
                        ViewData["ABTelephone"] = null;
                    }

                    // картинка
                    if (!string.IsNullOrEmpty(klvFriendsList.SelectedItem.Avatar))
                    {
                        ViewData["ImagePath"] = klvFriendsList.SelectedItem.Avatar;
                    }
                    else
                    {
                        ViewData["ImagePath"] = null;
                    }

                    // день рождения
                    if (!klvFriendsList.SelectedItem.Birthday.Equals(new DateTime(0)))
                    {
                        ViewData["ABBD"] = klvFriendsList.SelectedItem.Birthday;
                    }
                    else
                    {
                        ViewData["ABBD"] = null;
                    }
                }
                else
                {
                    menuItemSaveNumber.Enabled = false;
                    menuItemCallNumber.Enabled = false;
                    menuItemSendSms.Enabled = false;

                    ViewData["ABFirstName"] = null;
                    ViewData["ABLastName"] = null;
                    ViewData["ABTelephone"] = null;
                    ViewData["ABBD"] = null;
                }

                contexMenu.Show((Control)sender, new Point(e.ClickCoordinates.X, e.ClickCoordinates.Y));

                klvFriendsList.SelectedIndex = -1;
                klvFriendsList.Refresh();
            }
        }

        private void KlvFriendsListClick(object sender, MouseEventArgs e)
        {
            if (klvFriendsList.SelectedIndex > -1)
            {
                //klvFriendsList.ShowSelectedItem = true;
                klvFriendsList.Refresh();

                ViewData["Uid"] = klvFriendsList.SelectedItem.Uid;

                if (!string.IsNullOrEmpty(klvFriendsList.SelectedItem.Telephone))
                {
                    menuItemSaveNumber.Enabled = true;
                    menuItemCallNumber.Enabled = true;
                    //menuItemSendSms.Enabled = true;

                    // имя
                    if (!string.IsNullOrEmpty(klvFriendsList.SelectedItem.FirstName))
                    {
                        ViewData["ABFirstName"] = klvFriendsList.SelectedItem.FirstName;
                    }
                    else
                    {
                        ViewData["ABFirstName"] = null;
                    }

                    // фамилия
                    if (!string.IsNullOrEmpty(klvFriendsList.SelectedItem.LastName))
                    {
                        ViewData["ABLastName"] = klvFriendsList.SelectedItem.LastName;
                    }
                    else
                    {
                        ViewData["ABLastName"] = null;
                    }

                    // телефон
                    if (!string.IsNullOrEmpty(klvFriendsList.SelectedItem.Telephone))
                    {
                        ViewData["ABTelephone"] = klvFriendsList.SelectedItem.Telephone;
                    }
                    else
                    {
                        ViewData["ABTelephone"] = null;
                    }

                    // день рождения
                    if (!klvFriendsList.SelectedItem.Birthday.Equals(new DateTime(0)))
                    {
                        ViewData["ABBD"] = klvFriendsList.SelectedItem.Birthday;
                    }
                    else
                    {
                        ViewData["ABBD"] = null;
                    }
                }
                else
                {
                    menuItemSaveNumber.Enabled = false;
                    menuItemCallNumber.Enabled = false;
                    //menuItemSendSms.Enabled = false;

                    ViewData["ABFirstName"] = null;
                    ViewData["ABLastName"] = null;
                    ViewData["ABTelephone"] = null;
                    ViewData["ABBD"] = null;
                }

                contexMenu.Show((Control)sender, new Point(e.X, e.Y));

                klvFriendsList.SelectedIndex = -1;
                klvFriendsList.Refresh();
            }
        }

        #endregion

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
                DialogControl.ShowQuery((string) ViewData["LoadListResponseMessage"], DialogButtons.OK);
            }

            #endregion

            #region SaveNumberResponse

            if (key == "SaveNumberResponse")
            {
                DialogControl.ShowQuery((string)ViewData["SaveNumberResponseMessage"], DialogButtons.OK);
            }

            #endregion

            #region CallNumberResponse

            if (key == "CallNumberResponse")
            {
                DialogControl.ShowQuery((string)ViewData["CallNumberResponseMessage"], DialogButtons.OK);
            }

            #endregion

            if (key == "GoToLogin")
            {
                OnViewStateChanged("GoGoToLogin");
            }

            //RenewMessageImage();
        }

        private void OnFriendAvatarLoad(object sender, EventArgs e)
        {
            AfterLoadFriendAvatarEventArgs ee = (AfterLoadFriendAvatarEventArgs)e;

            string friendAvatarPath = ee.ImageFileName;

            if (!string.IsNullOrEmpty(friendAvatarPath))
            {
                foreach (FriendListViewItem item in klvFriendsList.Items)
                {
                    if (item.Avatar == friendAvatarPath)
                    {
                        item.IsAvatarLoaded = true;
                        klvFriendsList.UpdateUserPhoto(item);

                        OnViewStateChanged("RefreshList");
                    }
                }

                if (ee.ImageLast)
                {
                    //
                }
            }
        }

        void FriendsListView_Resize(object sender, EventArgs e)
        {

        }

        void TextBoxLostFocus(object sender, EventArgs e)
        {
            toolBar.Visible = true;
            klvFriendsList.SuspendLayout();
            ViewManager.HideMenu();
            klvFriendsList.Size = new Size(klvFriendsList.Size.Width, klvFriendsList.Size.Height - toolBar.Height);
            klvFriendsList.ResumeLayout();
        }

        void TextBoxGotFocus(object sender, EventArgs e)
        {
            toolBar.Visible = false;
            klvFriendsList.SuspendLayout();
            ViewManager.ShowMenu(filterMenu);
            klvFriendsList.Size = new Size(klvFriendsList.Size.Width, klvFriendsList.Size.Height + toolBar.Height);
            klvFriendsList.ResumeLayout();
        }


        void InputPanelEnabledChanged(object sender, EventArgs e)
        {
            /*
            if (filter.inputPanel.Enabled) ViewManager.ShowMenu();
            else
            {
                filter._filter.Text = string.Empty;
                filter._filter.Focus(false);
                ViewManager.HideMenu();
            }
            */
        }


        void CancelMenuItemClick(object sender, EventArgs e)
        {
            filter._filter.TextBox.Text = String.Empty;
            klvFriendsList.Focus();
        }

        void TextChange(object sender, EventArgs e)
        {
            if (ViewData["OriginalFrendListResponse"] != null)
            {
                if (filter.FilterText != Resources.FilterText)
                {
                    if (!(filter.FilterText == string.Empty &&
                          ((FriendsListResponse) ViewData["OriginalFrendListResponse"]).Users.Count == Model.Count
                         ))
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

        ///// <summary>
        ///// Проверка на необходимость обновления списка друзей
        ///// </summary>
        ///// <returns></returns>
        //private bool needUpdate()
        //{
        //    try
        //    {
        //        var friendsCache = Cache.LoadFromCache<FriendsListResponse>(string.Empty, "FriendsListResponse");

        //        TimeSpan ts = DateTime.Now - friendsCache.LastUpdate;

        //        var defaultTs = new TimeSpan(0, 5, 0);

        //        if (ts >= defaultTs)
        //        {
        //            return true;
        //        }
        //    }
        //    catch
        //    {
        //        //
        //    }
                                   
        //    return false;
        //}

        /// <summary>
        /// Проверка на необходимость смены иконки сообщений в тулбаре
        /// </summary>
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

        private void MiExitClick(object sender, EventArgs e)
        {
            OnViewStateChanged("GoToExit");
        }

        private void MiAboutClick(object sender, EventArgs e)
        {
            OnViewStateChanged("GoToAbout");
        }

        private void MiSettingsClick(object sender, EventArgs e)
        {
            OnViewStateChanged("GoToSettings");
        }

        private void MiUserDataClick(object sender, EventArgs e)
        {
            OnViewStateChanged("GoToUserData");
        }

        #endregion
    }
}
