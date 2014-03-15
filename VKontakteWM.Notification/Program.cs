using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.Common.Localization;
using Galssoft.VKontakteWM.Components.SystemHelpers;
using Galssoft.VKontakteWM.Notification.Properties;
using Galssoft.VKontakteWM.Notification.ServiceClasses;
using Galssoft.VKontakteWM.Notification.NfnsWithSoftKeys;
using Galssoft.VKontakteWM.Components;
using Galssoft.VKontakteWM.Components.Server;
using Galssoft.VKontakteWM.Components.Data;
using Galssoft.VKontakteWM.Components.ResponseClasses;
using Galssoft.VKontakteWM.Components.Cache;
using Galssoft.VKontakteWM.Components.Configuration;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using System.Diagnostics;
using Microsoft.WindowsMobile.Status;

namespace Galssoft.VKontakteWM.Notification
{
    class Program
    {
        //Предоставляет интерфейс для работы с логикой программы
        private static BaseLogic _baseLogic;

        //Класс для работы с нотификациями
        private static NotificationWithSoftKeys _notification;

        //Класс для запоминания предыдущих событий
        private static EventsGetResponse _eventsGetResponsePrev;

        private static DateTime _lastCheckedTime;

        private static int _checkInterval;

        /// <summary>
        /// Registry notification GUID
        /// </summary>
        private static readonly Guid NotificationCLSID = new Guid("{C9883E88-AAAA-4010-838E-7EEF42D55887}");

        private static int NotifyAbortFlag = 0;

        /// <summary>
        /// Initialize standard WM notification options in "Sound and Notifications" system page
        /// </summary>
        /// <returns></returns>
        private static bool InitNotification()
        {
            bool needToSave = false;

            //if (Device.PlatformType == PlatformType.Standard)
            //   return true; // Not yet supported on Smartphone
            NotificationClass programNotification = NotificationClass.Get(NotificationCLSID);

            if (programNotification == null)
            {
                programNotification = new NotificationClass();
                programNotification.Options = NotificationOptions.Message
                        | NotificationOptions.Vibrate | NotificationOptions.Sound;
                programNotification.WaveFile = @"\Windows\Alert-Dopple.wma";
                programNotification.Duration = 10;

                needToSave = true;
            }

            if (String.IsNullOrEmpty(programNotification.Name))
            {
                programNotification.Name = @"""В Контакте"" cобытия:";// This string have to be from the localize resources
                needToSave = true;
            }
            //programNotification.Options |= NotificationOptions.EnableRepeatSoundCheckBox;

            if (needToSave)
            {
                NotificationClass.Set(NotificationCLSID, programNotification);
            }

            return true;
        }

		[DllImport("CoreDll.DLL", EntryPoint = "PlaySound", SetLastError = true)]
		private extern static int WCE_PlaySound(string szSound, IntPtr hMod, int flags);

        private enum Flags
        {
            SND_SYNC = 0x0000, /* play synchronously (default) */
            SND_ASYNC = 0x0001, /* play asynchronously */
            SND_NODEFAULT = 0x0002, /* silence (!default) if sound not found */
            SND_MEMORY = 0x0004, /* pszSound points to a memory file */
            SND_LOOP = 0x0008, /* loop the sound until next sndPlaySound */
            SND_NOSTOP = 0x0010, /* don't stop any currently playing sound */
            SND_NOWAIT = 0x00002000, /* don't wait if the driver is busy */
            SND_ALIAS = 0x00010000, /* name is a registry alias */
            SND_ALIAS_ID = 0x00110000, /* alias is a predefined ID */
            SND_FILENAME = 0x00020000, /* name is file name */
            SND_RESOURCE = 0x00040004, /* name is resource name or atom */
            SND_APPLICATION = 0x0080
        }

        private static void CatchErrors(VKException ex)
        {
            //DebugHelper.WriteLogEntry(ex, ">>> CatchError!! <<<");
            //NotifyAbortFlag = 1;

            //Ошибка при получении информации о пользователе
            string errMessage = "";

            //Выдать сообщение
            switch (ex.LocalizedMessage)
            {
                case ExceptionMessage.IncorrectLoginOrPassword:
                    errMessage = "IncorrectLoginOrPassword";
                    break;
                case ExceptionMessage.NoConnection:
                    errMessage = "NoConnection";
                    break;
                case ExceptionMessage.ServerUnavalible:
                    errMessage = "ServerUnavalible";
                    break;
                case ExceptionMessage.UnknownError:
                    errMessage = "UnknownError";
                    break;
                case ExceptionMessage.NoSavedToken:
                    errMessage = "NoSavedToken";
                    break;
                /*case ExceptionMessage.AccountBloked:
                    errMessage = "AccountBloked";
                    break;*/
            }
            if (errMessage != "")
                DebugHelper.WriteLogEntry(errMessage);
            //errMessage += string.Format("<br>Нотификатор будет остановлен");

            //Icon curIcon = Properties.Resources.Events;
            //DebugHelper.WriteLogEntry("CatchErrors - > OnNotificationShow");
            //OnNotificationShow(errMessage, curIcon, false);
        }

        /// <summary>
        /// Возвращает период проверки
        /// </summary>
        /// <returns>Период в милисекундах</returns>
        private static int GetCheckInterval()
        {
            int interval = 300000; //По дефолту - 5 минут
            try
            {
                interval = Int32.Parse(_baseLogic.IDataLogic.GetNotificationTimer());
            }
            catch (System.ArgumentNullException ex)
            {
                DebugHelper.WriteLogEntry(String.Format(Resources.ServiceApplication_noTimerDataError, ex.ToString()));
            }
            catch (System.FormatException ex)
            {
                DebugHelper.WriteLogEntry(String.Format(Resources.ServiceApplication_timerDataFormatError, ex.ToString()));
            }
            catch (System.OverflowException ex)
            {
                DebugHelper.WriteLogEntry(String.Format(Resources.ServiceApplication_timerDataOverflowError, ex.ToString()));
            }
            return interval;

            //return 1 * 60 * 1000;
        }

        static void Main(string[] args)
        {
            InitNotification();

            //Определение нотификаций и событий 
            _notification = new NotificationWithSoftKeys();
            _notification.LeftSoftKey = new NotificationSoftKey(SoftKeyType.Hide, Properties.Resources.Program_LeftSoftKey);
            _notification.RightSoftKey = new NotificationSoftKey(SoftKeyType.Dismiss, Properties.Resources.Program_RightSoftKey);
            _notification.RightSoftKeyClick += OnNotificationRightSoftKeyClick;

            //Запуск сервиса и регистрация сообщения для него
            ServiceApplication.Name = Interprocess.ServiceName;
            ServiceApplication.RegisterMessage(Interprocess.WM_QUIT_SERVICE);
            ServiceApplication.RegisterMessage(Interprocess.WM_TIMER_TICK);
            ServiceApplication.OnRegisteredMessage += ServiceApplication_OnRegisteredMessage;

            Cache.InitPath(SystemConfiguration.AppInstallPath + "\\Cache");
            _baseLogic = new BaseLogic(new DataLogic(), new CommunicationLogic());
            //_baseLogic.IDataLogic.ClearNotificationTempDataInCache();
            _eventsGetResponsePrev = new EventsGetResponse();

            //автовход
            if (_baseLogic.IDataLogic.GetToken() != "")
            {
                try
                {
                    _baseLogic.AutoLogin();
                    _eventsGetResponsePrev = _baseLogic.EventsGet(true, false, true);

                    DebugHelper.WriteLogEntry("Notificator AutoLogin success.");

                    /*_eventsGetResponsePrev.CommentsCount = 0;
                    _eventsGetResponsePrev.FriendsCount = 0;
                    _eventsGetResponsePrev.MessagesCount = 0;*/

                }
                catch (VKException ex)
                {
                    CatchErrors(ex);
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteLogEntry(ex, "Unexpected error.");
                }
            }
            else
            {
                DebugHelper.WriteLogEntry("Notificator service can't be started, there are no token in registry.");
                Application.Exit();
            }

            //Устанавливаем время последней проверки
            _lastCheckedTime = DateTime.Now;

            //Считываем из реестра значение периода проверки
            _checkInterval = GetCheckInterval();

            //Задаем время, когда необходимо будет разбудить устройство
            try
            {
                WakeupScheduler.ScheduleWakeUp(_lastCheckedTime.AddMilliseconds(_checkInterval));
            }
            catch (ExternalException exception)
            {
                DebugHelper.WriteLogEntry(exception, exception.Message);
                throw;
            }
            catch (Exception exception)
            {
                DebugHelper.WriteLogEntry(exception, "Error in scheduling wakeup program launch");
                throw;
            }

            ServiceApplication.Run();
        }

        /// <summary>
        /// Перехват системных сообщений для обработки сервисом
        /// </summary>
        /// <param name="message"></param>
        static void ServiceApplication_OnRegisteredMessage(ref Microsoft.WindowsCE.Forms.Message message)
        {
            switch (message.Msg)
            {
                case Interprocess.WM_QUIT_SERVICE:
                    try
                    {
                        if (_notification != null)
                            _notification.Visible = false;
                    }
                    catch(Exception)
                    { }
                    ServiceApplication.Exit();
                    break;

                case Interprocess.WM_TIMER_TICK:
                    if ((SystemState.PhoneRoaming && _baseLogic.IDataLogic.GetInRoumingValue() == "1") || !SystemState.PhoneRoaming)
                        OnTimerTick();
                    break;
            }
        }
       
        /// <summary>
        /// События по таймеру
        /// </summary>
        static void OnTimerTick()
        {
            DebugHelper.WriteLogEntry("Notificator OnTimerTick");
            if (_lastCheckedTime.AddMilliseconds(_checkInterval) > DateTime.Now) return;

            //Проверяем, наступило ли время. Если не наступило, то просто выходим
            if (_lastCheckedTime.AddMilliseconds(_checkInterval) > DateTime.Now) return;

            //Задаем время, когда в следующий раз разбудить устройство
            try
            {
                WakeupScheduler.ScheduleWakeUp(DateTime.Now.AddMilliseconds(_checkInterval));
                DebugHelper.WriteLogEntry("WakeupScheduler.ScheduleWakeUp " + DateTime.Now.AddMilliseconds(_checkInterval));
            }
            catch (ExternalException exception)
            {
                DebugHelper.WriteLogEntry(exception, exception.Message);
                throw;
            }
            catch (Exception exception)
            {
                DebugHelper.WriteLogEntry(exception, "Error in scheduling wakeup program launch");
                throw;
            }

            //Сохраняем время последней проверки
            _lastCheckedTime = DateTime.Now;

            GetData();
        }

        #region Ненужный код
        /* Ненужный код
            DebugHelper.WriteLogEntry("Notificator OnTimerTick.");
            System.Drawing.Icon curIcon = Properties.Resources.Events;

            bool newEvent = false;
            bool newMessage = false;
            bool newComment = false;
            bool newFriend = false;
            bool newFriendsPhotos = false;
            bool newFriendsNews = false;
            bool newWallMessages = false;
            bool oldEvent = false;
            string nfnText = "";

            GetEventsResponse eventsGetResponse = _baseLogic.EventsGet(true, false);

            try
            {
                #region Проверка событий (пришли новые или нет)
                if (eventsGetResponse.MessagesCount > _eventsGetResponsePrev.MessagesCount) newMessage = true;
                if (eventsGetResponse.CommentsCount > _eventsGetResponsePrev.CommentsCount) newComment = true;
                if (eventsGetResponse.FriendsCount > _eventsGetResponsePrev.FriendsCount)   newFriend = true;
                if (eventsGetResponse.FriendsPhotosCount > _eventsGetResponsePrev.FriendsPhotosCount) newFriendsPhotos = true;
                if (eventsGetResponse.FriendsNewsCount > _eventsGetResponsePrev.FriendsNewsCount) newFriendsNews = true;
                if (eventsGetResponse.WallCount > _eventsGetResponsePrev.WallCount) newWallMessages = true;
                #endregion

                #region Проверяем пришли ли новые типы событий
                if (newMessage)
                    oldEvent = _eventsGetResponsePrev.MessagesCount > 0;
                if (newComment)
                    oldEvent = _eventsGetResponsePrev.CommentsCount > 0;
                if (newFriend)
                    oldEvent = _eventsGetResponsePrev.FriendsCount > 0;
                if (newFriendsPhotos)
                    oldEvent = _eventsGetResponsePrev.FriendsPhotosCount > 0;
                if (newFriendsNews)
                    oldEvent = _eventsGetResponsePrev.FriendsNewsCount > 0;
                if (newWallMessages)
                    oldEvent = _eventsGetResponsePrev.WallCount > 0;
                #endregion

                #region Обработка по каждому типу
                if (newMessage && !(newComment || newFriend || newFriendsPhotos || newFriendsNews || newWallMessages))
                {
                    DebugHelper.WriteLogEntry("Notificator new message.");
                    curIcon = Properties.Resources.Messages;
                    nfnText = String.Format("{0} ({1})", "Новых сообщений", eventsGetResponse.MessagesCount);
                    if (nfnText != "") newEvent = true;
                }
                if (!newMessage && (newComment || newFriend || newFriendsPhotos || newFriendsNews || newWallMessages))
                {
                    DebugHelper.WriteLogEntry("Notificator new event.");
                    curIcon = Properties.Resources.Events;
                    nfnText = "";
                    if (newComment) nfnText += String.Format("{0} ({1})<br>", "Новых комментариев к фото",  eventsGetResponse.CommentsCount);
                    if (newFriend)  nfnText += String.Format("{0} ({1})<br>", "Hовых приглашений дружить", eventsGetResponse.FriendsCount);
                    if (newFriendsPhotos) nfnText += String.Format("{0} ({1})<br>", "Новых фотографий друзей", eventsGetResponse.FriendsPhotosCount);
                    if (newFriendsNews) nfnText += String.Format("{0} ({1})<br>", "Новостей друзей", eventsGetResponse.FriendsNewsCount);
                    if (newWallMessages) nfnText += String.Format("{0} ({1})<br>", "Новых сообщений на стене", eventsGetResponse.WallCount);

                    if (nfnText != "") newEvent = true;
                }
                if (newMessage && (newComment || newFriend || newFriendsPhotos || newFriendsNews || newWallMessages))
                {
                    DebugHelper.WriteLogEntry("Notificator new message and event.");
                    curIcon = Properties.Resources.BothEvMes;
                    nfnText = "";

                    if (newMessage) nfnText += String.Format("{0} ({1})<br>", "Новых сообщений",            eventsGetResponse.MessagesCount);
                    if (newComment) nfnText += String.Format("{0} ({1})<br>", "Новых комментариев к фото",   eventsGetResponse.CommentsCount);
                    if (newFriend)  nfnText += String.Format("{0} ({1})<br>", "Hовых приглашений дружить",  eventsGetResponse.FriendsCount);
                    if (newFriendsPhotos) nfnText += String.Format("{0} ({1})<br>", "Новых фотографий друзей", eventsGetResponse.FriendsPhotosCount);
                    if (newFriendsNews) nfnText += String.Format("{0} ({1})<br>", "Новостей друзей", eventsGetResponse.FriendsNewsCount);
                    if (newWallMessages) nfnText += String.Format("{0} ({1})<br>", "Новых сообщений на стене", eventsGetResponse.WallCount);

                    if (nfnText != "") newEvent = true;

                }
                #endregion

                _eventsGetResponsePrev = eventsGetResponse;

                #region Вывод нотификации если есть новые события
                //Вывод нотификации если есть новые события
                try
                {
                    if (newEvent)
                    {
                        //SetEvent для приложения, сигнализирующий о том, что есть новые события
                        _baseLogic.IDataLogic.SetRefreshEventsFlag("1");

                        OnNotificationShow(nfnText, curIcon, oldEvent);
                    }
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteLogEntry(String.Format("{0}: {1}", Properties.Resources.Program_OnTimerTick_NfnError, ex.Message));
                    //#if DEBUG
                    //MessageBox.Show(String.Format("{0}: {1}", Properties.Resources.Program_OnTimerTick_NfnError, ex.Message));
                    //#endif
                }
                #endregion
            }
            catch (Exception ex)
            {
                DebugHelper.WriteLogEntry(String.Format("{0}: {1}", Properties.Resources.Program_OnTimerTick_Error, ex.Message));
                //#if DEBUG
                //MessageBox.Show(String.Format("{0}: {1}", Properties.Resources.Program_OnTimerTick_Error, ex.Message));
                //#endif
            }
        }
        */
        #endregion

        static void GetData()
        {
            DebugHelper.WriteLogEntry("Notificator GetData");
            bool showMessage, showComments, showFriends, showFriendsNews, showFriendsPhotos, showWallMessages, newEvent, oldEvent = true;
            bool newMessage, newComments, newFriends, newFriendsNews, newFriendsPhotos, newWallMessages;
            Icon curIcon = Resources.Messages;
            showMessage = showComments = showFriends = showFriendsNews = showFriendsPhotos = showWallMessages = newEvent = newMessage = newComments = newFriends = newFriendsNews = newFriendsPhotos = newWallMessages = false;
            string nfnText = "";

            try
            {
                //Инициализация _eventsGetResponsePrev значениями из реестра
                if (_eventsGetResponsePrev.events.Count == 0)
                {
                    _eventsGetResponsePrev = _baseLogic.IDataLogic.EventsGet();
                }

                //Оновление событий с сервера
                //получение обновленных значений из реестра
                EventsGetResponse eventsGetResponse = null;
                try
                {
                    eventsGetResponse = _baseLogic.EventsGet(true, false, true);
                }
                catch (VKException ex)
                {
                    DebugHelper.WriteLogEntry("***7");
                    CatchErrors(ex);
                    DebugHelper.WriteLogEntry("***8");
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteLogEntry(ex, "Unexpected error");
                }
                DebugHelper.WriteLogEntry("***9");

                //Если значений в реестре не было, то делаю количество всех ивентов 0
                if (_eventsGetResponsePrev.events.Count == 0)
                {
                    _eventsGetResponsePrev = eventsGetResponse;
                    foreach (Event ev in _eventsGetResponsePrev.events)
                    {
                        ev.number = 0;
                    }
                }
                DebugHelper.WriteLogEntry("***10");

                if (eventsGetResponse != null)
                {

                    #region Проверка какие события показывать
                    foreach (Event ev in eventsGetResponse.events)
                    {
                        switch (ev.type)
                        {
                            case EventType.Messages:
                                if (ev.number > 0) showMessage = true;
                                break;
                            case EventType.Comments:
                                if (ev.number > 0) showComments = true;
                                break;
                            case EventType.Friends:
                                if (ev.number > 0) showFriends = true;
                                break;
                            case EventType.FriendsNews:
                                if (ev.number > 0) showFriendsNews = true;
                                break;
                                /*
                            case EventType.FriendsPhotos:
                                if (ev.number > 0) showFriendsPhotos = true;
                                break;
                            case EventType.WallMessages:
                                if (ev.number > 0) showWallMessages = true;
                                break;
                                 */
                        }
                    }
                    #endregion
                    DebugHelper.WriteLogEntry("***11");

                    #region Проверяем изменился ли счетчик событий с последнего обновления
                    foreach (Event ev in eventsGetResponse.events)
                    {
                        switch (ev.type)
                        {
                            case EventType.Messages:
                                foreach (Event evPrev in _eventsGetResponsePrev.events)
                                    if (evPrev.type == ev.type)
                                        if (ev.number > evPrev.number) newMessage = true;
                                break;
                            case EventType.Comments:
                                foreach (Event evPrev in _eventsGetResponsePrev.events)
                                    if (evPrev.type == ev.type)
                                        if (ev.number > evPrev.number) newComments = true;
                                break;
                            case EventType.Friends:
                                foreach (Event evPrev in _eventsGetResponsePrev.events)
                                    if (evPrev.type == ev.type)
                                        if (ev.number > evPrev.number) newFriends = true;
                                break;
                            case EventType.FriendsNews:
                                foreach (Event evPrev in _eventsGetResponsePrev.events)
                                    if (evPrev.type == ev.type)
                                        if (ev.number > evPrev.number) newFriendsNews = true;
                                break;
                                /*
                case EventType.FriendsPhotos:
                    foreach (Event evPrev in _eventsGetResponsePrev.events)
                        if (evPrev.type == ev.type)
                             if (ev.number > evPrev.number) newFriendsPhotos = true;
                    break;
                case EventType.WallMessages:
                    foreach (Event evPrev in _eventsGetResponsePrev.events)
                        if (evPrev.type == ev.type)
                             if (ev.number > evPrev.number) newWallMessages = true;
                    break;
                     */
                        }
                    }

                    #endregion
                    DebugHelper.WriteLogEntry("***12");

                    #region Проверка происходило ли событие раньше
                    oldEvent = true;
                    foreach (Event evPrev in _eventsGetResponsePrev.events)
                    {
                        switch (evPrev.type)
                        {
                            case EventType.Messages:
                                if (newMessage)
                                    if (evPrev.number == 0) oldEvent = false;
                                break;
                            case EventType.Comments:
                                if (newComments)
                                    if (evPrev.number == 0) oldEvent = false;
                                break;
                            case EventType.Friends:
                                if (newFriends)
                                    if (evPrev.number == 0) oldEvent = false;
                                break;
                            case EventType.FriendsNews:
                                if (newFriendsNews)
                                    if (evPrev.number == 0) oldEvent = false;
                                break;
                                /*
                        case EventType.FriendsPhotos:
                            if (newFriendsPhotos)
                                if (evPrev.number == 0) oldEvent = false;
                            break;
                        case EventType.WallMessages:
                            if (newWallMessages)
                                if (evPrev.number == 0) oldEvent = false;
                            break;
                             */
                        }
                    }
                    #endregion
                    DebugHelper.WriteLogEntry("***13");

                    #region Определение иконки
                    curIcon = showMessage ? Resources.Messages : Resources.Events;

                    #endregion
                    DebugHelper.WriteLogEntry("***14");

                    //MessageBox.Show(showMessage + ":" + showFriends + ":" + showFriendsNews);
                    //MessageBox.Show(newMessage + ":" + newFriends + ":" + newFriendsNews);

                    #region Формирование текста сообщения

                    if (newMessage || newComments || newFriends || newFriendsNews /*|| newFriendsPhotos || newWallMessages*/)
                    {
                        DebugHelper.WriteLogEntry("Got new event");
                        nfnText = "";

                        #region текст
                        foreach (Event ev in eventsGetResponse.events)
                        {
                            switch (ev.type)
                            {
                                case EventType.Messages:
                                    if (showMessage) nfnText += String.Format("{0} (+{1})<br>", Resources.Program_OnTimerTick_NfnMess, ev.number);
                                    break;
                            }
                        }
                        foreach (Event ev in eventsGetResponse.events)
                        {
                            switch (ev.type)
                            {
                                case EventType.Comments:
                                    if (showComments) nfnText += String.Format("{0} (+{1})<br>", Resources.Program_OnTimerTick_NfnCmnts, ev.number);
                                    break;
                            }
                        }
                        foreach (Event ev in eventsGetResponse.events)
                        {
                            switch (ev.type)
                            {
                                case EventType.Friends:
                                    if (showFriends) nfnText += String.Format("{0} (+{1})<br>", Resources.Program_OnTimerTick_NfnFrnds, ev.number);
                                    break;
                            }
                        }

                        foreach (Event ev in eventsGetResponse.events)
                        {
                            switch (ev.type)
                            {
                                case EventType.FriendsNews:
                                    if (showFriendsNews) nfnText += String.Format("{0} (+{1})<br>", Resources.Program_OnTimerTick_NfnFrndsNs, ev.number);
                                    break;
                            }
                        }
                        /*
                        foreach (Event ev in eventsGetResponse.events)
                        {
                            switch (ev.type)
                            {
                                case EventType.FriendsPhotos:
                                    if (showFriendsPhotos) nfnText += String.Format("{0} (+{1})<br>", Resources.Program_OnTimerTick_NfnFrndsPh, ev.number);
                                    break;
                            }
                        }

                        foreach (Event ev in eventsGetResponse.events)
                        {
                            switch (ev.type)
                            {
                                case EventType.WallMessages:
                                    if (showWallMessages) nfnText += String.Format("{0} (+{1})<br>", Resources.Program_OnTimerTick_NfnFrnds, ev.number);
                                    break;
                            }
                        }
                         */

                        #endregion
                        DebugHelper.WriteLogEntry("***15");

                        DebugHelper.WriteLogEntry("Notification text(new event): " + nfnText);
                        if (nfnText != "") newEvent = true;

                        //Чтобы попапилась и будила
                        _notification.StraightToTray = false;
                        _notification.DisplayOn = true;
                    }
                    else
                    {
                        #region Если не пришло новое событие - модификация текста
                        // меняем текст в нотификаторе
                        DebugHelper.WriteLogEntry("No new events, just change text (dec counters)");
                        if (_notification != null)
                        {
                            nfnText = "";

                            #region текст
                            foreach (Event ev in eventsGetResponse.events)
                            {
                                switch (ev.type)
                                {
                                    case EventType.Messages:
                                        if (showMessage) nfnText += String.Format("{0} (+{1})<br>", Resources.Program_OnTimerTick_NfnMess, ev.number);
                                        break;
                                }
                            }
                            foreach (Event ev in eventsGetResponse.events)
                            {
                                switch (ev.type)
                                {
                                    case EventType.Comments:
                                        if (showComments) nfnText += String.Format("{0} (+{1})<br>", Resources.Program_OnTimerTick_NfnCmnts, ev.number);
                                        break;
                                }
                            }
                            foreach (Event ev in eventsGetResponse.events)
                            {
                                switch (ev.type)
                                {
                                    case EventType.Friends:
                                        if (showFriends) nfnText += String.Format("{0} (+{1})<br>", Resources.Program_OnTimerTick_NfnFrnds, ev.number);
                                        break;
                                }
                            }

                            foreach (Event ev in eventsGetResponse.events)
                            {
                                switch (ev.type)
                                {
                                    case EventType.FriendsNews:
                                        if (showFriendsNews) nfnText += String.Format("{0} (+{1})<br>", Resources.Program_OnTimerTick_NfnFrndsNs, ev.number);
                                        break;
                                }
                            }

                            /*
                            foreach (Event ev in eventsGetResponse.events)
                            {
                                switch (ev.type)
                                {
                                    case EventType.FriendsPhotos:
                                        if (showFriendsPhotos) nfnText += String.Format("{0} (+{1})<br>", Resources.Program_OnTimerTick_NfnFrnds, ev.number);
                                        break;
                                }
                            }

                            foreach (Event ev in eventsGetResponse.events)
                            {
                                switch (ev.type)
                                {
                                    case EventType.WallMessages:
                                        if (showWallMessages) nfnText += String.Format("{0} (+{1})<br>", Resources.Program_OnTimerTick_NfnFrnds, ev.number);
                                        break;
                                }
                            }
                             */
                            #endregion
                            DebugHelper.WriteLogEntry("***16");

                            DebugHelper.WriteLogEntry("Notification text(mod): " + nfnText);

                            //Чтобы не попапилась и не будила
                            _notification.StraightToTray = true;
                            _notification.DisplayOn = false;
                            //Если текст нотификации изменился на "ничто" - скрыть ее
                            if (nfnText == "") _notification.Visible = false;
                            else _notification.Text = nfnText;
                        }
                        #endregion
                    }
                    #endregion

                    //if (nfnText != "")
                    //nfnText += string.Format("<br>{0}", GetBirthdayString());

                    DebugHelper.WriteLogEntry("***17");
                }


                DebugHelper.WriteLogEntry("***21");

                #region Вывод нотификации если есть новые события
                //Вывод нотификации если есть новые события
                try
                {
                    if (newEvent)
                    {
                        DebugHelper.WriteLogEntry("***22");

                        //SetEvent для приложения, сигнализирующий о том, что есть новые события
                        _baseLogic.IDataLogic.SetRefreshEventsFlag("1");

                        OnNotificationShow(nfnText, curIcon, oldEvent);
                    }
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteLogEntry(ex, Resources.Program_OnTimerTick_NfnError);
                }
                #endregion
                DebugHelper.WriteLogEntry("***23");

                if (eventsGetResponse != null)
                    _eventsGetResponsePrev = eventsGetResponse;
            }
            catch (Exception ex)
            {
                DebugHelper.WriteLogEntry(ex, Resources.Program_OnTimerTick_Error);
            }
        }

        /// <summary>
        /// Отображение уведомления
        /// </summary>
        /// <param name="aText">Текст для отображения</param>
        /// <param name="aSilent">Иконка</param>
        /// <param name="aStraightToTray">Silent режим</param>
        static void OnNotificationShow(string aText, System.Drawing.Icon aIcon, bool aSilent)
        {
            DebugHelper.WriteLogEntry("OnNotificationShow " + aText + aSilent);

            _notification.Caption = Properties.Resources.Program_OnNtfnShow_Caption;
            _notification.Text = aText;
            _notification.Icon = aIcon;
            _notification.StraightToTray = aSilent;
            _notification.Silent = true;
            _notification.DisplayOn = !aSilent;
            _notification.Visible = true;

            //Get info from the registry about the notification type and the options
            DebugHelper.WriteLogEntry("OnNotificationShow NotificationClass.Get");
            var nc = NotificationClass.Get(NotificationCLSID);
            //Application.DoEvents();

            DebugHelper.WriteLogEntry("OnNotificationShow Vibrate");
            if (!aSilent)
            {
                #region vibrate if need
                if ((nc.Options & NotificationOptions.Vibrate) > 0)
                {
                    try
                    {
                        VibrateLED.Vibrate();
                    }
                    catch (Exception ex)
                    {
                        DebugHelper.WriteLogEntry(ex, "Vibrate error");
                    }
                }
                #endregion

                DebugHelper.WriteLogEntry("OnNotificationShow Sound");

                #region play music if need
                try
                {
                    if ((nc.Options & NotificationOptions.Sound) > 0)
                    {
                        if (!String.IsNullOrEmpty(nc.WaveFile))
                        {
                            if (PlatformDetection.IsWM5())
                            {
                                WCE_PlaySound(nc.WaveFile, IntPtr.Zero, (int)(Flags.SND_FILENAME | Flags.SND_ASYNC));
                            }
                            else
                            {
                                string ext = null;
                                try
                                {
                                    ext = Path.GetExtension(nc.WaveFile);
                                }
                                catch (ArgumentException)
                                {
                                }

                                int esuccess = 0;
                                switch (ext)
                                {
                                    //case ".wav":
                                    //    esuccess = WCE_PlaySound(nc.WaveFile, IntPtr.Zero, (int)(Flags.SND_FILENAME | Flags.SND_ASYNC));
                                    //    break;
                                    case ".wav":
                                    case ".mp3":
                                    case ".wma":
                                    case ".mid":
                                    case ".midi":
                                        //Application.DoEvents();
                                        using (SoundPlayer player = new SoundPlayer(nc.WaveFile))
                                        {
                                            player.Play();
                                            esuccess = 1;
                                        }
                                        break;
                                }

                                if (esuccess == 0)
                                {
                                    //WCE_PlaySound("Default", IntPtr.Zero, (int)(Flags.SND_ALIAS_ID | Flags.SND_ASYNC));
                                    WCE_PlaySound("\\Windows\\asterisk.wav", IntPtr.Zero, (int)(Flags.SND_FILENAME | Flags.SND_ASYNC));
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteLogEntry(ex, "Play sound error");
                }

                #endregion
            }
            DebugHelper.WriteLogEntry("OnNotificationShow End");
        }

        /// <summary>
        /// Открытие приложения для просмотра событий
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void OnNotificationRightSoftKeyClick(object sender, EventArgs e)
        {
            try
            {
                var proc = new Process();
                proc.StartInfo.FileName = SystemConfiguration.AppInstallPath + "\\VKontakteWM.exe";
                proc.StartInfo.UseShellExecute = true;
                proc.Start();

                if (NotifyAbortFlag == 1)
                {
                    DebugHelper.WriteLogEntry("Close notificator");
                    Interprocess.StopService();
                }
            }
            catch (Exception ex)
            {
                DebugHelper.WriteLogEntry(String.Format("{0}: {1}", Properties.Resources.Program_OnNtfnRSoftKey_Error, ex.Message));
            }
        }
    }
}
