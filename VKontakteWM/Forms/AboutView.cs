using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.Configuration;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.Transitions;
using Galssoft.VKontakteWM.Properties;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Components.UI.Controls;

namespace Galssoft.VKontakteWM.Forms
{
    public partial class AboutView : UIViewBase, IView
    {
        public AboutView()
        {
            InitializeComponent();
        }

        public void Load()
        {
            ViewData["AboutViewThis"] = this;

            OnViewStateChanged("ShowVersion");
        }

        private void FormResize(object sender, EventArgs e)
        {
            if (this.Width < this.Height)
            {
                giLogo.Location = new Point((Width - giLogo.Width) / 2, UISettings.CalcPix(45));

                lblVersion.Top = UISettings.CalcPix(125);     
                lblVersionShadow.Top = UISettings.CalcPix(125 + 1); 
                lblButtonSubstitue.Top = UISettings.CalcPix(145);                

                giGlassoftLogo.Location = new Point((Width - giGlassoftLogo.Width) / 2, Height - UISettings.CalcPix(35));
            }
            else
            {
                giLogo.Location = new Point((Width - giLogo.Width) / 2, UISettings.CalcPix(25));

                lblVersion.Top = UISettings.CalcPix(90);
                lblVersionShadow.Top = UISettings.CalcPix(90 + 1);
                lblButtonSubstitue.Top = UISettings.CalcPix(100);     

                giGlassoftLogo.Location = new Point((Width - giGlassoftLogo.Width) / 2, Height - UISettings.CalcPix(35));
            }

            var bmp = new Bitmap(Width, Height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                IntPtr hdc = g.GetHdc();

                using (Gdi loginGdi = Gdi.FromHdc(hdc, new Rectangle(0, 0, Width, Height)))
                {
                    loginGdi.GradientFill(ClientRectangle, Color.FromArgb(21, 70, 114), Color.FromArgb(78, 121, 161), FillDirection.TopToBottom);
                }

                g.ReleaseHdc(hdc);
            }

            BackgroundImage = bmp;            
        }

        #region IView Members

        public string Title
        {
            get { return Resources.AboutView_Text; }
        }

        public MainMenu ViewMenu
        {
            get { return mainMenu; }
        }

        public void OnBeforeActivate()
        {
            //
        }

        public void OnAfterDeactivate()
        {
            //
        }

        public void OnAfterActivate()
        {
            //
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        #endregion

        protected override void OnUpdateView(string key)
        {
            if (key == "ShowVersionResponse")
            {
                string currentVersion = (string)ViewData["CurrentVersion"];

                if (!string.IsNullOrEmpty(currentVersion))
                {
                    lblVersion.Text = lblVersionShadow.Text = currentVersion;                    
                }
            }

            if (key == "CheckUpdateResponse")
            {
                string updateMessage = (string)ViewData["UpdateMessage"];

                if (string.IsNullOrEmpty(updateMessage))
                {
                    string isNewVersionAvailable = (string)ViewData["IsNewVersionAvailable"];

                    try
                    {
                        if (Convert.ToBoolean(isNewVersionAvailable))
                        {
                            string newVersion = (string)ViewData["NewVersion"];
                            string newVersionInfo = (string)ViewData["NewVersionInfo"];
                            string newVersionURL = (string)ViewData["NewVersionURL"];

                            if (!string.IsNullOrEmpty(newVersion))
                            {
                                if (!string.IsNullOrEmpty(newVersionURL))
                                {
                                    menuItemActions.Click += new EventHandler(menuItemActions_Update_Click);

                                    menuItemActions.Text = Resources.AboutView_MenuItem_Update;

                                    lblButtonSubstitue.Text = Resources.ApplicationUpdate_Message_MayInstall + newVersion;
                                    lblButtonSubstitue.Visible = true;

                                    //buttonCheckOrUploadUpdate.Click += new EventHandler(buttonUploadUpdate_Click);

                                    //buttonCheckOrUploadUpdate.Text = Resources.AboutView_UploadUpdateButton + newVersion;

                                    //buttonCheckOrUploadUpdate.OnInvalidate();

                                    if (!string.IsNullOrEmpty(newVersionInfo))
                                    {
                                        //lblVersionDescription.Text = newVersionInfo;

                                        //lblVersionDescription.OnInvalidate();
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                    catch (Exception)
                    {
                        lblButtonSubstitue.Text = Resources.ApplicationUpdate_Message_AlreadyInstall;
                        lblButtonSubstitue.Visible = true;

                        //buttonCheckOrUploadUpdate.Visible = false;
                        //lblButtonSubstitue.Visible = true;
                    }
                }
                else
                {
                    //lblButtonSubstitue.Text = Resources.ApplicationUpdate_Message_Undefined;
                    //lblButtonSubstitue.Visible = true;

                    DialogControl.ShowQuery((string)ViewData["UpdateMessage"], DialogButtons.OK);
                }
            }
        }

        private void MenuItemBackClick(object sender, EventArgs e)
        {
            OnViewStateChanged("Back");
        }

        private void menuItemActions_Check_Click(object sender, EventArgs e)
        {
            OnViewStateChanged("CheckUpdate");
        }

        private void menuItemActions_Update_Click(object sender, EventArgs e)
        {
            OnViewStateChanged("UploadUpdate");
        }

        void GlassoftLogo_Click(object sender, EventArgs e)
        {
            if (DialogControl.ShowQuery(Resources.AboutView_Button_Support, DialogButtons.YesNo) == DialogResult.Yes)
            {
                Process.Start(SystemConfiguration.SupportSiteURL, null);
            }
        }
    }
}
