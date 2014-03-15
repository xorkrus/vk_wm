using System;
using Galssoft.VKontakteWM.Common;
using Galssoft.VKontakteWM.Components.Configuration;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.Server;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.Components.Common.ResponseClasses;
using Galssoft.VKontakteWM.Properties;
using System.Collections.Generic;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.Common.Localization;
using System.Threading;
using Galssoft.VKontakteWM.CustomControls;
using Galssoft.VKontakteWM.Components.Cache;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    class MessagesListController : Controller<List<MessagesListViewItem>>
    {
        #region Constructors

        public MessagesListController()
            : base(new MessagesListView())
        {
            Name = "MessagesListController";

            view.Model = new List<MessagesListViewItem>();

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

                ImageHelper.SaveScaledImage(fileName, fileName, e.ImageLinearSize, OpenNETCF.Drawing.RotationAngle.Zero);

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
                        int uid = Convert.ToInt32((string)ViewData["UserID"]);

                        MessUserCorrespondence newMessUserCorrespondence = Globals.BaseLogic.RefreshUserCorrespondence(uid, false, false, null);

                        if (newMessUserCorrespondence != null)
                        {
                            FillListModel(newMessUserCorrespondence);
                        }
                        else
                        {
                            //view.Model.Clear();
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

            #region SentMessage

            if (key == "SentMessage")
            {
                /*
                bool result = false;

                int uid = Convert.ToInt32((string)ViewData["UserID"]);
                string text = (string)ViewData["MessageDraftOutput"];

                try
                {
                    // проверка на Empty во вьюхе
                    result = Globals.BaseLogic.SendMessage(uid, text, false);
                }
                catch (VKException ex)
                {
                    string error = ExceptionTranslation.TranslateException(ex);

                    ViewData["SentMessageResponseMessage"] = error;
                    ViewData["MessageIsSent"] = "false";

                    view.UpdateView("SentMessageResponse");

                    if (ex.LocalizedMessage.Equals(ExceptionMessage.IncorrectLoginOrPassword))
                    {
                        Globals.BaseLogic.IDataLogic.SetToken(string.Empty);
                        MasterForm.Navigate<LoginController>();
                    }

                    return;                    
                }
                catch (OutOfMemoryException)
                {
                    ViewData["SentMessageResponseMessage"] = Resources.OutOfMemory;
                    ViewData["MessageIsSent"] = "false";

                    view.UpdateView("SentMessageResponse");
                    return;
                }

                if (result)
                {
                    DraftMessagesDataIO.DeleteDraftMessage(uid);

                    ViewData["SentMessageResponseMessage"] = Resources.MessagesList_Controller_Messages_MessageSentSuccessfully;
                    ViewData["MessageIsSent"] = "true";
                }
                else
                {
                    ViewData["SentMessageResponseMessage"] = Resources.MessagesList_Controller_Messages_MessageSentUnsuccessfully;
                    ViewData["MessageIsSent"] = "false";
                }

                view.UpdateView("SentMessageResponse");
                */

                MasterForm.Navigate<SendMessageController>("MessagesList", (string)ViewData["UserID"], (string)ViewData["UserName"]);
            }

            #endregion

            #region Back

            if (key == "Back")
            {
                int uid = Convert.ToInt32((string)ViewData["UserID"]);
                string text = ((string)ViewData["MessageDraftOutput"]).Trim();

                //DraftMessagesDataIO.SetDraftMessage(text, uid);

                MasterForm.Navigate<MessagesChainsListController>();
            }

            #endregion

            if (key == "GoGoToLogin")
            {
                MasterForm.Navigate<LoginController>();
            }
        }

        protected override void OnInitialize(params object[] parameters)
        {
            if (parameters != null)
            {
                if (parameters.Length > 0)
                {
                    int uid = 0;
                    string userName = string.Empty;

                    try
                    {
                        uid = Convert.ToInt32(parameters[0].ToString());
                        userName = (string)parameters[1];
                    }
                    catch
                    {
                        NavigationService.GoBack();
                    }
                    
                    ViewData["UserID"] = uid.ToString();
                    ViewData["UserName"] = userName;
                }
            }

            base.OnInitialize(parameters);
        }

        #endregion

        private void FillListModel(MessUserCorrespondence newMessUserCorrespondence)
        {
            view.Model.Clear();

            foreach (MessageCover newMessageCover in newMessUserCorrespondence.mMessages)
            {
                MessagesListViewItem newMessagesListViewItem = new MessagesListViewItem();

                newMessagesListViewItem.Uid = newMessageCover.mID.ToString();
                newMessagesListViewItem.UserName = newMessageCover.mMesSender.mUserName;
                newMessagesListViewItem.MessageText = newMessageCover.mData.mText;
                newMessagesListViewItem.MessageWroteDate = newMessageCover.mTime;

                #region текстовое представление даты

                string datatimeText = string.Empty;

                if (newMessageCover.mTime.Date == DateTime.Now.Date)
                {
                    datatimeText += Resources.Today + " ";
                }
                else if (newMessageCover.mTime.Date == DateTime.Now.AddDays(-1).Date)
                {
                    datatimeText += Resources.Yesterday + " ";
                }
                else
                {
                    datatimeText += newMessageCover.mTime.ToString("dd.MM.yyyy ");
                }

                datatimeText += newMessageCover.mTime.ToString("HH:mm");

                newMessagesListViewItem.MessageWroteDateString = datatimeText;

                #endregion

                // выделение входящего
                if (newMessageCover.mIOType == MessageIOType.Inbox)
                {
                    newMessagesListViewItem.IsMessageInbox = true;
                }
                else
                {
                    newMessagesListViewItem.IsMessageInbox = false;
                }

                #region аватар

                //newMessagesListViewItem.UserPhoto = SystemConfiguration.AppInstallPath + @"\Cache\Files\Thumb\" + HttpUtility.GetMd5Hash(newMessageCover.mMesSender.mUserPhotoURL);

                //bool result = Globals.BaseLogic.ICommunicationLogic.LoadImage(newMessageCover.mMesSender.mUserPhotoURL, @"Thumb\" + HttpUtility.GetMd5Hash(newMessageCover.mMesSender.mUserPhotoURL), false, _afterLoadImageEventHandler, UISettings.CalcPix(50), newMessageCover.mTime, "DateTime");

                //newMessagesListViewItem.IsUserPhotoLoaded = result;

                #endregion

                view.Model.Add(newMessagesListViewItem);
            }

            view.Model.Sort();
        }

        private void AsyncGetViewData(LoadingControlInterface lc)
        {
            lc.Current = 0;

            // приходит через initialize
            int uid = Convert.ToInt32((string)ViewData["UserID"]);
            //ViewData["UserName"] = Globals.BaseLogic.GetFriendName(uid.ToString());

            MessUserCorrespondence newMessUserCorrespondence = null;

            try
            {
                lc.Current = 5;

                newMessUserCorrespondence = Globals.BaseLogic.RefreshUserCorrespondence(uid, true, false, null);
                lc.Current = 95;
            }
            catch (VKException ex)
            {
                //timerKeepAwake.Enabled = false;
                string error = ExceptionTranslation.TranslateException(ex);

                if (!string.IsNullOrEmpty(error))
                {
                    //newMessUserCorrespondence = LoadDataFromCache(uid);

                    ViewData["LoadListResponseMessage"] = error;
                    view.UpdateView("LoadListResponseNegative");

                    if (ex.LocalizedMessage.Equals(ExceptionMessage.IncorrectLoginOrPassword))
                    {
                        Globals.BaseLogic.IDataLogic.SetToken(string.Empty);

                        view.UpdateView("GoToLogin"); ;
                    }
                }
            }
            catch (OutOfMemoryException)
            {
                ViewData["LoadListResponseMessage"] = Resources.OutOfMemory;
                view.UpdateView("LoadListResponseNegative");
            }

            if (newMessUserCorrespondence != null)
            {
                FillListModel(newMessUserCorrespondence);
            }
            else
            {
                //view.Model.Clear();
            }

            //ViewData["MessageDraftInput"] = DraftMessagesDataIO.GetDraftMessage(uid);

            view.UpdateView("LoadListResponse");

            // запускаем поток || прогрузки фотографий
            var t = new Thread(delegate { Globals.BaseLogic.ICommunicationLogic.LoadImagesInDictionary(); })
            {
                IsBackground = true
            };

            t.Start();

            lc.Current = 100;
        }

        //private MessUserCorrespondence LoadDataFromCache(int uid)
        //{
        //    try
        //    {
        //        MessCorrespondence newMessCorrespondence = Cache.LoadFromCache<MessCorrespondence>(string.Empty, "MessageCorrespondence");                

        //        if (newMessCorrespondence != null)
        //        {
        //            if (newMessCorrespondence.UserIsInList(uid))
        //            {
        //                MessUserCorrespondence newUserCorrespondence = null;

        //                newMessCorrespondence.SeekCorrespondence(uid, out newUserCorrespondence);

        //                if (newUserCorrespondence != null)
        //                {
        //                    return newUserCorrespondence;
        //                }
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
