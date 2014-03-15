using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Components.UI.Transitions;
using Galssoft.VKontakteWM.Components.UI.Wrappers;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Properties;
using Galssoft.VKontakteWM.ApplicationLogic;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;
using Microsoft.WindowsCE.Forms;

namespace Galssoft.VKontakteWM.Forms
{
    public partial class MessagesListView : UIViewBase, IView<List<MessagesListViewItem>>
    {
        public MessagesListView()
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
            get { return Resources.MessagesList_Designer_Title; }
        }

        public MainMenu ViewMenu
        {
            get { return mainMenu; }
        }

        public void OnBeforeActivate()
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

            CoreHelper.SetSipButton(false);
        }

        public void OnAfterDeactivate()
        {
            CoreHelper.SetSipButton(true);

            Model.Clear();

            klvMessagesList.DataSource = Model;

            klvMessagesList.Reload();
        }

        public void OnAfterActivate()
        {
            if (!(Model.Count > 0))
            {
                //NavigationService.GoBack();
            }

            klvMessagesList.ScrollToEnd();
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

        public new ViewDataDictionary<List<MessagesListViewItem>> ViewData { get; set; }

        public new List<MessagesListViewItem> Model { get; set; }

        #endregion

        protected override void OnUpdateView(string key)
        {
            #region LoadListResponse

            if (key == "LoadListResponse")
            {
                klvMessagesList.Clear();

                klvMessagesList.DataSource = Model;

                klvMessagesList.Reload();

                headerText.Text = headerShadowText.Text = (string)ViewData["UserName"];

                //string strMessageDraftInput = (string)ViewData["MessageDraftInput"];

                //tbxMessageData.Text = strMessageDraftInput;

                //klvMessagesList.ScrollToEnd();
            }

            #endregion

            #region ReloadListResponse

            if (key == "ReloadListResponse")
            {
                klvMessagesList.Reload();
                //klvMessagesList.ScrollToEnd();
            }

            #endregion

            #region RefreshListResponse

            if (key == "RefreshListResponse")
            {
                klvMessagesList.Refresh();
                //klvMessagesList.ScrollToEnd();
            }

            #endregion

            #region LoadListResponseNegative

            if (key == "LoadListResponseNegative")
            {
                DialogControl.ShowQuery((string) ViewData["LoadListResponseMessage"], DialogButtons.OK);
            }

            #endregion

            #region SentMessageResponse

            if (key == "SentMessageResponse")
            {
                DialogControl.ShowQuery((string)ViewData["SentMessageResponseMessage"], DialogButtons.OK);

                ViewData["SentMessageResponseMessage"] = string.Empty;

                if (Convert.ToBoolean((string)ViewData["MessageIsSent"]))
                {
                    tbxMessageData.Text = string.Empty;
                    
                    ViewData["IsRefresh"] = "true";
                    OnViewStateChanged("LoadList");
                }
            }

            #endregion

            if (key == "GoToLogin")
            {
                OnViewStateChanged("GoGoToLogin");
            }
        }

        #region обработка событий компонентов

        public void menuItemBack_Click(object sender, EventArgs e)
        {
            ViewData["MessageDraftOutput"] = tbxMessageData.Text;
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("Back");
            }
        }

        public void BtnRefreshClick(object sender, EventArgs e)
        {
            ViewData["IsRefresh"] = true;

            OnViewStateChanged("LoadList");

            klvMessagesList.ScrollToEnd();
        }

        public void menuItemSent_Click(object sender, EventArgs e)
        {
            /*
            if (!string.IsNullOrEmpty(tbxMessageData.Text.Trim()))
            {
                ViewData["MessageDraftOutput"] = tbxMessageData.Text.Trim();

                OnViewStateChanged("SentMessage");
            }
            else
            {
                DialogControl.ShowQuery(Resources.MessagesList_View_MessageIsEmpty, DialogButtons.OK);
            }
            */
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("SentMessage");
            }
        }

        public void tbxMessageDataGotFocus(object sender, EventArgs e)
        {
            tbxMessageData.ScrollBars = ScrollBars.Vertical;
        }

        public void tbxMessageDataLostFocus(object sender, EventArgs e)
        {
            tbxMessageData.ScrollBars = ScrollBars.None;
        }
        

        #endregion
    }
}
