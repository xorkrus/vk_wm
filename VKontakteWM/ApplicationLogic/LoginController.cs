using System;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Common;
using Galssoft.VKontakteWM.Components.Common.Localization;
using Galssoft.VKontakteWM.Components.Configuration;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.SystemHelpers;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.Properties;
using Galssoft.VKontakteWM.Components.UI.Wrappers;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    class LoginController : Controller
    {
        #region Constructors

        public LoginController() : base(new LoginView())
        {
            Name = "LoginController";
        }

        #endregion

        public override void Activate()
        {            
            view.Activate();
        }

        public override void Deactivate()
        {
            view.Deactivate();
        }

        protected override void OnViewStateChanged(string key)
        {
            if (key == "DoLogin")
            {
                string login = (string)view.ViewData["Login"];
                string password = (string)view.ViewData["Password"];
                
                try
                {
                    // авторизация по логину/паролю
                    using (new WaitWrapper())
                    {
                        Globals.BaseLogic.AuthLogin(login, password, true);
                    }

                    try
                    {
                        Globals.BaseLogic.IDataLogic.SetSavedLogin(login);
                    }
                    catch (Exception ex)
                    {
                        SystemConfiguration.Log.Write(LogEntryType.Error, "DataLogic.SetSavedLogin failed: " + ex.Message);
                    }

                    MasterForm.Navigate<StatusUpdatesListController>("LoginSuccess", "1");
                }
                catch (VKException ex)
                {
                    string message = ExceptionTranslation.TranslateException(ex);
                    
                    if (!string.IsNullOrEmpty(message))
                    {
                        ViewData["LoginError"] = message;

                        view.UpdateView("LoginFail");
                    }
                }
                catch (OutOfMemoryException)
                {                    
                    ViewData["LoginError"] = Resources.OutOfMemory;

                    view.UpdateView("LoginFail");
                }
            }

            if (key == "CancelLogin")
            {
                Application.Exit();
            }
        }

        protected override void OnInitialize(params object[] parameters)
        {
            if ((parameters != null) && (parameters.Length > 0))
            {
                ViewData["IncorrectLoginOrPassword"] = false;
                ViewData["UnknownError"] = false;
                ViewData["NoSavedToken"] = false;

                if (parameters.Length == 1 && (string)parameters[0] == "IncorrectLoginOrPassword")
                {
                    ViewData["IncorrectLoginOrPassword"] = true;
                }

                if (parameters.Length == 1 && (string)parameters[0] == "UnknownError")
                {
                    ViewData["UnknownError"] = true;
                }

                if (parameters.Length == 1 && (string)parameters[0] == "NoSavedToken")
                {
                    ViewData["NoSavedToken"] = true;
                }
            }

            base.OnInitialize(parameters);
        }
    }
}
