using System;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.Configuration;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.Transitions;
using Galssoft.VKontakteWM.Components.UI.Wrappers;
using Galssoft.VKontakteWM.Properties;
using Microsoft.WindowsCE.Forms;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Common;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Components.UI.Controls;

namespace Galssoft.VKontakteWM.Forms
{
    public partial class UploadPhotoView : UIViewBase, IView
    {
        public UploadPhotoView()
        {
            InitializeComponent();
        }

        public void Load()
        {
            //
        }

        private void FormResize(object sender, EventArgs e)
        {            
            Controller controllerUploadPhotoController;

            if (!NavigationService.Controllers.TryGetValue("UploadPhotoController", out controllerUploadPhotoController))
            {
                return;
            }
            else
            {
                if (!NavigationService.Current().Equals(controllerUploadPhotoController))
                {
                    return;
                }
            }

            upcUploadPhotoMobile.Left = UISettings.CalcPix(5);
            upcUploadPhotoMain.Left = UISettings.CalcPix(5) + this.ClientRectangle.Width / 2;
            upcUploadPhotoMain.Width = upcUploadPhotoMobile.Width = this.ClientRectangle.Width / 2 - UISettings.CalcPix(10);

            if (this.ClientRectangle.Width > this.ClientRectangle.Height)
            {
                cmcPhotoDescription.Visible = false;

                lblVoidPhoto.Height = upcUploadPhotoMobile.Top - UISettings.CalcPix(10) - UISettings.CalcPix(45);
            }
            else
            {
                cmcPhotoDescription.Visible = true;

                lblVoidPhoto.Height = cmcPhotoDescription.Top - UISettings.CalcPix(10) - UISettings.CalcPix(45);
            }

            btnRotareImageClockwise.Top = btnRotareImageCounterclockwise.Top = lblVoidPhoto.Height / 2 + UISettings.CalcPix(45) - UISettings.CalcPix(32) / 2;

            ViewData["DisplayAreaWidth"] = lblVoidPhoto.Width;
            ViewData["DisplayAreaHeight"] = lblVoidPhoto.Height;
            ViewData["DisplayAreaLeft"] = lblVoidPhoto.Left;
            ViewData["DisplayAreaTop"] = lblVoidPhoto.Top;

            OnViewStateChanged("ActivateForm");
        }

        #region IView Members

        public string Title
        {
            get { return Resources.UploadPhoto_View_Title; }
        }

        public MainMenu ViewMenu
        {
            //get { return mainMenu; }
            get { return null; }
        }

        public void OnBeforeActivate()
        {
            //RenewMessageImage();

            ViewData["DisplayAreaWidth"] = lblVoidPhoto.Width;
            ViewData["DisplayAreaHeight"] = lblVoidPhoto.Height;
            ViewData["DisplayAreaLeft"] = lblVoidPhoto.Left;
            ViewData["DisplayAreaTop"] = lblVoidPhoto.Top;

            OnViewStateChanged("ActivateForm");
        }

        public void OnAfterDeactivate()
        {
            OnViewStateChanged("DeactivateForm");
        }

        public void OnActivate()
        {
            
        }

        public void OnAfterActivate()
        {           
            (new InputPanel()).Enabled = false;

            toolBar.SelectButton(toolBar.ToolbarButtonPhotos);
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
            if (key == "MainResponse")
            {
                //выводим сообщение
                string response = (string)ViewData["ResponseMessage"];

                bool isNavigate = false;

                if (!string.IsNullOrEmpty(response))
                {
                    //if (response.Equals(Resources.UploadPhoto_Controller_ResponseMessage_ImageSuccessfullyDownloaded))
                    //{
                    //    DialogResult dlgRes = UploadPhotoDialogControl.ShowQuery(response);

                    //    if (dlgRes == DialogResult.No)
                    //    {
                    //        isNavigate = true;
                    //    }
                    //}
                    //else
                    //{
                    //    DialogControl.ShowQuery(response, DialogButtons.OK);
                    //}

                    //DialogControl.ShowQuery(response, DialogButtons.OK);

                    ViewData["ResponseMessage"] = string.Empty;
                }

                //восстанавливаем дефолтную позицию
                int displayAreaWidth = (int)ViewData["DisplayAreaWidth"];
                int displayAreaHeight = (int)ViewData["DisplayAreaHeight"];
                int displayAreaLeft = (int)ViewData["DisplayAreaLeft"];
                int displayAreaTop = (int)ViewData["DisplayAreaTop"];

                giPhotoBedding.Width = displayAreaWidth;
                giPhotoBedding.Height = displayAreaHeight;
                giPhotoBedding.Left = displayAreaLeft;
                giPhotoBedding.Top = displayAreaTop;

                giPhotoBedding.AlphaChannelImage = (IImage)ViewData["BaseImage"];
                giPhotoBedding.OnInvalidate();

                //устанавливаем новую позицию
                int newDisplayAreaWidth = 0;
                int newDisplayAreaHeight = 0;

                if (Globals.BaseLogic.IDataLogic.GetUplPhtViewHasMdfPht())
                {
                    lblVoidPhoto.Visible = false;

                    newDisplayAreaWidth = (int)ViewData["ImageWidth"];
                    newDisplayAreaHeight = (int)ViewData["ImageHeight"];
                }
                else
                {
                    lblVoidPhoto.Visible = true;

                    newDisplayAreaWidth = (int)ViewData["DummyImageWidth"];
                    newDisplayAreaHeight = (int)ViewData["DummyImageHeight"];
                }

                giPhotoPreview.Top = displayAreaTop + (displayAreaHeight - newDisplayAreaHeight) / 2;
                giPhotoPreview.Height = newDisplayAreaHeight;

                giPhotoPreview.Left = displayAreaLeft + (displayAreaWidth - newDisplayAreaWidth) / 2;
                giPhotoPreview.Width = newDisplayAreaWidth;

                //выводим изображение
                if (Globals.BaseLogic.IDataLogic.GetUplPhtViewHasMdfPht())
                {
                    giPhotoPreview.AlphaChannelImage = (IImage)ViewData["Image"];
                    giPhotoPreview.OnInvalidate();
                }
                else
                {
                    giPhotoPreview.AlphaChannelImage = (IImage)ViewData["DummyImage"];
                    giPhotoPreview.OnInvalidate();
                }

                cmcPhotoDescription.Text = Globals.BaseLogic.IDataLogic.GetUplPhtViewPhtCmnt();
                cmcPhotoDescription.OnInvalidate();

                //if (response.Equals(Resources.UploadPhoto_Controller_ResponseMessage_ImageSuccessfullyDownloaded))
                //{
                //    if (isNavigate)
                //    {
                //        if ((string)ViewData["IsMobileAlbum"] == "1")
                //        {
                //            MasterForm.Navigate<WebBrowserController>("", BrowserNavigationType.PhotoAlbum);
                //        }
                //        else
                //        {
                //            MasterForm.Navigate<WebBrowserController>("", BrowserNavigationType.PhotoPrivate);
                //        }
                //    }
                //}

                Refresh();
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

        #region кнопочки

        //с камеры
        private void btnSnapPhotoWithinCamera_Click(object sender, System.EventArgs e)
        {
            OnViewStateChanged("SnapPhotoWithinCamera");
        }

        //с диска
        private void btnLoadPhotoFromDisk_Click(object sender, System.EventArgs e)
        {
            OnViewStateChanged("LoadPhotoFromDisk");
        }

        //
        void upcUploadPhotoMain_Click(object sender, EventArgs e)
        {
            if (Globals.BaseLogic.IDataLogic.GetUplPhtViewHasMdfPht())
            {
                OnViewStateChanged("UploadPhotoMain");
            }
            else
            {
                DialogControl.ShowQuery(Resources.UploadPhoto_Controller_UploadPhotoResponseMessage_ImageIsNotPicked, DialogButtons.OK);                
            }
        }

        void upcUploadPhotoMobile_Click(object sender, EventArgs e)
        {
            if (Globals.BaseLogic.IDataLogic.GetUplPhtViewHasMdfPht())
            {
                OnViewStateChanged("UploadPhotoMobile");
            }
            else
            {
                DialogControl.ShowQuery(Resources.UploadPhoto_Controller_UploadPhotoResponseMessage_ImageIsNotPicked, DialogButtons.OK);                
            }
        }

        //по часовой
        private void btnRotareImageClockwise_Click(object sender, EventArgs e)
        {
            if (Globals.BaseLogic.IDataLogic.GetUplPhtViewHasMdfPht())
            {
                OnViewStateChanged("RotareImageClockwise");
            }
            else
            {
                DialogControl.ShowQuery(Resources.UploadPhoto_Controller_UploadPhotoResponseMessage_ImageIsNotPicked, DialogButtons.OK);                
            }
        }

        //против часовой
        private void btnRotareImageCounterclockwise_Click(object sender, EventArgs e)
        {
            if (Globals.BaseLogic.IDataLogic.GetUplPhtViewHasMdfPht())
            {
                OnViewStateChanged("RotareImageCounterclockwise");
            }
            else
            {
                DialogControl.ShowQuery(Resources.UploadPhoto_Controller_UploadPhotoResponseMessage_ImageIsNotPicked, DialogButtons.OK);                
            }
        }

        //изменить комментарий
        private void cmcPhotoDescription_Click(object sender, EventArgs e)
        {
            if (Globals.BaseLogic.IDataLogic.GetUplPhtViewHasMdfPht())
            {
                OnViewStateChanged("ChangePhotoComment");
            }
            else
            {
                DialogControl.ShowQuery(Resources.UploadPhoto_Controller_UploadPhotoResponseMessage_ImageIsNotPicked, DialogButtons.OK);                
            }
        }

        #region меню

        void ToolbarButtonNewsClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToNews");
            }
        }

        void ToolbarButtonMessagesClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToMessages");
            }
        }

        void ToolbarButtonFriendsClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToFriends");
            }
        }

        void ToolbarButtonExtrasClick(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToExtras");
            }
        }

        #endregion

        #endregion
    }
}
