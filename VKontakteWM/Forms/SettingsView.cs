using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;
using Galssoft.VKontakteWM.Components.UI.Transitions;
using Galssoft.VKontakteWM.Properties;
using Microsoft.WindowsCE.Forms;
using Galssoft.VKontakteWM.Components.UI.Wrappers;

namespace Galssoft.VKontakteWM.Forms
{
    public partial class SettingsView : UIViewBase, IView
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        public void Load()
        {
            OnViewStateChanged("LoadSettings");
            OnViewStateChanged("RefreshSettings");
        }

        #region IView Members

        public string Title
        {
            get { return Resources.SettingsView_Text; }
        }

        public MainMenu ViewMenu
        {
            get { return mainMenu; }
        }

        public void OnBeforeActivate()
        {

        }

        public void OnAfterDeactivate()
        {

        }

        public void OnAfterActivate()
        {
            AutoScroll = true;
            Invalidate();

            //Скрытие клавиатуры
            (new InputPanel()).Enabled = false;

            using (new WaitWrapper(false))
            {
                OnViewStateChanged("RefreshSettings");
            }
        }

        public Bitmap CreateScreenShot()
        {
            return null;
        }

        public TransitionType GetTransition(IView from)
        {
            return TransitionType.Basic;
        }

        #endregion

        protected override void OnUpdateView(string key)
        {
            if (key == "SettingsLoaded")
            {
                settingsKineticControl.DataSource = (List<SettingsListViewItems>)ViewData["Settings"];
                settingsKineticControl.Reload();
            }
        }

        private void MenuItemBackClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoBack");
            }
        }

        private void MenuItemSaveClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                ViewData["Settings"] = settingsKineticControl.DataSource;
                OnViewStateChanged("ApplyAndSaveSettings");
            }
        }

        void SettingsViewResize(object sender, EventArgs e)
        {
            headerText.Size = new Size(header.Width, header.Height);
            headerShadowText.Size = new Size(header.Width, header.Height);

            headerShadowText.Location = new Point(headerText.Location.X,
                headerText.Location.Y - UISettings.CalcPix(1));
        }
    }
}