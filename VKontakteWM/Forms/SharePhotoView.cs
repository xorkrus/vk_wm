using System;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.Transitions;
using Galssoft.VKontakteWM.Components.UI.Wrappers;
using Galssoft.VKontakteWM.Properties;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;
using Galssoft.VKontakteWM.ApplicationLogic;
using Microsoft.WindowsCE.Forms;

namespace Galssoft.VKontakteWM.Forms
{
    public partial class SharePhotoView : UIViewBase, IView
    {
        public SharePhotoView()
        {
            InitializeComponent();
        }

        public void Load()
        {
            //
        }

        private void FormResize(object sender, EventArgs e)
        {
            Controller controllerSharePhotoController;

            if (!NavigationService.Controllers.TryGetValue("SharePhotoController", out controllerSharePhotoController))
            {
                return;
            }
            else
            {
                if (!NavigationService.Current().Equals(controllerSharePhotoController))
                {
                    return;
                }
            }

            btnSentMessage.Location = new Point((Width - btnSentMessage.Width) / 2, UISettings.CalcPix(5));
            //btnSentMessageText.Location = btnSentMessage.Location;
            //btnSentMessageShadowText.Location = new Point(btnSentMessage.Location.X, btnSentMessage.Location.Y - UISettings.CalcPix(1));

            //upcUploadPhotoMobile.Left = UISettings.CalcPix(5);
            //upcUploadPhotoMain.Left = UISettings.CalcPix(5) + this.ClientRectangle.Width / 2;
            //upcUploadPhotoMain.Width = upcUploadPhotoMobile.Width = this.ClientRectangle.Width / 2 - UISettings.CalcPix(10);

            //if (this.ClientRectangle.Width > this.ClientRectangle.Height)
            //{
            //    cmcPhotoDescription.Visible = false;

            //    lblVoidPhoto.Height = upcUploadPhotoMobile.Top - UISettings.CalcPix(10) - UISettings.CalcPix(45);
            //}
            //else
            //{
            //    cmcPhotoDescription.Visible = true;

            //    lblVoidPhoto.Height = cmcPhotoDescription.Top - UISettings.CalcPix(10) - UISettings.CalcPix(45);
            //}

            //btnRotareImageClockwise.Top = btnRotareImageCounterclockwise.Top = lblVoidPhoto.Height / 2 + UISettings.CalcPix(45) - UISettings.CalcPix(32) / 2;

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
            get { return mainMenu; }
            //get { return null; }
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

            //(new InputPanel()).Enabled = false;
        }

        public void OnActivate()
        {
            //
        }

        public void OnAfterActivate()
        {
            //(new InputPanel()).Enabled = true;

            //(new InputPanel()).Enabled = false;

            //toolBar.SelectButton(toolBar.ToolbarButtonPhotos);
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
            if (key == "MainResponse")
            {
                // выводим сообщение
                string response = (string)ViewData["ResponseMessage"];

                ViewData["ResponseMessage"] = string.Empty;

                //bool isNavigate = false;

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

                    DialogControl.ShowQuery(response, DialogButtons.OK);

                    if (response.Equals(Resources.UploadPhoto_Controller_ResponseMessage_ImageSuccessfullyDownloaded))
                    {
                        //NavigationService.GoBack();

                        OnViewStateChanged("GoBack");

                        //MasterForm.Navigate<ShareController>("AllowReloadData", "false");
                    }                    

                    ViewData["ResponseMessage"] = string.Empty;
                }

                // восстанавливаем дефолтную позицию
                int displayAreaWidth = (int)ViewData["DisplayAreaWidth"];
                int displayAreaHeight = (int)ViewData["DisplayAreaHeight"];
                int displayAreaLeft = (int)ViewData["DisplayAreaLeft"];
                int displayAreaTop = (int)ViewData["DisplayAreaTop"];

                //giPhotoBedding.Width = displayAreaWidth;
                //giPhotoBedding.Height = displayAreaHeight;
                //giPhotoBedding.Left = displayAreaLeft;
                //giPhotoBedding.Top = displayAreaTop;

                //giPhotoBedding.AlphaChannelImage = (IImage)ViewData["BaseImage"];
                //giPhotoBedding.OnInvalidate();

                // устанавливаем новую позицию
                int newDisplayAreaWidth = 0;
                int newDisplayAreaHeight = 0;

                newDisplayAreaWidth = (int)ViewData["ImageWidth"];
                newDisplayAreaHeight = (int)ViewData["ImageHeight"];

                //if (Globals.BaseLogic.IDataLogic.GetUplPhtViewHasMdfPht())
                //{
                //    lblVoidPhoto.Visible = false;

                //    newDisplayAreaWidth = (int)ViewData["ImageWidth"];
                //    newDisplayAreaHeight = (int)ViewData["ImageHeight"];
                //}
                //else
                //{
                //    lblVoidPhoto.Visible = true;

                //    newDisplayAreaWidth = (int)ViewData["DummyImageWidth"];
                //    newDisplayAreaHeight = (int)ViewData["DummyImageHeight"];
                //}

                giPhotoPreview.Top = displayAreaTop + (displayAreaHeight - newDisplayAreaHeight) / 2;
                giPhotoPreview.Height = newDisplayAreaHeight;

                giPhotoPreview.Left = displayAreaLeft + (displayAreaWidth - newDisplayAreaWidth) / 2;
                giPhotoPreview.Width = newDisplayAreaWidth;

                if (ViewData["Image"] != null)
                {
                    giPhotoPreview.AlphaChannelImage = (IImage)ViewData["Image"];
                    giPhotoPreview.OnInvalidate();
                }

                ////выводим изображение
                //if (Globals.BaseLogic.IDataLogic.GetUplPhtViewHasMdfPht())
                //{
                //    giPhotoPreview.AlphaChannelImage = (IImage)ViewData["Image"];
                //    giPhotoPreview.OnInvalidate();
                //}
                //else
                //{
                //    giPhotoPreview.AlphaChannelImage = (IImage)ViewData["DummyImage"];
                //    giPhotoPreview.OnInvalidate();
                //}

                //cmcPhotoDescription.Text = Globals.BaseLogic.IDataLogic.GetUplPhtViewPhtCmnt();
                //cmcPhotoDescription.OnInvalidate();

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

            if (key == "GoToLogin")
            {
                OnViewStateChanged("GoGoToLogin");
            }
        }

        #region меню

        private void menuItemSend_Click(object sender, System.EventArgs e)
        {
            //ViewData["PhotoComment"] = filter.FilterText;

            OnViewStateChanged("UploadPhoto");
        }

        private void btnSentMessage_Click(object sender, System.EventArgs e)
        {
            OnViewStateChanged("UploadPhoto");
        }

        private void menuItemRCC_Click(object sender, System.EventArgs e)
        {
            OnViewStateChanged("RotareImageCounterclockwise");
        }

        private void menuItemRC_Click(object sender, System.EventArgs e)
        {
            OnViewStateChanged("RotareImageClockwise");            
        }

        private void menuItemCancel_Click(object sender, System.EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoBack");
            }
        }

        #endregion
    }
}
