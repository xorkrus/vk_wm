using System.Windows.Forms;
using Galssoft.VKontakteWM.Common;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Components.UI.Wrappers;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Forms;
//using Galssoft.VKontakteWM.Notification.ServiceClasses;
using Galssoft.VKontakteWM.Properties;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    class UserDataController : Controller
    {
        #region Constructors

        public UserDataController()
            : base(new UserDataView())
        {
            Name = "UserDataController";
        }

        #endregion

        #region Controller implementations

        public override void Activate()
        {
            view.Activate();
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
            #region LoadUserData
            if (key == "LoadUserData")
            {
                string login = Globals.BaseLogic.IDataLogic.GetSavedLogin();
                ViewData["UserLogin"] = login;
                view.UpdateView("UserDataLoaded");
            }
            #endregion

            #region GoBack
            if (key == "GoBack")
            {
                NavigationService.GoBack();
            }
            #endregion

            #region ClearPass
            if (key == "ClearPass")
            {
                DialogResult dlgRes = DialogControl.ShowQuery(Resources.UserData_ClearPass, DialogButtons.YesNo);
                switch (dlgRes)
                {
                    case DialogResult.Yes:
                        using (new WaitWrapper(false))
                        {
                            Globals.BaseLogic.IDataLogic.ClearPass();
                            Globals.BaseLogic.IDataLogic.ClearCache();
                            //надо еще и нотификатор остановить
                            //Interprocess.StopService();
                            MasterForm.Navigate<LoginController>();
                        }
                        break;
                }
                view.UpdateView("DeselectButton");
            }
            #endregion

            #region ClearCache
            if (key == "ClearCache")
            {
                DialogResult dlgRes = DialogControl.ShowQuery(Resources.UserData_ClearCache, DialogButtons.YesNo);
                switch (dlgRes)
                {
                    case DialogResult.Yes:
                        using (new WaitWrapper(false))
                        {
                            Globals.BaseLogic.IDataLogic.ClearCache();
                        }
                        break;
                }
                view.UpdateView("DeselectButton");
            }
            #endregion
        }

        protected override void OnInitialize(params object[] parameters)
        {
            if ((parameters != null) && (parameters.Length > 0))
            {
                string param = parameters[0] as string;

                if ((param != null) && (param == "ClearPass"))
                {
                    OnViewStateChanged("ClearPass");
                }

                else if ((param != null) && (param == "ClearCache"))
                {
                    OnViewStateChanged("ClearCache");

                }
            }

            base.OnInitialize(parameters);
        }
        #endregion
    }
}
