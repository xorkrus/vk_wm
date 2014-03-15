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

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    class PhotoCommentsUpdatesListController : Controller<List<PhotoCommentsUpdatesViewItem>>
    {
        #region Constructors

        public PhotoCommentsUpdatesListController()
            : base(new PhotoCommentsUpdatesListView())
        {
            Name = "PhotoCommentsUpdatesListController";

            view.Model = new List<PhotoCommentsUpdatesViewItem>();

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
                string fileName = SystemConfiguration.AppInstallPath + @"\Cache\Files\" + e.ImageFilename;

                //ImageHelper.SaveScaledImage(fileName, fileName, e.ImageLinearSize, OpenNETCF.Drawing.RotationAngle.Zero);
                ImageHelper.CustomSaveScaledImage(fileName, fileName, e.ImageLinearSize, OpenNETCF.Drawing.RotationAngle.Zero);

                if (FriendAvatarLoad != null)
                {
                    FriendAvatarLoad(this, new AfterLoadFriendAvatarEventArgs(fileName, e.ImageLast));
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
                        PhotosCommentsResponse newPhotosCommentsResponse = Globals.BaseLogic.LoadPhotosComments(25, false, false); // комментарии                        

                        if (newPhotosCommentsResponse != null)
                        {
                            FillListModel(newPhotosCommentsResponse);

                            Globals.BaseLogic.ICommunicationLogic.ClearImagesInDictionary();
                        }
                        else
                        {
                            view.Model.Clear();
                        }

                        ViewData["ListID"] = Globals.BaseLogic.IDataLogic.GetUid(); // сохраняем ID пользователя для которого был построен список

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

            #region GoToStatuses

            if (key == "GoToStatuses")
            {
                MasterForm.Navigate<StatusUpdatesListController>();
            }

            #endregion

            #region GoToSendMessage

            if (key == "GoToSendMessage")
            {
                MasterForm.Navigate<SendMessageController>("PhotoComments", ViewData["SenderID"], ViewData["SenderName"]);
            }

            #endregion

            #region GoDetailedView

            if (key == "GoDetailedView")
            {
                int uid = Convert.ToInt32((string)ViewData["Uid"]);
                int photoID = Convert.ToInt32((string)ViewData["PhotoID"]);
                string largePhotoURL = (string)ViewData["LargePhotoURL"];

                if (uid > 0 && photoID > 0 && !string.IsNullOrEmpty(largePhotoURL))
                {
                    MasterForm.Navigate<ImageCommentController>("Load", uid.ToString(), photoID.ToString(), largePhotoURL);
                }
            }

            #endregion

            if (key == "NewComment")
            {
                int uid = Convert.ToInt32((string)ViewData["Uid"]);
                int photoID = Convert.ToInt32((string)ViewData["PhotoID"]);
                var url = (string) ViewData["LargePhotoURL"]; 

                if (uid > 0 && photoID > 0)
                {
                    MasterForm.Navigate<SendCommentController>(uid.ToString(), photoID.ToString(), url);
                }
            }


            #region GoToMessages

            if (key == "GoToMessages")
            {
                MasterForm.Navigate<MessagesChainsListController>();
            }

            #endregion

            #region GoToFriends

            if (key == "GoToFriends")
            {
                MasterForm.Navigate<FriendsListController>();
            }

            #endregion

            #region GoToPhotos

            if (key == "GoToPhotos")
            {
                MasterForm.Navigate<ShareController>();
            }

            #endregion

            #region GoToExtras

            if (key == "GoToExtras")
            {
                MasterForm.Navigate<ExtraController>();
            }

            #endregion



            #region ListActualization

            if (key == "ListActualization")
            {
                string currentID = Globals.BaseLogic.IDataLogic.GetUid();
                string listID = (string)ViewData["ListID"];

                if (currentID != listID)
                {
                    ViewData["IsRefresh"] = false;

                    OnViewStateChanged("LoadList");
                }
            }

            #endregion

            if (key == "GoGoToLogin")
            {
                MasterForm.Navigate<LoginController>();
            }
        }

        #endregion

        private void FillListModel(PhotosCommentsResponse newPhotosCommentsResponse)
        {
            view.Model.Clear();

            foreach (CommentPost newCommentPost in newPhotosCommentsResponse.pcrComments)
            {
                PhotoCommentsUpdatesViewItem newPhotoCommentsUpdatesViewItem = new PhotoCommentsUpdatesViewItem();

                #region установка группы

                newPhotoCommentsUpdatesViewItem.Group = string.Empty;

                if (newCommentPost.cpTime.Date == DateTime.Now.Date)
                {
                    newPhotoCommentsUpdatesViewItem.Group = Resources.PhotoCommentsUpdatesList_Controller_Group_Today;
                }
                else if (newCommentPost.cpTime.Date == DateTime.Now.AddDays(-1).Date)
                {
                    newPhotoCommentsUpdatesViewItem.Group = Resources.PhotoCommentsUpdatesList_Controller_Group_Yesterday;
                }
                else
                {
                    newPhotoCommentsUpdatesViewItem.Group = newCommentPost.cpTime.Date.ToString("d MMMM");
                }

                #endregion

                newPhotoCommentsUpdatesViewItem.Uid = newCommentPost.cpID.ToString();
                newPhotoCommentsUpdatesViewItem.UserID = newCommentPost.cpPostSender.psUserID.ToString();
                newPhotoCommentsUpdatesViewItem.SenderName = newCommentPost.cpPostSender.psUserName;
                newPhotoCommentsUpdatesViewItem.Comment = newCommentPost.cpWallData.wdText;
                newPhotoCommentsUpdatesViewItem.CommentSetDate = newCommentPost.cpTime;
                newPhotoCommentsUpdatesViewItem.PhotoID = newCommentPost.cpPhotoData.pdPhotoID.ToString();
                newPhotoCommentsUpdatesViewItem.LargePhotoURL = Globals.BaseLogic.GetPhotoLargeURLByPhotoID(newCommentPost.cpPhotoData.pdPhotoID);

                if (newCommentPost.cpID > 0)
                {
                    newPhotoCommentsUpdatesViewItem.Photo = SystemConfiguration.AppInstallPath + @"\Cache\Files\Thumb\" + HttpUtility.GetMd5Hash(newCommentPost.cpPhotoData.pdPhotoURL130px);

                    bool result =
                        Globals.BaseLogic.ICommunicationLogic.LoadImage(newCommentPost.cpPhotoData.pdPhotoURL130px,
                                                                        @"Thumb\" +
                                                                        HttpUtility.GetMd5Hash(
                                                                            newCommentPost.cpPhotoData.pdPhotoURL130px),
                                                                        false, _afterLoadImageEventHandler,
                                                                        UISettings.CalcPix(50), newCommentPost.cpTime,
                                                                        "DateTime");

                    newPhotoCommentsUpdatesViewItem.IsPhotoLoaded = result;
                }
                else
                {
                    newPhotoCommentsUpdatesViewItem.Photo = string.Empty;
                    newPhotoCommentsUpdatesViewItem.IsPhotoLoaded = false;
                }

                view.Model.Add(newPhotoCommentsUpdatesViewItem);
            }

            view.Model.Sort();
        }

        private void AsyncGetViewData(LoadingControlInterface lc)
        {
            lc.Current = 0;

            PhotosCommentsResponse newPhotosCommentsResponse = null;

            try
            {
                lc.Current = 5;

                newPhotosCommentsResponse = Globals.BaseLogic.LoadPhotosComments(25, true, false); // комментарии
                lc.Current = 50;

                Globals.BaseLogic.ReloadUserPhotos(true, false); // фото пользователя
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

                    if (ex.LocalizedMessage == ExceptionMessage.UnknownError)
                        MessageBox.Show("!!!!!11!!!");
                }
            }
            catch (OutOfMemoryException)
            {
                ViewData["LoadListResponseMessage"] = Resources.OutOfMemory;
                view.UpdateView("LoadListResponseNegative");
            }

            if (newPhotosCommentsResponse != null)
            {
                FillListModel(newPhotosCommentsResponse);
            }
            else
            {
                //view.Model.Clear();
            }

            ViewData["ListID"] = Globals.BaseLogic.IDataLogic.GetUid(); // сохраняем ID пользователя для которого был построен список

            view.UpdateView("LoadListResponse");

            // запускаем поток || прогрузки фотографий
            var t = new Thread(delegate { Globals.BaseLogic.ICommunicationLogic.LoadImagesInDictionary(); })
            {
                IsBackground = true
            };

            t.Start();

            lc.Current = 100;
        }
    }
}
