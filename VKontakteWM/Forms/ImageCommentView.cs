using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.ApplicationLogic;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;
using Galssoft.VKontakteWM.Components.UI.Transitions;
using Galssoft.VKontakteWM.Components.UI.Wrappers;
using Galssoft.VKontakteWM.Properties;
using Microsoft.WindowsCE.Forms;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Common;

namespace Galssoft.VKontakteWM.Forms
{
    public partial class ImageCommentView : UIViewBase, IView<List<ImageDetailedListViewItem>>
    {
        public ImageCommentView()
        {
            InitializeComponent();
        }

        public void Load()
        {
            //
        }

        #region IView Members

        public string Title
        {
            get { return Resources.ImageCommentView_Title; }
        }

        public MainMenu ViewMenu
        {
            get { return mainMenu; }
        }

        public void OnBeforeActivate()
        {
            ViewData["ListMinSize"] = Math.Min(klvDetailed.Width, klvDetailed.Height);

            if (Configuration.DataRenewType == DataRenewVariants.UpdateAlways ||
                Configuration.DataRenewType == DataRenewVariants.AutoUpdateAtStart)
            {
                ViewData["IsRefresh"] = true;

                OnViewStateChanged("LoadList");
            }
            else
            {
                ViewData["IsRefresh"] = false;

                OnViewStateChanged("LoadList");
            }

            klvDetailed.SelectedIndex = -1;
        }

        public void OnAfterDeactivate()
        {
            klvDetailed.ReleaseUserPhotos();

            //проверить
            //klvDetailed.Clear();

            Model.Clear();

            klvDetailed.DataSource = Model;

            klvDetailed.Reload();
        }

        public void OnAfterActivate()
        {
            (new InputPanel()).Enabled = false;

            if (!(Model.Count > 0))
            {
                //NavigationService.GoBack();
                menuItemPutInAlbum.Enabled = false;
                menuItemSaveOnDisk.Enabled = false;
                menuItemSendComment.Enabled = false;
            }
            else
            {
                menuItemPutInAlbum.Enabled = true;
                menuItemSaveOnDisk.Enabled = true;
                menuItemSendComment.Enabled = true;
            }
        }

        //public void OnActivate()
        //{
        //    if (!(Model.Count > 0))
        //    {
        //        NavigationService.GoBack();
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

        public new ViewDataDictionary<List<ImageDetailedListViewItem>> ViewData { get; set; }

        public new List<ImageDetailedListViewItem> Model { get; set; }

        #endregion

        protected override void OnUpdateView(string key)
        {
            #region MainResponse

            if (key == "MainResponse")
            {
                DialogControl.ShowQuery((string)ViewData["ResponseMessage"], DialogButtons.OK);
            }

            #endregion

            #region LoadListResponse

            if (key == "LoadListResponse")
            {
                klvDetailed.Clear();

                klvDetailed.DataSource = Model;

                klvDetailed.Reload();

                if (klvDetailed.DataSource.Count > 0)
                {
                    menuItemPutInAlbum.Enabled = true;
                    menuItemSaveOnDisk.Enabled = true;
                    menuItemSendComment.Enabled = true;
                }
            }

            #endregion

            #region ReloadListResponse

            if (key == "ReloadListResponse")
            {
                //klvDetailed.Clear();

                //klvDetailed.DataSource = Model;

                klvDetailed.Reload();

                if (klvDetailed.DataSource.Count > 0)
                {
                    menuItemPutInAlbum.Enabled = true;
                    menuItemSaveOnDisk.Enabled = true;
                    menuItemSendComment.Enabled = true;
                }
            }

            #endregion

            #region RefreshListResponse

            if (key == "RefreshListResponse")
            {
                klvDetailed.Refresh();
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

        private void OnFriendAvatarLoad(object sender, EventArgs e)
        {
            AfterLoadFriendAvatarEventArgs ee = (AfterLoadFriendAvatarEventArgs)e;

            string friendAvatarPath = ee.ImageFileName;

            if (!string.IsNullOrEmpty(friendAvatarPath))
            {
                foreach (ImageDetailedListViewItem item in klvDetailed.Items)
                {
                    if (item.Photo == friendAvatarPath)
                    {
                        item.IsPhotoLoaded = true;
                        item.PhotoHeight = ee.ImageHeight;
                        klvDetailed.UpdateUserPhoto(item);

                        OnViewStateChanged("ReloadList");

                        //OnViewStateChanged("RefreshList");
                    }
                }

                if (ee.ImageLast)
                {
                    //
                }
            }
        }



        private void MenuItemCancelClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoBack");
            }
        }

        private void menuItemRefresh_Click(object sender, EventArgs e)
        {
            ViewData["IsRefresh"] = true;

            OnViewStateChanged("LoadList");
        }

        void MenuItemChangeClick(object sender, EventArgs e)
        {
            //
        }

        private void MenuItemSaveOnDiskClick(object sender, EventArgs e)
        {
            OnViewStateChanged("SaveOnDisk");
        }

        private void MenuItemPutInAlbumClick(object sender, EventArgs e)
        {
            OnViewStateChanged("PutInAlbum");
        }

        private void MenuItemSendCommentClick(object sender, EventArgs e)
        {
            OnViewStateChanged("NewComment");
        }

        private void menuItemSendMessage_Click(object sender, EventArgs e)
        {
            using (new WaitWrapper())
            {
                OnViewStateChanged("GoToSendMessage");
            }
        }

        private void klvDetailed_MouseUp(object sender, MouseEventArgs e)
        {
            if (klvDetailed.SelectedIndex > -1)
            {
                if (klvDetailed.SelectedItem.Type != ImageDetailedListViewItemType.Comment)
                {
                    return;
                }

                klvDetailed.Refresh();

                //ViewData["Uid"] = Globals.BaseLogic.IDataLogic.GetUid(); //klvPhotoCommentsUpdatesList.SelectedItem.UserID;
                //ViewData["PhotoID"] = klvPhotoCommentsUpdatesList.SelectedItem.PhotoID;
                //ViewData["LargePhotoURL"] = klvPhotoCommentsUpdatesList.SelectedItem.LargePhotoURL;
                //ViewData["SenderName"] = klvPhotoCommentsUpdatesList.SelectedItem.SenderName;
                //ViewData["SenderID"] = klvPhotoCommentsUpdatesList.SelectedItem.UserID;

                ViewData["SenderName"] = klvDetailed.SelectedItem.UserName;
                ViewData["SenderID"] = klvDetailed.SelectedItem.UserID;

                if (Globals.BaseLogic.IDataLogic.GetUid() == klvDetailed.SelectedItem.UserID)
                {
                    menuItemSendMessage.Enabled = false;
                }
                else
                {
                    menuItemSendMessage.Enabled = true;
                }

                contexMenu.Show((Control)sender, new Point(e.X, e.Y));

                klvDetailed.SelectedIndex = -1;
                klvDetailed.Refresh();
            }

            //if (klvPhotoCommentsUpdatesList.SelectedIndex > -1)
            //{
            //    klvPhotoCommentsUpdatesList.Refresh();

            //    ViewData["Uid"] = Globals.BaseLogic.IDataLogic.GetUid(); //klvPhotoCommentsUpdatesList.SelectedItem.UserID;
            //    ViewData["PhotoID"] = klvPhotoCommentsUpdatesList.SelectedItem.PhotoID;
            //    ViewData["LargePhotoURL"] = klvPhotoCommentsUpdatesList.SelectedItem.LargePhotoURL;
            //    ViewData["SenderName"] = klvPhotoCommentsUpdatesList.SelectedItem.SenderName;
            //    ViewData["SenderID"] = klvPhotoCommentsUpdatesList.SelectedItem.UserID;

            //    if (Globals.BaseLogic.IDataLogic.GetUid() == klvPhotoCommentsUpdatesList.SelectedItem.UserID)
            //    {
            //        menuItemSendMessage.Enabled = false;
            //    }
            //    else
            //    {
            //        menuItemSendMessage.Enabled = true;
            //    }

            //    contexMenu.Show((Control)sender, new Point(e.X, e.Y));

            //    klvPhotoCommentsUpdatesList.SelectedIndex = -1;
            //    klvPhotoCommentsUpdatesList.Refresh();
            //}
        }

        private void klvDetailed_ReturnLongPress(object sender, Galssoft.VKontakteWM.Components.UI.CompoundControls.ListViewLongPressEventArgs e)
        {
            if (e.ItemData != null)
            {
                if (klvDetailed.SelectedItem.Type != ImageDetailedListViewItemType.Comment)
                {
                    return;
                }

                //klvPhotoCommentsUpdatesList.Refresh();
                klvDetailed.Refresh();

                //ViewData["Uid"] = Globals.BaseLogic.IDataLogic.GetUid(); //klvPhotoCommentsUpdatesList.SelectedItem.UserID;
                //ViewData["PhotoID"] = klvPhotoCommentsUpdatesList.SelectedItem.PhotoID;
                //ViewData["LargePhotoURL"] = klvPhotoCommentsUpdatesList.SelectedItem.LargePhotoURL;

                ViewData["SenderName"] = klvDetailed.SelectedItem.UserName;
                ViewData["SenderID"] = klvDetailed.SelectedItem.UserID;

                //if (Globals.BaseLogic.IDataLogic.GetUid() == klvPhotoCommentsUpdatesList.SelectedItem.UserID)

                if (Globals.BaseLogic.IDataLogic.GetUid() == klvDetailed.SelectedItem.UserID)
                {
                    menuItemSendMessage.Enabled = false;
                }
                else
                {
                    menuItemSendMessage.Enabled = true;
                }

                contexMenu.Show((Control)sender, new Point(e.ClickCoordinates.X, e.ClickCoordinates.Y));

                //klvPhotoCommentsUpdatesList.SelectedIndex = -1;
                //klvPhotoCommentsUpdatesList.Refresh();
                klvDetailed.SelectedIndex = -1;
                klvDetailed.Refresh();
            }
        }
    }
}
