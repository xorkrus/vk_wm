using System;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.Configuration;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.Transitions;
using Galssoft.VKontakteWM.Properties;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Components.GDI;
using Microsoft.WindowsCE.Forms;

namespace Galssoft.VKontakteWM.Forms
{
    public partial class LoginView : UIViewBase, IView
    {
        public LoginView()
        {
            InitializeComponent();
        }

        public void Load()
        {
            //
        }

        private void FormResize(object sender, EventArgs e)
        {
            loginLogo.Location = new Point((Width - loginLogo.Width) / 2, UISettings.CalcPix(23));

            var bmp = new Bitmap(Width, Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                IntPtr hdc = g.GetHdc();
                using (Gdi loginGdi = Gdi.FromHdc(hdc, new Rectangle(0, 0, Width, Height)))
                {
                    loginGdi.GradientFill(ClientRectangle, Color.FromArgb(21, 70, 114), Color.FromArgb(78, 121, 161),
                                      FillDirection.TopToBottom);
                }
                g.ReleaseHdc(hdc);
            }
            BackgroundImage = bmp;
            //bmp.Dispose();
        }

        #region IView Members

        public string Title
        {
            get { return Resources.LoginView_Text; }
        }

        public MainMenu ViewMenu
        {
            get { return mainMenu; }
        }

        public void OnBeforeActivate()
        {
            CoreHelper.SetSipButton(true);
        }

        public void OnAfterDeactivate()
        {
            (new InputPanel()).Enabled = false;
        }

        public void OnAfterActivate()
        {
            (new InputPanel()).Enabled = true;

            textBoxLogin.TextColor = Color.DarkGray;
            textBoxLogin.Text = Resources.LoginView_Caption_Login;
            textBoxLogin.OnInvalidate();

            textBoxPass.TextColor = Color.DarkGray;
            textBoxPass.Text = Resources.LoginView_Caption_Password;
            textBoxPass.OnInvalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (ViewData["IncorrectLoginOrPassword"] != null && (bool)ViewData["IncorrectLoginOrPassword"])
            {
                ViewData["IncorrectLoginOrPassword"] = false;

                DialogControl.ShowQuery(Resources.IncorrectLoginOrPassword, DialogButtons.OK);                
            }
            else if (ViewData["UnknownError"] != null && (bool)ViewData["UnknownError"])
            {
                ViewData["UnknownError"] = false;

                DialogControl.ShowQuery(Resources.VK_ERRORS_UnknownError, DialogButtons.OK);
                
            }
            else if (ViewData["NoSavedToken"] != null && (bool)ViewData["NoSavedToken"])
            {
                ViewData["NoSavedToken"] = false;

                DialogControl.ShowQuery(Resources.VK_ERRORS_NoSavedToken, DialogButtons.OK);                
            }
        }

        public Bitmap CreateScreenShot()
        {
            var bitmap = new Bitmap(Width, Height);
            var rect = new Rectangle(0, 0, Width, Height);

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
            if (key == "LoginFail")
            {
                var loginError = (string)ViewData["LoginError"];

                DialogControl.ShowQuery(loginError, DialogButtons.OK);
            }
        }

        #region кнопочки

        private void BtnEnterClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxLogin.Text)
                || textBoxLogin.Text.Equals(Resources.LoginView_Caption_Login)
                || string.IsNullOrEmpty(textBoxPass.Text)
                || textBoxPass.Text.Equals(Resources.LoginView_Caption_Password))
            {
                DialogControl.ShowQuery(Resources.LoginView_LoginOrPassIsEmpty, DialogButtons.OK);
            }
            else
            {
                ViewData["Login"] = textBoxLogin.Text;
                ViewData["Password"] = textBoxPass.Text;

                // запомнить пароль?
                SystemConfiguration.DeleteOnExit = btnRemember.Name != "btnRememberChecked";

                OnViewStateChanged("DoLogin");
            }
        }

        private void ButtonCancelClick(object sender, EventArgs e)
        {
            OnViewStateChanged("CancelLogin");
        }

        private void LoginViewClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxLogin.Text))
            {
                textBoxLogin.TextColor = Color.DarkGray;
                textBoxLogin.Text = Resources.LoginView_Caption_Login;
                textBoxLogin.OnInvalidate();
            }

            if (string.IsNullOrEmpty(textBoxPass.Text))
            {
                textBoxPass.TextColor = Color.DarkGray;
                textBoxPass.Text = Resources.LoginView_Caption_Password;
                textBoxPass.OnInvalidate();
            }
        }

        private void TextBoxLoginMouseUp(object sender, MouseEventArgs e)
        {
            (new InputPanel()).Enabled = true;

            if (textBoxLogin.Text.Equals(Resources.LoginView_Caption_Login))
            {
                textBoxLogin.TextColor = Color.Black;
                textBoxLogin.Text = string.Empty;
                textBoxLogin.OnInvalidate();
            }

            if (string.IsNullOrEmpty(textBoxPass.Text))
            {
                textBoxPass.TextColor = Color.DarkGray;
                textBoxPass.Text = Resources.LoginView_Caption_Password;
                textBoxPass.OnInvalidate();
            }
        }

        private void TextBoxPassMouseUp(object sender, MouseEventArgs e)
        {
            (new InputPanel()).Enabled = true;

            if (textBoxPass.Text.Equals(Resources.LoginView_Caption_Password))
            {
                textBoxPass.TextColor = Color.Black;
                textBoxPass.Text = string.Empty;
                textBoxPass.OnInvalidate();
            }

            if (string.IsNullOrEmpty(textBoxLogin.Text))
            {
                textBoxLogin.TextColor = Color.DarkGray;
                textBoxLogin.Text = Resources.LoginView_Caption_Login;
                textBoxLogin.OnInvalidate();
            }
        }

        void BtnRememberClick(object sender, EventArgs e)
        {
            if (btnRemember.Name == "btnRememberUnChecked")
            {
                btnRemember.TransparentButton = MasterForm.SkinManager.GetImage("RememberBoxChecked");
                btnRemember.TransparentButtonPressed = MasterForm.SkinManager.GetImage("RememberBoxCheckedPressed");
                btnRemember.Name = "btnRememberChecked";
                btnRemember.OnInvalidate();
            }
            else
            {
                btnRemember.TransparentButton = MasterForm.SkinManager.GetImage("RememberBoxUnChecked");
                btnRemember.TransparentButtonPressed = MasterForm.SkinManager.GetImage("RememberBoxUnCheckedPressed");
                btnRemember.Name = "btnRememberUnChecked";
                btnRemember.OnInvalidate();
            }
        }

        #endregion
    }
}
