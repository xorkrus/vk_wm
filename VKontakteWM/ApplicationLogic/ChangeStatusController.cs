using System;
using System.Text;
using Galssoft.VKontakteWM.Components.Common.Localization;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.Common;
using Galssoft.VKontakteWM.Components;
using Galssoft.VKontakteWM.Properties;
using Galssoft.VKontakteWM.Components.UI.Wrappers;

namespace Galssoft.VKontakteWM.ApplicationLogic
{

    class ChangeStatusController : Controller
    {
        #region Constructors

        public ChangeStatusController()
            : base(new ChangeStatusView())
        {
            Name = "ChangeStatusController";
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
            #region SetStatus

            if (key == "SetStatus")
            {
                StringBuilder newStringBuilder = new StringBuilder();

                var status = (string)view.ViewData["CurrentStatus"];

                status = status.Replace("\r", string.Empty);
                status = status.Replace("\n", " ");
                status = status.Trim();

                //// пилим по переносам строки...
                //string[] lines = status.Split('\n');

                //foreach (string line in lines)
                //{
                //    newStringBuilder.Append(line);
                //    newStringBuilder.Append(" ");
                //}

                //status = newStringBuilder.ToString();

                //if (status.EndsWith(" "))
                //{
                //    status = status.Remove(status.Length - 1, 1);
                //}

                // ?
                ViewData["CurrentStatus"] = status;

                // обновление статуса
                try
                {
                    // обновление информации о пользователе
                    using (new WaitWrapper())
                    {
                        if (!string.IsNullOrEmpty(status))
                        {
                            Globals.BaseLogic.SetStatus(status, StatusActionType.Refresh, false);
                        }
                        else
                        {
                            Globals.BaseLogic.SetStatus(status, StatusActionType.Clear, false);
                        }
                    }

                    view.UpdateView("SetStatusSuccess");
                    view.UpdateView("HideCursor");

                    MasterForm.Navigate<ShareController>("UpdateStatus", view.ViewData["CurrentStatus"]);

                    //NavigationService.GoBack();

                    // ?
                    //MasterForm.Navigate<MainController>("Status", (string)ViewData["CurrentStatus"]);
                }
                catch (VKException ex)
                {
                    string error = ExceptionTranslation.TranslateException(ex);

                    if (!string.IsNullOrEmpty(error))
                    {
                        // показывать сообщение об ошибке вроде как не нужно
                        if (ex.LocalizedMessage == ExceptionMessage.IncorrectLoginOrPassword)
                        {
                            Globals.BaseLogic.IDataLogic.SetToken(string.Empty);
                            //MasterForm.Navigate<LoginController>();

                            view.UpdateView("GoToLogin");
                        }

                       ViewData["SetStatusError"] = error;
                       view.UpdateView("SetStatusFail");
                    }
                }
                catch (OutOfMemoryException)
                {                    
                    ViewData["SetStatusError"] = Resources.OutOfMemory;
                    view.UpdateView("SetStatusFail");
                }
            }

            #endregion

            #region Cancel

            if (key == "Cancel")
            {
                view.UpdateView("HideCursor");
                NavigationService.GoBack();
            }

            #endregion

            //OnViewStateChanged("GoGoToLogin");

            if (key == "GoGoToLogin")
            {
                MasterForm.Navigate<LoginController>();
            }
        }

        protected override void OnInitialize(params object[] parameters)
        {
            // т.к создаем новое то ничего не приходит

            //if ((parameters != null) && (parameters.Length > 0))
            //{
            //    string param = parameters[0] as string;

            //    if ((param != null))
            //    {
            //        ViewData["CurrentStatus"] = param;
            //        view.UpdateView("SetCurrentStatus");
            //    }
            //}

            base.OnInitialize(parameters);
        }

        #endregion
    }
}
