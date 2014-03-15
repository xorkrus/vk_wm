using System;
using Galssoft.VKontakteWM.Common;
using Galssoft.VKontakteWM.Components;
using Galssoft.VKontakteWM.Components.Common.Localization;
using Galssoft.VKontakteWM.Components.Configuration;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.ResponseClasses;
using Galssoft.VKontakteWM.Components.Server;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.Components.Common.ResponseClasses;
//using Galssoft.VKontakteWM.Notification.ServiceClasses;
using Galssoft.VKontakteWM.Properties;
using System.Threading;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.UI.Wrappers;
using System.Collections.Generic;
using Microsoft.WindowsCE.Forms;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.Cache;
using Galssoft.VKontakteWM.CustomControls;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    internal class MainController : Controller<List<EventButton>>
    {
        #region Constructors

        public MainController()
            : base(new MainView())
        {
            Name = "MainController";

            view.Model = new List<EventButton>();

            _afterLoadImageEventHandler += OnAfterLoadImage;
        }

        #endregion

        #region Events

        [PublishEvent("OnSetOnlineMode")]
        public event EventHandler SetOnlineMode;

        [PublishEvent("OnSetOfflineMode")]
        public event EventHandler SetOfflineMode;

        #endregion

        #region Controller implementations

        public override void Activate()
        {
            view.Activate();

            (new InputPanel()).Enabled = false;
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

                ImageHelper.SaveScaledImage(fileName, fileName, UISettings.CalcPix(50), OpenNETCF.Drawing.RotationAngle.Zero);

                ViewData["AvatarPath"] = fileName;
            }
            else
            {
                ViewData["AvatarPath"] = string.Empty;
            }

            view.UpdateView("RefreshAvatarFromCache");
        }

        protected override void OnViewStateChanged(string key)
        {
            #region Check AutoLogin

            if (key == "CheckAutoLogin")
            {
                try
                {
                    using (new WaitWrapper())
                    {
                        Globals.BaseLogic.AutoLogin();
                    }

                    Initialize("LoginSuccess");
                }
                catch (VKException ex)
                {
                    string message = ExceptionTranslation.TranslateException(ex);
                    
                    if (!string.IsNullOrEmpty(message))
                    {
                        if (ex.LocalizedMessage == ExceptionMessage.IncorrectLoginOrPassword)
                        {
                            Globals.BaseLogic.IDataLogic.SetToken(string.Empty);

                            MasterForm.Navigate<LoginController>("IncorrectLoginOrPassword");
                        }
                        else if (ex.LocalizedMessage == ExceptionMessage.NoSavedToken)
                        {
                            MasterForm.Navigate<LoginController>();
                        }
                        else if (ex.LocalizedMessage == ExceptionMessage.UnknownError)
                        {
                            MasterForm.Navigate<LoginController>("UnknownError");
                        }
                        else
                        {
                            Initialize("LoginSuccess");
                        }
                    }
                }
                catch (OutOfMemoryException)
                {
                    DialogControl.ShowQuery(Resources.OutOfMemory, DialogButtons.OK);
                }
            }

            #endregion

            #region InitializeSettings

            else if (key == "InitializeSettings")
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

                    #region SetShowEventButtons

                    if (Globals.BaseLogic.IDataLogic.GetShowButtonMessages())
                    {
                        Globals.BaseLogic.IDataLogic.SetShowButtonMessages();
                    }

                    if (Globals.BaseLogic.IDataLogic.GetShowButtonComments())
                    {
                        Globals.BaseLogic.IDataLogic.SetShowButtonComments();
                    }

                    if (Globals.BaseLogic.IDataLogic.GetShowButtonFriends())
                    {
                        Globals.BaseLogic.IDataLogic.SetShowButtonFriends();
                    }

                    if (Globals.BaseLogic.IDataLogic.GetShowButtonFriendsNews())
                    {
                        Globals.BaseLogic.IDataLogic.SetShowButtonFriendsNews();
                    }

                    if (Globals.BaseLogic.IDataLogic.GetShowButtonFriendsPhotos())
                    {
                        Globals.BaseLogic.IDataLogic.SetShowButtonFriendsPhotos();
                    }

                    if (Globals.BaseLogic.IDataLogic.GetShowButtonWallMessages())
                    {
                        Globals.BaseLogic.IDataLogic.SetShowButtonWallMessages();
                    }

                    #endregion

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
                    //// запуск службы нотификатора
                    //try
                    //{
                    //    if (!Interprocess.IsServiceRunning)
                    //    {
                    //        Interprocess.StartService();
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    ViewData["NotificatorStartError"] = ex.Message;
                    //    view.UpdateView("NotificatorStartFail");
                    //}
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

            #region GetMainViewData

            if (key == "GetMainViewData")
            {
                LoadingControlInterface lc = LoadingControl.CreateLoading(Resources.DataLoading);

                Thread asyncDataThread = new Thread(delegate { AsyncGetMainViewData(lc); });

                asyncDataThread.IsBackground = true;
                asyncDataThread.Start();

                lc.ShowLoading(false);

                if (lc.Abort)
                {
                    asyncDataThread.Abort();
                }
            }

            #endregion

            #region GoToUploadPhoto

            if (key == "GoToUploadPhoto")
            {
                MasterForm.Navigate<UploadPhotoController>();
            }

            #endregion

            #region AutoUpdate

            if (key == "AutoUpdate")
            {
                var updateHelper = new UpdateHelper((UIViewBase)ViewData["MainViewThis"]);

                updateHelper.CheckNewVersion();
            }

            #endregion

            #region ChangeStatus

            if (key == "ChangeStatus")
            {
                MasterForm.Navigate<ChangeStatusController>((string)ViewData["UserStatus"]);
            }

            #endregion

            #region CheckNewEvents

            if (key == "CheckNewEvents")
            {
                // проверка флага необходимости обновления списка событий
                if (Globals.BaseLogic.IDataLogic.GetRefreshEventsFlag() == "1")
                {
                    try
                    {                        
                        // загрузка событий
                        EventsGetResponse newEventsGetResponse = Globals.BaseLogic.EventsGet(false, false, false);

                        ViewData["MessagesCount"] = string.Empty;
                        ViewData["CommentsCount"] = string.Empty;
                        ViewData["FriendsCount"] = string.Empty;
                        ViewData["FriendsNewsCount"] = string.Empty;
                        ViewData["FriendsPhotosCount"] = string.Empty;
                        ViewData["WallCount"] = string.Empty;

                        foreach (Event newEvent in newEventsGetResponse.events)
                        {
                            switch (newEvent.type)
                            {
                                case EventType.Messages:
                                    ViewData["MessagesCount"] = newEvent.number.ToString();
                                    break;

                                case EventType.Comments:
                                    ViewData["CommentsCount"] = newEvent.number.ToString();
                                    break;

                                case EventType.Friends:
                                    ViewData["FriendsCount"] = newEvent.number.ToString();
                                    break;

                                case EventType.FriendsNews:
                                    ViewData["FriendsNewsCount"] = newEvent.number.ToString();
                                    break;

                                case EventType.FriendsPhotos:
                                    ViewData["FriendsPhotosCount"] = newEvent.number.ToString();
                                    break;

                                case EventType.WallMessages:
                                    ViewData["WallCount"] = newEvent.number.ToString();
                                    break;
                            }
                        }

                        view.UpdateView("RefreshEventsInfo");
                    }
                    catch (Exception ex)
                    {                        
                        //
                    }
                }
            }

            #endregion           



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

            #region GoToMessages

            if (key == "GoToMessages")
            {
                MasterForm.Navigate<MessagesChainsListController>();
            }

            #endregion

            #region GoToExtras

            if (key == "GoToExtras")
            {
                MasterForm.Navigate<ExtraController>();
            }

            #endregion
        }

        private void AsyncGetMainViewData(LoadingControlInterface lc)
        {
            bool isRefresh = (Configuration.AutoUpdateAtStart || (string)ViewData["IsRefreshMainViewData"] == "1");
            
            // загрузка данных главной страницы
            try
            {
                lc.Current = 5;

                // загрузка событий
                EventsGetResponse newEventsGetResponse = Globals.BaseLogic.EventsGet(isRefresh, false, true);                
                lc.Current = 10;

                FillEventsListModel(newEventsGetResponse);
                view.UpdateView("RefreshEventsInfo");

                if (lc.Abort)
                {
                    isRefresh = false;
                }

                // загрузка профиля
                User user = Globals.BaseLogic.GetAuthorizedUserInfo(isRefresh, false);
                lc.Current = 20;

                ViewData["UserName"] = user.FirstName + " " + user.LastName;
                ViewData["UserStatus"] = user.Status;
                view.UpdateView("RefreshUserInfo");
                
                if (lc.Abort)
                {
                    isRefresh = false;
                }

                //загрузка аватара
                bool res = Globals.BaseLogic.ICommunicationLogic.LoadImage(user.Photo200px, HttpUtility.GetMd5Hash(user.Photo200px), isRefresh, _afterLoadImageEventHandler, UISettings.CalcPix(50), 0, "int");
                lc.Current = 30;

                if (res)
                {
                    ViewData["AvatarPath"] = SystemConfiguration.AppInstallPath + "//Cache//Files//" + HttpUtility.GetMd5Hash(user.Photo200px);
                }
                else
                {
                    ViewData["AvatarPath"] = string.Empty;
                }

                view.UpdateView("RefreshAvatarFromCache");                

                // прогружаем очередь...
                var t = new Thread(delegate { Globals.BaseLogic.ICommunicationLogic.LoadImagesInDictionary(); })
                {
                    IsBackground = true
                };

                t.Start();
            }
            catch (VKException ex)
            {
                string error = ExceptionTranslation.TranslateException(ex);

                if (!String.IsNullOrEmpty(error))
                {
                    ViewData["GetError"] = error;

                    view.UpdateView("GetFail");

                    if (ex.LocalizedMessage.Equals(ExceptionMessage.IncorrectLoginOrPassword))
                    {
                        Globals.BaseLogic.IDataLogic.SetToken(string.Empty);

                        MasterForm.Navigate<LoginController>();
                    }
                }

                // если не обновились, грузим из кэша
                try
                {
                    // загрузка событий
                    EventsGetResponse newEventsGetResponse = Globals.BaseLogic.EventsGet(false, false, true);
                    lc.Current = 40;

                    FillEventsListModel(newEventsGetResponse);
                    view.UpdateView("RefreshEventsInfo");

                    // профиль
                    User user = Globals.BaseLogic.GetAuthorizedUserInfo(false, false);
                    lc.Current = 50;

                    ViewData["UserName"] = user.FirstName + " " + user.LastName;
                    ViewData["UserStatus"] = user.Status;

                    view.UpdateView("RefreshUserInfo");                    

                    // аватар
                    bool res = Globals.BaseLogic.ICommunicationLogic.LoadImage(user.Photo200px, HttpUtility.GetMd5Hash(user.Photo200px), false, _afterLoadImageEventHandler, UISettings.CalcPix(50), 0, "int");
                    lc.Current = 60;

                    if (res)
                    {
                        ViewData["AvatarPath"] = SystemConfiguration.AppInstallPath + "//Cache//Files//" + HttpUtility.GetMd5Hash(user.Photo200px);                        
                    }
                    else
                    {
                        ViewData["AvatarPath"] = string.Empty;
                    }

                    view.UpdateView("RefreshAvatarFromCache");
                }
                catch (Exception e)
                {
                    DebugHelper.WriteLogEntry(e, "Error reading from cache");
                }
            }
            catch (OutOfMemoryException)
            {
                ViewData["GetError"] = Resources.OutOfMemory;
                view.UpdateView("GetFail");
            }
            catch (Exception ex)
            {
                DebugHelper.WriteLogEntry(ex.Message);
            }

            // прогрузить остальные данные при старте
            if (((string)ViewData["IsFirstStart"]).Equals("1") && Configuration.AutoUpdateAtStart)
            {
                try
                {
                    // список друзей
                    if (lc.Abort)
                    {
                        isRefresh = false;
                    }
                    
                    Globals.BaseLogic.LoadFriendsList(isRefresh, false);
                    lc.Current = 70;

                    // список обновлений статусов
                    if (lc.Abort)
                    {
                        isRefresh = false;
                    }
                    
                    Globals.BaseLogic.LoadActivityDataList(25, isRefresh, false);
                    lc.Current = 80;

                    // список комментариев к фотографиям пользователя                   
                    if (lc.Abort)
                    {
                        isRefresh = false;
                    }
                    
                    Globals.BaseLogic.LoadPhotosComments(25, isRefresh, false);
                    lc.Current = 90;

                    // список заголовков цепочек сообщений пользователя
                    if (lc.Abort)
                    {
                        isRefresh = false;
                    }

                    Globals.BaseLogic.GetShortCorrespondence(isRefresh, false);
                    lc.Current = 95;
                }
                catch (VKException ex)
                {
                    DebugHelper.WriteLogEntry("Can't download data at start: " + ex.LocalizedMessage + ex.StackTrace);
                }
                catch (OutOfMemoryException ex)
                {
                    DebugHelper.WriteLogEntry("Can't download data at start: " + ex.Message + ex.StackTrace);
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteLogEntry("Can't download data at start: " + ex.Message);
                }
            }

            lc.Current = 100;
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
                        // загрузка событий
                        EventsGetResponse newEventsGetResponse = Globals.BaseLogic.EventsGet(false, false, true);

                        FillEventsListModel(newEventsGetResponse);
                        view.UpdateView("RefreshEventsInfo");

                        // профиль
                        User user = Globals.BaseLogic.GetAuthorizedUserInfo(false, false);

                        ViewData["UserName"] = user.FirstName + " " + user.LastName;
                        ViewData["UserStatus"] = user.Status;
                        ViewData["Gender"] = user.Sex;

                        view.UpdateView("RefreshUserInfo");

                        // аватар
                        bool res = Globals.BaseLogic.ICommunicationLogic.LoadImage(user.Photo200px, HttpUtility.GetMd5Hash(user.Photo200px), false, _afterLoadImageEventHandler, UISettings.CalcPix(50), 0, "int");

                        if (res)
                        {
                            ViewData["AvatarPath"] = SystemConfiguration.AppInstallPath + "//Cache//Files//" + HttpUtility.GetMd5Hash(user.Photo200px);
                            view.UpdateView("RefreshAvatarFromCache");
                        }
                    }
                    catch (Exception)
                    {
                        //
                    }
                }
                else if (param0.Equals("LoginSuccess"))
                {
                    SetOnlineMode(this, new EventArgs());

                    //???
                    OnViewStateChanged("InitializeSettings");
                    ViewData["IsFirstStart"] = "1";
                    OnViewStateChanged("GetMainViewData");
                }
                else if (param0.Equals("LoginFail"))
                {
                    if (SetOfflineMode != null)
                    {
                        SetOfflineMode(this, new EventArgs());
                    }
                }
                else if (param0.Equals("LoginConnectionErrorFail"))
                {
                    ViewData["ConnectionError"] = true;
                }
                else if (param0.Equals("Status"))
                {
                    string param1 = parameters[1] as string;

                    if (param1 != null)
                    {
                        ViewData["CurrentStatus"] = parameters[1] as string;
                        ViewData["UserStatus"] = ViewData["CurrentStatus"];

                        view.UpdateView("RefreshStatusBox");
                    }
                }
            }

            base.OnInitialize(parameters);
        }

        private void FillEventsListModel(EventsGetResponse eventsGetResponse)
        {
            view.Model.Clear();

            EventButton viewItemMessages = null;
            EventButton viewItemComments = null;
            EventButton viewItemFriends = null;
            EventButton viewItemFriendsNews = null;
            EventButton viewItemFriendsPhotos = null;
            EventButton viewItemWallMessages = null;

            foreach (Event ev in eventsGetResponse.events)
            {
                switch (ev.type)
                {
                    case EventType.Messages:
                        viewItemMessages = new EventButton(Resources.MainView_Label_Messages, Convert.ToInt16(ev.number), ev.type, MasterForm.SkinManager.GetImage("Message"));
                        viewItemMessages.Count = ev.number;
                        break;

                    case EventType.Comments:
                        viewItemComments = new EventButton(Resources.MainView_Label_Comments, Convert.ToInt16(ev.number), ev.type, MasterForm.GetSkinManagerImageInvoked("Guest"));
                        viewItemComments.Count = ev.number;
                        break;

                    case EventType.Friends:
                        viewItemFriends = new EventButton(Resources.MainView_Label_Friends, Convert.ToInt16(ev.number), ev.type, MasterForm.GetSkinManagerImageInvoked("Mark"));
                        viewItemFriends.Count = ev.number;
                        break;

                    case EventType.FriendsNews:
                        viewItemFriendsNews = new EventButton(Resources.MainView_Label_FriendsNews, Convert.ToInt16(ev.number), ev.type, MasterForm.GetSkinManagerImageInvoked("Notification"));
                        viewItemFriendsNews.Count = ev.number;
                        break;

                    case EventType.FriendsPhotos:
                        viewItemFriendsPhotos = new EventButton(Resources.MainView_Label_FriendsPhotos, Convert.ToInt16(ev.number), ev.type, MasterForm.GetSkinManagerImageInvoked("Activities"));
                        viewItemFriendsPhotos.Count = ev.number;
                        break;

                    case EventType.WallMessages:
                        viewItemWallMessages = new EventButton(Resources.MainView_Label_WallMessages, Convert.ToInt16(ev.number), ev.type, MasterForm.GetSkinManagerImageInvoked("Discussion"));
                        viewItemWallMessages.Count = ev.number;
                        break;
                }
            }

            // формирование списка по порядку

            // сообщения
            if (Globals.BaseLogic.IDataLogic.GetShowButtonMessages())
            {
                if (viewItemMessages == null)
                {
                    viewItemMessages = new EventButton(Resources.MainView_Label_Messages, 0, EventType.Messages, MasterForm.GetSkinManagerImageInvoked("Message"));
                    viewItemMessages.Count = 0;
                    view.Model.Add(viewItemMessages);
                }
                else
                {
                    view.Model.Add(viewItemMessages);
                }
            }

            // уведомления
            if (Globals.BaseLogic.IDataLogic.GetShowButtonComments())
            {
                if (viewItemComments == null)
                {
                    viewItemComments = new EventButton(Resources.MainView_Label_FriendsNews, 0, EventType.Comments, MasterForm.GetSkinManagerImageInvoked("Notification"));
                    viewItemComments.Count = 0;
                    view.Model.Add(viewItemComments);
                }
                else
                {
                    view.Model.Add(viewItemComments);
                }
            }

            // обсуждения
            if (Globals.BaseLogic.IDataLogic.GetShowButtonFriends())
            {
                if (viewItemFriends == null)
                {
                    viewItemFriends = new EventButton(Resources.MainView_Label_WallMessages, 0, EventType.Friends, MasterForm.GetSkinManagerImageInvoked("Discussion"));
                    viewItemFriends.Count = 0;
                    view.Model.Add(viewItemFriends);
                }
                else
                {
                    view.Model.Add(viewItemFriends);
                }
            }

            // лента
            if (Globals.BaseLogic.IDataLogic.GetShowButtonFriendsNews())
            {
                if (viewItemFriendsNews == null)
                {
                    viewItemFriendsNews = new EventButton(Resources.MainView_Label_FriendsPhotos, 0, EventType.FriendsNews, MasterForm.GetSkinManagerImageInvoked("Activities"));
                    viewItemFriendsNews.Count = 0;
                    view.Model.Add(viewItemFriendsNews);
                }
                else
                {
                    view.Model.Add(viewItemFriendsNews);
                }
            }

            // гости
            if (Globals.BaseLogic.IDataLogic.GetShowButtonFriendsPhotos())
            {
                if (viewItemFriendsPhotos == null)
                {
                    viewItemFriendsPhotos = new EventButton(Resources.MainView_Label_Comments, 0, EventType.FriendsPhotos, MasterForm.GetSkinManagerImageInvoked("Guest"));
                    viewItemFriendsPhotos.Count = 0;
                    view.Model.Add(viewItemFriendsPhotos);
                }
                else
                {
                    view.Model.Add(viewItemFriendsPhotos);
                }
            }

            // оценки
            if (Globals.BaseLogic.IDataLogic.GetShowButtonWallMessages())
            {
                if (viewItemWallMessages == null)
                {
                    viewItemWallMessages = new EventButton(Resources.MainView_Label_Friends, 0, EventType.WallMessages, MasterForm.GetSkinManagerImageInvoked("Mark"));
                    viewItemWallMessages.Count = 0;
                    view.Model.Add(viewItemWallMessages);
                }
                else
                {
                    view.Model.Add(viewItemWallMessages);
                }
            }
        }

        #endregion
    }
}
