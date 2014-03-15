using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Common;
using Galssoft.VKontakteWM.Components;
using Galssoft.VKontakteWM.Components.Common.Localization;
using Galssoft.VKontakteWM.Components.Configuration;
using Galssoft.VKontakteWM.Components.UI.Wrappers;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Properties;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    class WebBrowserController : Controller
    {
        #region Constructors

        public WebBrowserController()
            : base(new WebBrowserView())
        {
            Name = "WebBrowserController";
            _urlHistory = new Stack<string>();
        }

        #endregion

        #region Controller implementations

        public override void Activate()
        {
            //AutoLogin();
            Cursor.Current = Cursors.WaitCursor;
            view.Activate();
        }

        public override void Deactivate()
        {
            //ViewData["Url"] = "about:blank";
            //view.UpdateView("WebBrowserStop");
            _urlHistory.Clear();
            view.Deactivate();
        }

        /// <summary>
        /// This method indicates that something has been changed in the view.
        /// </summary>
        /// <param name="key">The string key to identify what has been changed.</param>
        protected override void OnViewStateChanged(string key)
        {
            if (key == "GoBack")
            {
                if (_helpFileUrl != null)
                {
                    ViewData["Url"] = _helpFileUrl + "#top";
                    ViewData["BackButtonVisible"] = true;
                    view.UpdateView("WebBrowserGoToUrl");
                }
                else if (_urlHistory.Count > 1)
                {
                    _urlHistory.Pop();
                    ViewData["Url"] = _urlHistory.Peek();
                    ViewData["BackButtonVisible"] = _urlHistory.Count > 1;
                    view.UpdateView("WebBrowserGoToUrl");
                }
            }

            //if (key == "GoBack")
            //{
            //    if (_urlHistory.Count > 1)
            //    {
            //        ViewData["Url"] = _urlHistory[0];
            //		ViewData["BackButtonVisible"] = _urlHistory.Count > 1;
            //        view.UpdateView("WebBrowserGoToUrl");
            //        _urlHistory.Clear();
            //    }
            //}

            else if (key == "GoToUrl")
            {
                _st_cmd_errors = 0;
                _autoLoginErrors = 0;

                _urlHistory.Push((string)ViewData["Url"]);
                ViewData["BackButtonVisible"] = _urlHistory.Count > 1;
                view.UpdateView("WebBrowserGoToUrl");
            }
            else if (key == "GoToApp")
            {
                view.UpdateView("WebBrowserStop");
                _urlHistory.Clear();
                _helpFileUrl = null;
                NavigationService.GoBack();
            }
            else if (key == "DocumentCompleted")
            {
                string url = ViewData["DocumentCompleteUrl"] as string;
                if (url != null)
                {
                    Uri uri = null;
                    try
                    {
                        uri = new Uri(url);
                    }
                    catch (UriFormatException)
                    { }

                    string path = uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);
                    if (path == "apphook/logoff")
                    {
                        //у пользователя скидывался пароль 
                        Globals.BaseLogic.IDataLogic.SetToken("");
                        //пользователя сразу переводили из формы браузера на форму логина
                        MasterForm.Navigate<LoginController>();
                    }
                    else if (path == "apphook/login")
                    {
                        if (_autoLoginErrors == 0)
                        {
                            _autoLoginErrors++;
                            //Обновление сессии
                            AutoLogin();
                            //открыть ту же самую страницу, что пользователь запрашивал ранее
                            view.UpdateView("WebBrowserGoToUrl");
                        }
                    }
                    else
                    {
                        int i = url.IndexOf('?');
                        if (i > -1 && i + 1 < url.Length)
                        {
                            string query = url.Substring(i + 1);
                            if (query.Length > 0)
                            {
                                string[] qparams = query.Split(new char[] { '&' });
                                for (i = 0; i < qparams.Length; i++)
                                {
                                    if (qparams[i] == "st.cmd=error")
                                    {
                                        if (_st_cmd_errors == 0)
                                        {
                                            _st_cmd_errors++;
                                            AutoLogin();
                                            view.UpdateView("WebBrowserGoToUrl");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Counter for web site errors
        /// </summary>
        private int _st_cmd_errors = 0;

        private int _autoLoginErrors = 0;

        private Stack<string> _urlHistory;

        /// <summary>
        /// Link to the help file if user views help
        /// </summary>
        private string _helpFileUrl = null;

        private void AutoLogin()
        {
            try
            {
                Globals.BaseLogic.AutoLogin();
            }
            catch (VKException ex)
            {
                string errMessage = "";
                switch (ex.LocalizedMessage)
                {
                    case ExceptionMessage.IncorrectLoginOrPassword:
                        errMessage = Resources.IncorrectLoginOrPassword;
                        break;
                    case ExceptionMessage.NoConnection:
                        errMessage = Resources.NoConnection;
                        break;
                    case ExceptionMessage.ServerUnavalible:
                        errMessage = Resources.VK_ERRORS_ServerUnavalible;
                        break;
                    case ExceptionMessage.UnknownError:
                        errMessage = Resources.VK_ERRORS_UnknownError;
                        break;
                    case ExceptionMessage.NoSavedToken:
                        errMessage = Resources.VK_ERRORS_NoSavedToken;
                        break;
                    case ExceptionMessage.AccountBloked:
                        errMessage = Resources.VK_ERRORS_AccountBloked;
                        break;
                }
                if (!String.IsNullOrEmpty(errMessage))
                {
                    ViewData["AutoLoginError"] = errMessage;
                    view.UpdateView("AutoLoginFail");

                    if (ex.LocalizedMessage == ExceptionMessage.IncorrectLoginOrPassword)
                    {
                        Globals.BaseLogic.IDataLogic.SetToken("");
                        MasterForm.Navigate<LoginController>();
                    }
                }
            }
            catch (OutOfMemoryException)
            {
                // недостаточно памяти
                ViewData["AutoLoginError"] = Resources.OutOfMemory;
                view.UpdateView("AutoLoginFail");
            }
        }

        protected override void OnInitialize(params object[] parameters)
        {
            //using (new WaitWrapper(false))
            {
                if ((parameters != null) && (parameters.Length > 0))
                {
                    //uid
                    var param = parameters[0] as string;

                    if (param == "Help")
                    {
                        //Переход на справку
                        ViewData["Title"] = Resources.WebBrowser_View_Help_Title;
                        ViewData["FirstButtonTitle"] = Resources.HelpBackButton_Title;
                        ViewData["BackButtonTitle"] = Resources.HelpIndexButton_Title;
                        view.UpdateView("ChangeTitle");

                        _helpFileUrl = "file://" + SystemConfiguration.AppInstallPath + "\\Help\\Index-RU.htm";
                        //_urlHistory.Push(url);
                        ViewData["Url"] = _helpFileUrl;
                        ViewData["BackButtonVisible"] = true;
                        view.UpdateView("WebBrowserGoToUrl");
                    }
                    /*
                    else
                    {
                        _helpFileUrl = null;

                        if (!String.IsNullOrEmpty(parameters[1].ToString()))
                        {
                            var param2 = (BrowserNavigationType)parameters[1];
                            //Переходы на сайт
                            if (String.IsNullOrEmpty(SystemConfiguration.SessionKey) ||
                                String.IsNullOrEmpty(SystemConfiguration.Uid) ||
                                String.IsNullOrEmpty(SystemConfiguration.SessionSecretKey))
                                AutoLogin();

                            string method = "";
                            bool isAlbum = false;
                            switch (param2)
                            {
                                case BrowserNavigationType.Messages:
                                    ViewData["Title"] = SystemConfiguration.ApplicationName; //Resources.Messages;
                                    method = "/api/messages?";
                                    break;
                                case BrowserNavigationType.Guests:
                                    ViewData["Title"] = SystemConfiguration.ApplicationName; //Resources.Guests;
                                    method = "/api/guests?";
                                    break;
                                case BrowserNavigationType.Marks:
                                    ViewData["Title"] = SystemConfiguration.ApplicationName; //Resources.Marks;
                                    method = "/api/photo_marks?";
                                    break;
                                case BrowserNavigationType.Notifications:
                                    ViewData["Title"] = SystemConfiguration.ApplicationName; //Resources.Notifications;
                                    method = "/api/notifications?";
                                    break;
                                case BrowserNavigationType.Activities:
                                    ViewData["Title"] = SystemConfiguration.ApplicationName; //Resources.Activities;
                                    method = "/api/activities?";
                                    break;
                                case BrowserNavigationType.Discussions:
                                    ViewData["Title"] = SystemConfiguration.ApplicationName; //Resources.Discussions;
                                    method = "/api/discussions?";
                                    break;
                                case BrowserNavigationType.FriendProfile:
                                    ViewData["Title"] = SystemConfiguration.ApplicationName; //Resources.WebBrowser_View_FriendProfile_Title;
                                    method = "/api/user?";
                                    break;
                                case BrowserNavigationType.SendGift:
                                    ViewData["Title"] = SystemConfiguration.ApplicationName; //Resources.WebBrowser_View_SendGift_Title;
                                    method = "/api/make_present?";
                                    break;
                                case BrowserNavigationType.WriteMessage:
                                    ViewData["Title"] = SystemConfiguration.ApplicationName; //Resources.WebBrowser_View_WriteMessage_Title;
                                    method = "/api/write_message?";
                                    break;
                                case BrowserNavigationType.UserProfile:
                                    ViewData["Title"] = SystemConfiguration.ApplicationName; //Resources.WebBrowser_View_UserProfile_Title;
                                    method = "/api/?";
                                    break;
                                case BrowserNavigationType.PhotoPrivate:
                                    ViewData["Title"] = SystemConfiguration.ApplicationName; //Resources.WebBrowser_View_Album_Title;
                                    method = "/api/photo_album?";
                                    isAlbum = false;
                                    break;
                                case BrowserNavigationType.PhotoAlbum:
                                    ViewData["Title"] = SystemConfiguration.ApplicationName; //Resources.WebBrowser_View_Album_Title;
                                    method = "/api/photo_album?";
                                    isAlbum = true;
                                    break;
                            }

                            ViewData["FirstButtonTitle"] = Resources.WebBrowser_menuItemBack_Text;
                            ViewData["BackButtonTitle"] = Resources.WebBrowser_menuItemToApp_Text;
                            view.UpdateView("ChangeTitle");

                            if (!String.IsNullOrEmpty(method))
                            {
                                var uri = new Components.Server.Uri
                                {
                                    Address = SystemConfiguration.MobileSiteUrl,
                                    Method = method
                                };
                                if (isAlbum)
                                {
                                    uri.Parameters.Add("aid=application");
                                }

                                uri.Parameters.Add("application_key=" + SystemConfiguration.ApplicationKey);
                                uri.Parameters.Add("client=wm");
                                if (param != "UserProfile")
                                    uri.Parameters.Add("ref_uid=" + param);

                                uri.Parameters.Add("session_key=" + SystemConfiguration.SessionKey);
                                uri.Parameters.Add("uid=" + SystemConfiguration.Uid);
                                ViewData["Url"] = uri.AppendSig(true, SystemConfiguration.SessionSecretKey);

                                _urlHistory.Push((string)ViewData["Url"]);
                                ViewData["BackButtonVisible"] = _urlHistory.Count > 1;
                                view.UpdateView("WebBrowserGoToUrl");
                            }
                        }
                    }
                    */
                }

                base.OnInitialize(parameters);
            }
        }

        #endregion
    }
}
