using System;

using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml;
using Galssoft.VKontakteWM.Components.SystemHelpers;

namespace Galssoft.VKontakteWM.Components.Configuration
{
	/// <summary>
	/// Класс системных настроек.
	/// </summary>
	public static class SystemConfiguration
	{
		#region Private vars

		private static string _appInstallPath;
		private static string _applicationName;
	    private static string _logFileName = "ApplicationLog.txt";
	    private static LogEntryType _logLevel = LogEntryType.Error;
	    private static Logger _log;        

        private static void LoadConfig(string configFileName)
        {
            var xmlReader = new XmlTextReader(_appInstallPath + @"\" + configFileName);

            while (xmlReader.Read())
            {
                if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "add"))
                {
                    string key = xmlReader.GetAttribute("key");
                    string val = xmlReader.GetAttribute("value");
                    
                    switch (key)
                    {
                        case "logfilename":
                            _logFileName = val;
                            break;
                        case "loglevel":
                            try
                            {
                                _logLevel = (LogEntryType)Int32.Parse(val);
                            }
                            catch (Exception)
                            {
                                _logLevel = LogEntryType.Error;
                            }
                            break;
                    }
                }
            }

            _log = new Logger(_logFileName, _logLevel);
        }

	    #endregion

		#region Public properties

        public static string SessionKey { get; set; }
        public static string SessionSecretKey { get; set; }
        public static string Uid { get; set; }
        public static bool DeleteOnExit { get; set; }

        public static string photoRHash { get; set; }
        public static string photoHash { get; set; }
        public static string photoUploadAddress { get; set; }
        public static string avatarRHash { get; set; }
        public static string avatarHash { get; set; }
	    public static string avatarUploadAddress { get; set; }
        public static string Aid { get; set; }

		/// <summary>
		/// Name of the application in the TaskManager apps list
		/// </summary>
		public static string ApplicationName
		{
			get { return _applicationName; }
		}

        public static bool IsSandBox { get; set; }

        public static string ServerConnectionToLogin
        {
            get { return "http://login.userapi.com"; }
        }

        public static string ServerConnectionToApiCalls
        {
            get { return "http://userapi.com"; }
        }

        #region Ключи реестра

        // Ключ реестра VKontakte
	    public static string CommonRegKey
	    {
            get { return "VKontakte"; }
	    }

        // Ключ реестра VKontakte\\events
	    public static string EventsRegKey
	    {
            get { return "VKontakte\\events"; }
	    }

        // Ключ реестра VKontakte\\events\\messages
	    public static string MessagesRegKey
	    {
            get { return "VKontakte\\events\\messages"; }
	    }

        // Ключ реестра VKontakte\\events\\comments
	    public static string CommentsRegKey
	    {
            get { return "VKontakte\\events\\comments"; }
	    }

        // Ключ реестра VKontakte\\events\\friends
	    public static string FriendsRegKey
	    {
            get { return "VKontakte\\events\\friends"; }
	    }

        // Ключ реестра VKontakte\\events\\friendsnews
	    public static string FriendsNewsRegKey
	    {
            get { return "VKontakte\\events\\friendsnews"; }
	    }

        // Ключ реестра VKontakte\\events\\friendsphotos
	    public static string FriendsPhotosRegKey
	    {
            get { return "VKontakte\\events\\friendsphotos"; }
	    }

        // Ключ реестра VKontakte\\events\\wallmessages
	    public static string WallMessagesRegKey
	    {
            get { return "VKontakte\\events\\wallmessages"; }
	    }

        // Ключ реестра VKontakte\\UplPhtView
	    public static string UplPhtRegKey
	    {
            get { return "VKontakte\\UplPhtView"; }
        }

        #endregion

        /// <summary>
        /// Значение задержки (Timeout) при выполнении высокоприоритетных запросов на сервер
        /// </summary>
        public static int ServerConnectionTimeOutLong
        {
            get { return 1000 * 60 * 2; }
        }

        /// <summary>
        /// Значение задержки (Timeout) при выполнении низкоприоритетных запросов на сервер
        /// </summary>
        public static int ServerConnectionTimeOutShort
        {
            get { return 1000 * 60 * 2; }
        }

        public static string api_id
        {
            get
            {
                return "1";
            }
        }

		/// <summary>
		/// Версия продукта. 
		/// </summary>
		public static Version CurrentProductVersion
		{
			get
			{
				return Assembly.GetCallingAssembly().GetName().Version;
			}
		}

		/// <summary>
		/// Версия продукта в формате "Major.Minor.Build"
		/// </summary>
		public static string FullVersionString
		{
			get
			{
				Version assemblyVersion = CurrentProductVersion;
				return string.Format("{0}.{1}.{2}", assemblyVersion.Major, assemblyVersion.Minor, assemblyVersion.Build);
			}
		}

		/// <summary>
		/// Путь к папке с приложением
		/// </summary>
		public static string AppInstallPath
		{
			get { return _appInstallPath; }
		}

		/// <summary>
		/// Путь к файлу логирования.
		/// </summary>
		public static string LogFilePath
		{
			get { return AppInstallPath + @"\" + _logFileName; }
		}

        /// <summary>
        /// Уровень вывода ошибок логирования (от NoLogging,  когда лог не ведётся до DebugInfo, когда выводится отладочная информация)
        /// </summary>
	    public static LogEntryType LogLevel
	    {
            get { return _logLevel; }
	    }

        /// <summary>
        /// Логгер (создаётся после инициализации)
        /// </summary>
	    public static Logger Log
	    {
            //FIXME? возможно, стоит перенести со временем в другое место, если будет создан статический класс
            // для синглтонов приложения. Пока SystemConfiguration доступен из всего кода приложения
            // Поскольку файл лога при записи ошибки открывается и закрывается (объект хранит имя файла),
            // то использование класса SystemConfiguration одновременно в приложении и службе нотификации
            // при одинаковом конфиге позволит одновременно писать в один лог им обоим
	        get { return _log;}
	    }


        /// <summary>
        /// Ссылка для осуществления обновления
        /// </summary>
        /// 
        public static string UpdateVersionURL
        {
            get { return "http://vkontakte-wm.galssoft.ru/vk_update.txt"; }
        }

        /// <summary>
        /// Ссылка для осуществления обновления
        /// </summary>
        /// 
        public static string SupportSiteURL
        {
            get { return "http://vkontakte-wm.galssoft.ru"; }
        }
		#endregion

		#region Constructors

		static SystemConfiguration()
		{
			_appInstallPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
		}

		/// <summary>
		/// Initialize configuration
		/// </summary>
		/// <param name="applicationName">Name of the application in the TaskManager apps list</param>
        /// <param name="configFileName">Локальное имя файла конфигурации в папке приложения</param>
		public static void Init(string applicationName, string configFileName)
		{
			_applicationName = applicationName;

            //FIXME? подумать, что делать при выбросе исключения
            // возможно, файл с конфигом и не нужен, если параметры не будут добавляться
            if (File.Exists(_appInstallPath + @"\" + configFileName))
                LoadConfig(configFileName);
		}

		#endregion

	}
}
