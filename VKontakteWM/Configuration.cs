using Galssoft.VKontakteWM.Common;

namespace Galssoft.VKontakteWM
{
    public enum ImageMaxSizeTypes
    {
        Res640X480, //default
        DoNotChange
    }

    public enum BackgroundNotificationTypes
    {
        In5Min,
        In10Min, 
        InHour,
        Off,//default
        OnStart
    }

    public enum DataRenewVariants
    {
        AutoUpdateAtStart,
        DontUpdate,
        UpdateAlways//Default
    }

	/// <summary>
	/// Класс настроек привязанный к Configuration.xml
	/// </summary>
	public static class Configuration
	{
		#region Private vars

		#endregion

		#region Properties

                //Автообновление при старте
        public static bool AutoUpdateAtStart { get; set; }

        //Закачивать статусы друзей
        public static bool UpdateFriendsStatus { get; set; }

        //Максимальный размер
        public static ImageMaxSizeTypes ImageMaxSize { get; set; }

        //Фоновые уведомления
        // Нотификатор отключён по просьбе пользователей - сильно садит батарейку и кушает много траффика
        //public static BackgroundNotificationTypes BackgroundNotification { get; set; }
	    public static BackgroundNotificationTypes BackgroundNotification
	    {
	        get
	        {
	            return BackgroundNotificationTypes.Off;
	        }
            set
            {
                value = BackgroundNotificationTypes.Off;
            }
	    }

        //Тип обновления данных при работе
        public static DataRenewVariants DataRenewType { get; set; }

        //Обновление в роуминге
        public static bool RenewInRoam { get; set; }

        //Только через Wi-Fi
        public static bool OnlyWiFi { get; set; }

        //Показывать аватары
        public static bool ShowAvatars { get; set; }
        
        /*
        //Напоминать про дни рождения
        public static bool NotificateFriendsBirthdays { get; set; }
        */

        //Фоновая проверка Сообщений
        public static bool TraceMessagess { get; set; }

        //Фоновая проверка Комментариев
        public static bool TraceComments { get; set; }

        //Фоновая проверка овых
        public static bool TraceFriends { get; set; }

        //Фоновая проверка Уведомления
        public static bool TraceFriendsNews { get; set; }

        //Фоновая проверка Действия друзей
        public static bool TraceFriendsPhotos { get; set; }

        //Фоновая проверка Обсуждения
        public static bool TraceWallMessages { get; set; }

		#endregion

		#region Constructor

	    #endregion

		#region Load/Save methods

        public static void SaveConfigSettings()
        {
            //AutoUpdateAtStart
            Globals.BaseLogic.IDataLogic.SetDownloadAtStart(AutoUpdateAtStart ? "1" : "0");

            //OnlyWiFi
            Globals.BaseLogic.IDataLogic.SetOnlyWIFI(OnlyWiFi ? "1" : "0");

            //UpdateFriendsStatus
            Globals.BaseLogic.IDataLogic.SetUpdateFriendsStatus(UpdateFriendsStatus ? "1" : "0");

            //ImageMaxSize
            switch (ImageMaxSize)
            {
                case ImageMaxSizeTypes.DoNotChange:
                    Globals.BaseLogic.IDataLogic.SetImageMaxSize("1");
                    break;
                case ImageMaxSizeTypes.Res640X480:
                    Globals.BaseLogic.IDataLogic.SetImageMaxSize("2");
                    break;
            }

            //DataRenewType
            switch (DataRenewType)
            {
                case DataRenewVariants.DontUpdate:
                    Globals.BaseLogic.IDataLogic.SetDataRenewType("1");
                    break;
                case DataRenewVariants.AutoUpdateAtStart:
                    Globals.BaseLogic.IDataLogic.SetDataRenewType("2");
                    break;
                case DataRenewVariants.UpdateAlways:
                    Globals.BaseLogic.IDataLogic.SetDataRenewType("3");
                    break;
            }

            //BackgroundNotification
            switch (BackgroundNotification)
            {
                case BackgroundNotificationTypes.Off:
                    Globals.BaseLogic.IDataLogic.SetBackgroundNotification("1");
                    Globals.BaseLogic.IDataLogic.SetNotificationTimer("");
                    break;
                case BackgroundNotificationTypes.In5Min:
                    Globals.BaseLogic.IDataLogic.SetBackgroundNotification("2");
                    Globals.BaseLogic.IDataLogic.SetNotificationTimer("300000");
                    break;
                case BackgroundNotificationTypes.In10Min:
                    Globals.BaseLogic.IDataLogic.SetBackgroundNotification("3");
                    Globals.BaseLogic.IDataLogic.SetNotificationTimer("600000");
                    break;
                case BackgroundNotificationTypes.InHour:
                    Globals.BaseLogic.IDataLogic.SetBackgroundNotification("4");
                    Globals.BaseLogic.IDataLogic.SetNotificationTimer("3600000");
                    break;
            }

            //RenewInRoam
            Globals.BaseLogic.IDataLogic.SetInRoumingValue(RenewInRoam ? "1" : "0");

            /*
            //NotificateFriendsBirthdays
            Globals.BaseLogic.IDataLogic.SetNotificateFriendsBirthdays(NotificateFriendsBirthdays ? "1" : "0");
            */

            //TraceMessagess
            Globals.BaseLogic.IDataLogic.SetTraceMessages();
            
            //TraceComments
            if (TraceComments)
                Globals.BaseLogic.IDataLogic.SetTraceComments();
            else
                Globals.BaseLogic.IDataLogic.SetUntraceComments();

            //TraceFriends
            if (TraceFriends)
                Globals.BaseLogic.IDataLogic.SetTraceFriends();
            else
                Globals.BaseLogic.IDataLogic.SetUntraceFriends();

            //TraceFriendsNews
            if (TraceFriendsNews)
                Globals.BaseLogic.IDataLogic.SetTraceFriendsNews();
            else
                Globals.BaseLogic.IDataLogic.SetUntraceFriendsNews();

            //TraceFriendsPhotos
            if (TraceFriendsPhotos)
                Globals.BaseLogic.IDataLogic.SetTraceFriendsPhotos();
            else
                Globals.BaseLogic.IDataLogic.SetUntraceFriendsPhotos();

            //TraceWallMessages
            if (TraceWallMessages)
                Globals.BaseLogic.IDataLogic.SetTraceWallMessages();
            else
                Globals.BaseLogic.IDataLogic.SetUntraceWallMessages();
        }

        public static void LoadConfigSettings()
        {
            //AutoUpdateAtStart
            string autoUpdateAtStart = Globals.BaseLogic.IDataLogic.GetDownloadAtStart();
            AutoUpdateAtStart = (autoUpdateAtStart == "1");
            //default
            if (autoUpdateAtStart == "") AutoUpdateAtStart = true;

            //OnlyWiFi
            string onlyWifi = Globals.BaseLogic.IDataLogic.GetOnlyWIFI();
            OnlyWiFi = (onlyWifi == "1");
            //default
            if (onlyWifi == "") OnlyWiFi = false;

            /*
            //UpdateFriendsStatus
            string updateFriendsStatus = Globals.BaseLogic.IDataLogic.GetUpdateFriendsStatus();
            UpdateFriendsStatus = updateFriendsStatus == "1";
            //default
            if (updateFriendsStatus == "") UpdateFriendsStatus = true;

            //ImageMaxSize
            string imageMaxSize = Globals.BaseLogic.IDataLogic.GetImageMaxSize();
            if (imageMaxSize == "1")
                ImageMaxSize = ImageMaxSizeTypes.DoNotChange;
            else if (imageMaxSize == "2")
                ImageMaxSize = ImageMaxSizeTypes.Res640X480;
            */

            //BackgroundNotification
            string backgroundNotification = Globals.BaseLogic.IDataLogic.GetBackgroundNotification();
            if (backgroundNotification == "1")
                BackgroundNotification = BackgroundNotificationTypes.Off;
            else if (backgroundNotification == "2")
                BackgroundNotification = BackgroundNotificationTypes.In5Min;
            else if (backgroundNotification == "3")
                BackgroundNotification = BackgroundNotificationTypes.In10Min;
            else if (backgroundNotification == "4")
                BackgroundNotification = BackgroundNotificationTypes.InHour;
            else
                BackgroundNotification = BackgroundNotificationTypes.Off;

            //DataRenewType
            string dataRenewType = Globals.BaseLogic.IDataLogic.GetDataRenewType();
            switch (dataRenewType)
            {
                case "1":
                    DataRenewType = DataRenewVariants.DontUpdate; break;
                case "2":
                    DataRenewType = DataRenewVariants.AutoUpdateAtStart; break;
                case "3":
                    DataRenewType = DataRenewVariants.UpdateAlways; break;
                default:
                    DataRenewType = DataRenewVariants.AutoUpdateAtStart; break;
            }

            //RenewInRoam
            string renewInRoam = Globals.BaseLogic.IDataLogic.GetInRoumingValue();
            RenewInRoam = renewInRoam == "1";
            //default
            if (renewInRoam == "") RenewInRoam = false;

            //ShowAvatars

            /*
            //NotificateFriendsBirthdays
            string notificateFriendsBirthdays = Globals.BaseLogic.IDataLogic.GetNotificateFriendsBirthdays();
            NotificateFriendsBirthdays = notificateFriendsBirthdays == "1";
            //default
            if (notificateFriendsBirthdays == "") NotificateFriendsBirthdays = true;
            */
            /*
            //TraceMessagess
            //На всякий случай
            TraceMessagess = true;
            //TraceComments
            TraceComments = Globals.BaseLogic.IDataLogic.GetTraceComments();
            //TraceFriends
            TraceFriends = Globals.BaseLogic.IDataLogic.GetTraceFriends();
            //TraceFriendsNews
            TraceFriendsNews = Globals.BaseLogic.IDataLogic.GetTraceFriendsNews();
            //TraceFriendsPhotos
            TraceFriendsPhotos = Globals.BaseLogic.IDataLogic.GetTraceFriendsPhotos();
            //TraceDiscussions
            TraceWallMessages = Globals.BaseLogic.IDataLogic.GetTraceWallMessages();
            */
        }

	    #endregion
	}
}
