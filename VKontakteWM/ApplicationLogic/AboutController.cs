using System;

using System.Collections.Generic;
using System.Text;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.Server;
using Galssoft.VKontakteWM.Properties;

using Galssoft.VKontakteWM.Components.Configuration;
using Galssoft.VKontakteWM.Common;
using Galssoft.VKontakteWM.Components.SystemHelpers;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.Wrappers;
using System.Diagnostics;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    class AboutController : Controller
    {
        #region Constructors

        public AboutController()
            : base(new AboutView())
        {
            Name = "AboutController";
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

        protected override void OnViewStateChanged(string key)
        {
            if (key == "CheckUpdate")
            {
                ViewData["UpdateMessage"] = string.Empty;
                ViewData["IsNewVersionAvailable"] = "false";

                var updateHelper = new UpdateHelper((UIViewBase)ViewData["AboutViewThis"]);

                using (new WaitWrapper())
                {
                    updateHelper.CheckInternal(true);
                }

                if (!updateHelper.IsUnableToDefineLatestVersion)
                {
                    if (updateHelper.IsNewVersionAvailable)
                    {
                        ViewData["IsNewVersionAvailable"] = "true";

                        ViewData["NewVersion"] = Globals.BaseLogic.IDataLogic.GetLastCheckedVersionStr();
                        ViewData["NewVersionInfo"] = Globals.BaseLogic.IDataLogic.GetLastCheckedVersionInfo();
                        ViewData["NewVersionURL"] = Globals.BaseLogic.IDataLogic.GetLastCheckedVersionUpdateURL();
                    }
                }
                else
                {
                    ViewData["UpdateMessage"] = Resources.ApplicationUpdate_Message_Undefined;
                }

                view.UpdateView("CheckUpdateResponse");
            }

            if (key == "UploadUpdate")
            {
                string lastCheckedVersionUpdateURL = Globals.BaseLogic.IDataLogic.GetLastCheckedVersionUpdateURL();

                if (!string.IsNullOrEmpty(lastCheckedVersionUpdateURL))
                {
                    Process.Start(lastCheckedVersionUpdateURL, null);
                }
            }

            if (key == "Back")
            {
                using (new WaitWrapper(false))
                {
                    NavigationService.GoBack();
                }
            }

            if (key == "ShowVersion")
            {
                ViewData["CurrentVersion"] = SystemConfiguration.CurrentProductVersion.ToString(3);

                view.UpdateView("ShowVersionResponse");
            }
        }

        #endregion
    }
}
