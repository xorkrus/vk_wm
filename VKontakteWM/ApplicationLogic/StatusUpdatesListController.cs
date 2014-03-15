using System;
using Galssoft.VKontakteWM.Common;
using Galssoft.VKontakteWM.Components.Configuration;
using Galssoft.VKontakteWM.Components.Data;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.Server;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.Components.Common.ResponseClasses;
//using Galssoft.VKontakteWM.Notification.ServiceClasses;
using Galssoft.VKontakteWM.Properties;
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
using System.IO;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    class StatusUpdatesListController : Controller<NewsItems>
    {
        #region Constructors

        public StatusUpdatesListController()
            : base(new StatusUpdatesListView())
        {
            Name = "StatusUpdatesListController";

            view.Model = new NewsItems();

            _afterLoadImageEventHandler += OnAfterLoadImage;

            timerKeepAwake = new System.Windows.Forms.Timer();
            timerKeepAwake.Tick += TimerKeepAwakeTick;
            timerKeepAwake.Interval = 10000;
        }

        #endregion

        #region Events

        [PublishEvent("OnSetOnlineMode")]
        public event EventHandler SetOnlineMode;

        [PublishEvent("OnSetOfflineMode")]
        public event EventHandler SetOfflineMode;

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
                ImageHelper.CustomSaveScaledImage(fileName, fileName, e.ImageLinearSize,
                                                  OpenNETCF.Drawing.RotationAngle.Zero);

                if (FriendAvatarLoad != null)
                {
                    FriendAvatarLoad(this, new AfterLoadFriendAvatarEventArgs(fileName, e.ImageLast));
                }
            }
        }

        protected override void OnViewStateChanged(string key)
        {
            #region InitializeSettings

            if (key == "InitializeSettings")
            {
                using (new WaitWrapper())
                {
                    // если приложение запущено при остановленном нотификаторе, то очищаем кэш
                    // это единственное место, где осуществляется это действие
                    //if (!Interprocess.IsServiceRunning)
                    {
                        Cache.DeleteEntryFromCache(string.Empty, "ShortActivityResponse");
                        Cache.DeleteEntryFromCache(string.Empty, "ShortUpdatesPhotosResponse");
                        Cache.DeleteEntryFromCache(string.Empty, "ShortWallResponse");
                        Cache.DeleteEntryFromCache(string.Empty, "ShortPhotosCommentsRespounse");
                    }

                    Configuration.LoadConfigSettings();
                    Configuration.SaveConfigSettings();

                    #region старт/стоп нотификатора

                    if (Configuration.BackgroundNotification != BackgroundNotificationTypes.Off)
                    {
                        Globals.BaseLogic.IDataLogic.SetNtfAutorun();

                        OnViewStateChanged("StartNotificator");
                    }
                    else
                    {
                        OnViewStateChanged("StopNotificator");

                        Globals.BaseLogic.IDataLogic.DelNtfAutorun();
                    }

                    OnViewStateChanged("StopNotificator");

                    #endregion
                }
            }

            #endregion

            #region StartNotificator

            if (key == "StartNotificator")
            {
                if (!string.IsNullOrEmpty(Globals.BaseLogic.IDataLogic.GetToken()))
                {
                    // запуск службы нотификатора
                    try
                    {
                        //if (!Interprocess.IsServiceRunning)
                        //{
                        //    Interprocess.StartService();
                        //}
                    }
                    catch (Exception ex)
                    {
                        ViewData["NotificatorStartError"] = ex.Message;
                        view.UpdateView("NotificatorStartFail");
                    }
                }
                else
                {
                    DialogControl.ShowQuery(Resources.MainView_Button_NotificatorCantStart, DialogButtons.OK);
                }
            }

            #endregion

            #region StopNotificator

            if (key == "StopNotificator")
            {
                // остановка службы нотификатора
                //Interprocess.StopService();
            }

            #endregion

            #region GetViewData

            if (key == "GetViewData")
            {
                bool isRefrsh = Convert.ToBoolean(ViewData["IsRefresh"]);

                if (isRefrsh)
                {

                    LoadingControlInterface lc = LoadingControl.CreateLoading((string)ViewData["NeedChangeText"] != "1" ? Resources.DataLoading : Resources.FirstDataLoading);
                    ViewData["NeedChangeText"] = "0";

                    timerKeepAwake.Enabled = true;

                    Thread asyncDataThread = new Thread(delegate
                                                            {
                                                                try
                                                                {
                                                                    timerKeepAwake.Enabled = true;

                                                                    AsyncGetViewData(lc);

                                                                }
                                                                finally
                                                                {
                                                                    timerKeepAwake.Enabled = false;
                                                                }
                                                            });
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
                        ActivityResponse newActivityResponse = Globals.BaseLogic.LoadActivityDataList(25, false, false);
                        UpdatesPhotosResponse newUpdatesPhotosResponse = Globals.BaseLogic.LoadUpdatesPhotos(25, false, false);

                        if (newActivityResponse != null)
                        {
                            FillListModel(newActivityResponse, false);
                        }
                        else
                        {
                            view.Model.Statuses.Clear();
                        }

                        if (newUpdatesPhotosResponse != null)
                        {
                            FillListModel(newUpdatesPhotosResponse, false);
                        }
                        else
                        {
                            view.Model.Photos.Clear();
                        }

                        view.UpdateView("LoadListResponse");
                    }
                    catch
                    {
                        // гасим
                    }
                }
            }

            #endregion

            #region Check AutoLogin

            if (key == "CheckAutoLogin")
            {
                try
                {
                    using (new WaitWrapper())
                    {
                        //Globals.BaseLogic.AutoLogin();
                    }

                    Initialize("AutoLoginSuccess");
                }
                catch (VKException ex)
                {
                    string message = ExceptionTranslation.TranslateException(ex);

                    //if (!string.IsNullOrEmpty(message))
                    //{
                    //    if (ex.LocalizedMessage == ExceptionMessage.IncorrectLoginOrPassword)
                    //    {
                    //        Globals.BaseLogic.IDataLogic.SetToken(string.Empty);

                    //        MasterForm.Navigate<LoginController>("IncorrectLoginOrPassword");
                    //    }
                    //    else if (ex.LocalizedMessage == ExceptionMessage.NoSavedToken)
                    //    {
                    //        MasterForm.Navigate<LoginController>("NoSavedToken");
                    //    }
                    //    else if (ex.LocalizedMessage == ExceptionMessage.UnknownError)
                    //    {
                    //        MasterForm.Navigate<LoginController>("UnknownError");
                    //    }
                    //    else
                    //    {
                    //        Initialize("LoginSuccess");
                    //    }
                    //}
                }
                catch (OutOfMemoryException)
                {
                    DialogControl.ShowQuery(Resources.OutOfMemory, DialogButtons.OK);
                }
            }

            #endregion

            #region AutoUpdate

            if (key == "AutoUpdate")
            {
                UpdateHelper updateHelper = new UpdateHelper((UIViewBase)ViewData["StatusUpdatesListView"]);
                updateHelper.CheckNewVersion();
            }

            #endregion

            #region GoToSendMessage

            if (key == "GoToSendMessage")
            {
                MasterForm.Navigate<SendMessageController>("FriendsStatus", ViewData["Uid"]);
            }

            #endregion



            // верхнее меню

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

            #region GoToPhotoComments

            if (key == "GoToPhotoComments")
            {
                MasterForm.Navigate<PhotoCommentsUpdatesListController>();
            }

            #endregion



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

            #region GoToPhoto

            if (key == "GoToPhotos")
            {
                //MasterForm.Navigate<UploadPhotoController>();
                MasterForm.Navigate<ShareController>();
            }

            #endregion

            #region GoDetailedView

            if (key == "GoDetailedView")
            {
                int uid = Convert.ToInt32((string)ViewData["Uid"]);
                int photoID = Convert.ToInt32((string)ViewData["PhotoID"]);
                string largePhotoURL = (string)ViewData["LargePhotoURL"];

                if (uid > 0 && photoID > 0)
                {
                    MasterForm.Navigate<ImageCommentController>("Load", uid.ToString(), photoID.ToString(), largePhotoURL);
                }
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
                // на этой форме необходимости проверять нет, так как переход Login -> this вызывает Reload списка

                //string currentID = Globals.BaseLogic.IDataLogic.GetUid();
                //string listID = (string)ViewData["ListID"];

                //if (currentID != listID)
                //{
                //    OnViewStateChanged("GetViewData");
                //}
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

                if (param0.Equals("LoadingPreview"))
                {
                    try
                    {
                        ActivityResponse newActivityResponse = Globals.BaseLogic.LoadActivityDataList(25, false, false);
                        UpdatesPhotosResponse newUpdatesPhotosResponse = Globals.BaseLogic.LoadUpdatesPhotos(25, false, false);

                        if (newActivityResponse != null)
                        {
                            FillListModel(newActivityResponse, false);
                        }

                        if (newUpdatesPhotosResponse != null)
                        {
                            FillListModel(newUpdatesPhotosResponse, false);
                        }

                        ViewData["ListID"] = Globals.BaseLogic.IDataLogic.GetUid(); // сохраняем ID пользователя для которого был построен список

                        view.UpdateView("LoadListResponse");
                    }
                    catch
                    {
                        //
                    }
                }
                else if (param0.Equals("AutoLoginSuccess")) // вызывается при первой инициализации главной формы
                {
                    SetOnlineMode(this, new EventArgs());

                    if (ViewData["IsFirstStart"] == null)
                    {
                        OnViewStateChanged("InitializeSettings");

                        ViewData["IsFirstStart"] = "1";

                        if (Configuration.DataRenewType != DataRenewVariants.DontUpdate) // т.к. это происходит раз и при старте разницы между 2-мя оставшимися вариантами не вижу здесь
                        {
                            ViewData["IsRefresh"] = "true";
                        }
                        else
                        {
                            ViewData["IsRefresh"] = "false";
                        }

                        OnViewStateChanged("GetViewData");

                        //ViewData["IsFirstStart"] = "0"; // если не сбрасывать в 0, то грузит все при каждом обновлении!!!
                        // сбрасываем в 0 т.к. иначе не успевает отобразится сообщение об ошибке
                        // ViewData["IsFirstStart"] = "1";
                        // меняется на 
                        // ViewData["IsFirstStart"] = "0";
                        // быстрее, чем вызывается Paint
                    }
                }
                else if (param0.Equals("LoginSuccess")) // вызывается при переходе LoginForm -> наша форма
                {
                    SetOnlineMode(this, new EventArgs());

                    OnViewStateChanged("InitializeSettings");

                    //if (ViewData["IsFirstStart"] == null)
                    //{
                        ViewData["IsFirstStart"] = "1";
                        ViewData["NeedChangeText"] = "1";
                    //}

                    if (Configuration.DataRenewType != DataRenewVariants.DontUpdate) // // т.к. это происходит раз и при старте разницы между 2-мя оставшимися вариантами не вижу здесь
                    {
                        ViewData["IsRefresh"] = "true";
                    }
                    else
                    {
                        ViewData["IsRefresh"] = "false";
                    }

                    OnViewStateChanged("GetViewData");

                    //ViewData["IsFirstStart"] = "0"; // если не сбрасывать в 0, то грузит все при каждом обновлении!!!
                    // см. выше
                }
                else if (param0.Equals("LoginFail"))
                {
                    if (SetOfflineMode != null)
                    {
                        SetOfflineMode(this, new EventArgs());
                    }
                }
            }

            base.OnInitialize(parameters);
        }

        #endregion

        private void FillListModel(UpdatesPhotosResponse newUpdatesPhotosResponse, bool loadImageData)
        {
            //throw new NullReferenceException();

            view.Model.Photos.Clear();

            foreach (PhotoData newPhotoData in newUpdatesPhotosResponse.uprPhotoDatas)
            {
                PhotosUpdatesListViewItem newPhotosUpdatesListViewItem = new PhotosUpdatesListViewItem();

                newPhotosUpdatesListViewItem.Uid = newPhotoData.pdPhotoID.ToString();
                newPhotosUpdatesListViewItem.UserID = newPhotoData.pdUserID.ToString();
                newPhotosUpdatesListViewItem.Photo = SystemConfiguration.AppInstallPath + @"\Cache\Files\Thumb\" + HttpUtility.GetMd5Hash(newPhotoData.pdPhotoURL130px);
                newPhotosUpdatesListViewItem.LargePhotoURL = newPhotoData.pdPhotoURL604px;

                bool result;

                if (loadImageData)
                {
                    result = Globals.BaseLogic.ICommunicationLogic.LoadImage(newPhotoData.pdPhotoURL130px, @"Thumb\" + HttpUtility.GetMd5Hash(newPhotoData.pdPhotoURL130px), false, _afterLoadImageEventHandler, UISettings.CalcPix(50), newPhotoData.pdPhotoID, "int");
                }
                else
                {
                    if (File.Exists(newPhotosUpdatesListViewItem.Photo)) // в сущности LoadImage делает то же самое но при отсутствии файла лезет в сеть а для отображения "превью" этого не требуется
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }

                newPhotosUpdatesListViewItem.IsPhotoLoaded = result;

                view.Model.Photos.Add(newPhotosUpdatesListViewItem);
            }

            view.Model.Photos.Sort();
        }

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
                //newStatusUpdateListViewItem.UserPhoto = SystemConfiguration.AppInstallPath + @"\Cache\Files\Thumb\" + HttpUtility.GetMd5Hash(newActivityData.adDataSender.psUserPhotoURL);

                //bool result;

                //if (loadImageData)
                //{
                //    result = Globals.BaseLogic.ICommunicationLogic.LoadImage(newActivityData.adDataSender.psUserPhotoURL, @"Thumb\" + HttpUtility.GetMd5Hash(newActivityData.adDataSender.psUserPhotoURL), false, _afterLoadImageEventHandler, UISettings.CalcPix(50), newActivityData.adTime, "DateTime");
                //}
                //else
                //{
                //    result = false;
                //}

                //newStatusUpdateListViewItem.IsUserPhotoLoaded = result;

                view.Model.Statuses.Add(newStatusUpdateListViewItem);
            }

            view.Model.Statuses.Sort();
        }

        private System.Windows.Forms.Timer timerKeepAwake;

        private static void TimerKeepAwakeTick(object sender, EventArgs eventArgs)
        {
            CoreHelper.KeepDeviceAwake();
            DebugHelper.WriteLogEntry("TimerKeepAwakeTick");
        }

        private void AsyncGetViewData(LoadingControlInterface lc)
        {
            string isFirstStart = (string)ViewData["IsFirstStart"];

            ViewData["IsFirstStart"] = "0";

            lc.Current = 2;

            ActivityResponse newActivityResponse = null;
            UpdatesPhotosResponse newUpdatesPhotosResponse = null;

            // первичные данные формы
            try
            {
                // загружаем новые новости
                newActivityResponse = Globals.BaseLogic.LoadActivityDataList(25, true, false);
                lc.Current = 4;

                // загружаем новые фото
                newUpdatesPhotosResponse = Globals.BaseLogic.LoadUpdatesPhotos(25, true, false);
                lc.Current = 10;

                ViewData["FirstActivate"] = string.Empty;
            }
            catch (VKException ex)
            {
                timerKeepAwake.Enabled = false;
                string error = ExceptionTranslation.TranslateException(ex);

                if (!string.IsNullOrEmpty(error))
                {
                    ViewData["GetViewDataResponseMessage"] = error;
                    view.UpdateView("GetViewDataResponseNegative");

                    if (ex.LocalizedMessage.Equals(ExceptionMessage.IncorrectLoginOrPassword))
                    {
                        Globals.BaseLogic.IDataLogic.SetToken(string.Empty);

                        //view.UpdateView("GoToLogin");
                        OnViewStateChanged("GoToLogin");
                    }
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
            }

            if (newUpdatesPhotosResponse != null)
            {
                FillListModel(newUpdatesPhotosResponse, true);
            }

            ViewData["ListID"] = Globals.BaseLogic.IDataLogic.GetUid(); // сохраняем ID пользователя для которого был построен список

            view.UpdateView("LoadListResponse");

            // прочие данные при старте приложения
            if (isFirstStart.Equals("1") && Configuration.DataRenewType != DataRenewVariants.DontUpdate)
            {
                try
                {
                    int totalCount;
                    int alreadyGet;
                    bool isData;

                    // список друзей
                    Globals.BaseLogic.LoadFriendsList(true, false);
                    lc.Current = 12;

                    // список комментариев к фотографиям пользователя                   
                    Globals.BaseLogic.LoadPhotosComments(25, true, false);
                    lc.Current = 14;

                    // список заголовков цепочек сообщений пользователя
                    Globals.BaseLogic.GetShortCorrespondence(true, false);
                    lc.Current = 16;

                    // список статусов пользователя (для формы поделиться)
                    Globals.BaseLogic.LoadUserActivityDataList(25, true, false);
                    lc.Current = 18;

                    // список фотографий пользователя
                    Globals.BaseLogic.ReloadUserPhotos(true, false);
                    lc.Current = 20;

                    // З А Г Р У З К А   Ц Е П О Ч Е К   П Е Р Е П И С О К
                    Globals.BaseLogic.ClearMessageCorrespondence();
                    totalCount = Globals.BaseLogic.PredictCorrespondCount();
                    alreadyGet = 0;
                    isData = false;

                    MessShortCorrespondence newMessShortCorrespondence = DataModel.Data.MessageShortCorrespondence;

                    if (newMessShortCorrespondence == null)
                    {
                        newMessShortCorrespondence = new MessShortCorrespondence();
                    }

                    MessCorrespondence newMessCorrespondence = DataModel.Data.MessageCorrespondence;

                    if (newMessCorrespondence == null)
                    {
                        newMessCorrespondence = new MessCorrespondence();
                    }

                    do
                    {
                        isData = Globals.BaseLogic.UploadNextUserCorrespond(newMessShortCorrespondence, newMessCorrespondence);

                        if (isData)
                        {
                            alreadyGet++;

                            if (totalCount > 0)
                            {
                                lc.Current = 40 - (int)(20 * (((double)(totalCount - alreadyGet)) / ((double)totalCount)));
                            }
                        }

                    }
                    while (isData);

                    DataModel.Data.MessageCorrespondence = newMessCorrespondence;

                    // З А Г Р У З К А   К О М М Е Н Т А Р И Е В
                    Globals.BaseLogic.ClearAllPhotoComments();
                    totalCount = Globals.BaseLogic.PredictPhotoCommentsCount();
                    alreadyGet = 0;
                    isData = false;

                    PhotosCommentsResponseHistory newPhotosCommentsRespounseHistory = DataModel.Data.PhotosCommentsResponseHistoryData;

                    if (newPhotosCommentsRespounseHistory == null)
                    {
                        newPhotosCommentsRespounseHistory = new PhotosCommentsResponseHistory();
                    }

                    PhotosCommentsResponse newPhotosCommentsRespounse = DataModel.Data.PhotosCommentsResponseData;

                    if (newPhotosCommentsRespounse == null)
                    {
                        newPhotosCommentsRespounse = new PhotosCommentsResponse();
                    }

                    UpdatesPhotosResponse nwUpdatesPhotosResponse = DataModel.Data.UpdatesPhotosResponseData;

                    if (nwUpdatesPhotosResponse == null)
                    {
                        nwUpdatesPhotosResponse = new UpdatesPhotosResponse();
                    }

                    do
                    {
                        isData = Globals.BaseLogic.UploadNextPhotoComments(newPhotosCommentsRespounseHistory, newPhotosCommentsRespounse, nwUpdatesPhotosResponse);

                        if (isData)
                        {
                            alreadyGet++;

                            if (totalCount > 0)
                            {
                                lc.Current = 60 - (int)(20 * (((double)(totalCount - alreadyGet)) / ((double)totalCount)));
                            }
                        }

                    }
                    while (isData);

                    DataModel.Data.PhotosCommentsResponseHistoryData = newPhotosCommentsRespounseHistory;

                    // З А Г Р У З К А   П Р О Ф И Л Е Й   П О Л Ь З О В А Т Е Л Е Й
                    totalCount = Globals.BaseLogic.PredictFriendsCount();
                    alreadyGet = 0;
                    isData = false;

                    FriendsListResponse additionalFriendsListResponse = additionalFriendsListResponse = DataModel.Data.FriendsListAdditionalResponseData;

                    if (additionalFriendsListResponse == null)
                    {
                        additionalFriendsListResponse = new FriendsListResponse();
                    }

                    FriendsListResponse oldFriendsListResponse = DataModel.Data.FriendsListResponseData;

                    if (oldFriendsListResponse == null)
                    {
                        oldFriendsListResponse = new FriendsListResponse();
                    }

                    do
                    {
                        isData = Globals.BaseLogic.GetNextFriendsInfo(true, false, additionalFriendsListResponse, oldFriendsListResponse);

                        if (isData)
                        {
                            alreadyGet++;

                            if (totalCount > 0)
                            {
                                lc.Current = 98 - (int)(38 * (((double)(totalCount - alreadyGet)) / ((double)totalCount)));
                            }
                        }
                    } while (isData);

                    DataModel.Data.FriendsListAdditionalResponseData = additionalFriendsListResponse;
                    
                    // все фото
                    Globals.BaseLogic.DownloadAllPhotoData();

                    lc.Current = 99;
                }
                catch (VKException)
                {
                    DebugHelper.WriteLogEntry("Ошибка при загрузке данных при старте приложения");
                }
                catch (OutOfMemoryException)
                {
                    DebugHelper.WriteLogEntry("Ошибка при загрузке данных при старте приложения");
                }
                catch (Exception)
                {
                    DebugHelper.WriteLogEntry("Ошибка при загрузке данных при старте приложения");
                }
            }

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
