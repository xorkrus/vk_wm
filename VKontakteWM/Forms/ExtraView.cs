using System;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Common;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Components.UI.Transitions;
using Galssoft.VKontakteWM.Components.UI.Wrappers;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Properties;
using Microsoft.WindowsCE.Forms;

namespace Galssoft.VKontakteWM.Forms
{
    public partial class ExtraView : UIViewBase, IView
    {
        public ExtraView()
        {
            InitializeComponent();
        }

        public void Load()
        {
            OnViewStateChanged("ShowVersion");
            extraKineticListView.Reload();
        }

        #region IView Members

        public string Title
        {
            get { return Resources.ExtraView_Text; }
        }

        public MainMenu ViewMenu
        {
            get { return null; }
        }

        public void OnBeforeActivate()
        {
            //RenewMessageImage();
        }

        public void OnAfterDeactivate()
        {
            extraKineticListView.SelectedIndex = -1;
        }

        public void OnAfterActivate()
        {
            AutoScroll = true;
            Invalidate();
            //Скрытие клавиатуры
            (new InputPanel()).Enabled = false;

            //Выделение кнопки toolbarButtonMain
            toolBar.SelectButton(toolBar.ToolbarButtonExtras);
        }

        public void OnActivate()
        {
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
            // Что то пришло от контроллера
            if (key == "UpdateResponse")
            {
                var updateMessage = (string)ViewData["UpdateMessage"];
                DialogControl.ShowQuery(updateMessage, DialogButtons.OK);
                //MessageBox.Show(updateMessage);
            }

            //RenewMessageImage();
        }

        private void RenewMessageImage()
        {
            if (Globals.BaseLogic.IsNewMessages())
            {
                toolBar.ToolbarButtonMessages.TransparentButton = MasterForm.SkinManager.GetImage("TBButtonMessagesNew1");
                toolBar.ToolbarButtonMessages.TransparentButtonPressed = MasterForm.SkinManager.GetImage("TBButtonMessagesNew3");
                toolBar.ToolbarButtonMessages.TransparentButtonSelected = MasterForm.SkinManager.GetImage("TBButtonMessagesNew2");
            }
            else
            {
                toolBar.ToolbarButtonMessages.TransparentButton = MasterForm.SkinManager.GetImage("TBButtonMessages1");
                toolBar.ToolbarButtonMessages.TransparentButtonPressed = MasterForm.SkinManager.GetImage("TBButtonMessages3");
                toolBar.ToolbarButtonMessages.TransparentButtonSelected = MasterForm.SkinManager.GetImage("TBButtonMessages2");
            }
        }

        void ExtraViewResize(object sender, EventArgs e)
        {
            headerText.Size = new Size(header.Width, header.Height);
            headerShadowText.Size = new Size(header.Width, header.Height);

            headerShadowText.Location = new Point(headerText.Location.X - 1,
                headerText.Location.Y - 1);
        }

        #region кнопочки

        #region меню

        private void ButtonNewsClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToNews");
            }
        }

        private void ButtonMessagesClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToMessages");
            }
        }

        private void ButtonFriendsClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToFriends");
            }
        }

        private void ButtonPhotosClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToPhotos");
            }
        }

        #endregion

        #endregion
    }
}
