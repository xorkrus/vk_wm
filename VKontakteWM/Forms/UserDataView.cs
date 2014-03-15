using System;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;
using Galssoft.VKontakteWM.Components.UI.Transitions;
using Galssoft.VKontakteWM.Properties;
using Microsoft.WindowsCE.Forms;

namespace Galssoft.VKontakteWM.Forms
{
    public partial class UserDataView : UIViewBase, IView
    {
        public UserDataView()
        {
            InitializeComponent();
        }

        public void Load()
        {
            OnViewStateChanged("LoadUserData");
            userDataKineticListView.Reload();
        }

        #region IView Members

        public string Title
        {
            get { return Resources.UserDataView_Text; }
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
            userDataKineticListView.SelectedIndex = -1;
        }

        public void OnAfterActivate()
        {
            AutoScroll = true;
            Invalidate();
            //Скрытие клавиатуры
            (new InputPanel()).Enabled = false;

            OnViewStateChanged("LoadUserData");
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
            if (key == "UserDataLoaded")
            {
                if (!String.IsNullOrEmpty((string)ViewData["UserLogin"]))
                {
                    headerText.Text = (string)ViewData["UserLogin"];
                    headerShadowText.Text = (string)ViewData["UserLogin"];
                }
            }
            if (key == "DeselectButton")
            {
                userDataKineticListView.SelectedIndex = -1;
                userDataKineticListView.Clear();
                userDataKineticListView.Reload();
            }
        }

        private void buttonClearPass_Click(object sender, EventArgs e)
        {
            OnViewStateChanged("ClearPass");
        }

        private void buttonClearCache_Click(object sender, EventArgs e)
        {
            OnViewStateChanged("ClearCache");
        }

        private void menuItemBack_Click(object sender, EventArgs e)
        {
            OnViewStateChanged("GoBack");
        }

    }
}
