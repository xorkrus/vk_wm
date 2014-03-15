using System;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;
using Galssoft.VKontakteWM.Components.UI.Transitions;
using Galssoft.VKontakteWM.Components.UI.Wrappers;
using Microsoft.WindowsCE.Forms;
using Galssoft.VKontakteWM.Properties;
using System.Text;

namespace Galssoft.VKontakteWM.Forms
{
    public partial class ChangeCommentView : UIViewBase, IView
    {
        public ChangeCommentView()
        {
            InitializeComponent();
        }

        public void Load()
        {
            //
        }

        #region IView Members

        public string Title
        {
            get { return Resources.ChangeCommentView_Title; }
        }

        public MainMenu ViewMenu
        {
            get { return mainMenu; }
        }

        public void OnBeforeActivate()
        {
            tbxCommentText.Visible = true;
        }

        public void OnAfterDeactivate()
        {
            //
        }

        public void OnAfterActivate()
        {
            (new InputPanel()).Enabled = true;
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
            if (key == "SetCurrentComment")
            {
                tbxCommentText.Text = (string)ViewData["CurrentComment"];

                tbxCommentText.Refresh();
            }
        }

        #region кнопочки

        void ButtonClearTextClick(object sender, EventArgs e)
        {
            tbxCommentText.Text = string.Empty;
        }

        void MenuItemCancelClick(object sender, EventArgs e)
        {
            OnViewStateChanged("Cancel");
        }

        void MenuItemChangeClick(object sender, EventArgs e)
        {
            tbxCommentText.Visible = false;
            ViewData["CurrentComment"] = tbxCommentText.Text;

            using (new WaitWrapper(false))
            {
                OnViewStateChanged("SetComment");
            }

            tbxCommentText.Visible = true;
        }

        #endregion
    }
}
