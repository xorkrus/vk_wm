using System;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Common;
using Galssoft.VKontakteWM.Components.Common.Localization;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.Configuration;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.ResponseClasses;
using Galssoft.VKontakteWM.Components.Server;
using Galssoft.VKontakteWM.Components.UI.Wrappers;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.Properties;

//using OpenNETCF.Drawing.Imaging;
//using BitmapData = OpenNETCF.Drawing.Imaging.BitmapData;
using Galssoft.VKontakteWM.Components.Skin;

using Microsoft.WindowsMobile.Forms;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.Common.Configuration;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    class UploadPhotoController : Controller
    {
        #region Constructors

        public UploadPhotoController()
            : base(new UploadPhotoView())
        {
            Name = "UploadPhotoController";
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
            #region key == "DeactivateForm"

            if (key == "DeactivateForm")
            {
                if (ViewData["EmptyImage"] != null)
                {
                    Marshal.FinalReleaseComObject((IImage)ViewData["EmptyImage"]);
                    ViewData["EmptyImage"] = null;
                }

                if (ViewData["BaseImage"] != null)
                {
                    Marshal.FinalReleaseComObject((IImage)ViewData["BaseImage"]);
                    ViewData["BaseImage"] = null;
                }

                if (ViewData["DummyImage"] != null)
                {
                    Marshal.FinalReleaseComObject((IImage)ViewData["DummyImage"]);
                    ViewData["DummyImage"] = null;
                }

                if (ViewData["Image"] != null)
                {
                    Marshal.FinalReleaseComObject((IImage)ViewData["Image"]);
                    ViewData["Image"] = null;
                }
            }

            #endregion

            #region key == "ActivateForm"

            if (key == "ActivateForm")
            {
                try
                {
                    ViewData["ImageFile"] = SystemConfiguration.AppInstallPath + @"\Cache\Files\image.jpg";
                    ViewData["ThumbImageFile"] = SystemConfiguration.AppInstallPath + @"\Cache\Files\t_image.jpg";
                    ViewData["BaseImageFile"] = SystemConfiguration.AppInstallPath + @"\Cache\Files\base.jpg";
                    ViewData["ThumbBaseImageFile"] = SystemConfiguration.AppInstallPath + @"\Cache\Files\t_base.jpg";
                    ViewData["DummyImageFile"] = SystemConfiguration.AppInstallPath + @"\Cache\Files\dummy.jpg";
                    ViewData["ThumbDummyImageFile"] = SystemConfiguration.AppInstallPath + @"\Cache\Files\t_dummy.jpg";
                    ViewData["EmptyImageFile"] = SystemConfiguration.AppInstallPath + @"\Cache\Files\empty.jpg";

                    if (ViewData["EmptyImage"] == null)
                    {
                        IImage newIImage;

                        ImageHelper.SaveImageFromMemory(MasterForm.SkinManager.GetImage("EmptyImage"), (string)ViewData["EmptyImageFile"]);

                        ImageHelper.LoadImageFromFile((string)ViewData["EmptyImageFile"], out newIImage);

                        ViewData["EmptyImage"] = newIImage;
                    }

                    int displayAreaWidth = (int)view.ViewData["DisplayAreaWidth"];
                    int displayAreaHeight = (int)view.ViewData["DisplayAreaHeight"];

                    if (ViewData["BaseImage"] == null)
                    {
                        IImage newIImage;

                        ImageHelper.SaveImageFromMemory(MasterForm.SkinManager.GetImage("BaseImage"), (string)ViewData["BaseImageFile"]);

                        ImageHelper.SaveScaledImage((string)ViewData["BaseImageFile"], (string)ViewData["ThumbBaseImageFile"], new System.Drawing.Size(displayAreaWidth, displayAreaHeight));

                        ImageHelper.LoadImageFromFile((string)ViewData["ThumbBaseImageFile"], out newIImage);

                        ViewData["BaseImage"] = newIImage;
                        ViewData["BaseImageWidth"] = displayAreaWidth;
                        ViewData["BaseImageHeight"] = displayAreaHeight;
                    }
                    else
                    {
                        if ((int)ViewData["BaseImageWidth"] != displayAreaWidth || (int)ViewData["BaseImageHeight"] != displayAreaHeight)
                        {
                            IImage newIImage;

                            newIImage = (IImage)ViewData["BaseImage"];
                            ViewData["BaseImage"] = ViewData["EmptyImage"];
                            Marshal.FinalReleaseComObject(newIImage);

                            ImageHelper.SaveScaledImage((string)ViewData["BaseImageFile"], (string)ViewData["ThumbBaseImageFile"], new System.Drawing.Size(displayAreaWidth, displayAreaHeight));

                            ImageHelper.LoadImageFromFile((string)ViewData["ThumbBaseImageFile"], out newIImage);

                            ViewData["BaseImage"] = newIImage;
                            ViewData["BaseImageWidth"] = displayAreaWidth;
                            ViewData["BaseImageHeight"] = displayAreaHeight;
                        }
                    }

                    displayAreaWidth -= UISettings.CalcPix(10);
                    displayAreaHeight -= UISettings.CalcPix(10);

                    if (ViewData["DummyImage"] == null)
                    {
                        IImage newIImage;

                        ImageHelper.SaveImageFromMemory(MasterForm.SkinManager.GetImage("DummyImage"), (string)ViewData["DummyImageFile"]);

                        ImageHelper.SaveScaledImage((string)ViewData["DummyImageFile"], (string)ViewData["ThumbDummyImageFile"], new System.Drawing.Size(displayAreaWidth, displayAreaHeight));

                        ImageHelper.LoadImageFromFile((string)ViewData["ThumbDummyImageFile"], out newIImage);

                        ViewData["DummyImage"] = newIImage;
                        ViewData["DummyImageWidth"] = displayAreaWidth;
                        ViewData["DummyImageHeight"] = displayAreaHeight;
                    }
                    else
                    {
                        if ((int)ViewData["DummyImageWidth"] != displayAreaWidth || (int)ViewData["DummyImageHeight"] != displayAreaHeight)
                        {
                            IImage newIImage;

                            newIImage = (IImage)ViewData["DummyImage"];
                            ViewData["DummyImage"] = ViewData["EmptyImage"];
                            Marshal.FinalReleaseComObject(newIImage);

                            ImageHelper.SaveScaledImage((string)ViewData["DummyImageFile"], (string)ViewData["ThumbDummyImageFile"], new System.Drawing.Size(displayAreaWidth, displayAreaHeight));

                            ImageHelper.LoadImageFromFile((string)ViewData["ThumbDummyImageFile"], out newIImage);

                            ViewData["DummyImage"] = newIImage;
                            ViewData["DummyImageWidth"] = displayAreaWidth;
                            ViewData["DummyImageHeight"] = displayAreaHeight;
                        }
                    }

                    if (File.Exists((string)ViewData["ImageFile"]))
                    {
                        Globals.BaseLogic.IDataLogic.SetUplPhtViewHasMdfPht(true);

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
                        Globals.BaseLogic.IDataLogic.SetUplPhtViewHasMdfPht(false);
                        Globals.BaseLogic.IDataLogic.SetUplPhtViewPhtCmnt(string.Empty);
                        Globals.BaseLogic.IDataLogic.SetUplPhtViewPhtRtnAnglZero();
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

            #region key == "RotareImageClockwise" || key == "RotareImageCounterclockwise"

            if (key == "RotareImageClockwise" || key == "RotareImageCounterclockwise")
            {
                try
                {
                    IImage newIImage;
                    System.Drawing.Size imageSize;

                    int displayAreaWidth = (int)view.ViewData["DisplayAreaWidth"];
                    int displayAreaHeight = (int)view.ViewData["DisplayAreaHeight"];
                    displayAreaWidth -= UISettings.CalcPix(10);
                    displayAreaHeight -= UISettings.CalcPix(10);

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

            #region key == "LoadPhotoFromDisk"

            if (key == "LoadPhotoFromDisk")
            {
                try
                {
                    IImage newIImage;
                    System.Drawing.Size imageSize;

                    int displayAreaWidth = (int)view.ViewData["DisplayAreaWidth"];
                    int displayAreaHeight = (int)view.ViewData["DisplayAreaHeight"];
                    displayAreaWidth -= UISettings.CalcPix(10);
                    displayAreaHeight -= UISettings.CalcPix(10);

                    SelectPictureDialog newSelectPictureDialog = new SelectPictureDialog();
                    newSelectPictureDialog.Filter = Resources.UploadPhoto_Controller_newOpenFileDialog_Filter;

                    if (newSelectPictureDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        if (File.Exists(newSelectPictureDialog.FileName))
                        {
                            int maxLinearSize;

                            switch (Configuration.ImageMaxSize)
                            {
                                case ImageMaxSizeTypes.Res640X480:
                                    maxLinearSize = 640;
                                    break;

                                default:
                                    maxLinearSize = ImageHelper.ImageMaxLinearSize;
                                    break;
                            }

                            ImageHelper.PreProcessImageFile(newSelectPictureDialog.FileName, (string)ViewData["ImageFile"], maxLinearSize);

                            Globals.BaseLogic.IDataLogic.SetUplPhtViewPhtRtnAnglZero();

                            newIImage = (IImage)ViewData["Image"];
                            ViewData["Image"] = ViewData["EmptyImage"];

                            if (newIImage != null)
                            {
                                Marshal.FinalReleaseComObject(newIImage);
                            }

                            ImageHelper.SaveScaledImage((string)ViewData["ImageFile"], (string)ViewData["ThumbImageFile"], (int)Math.Min(displayAreaWidth, displayAreaHeight), Globals.BaseLogic.IDataLogic.GetUplPhtViewPhtRtnAngl());

                            ImageHelper.LoadImageFromFile((string)ViewData["ThumbImageFile"], out newIImage, out imageSize);

                            ViewData["Image"] = newIImage;
                            ViewData["ImageWidth"] = imageSize.Width;
                            ViewData["ImageHeight"] = imageSize.Height;

                            Globals.BaseLogic.IDataLogic.SetUplPhtViewHasMdfPht(true);
                        }
                    }
                }
                catch (OutOfMemoryException ex)
                {
                    ViewData["ResponseMessage"] = Resources.OutOfMemory;

                    DebugHelper.WriteLogEntry("UploadPhotoController.LoadPhotoFromDisk OutOfMemoryException Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("UploadPhotoController.LoadPhotoFromDisk OutOfMemoryException StackTrace: " + ex.StackTrace);
                }
                catch (Exception ex)
                {
                    ViewData["ResponseMessage"] = Resources.ErrorMessages_OperationIsDoneUnsuccsessfully;

                    DebugHelper.WriteLogEntry("UploadPhotoController.LoadPhotoFromDisk Exception Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("UploadPhotoController.LoadPhotoFromDisk Exception StackTrace: " + ex.StackTrace);
                }
                finally
                {
                    view.UpdateView("MainResponse");
                }
            }

            #endregion

            #region key == "SnapPhotoWithinCamera"

            if (key == "SnapPhotoWithinCamera")
            {
                try
                {
                    IImage newIImage;
                    System.Drawing.Size imageSize;

                    int displayAreaWidth = (int)view.ViewData["DisplayAreaWidth"];
                    int displayAreaHeight = (int)view.ViewData["DisplayAreaHeight"];
                    displayAreaWidth -= UISettings.CalcPix(10);
                    displayAreaHeight -= UISettings.CalcPix(10);

                    CameraCaptureDialog newCameraCaptureDialog = new Microsoft.WindowsMobile.Forms.CameraCaptureDialog();
                    newCameraCaptureDialog.Mode = Microsoft.WindowsMobile.Forms.CameraCaptureMode.Still;

                    if (newCameraCaptureDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        if (File.Exists(newCameraCaptureDialog.FileName))
                        {
                            int maxLinearSize;

                            switch (Configuration.ImageMaxSize)
                            {
                                case ImageMaxSizeTypes.Res640X480:
                                    maxLinearSize = 640;
                                    break;

                                default:
                                    maxLinearSize = ImageHelper.ImageMaxLinearSize;
                                    break;
                            }

                            ImageHelper.PreProcessImageFile(newCameraCaptureDialog.FileName, (string)ViewData["ImageFile"], maxLinearSize);

                            Globals.BaseLogic.IDataLogic.SetUplPhtViewPhtRtnAnglZero();

                            newIImage = (IImage)ViewData["Image"];
                            ViewData["Image"] = ViewData["EmptyImage"];

                            if (newIImage != null)
                            {
                                Marshal.FinalReleaseComObject(newIImage);
                            }

                            ImageHelper.SaveScaledImage((string)ViewData["ImageFile"], (string)ViewData["ThumbImageFile"], (int)Math.Min(displayAreaWidth, displayAreaHeight), Globals.BaseLogic.IDataLogic.GetUplPhtViewPhtRtnAngl());

                            ImageHelper.LoadImageFromFile((string)ViewData["ThumbImageFile"], out newIImage, out imageSize);

                            ViewData["Image"] = newIImage;
                            ViewData["ImageWidth"] = imageSize.Width;
                            ViewData["ImageHeight"] = imageSize.Height;

                            Globals.BaseLogic.IDataLogic.SetUplPhtViewHasMdfPht(true);

                            File.Delete(newCameraCaptureDialog.FileName);
                        }
                    }

                    view.UpdateView("SnapPhotoWithinCameraResponse");
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

                    DebugHelper.WriteLogEntry("UploadPhotoController.SnapPhotoWithinCamera Exception Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("UploadPhotoController.SnapPhotoWithinCamera Exception StackTrace: " + ex.StackTrace);
                }
                finally
                {
                    view.UpdateView("MainResponse");
                }
            }

            #endregion

            #region key == "UploadPhoto"

            if (key == "UploadPhotoMobile" || key == "UploadPhotoMain")
            {
                try
                {
                    ImageHelper.PostProcessImageFile((string)ViewData["ImageFile"], Globals.BaseLogic.IDataLogic.GetUplPhtViewPhtRtnAngl());

                    byte[] file;

                    using (BinaryReader newBinaryReader = new BinaryReader(File.Open((string)ViewData["ImageFile"], FileMode.Open)))
                    {
                        file = new byte[newBinaryReader.BaseStream.Length];
                        newBinaryReader.Read(file, 0, file.Length);
                    }

                    bool isAvatar;

                    if (key == "UploadPhotoMobile")
                    {
                        isAvatar = false;
                    }
                    else
                    {
                        isAvatar = true;
                    }

                    using (new WaitWrapper())
                    {
                        Globals.BaseLogic.UploadPhoto(file, isAvatar, false);
                    }

                    File.Delete((string)ViewData["ImageFile"]);

                    Globals.BaseLogic.IDataLogic.SetUplPhtViewHasMdfPht(false);
                    Globals.BaseLogic.IDataLogic.SetUplPhtViewPhtCmnt(string.Empty);
                    Globals.BaseLogic.IDataLogic.SetUplPhtViewPhtRtnAnglZero();

                    ViewData["ResponseMessage"] = Resources.UploadPhoto_Controller_ResponseMessage_ImageSuccessfullyDownloaded;
                }
                catch (VKException ex)
                {
                    string errMessage;

                    switch (ex.LocalizedMessage)
                    {
                        case ExceptionMessage.UnknownError:
                            errMessage = Resources.VK_ERRORS_UnknownError;
                            break;

                        case ExceptionMessage.ServerUnavalible:
                            errMessage = Resources.VK_ERRORS_ServerUnavalible;
                            break;

                        case ExceptionMessage.NoConnection:
                            errMessage = Resources.VK_ERRORS_NoConnection;
                            break;

                        default:
                            errMessage = string.Empty;
                            break;
                    }

                    ViewData["ResponseMessage"] = errMessage;
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

            #region ChangePhotoComment

            if (key == "ChangePhotoComment")
            {
                MasterForm.Navigate<ChangeCommentController>(Globals.BaseLogic.IDataLogic.GetUplPhtViewPhtCmnt());
            }

            #endregion



            #region переходы

            if (key == "GoToNews")
            {
                MasterForm.Navigate<StatusUpdatesListController>();
            }

            if (key == "GoToMessages")
            {
                MasterForm.Navigate<MessagesChainsListController>();
            }

            if (key == "GoToFriends")
            {
                MasterForm.Navigate<FriendsListController>();
            }

            if (key == "GoToExtras")
            {
                MasterForm.Navigate<ExtraController>();
            }

            #endregion
        }

        protected override void OnInitialize(params object[] parameters)
        {
            if (parameters != null)
            {
                if (parameters.Length > 1)
                {
                    string param0 = parameters[0] as string;

                    if (string.IsNullOrEmpty(param0))
                    {
                        param0 = string.Empty;
                    }

                    if (param0.Equals("Comment"))
                    {
                        string param1 = parameters[1] as string;

                        if (string.IsNullOrEmpty(param1))
                        {
                            param1 = string.Empty;
                        }

                        Globals.BaseLogic.IDataLogic.SetUplPhtViewPhtCmnt(param1);
                    }
                }
            }

            base.OnInitialize(parameters);
        }

        #endregion
    }
}
