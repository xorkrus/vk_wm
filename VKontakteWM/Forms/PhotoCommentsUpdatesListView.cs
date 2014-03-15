using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Common;
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
    public partial class PhotoCommentsUpdatesListView : UIViewBase, IView<List<PhotoCommentsUpdatesViewItem>>
    {
        public PhotoCommentsUpdatesListView()
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
            get { return Resources.PhotoCommentsUpdatesList_View_Title; }
        }

        public MainMenu ViewMenu
        {
            get { return null; }
        }

        public void OnBeforeActivate()
        {
            //RenewMessageImage();

            //if (Configuration.DataRenewType == DataRenewVariants.UpdateAlways)
            //{
            //    ViewData["IsRefresh"] = true;

            //    OnViewStateChanged("LoadList");
            //}
            //else
            //{
            //    ViewData["IsRefresh"] = false;

            //    OnViewStateChanged("LoadList");
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
           
            klvPhotoCommentsUpdatesList.ReloadUserPhotos();            
        }

        public void OnAfterDeactivate()
        {
            klvPhotoCommentsUpdatesList.ReleaseUserPhotos();
        }

        public void OnActivate()
        {
            //
        }

        public void OnAfterActivate()
        {
            (new InputPanel()).Enabled = false;
            
            toolBar.SelectButton(toolBar.ToolbarButtonNews);
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

        public new ViewDataDictionary<List<PhotoCommentsUpdatesViewItem>> ViewData { get; set; }

        public new List<PhotoCommentsUpdatesViewItem> Model { get; set; }

        #endregion

        #region кнопочки

        //меню

        private void ToolbarButtonExtras_Click(object sender, EventArgs e)
        {
            toolBar.SelectButton(toolBar.ToolbarButtonExtras);
            toolBar.contextMenu.Show(this, new Point(Width, Height - toolBar.GetCurrentShift()));
            toolBar.SelectButton(toolBar.ToolbarButtonNews);
        }

        private void ToolbarButtonPhotos_Click(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToPhotos");
            }
        }

        private void ToolbarButtonFriends_Click(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToFriends");
            }
        }

        private void ToolbarButtonMessages_Click(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToMessages");
            }
        }        

        //форма

        void btnRefresh_Click(object sender, EventArgs e)
        {
            //klvPhotoCommentsUpdatesList.DataSource = Model;

            ViewData["IsRefresh"] = true;

            OnViewStateChanged("LoadList");           
        }

        void btnPhotoCommentsUpdates_Click(object sender, EventArgs e)
        {
            //using (new WaitWrapper())
            //{
            //    OnViewStateChanged("NewComment");
            //}
        }

        void btnStatusUpdates_Click(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToStatuses");
            }
        }

        // контекстное меню

        private void menuItemSendMessage_Click(object sender, EventArgs e)
        {
            using (new WaitWrapper())
            {
                OnViewStateChanged("GoToSendMessage");
            }
        }

        private void menuItemSendComment_Click(object sender, EventArgs e)
        {
            using (new WaitWrapper())
            {
                OnViewStateChanged("NewComment");
            }
        }

        private void menuItemViewComments_Click(object sender, EventArgs e)
        {
            using (new WaitWrapper())
            {
                OnViewStateChanged("GoDetailedView");
            }
        }

        private void klvPhotoCommentsUpdatesList_ReturnLongPress(object sender, ListViewLongPressEventArgs e)
        {
            if (e.ItemData != null)
            {                                
                klvPhotoCommentsUpdatesList.Refresh();

                ViewData["Uid"] = Globals.BaseLogic.IDataLogic.GetUid(); //klvPhotoCommentsUpdatesList.SelectedItem.UserID;
                ViewData["PhotoID"] = klvPhotoCommentsUpdatesList.SelectedItem.PhotoID;
                ViewData["LargePhotoURL"] = klvPhotoCommentsUpdatesList.SelectedItem.LargePhotoURL;
                ViewData["SenderName"] = klvPhotoCommentsUpdatesList.SelectedItem.SenderName;
                ViewData["SenderID"] = klvPhotoCommentsUpdatesList.SelectedItem.UserID;

                if (Globals.BaseLogic.IDataLogic.GetUid() == klvPhotoCommentsUpdatesList.SelectedItem.UserID)
                {
                    menuItemSendMessage.Enabled = false;
                }
                else
                {
                    menuItemSendMessage.Enabled = true;
                }

                contexMenu.Show((Control)sender, new Point(e.ClickCoordinates.X, e.ClickCoordinates.Y));

                klvPhotoCommentsUpdatesList.SelectedIndex = -1;
                klvPhotoCommentsUpdatesList.Refresh();
            }
        }

        private void klvPhotoCommentsUpdatesList_MouseUp(object sender, MouseEventArgs e)
        {
            if (klvPhotoCommentsUpdatesList.SelectedIndex > -1)
            {
                klvPhotoCommentsUpdatesList.Refresh();

                ViewData["Uid"] = Globals.BaseLogic.IDataLogic.GetUid(); //klvPhotoCommentsUpdatesList.SelectedItem.UserID;
                ViewData["PhotoID"] = klvPhotoCommentsUpdatesList.SelectedItem.PhotoID;
                ViewData["LargePhotoURL"] = klvPhotoCommentsUpdatesList.SelectedItem.LargePhotoURL;
                ViewData["SenderName"] = klvPhotoCommentsUpdatesList.SelectedItem.SenderName;
                ViewData["SenderID"] = klvPhotoCommentsUpdatesList.SelectedItem.UserID;

                if (Globals.BaseLogic.IDataLogic.GetUid() == klvPhotoCommentsUpdatesList.SelectedItem.UserID)
                {
                    menuItemSendMessage.Enabled = false;
                }
                else
                {
                    menuItemSendMessage.Enabled = true;
                }

                contexMenu.Show((Control)sender, new Point(e.X, e.Y));

                klvPhotoCommentsUpdatesList.SelectedIndex = -1;
                klvPhotoCommentsUpdatesList.Refresh();
            }
        }

        #endregion

        protected override void OnUpdateView(string key)
        {
            #region LoadListResponse

            if (key == "LoadListResponse")
            {
                klvPhotoCommentsUpdatesList.Clear();

                klvPhotoCommentsUpdatesList.DataSource = Model;

                klvPhotoCommentsUpdatesList.Reload();
            }

            #endregion

            #region ReloadListResponse

            if (key == "ReloadListResponse")
            {
                klvPhotoCommentsUpdatesList.Reload();
            }

            #endregion

            #region RefreshListResponse

            if (key == "RefreshListResponse")
            {
                klvPhotoCommentsUpdatesList.Refresh();
            }

            #endregion

            #region LoadListResponseNegative

            if (key == "LoadListResponseNegative")
            {
                DialogControl.ShowQuery((string) ViewData["LoadListResponseMessage"], DialogButtons.OK);
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
                foreach (PhotoCommentsUpdatesViewItem item in klvPhotoCommentsUpdatesList.Items)
                {
                    if (item.Photo == friendAvatarPath)
                    {
                        item.IsPhotoLoaded = true;
                        klvPhotoCommentsUpdatesList.UpdateUserPhoto(item);

                        OnViewStateChanged("RefreshList");
                    }
                }

                if (ee.ImageLast)
                {
                    //
                }
            }
        }

        void PhotoCommentsUpdatesListViewResize(object sender, EventArgs e)
        {
            btnStatusUpdates.Location = new Point((int)((Width - 2 * btnStatusUpdates.Width) / 2),
                (int)((header.Height - btnStatusUpdates.Height) / 2));
            btnPhotoCommentsUpdates.Location = new Point((int)((Width - 2 * btnStatusUpdates.Width) / 2) + btnPhotoCommentsUpdates.Width,
                (int)((header.Height - btnPhotoCommentsUpdates.Height) / 2));

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
