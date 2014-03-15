using System.Collections.Generic;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.UI.Wrappers;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.Components.Common.ResponseClasses;
using System;
using Galssoft.VKontakteWM.Common;
using Galssoft.VKontakteWM.Components.Common.Localization;
using Galssoft.VKontakteWM.Properties;
using System.Threading;
using Galssoft.VKontakteWM.CustomControls;
using Galssoft.VKontakteWM.Components.Configuration;
using Galssoft.VKontakteWM.Components.Server;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using System.IO;
using System.Drawing;
using Galssoft.VKontakteWM.Components.Cache;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    class ImageCommentController : Controller<List<ImageDetailedListViewItem>>
    {
        #region Constructors

        public ImageCommentController()
            : base(new ImageCommentView())
        {
            Name = "ImageCommentController";

            view.Model = new List<ImageDetailedListViewItem>();

            _afterLoadImageEventHandler += OnAfterLoadImage;
        }

        #endregion

        #region Events

        [PublishEvent("OnFriendAvatarLoad")]
        public event EventHandler FriendAvatarLoad;

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

        private AfterLoadImageEventHandler _afterLoadImageEventHandler;

        private void OnAfterLoadImage(object sender, AfterLoadImageEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.ImageFilename))
            {
                string bigFileName = SystemConfiguration.AppInstallPath + @"\Cache\Files\" + e.ImageFilename;
                string smallFileName = SystemConfiguration.AppInstallPath + @"\Cache\Files\Thumb\" + e.ImageFilename;

                //ImageHelper.SaveScaledImage(bigFileName, smallFileName, e.ImageLinearSize, OpenNETCF.Drawing.RotationAngle.Zero);
                ImageHelper.CustomSaveScaledImage(bigFileName, smallFileName, e.ImageLinearSize,
                                                  OpenNETCF.Drawing.RotationAngle.Zero);

                Size newSize;
                ImageHelper.GetImageSize(smallFileName, out newSize);

                if (FriendAvatarLoad != null)
                {
                    FriendAvatarLoad(this, new AfterLoadFriendAvatarEventArgs(smallFileName, e.ImageLast) { ImageHeight = newSize.Height });
                }
            }
        }

        protected override void OnViewStateChanged(string key)
        {
            #region LoadList

            if (key == "LoadList")
            {
                bool isRefrsh = Convert.ToBoolean(ViewData["IsRefresh"]);

                if (isRefrsh)
                {
                    LoadingControlInterface lc = LoadingControl.CreateLoading(Resources.DataLoading);

                    Thread asyncDataThread = new Thread(delegate { AsyncGetViewData(lc); });
                    asyncDataThread.IsBackground = true;
                    asyncDataThread.Start();

                    lc.ShowLoading(true);

                    if (lc.Abort)
                    {
                        asyncDataThread.Abort();
                    }
                }
                else
                {
                    try
                    {                        
                        int uid = Convert.ToInt32((string)ViewData["UserID"]);
                        int photoID = Convert.ToInt32((string)ViewData["PhotoID"]);
                        string largePhotoURL = (string)ViewData["LargePhotoURL"];

                        PhotosCommentsResponse newPhotosCommentsResponse = Globals.BaseLogic.LoadCommentsToPhoto(uid, photoID, 50, false, false, null);

                        if (newPhotosCommentsResponse != null)
                        {
                            FillListModel(largePhotoURL, newPhotosCommentsResponse);

                            Globals.BaseLogic.ICommunicationLogic.ClearImagesInDictionary();
                        }
                        else
                        {
                            //view.Model.Clear();

                            //NavigationService.GoBack();
                        }

                        view.UpdateView("LoadListResponse");
                    }
                    catch
                    {
                        //
                    }
                }
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

            #region NewComment

            if (key == "NewComment")
            {
                using (new WaitWrapper(false))
                {
                    MasterForm.Navigate<SendCommentController>(ViewData["UserID"], ViewData["PhotoID"], ViewData["LargePhotoURL"]);
                }
            }

            #endregion

            #region PutInAlbum

            if (key == "PutInAlbum")
            {
                try
                {
                    byte[] file;

                    using (
                        var newBinaryReader =
                            new BinaryReader(File.Open(SystemConfiguration.AppInstallPath + @"\Cache\Files\" +
                                                       HttpUtility.GetMd5Hash((string) ViewData["LargePhotoURL"]),
                                                       FileMode.Open)))
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

            #region SaveOnDisk

            if (key == "SaveOnDisk")
            {
                var sfDialog = new SaveFileDialog();

                var bmp = new Bitmap(SystemConfiguration.AppInstallPath + @"\Cache\Files\" +
                                     HttpUtility.GetMd5Hash((string) ViewData["LargePhotoURL"]));

                sfDialog.Filter = "файлы jpeg (*.jpeg)|*.jpeg";

                sfDialog.FileName = Resources.DefaultFileNameToSave;
                if (sfDialog.ShowDialog() == DialogResult.OK)
                {
                    bmp.Save(sfDialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                }

                bmp.Dispose();
            }

            #endregion

            #region GoToSendMessage

            if (key == "GoToSendMessage")
            {
                MasterForm.Navigate<SendMessageController>("ImageDetailedView", ViewData["SenderID"], ViewData["SenderName"]);
            }

            #endregion

            #region GoBack

            if (key == "GoBack")
            {
                NavigationService.GoBack();
            }

            #endregion

            if (key == "GoGoToLogin")
            {
                MasterForm.Navigate<LoginController>();
            }
        }

        protected override void OnInitialize(params object[] parameters)
        {
            if ((parameters != null) && (parameters.Length > 0))
            {
                string param0 = parameters[0] as string;

                if (param0.Equals("Load"))
                {
                    string uid = parameters[1] as string;
                    string photoID = parameters[2] as string;
                    string largePhotoURL = parameters[3] as string;

                    ViewData["UserID"] = uid;
                    ViewData["PhotoID"] = photoID;
                    ViewData["LargePhotoURL"] = largePhotoURL;
                }
            }

            base.OnInitialize(parameters);
        }

        #endregion

        private void FillListModel(string imageURL, PhotosCommentsResponse newPhotosCommentsResponse)
        {
            ImageDetailedListViewItem newImageDetailedListViewItem;

            view.Model.Clear();

            #region формирование первого элемента списка // фото

            newImageDetailedListViewItem = new ImageDetailedListViewItem();

            string fileSmallName = SystemConfiguration.AppInstallPath + @"\Cache\Files\Thumb\" + HttpUtility.GetMd5Hash(imageURL);
            string fileBigName = SystemConfiguration.AppInstallPath + @"\Cache\Files\" + HttpUtility.GetMd5Hash(imageURL);

            newImageDetailedListViewItem.Type = ImageDetailedListViewItemType.Photo;
            newImageDetailedListViewItem.Uid = string.Empty;
            newImageDetailedListViewItem.UserID = string.Empty;
            newImageDetailedListViewItem.Photo = fileSmallName;

            int listMinSize = (int)ViewData["ListMinSize"];
            //listMinSize -= UISettings.CalcPix(5); // добавим хоть какой-то отступ...

            // загрузка фото (большого)
            bool result = Globals.BaseLogic.ICommunicationLogic.LoadImage(imageURL, HttpUtility.GetMd5Hash(imageURL), false, _afterLoadImageEventHandler, listMinSize, 0, "int");

            if (result) // если в кэше есть большая картинка надо удостовериться, что есть и маленькая в thumb
            {
                if (!File.Exists(fileSmallName)) // а если нет, то преобразовать и добавить
                {
                    //ImageHelper.SaveScaledImage(fileBigName, fileSmallName, listMinSize, OpenNETCF.Drawing.RotationAngle.Zero);
                    ImageHelper.CustomSaveScaledImage(fileBigName, fileSmallName, listMinSize, OpenNETCF.Drawing.RotationAngle.Zero);
                }

                Size newSize;

                ImageHelper.GetImageSize(fileSmallName, out newSize);

                newImageDetailedListViewItem.PhotoHeight = newSize.Height;
            }

            newImageDetailedListViewItem.IsPhotoLoaded = result;
            newImageDetailedListViewItem.UserComment = string.Empty;
            newImageDetailedListViewItem.CommentSetDate = new DateTime(0);
            newImageDetailedListViewItem.CommentWroteDateString = string.Empty;
            //newImageDetailedListViewItem.Group = string.Empty;
            newImageDetailedListViewItem.Group = null;

            view.Model.Add(newImageDetailedListViewItem);

            #endregion

            #region формирование второго элемента списка // автор

            newImageDetailedListViewItem = new ImageDetailedListViewItem();

            newImageDetailedListViewItem.Type = ImageDetailedListViewItemType.Author;
            newImageDetailedListViewItem.Uid = string.Empty;
            newImageDetailedListViewItem.UserID = string.Empty;
            newImageDetailedListViewItem.Photo = string.Empty;
            newImageDetailedListViewItem.IsPhotoLoaded = false;
            if (newPhotosCommentsResponse.pcrAuthor != null)
                newImageDetailedListViewItem.UserName = newPhotosCommentsResponse.pcrAuthor.FullName;
            else
                newImageDetailedListViewItem.UserName = Resources.MessageI;
            newImageDetailedListViewItem.UserComment = string.Empty;
            newImageDetailedListViewItem.CommentSetDate = new DateTime(1);
            newImageDetailedListViewItem.CommentWroteDateString = string.Empty;
            newImageDetailedListViewItem.Group = Resources.ImageDetailedList_Group_Author;

            view.Model.Add(newImageDetailedListViewItem);

            #endregion

            // формирование последующих элементов списка
            foreach (var val in newPhotosCommentsResponse.pcrComments)
            {
                newImageDetailedListViewItem = new ImageDetailedListViewItem();

                newImageDetailedListViewItem.Type = ImageDetailedListViewItemType.Comment;
                newImageDetailedListViewItem.Uid = val.cpID.ToString();
                newImageDetailedListViewItem.UserID = val.cpPostSender.psUserID.ToString();
                newImageDetailedListViewItem.Photo = string.Empty;
                newImageDetailedListViewItem.IsPhotoLoaded = false;
                newImageDetailedListViewItem.UserName = val.cpPostSender.psUserName;
                newImageDetailedListViewItem.UserComment = val.cpWallData.wdText;
                newImageDetailedListViewItem.CommentSetDate = val.cpTime;
                newImageDetailedListViewItem.Group = Resources.ImageDetailedList_Group_Comments;

                #region текстовое представление даты

                string datatimeText;

                if (val.cpTime.Date == DateTime.Now.Date)
                {
                    datatimeText = val.cpTime.ToString("HH:mm");                    
                }
                else if (val.cpTime.Date == DateTime.Now.AddDays(-1).Date)
                {
                    datatimeText = Resources.Yesterday;
                }
                else
                {
                    datatimeText = val.cpTime.ToString("d.MM.yyyy");
                }                

                newImageDetailedListViewItem.CommentWroteDateString = datatimeText;

                #endregion

                view.Model.Add(newImageDetailedListViewItem);
            }

            //view.Model.Sort();
        }

        private void AsyncGetViewData(LoadingControlInterface lc)
        {
            lc.Current = 0;

            PhotosCommentsResponse newPhotosCommentsResponse = null;            

            //

            int uid = Convert.ToInt32((string)ViewData["UserID"]);
            int photoID = Convert.ToInt32((string)ViewData["PhotoID"]);
            string largePhotoURL = (string)ViewData["LargePhotoURL"];            

            try
            {
                lc.Current = 5;
                
                newPhotosCommentsResponse = Globals.BaseLogic.LoadCommentsToPhoto(uid, photoID, 50, true, false, null);
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
                        Globals.BaseLogic.IDataLogic.SetToken(string.Empty);

                        view.UpdateView("GoToLogin");
                    }

                    //newPhotosCommentsResponse = LoadDataFromCache(photoID); 
                }
            }
            catch (OutOfMemoryException)
            {
                ViewData["LoadListResponseMessage"] = Resources.OutOfMemory;
                view.UpdateView("LoadListResponseNegative");
            }            

            if (newPhotosCommentsResponse != null)
            {
                FillListModel(largePhotoURL, newPhotosCommentsResponse);
            }
            else
            {
                //view.Model.Clear();
            }

            view.UpdateView("LoadListResponse");

            // запускаем поток || прогрузки фотографий
            var t = new Thread(delegate { Globals.BaseLogic.ICommunicationLogic.LoadImagesInDictionary(); })
            {
                IsBackground = true
            };

            t.Start();

            lc.Current = 100;
        }

        //private PhotosCommentsResponse LoadDataFromCache(int photoID)
        //{
        //    try
        //    {
        //        PhotosCommentsResponseHistory newPhotosCommentsRespounseHistory = Cache.LoadFromCache<PhotosCommentsResponseHistory>(string.Empty, "PhotosCommentsResponseHistory");

        //        if (newPhotosCommentsRespounseHistory != null)
        //        {
        //            PhotosCommentsResponse newPhotosCommentsResponse = newPhotosCommentsRespounseHistory.GetItem(photoID);

        //            if (newPhotosCommentsResponse != null)
        //            {
        //                return newPhotosCommentsResponse; // выводим, если нашли
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        //
        //    }

        //    return null;
        //}
    }
}
