using System;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Common;
using Galssoft.VKontakteWM.Components;
using Galssoft.VKontakteWM.Components.Configuration;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.Server;
using Galssoft.VKontakteWM.Components.UI.Wrappers;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.Components.Common.ResponseClasses;
//using Galssoft.VKontakteWM.Notification.ServiceClasses;
using Galssoft.VKontakteWM.Properties;
using System.Collections.Generic;
using Galssoft.VKontakteWM.Components.ImageClass;
using System.Text.RegularExpressions;
using System.Threading;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.Common.Localization;
using Galssoft.VKontakteWM.CustomControls;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    class MessagesChainsListController : Controller<List<MessagesChainsListViewItem>>
    {
        #region Constructors

        public MessagesChainsListController()
            : base(new MessagesChainsListView())
        {
            Name = "MessagesChainsListController";

            view.Model = new List<MessagesChainsListViewItem>();

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
                        MessShortCorrespondence newMessShortCorrespondence = Globals.BaseLogic.GetShortCorrespondence(false, false);

                        if (newMessShortCorrespondence != null)
                        {
                            FillListModel(newMessShortCorrespondence);

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

            #region ChangeReceiver

            if (key == "ChangeReceiver")
            {
                using (new WaitWrapper(false))
                {
                    MasterForm.Navigate<FriendsSearchListController>("");
                }
            }

            #endregion

            #region перехеды

            #region GoToNews

            if (key == "GoToNews")
            {
                MasterForm.Navigate<StatusUpdatesListController>();
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

            #endregion



            #region ListActualization

            if (key == "ListActualization")
            {
                //string currentID = Globals.BaseLogic.IDataLogic.GetUid();
                //string listID = (string)ViewData["ListID"];

                //if (currentID != listID)
                //{
                //    ViewData["IsRefresh"] = false;

                //    OnViewStateChanged("LoadList");
                //}

                ViewData["IsRefresh"] = false;
                OnViewStateChanged("LoadList");
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
                if (parameters.Length > 1)
                {
                    {
                        string param0 = parameters[0].ToString();
                        string param1 = parameters[1].ToString();
                        string param2 = parameters[2].ToString();

                        if (!string.IsNullOrEmpty(param0))
                        {
                            if (param0 == "GoToMessagesList")
                            {
                                MasterForm.Navigate<MessagesListController>(param1, param2);
                            }
                        }
                    }
                }
            }

            base.OnInitialize(parameters);
        }

        #endregion

        private void FillListModel(MessShortCorrespondence newMessShortCorrespondence)
        {
            view.Model.Clear();

            foreach (MessShort newMessShort in newMessShortCorrespondence.MessList)
            {
                MessagesChainsListViewItem newMessagesChainsListViewItem = new MessagesChainsListViewItem();

                newMessagesChainsListViewItem.Uid = newMessShort.mID.ToString();
                newMessagesChainsListViewItem.UserID = newMessShort.mUserID;
                newMessagesChainsListViewItem.UserName = newMessShort.mUserName;
                newMessagesChainsListViewItem.MessageText = newMessShort.mLastMessageText;
                newMessagesChainsListViewItem.MessageWroteDate = newMessShort.mTime;

                // многострочное представление в однострочное
                newMessagesChainsListViewItem.MessageText = Regex.Replace(newMessagesChainsListViewItem.MessageText, "\n", " ");

                #region текстовое представление даты

                string datatimeText = string.Empty;

                if (newMessShort.mTime.Date == DateTime.Now.Date)
                {
                    datatimeText += Resources.Today;
                }
                else if (newMessShort.mTime.Date == DateTime.Now.AddDays(-1).Date)
                {
                    datatimeText += Resources.Yesterday;
                }
                else
                {
                    datatimeText += newMessShort.mTime.ToString("dd.MM.yyyy");
                }

                //datatimeText += newMessShort.mTime.ToString("HH:mm");

                newMessagesChainsListViewItem.MessageWroteDateString = datatimeText;

                #endregion

                // значок исходящего
                if (newMessShort.mType == MessageIOType.Inbox)
                {
                    newMessagesChainsListViewItem.IsMessageOutbox = false;
                }
                else
                {
                    newMessagesChainsListViewItem.IsMessageOutbox = true;
                }

                // выделение непрочитанного
                if (!newMessShort.mIsRead)
                {
                    newMessagesChainsListViewItem.IsMessageNew = true;
                }
                else
                {
                    newMessagesChainsListViewItem.IsMessageNew = false;
                }

                #region изображение

                //newMessagesChainsListViewItem.UserPhoto = SystemConfiguration.AppInstallPath + @"\Cache\Files\Thumb\" + HttpUtility.GetMd5Hash(newMessShort.mUserPhotoURL);

                //bool result = Globals.BaseLogic.ICommunicationLogic.LoadImage(newMessShort.mUserPhotoURL, @"Thumb\" + HttpUtility.GetMd5Hash(newMessShort.mUserPhotoURL), false, _afterLoadImageEventHandler, UISettings.CalcPix(50), newMessShort.mTime, "DateTime");

                //newMessagesChainsListViewItem.IsUserPhotoLoaded = result;

                #endregion

                view.Model.Add(newMessagesChainsListViewItem);
            }

            view.Model.Sort();
        }

        private void AsyncGetViewData(LoadingControlInterface lc)
        {
            lc.Current = 0;

            MessShortCorrespondence newMessShortCorrespondence = null;

            try
            {
                lc.Current = 5;

                newMessShortCorrespondence = Globals.BaseLogic.GetShortCorrespondence(true, false);
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
                }
            }
            catch (OutOfMemoryException)
            {
                ViewData["LoadListResponseMessage"] = Resources.OutOfMemory;
                view.UpdateView("LoadListResponseNegative");
            }

            if (newMessShortCorrespondence != null)
            {
                FillListModel(newMessShortCorrespondence);
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
