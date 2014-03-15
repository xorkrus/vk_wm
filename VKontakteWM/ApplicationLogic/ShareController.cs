using System;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Common;
using Galssoft.VKontakteWM.Components;
using Galssoft.VKontakteWM.Components.Configuration;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.Server;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.Components.Common.ResponseClasses;
//using Galssoft.VKontakteWM.Notification.ServiceClasses;
using Galssoft.VKontakteWM.Properties;
using System.Collections.Generic;
using Galssoft.VKontakteWM.Components.ImageClass;
using Galssoft.VKontakteWM.Components.Common.Localization;
using System.Threading;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.CustomControls;
using Galssoft.VKontakteWM.Components.Cache;
using Galssoft.VKontakteWM.Components.UI.Wrappers;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.GDI;
using System.IO;
using Microsoft.WindowsMobile.Forms;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    class ShareController : Controller<NewsItems>
    {
        #region Constructors

        public ShareController()
            : base(new ShareView())
        {
            Name = "ShareController";

            view.Model = new NewsItems();            
        }

        #endregion

        #region Events

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
            #region GetCurrentStatus

            if (key == "GetCurrentStatus")
            {
                try
                {
                    User currentUser = Globals.BaseLogic.GetAuthorizedUserInfo(true, false);
                    ViewData["LastStatus"] = currentUser.Status;
                }
                catch
                {
                    //
                }
            }

            #endregion

            #region LoadList

            if (key == "LoadList")
            {
                bool isRefrsh = Convert.ToBoolean(ViewData["IsRefresh"]);
                bool allowReloadData = ViewData["AllowReloadData"] == null || Convert.ToBoolean(ViewData["AllowReloadData"]);

                ViewData["AllowReloadData"] = null;

                if (isRefrsh)
                {
                    if (allowReloadData)
                    {
                        LoadingControlInterface lc = LoadingControl.CreateLoading(Resources.DataLoading);

                        var asyncDataThread = new Thread(() => AsyncGetViewData(lc)) { IsBackground = true };
                        asyncDataThread.Start();

                        lc.ShowLoading(true);

                        if (lc.Abort)
                        {
                            asyncDataThread.Abort();
                        }
                    }                    
                }
                else
                {
                    try
                    {
                        ActivityResponse newActivityResponse = Globals.BaseLogic.LoadUserActivityDataList(25, false, false);

                        if (newActivityResponse != null)
                        {
                            FillListModel(newActivityResponse, true);

                            Globals.BaseLogic.ICommunicationLogic.ClearImagesInDictionary();
                        }
                        else
                        {
                            view.Model.Statuses.Clear();
                        }

                        ViewData["ListID"] = Globals.BaseLogic.IDataLogic.GetUid(); // сохраняем ID пользователя для которого был построен список

                        view.UpdateView("LoadListResponse");
                    }
                    catch
                    {
                        //
                    }
                    finally
                    {
                        //ViewData["AllowReloadData"] = null;
                    }
                }

                //ViewData["AllowReloadData"] = null;
            }

            #endregion

            #region ReloadList

            if (key == "ReloadList")
            {
                view.UpdateView("ReloadListResponse");
            }

            #endregion

            #region RefreshList

            if (key == "RefreshList")
            {
                view.UpdateView("RefreshListResponse");
            }

            #endregion


            #region key == "LoadPhotoFromDisk"

            if (key == "LoadPhotoFromDisk")
            {
                try
                {
                    string fileName = SystemConfiguration.AppInstallPath + @"\Cache\Files\image.jpg";

                    //IImage newIImage;
                    //System.Drawing.Size imageSize;

                    //int displayAreaWidth = (int)view.ViewData["DisplayAreaWidth"];
                    //int displayAreaHeight = (int)view.ViewData["DisplayAreaHeight"];
                    //displayAreaWidth -= UISettings.CalcPix(10);
                    //displayAreaHeight -= UISettings.CalcPix(10);

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

                            ImageHelper.PreProcessImageFile(newSelectPictureDialog.FileName, fileName, maxLinearSize);

                            // ???
                            //Globals.BaseLogic.IDataLogic.SetUplPhtViewPhtRtnAnglZero();

                            //newIImage = (IImage)ViewData["Image"];
                            //ViewData["Image"] = ViewData["EmptyImage"];

                            //if (newIImage != null)
                            //{
                            //    Marshal.FinalReleaseComObject(newIImage);
                            //}

                            //ImageHelper.SaveScaledImage((string)ViewData["ImageFile"], (string)ViewData["ThumbImageFile"], (int)Math.Min(displayAreaWidth, displayAreaHeight), Globals.BaseLogic.IDataLogic.GetUplPhtViewPhtRtnAngl());

                            //ImageHelper.LoadImageFromFile((string)ViewData["ThumbImageFile"], out newIImage, out imageSize);

                            //ViewData["Image"] = newIImage;
                            //ViewData["ImageWidth"] = imageSize.Width;
                            //ViewData["ImageHeight"] = imageSize.Height;

                            // ???
                            //Globals.BaseLogic.IDataLogic.SetUplPhtViewHasMdfPht(true);

                            MasterForm.Navigate<SharePhotoController>();
                        }
                    }
                }
                catch (OutOfMemoryException ex)
                {
                    ViewData["ResponseMessage"] = Resources.OutOfMemory;

                    DebugHelper.WriteLogEntry("ShareController.LoadPhotoFromDisk OutOfMemoryException Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("ShareController.LoadPhotoFromDisk OutOfMemoryException StackTrace: " + ex.StackTrace);
                }
                catch (Exception ex)
                {
                    ViewData["ResponseMessage"] = Resources.ErrorMessages_OperationIsDoneUnsuccsessfully;

                    DebugHelper.WriteLogEntry("ShareController.LoadPhotoFromDisk Exception Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("ShareController.LoadPhotoFromDisk Exception StackTrace: " + ex.StackTrace);
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
                    string fileName = SystemConfiguration.AppInstallPath + @"\Cache\Files\image.jpg";

                    //IImage newIImage;
                    //System.Drawing.Size imageSize;

                    //int displayAreaWidth = (int)view.ViewData["DisplayAreaWidth"];
                    //int displayAreaHeight = (int)view.ViewData["DisplayAreaHeight"];
                    //displayAreaWidth -= UISettings.CalcPix(10);
                    //displayAreaHeight -= UISettings.CalcPix(10);

                    CameraCaptureDialog newCameraCaptureDialog = new Microsoft.WindowsMobile.Forms.CameraCaptureDialog();
                    newCameraCaptureDialog.Mode = Microsoft.WindowsMobile.Forms.CameraCaptureMode.Still;

                    // На некоторых телефонах в камере кнопка "Отмена" может не обрабатываться корректно.
                    // Надатие на эту кнопку на Philips приводит к исключению, его и игнорируем.
                    DialogResult result = DialogResult.Cancel;
                    try
                    {
                        result = newCameraCaptureDialog.ShowDialog();
                    }
                    catch(InvalidOperationException)
                    {
                        result = DialogResult.Cancel;
                    }

                    if (result == System.Windows.Forms.DialogResult.OK)
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

                            ImageHelper.PreProcessImageFile(newCameraCaptureDialog.FileName, fileName, maxLinearSize);

                            // ???
                            //Globals.BaseLogic.IDataLogic.SetUplPhtViewPhtRtnAnglZero();

                            //newIImage = (IImage)ViewData["Image"];
                            //ViewData["Image"] = ViewData["EmptyImage"];

                            //if (newIImage != null)
                            //{
                            //    Marshal.FinalReleaseComObject(newIImage);
                            //}

                            //ImageHelper.SaveScaledImage((string)ViewData["ImageFile"], (string)ViewData["ThumbImageFile"], (int)Math.Min(displayAreaWidth, displayAreaHeight), Globals.BaseLogic.IDataLogic.GetUplPhtViewPhtRtnAngl());

                            //ImageHelper.LoadImageFromFile((string)ViewData["ThumbImageFile"], out newIImage, out imageSize);

                            //ViewData["Image"] = newIImage;
                            //ViewData["ImageWidth"] = imageSize.Width;
                            //ViewData["ImageHeight"] = imageSize.Height;

                            //Globals.BaseLogic.IDataLogic.SetUplPhtViewHasMdfPht(true);

                            File.Delete(newCameraCaptureDialog.FileName);

                            MasterForm.Navigate<SharePhotoController>();
                        }
                    }
                }
                catch (OutOfMemoryException ex)
                {
                    ViewData["ResponseMessage"] = Resources.OutOfMemory;

                    DebugHelper.WriteLogEntry("ShareController.SnapPhotoWithinCamera OutOfMemoryException Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("ShareController.SnapPhotoWithinCamera OutOfMemoryException StackTrace: " + ex.StackTrace);
                }
                catch (Exception ex)
                {
                    ViewData["ResponseMessage"] = Resources.ErrorMessages_OperationIsDoneUnsuccsessfully;

                    DebugHelper.WriteLogEntry("ShareController.SnapPhotoWithinCamera Exception Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("ShareController.SnapPhotoWithinCamera Exception StackTrace: " + ex.StackTrace);
                }
                finally
                {
                    view.UpdateView("MainResponse");
                }
            }

            #endregion



            #region ChangeStatus

            if (key == "ChangeStatus")
            {
                MasterForm.Navigate<ChangeStatusController>();
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



            #region ListActualization

            if (key == "ListActualization")
            {
                ViewData["AllowReloadData"] = null;

                string currentID = Globals.BaseLogic.IDataLogic.GetUid();
                string listID = (string)ViewData["ListID"];

                if (currentID != listID)
                {
                    ViewData["IsRefresh"] = false;
                    OnViewStateChanged("LoadList");
                }
            }

            #endregion

            #region UserActivityCacheModification

            if (key == "UserActivityCacheModification")
            {
                if (Configuration.DataRenewType != DataRenewVariants.UpdateAlways)
                {
                    Globals.BaseLogic.UpdateCacheOfUserActivities((string) view.ViewData["ReceivedStatus"],
                                                                  Resources.Empty_Status, Resources.MessageI);
                    OnViewStateChanged("LoadList");
                }
            }

            #endregion

            if (key == "GoGoToLogin")
            {
                MasterForm.Navigate<LoginController>();
            }
        }

        protected override void OnInitialize(params object[] parameters)
        {
            //ViewData["AllowReloadData"] = null;

            if ((parameters != null) && (parameters.Length > 0))
            {
                var param0 = parameters[0] as string;

                if (param0 != null)
                {
                    if (param0.Equals("UpdateStatus"))
                    {
                        var param1 = parameters[1] as string;

                        if (param1 != null)
                        {
                            ViewData["ReceivedStatus"] = param1;
                            OnViewStateChanged("UserActivityCacheModification");
                        }
                    }
                    else if (param0.Equals("AllowReloadData"))
                    {
                        var param1 = parameters[1] as string;

                        if (param1 != null)
                        {
                            ViewData["AllowReloadData"] = false;
                        }                        
                    }
                }                
            }

            base.OnInitialize(parameters);
        }

        #endregion

        private void FillListModel(ActivityResponse newActivityResponse, bool loadImageData)
        {
            view.Model.Statuses.Clear();

            foreach (ActivityData newActivityData in newActivityResponse.arActivityDatas)
            {
                StatusUpdatesListViewItem newStatusUpdateListViewItem = new StatusUpdatesListViewItem();

                #region установка группы

                newStatusUpdateListViewItem.Group = string.Empty;

                if (newActivityData.adTime.Date == DateTime.Now.Date)
                {
                    newStatusUpdateListViewItem.Group = Resources.StatusUpdatesList_Controller_Group_Today;
                }
                else if (newActivityData.adTime.Date == DateTime.Now.AddDays(-1).Date)
                {
                    newStatusUpdateListViewItem.Group = Resources.StatusUpdatesList_Controller_Group_Yesterday;
                }
                else
                {
                    newStatusUpdateListViewItem.Group = newActivityData.adTime.Date.ToString("d MMMM");
                }

                #endregion

                newStatusUpdateListViewItem.Uid = newActivityData.adStatusID.ToString();
                newStatusUpdateListViewItem.UserID = newActivityData.adDataSender.psUserID.ToString();
                newStatusUpdateListViewItem.UserName = newActivityData.adDataSender.psUserName;
                newStatusUpdateListViewItem.UserStatus = newActivityData.adText;
                newStatusUpdateListViewItem.StatusSetDate = newActivityData.adTime;                

                view.Model.Statuses.Add(newStatusUpdateListViewItem);
            }

            view.Model.Statuses.Sort();
        }

        private void UpdateListModel(string status)
        {
            if (status == string.Empty)
            {
                StatusUpdatesListViewItem newStatusUpdateListViewItem = new StatusUpdatesListViewItem();
                    
                newStatusUpdateListViewItem.Group = Resources.StatusUpdatesList_Controller_Group_Today;
                newStatusUpdateListViewItem.UserID = Globals.BaseLogic.IDataLogic.GetUid();
                newStatusUpdateListViewItem.Uid = "0";
                newStatusUpdateListViewItem.UserName = Resources.MessageI;
                newStatusUpdateListViewItem.StatusSetDate = DateTime.Now;
                newStatusUpdateListViewItem.UserStatus = Resources.Empty_Status;

                view.Model.Statuses.Add(newStatusUpdateListViewItem);
                view.Model.Statuses.Sort();

                Globals.BaseLogic.UpdateCacheOfUserActivities(status, Resources.Empty_Status, Resources.MessageI);
            }
        }

        private void AsyncGetViewData(LoadingControlInterface lc)
        {
            lc.Current = 0;

            ActivityResponse newActivityResponse = null;            
            User currentUser = null;
            try
            {
                lc.Current = 5;

                // загружаем прошлые статусы пользователя
                newActivityResponse = Globals.BaseLogic.LoadUserActivityDataList(25, true, false);
                lc.Current = 50;

                currentUser = Globals.BaseLogic.GetAuthorizedUserInfo(true, false);
                lc.Current = 95;                
            }
            catch (VKException ex)
            {
                //timerKeepAwake.Enabled = false;
                string error = ExceptionTranslation.TranslateException(ex);

                if (!string.IsNullOrEmpty(error))
                {
                    ViewData["LoadListResponseMessage"] = error;
                    view.UpdateView("LoadListResponseNegative");

                    if (ex.LocalizedMessage.Equals(ExceptionMessage.IncorrectLoginOrPassword))
                    {
                        //Globals.BaseLogic.IDataLogic.SetToken(string.Empty);
                        //MasterForm.Navigate<LoginController>();

                        view.UpdateView("GoToLogin");
                    }

                    //newActivityResponse = LoadDataFromCache();


                }
            }
            catch (OutOfMemoryException)
            {
                ViewData["GetViewDataResponseMessage"] = Resources.OutOfMemory;
                view.UpdateView("GetViewDataResponseNegative");
            }

            if (newActivityResponse != null)
            {
                FillListModel(newActivityResponse, true);
                
                if (currentUser != null)
                {
                    UpdateListModel(currentUser.Status);
                }
            }
            else
            {
                //view.Model.Statuses.Clear();
            }

            ViewData["ListID"] = Globals.BaseLogic.IDataLogic.GetUid(); // сохраняем ID пользователя для которого был построен список

            view.UpdateView("LoadListResponse");         

            lc.Current = 100;
        }
    }
}
