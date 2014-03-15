using System;
using System.IO;
using System.Text;
using Galssoft.VKontakteWM.Components.Common.ResponseClasses;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.ResponseClasses;
using Galssoft.VKontakteWM.Components.SystemHelpers;
using Microsoft.Win32;
using Galssoft.VKontakteWM.Components.Configuration;

namespace Galssoft.VKontakteWM.Components.Data
{
    public class DataLogic : IDataLogic
    {

        #region AutorunMethods

        /// <summary>
        /// Проверка существования ключа автозапуска в реестре
        /// </summary>
        /// <returns></returns>
        public bool GetNtfAutorun()
        {
            RegistryKey getNtfKey = Registry.LocalMachine.OpenSubKey("init\\VKontakteWM");
            if (getNtfKey != null)
            {
                try
                {
                    if ((string) getNtfKey.GetValue("Launch158") != null)
                        return true;
                }
                catch (Exception ex)
                {
                    return false;
                }               
            }
            return false;
        }

        /// <summary>
        /// Включение автозагрузки нотификации
        /// </summary>
        public void SetNtfAutorun()
        {
            RegistryKey setNtfKey = null;

            if ((setNtfKey = Registry.LocalMachine.CreateSubKey("init")) != null)
            {
                try
                {
                    setNtfKey.SetValue("Launch158", Path.Combine(SystemConfiguration.AppInstallPath, "vkontaktewm.notification.exe"), RegistryValueKind.String);
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteLogEntry(ex, "Не удалось прописать автозапуск нотификатора в реестр");
                }                
            }
            else
            {
                DebugHelper.WriteLogEntry("Не удалось прописать автозапуск нотификатора в реестр");
            }
        }

        /// <summary>
        /// Отключение автозагузки нотификации
        /// </summary>
        public void DelNtfAutorun()
        {
            RegistryKey delNtfKey = null;
            if ((delNtfKey = Registry.LocalMachine.OpenSubKey("init", true)) != null)
            {
                try
                {
                    delNtfKey.DeleteValue("Launch158");
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteLogEntry(ex, "Не удалось прописать автозапуск нотификатора в реестр");
                }                      
            }
            else
            {
                DebugHelper.WriteLogEntry("Не удалось прописать автозапуск нотификатора в реестр");
            }
        }

        #endregion

        #region RegistrySettings

        #region Get's
        public byte[] GetCSPBlobValue()
        {
            byte[] val = (byte[]) RegistryUtility.GetBinaryValue("CSPBlob", SystemConfiguration.CommentsRegKey);
            return val;
        }

        public string GetUpdateValue()
        {
            string val = string.Empty;
            if ((val = RegistryUtility.GetValue("UpdateValue", SystemConfiguration.CommonRegKey)) != "")
            {
                return val;
            }
            else
            {
                SetUpdateValue("3");
                return RegistryUtility.GetValue("UpdateValue", SystemConfiguration.CommonRegKey);
            }
        }

        public string GetLoadPhotoValue()
        {
            string val = string.Empty;
            if ((val = RegistryUtility.GetValue("LoadPhotoValue", SystemConfiguration.CommonRegKey)) != "")
            {
                return val;
            }
            else
            {
                SetLoadPhotoValue("1");
                return RegistryUtility.GetValue("LoadPhotoValue", SystemConfiguration.CommonRegKey);
            }
        }

        public string GetInRoumingValue()
        {
            string val = string.Empty;
            if ((val = RegistryUtility.GetValue("InRoumingValue", SystemConfiguration.CommonRegKey)) != "")
            {
                return val;
            }
            else
            {
                SetInRoumingValue("0");
                return RegistryUtility.GetValue("InRoumingValue", SystemConfiguration.CommonRegKey);
            }
        }


        #endregion

        #region Set's      
        public void SetCSPBlobValue(byte[] val)
        {
            RegistryUtility.SetBinaryValue("CSPBlob", SystemConfiguration.CommentsRegKey, val, RegistryValueKind.Binary);
        }

        public void SetUpdateValue(string val)
        {
            try
            {
                RegistryUtility.SetValue("UpdateValue", SystemConfiguration.CommonRegKey, val, RegistryValueKind.String);
            }
            catch (Exception)
            {
                DebugHelper.WriteLogEntry("Не удалось установить в реестре интервал обновления");
            }
        }

        public void SetInRoumingValue(string val)
        {
            try
            {
                RegistryUtility.SetValue("InRoumingValue", SystemConfiguration.CommonRegKey, val, RegistryValueKind.String);
            }
            catch (Exception)
            {
                DebugHelper.WriteLogEntry("Не удалось установить в реестре значение параметра необходимости обновления в роуминге");
            }
        }

        public void SetLoadPhotoValue(string val)
        {
            try
            {
                RegistryUtility.SetValue("LoadPhotoValue", SystemConfiguration.CommonRegKey, val, RegistryValueKind.String);
            }
            catch (Exception)
            {
                DebugHelper.WriteLogEntry("Не удалось установить в реестре значение параметра необходимости загрузки картинок");
            }
        }
        #endregion

        #endregion

        #region GetMethods

        /// <summary>
        /// Возвращает токен из реестра
        /// </summary>
        /// <returns></returns>
        public string GetToken()
        {
            //var byteConverter = new UnicodeEncoding();
            var byteConverter = new UTF8Encoding();

            if (RegistryUtility.GetBinaryValue("Token", SystemConfiguration.CommonRegKey) == null)
            {
                return string.Empty;
            }

            try
            {
                var val = RegistryUtility.GetBinaryValue("Token", SystemConfiguration.CommonRegKey);

                byte[] encryptedValue = (byte[])RegistryUtility.GetBinaryValue("Token", SystemConfiguration.CommonRegKey);

                string regVal = null;

                if (encryptedValue != null)
                {
                    byte[] decryptedValue = CryptoServiceProvider.RSADecrypt(encryptedValue);

                    if (decryptedValue != null)
                    {
                        regVal = byteConverter.GetString(decryptedValue, 0, decryptedValue.Length);
                    }
                    else
                    {
                        regVal = string.Empty;
                    }
                }

                return regVal;
            }
            catch (Exception ex)
            {
                DebugHelper.WriteLogEntry("GetToken error " + ex.Message + "\r\n" + ex.StackTrace);

                return string.Empty;
            }
        }

        /// <summary>
        /// Возвращает ID авторизовавшегося пользователя из реестра
        /// </summary>
        /// <returns></returns>
        public string GetUid()
        {
            return RegistryUtility.GetValue("Uid", SystemConfiguration.CommonRegKey);
            //return SystemConfiguration.Uid;
        }

        /// <summary>
        /// Возвращает сохраненный логин из реестра
        /// </summary>
        /// <returns></returns>
        public string GetSavedLogin()
        {
            return RegistryUtility.GetValue("Login", SystemConfiguration.CommonRegKey);
        }

        /// <summary>
        /// Возвращает сохраненный пароль из реестра
        /// </summary>
        /// <returns></returns>
        public byte[] GetSavedCryptoPass()
        {
            return (byte[]) RegistryUtility.GetBinaryValue("Pass", SystemConfiguration.CommonRegKey);
        }

        /// <summary>
        /// Возвращает сохраненный сессионный ключ из реестра
        /// </summary>
        /// <returns></returns>
        public string GetSessionKey()
        {
            return SystemConfiguration.SessionKey;
        }

        /// <summary>
        /// Возвращает сохраненный секретный сессионный ключ из реестра
        /// </summary>
        /// <returns></returns>
        public string GetSessionSecretKey()
        {
            return SystemConfiguration.SessionSecretKey;
        }

        /// <summary>
        /// Возвращает хэш для загрузки фото в альбом
        /// </summary>
        /// <returns></returns>
        public string GetPhotoHash()
        {
            return SystemConfiguration.photoHash;
        }

        /// <summary>
        /// Возвращает обратный хэш для загрузки фото в альбом
        /// </summary>
        /// <returns></returns>
        public string GetPhotoRHash()
        {
            return SystemConfiguration.photoRHash;
        }

        /// <summary>
        /// Возвращает адрес для загрузки фото в альбом
        /// </summary>
        /// <returns></returns>
        public string GetPhotoUploadAddress()
        {
            return SystemConfiguration.photoUploadAddress;
        }

        /// <summary>
        /// Возвращает идентификатор альбома для загрузки фото в альбом
        /// </summary>
        /// <returns></returns>
        public string GetAid()
        {
            return SystemConfiguration.Aid;
        }

        /// <summary>
        /// Возвращает хэш для загрузки фото на аватар
        /// </summary>
        /// <returns></returns>
        public string GetAvatarHash()
        {
            return SystemConfiguration.avatarHash;
        }

        /// <summary>
        /// Возвращает обратный хэш для загрузки фото на аватар
        /// </summary>
        /// <returns></returns>
        public string GetAvatarRHash()
        {
            return SystemConfiguration.avatarRHash;
        }

        /// <summary>
        /// Возвращает адрес для загрузки фото на аватар
        /// </summary>
        /// <returns></returns>
        public string GetAvatarUploadAddress()
        {
            return SystemConfiguration.avatarUploadAddress;
        }

        /// <summary>
        /// Возвращает промежуток времени между запросами к серверу нотификации
        /// </summary>
        /// <returns></returns>
        public string GetNotificationTimer()
        {
            return RegistryUtility.GetValue("NotificationTimer", SystemConfiguration.EventsRegKey);
        }

        /// <summary>
        /// Десереализует данные по событиям из реестра в объект класса GetEventsResponse
        /// </summary>
        /// <returns>GetEventsResponse</returns>
        /// <exception>исключения, выброшенные RegistryUtility</exception>
        public RawEventsGetResponse GetEvents()
        {
            try
            {
                var eventsGetResponse = new RawEventsGetResponse
                                            {
                                                MessagesCount =
                                                    Convert.ToInt32(RegistryUtility.GetValue("MessagesCount",
                                                                                             SystemConfiguration.EventsRegKey)),
                                                FriendsCount =
                                                    Convert.ToInt32(RegistryUtility.GetValue("FriendsCount",
                                                                                             SystemConfiguration.EventsRegKey)),
                                                CommentsCount =
                                                    Convert.ToInt32(RegistryUtility.GetValue("CommentsCount",
                                                                                             SystemConfiguration.EventsRegKey)),
                                                FriendsNewsCount = 
                                                    Convert.ToInt32(RegistryUtility.GetValue("FriendsNewsCount",
                                                                                             SystemConfiguration.EventsRegKey)),
                                                FriendsPhotosCount = 
                                                    Convert.ToInt32(RegistryUtility.GetValue("FriendsPhotosCount",
                                                                                             SystemConfiguration.EventsRegKey)),
                                                WallCount = 
                                                    Convert.ToInt32(RegistryUtility.GetValue("WallCount",
                                                                                             SystemConfiguration.EventsRegKey))
                                            };

                return eventsGetResponse;
            }
            catch (Exception ex)
            {
                DebugHelper.WriteLogEntry(ex, null);

                return null;
            }
        }

        public EventsGetResponse EventsGet()
        {
            EventsGetResponse eventsGetResponse = new EventsGetResponse();

            try
            {
                DebugHelper.WriteTraceEntry("DataLogic.GetEvents: messages");

                // проверяю наличие записи messages в реестре по одному параметру
                if (!string.IsNullOrEmpty(RegistryUtility.GetValue("MessagesCount", SystemConfiguration.EventsRegKey)))
                {
                    Event eventMessages = new Event();

                    eventMessages.type = EventType.Messages;
                    eventMessages.number = Convert.ToInt32(RegistryUtility.GetValue("MessagesCount", SystemConfiguration.EventsRegKey));
                    
                    eventsGetResponse.events.Add(eventMessages);
                }

                DebugHelper.WriteTraceEntry("DataLogic.GetEvents: comments");

                // проверяю наличие записи comments в реестре по одному параметру
                if (!string.IsNullOrEmpty(RegistryUtility.GetValue("CommentsCount", SystemConfiguration.EventsRegKey)))
                {
                    Event ev = new Event();

                    ev.type = EventType.Comments;
                    ev.number = Convert.ToInt32(RegistryUtility.GetValue("CommentsCount", SystemConfiguration.EventsRegKey));

                    eventsGetResponse.events.Add(ev);
                }

                DebugHelper.WriteTraceEntry("DataLogic.GetEvents: friends");

                // проверяю наличие записи friends в реестре по одному параметру
                if (!string.IsNullOrEmpty(RegistryUtility.GetValue("FriendsCount", SystemConfiguration.EventsRegKey)))
                {
                    Event ev = new Event();

                    ev.type = EventType.Friends;
                    ev.number = Convert.ToInt32(RegistryUtility.GetValue("FriendsCount", SystemConfiguration.EventsRegKey));
                    
                    eventsGetResponse.events.Add(ev);
                }

                DebugHelper.WriteTraceEntry("DataLogic.GetEvents: friends_photos");

                // проверяю наличие записи friendsphotos в реестре по одному параметру
                if (!string.IsNullOrEmpty(RegistryUtility.GetValue("FriendsPhotosCount", SystemConfiguration.EventsRegKey)))
                {
                    Event ev = new Event();

                    ev.type = EventType.FriendsPhotos;
                    ev.number = Convert.ToInt32(RegistryUtility.GetValue("FriendsPhotosCount", SystemConfiguration.EventsRegKey));
                    
                    eventsGetResponse.events.Add(ev);
                }

                DebugHelper.WriteTraceEntry("DataLogic.GetEvents: friends_news");

                // проверяю наличие записи friendsnews в реестре по одному параметру
                if (!string.IsNullOrEmpty(RegistryUtility.GetValue("FriendsNewsCount", SystemConfiguration.EventsRegKey)))
                {
                    Event ev = new Event();

                    ev.type = EventType.FriendsNews;
                    ev.number = Convert.ToInt32(RegistryUtility.GetValue("FriendsNewsCount", SystemConfiguration.EventsRegKey));
                    
                    eventsGetResponse.events.Add(ev);
                }

                DebugHelper.WriteTraceEntry("DataLogic.GetEvents: wall_messages");

                // проверяю наличие записи wallmessages в реестре по одному параметру
                if (!string.IsNullOrEmpty(RegistryUtility.GetValue("WallCount", SystemConfiguration.EventsRegKey)))
                {
                    Event ev = new Event();

                    ev.type = EventType.WallMessages;
                    ev.number = Convert.ToInt32(RegistryUtility.GetValue("WallCount", SystemConfiguration.EventsRegKey));
                    
                    eventsGetResponse.events.Add(ev);
                }
            }
            catch (Exception ex)
            {
                DebugHelper.WriteLogEntry(ex, null);
            }

            return eventsGetResponse;
        }

        /// <summary>
        /// Флаг необходимости обновления списка событий на главной форме
        /// </summary>
        /// <returns></returns>
        public string GetRefreshEventsFlag()
        {
            return RegistryUtility.GetValue("RefreshEventsFlag", SystemConfiguration.CommonRegKey);
        }

        public bool GetTraceMessages()
        {
            string res = RegistryUtility.GetValue("TraceMessages", SystemConfiguration.EventsRegKey);
            if (res == "1")
                return true;
            return false;
        }

        public bool GetTraceComments()
        {
            string res = RegistryUtility.GetValue("TraceComments", SystemConfiguration.EventsRegKey);
            if (res == "1")
                return true;
            return false;
        }

        public bool GetTraceFriends()
        {
            string res = RegistryUtility.GetValue("TraceFriends", SystemConfiguration.EventsRegKey);
            if (res == "1")
                return true;
            return false;
        }

        public bool GetTraceFriendsPhotos()
        {
            string res = RegistryUtility.GetValue("TraceFriends", SystemConfiguration.EventsRegKey);
            if (res == "1")
                return true;
            return false;
        }

        public bool GetTraceFriendsNews()
        {
            string res = RegistryUtility.GetValue("TraceFriendsNews", SystemConfiguration.EventsRegKey);
            if (res == "1")
                return true;
            return false;
        }

        public bool GetTraceWallMessages()
        {
            string res = RegistryUtility.GetValue("TraceWallMessages", SystemConfiguration.EventsRegKey);
            if (res == "1")
                return true;
            return false;
        }

        public string GetImageMaxSize()
        {
            return RegistryUtility.GetValue("ImageMaxSize", SystemConfiguration.CommonRegKey);
        }

        public string GetBackgroundNotification()
        {
            return RegistryUtility.GetValue("BackgroundNotification", SystemConfiguration.CommonRegKey);
        }

        public string GetDataRenewType()
        {
            return RegistryUtility.GetValue("DataRenewType", SystemConfiguration.CommonRegKey);
        }

        public string GetUpdateFriendsStatus()
        {
            return RegistryUtility.GetValue("UpdateFriendsStatus", SystemConfiguration.CommonRegKey);
        }

        /// <summary>
        /// Возвращает флаг необходимости скачивания данных при старте приложения
        /// </summary>
        /// <returns></returns>
        public string GetDownloadAtStart()
        {
            return RegistryUtility.GetValue("DownloadAtStart", SystemConfiguration.CommonRegKey);
        }

        public string GetOnlyWIFI()
        {
            return RegistryUtility.GetValue("OnlyWIFI", SystemConfiguration.CommonRegKey);
        }

        #region ShowEventButtons

        public bool GetShowButtonMessages()
        {
            string res = RegistryUtility.GetValue("ShowButtonMessages", SystemConfiguration.EventsRegKey);

            if (res != "0")
            {
                return true;
            }

            return false;
        }

        public bool GetShowButtonComments()
        {
            string res = RegistryUtility.GetValue("ShowButtonComments", SystemConfiguration.EventsRegKey);

            if (res != "0")
            {
                return true;
            }

            return false;
        }

        public bool GetShowButtonFriends()
        {
            string res = RegistryUtility.GetValue("ShowButtonFriends", SystemConfiguration.EventsRegKey);

            if (res != "0")
            {
                return true;
            }

            return false;
        }

        public bool GetShowButtonFriendsNews()
        {
            string res = RegistryUtility.GetValue("ShowButtonFriendsNews", SystemConfiguration.EventsRegKey);

            if (res != "0")
            {
                return true;
            }

            return false;
        }

        public bool GetShowButtonFriendsPhotos()
        {
            string res = RegistryUtility.GetValue("ShowButtonFriendsPhotos", SystemConfiguration.EventsRegKey);

            if (res != "0")
            {
                return true;
            }

            return false;
        }

        public bool GetShowButtonWallMessages()
        {
            string res = RegistryUtility.GetValue("ShowButtonWallMessages", SystemConfiguration.EventsRegKey);

            if (res != "0")
            {
                return true;
            }

            return false;
        }

        #endregion

        #region для UpdateHelper`а

        /// <summary>
        /// Загрузка LastCheckedVersion
        /// </summary>
        /// <returns></returns>
        public Version GetLastCheckedVersion()
        {
            try
            {
                return new Version(RegistryUtility.GetValue("LastCheckedVersion", "VKontakte"));
            }
            catch
            {
                return new Version(0, 0, 0);
            }
        }

        public string GetLastCheckedVersionStr()
        {
            return RegistryUtility.GetValue("LastCheckedVersion", "VKontakte");
        }

        /// <summary>
        /// Загрузка LastCheckedNewFeatures
        /// </summary>
        /// <returns></returns>
        public string GetLastCheckedVersionInfo()
        {
            string result = RegistryUtility.GetValue("LastCheckedVersionInfo", "VKontakte");

            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Загрузка LastCheckedVersionIsMandatory
        /// </summary>
        /// <returns></returns>
        public bool GetLastCheckedVersionIsMandatory()
        {
            try
            {
                return Convert.ToBoolean(RegistryUtility.GetValue("LastCheckedVersionIsMandatory", "VKontakte"));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Загрузка LastCheckedVersionIsSkipped
        /// </summary>
        /// <returns></returns>
        public Version GetLastCheckedVersionIsSkipped()
        {
            try
            {
                return new Version(RegistryUtility.GetValue("LastCheckedVersionIsSkipped", SystemConfiguration.CommonRegKey));
            }
            catch
            {
                return new Version(0, 0, 0);
            }
        }

        /// <summary>
        /// Загрузка LastCheckedVersionUpdateURL
        /// </summary>
        /// <returns></returns>
        public string GetLastCheckedVersionUpdateURL()
        {
            string result = RegistryUtility.GetValue("LastCheckedVersionUpdateURL", SystemConfiguration.CommonRegKey);

            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion

        #region для UploadPhoto и EditPhoto

        public bool GetUplPhtViewHasMdfPht()
        {
            try
            {
                return Convert.ToBoolean(RegistryUtility.GetValue("HasMdfPht", @"VKontakte\UplPhtView"));
            }
            catch
            {
                return false;
            }
        }

        public OpenNETCF.Drawing.RotationAngle GetUplPhtViewPhtRtnAngl()
        {
            try
            {
                switch (Convert.ToUInt32(RegistryUtility.GetValue("PhtRtnAngl", @"VKontakte\UplPhtView")))
                {
                    case 0:
                        return OpenNETCF.Drawing.RotationAngle.Zero;
                        break;

                    case 90:
                        return OpenNETCF.Drawing.RotationAngle.Clockwise90;
                        break;

                    case 180:
                        return OpenNETCF.Drawing.RotationAngle.Clockwise180;
                        break;

                    case 270:
                        return OpenNETCF.Drawing.RotationAngle.Clockwise270;
                        break;

                    default:
                        throw new Exception();
                        break;
                }
            }
            catch (Exception)
            {
                return OpenNETCF.Drawing.RotationAngle.Zero;
            }
        }

        public string GetUplPhtViewPhtCmnt()
        {
            string result = RegistryUtility.GetValue("ViewPhtCmnt", @"VKontakte\UplPhtView");

            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion
    
        #endregion

        #region SetMethods
        /// <summary>
        /// Записывает в реестр токен
        /// </summary>
        /// <param name="val"></param>
        public void SetToken(string val)
        {
            byte[] encryptedData = new byte[] {};

            if (!string.IsNullOrEmpty(val))
            {
                var byteConverter = new UTF8Encoding();
                byte[] dataToEncrypt = byteConverter.GetBytes(val);

                encryptedData = CryptoServiceProvider.RSAEncrypt(dataToEncrypt);

                RegistryUtility.SetBinaryValue("Token", SystemConfiguration.CommonRegKey, encryptedData, RegistryValueKind.Binary);
            }
            else
            {
                RegistryUtility.SetBinaryValue("Token", SystemConfiguration.CommonRegKey, encryptedData, RegistryValueKind.Binary);
            }
        }

        /// <summary>
        /// Записывает в реестр ID пользователя
        /// </summary>
        /// <param name="val"></param>
        public void SetUid(string val)
        {
            RegistryUtility.SetValue("Uid", SystemConfiguration.CommonRegKey, val, RegistryValueKind.String);
            //SystemConfiguration.Uid = val;
        }
       
        /// <summary>
        /// Задает обратный хэш для загрузки фото в альбом
        /// </summary>
        /// <param name="val"></param>
        public void SetPhotoRHash(string val)
        {
            SystemConfiguration.photoRHash = val;
        }

        /// <summary>
        /// Задает хэш для загрузки фото в альбом
        /// </summary>
        /// <param name="val"></param>
        public void SetPhotoHash(string val)
        {
            SystemConfiguration.photoHash = val;
        }

        /// <summary>
        /// Задает адрес для загрузки фото в альбом
        /// </summary>
        /// <param name="val"></param>
        public void SetPhotoUploadAddress(string val)
        {
            SystemConfiguration.photoUploadAddress = val;
        }

        /// <summary>
        /// Задает хэш для загрузки фото на аватар
        /// </summary>
        /// <param name="val"></param>
        public void SetAvatarHash(string val)
        {
            SystemConfiguration.avatarHash = val;
        }

        /// <summary>
        /// Задает обратный хзш для загрузки фото на аватар
        /// </summary>
        /// <param name="val"></param>
        public void SetAvatarRHash(string val)
        {
            SystemConfiguration.avatarRHash = val;
        }

        /// <summary>
        /// Задает адрес для загрузки фото на аватар
        /// </summary>
        /// <param name="val"></param>
        public void SetAvatarUploadAddress(string val)
        {
            SystemConfiguration.avatarUploadAddress = val;
        }

        /// <summary>
        /// Задает идентификатор альбома для загрузки фото
        /// </summary>
        /// <param name="val"></param>
        public void SetAid(string val)
        {
            SystemConfiguration.Aid = val;
        }

        /// <summary>
        /// Записывает в реестр сессионный ключ
        /// </summary>
        /// <param name="val"></param>
        public void SetSessionKey(string val)
        {
            SystemConfiguration.SessionKey = val;
        }

        /// <summary>
        /// Записывает в реестр секретный сессионный ключ
        /// </summary>
        /// <param name="val"></param>
        public void SetSessionSecretKey(string val)
        {
            SystemConfiguration.SessionSecretKey = val;
        }

        /// <summary>
        /// Записывает промежуток времени между запросами к серверу нотификации
        /// </summary>
        /// <param name="val"></param>
        public void SetNotificationTimer(string val)
        {
            RegistryUtility.SetValue("NotificationTimer", SystemConfiguration.EventsRegKey, val, RegistryValueKind.String);
        }

        /// <summary>
        /// Сохраняет события в реестр. Генерирует исключение в случае неудачи.
        /// </summary>
        /// <param name="eventsGetResponse"></param>
        public void SetEvents(RawEventsGetResponse eventsGetResponse)
        {
            try
            {
                if (eventsGetResponse.MessagesCount > -1)
                {
                    RegistryUtility.SetValue("MessagesCount", SystemConfiguration.EventsRegKey, eventsGetResponse.MessagesCount.ToString(), RegistryValueKind.String);
                }

                if (eventsGetResponse.FriendsCount > -1)
                {
                    RegistryUtility.SetValue("FriendsCount", SystemConfiguration.EventsRegKey, eventsGetResponse.FriendsCount.ToString(), RegistryValueKind.String);
                }

                if (eventsGetResponse.CommentsCount > -1)
                {
                    RegistryUtility.SetValue("CommentsCount", SystemConfiguration.EventsRegKey, eventsGetResponse.CommentsCount.ToString(), RegistryValueKind.String);
                }

                if (eventsGetResponse.FriendsPhotosCount > -1)
                {
                    RegistryUtility.SetValue("FriendsPhotosCount", SystemConfiguration.EventsRegKey, eventsGetResponse.FriendsPhotosCount.ToString(), RegistryValueKind.String);
                }

                if (eventsGetResponse.FriendsNewsCount > -1)
                {
                    RegistryUtility.SetValue("FriendsNewsCount", SystemConfiguration.EventsRegKey, eventsGetResponse.FriendsNewsCount.ToString(), RegistryValueKind.String);
                }

                if (eventsGetResponse.WallCount > -1)
                {
                    RegistryUtility.SetValue("WallCount", SystemConfiguration.EventsRegKey, eventsGetResponse.WallCount.ToString(), RegistryValueKind.String);
                }
            }
            catch (Exception ex)
            {
                DebugHelper.WriteLogEntry(ex, null);
            }
        }

        /// <summary>
        /// Флаг необходимости обновления списка событий на главной форме
        /// </summary>
        /// <param name="val"></param>
        public void SetRefreshEventsFlag(string val)
        {
            RegistryUtility.SetValue("RefreshEventsFlag", SystemConfiguration.CommonRegKey, val, RegistryValueKind.String);
        }

        /// <summary>
        /// Сохраняет удачный логин
        /// </summary>
        /// <param name="val"></param>
        public void SetSavedLogin(string val)
        {
            RegistryUtility.SetValue("Login", SystemConfiguration.CommonRegKey, val, RegistryValueKind.String);
        }

        /// <summary>
        /// Сохраняет удачный пароль
        /// </summary>
        /// <param name="val"></param>
        public void SetSavedCryptoPass(byte[] val)
        {
            RegistryUtility.SetBinaryValue("Pass", SystemConfiguration.CommonRegKey, val, RegistryValueKind.Binary);
        }

        public void SetBackgroundNotification(string val)
        {
            RegistryUtility.SetValue("BackgroundNotification", SystemConfiguration.CommonRegKey, val, RegistryValueKind.String);
        }

        public void SetDataRenewType(string val)
        {
            RegistryUtility.SetValue("DataRenewType", SystemConfiguration.CommonRegKey, val, RegistryValueKind.String);
        }

        public void SetImageMaxSize(string val)
        {
            RegistryUtility.SetValue("ImageMaxSize", SystemConfiguration.CommonRegKey, val, RegistryValueKind.String);
        }

        public void SetUpdateFriendsStatus(string val)
        {
            RegistryUtility.SetValue("UpdateFriendsStatus", SystemConfiguration.CommonRegKey, val, RegistryValueKind.String);
        }

        /// <summary>
        /// Записывает флаг необходимости скачивания данных при старте приложения
        /// </summary>
        /// <param name="val"></param>
        public void SetDownloadAtStart(string val)
        {
            RegistryUtility.SetValue("DownloadAtStart", SystemConfiguration.CommonRegKey, val, RegistryValueKind.String);
        }

        public void SetOnlyWIFI(string val)
        {
            RegistryUtility.SetValue("OnlyWIFI", SystemConfiguration.CommonRegKey, val, RegistryValueKind.String);
        }

        #region set

        #region trace

        public void SetTraceMessages()
        {
            RegistryUtility.SetValue("TraceMessages", SystemConfiguration.EventsRegKey, "1", RegistryValueKind.String);
        }

        public void SetTraceComments()
        {
            RegistryUtility.SetValue("TraceComments", SystemConfiguration.EventsRegKey, "1", RegistryValueKind.String);
        }

        public void SetTraceFriends()
        {
            RegistryUtility.SetValue("TraceFriends", SystemConfiguration.EventsRegKey, "1", RegistryValueKind.String);
        }

        public void SetTraceFriendsPhotos()
        {
            RegistryUtility.SetValue("TraceFriendsPhotos", SystemConfiguration.EventsRegKey, "1", RegistryValueKind.String);
        }

        public void SetTraceFriendsNews()
        {
            RegistryUtility.SetValue("TraceFriendsNews", SystemConfiguration.EventsRegKey, "1", RegistryValueKind.String);
        }

        public void SetTraceWallMessages()
        {
            RegistryUtility.SetValue("TraceWallMessages", SystemConfiguration.EventsRegKey, "1", RegistryValueKind.String);
        }

        #endregion

        #region untrace

        public void SetUntraceMessages()
        {
            RegistryUtility.SetValue("TraceMessages", SystemConfiguration.EventsRegKey, "0", RegistryValueKind.String);
        }

        public void SetUntraceComments()
        {
            RegistryUtility.SetValue("TraceComments", SystemConfiguration.EventsRegKey, "0", RegistryValueKind.String);
        }

        public void SetUntraceFriends()
        {
            RegistryUtility.SetValue("TraceFriends", SystemConfiguration.EventsRegKey, "0", RegistryValueKind.String);
        }

        public void SetUntraceFriendsPhotos()
        {
            RegistryUtility.SetValue("TraceFriendsPhotos", SystemConfiguration.EventsRegKey, "0", RegistryValueKind.String);
        }

        public void SetUntraceFriendsNews()
        {
            RegistryUtility.SetValue("TraceFriendsNews", SystemConfiguration.EventsRegKey, "0", RegistryValueKind.String);
        }

        public void SetUntraceWallMessages()
        {
            RegistryUtility.SetValue("TraceWallMessages", SystemConfiguration.EventsRegKey, "0", RegistryValueKind.String);
        }

        #endregion

        #endregion

        #region для UpdateHelper`а

        #region преобразует любой version в string формата x.x.x

        public string VersionToString(Version version)
        {
            StringBuilder newStringBuilder = new StringBuilder();

            if (version.Major > -1)
            {
                newStringBuilder.Append(version.Major.ToString());

                if (version.Minor > -1)
                {
                    newStringBuilder.Append(".");
                    newStringBuilder.Append(version.Minor.ToString());

                    if (version.Build > -1)
                    {
                        newStringBuilder.Append(".");
                        newStringBuilder.Append(version.Build.ToString());
                    }
                    else
                    {
                        newStringBuilder.Append(".0");
                    }
                }
                else
                {
                    newStringBuilder.Append(".0.0");
                }
            }
            else
            {
                newStringBuilder.Append("0.0.0");
            }

            return newStringBuilder.ToString();
        }

        #endregion

        /// <summary>
        /// Сохраняет LastCheckedVersion
        /// </summary>
        /// <param name="val"></param>        
        public void SetLastCheckedVersion(Version val)
        {
            RegistryUtility.SetValue("LastCheckedVersion", SystemConfiguration.CommonRegKey, VersionToString(val), RegistryValueKind.String);
        }

        /// <summary>
        /// Сохраняет LastCheckedNewFeatures
        /// </summary>
        /// <param name="val"></param>        
        public void SetLastCheckedVersionInfo(string val)
        {
            RegistryUtility.SetValue("LastCheckedVersionInfo", SystemConfiguration.CommonRegKey, val, RegistryValueKind.String);
        }

        /// <summary>
        /// Сохраняет LastCheckedVersionIsMandatory
        /// </summary>
        /// <returns></returns>
        public void SetLastCheckedVersionIsMandatory(bool val)
        {
            RegistryUtility.SetValue("LastCheckedVersionIsMandatory", SystemConfiguration.CommonRegKey, val.ToString(), RegistryValueKind.String);
        }

        /// <summary>
        /// Сохраняет LastCheckedVersionIsSkipped
        /// </summary>
        /// <returns></returns>
        public void SetLastCheckedVersionIsSkipped(Version val)
        {
            RegistryUtility.SetValue("LastCheckedVersionIsSkipped", SystemConfiguration.CommonRegKey, VersionToString(val), RegistryValueKind.String);
        }

        /// <summary>
        /// Сохраняет LastCheckedVersionUpdateURL
        /// </summary>
        /// <returns></returns>
        public void SetLastCheckedVersionUpdateURL(string val)
        {
            RegistryUtility.SetValue("LastCheckedVersionUpdateURL", SystemConfiguration.CommonRegKey, val, RegistryValueKind.String);
        }

        #endregion

        #region для UploadPhoto и EditPhoto

        public void SetUplPhtViewHasMdfPht(bool val)
        {
            if (val == null)
            {
                val = false;
            }

            RegistryUtility.SetValue("HasMdfPht", @"VKontakte\UplPhtView", val.ToString(), RegistryValueKind.String);
        }

        public void SetUplPhtViewPhtRtnAnglZero()
        {
            RegistryUtility.SetValue("PhtRtnAngl", @"VKontakte\UplPhtView", "0", RegistryValueKind.String);
        }

        public void SetUplPhtViewPhtRtnAnglCW()
        {
            OpenNETCF.Drawing.RotationAngle angle = GetUplPhtViewPhtRtnAngl();

            OpenNETCF.Drawing.RotationAngle angleCW = OpenNETCF.Drawing.RotationAngle.Zero;

            switch (angle)
            {
                case OpenNETCF.Drawing.RotationAngle.Zero:
                    angleCW = OpenNETCF.Drawing.RotationAngle.Clockwise90;
                    break;

                case OpenNETCF.Drawing.RotationAngle.Clockwise90:
                    angleCW = OpenNETCF.Drawing.RotationAngle.Clockwise180;
                    break;

                case OpenNETCF.Drawing.RotationAngle.Clockwise180:
                    angleCW = OpenNETCF.Drawing.RotationAngle.Clockwise270;
                    break;

                case OpenNETCF.Drawing.RotationAngle.Clockwise270:
                    angleCW = OpenNETCF.Drawing.RotationAngle.Zero;
                    break;

                default:
                    angleCW = OpenNETCF.Drawing.RotationAngle.Zero;
                    break;
            }

            string result = string.Empty;

            switch (angleCW)
            {
                case OpenNETCF.Drawing.RotationAngle.Zero:
                    result = "0";
                    break;

                case OpenNETCF.Drawing.RotationAngle.Clockwise90:
                    result = "90";
                    break;

                case OpenNETCF.Drawing.RotationAngle.Clockwise180:
                    result = "180";
                    break;

                case OpenNETCF.Drawing.RotationAngle.Clockwise270:
                    result = "270";
                    break;

                default:
                    result = "0";
                    break;
            }

            RegistryUtility.SetValue("PhtRtnAngl", @"VKontakte\UplPhtView", result, RegistryValueKind.String);
        }

        public void SetUplPhtViewPhtRtnAnglCCW()
        {
            OpenNETCF.Drawing.RotationAngle angle = GetUplPhtViewPhtRtnAngl();

            OpenNETCF.Drawing.RotationAngle angleCCW = OpenNETCF.Drawing.RotationAngle.Zero;

            switch (angle)
            {
                case OpenNETCF.Drawing.RotationAngle.Zero:
                    angleCCW = OpenNETCF.Drawing.RotationAngle.Clockwise270;
                    break;

                case OpenNETCF.Drawing.RotationAngle.Clockwise90:
                    angleCCW = OpenNETCF.Drawing.RotationAngle.Zero;
                    break;

                case OpenNETCF.Drawing.RotationAngle.Clockwise180:
                    angleCCW = OpenNETCF.Drawing.RotationAngle.Clockwise90;
                    break;

                case OpenNETCF.Drawing.RotationAngle.Clockwise270:
                    angleCCW = OpenNETCF.Drawing.RotationAngle.Clockwise180;
                    break;

                default:
                    angleCCW = OpenNETCF.Drawing.RotationAngle.Zero;
                    break;
            }

            string result = string.Empty;

            switch (angleCCW)
            {
                case OpenNETCF.Drawing.RotationAngle.Zero:
                    result = "0";
                    break;

                case OpenNETCF.Drawing.RotationAngle.Clockwise90:
                    result = "90";
                    break;

                case OpenNETCF.Drawing.RotationAngle.Clockwise180:
                    result = "180";
                    break;

                case OpenNETCF.Drawing.RotationAngle.Clockwise270:
                    result = "270";
                    break;

                default:
                    result = "0";
                    break;
            }

            RegistryUtility.SetValue("PhtRtnAngl", @"VKontakte\UplPhtView", result, RegistryValueKind.String);
        }

        public void SetUplPhtViewPhtCmnt(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                val = string.Empty;
            }

            RegistryUtility.SetValue("ViewPhtCmnt", @"VKontakte\UplPhtView", val, RegistryValueKind.String);
        }

        #endregion

        #region ShowEventButtons

        public void SetShowButtonMessages()
        {
            RegistryUtility.SetValue("ShowButtonMessages", SystemConfiguration.EventsRegKey, "1", RegistryValueKind.String);
        }

        public void SetShowButtonComments()
        {
            RegistryUtility.SetValue("ShowButtonComments", SystemConfiguration.EventsRegKey, "1", RegistryValueKind.String);
        }

        public void SetShowButtonFriends()
        {
            RegistryUtility.SetValue("ShowButtonFriends", SystemConfiguration.EventsRegKey, "1", RegistryValueKind.String);
        }

        public void SetShowButtonFriendsNews()
        {
            RegistryUtility.SetValue("ShowButtonFriendsNews", SystemConfiguration.EventsRegKey, "1", RegistryValueKind.String);
        }

        public void SetShowButtonFriendsPhotos()
        {
            RegistryUtility.SetValue("ShowButtonFriendsPhotos", SystemConfiguration.EventsRegKey, "1", RegistryValueKind.String);
        }

        public void SetShowButtonWallMessages()
        {
            RegistryUtility.SetValue("ShowButtonWallMessages", SystemConfiguration.EventsRegKey, "1", RegistryValueKind.String);
        }

        #endregion

        #endregion

        /// <summary>
        /// Очищает все закэшированные данные
        /// </summary>
        public void ClearCache()
        {
            try
            {
                // xml
                try
                {                    
                    //Cache.Cache.DeleteEntryFromCache(string.Empty, "MessageShortCorrespondence"); // заголовки цепочек
                    DataModel.Data.MessageShortCorrespondence= null;

                    //Cache.Cache.DeleteEntryFromCache(string.Empty, "MessageCorrespondence"); // цепочки (все)
                    DataModel.Data.MessageCorrespondence = null;

                    Cache.Cache.DeleteEntryFromCache(string.Empty, "ActivityResponse");  // новости
                    Cache.Cache.DeleteEntryFromCache(string.Empty, "PhotosCommentsResponse"); // комментарии к фото
                    Cache.Cache.DeleteEntryFromCache(string.Empty, "FriendsListResponse"); // список друзей
                    Cache.Cache.DeleteEntryFromCache(string.Empty, "LoggedInUserData"); // инфо о пользователе
                    Cache.Cache.DeleteEntryFromCache(string.Empty, "UpdatesPhotosResponse"); // новые фото друзей пользователя
                    Cache.Cache.DeleteEntryFromCache(string.Empty, "DraftMessagesData"); // черновики сообщений
                    Cache.Cache.DeleteEntryFromCache(string.Empty, "PhotosCommentsResponseHistory");
                    Cache.Cache.DeleteEntryFromCache(string.Empty, "UserActivityResponse");
                    //Cache.Cache.DeleteEntryFromCache(string.Empty, "UserActivityResponse");
                    Cache.Cache.DeleteEntryFromCache(string.Empty, "FriendsListAdditionalResponse");
                    Cache.Cache.DeleteEntryFromCache(string.Empty, "UserPhotosResponse");

                    Cache.Cache.DeleteEntryFromCache(string.Empty, "ShortActivityResponse"); // новости (коротко)
                    Cache.Cache.DeleteEntryFromCache(string.Empty, "ShortUpdatesPhotosResponse"); // фото (коротко)
                    Cache.Cache.DeleteEntryFromCache(string.Empty, "ShortWallResponse"); // сообщения на стене (коротко)
                    Cache.Cache.DeleteEntryFromCache(string.Empty, "ShortPhotosCommentsRespounse"); // коментарии к фото (коротко) 
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteLogEntry(ex, "Cache clear error");
                }

                // удаляем всё из папки //Cache//Files
                string path = SystemConfiguration.AppInstallPath + "//Cache//Files";

                if (Directory.Exists(path))
                {
                    foreach (string subDir in Directory.GetDirectories(path))
                    {
                        if (!subDir.Equals(Path.Combine(path, "Thumb")))
                        {
                            try
                            {
                                Directory.Delete(subDir, true);
                            }
                            catch (Exception ex)
                            {
                                DebugHelper.WriteLogEntry(ex, "Files directory clear error");
                            }
                        }
                        else
                        {
                            foreach (string currFile in Directory.GetFiles(subDir))
                            {
                                try
                                {                                                                       
                                    File.Delete(currFile);
                                }
                                catch (Exception ex)
                                {
                                    DebugHelper.WriteLogEntry(ex, "Files directory clear error");
                                }
                            }
                        }
                    }

                    foreach (string currFile in Directory.GetFiles(path))
                    {
                        try
                        {
                            File.Delete(currFile);
                        }
                        catch (Exception ex)
                        {
                            DebugHelper.WriteLogEntry(ex, "Files directory clear error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DebugHelper.WriteLogEntry(ex, null);
            }        
        }

        /// <summary>
        /// Очищает токен и сессионные ключи
        /// </summary>
        public void ClearPass()
        {
            try
            {
                SetToken(string.Empty);
                SetSessionKey(string.Empty);
                SetSessionSecretKey(string.Empty);

                ClearRegistry();
            }
            catch (Exception ex)
            {
                DebugHelper.WriteLogEntry(ex, null);
            }
        }

        private void ClearRegistry()
        {

            RegistryUtility.SetValue("Login", SystemConfiguration.CommonRegKey, string.Empty, RegistryValueKind.String);
            RegistryUtility.SetValue("DownloadAtStart", SystemConfiguration.CommonRegKey, string.Empty, RegistryValueKind.String);
            RegistryUtility.SetValue("NotificationTimer", SystemConfiguration.CommonRegKey, string.Empty, RegistryValueKind.String);            
            RegistryUtility.SetValue("RefreshEventsFlag", SystemConfiguration.CommonRegKey, string.Empty, RegistryValueKind.String);
            RegistryUtility.SetValue("Uid", SystemConfiguration.CommonRegKey, string.Empty, RegistryValueKind.String);

            RegistryUtility.SetValue("LastCheckedVersion", SystemConfiguration.CommonRegKey, string.Empty, RegistryValueKind.String);
            RegistryUtility.SetValue("LastCheckedVersionInfo", SystemConfiguration.CommonRegKey, string.Empty, RegistryValueKind.String);
            RegistryUtility.SetValue("LastCheckedVersionIsMandatory", SystemConfiguration.CommonRegKey, string.Empty, RegistryValueKind.String);
            RegistryUtility.SetValue("LastCheckedVersionIsSkipped", SystemConfiguration.CommonRegKey, string.Empty, RegistryValueKind.String);
            RegistryUtility.SetValue("LastCheckedVersionUpdateURL", SystemConfiguration.CommonRegKey, string.Empty, RegistryValueKind.String);

            RegistryUtility.SetValue("ImageMaxSize", SystemConfiguration.CommonRegKey, string.Empty, RegistryValueKind.String);

            RegistryUtility.SetValue("BackgroundNotification", SystemConfiguration.CommonRegKey, string.Empty, RegistryValueKind.String);

            RegistryUtility.SetValue("DataRenewType", SystemConfiguration.CommonRegKey, string.Empty, RegistryValueKind.String);

            RegistryUtility.SetValue("UpdateFriendsStatus", SystemConfiguration.CommonRegKey, string.Empty, RegistryValueKind.String);

            RegistryUtility.SetValue("RenewInRoam", SystemConfiguration.CommonRegKey, string.Empty, RegistryValueKind.String);            

            RegistryUtility.SetValue("TraceMessages", SystemConfiguration.EventsRegKey, string.Empty, RegistryValueKind.String);
            RegistryUtility.SetValue("TraceComments", SystemConfiguration.EventsRegKey, string.Empty, RegistryValueKind.String);
            RegistryUtility.SetValue("TraceFriends", SystemConfiguration.EventsRegKey, string.Empty, RegistryValueKind.String);
            RegistryUtility.SetValue("TraceFriendsNews", SystemConfiguration.EventsRegKey, string.Empty, RegistryValueKind.String);
            RegistryUtility.SetValue("TraceFriendsPhotos", SystemConfiguration.EventsRegKey, string.Empty, RegistryValueKind.String);
            RegistryUtility.SetValue("TraceWallMessages", SystemConfiguration.EventsRegKey, string.Empty, RegistryValueKind.String);

            RegistryUtility.SetValue("ShowButtonMessages", SystemConfiguration.EventsRegKey, string.Empty, RegistryValueKind.String);
            RegistryUtility.SetValue("ShowButtonGuests", SystemConfiguration.EventsRegKey, string.Empty, RegistryValueKind.String);
            RegistryUtility.SetValue("ShowButtonMarks", SystemConfiguration.EventsRegKey, string.Empty, RegistryValueKind.String);
            RegistryUtility.SetValue("ShowButtonNotifications", SystemConfiguration.EventsRegKey, string.Empty, RegistryValueKind.String);
            RegistryUtility.SetValue("ShowButtonActivities", SystemConfiguration.EventsRegKey, string.Empty, RegistryValueKind.String);
            RegistryUtility.SetValue("ShowButtonDiscussions", SystemConfiguration.EventsRegKey, string.Empty, RegistryValueKind.String);

            RegistryUtility.SetValue("number", SystemConfiguration.MessagesRegKey, string.Empty, RegistryValueKind.String);
            RegistryUtility.SetValue("lastId", SystemConfiguration.MessagesRegKey, string.Empty, RegistryValueKind.String);

            RegistryUtility.SetValue("number", SystemConfiguration.CommentsRegKey, string.Empty, RegistryValueKind.String);
            RegistryUtility.SetValue("lastId", SystemConfiguration.CommentsRegKey, string.Empty, RegistryValueKind.String);

            RegistryUtility.SetValue("number", SystemConfiguration.FriendsRegKey, string.Empty, RegistryValueKind.String);
            RegistryUtility.SetValue("lastId", SystemConfiguration.FriendsRegKey, string.Empty, RegistryValueKind.String);

            RegistryUtility.SetValue("number", SystemConfiguration.FriendsNewsRegKey, string.Empty, RegistryValueKind.String);
            RegistryUtility.SetValue("lastId", SystemConfiguration.FriendsNewsRegKey, string.Empty, RegistryValueKind.String);

            RegistryUtility.SetValue("number", SystemConfiguration.FriendsPhotosRegKey, string.Empty, RegistryValueKind.String);
            RegistryUtility.SetValue("lastId", SystemConfiguration.FriendsPhotosRegKey, string.Empty, RegistryValueKind.String);

            RegistryUtility.SetValue("number", SystemConfiguration.WallMessagesRegKey, string.Empty, RegistryValueKind.String);
            RegistryUtility.SetValue("lastId", SystemConfiguration.WallMessagesRegKey, string.Empty, RegistryValueKind.String);

            RegistryUtility.SetValue("HasMdfPht", SystemConfiguration.UplPhtRegKey, string.Empty, RegistryValueKind.String);
            RegistryUtility.SetValue("PhtRtnAngl", SystemConfiguration.UplPhtRegKey, string.Empty, RegistryValueKind.String);
            RegistryUtility.SetValue("ViewPhtCmnt", SystemConfiguration.UplPhtRegKey, string.Empty, RegistryValueKind.String);

            RegistryUtility.SetValue("Uid", SystemConfiguration.CommonRegKey, string.Empty, RegistryValueKind.String);
            RegistryUtility.SetValue("InRoumingValue", SystemConfiguration.CommonRegKey, string.Empty, RegistryValueKind.String);
            RegistryUtility.SetBinaryValue("CSPBlob", SystemConfiguration.CommonRegKey, new byte[] { }, RegistryValueKind.Binary);

            ClearEvents();
        }

        /// <summary>
        /// Затирает поля
        /// </summary>
        private void ClearEvents()
        {
            try
            {
                RegistryUtility.SetValue("MessagesCount", SystemConfiguration.EventsRegKey, string.Empty, RegistryValueKind.String);
                RegistryUtility.SetValue("FriendsCount", SystemConfiguration.EventsRegKey, string.Empty, RegistryValueKind.String);
                RegistryUtility.SetValue("CommentsCount", SystemConfiguration.EventsRegKey, string.Empty, RegistryValueKind.String);
                RegistryUtility.SetValue("FriendsPhotosCount", SystemConfiguration.EventsRegKey, string.Empty, RegistryValueKind.String);
                RegistryUtility.SetValue("FriendsNewsCount", SystemConfiguration.EventsRegKey, string.Empty, RegistryValueKind.String);
                RegistryUtility.SetValue("WallCount", SystemConfiguration.EventsRegKey, string.Empty, RegistryValueKind.String);
            }
            catch (Exception ex)
            {
                DebugHelper.WriteLogEntry(ex, null);
            }
        }

    }
}
