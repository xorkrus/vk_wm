using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.Configuration;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Components.UI.Transitions;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Properties;
using Microsoft.WindowsCE.Forms;
using Uri = System.Uri;
using Galssoft.VKontakteWM.Components.UI.WebBrowser;
using System.Collections;
using System.Collections.Generic;
using Galssoft.VKontakteWM.Components.UI.Wrappers;

namespace Galssoft.VKontakteWM.Forms
{
    public partial class WebBrowserView : UIViewBase, IView
    {
        private readonly Timer _enableBackButtonTimer = new Timer();

        public WebBrowserView()
        {
            using (new WaitWrapper())
            {
                InitializeComponent();
                webBrowser.Navigating += ViewWebBrowserNavigating;
                webBrowser.DocumentComplete += ViewWebBrowserDocumentComplete;
                webBrowser.ShowWaitCursorWhileLoading = true;
                webBrowser.SpecialUrlParams = "client=wm";

                menuItemToFriends.Enabled = false;
                menuItemBack.Enabled = false;

                _enableBackButtonTimer.Interval = 3000;
                _enableBackButtonTimer.Tick += _enableBackButton_Tick;
            }
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            DisposeBrowser();
            base.Dispose(disposing);
        }

        private void DisposeBrowser()
        {
            if (webBrowser != null)
            {
                using (new WaitWrapper())
                {
                    Application.DoEvents();
                    //if (webBrowser != null)
                    //	webBrowser.Stop();
                    if (webBrowser != null)
                        webBrowser.Dispose();
                    webBrowser = null;
                    Application.DoEvents();
                    GC.Collect();
                }
            }
        }

        private void _enableBackButton_Tick(object sender, EventArgs e)
        {
            ActivateMenuButtons();
        }

        /// <summary>
        /// Deactivate menu buttons for a while
        /// </summary>
        private void DeactivateMenuButtons()
        {
            menuItemToFriends.Enabled = false;
            menuItemBack.Enabled = false;

            _enableBackButtonTimer.Enabled = true;
        }

        /// <summary>
        /// Activate menu buttons after the time out 
        /// </summary>
        private void ActivateMenuButtons()
        {
            _enableBackButtonTimer.Enabled = false;
            //if (!menuItemBack.Enabled)
            {
                menuItemBack.Enabled = true;
                menuItemToFriends.Enabled = _backButtonVisible;
            }
        }

        /// <summary>
        /// Show the second menu button as active
        /// </summary>
        private bool _backButtonVisible = false;

        public void Load()
        {
            //webBrowser.Navigate("http://m.odnoklassniki.ru");
        }

        #region IView Members

        public string Title { get; set; }

        public MainMenu ViewMenu
        {
            get { return mainMenu; }
        }

        public void OnBeforeActivate()
        {

        }

        public void OnAfterDeactivate()
        {
            webBrowser.Stop();
        }

        public void OnAfterActivate()
        {
            AutoScroll = true;
            Invalidate();
            //Скрытие клавиатуры
            (new InputPanel()).Enabled = false;
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

                    // Pass the graphics to the canvas to render
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

        #endregion

        protected override void OnUpdateView(string key)
        {
            // Что то пришло от контроллера
            if (key == "WebBrowserGoToUrl")
            {
                _backButtonVisible = ViewData.ContainsKey("BackButtonVisible")
                    ? (bool)ViewData["BackButtonVisible"]
                    : false;

                // turn on buttons in time
                DeactivateMenuButtons();

                try
                {
                    webBrowser.Navigate((string)ViewData["Url"]);
                }
                catch (UriFormatException ex)
                {
                    DialogControl.ShowQuery(Resources.WebBrowser_View_UriError, DialogButtons.OK);
                    //MessageBox.Show(Resources.WebBrowser_View_UriError);
                    DebugHelper.WriteLogEntry("UriError: " + ex.Message + "\r\n" + ex.StackTrace);
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteLogEntry("The unexpected browser's error: " + ex.Message + "\r\n" + ex.StackTrace);
                }
            }
            else if (key == "ChangeTitle")
            {
                Title = (string)ViewData["Title"];
                menuItemBack.Text = (string)ViewData["FirstButtonTitle"];
                menuItemToFriends.Text = (string)ViewData["BackButtonTitle"];
            }
            else if (key == "AutoLoginFail")
            {
                var err = (string)ViewData["AutoLoginError"];
                DialogControl.ShowQuery(err, DialogButtons.OK);
                //MessageBox.Show(err);
            }
            else if (key == "WebBrowserStop")
            {
                webBrowser.Stop();
                webBrowser.Navigate("about:blank");
            }
        }

        private void ViewWebBrowserDocumentComplete(WebBrowserAPI.HtmlViewMessage message)
        {
            ActivateMenuButtons();

            ViewData["DocumentCompleteUrl"] = message.Url;
            OnViewStateChanged("DocumentCompleted");
        }

        private void ViewWebBrowserNavigating(object sender, WebBrowserNavigatingExEventArgs e)
        {
            /*
            // Enable POST data

            if (!String.IsNullOrEmpty(e.Data))
            {
                e.Cancel = false;
                return;
            }

            // Process other GET requests

            if (e.Url == null && e.TargetFrameName != null)
            {
                e.Cancel = false;
                return;
            }
            else if (e.Url.Scheme == "file")
            {
                if (!String.IsNullOrEmpty(e.TargetFrameName)
                    && e.TargetFrameName[0] == '#'
                    && e.Url.ToString() == webBrowser.Url.ToString())
                {
                    e.Cancel = false;
                    return;
                }
            }
            else if (e.Url.Scheme == "http" || e.Url.Scheme == "https")
            {
                Uri url = String.IsNullOrEmpty(e.Url.Host) ?
                    new Uri(SystemConfiguration.MobileSiteUrl + e.Url.LocalPath) :
                    new Uri(e.Url.ToString());

                if (url.Host == new Uri(SystemConfiguration.MobileSiteUrl).Host)
                {
                    //Если не внешний сайт (== http://m.odnoklassniki.ru)
                    ViewData["Url"] = url.ToString();
                    OnViewStateChanged("GoToUrl");
                }
                else
                {
                    //Открыть в default браузере
                    var dRes = DialogControl.ShowQuery(Resources.WebBrowser_dialog_Text, DialogButtons.YesNo);
                    switch (dRes)
                    {
                        case DialogResult.Yes:
                            webBrowser.Stop();
                            CoreHelper.NavigateBrowserExternal(e.Url.ToString() + e.TargetFrameName);
                            break;
                        case DialogResult.No:
                            break;
                    }
                }
            }

            e.Cancel = true;
            */
        }

        private void menuItemBack_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;
            OnViewStateChanged("GoToApp");
        }

        private void menuItemToApp_Click(object sender, EventArgs e)
        {
            OnViewStateChanged("GoBack");
        }
    }
}
