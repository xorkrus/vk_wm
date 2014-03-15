using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.Components.Configuration;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using System.IO;
using Galssoft.VKontakteWM.Common;
using System;
using System.Runtime.InteropServices;
using Galssoft.VKontakteWM.Properties;
using Galssoft.VKontakteWM.Components.UI.Wrappers;
using Galssoft.VKontakteWM.Components.Common.Localization;
using Galssoft.VKontakteWM.CustomControls;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    class SharePhotoController : Controller
    {
        #region Constructors

        public SharePhotoController()
            : base(new SharePhotoView())
        {
            Name = "SharePhotoController";
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
            //ViewData["ResponseMessage"] = string.Empty; // а раньше работало как-то без этого

            #region key == "ClearRegistry"

            if (key == "ClearRegistry")
            {
                Globals.BaseLogic.IDataLogic.SetUplPhtViewHasMdfPht(false);
                Globals.BaseLogic.IDataLogic.SetUplPhtViewPhtCmnt(string.Empty);
                Globals.BaseLogic.IDataLogic.SetUplPhtViewPhtRtnAnglZero();
            }

            #endregion

            #region key == "ActivateForm"

            if (key == "ActivateForm")
            {
                try
                {
                    ViewData["ResponseMessage"] = string.Empty; // а раньше работало как-то без этого

                    ViewData["ImageFile"] = SystemConfiguration.AppInstallPath + @"\Cache\Files\image.jpg";
                    ViewData["ThumbImageFile"] = SystemConfiguration.AppInstallPath + @"\Cache\Files\t_image.jpg";
                    ViewData["EmptyImageFile"] = SystemConfiguration.AppInstallPath + @"\Cache\Files\empty.jpg";

                    // "пустышка" для подмены
                    if (ViewData["EmptyImage"] == null)
                    {
                        IImage newIImage;

                        ImageHelper.SaveImageFromMemory(MasterForm.SkinManager.GetImage("EmptyImage"), (string)ViewData["EmptyImageFile"]);

                        ImageHelper.LoadImageFromFile((string)ViewData["EmptyImageFile"], out newIImage);

                        ViewData["EmptyImage"] = newIImage;
                    }

                    int displayAreaWidth = (int)view.ViewData["DisplayAreaWidth"];
                    int displayAreaHeight = (int)view.ViewData["DisplayAreaHeight"];
                                     
                    // изображение
                    if (File.Exists((string)ViewData["ImageFile"]))
                    {
                        if (ViewData["Image"] == null)
                        {
                            IImage newIImage;
                            System.Drawing.Size imageSize;

                            ImageHelper.SaveScaledImage((string)ViewData["ImageFile"], (string)ViewData["ThumbImageFile"], (int)Math.Min(displayAreaWidth, displayAreaHeight), Globals.BaseLogic.IDataLogic.GetUplPhtViewPhtRtnAngl());

                            ImageHelper.LoadImageFromFile((string)ViewData["ThumbImageFile"], out newIImage, out imageSize);

                            ViewData["Image"] = newIImage;
                            ViewData["ImageWidth"] = imageSize.Width;
                            ViewData["ImageHeight"] = imageSize.Height;
                        }
                        else
                        {
                            if (Math.Max((int)ViewData["ImageWidth"], (int)ViewData["ImageHeight"]) != Math.Min(displayAreaWidth, displayAreaHeight))
                            {
                                IImage newIImage;
                                System.Drawing.Size imageSize;

                                newIImage = (IImage)ViewData["Image"];
                                ViewData["Image"] = ViewData["EmptyImage"];
                                Marshal.FinalReleaseComObject(newIImage);

                                ImageHelper.SaveScaledImage((string)ViewData["ImageFile"], (string)ViewData["ThumbImageFile"], (int)Math.Min(displayAreaWidth, displayAreaHeight), Globals.BaseLogic.IDataLogic.GetUplPhtViewPhtRtnAngl());

                                ImageHelper.LoadImageFromFile((string)ViewData["ThumbImageFile"], out newIImage, out imageSize);

                                ViewData["Image"] = newIImage;
                                ViewData["ImageWidth"] = imageSize.Width;
                                ViewData["ImageHeight"] = imageSize.Height;
                            }
                        }
                    }
                    else
                    {
                        NavigationService.GoBack();
                    }
                }
                catch (OutOfMemoryException ex)
                {
                    ViewData["ResponseMessage"] = Resources.OutOfMemory;

                    DebugHelper.WriteLogEntry("UploadPhotoController.SnapPhotoWithinCamera OutOfMemoryException Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("UploadPhotoController.SnapPhotoWithinCamera OutOfMemoryException StackTrace: " + ex.StackTrace);
                }
                catch (Exception ex)
                {
                    ViewData["ResponseMessage"] = Resources.ErrorMessages_OperationIsDoneUnsuccsessfully;

                    DebugHelper.WriteLogEntry("UploadPhotoController.ActivateForm Exception Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("UploadPhotoController.ActivateForm Exception StackTrace: " + ex.StackTrace);
                }
                finally
                {
                    view.UpdateView("MainResponse");
                }
            }

            #endregion

            #region key == "DeactivateForm"

            if (key == "DeactivateForm")
            {
                if (ViewData["EmptyImage"] != null)
                {
                    Marshal.FinalReleaseComObject((IImage)ViewData["EmptyImage"]);
                    ViewData["EmptyImage"] = null;
                }

                if (ViewData["Image"] != null)
                {
                    Marshal.FinalReleaseComObject((IImage)ViewData["Image"]);
                    ViewData["Image"] = null;
                }

                ViewData["ImageFile"] = SystemConfiguration.AppInstallPath + @"\Cache\Files\image.jpg";
                ViewData["ThumbImageFile"] = SystemConfiguration.AppInstallPath + @"\Cache\Files\t_image.jpg";
                ViewData["EmptyImageFile"] = SystemConfiguration.AppInstallPath + @"\Cache\Files\empty.jpg";

                if (File.Exists((string)ViewData["ImageFile"]))
                {
                    File.Delete((string)ViewData["ImageFile"]);
                }

                if (File.Exists((string)ViewData["ThumbImageFile"]))
                {
                    File.Delete((string)ViewData["ThumbImageFile"]);
                }

                if (File.Exists((string)ViewData["EmptyImageFile"]))
                {
                    File.Delete((string)ViewData["EmptyImageFile"]);
                }
            }

            #endregion

            #region key == "RotareImageClockwise" || key == "RotareImageCounterclockwise"

            if (key == "RotareImageClockwise" || key == "RotareImageCounterclockwise")
            {
                try
                {
                    IImage newIImage;
                    System.Drawing.Size imageSize;

                    int displayAreaWidth = (int)view.ViewData["DisplayAreaWidth"];
                    int displayAreaHeight = (int)view.ViewData["DisplayAreaHeight"];

                    if (key == "RotareImageClockwise")
                    {
                        Globals.BaseLogic.IDataLogic.SetUplPhtViewPhtRtnAnglCW();
                    }

                    if (key == "RotareImageCounterclockwise")
                    {
                        Globals.BaseLogic.IDataLogic.SetUplPhtViewPhtRtnAnglCCW();
                    }

                    newIImage = (IImage)ViewData["Image"];
                    ViewData["Image"] = ViewData["EmptyImage"];
                    Marshal.FinalReleaseComObject(newIImage);

                    ImageHelper.SaveScaledImage((string)ViewData["ImageFile"], (string)ViewData["ThumbImageFile"], (int)Math.Min(displayAreaWidth, displayAreaHeight), Globals.BaseLogic.IDataLogic.GetUplPhtViewPhtRtnAngl());

                    ImageHelper.LoadImageFromFile((string)ViewData["ThumbImageFile"], out newIImage, out imageSize);

                    ViewData["Image"] = newIImage;
                    ViewData["ImageWidth"] = imageSize.Width;
                    ViewData["ImageHeight"] = imageSize.Height;
                }
                catch (OutOfMemoryException ex)
                {
                    ViewData["ResponseMessage"] = Resources.OutOfMemory;

                    DebugHelper.WriteLogEntry("UploadPhotoController.RotareImage OutOfMemoryException Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("UploadPhotoController.RotareImage OutOfMemoryException StackTrace: " + ex.StackTrace);
                }
                catch (Exception ex)
                {
                    ViewData["ResponseMessage"] = Resources.ErrorMessages_OperationIsDoneUnsuccsessfully;

                    DebugHelper.WriteLogEntry("UploadPhotoController.RotareImage Exception Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("UploadPhotoController.RotareImage Exception StackTrace: " + ex.StackTrace);
                }
                finally
                {
                    view.UpdateView("MainResponse");
                }
            }

            #endregion

            #region key == "UploadPhoto"

            if (key == "UploadPhoto")
            {
                /*
                LoadingControlInterface lc = LoadingControl.CreateLoading(Resources.ImageUpload);

                Thread asyncDataThread = new Thread(delegate { AsyncGetViewData(lc); });
                asyncDataThread.IsBackground = true;
                asyncDataThread.Start();

                lc.ShowLoading(true);

                if (lc.Abort)
                {
                    asyncDataThread.Abort();
                }           
                 */

                try
                {

                    ImageHelper.PostProcessImageFile((string)ViewData["ImageFile"], Globals.BaseLogic.IDataLogic.GetUplPhtViewPhtRtnAngl());
                    Globals.BaseLogic.IDataLogic.SetUplPhtViewPhtRtnAnglZero(); // после PostProcess угол необходимо сбросить

                    byte[] file;

                    using (BinaryReader newBinaryReader = new BinaryReader(File.Open((string)ViewData["ImageFile"], FileMode.Open)))
                    {
                        file = new byte[newBinaryReader.BaseStream.Length];
                        newBinaryReader.Read(file, 0, file.Length);
                    }

                    using (new WaitWrapper())
                    {
                    Globals.BaseLogic.UploadPhoto(file, false, false);

                    }

                    ViewData["ResponseMessage"] = Resources.UploadPhoto_Controller_ResponseMessage_ImageSuccessfullyDownloaded;
                }
                catch (VKException ex)
                {
                    string error = ExceptionTranslation.TranslateException(ex);

                    if (ex.LocalizedMessage.Equals(ExceptionMessage.IncorrectLoginOrPassword))
                    {
                        Globals.BaseLogic.IDataLogic.SetToken(string.Empty);

                        view.UpdateView("GoToLogin");
                    }

                    ViewData["ResponseMessage"] = error;
                }
                catch (OutOfMemoryException ex)
                {
                    ViewData["ResponseMessage"] = Resources.OutOfMemory;

                    DebugHelper.WriteLogEntry("UploadPhotoController.UploadPhoto OutOfMemoryException Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("UploadPhotoController.UploadPhoto OutOfMemoryException StackTrace: " + ex.StackTrace);
                }
                catch (Exception ex)
                {
                    ViewData["ResponseMessage"] = Resources.ErrorMessages_OperationIsDoneUnsuccsessfully;

                    DebugHelper.WriteLogEntry("UploadPhotoController.UploadPhoto Exception Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("UploadPhotoController.UploadPhoto Exception StackTrace: " + ex.StackTrace);
                }
                finally
                {
                    view.UpdateView("MainResponse");
                }

            }

            #endregion

            //

            #region key == "GoBack"

            if (key == "GoBack")
            {
                //NavigationService.GoBack();

                MasterForm.Navigate<ShareController>("AllowReloadData", "false");
            }

            #endregion

            if (key == "GoGoToLogin")
            {
                MasterForm.Navigate<LoginController>();
            }
        }

        protected override void OnInitialize(params object[] parameters)
        {
            OnViewStateChanged("ClearRegistry");

            base.OnInitialize(parameters);
        }

        #endregion
        
        private void AsyncGetViewData(LoadingControlInterface lc)
        {
            /*
            lc.Current = 0;

            try
            {
                lc.Current = 5;

                ImageHelper.PostProcessImageFile((string)ViewData["ImageFile"], Globals.BaseLogic.IDataLogic.GetUplPhtViewPhtRtnAngl());
                Globals.BaseLogic.IDataLogic.SetUplPhtViewPhtRtnAnglZero(); // после PostProcess угол необходимо сбросить

                byte[] file;

                using (BinaryReader newBinaryReader = new BinaryReader(File.Open((string)ViewData["ImageFile"], FileMode.Open)))
                {
                    file = new byte[newBinaryReader.BaseStream.Length];
                    newBinaryReader.Read(file, 0, file.Length);
                }

                //using (new WaitWrapper())
                //{
                    Globals.BaseLogic.UploadPhoto(file, false, false);

                    lc.Current = 95;
                //}

                ViewData["ResponseMessage"] = Resources.UploadPhoto_Controller_ResponseMessage_ImageSuccessfullyDownloaded;
            }
            catch (VKException ex)
            {
                string error = ExceptionTranslation.TranslateException(ex);

                if (ex.LocalizedMessage.Equals(ExceptionMessage.IncorrectLoginOrPassword))
                {
                    Globals.BaseLogic.IDataLogic.SetToken(string.Empty);
                    MasterForm.Navigate<LoginController>();
                }

                ViewData["ResponseMessage"] = error;
            }
            catch (OutOfMemoryException ex)
            {
                ViewData["ResponseMessage"] = Resources.OutOfMemory;

                DebugHelper.WriteLogEntry("UploadPhotoController.UploadPhoto OutOfMemoryException Message: " + ex.Message);
                DebugHelper.WriteLogEntry("UploadPhotoController.UploadPhoto OutOfMemoryException StackTrace: " + ex.StackTrace);
            }
            catch (Exception ex)
            {
                ViewData["ResponseMessage"] = Resources.ErrorMessages_OperationIsDoneUnsuccsessfully;

                DebugHelper.WriteLogEntry("UploadPhotoController.UploadPhoto Exception Message: " + ex.Message);
                DebugHelper.WriteLogEntry("UploadPhotoController.UploadPhoto Exception StackTrace: " + ex.StackTrace);
            }
            finally
            {
                view.UpdateView("MainResponse");
            }

            lc.Current = 100;
             */
        }

    }
}
