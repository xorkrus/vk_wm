using System;
using System.Collections.Generic;
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
    public partial class MessagesChainsListView : UIViewBase, IView<List<MessagesChainsListViewItem>>
    {
        public MessagesChainsListView()
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
            get { return Resources.MessagesChainsList_View_Title; }
        }

        public MainMenu ViewMenu
        {
            get { return null; }
        }

        public void OnBeforeActivate()
        {
            //RenewMessageImage();

            klvMessagesChainsList.SelectedIndex = -1;

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
        }

        public void OnAfterDeactivate()
        {
            //
        }

        public void OnAfterActivate()
        {
            (new InputPanel()).Enabled = false;

            toolBar.SelectButton(toolBar.ToolbarButtonMessages);           
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

        public new ViewDataDictionary<List<MessagesChainsListViewItem>> ViewData { get; set; }

        public new List<MessagesChainsListViewItem> Model { get; set; }

        #endregion

        protected override void OnUpdateView(string key)
        {
            #region LoadListResponse

            if (key == "LoadListResponse")
            {
                klvMessagesChainsList.Clear();

                klvMessagesChainsList.DataSource = Model;

                klvMessagesChainsList.Reload();
            }

            #endregion

            #region ReloadListResponse

            if (key == "ReloadListResponse")
            {
                klvMessagesChainsList.Reload();
            }

            #endregion

            #region RefreshListResponse

            if (key == "RefreshListResponse")
            {
                klvMessagesChainsList.Refresh();
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

            //RenewMessageImage();
        }

        #region кнопочки

        #region менюха

        private void ButtonNewsClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToNews");
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
            toolBar.SelectButton(toolBar.ToolbarButtonMessages);
        }

        #endregion

        #region формочка

        private void BtnRefreshClick(object sender, EventArgs e)
        {
            //klvMessagesChainsList.DataSource = Model;

            ViewData["IsRefresh"] = true;

            OnViewStateChanged("LoadList");            
        }

        void BtnSentMessageClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("ChangeReceiver");
            }
        }  

        #endregion

        #endregion

        void MessagesChainsListView_Resize(object sender, EventArgs e)
        {
            btnSentMessage.Location = new Point((Width - btnSentMessage.Width) / 2, UISettings.CalcPix(5));
            btnSentMessageText.Location = btnSentMessage.Location;
            btnSentMessageShadowText.Location = new Point(btnSentMessage.Location.X, btnSentMessage.Location.Y - 1);
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
