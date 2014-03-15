using System;
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
    public partial class ShareView : UIViewBase, IView<NewsItems>
    {
        public ShareView()
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
            get { return Resources.ShareView_Title; }
        }

        public MainMenu ViewMenu
        {
            get { return null; }
        }

        public void OnBeforeActivate()
        {
            //RenewMessageImage();

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
                    ViewData["IsRefresh"] = false;

                    OnViewStateChanged("ListActualization");
                }
            }            
        }

        public void OnActivate()
        {
            //
        }

        public void OnAfterDeactivate()
        {
            //(new InputPanel()).Enabled = false;
        }

        public void OnAfterActivate()
        {
            (new InputPanel()).Enabled = false;

            toolBar.SelectButton(toolBar.ToolbarButtonPhotos);
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

        public new ViewDataDictionary<NewsItems> ViewData { get; set; }

        public new NewsItems Model { get; set; }

        #endregion

        protected override void OnUpdateView(string key)
        {
            #region LoadListResponse

            if (key == "LoadListResponse")
            {
                klvStatusUpdatesList.Clear();

                klvStatusUpdatesList.DataSource = Model.Statuses;

                klvStatusUpdatesList.Reload();
            }

            #endregion

            #region ReloadListResponse

            if (key == "ReloadListResponse")
            {
                klvStatusUpdatesList.Reload();
            }

            #endregion

            #region RefreshListResponse

            if (key == "RefreshListResponse")
            {
                klvStatusUpdatesList.Refresh();
            }

            #endregion

            #region LoadListResponseNegative

            if (key == "LoadListResponseNegative")
            {
                DialogControl.ShowQuery((string)ViewData["LoadListResponseMessage"], DialogButtons.OK);
            }

            #endregion

            #region MainResponse

            if (key == "MainResponse")
            {
                // выводим сообщение
                string response = (string)ViewData["ResponseMessage"];

                if (!string.IsNullOrEmpty(response))
                {
                    DialogControl.ShowQuery(response, DialogButtons.OK);

                    ViewData["ResponseMessage"] = string.Empty;
                }

                Refresh();
            }

            #endregion

            if (key == "GoToLogin")
            {
                OnViewStateChanged("GoGoToLogin");
            }

            //RenewMessageImage();
        }

        #region форма

        private void wbwiChangeStatus_Click(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("ChangeStatus");
            }
        }

        private void wbwiLoadImage_Click(object sender, EventArgs e)
        {
            OnViewStateChanged("LoadPhotoFromDisk");
        }

        private void wbwiSnapPhoto_Click(object sender, EventArgs e)
        {
            OnViewStateChanged("SnapPhotoWithinCamera");
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ViewData["IsRefresh"] = true;

            OnViewStateChanged("LoadList");

            if (Configuration.DataRenewType != DataRenewVariants.UpdateAlways)
                ViewData["IsRefresh"] = false;
        }

        #endregion

        #region меню

        private void ToolbarButtonNewsClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToNews");
            }
        }

        private void ToolbarButtonMessagesClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToMessages");
            }
        }

        private void ToolbarButtonFriendsClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToFriends");
            }
        }

        private void ToolbarButtonExtrasClick(object sender, EventArgs e)
        {
            toolBar.SelectButton(toolBar.ToolbarButtonExtras);
            toolBar.contextMenu.Show(this, new Point(Width, Height - toolBar.GetCurrentShift()));
            toolBar.SelectButton(toolBar.ToolbarButtonPhotos);
        }

        #endregion

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
