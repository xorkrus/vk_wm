using System;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Components.UI.Wrappers;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.Components.MVC;
//using Galssoft.VKontakteWM.Notification.ServiceClasses;
using Galssoft.VKontakteWM.Properties;
using Galssoft.VKontakteWM.Common;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    class ExtraController : Controller
    {
        #region Constructors

        public ExtraController()
            : base(new ExtraView())
        {
            Name = "ExtraController";
        }

        #endregion

        #region Controller implementations

        public override void Activate()
        {
            view.Activate();

            using (new WaitWrapper(false))
            {
                Configuration.LoadConfigSettings();
            }
        }

        public override void Deactivate()
        {
            view.Deactivate();
        }

        /// <summary>
        /// This method indicates that something has been changed in the view.
        /// </summary>
        /// <param name="key">The string key to identify what has been changed.</param>
        protected override void OnViewStateChanged(string key)
        {
            #region StartNotificator
            if (key == "StartNotificator")
            {
                if (Globals.BaseLogic.IDataLogic.GetToken() != "")
                {
                    ////Запуск службы нотификатора
                    //try
                    //{
                    //    if (!Interprocess.IsServiceRunning) Interprocess.StartService();
                    //}
                    //catch (Exception ex)
                    //{
                    //    //Ошибка при запуске нотификатора
                    //    ViewData["NotificatorStartError"] = ex.Message;
                    //    view.UpdateView("NotificatorStartFail");
                    //}
                }
                else
                    DialogControl.ShowQuery(Resources.MainView_Button_NotificatorCantStart, DialogButtons.OK);
                //MessageBox.Show(Resources.MainView_Button_NotificatorCantStart);
            }
            #endregion

            if (key == "StopNotificator")
            {
               // Interprocess.StopService();
            }

            #region Переходы по тулбару

            #region GoToNews

            if (key == "GoToNews")
            {
                MasterForm.Navigate<StatusUpdatesListController>();
            }

            #endregion

            #region GoToMessages

            if (key == "GoToMessages")
            {
                MasterForm.Navigate<MessagesChainsListController>();
            }

            #endregion

            #region GoToFriends

            if (key == "GoToFriends")
            {
                MasterForm.Navigate<FriendsListController>();
            }

            #endregion

            #region GoToPhotos

            if (key == "GoToPhotos")
            {
                MasterForm.Navigate<ShareController>();
            }

            #endregion

            #endregion
        }

        protected override void OnInitialize(params object[] parameters)
        {
            if ((parameters != null) && (parameters.Length > 0))
            {
                string param = parameters[0] as string;

                if ((param != null) && (param == "GoToUserData"))
                {
                    using (new WaitWrapper(false))
                    {
                        MasterForm.Navigate<UserDataController>();
                    }
                }

                /*
                else if ((param != null) && (param == "GoToSettings"))
                {
                    using (new WaitWrapper(false))
                    {
                        Configuration.LoadConfigSettings();
                        MasterForm.Navigate<SettingsController>();
                    }
                }
                 */

                else if ((param != null) && (param == "GoToAbout"))
                {
                    using (new WaitWrapper(false))
                    {
                        MasterForm.Navigate<AboutController>();
                    }
                }

                else if ((param != null) && (param == "GoToHelp"))
                {
                    using (new WaitWrapper(false))
                    {
                        MasterForm.Navigate<WebBrowserController>("Help");
                    }
                }

                else if ((param != null) && (param == "GoToExit"))
                {
                    using (new WaitWrapper(false))
                    {
                        Application.Exit();
                    }
                }
            }

            base.OnInitialize(parameters);
        }

        #endregion
    }
}
