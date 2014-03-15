using System;
using System.IO;
using System.Net;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.Server;
using Galssoft.VKontakteWM.Components.Data;
using Galssoft.VKontakteWM.Components.ResponseClasses;
using Galssoft.VKontakteWM.Components.Common.Localization;
using Galssoft.VKontakteWM.Components.Common.ResponseClasses;
using System.Text;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using System.Collections;
using System.Collections.Generic;

namespace Galssoft.VKontakteWM.Components
{
    public enum StatusActionType
    {
        Refresh,
        Clear,
        Delete
    }

    public enum EventType
    {
        Messages,
        Comments,
        Friends,
        FriendsPhotos,
        FriendsNews,
        WallMessages
    }

    public enum BrowserNavigationType
    {
        Messages,
        Guests,
        Marks,
        Notifications,
        Activities,
        Discussions,
        FriendProfile,
        WriteMessage,
        SendGift,
        UserProfile,
        PhotoPrivate,
        PhotoAlbum
    }

    //Логика требующая взаимодействия логики коммуникаций и данных
    public class BaseLogic
    {
        //Предоставляет интерфейс для работы с логикой работы с данными
        private IDataLogic _iDataLogic;

        //Предоставляет интерфейс для работы с коммуникационной логикой
        private ICommunicationLogic _iCommunicationLogic;

        public IDataLogic IDataLogic { get { return _iDataLogic; } }

        public ICommunicationLogic ICommunicationLogic { get { return _iCommunicationLogic; } }

        public BaseLogic(IDataLogic iDataLogic, ICommunicationLogic iCommunicationLogic)
        {
            _iDataLogic = iDataLogic;
            _iCommunicationLogic = iCommunicationLogic;

            timerKeepAwake = new System.Windows.Forms.Timer();
            timerKeepAwake.Tick += TimerKeepAwakeTick;
            timerKeepAwake.Interval = 10000;

#if DEBUG

            iCommunicationLogic.LogRequestEvent += LogRequest;
            iCommunicationLogic.LogResponseEvent += LogResponse;

#endif
        }

        #region 1 (автор., инф., загруз. изобр.)

        #region авторизация

        /// <summary>
        /// автологин по токену
        /// </summary>
        public void AutoLogin()
        {
            string token = _iDataLogic.GetToken();
            DebugHelper.WriteLogEntry("AutoLogin with token: " + token);
            string uid = _iDataLogic.GetUid();

            DebugHelper.WriteLogEntry("AutoLogin with uid: " + uid);

            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(uid))
            {
                try
                {
                    AuthLoginByToken(token, uid);
                }
                catch (VKException e)
                {
                    if (e.LocalizedMessage == ExceptionMessage.IncorrectLoginOrPassword)
                    {
                        string login = _iDataLogic.GetSavedLogin();
                        byte[] passCrypted = _iDataLogic.GetSavedCryptoPass();
                        if (!string.IsNullOrEmpty(login) && passCrypted != null)
                        {
                            UnicodeEncoding byteConverter = new UnicodeEncoding();
                            byte[] decryptedData = CryptoServiceProvider.RSADecrypt(passCrypted);
                            string pass = byteConverter.GetString(decryptedData, 0, decryptedData.Length);

                            AuthLogin(login, pass, true);
                        }
                        else
                        {
                            throw;
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                //throw new VKException(ExceptionMessage.NoSavedToken);
                throw new VKException(ExceptionMessage.IncorrectLoginOrPassword);
            }
        }

        /// <summary>
        /// Авторизация пользователя по логину/паролю
        /// Выбрасывает исключение с ошибкой, если логин неудачный
        /// </summary>
        /// <param name="login">Логин </param>
        /// <param name="pass">Пароль </param>
        /// <param name="isRemember">Флаг необходимости сохранения токена</param>
        public void AuthLogin(string login, string pass, bool isRemember)
        {
            AuthLoginResponse response = null;

            try
            {
                response = _iCommunicationLogic.AuthLogin(login, pass, isRemember);
                // Проверка на активацию приложения
                try
                {
                    AuthLoginResponse reresponse = _iCommunicationLogic.AuthLoginByToken(response.auth_token, response.uid);
                }
                catch (VKException)
                {
                    throw new VKException(ExceptionMessage.NoLinkedApplication);
                }

            }
            catch (VKException)
            {
                throw;
            }
            catch (TimeoutException ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.AuthLogin Timeout Exception message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.AuthLogin Timeout Exception stack trace: " + ex.StackTrace);

                throw new VKException(ExceptionMessage.NoConnection);
            }
            catch (WebException ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.AuthLogin Web Exception message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.AuthLogin Web Exception stack trace: " + ex.StackTrace);

                throw new VKException(ExceptionMessage.NoConnection);
            }
            catch (OutOfMemoryException ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.AuthLogin OutOfMemory Exception message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.AuthLogin OutOfMemory Exception stack trace: " + ex.StackTrace);

                throw;
            }
            catch (Exception ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.AuthLogin Unknown Exception message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.AuthLogin Unknown Exception stack trace: " + ex.StackTrace);
            }

            if (response == null)
            {
                // ошибка при обращении к серверу
                throw new VKException(ExceptionMessage.UnknownError);
            }

            // прочистка кэша если это новый пользователь
            if (_iDataLogic.GetSavedLogin() != login)
            {
                _iDataLogic.ClearPass();
                _iDataLogic.ClearCache();
            }

            // cохранение удачного логина
            _iDataLogic.SetSavedLogin(login);

            // сохранение пароля
            var byteConverter = new UnicodeEncoding();
            byte[] dataToEncrypt = byteConverter.GetBytes(pass);
            byte[] encryptedData = CryptoServiceProvider.RSAEncrypt(dataToEncrypt);
            _iDataLogic.SetSavedCryptoPass(encryptedData);

            // session_key
            _iDataLogic.SetSessionKey(response.session_key);

            // iud
            _iDataLogic.SetUid(response.uid);

            //токен
            if (isRemember)
            {
                _iDataLogic.SetToken(response.auth_token);
            }
        }

        /// <summary>
        /// Авторизация пользователя по токену
        /// Выбрасывает исключение с ошибкой, если логин неудачный
        /// </summary>
        /// <param name="token">Токен </param>
        /// <param name="uid"></param>
        public void AuthLoginByToken(string token, string uid)
        {
            AuthLoginResponse response = null;

            try
            {
                response = _iCommunicationLogic.AuthLoginByToken(token, uid);
            }
            catch (VKException e)
            {
                throw;
            }
            catch (TimeoutException ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.AuthLoginByToken Timeout Exception message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.AuthLoginByToken Timeout Exception stack trace: " + ex.StackTrace);

                throw new VKException(ExceptionMessage.NoConnection);
            }
            catch (WebException ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.AuthLoginByToken Web Exception message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.AuthLoginByToken Web Exception stack trace: " + ex.StackTrace);

                throw new VKException(ExceptionMessage.NoConnection);
            }
            catch (OutOfMemoryException ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.AuthLoginByToken OutOfMemory Exception message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.AuthLoginByToken OutOfMemory Exception stack trace: " + ex.StackTrace);

                throw;
            }
            catch (Exception ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.AuthLoginByToken Unknown Exception message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.AuthLoginByToken Unknown Exception stack trace: " + ex.StackTrace);
            }

            if (response == null)
            {
                // ошибка при обращении к серверу
                throw new VKException(ExceptionMessage.UnknownError);
            }

            // uid
            _iDataLogic.SetUid(response.uid);

            // session_key
            _iDataLogic.SetSessionKey(response.session_key);
            DebugHelper.WriteLogEntry("AuthLoginByToken new session_key: " + response.session_key);
        }

        /// <summary>
        /// Уничтожение текущей сессии
        /// </summary>
        public void ExpireCurrentSession()
        {
            DebugHelper.WriteLogEntry("BaseLogic.ExpireCurrentSession");

            ErrorResponse errorResponse;
            bool res;

            try
            {
                res = _iCommunicationLogic.ExpireSession(_iDataLogic.GetSessionKey(), out errorResponse);
            }
            catch (Exception ex)
            {
                // Сервер недоступен

                DebugHelper.WriteLogEntry("BaseLogic.ExpireCurrentSession " + ex.Message);
                DebugHelper.WriteLogEntry(ex.GetType() + ex.StackTrace);
                throw new VKException(ExceptionMessage.ServerUnavalible);
            }

            if (res)
            {
                DebugHelper.WriteLogEntry("ExpireCurrentSession complete");
            }
            else
            {
                DebugHelper.WriteLogEntry("ExpireSession fail");
                if (errorResponse != null)
                {
                    DebugHelper.WriteLogEntry(errorResponse.error_msg);
                    throw new VKException(ExceptionMessage.ServerUnavalible);
                }
                throw new VKException(ExceptionMessage.UnknownError);
            }
        }

        #endregion

        #region инфо

        /// <summary>
        /// Получает информацию о залогиневшемся пользователе и сохраняет в кэш
        /// </summary>
        /// <param name="isRefresh">Указывает на необходимость обновления с сайта, если true - данные из кэша не считываются</param>
        /// <param name="restoreSession"></param>
        public User GetAuthorizedUserInfo(bool isRefresh, bool restoreSession)
        {
            // проверить данные в кеше
            try
            {
                if (isRefresh)
                {
                    throw new Exception();
                }

                //try
                //{
                //    User loggedInUser = Cache.Cache.LoadFromCache<User>(string.Empty, "LoggedInUserData");
                //    return loggedInUser;
                //}
                //catch (IOException)
                //{
                //    return null;
                //}

                return null;
            }
            catch (Exception)
            {
                //обновление с сервера

                //WiFi
                if (_iDataLogic.GetOnlyWIFI() == "1")
                    if (!CoreHelper.TurnWiFi(true)) throw new VKException(ExceptionMessage.NoConnection);

                ErrorResponse errorResponse = null;

                if (restoreSession)
                {
                    AutoLogin();
                }

                User response = null;

                try
                {
                    response = _iCommunicationLogic.UserGetInfo(_iDataLogic.GetUid(), _iDataLogic.GetSessionKey(), out errorResponse);
                }
                catch (VKException)
                {
                    throw;
                }
                catch (TimeoutException ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.GetAuthorizedUserInfo Timeout Exception message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.GetAuthorizedUserInfo Timeout Exception stack trace: " + ex.StackTrace);

                    throw new VKException(ExceptionMessage.NoConnection);
                }
                catch (WebException ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.GetAuthorizedUserInfo Web Exception message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.GetAuthorizedUserInfo Web Exception stack trace: " + ex.StackTrace);

                    throw new VKException(ExceptionMessage.NoConnection);
                }
                catch (OutOfMemoryException ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.GetAuthorizedUserInfo OutOfMemory Exception message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.GetAuthorizedUserInfo OutOfMemory Exception stack trace: " + ex.StackTrace);

                    throw;
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.GetAuthorizedUserInfo Unknown Exception message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.GetAuthorizedUserInfo Unknown Exception stack trace: " + ex.StackTrace);
                }

                if (response == null)
                {
                    if (errorResponse != null) // ошибка при обращении к серверу
                    {
                        if (errorResponse.error_code == "1")
                        {
                            if (!restoreSession)
                            {
                                return GetAuthorizedUserInfo(isRefresh, true);
                            }
                            else
                            {
                                throw new VKException(ExceptionMessage.ServerUnavalible);
                            }
                        }
                        else if (errorResponse.error_code == "2")
                        {
                            throw new VKException(ExceptionMessage.AccountBloked);
                        }
                        else
                        {
                            throw new VKException(ExceptionMessage.UnknownError);
                        }
                    }
                    else
                    {
                        //throw new VKException(ExceptionMessage.UnknownError);
                    }
                }

                ////сохраняем в кэш
                //try
                //{
                //    DebugHelper.WriteLogEntry("(Кэш) Сохранение данных LoggedInUserData...");

                //    bool result;

                //    result = Cache.Cache.SaveToCache(response, string.Empty, "LoggedInUserData");

                //    if (result)
                //    {
                //        DebugHelper.WriteLogEntry("(Кэш) Данные LoggedInUserData успешно сохранены.");
                //    }
                //    else
                //    {
                //        DebugHelper.WriteLogEntry("(Кэш) Данные LoggedInUserData не сохранены.");
                //    }
                //}
                //catch (IOException)
                //{
                //    DebugHelper.WriteLogEntry("(Кэш) В процессе сохранения данных LoggedInUserData произошла ошибка.");
                //}

                return response;
            }
        }

        /// <summary>
        /// Проверка на существование новых сообщений
        /// </summary>
        /// <returns></returns>
        public bool IsNewMessages()
        {
            MessShortCorrespondence newMessShortCorrespondence = null;

            try
            {
                //newMessShortCorrespondence = Cache.Cache.LoadFromCache<MessShortCorrespondence>(string.Empty, "MessageShortCorrespondence");
                newMessShortCorrespondence = DataModel.Data.MessageShortCorrespondence;

                if (newMessShortCorrespondence == null)
                {
                    return false;
                }

                foreach (MessShort mShort in newMessShortCorrespondence.MessList)
                {
                    if (mShort != null && !mShort.mIsRead)
                    {
                        return true;
                    }
                }
            }
            catch
            {
                //                
            }

            return false;
        }

        /// <summary>
        /// Получает список событий и сохраняет в реестр
        /// </summary>
        /// <param name="isRefresh">Указывает на необходимость обновления с сайта, если true - данные из кэша не считываются</param>
        /// <param name="restoreSession"></param>
        /// <param name="useKeepAwake"></param>
        public EventsGetResponse EventsGet(bool isRefresh, bool restoreSession, bool useKeepAwake)
        {
            DebugHelper.WriteLogEntry("BaseLogic.EventsGet");

            try
            {
                if (isRefresh)
                {
                    throw new Exception();
                }

                return _iDataLogic.EventsGet();
            }
            catch (Exception)
            {
                try
                {
                    if (useKeepAwake)
                        timerKeepAwake.Enabled = true;

                    //попытка установить WiFi на всякий случай
                    CoreHelper.TurnWiFi(true);
                    //if (_iDataLogic.GetOnlyWIFI() == "1")
                        //if (!CoreHelper.TurnWiFi(true)) throw new VKException(ExceptionMessage.NoConnection);

                    // восстановливаем сессию
                    if (restoreSession)
                    {
                        AutoLogin();
                    }

                    RawEventsGetResponse newGetEventsResponse = new RawEventsGetResponse();

                    ErrorResponse newErrorResponse_LoadShortActivityResponseData = null;
                    ErrorResponse newErrorResponse_LoadShortUpdatesPhotosResponse = null;
                    ErrorResponse newErrorResponse_LoadShortWallResponseData = null;
                    ErrorResponse newErrorResponse_LoadShortPhotosCommentsRespounse = null;

                    ShortActivityResponse newShortActivityResponse = null;
                    ShortUpdatesPhotosResponse newShortUpdatesPhotosResponse = null;
                    ShortWallResponse newShortWallResponse = null;
                    ShortPhotosCommentsRespounse newShortPhotosCommentsRespounse = null;

                    ShortActivityResponse oldShortActivityResponse = null;
                    ShortUpdatesPhotosResponse oldShortUpdatesPhotosResponse = null;
                    ShortWallResponse oldShortWallResponse = null;
                    ShortPhotosCommentsRespounse oldShortPhotosCommentsRespounse = null;

                    try
                    {

                        #region Н О В Ы Е   С О О Б Щ Е Н И Я   И   З А П Р О С Ы   Н А    Д Р У Ж Б У

                        try
                        {
                            DebugHelper.WriteLogEntry("(Сервер) Загрузка newGetEventsResponse...");

                            newGetEventsResponse = _iCommunicationLogic.GetEvents(_iDataLogic.GetUid(), _iDataLogic.GetSessionKey());

                            DebugHelper.WriteLogEntry("(Сервер) newGetEventsResponse успешно загружено.");
                        }
                        catch
                        {
                            newGetEventsResponse.FriendsCount = -1;
                            newGetEventsResponse.MessagesCount = -1;
                        }

                        #endregion

                        #region Н О В Ы Е   С Т А Т У С Ы   Д Р У З Е Й

                        //загружаем список ID последних 50 статусов

                        //с сервера
                        try
                        {
                            DebugHelper.WriteLogEntry("(Сервер) Загрузка newShortActivityResponse...");

                            newShortActivityResponse = _iCommunicationLogic.LoadShortActivityResponseData(_iDataLogic.GetUid(), _iDataLogic.GetSessionKey(), "0", "50", out newErrorResponse_LoadShortActivityResponseData);

                            DebugHelper.WriteLogEntry("(Сервер) newShortActivityResponse успешно загружено.");
                        }
                        catch
                        {
                            newGetEventsResponse.FriendsNewsCount = -1;
                        }

                        //из кэша
                        try
                        {
                            DebugHelper.WriteLogEntry("(Кэш) Загрузка oldShortActivityResponse...");

                            oldShortActivityResponse = Cache.Cache.LoadFromCache<ShortActivityResponse>(string.Empty, "ShortActivityResponse");

                            DebugHelper.WriteLogEntry("(Кэш) oldShortActivityResponse успешно загружено.");
                        }
                        catch (Exception newExeption)
                        {
                            newGetEventsResponse.FriendsNewsCount = -1;

                            DebugHelper.WriteLogEntry("(Кэш) В процессе загрузки oldShortActivityResponse произошла ошибка: " + newExeption.Message);
                        }

                        //если операции выполнены успешно...
                        if (newShortActivityResponse != null)
                        {
                            if (oldShortActivityResponse != null)
                            {
                                //определяем: не появилось ли новых id?
                                int newIDCount = 0;

                                foreach (int newID in newShortActivityResponse.sadStatusID)
                                {
                                    foreach (int oldID in oldShortActivityResponse.sadStatusID)
                                    {
                                        if (newID == oldID)
                                        {
                                            newIDCount++;
                                        }
                                    }
                                }

                                newGetEventsResponse.FriendsNewsCount = newShortActivityResponse.sadStatusID.Count - newIDCount;
                            }
                            else
                            {
                                #region сохраняем в кэш полученные данные

                                try
                                {
                                    // чтобы избежать перезаписи
                                    string[] cacheData = Cache.Cache.GetEntryNames(string.Empty);

                                    foreach (string cacheEntity in cacheData)
                                    {
                                        if (cacheEntity.Equals("ShortActivityResponse"))
                                        {
                                            throw new Exception();
                                        }
                                    }

                                    DebugHelper.WriteLogEntry("(Кэш) Сохранение newShortActivityResponse...");

                                    bool result;

                                    result = Cache.Cache.SaveToCache(newShortActivityResponse, string.Empty, "ShortActivityResponse");

                                    if (result)
                                    {
                                        newGetEventsResponse.FriendsNewsCount = 0;

                                        DebugHelper.WriteLogEntry("(Кэш) newShortActivityResponse успешно сохранено.");
                                    }
                                    else
                                    {
                                        DebugHelper.WriteLogEntry("(Кэш) newShortActivityResponse не сохранено.");
                                    }
                                }
                                catch (Exception newException)
                                {
                                    DebugHelper.WriteLogEntry("(Кэш) В процессе сохранения ShortActivityResponse произошла ошибка: " + newException.Message);
                                }

                                #endregion
                            }
                        }

                        #endregion

                        #region Н О В Ы Е   Ф О Т О Г Р А Ф И И   П О Л Ь З О В А Т Е Л Е Й

                        //загружаем список последних 100 фотографий пользователя

                        //с сервера
                        try
                        {
                            DebugHelper.WriteLogEntry("(Сервер) Загрузка newShortUpdatesPhotosResponse...");

                            newShortUpdatesPhotosResponse = _iCommunicationLogic.LoadShortUpdatesPhotosResponse(_iDataLogic.GetSessionKey(), "0", "100", out newErrorResponse_LoadShortUpdatesPhotosResponse);

                            DebugHelper.WriteLogEntry("(Сервер) newShortUpdatesPhotosResponse успешно загружено.");
                        }
                        catch
                        {
                            newGetEventsResponse.FriendsPhotosCount = -1;
                        }

                        //из кэша
                        try
                        {
                            DebugHelper.WriteLogEntry("(Кэш) Загрузка oldShortUpdatesPhotosResponse...");

                            oldShortUpdatesPhotosResponse = Cache.Cache.LoadFromCache<ShortUpdatesPhotosResponse>(string.Empty, "ShortUpdatesPhotosResponse");

                            DebugHelper.WriteLogEntry("(Кэш) oldShortUpdatesPhotosResponse успешно загружено.");
                        }
                        catch (Exception newExeption)
                        {
                            newGetEventsResponse.FriendsPhotosCount = -1;

                            DebugHelper.WriteLogEntry("(Кэш) В процессе загрузки oldShortUpdatesPhotosResponse произошла ошибка: " + newExeption.Message);
                        }

                        //если операции выполнены успешно...
                        if (newShortUpdatesPhotosResponse != null)
                        {
                            if (oldShortUpdatesPhotosResponse != null)
                            {
                                //определяем: не появилось ли новых id?
                                int newIDCount = 0;

                                foreach (int newID in newShortUpdatesPhotosResponse.suprPhotoID)
                                {
                                    foreach (int oldID in oldShortUpdatesPhotosResponse.suprPhotoID)
                                    {
                                        if (newID == oldID)
                                        {
                                            newIDCount++;
                                        }
                                    }
                                }

                                //если > 100 фотографий - не имеем возможности отслежавать (т.к. подмножество фотографии в списке из множества на сервере определяется случайным образом)
                                if (!(oldShortUpdatesPhotosResponse.suprPhotoID.Count < 100))
                                {
                                    newGetEventsResponse.FriendsPhotosCount = 100;
                                }
                                else
                                {
                                    newGetEventsResponse.FriendsPhotosCount = newShortUpdatesPhotosResponse.suprPhotoID.Count - newIDCount;
                                }
                            }
                            else
                            {
                                #region сохраняем в кэш полученные данные
                                try
                                {
                                    // чтобы избежать перезаписи
                                    string[] cacheData = Cache.Cache.GetEntryNames(string.Empty);

                                    foreach (string cacheEntity in cacheData)
                                    {
                                        if (cacheEntity.Equals("ShortUpdatesPhotosResponse"))
                                        {
                                            throw new Exception();
                                        }
                                    }

                                    DebugHelper.WriteLogEntry("(Кэш) Сохранение newShortUpdatesPhotosResponse...");

                                    bool result;

                                    result = Cache.Cache.SaveToCache(newShortUpdatesPhotosResponse, string.Empty, "ShortUpdatesPhotosResponse");

                                    if (result)
                                    {
                                        newGetEventsResponse.FriendsPhotosCount = 0;

                                        DebugHelper.WriteLogEntry("(Кэш) newShortUpdatesPhotosResponse успешно сохранено.");
                                    }
                                    else
                                    {
                                        DebugHelper.WriteLogEntry("(Кэш) newShortUpdatesPhotosResponse не сохранено.");
                                    }
                                }
                                catch (Exception newException)
                                {
                                    DebugHelper.WriteLogEntry("(Кэш) В процессе сохранения newShortUpdatesPhotosResponse произошла ошибка: " + newException.Message);
                                }
                                #endregion
                            }
                        }

                        #endregion

                        #region Н О В Ы Е   С О О Б Щ Е Н И Я   Н А   С Т Е Н У
                        newGetEventsResponse.WallCount = -1;
                        /*
                        //загружаем список последних 50 сообщений со стены

                        //с сервера
                        try
                        {
                            DebugHelper.WriteLogEntry("(Сервер) Загрузка newShortWallResponse...");

                            newShortWallResponse = _iCommunicationLogic.LoadShortWallResponseData(_iDataLogic.GetUid(), _iDataLogic.GetSessionKey(), "0", "50", out newErrorResponse_LoadShortWallResponseData);

                            DebugHelper.WriteLogEntry("(Сервер) newShortWallResponse успешно загружено.");
                        }
                        catch
                        {
                            newGetEventsResponse.WallCount = -1;
                        }

                        //из кэша
                        try
                        {
                            DebugHelper.WriteLogEntry("(Кэш) Загрузка oldShortWallResponse...");

                            oldShortWallResponse = Cache.Cache.LoadFromCache<ShortWallResponse>(string.Empty, "ShortWallResponse");

                            DebugHelper.WriteLogEntry("(Кэш) oldShortWallResponse успешно загружено.");
                        }
                        catch (Exception newExeption)
                        {
                            newGetEventsResponse.WallCount = -1;

                            DebugHelper.WriteLogEntry("(Кэш) В процессе загрузки oldShortWallResponse произошла ошибка: " + newExeption.Message);
                        }

                        //если операции выполнены успешно...
                        if (newShortWallResponse != null)
                        {
                            if (oldShortWallResponse != null)
                            {
                                //определяем: не появилось ли новых id?
                                int newIDCount = 0;

                                foreach (int newID in newShortWallResponse.swrMessageID)
                                {
                                    foreach (int oldID in oldShortWallResponse.swrMessageID)
                                    {
                                        if (newID == oldID)
                                        {
                                            newIDCount++;
                                        }
                                    }
                                }

                                newGetEventsResponse.WallCount = newShortWallResponse.swrMessageID.Count - newIDCount;
                            }
                            else
                            {
                                #region сохраняем в кэш полученные данные
                                try
                                {
                                    // чтобы избежать перезаписи
                                    string[] cacheData = Cache.Cache.GetEntryNames(string.Empty);

                                    foreach (string cacheEntity in cacheData)
                                    {
                                        if (cacheEntity.Equals("ShortWallResponse"))
                                        {
                                            throw new Exception();
                                        }
                                    }

                                    DebugHelper.WriteLogEntry("(Кэш) Сохранение newShortWallResponse...");

                                    bool result;

                                    result = Cache.Cache.SaveToCache(newShortWallResponse, string.Empty, "ShortWallResponse");

                                    if (result)
                                    {
                                        newGetEventsResponse.WallCount = 0;

                                        DebugHelper.WriteLogEntry("(Кэш) newShortWallResponse успешно сохранено.");
                                    }
                                    else
                                    {
                                        DebugHelper.WriteLogEntry("(Кэш) newShortWallResponse не сохранено.");
                                    }
                                }
                                catch (Exception newException)
                                {
                                    DebugHelper.WriteLogEntry("(Кэш) В процессе сохранения newShortWallResponse произошла ошибка: " + newException.Message);
                                }
                                #endregion
                            }
                        }
                        */
                        #endregion

                        #region Н О В Ы Е   К О М Е Н Т А Р И И   К   Ф О Т О Г Р А Ф И Я М

                        //загружаем список последних 50 комменариев к фотографиям пользователя

                        //с сервера
                        try
                        {
                            DebugHelper.WriteLogEntry("(Сервер) Загрузка newShortPhotosCommentsRespounse...");

                            newShortPhotosCommentsRespounse = _iCommunicationLogic.LoadShortPhotosCommentsRespounse(_iDataLogic.GetUid(), _iDataLogic.GetSessionKey(), "0", "50", "-1", out newErrorResponse_LoadShortPhotosCommentsRespounse);

                            DebugHelper.WriteLogEntry("(Сервер) newShortPhotosCommentsRespounse успешно загружено.");
                        }
                        catch
                        {
                            newGetEventsResponse.CommentsCount = -1;
                        }

                        //из кэша
                        try
                        {
                            DebugHelper.WriteLogEntry("(Кэш) Загрузка oldShortPhotosCommentsRespounse...");

                            oldShortPhotosCommentsRespounse = Cache.Cache.LoadFromCache<ShortPhotosCommentsRespounse>(string.Empty, "ShortPhotosCommentsRespounse");

                            DebugHelper.WriteLogEntry("(Кэш) oldShortPhotosCommentsRespounse успешно загружено.");
                        }
                        catch (Exception newExeption)
                        {
                            newGetEventsResponse.CommentsCount = -1;

                            DebugHelper.WriteLogEntry("(Кэш) В процессе загрузки oldShortPhotosCommentsRespounse произошла ошибка: " + newExeption.Message);
                        }

                        // если операции выполнены успешно...
                        if (newShortPhotosCommentsRespounse != null)
                        {
                            if (oldShortPhotosCommentsRespounse != null)
                            {
                                // определяем: не появилось ли новых id?
                                int newIDCount = 0;

                                foreach (int newID in newShortPhotosCommentsRespounse.spcrCommentIDs)
                                {
                                    foreach (int oldID in oldShortPhotosCommentsRespounse.spcrCommentIDs)
                                    {
                                        if (newID == oldID)
                                        {
                                            newIDCount++;
                                        }
                                    }
                                }

                                newGetEventsResponse.CommentsCount = newShortPhotosCommentsRespounse.spcrCommentIDs.Count - newIDCount;
                            }
                            else
                            {
                                #region сохраняем в кэш полученные данные
                                try
                                {
                                    // чтобы избежать перезаписи
                                    string[] cacheData = Cache.Cache.GetEntryNames(string.Empty);

                                    foreach (string cacheEntity in cacheData)
                                    {
                                        if (cacheEntity.Equals("ShortPhotosCommentsRespounse"))
                                        {
                                            throw new Exception();
                                        }
                                    }

                                    DebugHelper.WriteLogEntry("(Кэш) Сохранение newShortPhotosCommentsRespounse...");

                                    bool result;

                                    result = Cache.Cache.SaveToCache(newShortPhotosCommentsRespounse, string.Empty, "ShortPhotosCommentsRespounse");

                                    if (result)
                                    {
                                        newGetEventsResponse.CommentsCount = 0;

                                        DebugHelper.WriteLogEntry("(Кэш) newShortPhotosCommentsRespounse успешно сохранено.");
                                    }
                                    else
                                    {
                                        DebugHelper.WriteLogEntry("(Кэш) newShortPhotosCommentsRespounse не сохранено.");
                                    }
                                }
                                catch (Exception newException)
                                {
                                    DebugHelper.WriteLogEntry("(Кэш) В процессе сохранения newShortPhotosCommentsRespounse произошла ошибка: " + newException.Message);
                                }
                                #endregion
                            }
                        }

                        #endregion

                    }
                    catch (VKException)
                    {
                        throw;
                    }
                    catch (TimeoutException ex)
                    {
                        DebugHelper.WriteLogEntry("BaseLogic.EventsGet Timeout Exception message: " + ex.Message);
                        DebugHelper.WriteLogEntry("BaseLogic.EventsGet Timeout Exception stack trace: " + ex.StackTrace);

                        throw new VKException(ExceptionMessage.NoConnection);
                    }
                    catch (WebException ex)
                    {
                        DebugHelper.WriteLogEntry("BaseLogic.EventsGet Web Exception message: " + ex.Message);
                        DebugHelper.WriteLogEntry("BaseLogic.EventsGet Web Exception stack trace: " + ex.StackTrace);

                        throw new VKException(ExceptionMessage.NoConnection);
                    }
                    catch (OutOfMemoryException ex)
                    {
                        DebugHelper.WriteLogEntry("BaseLogic.EventsGet OutOfMemory Exception message: " + ex.Message);
                        DebugHelper.WriteLogEntry("BaseLogic.EventsGet OutOfMemory Exception stack trace: " + ex.StackTrace);

                        throw;
                    }
                    catch (Exception ex)
                    {
                        DebugHelper.WriteLogEntry("BaseLogic.EventsGet Unknown Exception message: " + ex.Message);
                        DebugHelper.WriteLogEntry("BaseLogic.EventsGet Unknown Exception stack trace: " + ex.StackTrace);
                    }

                    #region здесь пытаемся ловить ошибку, что сессия истекла или captcha

                    if (newErrorResponse_LoadShortActivityResponseData != null)
                    {
                        if (newErrorResponse_LoadShortActivityResponseData.error_code.Equals("1"))
                        {
                            if (!restoreSession)
                            {
                                return EventsGet(isRefresh, true, true);
                            }
                            else
                            {
                                throw new VKException(ExceptionMessage.ServerUnavalible);
                            }
                        }
                        else if (newErrorResponse_LoadShortActivityResponseData.error_code.Equals("2"))
                        {
                            throw new VKException(ExceptionMessage.AccountBloked);
                        }
                        else
                        {
                            throw new VKException(ExceptionMessage.UnknownError);
                        }
                    }

                    if (newErrorResponse_LoadShortUpdatesPhotosResponse != null)
                    {
                        if (newErrorResponse_LoadShortUpdatesPhotosResponse.error_code.Equals("1"))
                        {
                            if (!restoreSession)
                            {
                                return EventsGet(isRefresh, true, true);
                            }
                            else
                            {
                                throw new VKException(ExceptionMessage.ServerUnavalible);
                            }
                        }
                        else if (newErrorResponse_LoadShortUpdatesPhotosResponse.error_code.Equals("2"))
                        {
                            throw new VKException(ExceptionMessage.AccountBloked);
                        }
                        else
                        {
                            throw new VKException(ExceptionMessage.UnknownError);
                        }
                    }

                    if (newErrorResponse_LoadShortWallResponseData != null)
                    {
                        if (newErrorResponse_LoadShortWallResponseData.error_code.Equals("1"))
                        {
                            if (!restoreSession)
                            {
                                return EventsGet(isRefresh, true, true);
                            }
                            else
                            {
                                throw new VKException(ExceptionMessage.ServerUnavalible);
                            }
                        }
                        else if (newErrorResponse_LoadShortWallResponseData.error_code.Equals("2"))
                        {
                            throw new VKException(ExceptionMessage.AccountBloked);
                        }
                        else
                        {
                            throw new VKException(ExceptionMessage.UnknownError);
                        }
                    }

                    if (newErrorResponse_LoadShortPhotosCommentsRespounse != null)
                    {
                        if (newErrorResponse_LoadShortPhotosCommentsRespounse.error_code.Equals("1"))
                        {
                            if (!restoreSession)
                            {
                                return EventsGet(isRefresh, true, true);
                            }
                            else
                            {
                                throw new VKException(ExceptionMessage.ServerUnavalible);
                            }
                        }
                        else if (newErrorResponse_LoadShortPhotosCommentsRespounse.error_code.Equals("2"))
                        {
                            throw new VKException(ExceptionMessage.AccountBloked);
                        }
                        else
                        {
                            throw new VKException(ExceptionMessage.UnknownError);
                        }
                    }

                    #endregion

                    _iDataLogic.SetEvents(newGetEventsResponse);

                    return _iDataLogic.EventsGet();
                }
                finally
                {
                    timerKeepAwake.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Меняет статус пользователя на сервере и сохраняет его в кэш
        /// </summary>
        /// <param name="status">Требуемый статус</param>
        /// <param name="action">Требуемое действие</param>
        /// <param name="restoreSession">Необходимость обновления сессии</param>
        public void SetStatus(string status, StatusActionType actionType, bool restoreSession)
        {
            DebugHelper.WriteLogEntry("BaseLogic.SetStatus");

            //WiFi
            if (_iDataLogic.GetOnlyWIFI() == "1")
                if (!CoreHelper.TurnWiFi(true)) throw new VKException(ExceptionMessage.NoConnection);

            // восстановление сессии
            if (restoreSession)
            {
                AutoLogin();
            }

            ErrorResponse errorResponse = null;

            bool res = false;

            try
            {
                res = _iCommunicationLogic.SetStatus(_iDataLogic.GetUid(), _iDataLogic.GetSessionKey(), status, actionType, out errorResponse);
            }
            catch (VKException)
            {
                throw;
            }
            catch (TimeoutException ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.SetStatus Timeout Exception message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.SetStatus Timeout Exception stack trace: " + ex.StackTrace);

                throw new VKException(ExceptionMessage.NoConnection);
            }
            catch (WebException ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.SetStatus Web Exception message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.SetStatus Web Exception stack trace: " + ex.StackTrace);

                throw new VKException(ExceptionMessage.NoConnection);
            }
            catch (OutOfMemoryException ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.SetStatus OutOfMemory Exception message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.SetStatus OutOfMemory Exception stack trace: " + ex.StackTrace);

                throw;
            }
            catch (Exception ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.SetStatus Unknown Exception message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.SetStatus Unknown Exception stack trace: " + ex.StackTrace);
            }

            if (res)
            {
                //// cохранение в кэш // что-то ни одного try ... catch ... нет =(
                //var loggedInUser = Cache.Cache.LoadFromCache<User>(string.Empty, "LoggedInUserData");

                //loggedInUser.Status = status;

                //Cache.Cache.SaveToCache(loggedInUser, string.Empty, "LoggedInUserData");
            }
            else
            {
                if (errorResponse != null)
                {
                    if (errorResponse.error_code == "1")
                    {
                        if (!restoreSession)
                        {
                            SetStatus(status, actionType, true);
                        }
                        else
                        {
                            throw new VKException(ExceptionMessage.ServerUnavalible);
                        }
                    }
                    else if (errorResponse.error_code == "2")
                    {
                        throw new VKException(ExceptionMessage.AccountBloked);
                    }
                    else if (errorResponse.error_code == "3")
                    {
                        throw new VKException(ExceptionMessage.UnsuccessfullOperation);
                    }
                    else
                    {
                        throw new VKException(ExceptionMessage.UnknownError);
                    }
                }
                else
                {
                    //throw new VKException(ExceptionMessage.UnknownError);
                }
            }
        }

        public int PredictFriendsCount()
        {
            try
            {
                //FriendsListResponse oldFriendsListResponse = Cache.Cache.LoadFromCache<FriendsListResponse>(string.Empty, "FriendsListResponse");
                FriendsListResponse oldFriendsListResponse = DataModel.Data.FriendsListResponseData;

                if (oldFriendsListResponse == null || oldFriendsListResponse.Users.Count == 0)
                {
                    throw new Exception();
                }

                try
                {
                    //FriendsListResponse additionalFriendsListResponse = additionalFriendsListResponse = Cache.Cache.LoadFromCache<FriendsListResponse>(string.Empty, "FriendsListAdditionalResponse");
                    FriendsListResponse additionalFriendsListResponse = DataModel.Data.FriendsListAdditionalResponseData;

                    if (additionalFriendsListResponse == null || additionalFriendsListResponse.Users.Count == 0)
                    {
                        throw new Exception();
                    }

                    int result = oldFriendsListResponse.Users.Count - additionalFriendsListResponse.Users.Count;

                    if (result > 0)
                    {
                        return result;
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch
                {
                    return oldFriendsListResponse.Users.Count;
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public bool GetNextFriendsInfo(bool isRefresh, bool restoreSession, FriendsListResponse additionalFriendsListResponse, FriendsListResponse oldFriendsListResponse)
        {
            DebugHelper.WriteLogEntry("BaseLogic.GetAdditionalFriendsInfo"); // запускаем функцию

            try
            {
                if (isRefresh)
                {
                    throw new Exception();
                }

                return false; // если не обновление, то возвращаем что есть
            }
            catch (Exception)
            {
                //WiFi
                if (_iDataLogic.GetOnlyWIFI() == "1")
                {
                    if (!CoreHelper.TurnWiFi(true))
                    {
                        throw new VKException(ExceptionMessage.NoConnection);
                    }
                }

                // восстанавливаем сессию
                if (restoreSession)
                {
                    AutoLogin();
                }

                ErrorResponse newErrorResponse = null; // ошибка

                if (oldFriendsListResponse == null)
                {
                    return false;
                }

                // проходим по списку "стандартных" данных
                // если в "стандартном" списке есть пользователь, данных которого нет в "дополнительном"
                // закачиваем профиль
                foreach (User oldUser in oldFriendsListResponse.Users)
                {
                    User additionalUserInfo = null; // буферная переменная для хранения полного профиля друга

                    if (additionalFriendsListResponse.IsUserInList(oldUser.Uid))
                    {
                        continue; // не добавляем, если уже есть инфо
                    }
                    else
                    {
                        // пользователя вносим в любом случае...

                        User newUser = new User(); // для минимизирования данных создаем нового пользователя для которого храним только 3 поля
                        additionalFriendsListResponse.Users.Add(newUser);

                        newUser.Uid = oldUser.Uid; // ID пользователя

                        // получаем профиль друга
                        try
                        {
                            additionalUserInfo = _iCommunicationLogic.UserGetInfo(oldUser.Uid, _iDataLogic.GetSessionKey(), out newErrorResponse);
                        }
                        catch (VKException)
                        {
                            throw;
                        }
                        catch (TimeoutException ex)
                        {
                            DebugHelper.WriteLogEntry("BaseLogic.GetFriendsInfo TimeoutException Message: " + ex.Message);
                            DebugHelper.WriteLogEntry("BaseLogic.GetFriendsInfo TimeoutException StackTrace: " + ex.StackTrace);

                            throw new VKException(ExceptionMessage.NoConnection);
                        }
                        catch (WebException ex)
                        {
                            DebugHelper.WriteLogEntry("BaseLogic.GetFriendsInfo WebException Message: " + ex.Message);
                            DebugHelper.WriteLogEntry("BaseLogic.GetFriendsInfo WebException StackTrace: " + ex.StackTrace);

                            throw new VKException(ExceptionMessage.NoConnection);
                        }
                        catch (OutOfMemoryException ex)
                        {
                            DebugHelper.WriteLogEntry("BaseLogic.GetFriendsInfo OutOfMemoryException Message: " + ex.Message);
                            DebugHelper.WriteLogEntry("BaseLogic.GetFriendsInfo OutOfMemoryException StackTrace: " + ex.StackTrace);

                            throw;
                        }
                        catch (Exception ex)
                        {
                            DebugHelper.WriteLogEntry("BaseLogic.GetFriendsInfo Exception Message: " + ex.Message);
                            DebugHelper.WriteLogEntry("BaseLogic.GetFriendsInfo Exception StackTrace: " + ex.StackTrace);
                        }

                        if (additionalUserInfo != null) // что-то получили от сервера
                        {
                            newUser.Birthday = additionalUserInfo.Birthday; // бездэй и мобилка из нового инфо
                            newUser.MobilePhone = additionalUserInfo.MobilePhone;
                        }
                        else // ищем ошибку...
                        {
                            if (newErrorResponse != null)
                            {
                                if (newErrorResponse.error_code == "1")
                                {
                                    if (!restoreSession)
                                    {
                                        return GetNextFriendsInfo(isRefresh, true, additionalFriendsListResponse, oldFriendsListResponse);
                                    }
                                    else
                                    {
                                        throw new VKException(ExceptionMessage.ServerUnavalible);
                                    }
                                }
                                else if (newErrorResponse.error_code == "2")
                                {
                                    throw new VKException(ExceptionMessage.AccountBloked);
                                }
                                else
                                {
                                    throw new VKException(ExceptionMessage.UnknownError);
                                }
                            }
                            else
                            {
                                //throw new VKException(ExceptionMessage.UnknownError);
                            }
                        }

                        // сохраняем результат
                        //try
                        //{
                        //    DebugHelper.WriteLogEntry("(Кэш) Сохранение данных FriendsListAdditionalResponse...");

                        //    bool result;

                        //    result = Cache.Cache.SaveToCache(additionalFriendsListResponse, string.Empty, "FriendsListAdditionalResponse");

                        //    if (result)
                        //    {
                        //        DebugHelper.WriteLogEntry("(Кэш) Данные FriendsListAdditionalResponse успешно сохранены.");
                        //    }
                        //    else
                        //    {
                        //        DebugHelper.WriteLogEntry("(Кэш) Данные FriendsListAdditionalResponse не сохранены.");
                        //    }
                        //}
                        //catch (Exception)
                        //{
                        //    DebugHelper.WriteLogEntry("(Кэш) В процессе сохранения данных FriendsListAdditionalResponse произошла ошибка.");
                        //}

                        return true; // признак того что последний пользователь был обработан успешно
                    }
                }

                //// сохраняем результат в кэш
                //try
                //{
                //    DebugHelper.WriteLogEntry("(Кэш) Сохранение данных FriendsListAdditionalResponse...");

                //    bool result;

                //    result = Cache.Cache.SaveToCache(additionalFriendsListResponse, string.Empty, "FriendsListAdditionalResponse");

                //    if (result)
                //    {
                //        DebugHelper.WriteLogEntry("(Кэш) Данные FriendsListAdditionalResponse успешно сохранены.");
                //    }
                //    else
                //    {
                //        DebugHelper.WriteLogEntry("(Кэш) Данные FriendsListAdditionalResponse не сохранены.");
                //    }
                //}
                //catch (Exception)
                //{
                //    DebugHelper.WriteLogEntry("(Кэш) В процессе сохранения данных FriendsListAdditionalResponse произошла ошибка.");
                //}

                return false; // если дошли до сюда, то значит юзверей для которых можно закачать данные больше нет
            }
        }

        //public FriendsListResponse GetAdditionalFriendsInfo(bool isRefresh, bool restoreSession)
        //{
        //    DebugHelper.WriteLogEntry("BaseLogic.GetAdditionalFriendsInfo"); // запускаем функцию

        //    FriendsListResponse additionalFriendsListResponse; // работаем с набором "дополнительных" данных

        //    // загрузка "дополнительных" данных по пользователю из кэша
        //    try
        //    {
        //        additionalFriendsListResponse = Cache.Cache.LoadFromCache<FriendsListResponse>(string.Empty, "FriendsListAdditionalResponse");
        //    }
        //    catch
        //    {
        //        additionalFriendsListResponse = new FriendsListResponse(); // если нет, создаем пустой
        //    }

        //    try
        //    {
        //        if (isRefresh)
        //        {
        //            throw new Exception();
        //        }

        //        return additionalFriendsListResponse; // если не обновление, то возвращаем что есть
        //    }
        //    catch (Exception)
        //    {
        //        //WiFi
        //        if (_iDataLogic.GetOnlyWIFI() == "1")
        //            if (!CoreHelper.TurnWiFi(true)) throw new VKException(ExceptionMessage.NoConnection);

        //        // восстанавливаем сессию
        //        if (restoreSession)
        //        {
        //            AutoLogin();
        //        }

        //        ErrorResponse newErrorResponse = null; // ошибка
        //        FriendsListResponse oldFriendsListResponse = null; // набор "стандартных" данных

        //        // поднимаем набор "стандартных" данных
        //        try
        //        {
        //            oldFriendsListResponse = Cache.Cache.LoadFromCache<FriendsListResponse>(string.Empty, "FriendsListResponse");
        //        }
        //        catch (Exception)
        //        {
        //            return additionalFriendsListResponse; // если нет в кэше, , то возвращаем что есть
        //        }

        //        // проходим по списку "стандартных" данных
        //        // если в "стандартном" списке есть пользователь, данных которого нет в "дополнительном"
        //        // закачиваем профиль
        //        foreach (User oldUser in oldFriendsListResponse.Users)
        //        {
        //            User additionalUserInfo = null; // буферная переменная для хранения полного профиля друга

        //            if (additionalFriendsListResponse.IsUserInList(oldUser.Uid))
        //            {
        //                continue; // не добавляем, если уже есть инфо
        //            }
        //            else
        //            {
        //                // получаем профиль друга
        //                try
        //                {
        //                    additionalUserInfo = _iCommunicationLogic.UserGetInfo(oldUser.Uid, _iDataLogic.GetSessionKey(), out newErrorResponse);
        //                }
        //                catch (VKException)
        //                {
        //                    throw;
        //                }
        //                catch (TimeoutException ex)
        //                {
        //                    DebugHelper.WriteLogEntry("BaseLogic.GetFriendsInfo TimeoutException Message: " + ex.Message);
        //                    DebugHelper.WriteLogEntry("BaseLogic.GetFriendsInfo TimeoutException StackTrace: " + ex.StackTrace);

        //                    throw new VKException(ExceptionMessage.NoConnection);
        //                }
        //                catch (WebException ex)
        //                {
        //                    DebugHelper.WriteLogEntry("BaseLogic.GetFriendsInfo WebException Message: " + ex.Message);
        //                    DebugHelper.WriteLogEntry("BaseLogic.GetFriendsInfo WebException StackTrace: " + ex.StackTrace);

        //                    throw new VKException(ExceptionMessage.NoConnection);
        //                }
        //                catch (OutOfMemoryException ex)
        //                {
        //                    DebugHelper.WriteLogEntry("BaseLogic.GetFriendsInfo OutOfMemoryException Message: " + ex.Message);
        //                    DebugHelper.WriteLogEntry("BaseLogic.GetFriendsInfo OutOfMemoryException StackTrace: " + ex.StackTrace);

        //                    throw;
        //                }
        //                catch (Exception ex)
        //                {
        //                    DebugHelper.WriteLogEntry("BaseLogic.GetFriendsInfo Exception Message: " + ex.Message);
        //                    DebugHelper.WriteLogEntry("BaseLogic.GetFriendsInfo Exception StackTrace: " + ex.StackTrace);
        //                }

        //                if (additionalUserInfo != null) // что-то получили от сервера
        //                {
        //                    User newUser = new User(); // для минимизирования данных создаем нового пользователя для которого храним только 3 поля

        //                    newUser.Uid = oldUser.Uid; // ID пользователя
        //                    newUser.Birthday = additionalUserInfo.Birthday; // бездэй и мобилка из нового инфо
        //                    newUser.MobilePhone = additionalUserInfo.MobilePhone;

        //                    additionalFriendsListResponse.Users.Add(newUser);
        //                }
        //                else // ищем ошибку...
        //                {
        //                    if (newErrorResponse != null)
        //                    {
        //                        if (newErrorResponse.error_code == "1")
        //                        {
        //                            if (!restoreSession)
        //                            {
        //                                return GetAdditionalFriendsInfo(isRefresh, true);
        //                            }
        //                            else
        //                            {
        //                                throw new VKException(ExceptionMessage.ServerUnavalible);
        //                            }
        //                        }
        //                        else if (newErrorResponse.error_code == "2")
        //                        {
        //                            throw new VKException(ExceptionMessage.AccountBloked);
        //                        }
        //                        else
        //                        {
        //                            throw new VKException(ExceptionMessage.UnknownError);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        throw new VKException(ExceptionMessage.UnknownError);
        //                    }
        //                }

        //                // сохраняем результат в кэш после каждой успешной попытки
        //                try
        //                {
        //                    DebugHelper.WriteLogEntry("(Кэш) Сохранение данных FriendsListAdditionalResponse...");

        //                    bool result;

        //                    result = Cache.Cache.SaveToCache(additionalFriendsListResponse, string.Empty, "FriendsListAdditionalResponse");

        //                    if (result)
        //                    {
        //                        DebugHelper.WriteLogEntry("(Кэш) Данные FriendsListAdditionalResponse успешно сохранены.");
        //                    }
        //                    else
        //                    {
        //                        DebugHelper.WriteLogEntry("(Кэш) Данные FriendsListAdditionalResponse не сохранены.");
        //                    }
        //                }
        //                catch (Exception)
        //                {
        //                    DebugHelper.WriteLogEntry("(Кэш) В процессе сохранения данных FriendsListAdditionalResponse произошла ошибка.");
        //                }
        //            }
        //        }

        //        //// сохраняем результат в кэш
        //        //try
        //        //{
        //        //    DebugHelper.WriteLogEntry("(Кэш) Сохранение данных FriendsListAdditionalResponse...");

        //        //    bool result;

        //        //    result = Cache.Cache.SaveToCache(additionalFriendsListResponse, string.Empty, "FriendsListAdditionalResponse");

        //        //    if (result)
        //        //    {
        //        //        DebugHelper.WriteLogEntry("(Кэш) Данные FriendsListAdditionalResponse успешно сохранены.");
        //        //    }
        //        //    else
        //        //    {
        //        //        DebugHelper.WriteLogEntry("(Кэш) Данные FriendsListAdditionalResponse не сохранены.");
        //        //    }
        //        //}
        //        //catch (Exception)
        //        //{
        //        //    DebugHelper.WriteLogEntry("(Кэш) В процессе сохранения данных FriendsListAdditionalResponse произошла ошибка.");
        //        //}

        //        return additionalFriendsListResponse;
        //    }
        //}

        #endregion

        #region загрузка изображения

        public void UploadPhoto(byte[] file, bool toAvatar, bool restoreSession)
        {
            DebugHelper.WriteLogEntry("BaseLogic.UploadPhoto");

            //WiFi
            if (_iDataLogic.GetOnlyWIFI() == "1")
                if (!CoreHelper.TurnWiFi(true)) throw new VKException(ExceptionMessage.NoConnection);

            // восстановить сессию
            if (restoreSession)
            {
                AutoLogin();
            }

            ErrorResponse errorResponse = null;

            if (_iDataLogic.GetPhotoRHash() == null || _iDataLogic.GetAvatarRHash() == null)
            {
                User newUser = null;
                try
                {
                    newUser = _iCommunicationLogic.UserGetInfo(_iDataLogic.GetUid(), _iDataLogic.GetSessionKey(), out errorResponse);
                }
                catch (VKException)
                {
                    throw;
                }
                catch (TimeoutException ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.GetAuthorizedUserInfo Timeout Exception message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.GetAuthorizedUserInfo Timeout Exception stack trace: " + ex.StackTrace);

                    throw new VKException(ExceptionMessage.NoConnection);
                }
                catch (WebException ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.GetAuthorizedUserInfo Web Exception message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.GetAuthorizedUserInfo Web Exception stack trace: " + ex.StackTrace);

                    throw new VKException(ExceptionMessage.NoConnection);
                }
                catch (OutOfMemoryException ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.GetAuthorizedUserInfo OutOfMemory Exception message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.GetAuthorizedUserInfo OutOfMemory Exception stack trace: " + ex.StackTrace);

                    throw;
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.GetAuthorizedUserInfo Unknown Exception message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.GetAuthorizedUserInfo Unknown Exception stack trace: " + ex.StackTrace);
                }
                if (newUser == null)
                {
                    if (errorResponse != null) // ошибка при обращении к серверу
                    {
                        if (errorResponse.error_code == "1")
                        {
                            if (!restoreSession)
                            {
                                UploadPhoto(file, toAvatar, true);
                                return;
                            }
                            else
                            {
                                throw new VKException(ExceptionMessage.ServerUnavalible);
                            }
                        }
                        else if (errorResponse.error_code == "2")
                        {
                            throw new VKException(ExceptionMessage.AccountBloked);
                        }
                        else
                        {
                            throw new VKException(ExceptionMessage.UnknownError);
                        }
                    }
                    else
                    {
                        //throw new VKException(ExceptionMessage.UnknownError);
                    }
                }
            }

            bool res = false;

            try
            {
                if (toAvatar)
                {
                    res = _iCommunicationLogic.ChangeAvatar(_iDataLogic.GetUid(), _iDataLogic.GetSessionKey(), _iDataLogic.GetAvatarUploadAddress(), _iDataLogic.GetAvatarHash(), _iDataLogic.GetAvatarRHash(), file, out errorResponse);
                }
                else
                {
                    res = _iCommunicationLogic.UploadPhoto(_iDataLogic.GetUid(), _iDataLogic.GetSessionKey(), _iDataLogic.GetPhotoHash(), _iDataLogic.GetPhotoRHash(), _iDataLogic.GetAid(), _iDataLogic.GetPhotoUploadAddress(), file, out errorResponse);
                }
            }
            catch (VKException)
            {
                throw;
            }
            catch (TimeoutException ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.UploadPhoto Timeout Exception message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.UploadPhoto Timeout stack trace: " + ex.StackTrace);

                throw new VKException(ExceptionMessage.NoConnection);
            }
            catch (WebException ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.UploadPhoto Web Exception message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.UploadPhoto Web Exception stack trace: " + ex.StackTrace);

                throw new VKException(ExceptionMessage.NoConnection);
            }
            catch (OutOfMemoryException ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.UploadPhoto OutOfMemory Exception message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.UploadPhoto OutOfMemory Exception stack trace: " + ex.StackTrace);

                throw;
            }
            catch (Exception ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.UploadPhoto Unknown Exception message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.UploadPhoto Unknown Exception stack trace: " + ex.StackTrace);
            }

            if (res)
            {
                DebugHelper.WriteLogEntry("UploadPhoto complete");
            }
            else
            {
                if (errorResponse != null)
                {
                    if (errorResponse.error_code == "1")
                    {
                        if (!restoreSession)
                        {
                            UploadPhoto(file, toAvatar, true);
                        }
                        else
                        {
                            throw new VKException(ExceptionMessage.ServerUnavalible);
                        }
                    }
                    else if (errorResponse.error_code == "2")
                    {
                        throw new VKException(ExceptionMessage.AccountBloked);
                    }
                    else
                    {
                        throw new VKException(ExceptionMessage.UnknownError);
                    }
                }
                else
                {
                    //throw new VKException(ExceptionMessage.UnknownError);
                }
            }
        }

        #endregion

        #region Добавление комментария

        public void AddComments(string message, string parent, bool restoreSession)
        {
            HttpWebResponse response = null;
            ErrorResponse errorResponse = null;

            if (restoreSession)
                AutoLogin();

            try
            {
                response = ICommunicationLogic.AddPhotosComments(IDataLogic.GetUid(), message, parent,
                                                                 IDataLogic.GetSessionKey(), out errorResponse);
            }
            catch (VKException)
            {
                throw;
            }
            catch (TimeoutException ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.UploadPhoto Timeout Exception message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.UploadPhoto Timeout stack trace: " + ex.StackTrace);

                throw new VKException(ExceptionMessage.NoConnection);
            }
            catch (WebException ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.UploadPhoto Web Exception message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.UploadPhoto Web Exception stack trace: " + ex.StackTrace);

                throw new VKException(ExceptionMessage.NoConnection);
            }
            catch (OutOfMemoryException ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.UploadPhoto OutOfMemory Exception message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.UploadPhoto OutOfMemory Exception stack trace: " + ex.StackTrace);

                throw;
            }
            catch (Exception ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.UploadPhoto Unknown Exception message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.UploadPhoto Unknown Exception stack trace: " + ex.StackTrace);
            }
            finally
            {
                if (response != null) response.Close();
            }

            if (response == null)
            {
                if (errorResponse != null)
                {
                    switch (errorResponse.error_code)
                    {
                        case "0":
                            throw new VKException(ExceptionMessage.UnsuccessfullOperation);
                        case "-1":
                            {
                                if (!restoreSession)
                                    AddComments(message, parent, true);
                                else
                                    throw new VKException(ExceptionMessage.ServerUnavalible);
                            }
                            break;
                        case "-2":
                            throw new VKException(ExceptionMessage.AccountBloked);
                        case "-3":
                            throw new VKException(ExceptionMessage.OperationIsProhibitedByPrivacy);
                        default:
                            throw new VKException(ExceptionMessage.UnknownError);
                    }
                }
                else
                    throw new VKException(ExceptionMessage.UnknownError);
            }
        }

        #endregion

        #endregion

        #region 2 (сообщ.)

        #region сообщ.

        // комментарии
        public void ClearAllPhotoComments()
        {
            try
            {
                //Cache.Cache.DeleteEntryFromCache(string.Empty, "PhotosCommentsResponseHistory");
                DataModel.Data.PhotosCommentsResponseHistoryData = null;
            }
            catch
            {
                //
            }
        }

        public int PredictPhotoCommentsCount()
        {
            int count = 0;

            try
            {
                //PhotosCommentsResponse newPhotosCommentsRespounse = Cache.Cache.LoadFromCache<PhotosCommentsResponse>(string.Empty, "PhotosCommentsResponse");
                PhotosCommentsResponse newPhotosCommentsRespounse = DataModel.Data.PhotosCommentsResponseData;

                if (newPhotosCommentsRespounse == null)
                {
                    throw new Exception();
                }

                List<int> newArrayList = new List<int>();

                foreach (var val1 in newPhotosCommentsRespounse.pcrComments)
                {
                    bool isInList = false;

                    foreach (var val2 in newArrayList)
                    {
                        if (val1.cpPhotoData.pdPhotoID == val2)
                        {
                            isInList = true;
                        }
                    }

                    if (!isInList)
                    {
                        newArrayList.Add(val1.cpPhotoData.pdPhotoID);

                        count++;
                    }
                }
            }
            catch
            {
                //
            }

            try
            {
                //UpdatesPhotosResponse newUpdatesPhotosResponse = Cache.Cache.LoadFromCache<UpdatesPhotosResponse>(string.Empty, "UpdatesPhotosResponse");
                UpdatesPhotosResponse newUpdatesPhotosResponse = DataModel.Data.UpdatesPhotosResponseData;

                if (newUpdatesPhotosResponse == null)
                {
                    throw new Exception();
                }

                count += newUpdatesPhotosResponse.uprPhotoDatas.Count;
            }
            catch
            {
                //
            }

            return count;
        }

        public bool UploadNextPhotoComments(PhotosCommentsResponseHistory newPhotosCommentsRespounseHistory, PhotosCommentsResponse newPhotosCommentsRespounse, UpdatesPhotosResponse newUpdatesPhotosResponse)
        {
            // загрузка комментариев со списка Новости (Комментарии)
            try
            {
                if (newPhotosCommentsRespounse == null)
                {
                    throw new Exception();
                }

                foreach (var val in newPhotosCommentsRespounse.pcrComments)
                {
                    try
                    {
                        PhotosCommentsResponse oldPhotosCommentsResponse = newPhotosCommentsRespounseHistory.GetItem(val.cpPhotoData.pdPhotoID);

                        if (oldPhotosCommentsResponse == null)
                        {
                            LoadCommentsToPhoto(Convert.ToInt32(_iDataLogic.GetUid()), val.cpPhotoData.pdPhotoID, 50, true, false, newPhotosCommentsRespounseHistory);

                            return true;
                        }
                    }
                    catch (VKException)
                    {
                        throw;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            catch (Exception)
            {
                //
            }

            // загрузка комментариев со списка Новости (Друзья)
            try
            {
                if (newUpdatesPhotosResponse == null)
                {
                    throw new Exception();
                }

                foreach (var val in newUpdatesPhotosResponse.uprPhotoDatas)
                {
                    try
                    {
                        PhotosCommentsResponse oldPhotosCommentsResponse = newPhotosCommentsRespounseHistory.GetItem(val.pdPhotoID);

                        if (oldPhotosCommentsResponse == null)
                        {
                            LoadCommentsToPhoto(val.pdUserID, val.pdPhotoID, 50, true, false, newPhotosCommentsRespounseHistory);

                            return true;
                        }
                    }
                    catch (VKException)
                    {
                        throw;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            catch (Exception)
            {
                //
            }

            return false;
        }



        // переписки
        public void ClearMessageCorrespondence()
        {
            try
            {
                DataModel.Data.MessageCorrespondence = null;
                //Cache.Cache.DeleteEntryFromCache(string.Empty, "MessageCorrespondence");
            }
            catch
            {
                //
            }
        }

        public int PredictCorrespondCount()
        {
            try
            {
                //MessShortCorrespondence newMessShortCorrespondence = Cache.Cache.LoadFromCache<MessShortCorrespondence>(string.Empty, "MessageShortCorrespondence");
                MessShortCorrespondence newMessShortCorrespondence = DataModel.Data.MessageShortCorrespondence;

                if (newMessShortCorrespondence != null)
                {
                    return newMessShortCorrespondence.MessList.Count;
                }
            }
            catch
            {
                //
            }

            return 0;
        }

        public bool UploadNextUserCorrespond(MessShortCorrespondence newMessShortCorrespondence, MessCorrespondence newMessCorrespondence)
        {
            try
            {
                if (newMessShortCorrespondence == null)
                {
                    throw new Exception();
                }

                try
                {
                    foreach (var val in newMessShortCorrespondence.MessList)
                    {
                        if (!newMessCorrespondence.UserIsInList(val.mUserID))
                        {
                            RefreshUserCorrespondence(val.mUserID, true, false, newMessCorrespondence); // для каждого пользователя грузим цепочку                        
                            return true;
                        }
                    }
                }
                catch (VKException)
                {
                    throw;
                }
                catch (OutOfMemoryException)
                {
                    throw;
                }
            }
            catch (Exception)
            {
                // если нет в кэше то выходим

                return false;
            }

            return false;
        }

        public void DownloadAllPhotoData()
        {
            // загрузка фото из нижнего списка (маленьких и больших)
            try
            {
                //UpdatesPhotosResponse newUpdatesPhotosResponse = Cache.Cache.LoadFromCache<UpdatesPhotosResponse>(string.Empty, "UpdatesPhotosResponse");
                UpdatesPhotosResponse newUpdatesPhotosResponse = DataModel.Data.UpdatesPhotosResponseData;

                if (newUpdatesPhotosResponse == null)
                {
                    throw new Exception();
                }

                foreach (var val in newUpdatesPhotosResponse.uprPhotoDatas)
                {
                    try
                    {
                        //_iCommunicationLogic.LoadImage(val.pdPhotoURL130px, @"Thumb\" + HttpUtility.GetMd5Hash(val.pdPhotoURL130px), false, null, UISettings.CalcPix(50), val.pdPhotoID, "int"); // это итак закачивается при старте
                        _iCommunicationLogic.LoadImage(val.pdPhotoURL604px, HttpUtility.GetMd5Hash(val.pdPhotoURL604px), false, null, 0, val.pdPhotoID, "int");
                    }
                    catch (Exception ex)
                    {
                        DebugHelper.WriteLogEntry(ex, "DownloadAllPhotoData (при обработке UpdatesPhotosResponse) вызвал Exception при вызове LoadImage для изображения: " + val.pdPhotoURL130px);
                    }
                }
            }
            catch (Exception)
            {
                //
            }

            // загрузка фото со списка Новости (Комментарии)
            try
            {
                //PhotosCommentsResponse newPhotosCommentsRespounse = Cache.Cache.LoadFromCache<PhotosCommentsResponse>(string.Empty, "PhotosCommentsResponse");
                PhotosCommentsResponse newPhotosCommentsRespounse = DataModel.Data.PhotosCommentsResponseData;

                if (newPhotosCommentsRespounse == null)
                {
                    throw new Exception();
                }

                foreach (var val in newPhotosCommentsRespounse.pcrComments)
                {
                    try
                    {
                        _iCommunicationLogic.LoadImage(val.cpPhotoData.pdPhotoURL130px, @"Thumb\" + HttpUtility.GetMd5Hash(val.cpPhotoData.pdPhotoURL130px), false, null, UISettings.CalcPix(50), val.cpPhotoData.pdPhotoID, "int");

                        string largeURL = GetPhotoLargeURLByPhotoID(val.cpPhotoData.pdPhotoID);

                        if (!string.IsNullOrEmpty(largeURL))
                        {
                            _iCommunicationLogic.LoadImage(largeURL, HttpUtility.GetMd5Hash(largeURL), false, null, 0, val.cpPhotoData.pdPhotoID, "int");
                        }
                    }
                    catch (Exception ex)
                    {
                        DebugHelper.WriteLogEntry(ex, "DownloadAllPhotoData (при обработке PhotosCommentsResponse) вызвал Exception при вызове LoadImage для изображения: " + val.cpPhotoData.pdPhotoURL130px);
                    }
                }
            }
            catch (Exception)
            {
                //
            }

            // загрузка аватаров друзей
            try
            {
                //FriendsListResponse newFriendsListResponse = Cache.Cache.LoadFromCache<FriendsListResponse>(string.Empty, "FriendsListResponse");
                FriendsListResponse newFriendsListResponse = DataModel.Data.FriendsListResponseData;

                if (newFriendsListResponse == null)
                {
                    throw new Exception();
                }

                foreach (var val in newFriendsListResponse.Users)
                {
                    try
                    {
                        _iCommunicationLogic.LoadImage(val.Photo100px, @"Thumb\" + HttpUtility.GetMd5Hash(val.Photo100px), false, null, UISettings.CalcPix(50), val.LastName, "string");
                    }
                    catch (Exception ex)
                    {
                        DebugHelper.WriteLogEntry(ex, "DownloadAllPhotoData (при обработке FriendsListResponse) вызвал Exception при вызове LoadImage для изображения: " + val.Photo100px);
                    }
                }
            }
            catch (Exception)
            {
                //
            }
        }

        //public

        /// <summary>
        /// Загружает краткую переписку с пользователями
        /// </summary>
        /// <param name="isRefresh">Указывает на необходимость обновления с сайта, если true - данные из кэша не считываются</param>
        /// <param name="restoreSession">Возобновить сессию</param>
        public MessShortCorrespondence GetShortCorrespondence(bool isRefresh, bool restoreSession)
        {
            DebugHelper.WriteLogEntry("BaseLogic.GetShortCorrespondence");

            try
            {
                if (isRefresh)
                {
                    throw new Exception();
                }

                try
                {
                    //MessShortCorrespondence newMessShortCorrespondence = Cache.Cache.LoadFromCache<MessShortCorrespondence>(string.Empty, "MessageShortCorrespondence");
                    MessShortCorrespondence newMessShortCorrespondence = DataModel.Data.MessageShortCorrespondence;
                    return newMessShortCorrespondence;
                }
                catch (IOException)
                {
                    return null;
                }

            }
            catch (Exception)
            {
                //WiFi
                if (_iDataLogic.GetOnlyWIFI() == "1")
                    if (!CoreHelper.TurnWiFi(true)) throw new VKException(ExceptionMessage.NoConnection);

                // восстановливаем сессию
                if (restoreSession)
                {
                    AutoLogin();
                }

                MessShortCorrespondence newMessCorrespondence = new MessShortCorrespondence();

                MessResponse newMessResponseInbox = null;
                MessResponse newMessResponseOutbox = null;

                ErrorResponse newErrorResponseInbox = null;
                ErrorResponse newErrorResponseOutbox = null;

                try
                {
                    newMessResponseInbox = _iCommunicationLogic.LoadMessages("inbox", _iDataLogic.GetUid(), _iDataLogic.GetSessionKey(), "0", "50", 0, out newErrorResponseInbox);
                    newMessResponseOutbox = _iCommunicationLogic.LoadMessages("outbox", _iDataLogic.GetUid(), _iDataLogic.GetSessionKey(), "0", "50", 0, out newErrorResponseOutbox);
                    //throw new ObjectDisposedException("obj1");
                }
                catch (VKException)
                {
                    throw;
                }
                catch (TimeoutException ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.GetShortCorrespondence TimeoutException Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.GetShortCorrespondence TimeoutException StackTrace: " + ex.StackTrace);

                    throw new VKException(ExceptionMessage.NoConnection);
                }
                catch (WebException ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.GetShortCorrespondence WebException Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.GetShortCorrespondence WebException StackTrace: " + ex.StackTrace);

                    throw new VKException(ExceptionMessage.NoConnection);
                }
                catch (OutOfMemoryException ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.GetShortCorrespondence OutOfMemoryException Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.GetShortCorrespondence OutOfMemoryException StackTrace: " + ex.StackTrace);

                    throw;
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.GetShortCorrespondence Exception Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.GetShortCorrespondence Exception StackTrace: " + ex.StackTrace);
                }

                if (newMessResponseInbox != null || newMessResponseOutbox != null)
                {
                    // вход.
                    foreach (MessageCover newMessageCover in newMessResponseInbox.mMessages)
                    {
                        MessShort newMessShort = newMessCorrespondence.UserIsInList(newMessageCover.mMesSender);

                        if (newMessShort == null)
                        {
                            newMessShort = new MessShort();

                            newMessShort.mID = newMessageCover.mID;

                            newMessShort.mUserID = newMessageCover.mMesSender.mUserID;
                            newMessShort.mUserName = newMessageCover.mMesSender.mUserName;
                            newMessShort.mUserPhotoURL = newMessageCover.mMesSender.mUserPhotoURL;

                            newMessShort.mLastMessageText = newMessageCover.mData.mText;
                            newMessShort.mTime = newMessageCover.mTime;
                            newMessShort.mType = newMessageCover.mIOType;

                            newMessShort.mIsRead = newMessageCover.mMesIsRead;

                            newMessCorrespondence.MessList.Add(newMessShort);
                        }
                        else
                        {
                            if (newMessageCover.mTime > newMessShort.mTime)
                            {
                                newMessShort.mID = newMessageCover.mID;

                                newMessShort.mLastMessageText = newMessageCover.mData.mText;
                                newMessShort.mTime = newMessageCover.mTime;
                                newMessShort.mType = newMessageCover.mIOType;
                            }

                            if (!newMessageCover.mMesIsRead)
                            {
                                newMessShort.mIsRead = false;
                            }
                        }
                    }

                    // исход.
                    foreach (MessageCover newMessageCover in newMessResponseOutbox.mMessages)
                    {
                        MessShort newMessShort = newMessCorrespondence.UserIsInList(newMessageCover.mMesReceiver);

                        if (newMessShort == null)
                        {
                            newMessShort = new MessShort();

                            newMessShort.mID = newMessageCover.mID;

                            newMessShort.mUserID = newMessageCover.mMesReceiver.mUserID;
                            newMessShort.mUserName = newMessageCover.mMesReceiver.mUserName;
                            newMessShort.mUserPhotoURL = newMessageCover.mMesReceiver.mUserPhotoURL;

                            newMessShort.mLastMessageText = newMessageCover.mData.mText;
                            newMessShort.mTime = newMessageCover.mTime;
                            newMessShort.mType = newMessageCover.mIOType;

                            newMessShort.mIsRead = newMessageCover.mMesIsRead;

                            newMessCorrespondence.MessList.Add(newMessShort);
                        }
                        else
                        {
                            if (newMessageCover.mTime > newMessShort.mTime)
                            {
                                newMessShort.mID = newMessageCover.mID;

                                newMessShort.mLastMessageText = newMessageCover.mData.mText;
                                newMessShort.mTime = newMessageCover.mTime;
                                newMessShort.mType = newMessageCover.mIOType;
                            }
                        }
                    }
                }
                else
                {
                    if (newErrorResponseInbox != null)
                    {
                        if (newErrorResponseInbox.error_code == "1")
                        {
                            if (!restoreSession)
                            {
                                return GetShortCorrespondence(isRefresh, true);
                            }
                            else
                            {
                                throw new VKException(ExceptionMessage.ServerUnavalible);
                            }
                        }
                        else if (newErrorResponseInbox.error_code == "2")
                        {
                            throw new VKException(ExceptionMessage.AccountBloked);
                        }
                        else
                        {
                            throw new VKException(ExceptionMessage.UnknownError);
                        }
                    }
                    else if (newErrorResponseOutbox != null)
                    {
                        if (newErrorResponseOutbox.error_code == "1")
                        {
                            if (!restoreSession)
                            {
                                return GetShortCorrespondence(isRefresh, true);
                            }
                            else
                            {
                                throw new VKException(ExceptionMessage.ServerUnavalible);
                            }
                        }
                        else if (newErrorResponseOutbox.error_code == "2")
                        {
                            throw new VKException(ExceptionMessage.AccountBloked);
                        }
                        else
                        {
                            throw new VKException(ExceptionMessage.UnknownError);
                        }
                    }
                    else
                    {
                        throw new VKException(ExceptionMessage.UnknownError);
                    }
                }

                DataModel.Data.MessageShortCorrespondence = newMessCorrespondence;
                /*
                try
                {
                    bool result;

                    result = Cache.Cache.SaveToCache(newMessCorrespondence, string.Empty, "MessageShortCorrespondence");
             
                    if (result)
                    {
                        DebugHelper.WriteLogEntry("Данные MessageShortCorrespondence сохранены в кэш.");
                    }
                    else
                    {
                        DebugHelper.WriteLogEntry("Данные MessageShortCorrespondence не сохранены в кэш.");
                    }
              
                }
                catch (Exception newException)
                {
                    DebugHelper.WriteLogEntry("Ошибка сохранения данных MessageShortCorrespondence в кэш: " + newException.Message);
                }
                */

                return newMessCorrespondence;
            }
        }

        /// <summary>
        /// Обновляет переписку с пользователем
        /// </summary>
        /// <param name="toUserId">ID пользователя, переписка с которым загружается</param>
        /// <param name="restoreSession">Возобновить сессию</param>
        public MessUserCorrespondence RefreshUserCorrespondence(int toUserId, bool isRefresh, bool restoreSession, MessCorrespondence messCorrespondence)
        {
            DebugHelper.WriteLogEntry("BaseLogic.RefreshUserCorrespondence");

            MessCorrespondence newMessCorrespondence = null;
            MessUserCorrespondence newUserCorrespondence = null;

            try
            {
                // поднимаем данные (все цепочки) из кэша
                try
                {
                    if (messCorrespondence == null)
                    {
                        newMessCorrespondence = DataModel.Data.MessageCorrespondence;
                        //newMessCorrespondence = Cache.Cache.LoadFromCache<MessCorrespondence>(string.Empty, "MessageCorrespondence");
                    }
                    else
                        newMessCorrespondence = messCorrespondence;

                    // если в кэше нет нужных данных, переходим к загрузке с сервера
                    if (newMessCorrespondence == null)
                    {
                        throw new CorrException();
                    }
                }
                catch (IOException ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.RefreshUserCorrespondence IO Exception message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.RefreshUserCorrespondence IO Exception stack trace: " + ex.StackTrace);

                    // если ошибка кэша, переходим к загрузке с сервера
                    throw new CorrException();
                }

                MessageUser mU = new MessageUser();

                mU.mUserID = toUserId;

                // есть ли среди данных переписка с пользователем?
                if (newMessCorrespondence.UserIsInList(toUserId))
                {
                    // вытаскиваем из всех цепочек одну
                    newMessCorrespondence.SeekCorrespondence(toUserId, out newUserCorrespondence);

                    if (newUserCorrespondence == null)
                    {
                        throw new CorrException();
                    }

                    if (!isRefresh)
                    {
                        return newUserCorrespondence;
                    }

                    //bool completeOperation;

                    //do
                    //{
                    //    MessageCover newMessageCover = null;

                    //    foreach (var val in newUserCorrespondence.mMessages)
                    //    {
                    //        if (val.mID == 0)
                    //        {
                    //            newMessageCover = val;

                    //            break;
                    //        }
                    //    }

                    //    if (newMessageCover != null)
                    //    {
                    //        newUserCorrespondence.mMessages.Remove(newMessageCover);

                    //        completeOperation = true;
                    //    }
                    //    else
                    //    {
                    //        completeOperation = false;
                    //    }
                    //}
                    //while (completeOperation);

                    //// удаляем сообщения с ID = 0, т.к. это пустышки
                    //for (int i = 0; i < newUserCorrespondence.mMessages.Count; i++)
                    //{
                    //    if (newUserCorrespondence.mMessages[i].mID == 0)
                    //    {
                    //        newUserCorrespondence.mMessages.RemoveAt(i);
                    //    }
                    //}

                    ErrorResponse newErrorResponseData = null;
                    MessChangesResponse newMessChangesResponse = null;

                    //WiFi
                    if (_iDataLogic.GetOnlyWIFI() == "1")
                        if (!CoreHelper.TurnWiFi(true)) throw new VKException(ExceptionMessage.NoConnection);

                    // восстановить сессию
                    if (restoreSession)
                    {
                        AutoLogin();
                    }

                    // загружаем список изменений в цепочке
                    try
                    {
                        newMessChangesResponse = _iCommunicationLogic.LoadMessChanges("message", _iDataLogic.GetUid(), _iDataLogic.GetSessionKey(), toUserId, newUserCorrespondence.VersionNum, out newErrorResponseData);
                    }
                    catch (TimeoutException ex)
                    {
                        DebugHelper.WriteLogEntry("BaseLogic.RefreshUserCorrespondence Timeout Exception message: " + ex.Message);
                        DebugHelper.WriteLogEntry("BaseLogic.RefreshUserCorrespondence Timeout Exception stack trace: " + ex.StackTrace);

                        throw new VKException(ExceptionMessage.NoConnection);
                    }
                    catch (WebException ex)
                    {
                        DebugHelper.WriteLogEntry("BaseLogic.RefreshUserCorrespondence Web Exception message: " + ex.Message);
                        DebugHelper.WriteLogEntry("BaseLogic.RefreshUserCorrespondence Web Exception stack trace: " + ex.StackTrace);

                        throw new VKException(ExceptionMessage.NoConnection);
                    }
                    catch (OutOfMemoryException ex)
                    {
                        DebugHelper.WriteLogEntry("BaseLogic.RefreshUserCorrespondence OutOfMemory Exception message: " + ex.Message);
                        DebugHelper.WriteLogEntry("BaseLogic.RefreshUserCorrespondence OutOfMemory Exception stack trace: " + ex.StackTrace);

                        throw;
                    }
                    catch (Exception ex)
                    {
                        DebugHelper.WriteLogEntry("BaseLogic.RefreshUserCorrespondence Unknown Exception message: " + ex.Message);
                        DebugHelper.WriteLogEntry("BaseLogic.RefreshUserCorrespondence Unknown Exception stack trace: " + ex.StackTrace);
                    }

                    if (newMessChangesResponse == null) // поиск ошибки
                    {
                        if (newErrorResponseData != null)
                        {
                            if (newErrorResponseData.error_code == "1")
                            {
                                if (!restoreSession)
                                {
                                    RefreshUserCorrespondence(toUserId, isRefresh, true, null);
                                }
                                else
                                {
                                    throw new VKException(ExceptionMessage.ServerUnavalible);
                                }
                            }
                            else if (newErrorResponseData.error_code == "2")
                            {
                                throw new VKException(ExceptionMessage.AccountBloked);
                            }
                            else
                            {
                                throw new VKException(ExceptionMessage.UnknownError);
                            }
                        }
                        else
                        {
                            throw new VKException(ExceptionMessage.UnknownError);
                        }
                    }
                    else
                    {
                        // применяем полученные изменения
                        // UPD:
                        // применяем только события add, чтобы минимизировать количество ошибок
                        // в случае ошибки грузим всю цепочку заново
                        foreach (MessChange сurrentChange in newMessChangesResponse.MessList)
                        {
                            // bugfix: проверка, что сообщение из нужной цепочки
                            // т.к. сервер иногда присылает хрень какую-то
                            if (сurrentChange.Message.mMesReceiver.mUserID == toUserId || сurrentChange.Message.mMesSender.mUserID == toUserId)
                            {
                                switch (сurrentChange.ActionType)
                                {
                                    case "add":
                                        // есть ли сообщение в списке?
                                        if (newUserCorrespondence.MessageIsInList(сurrentChange.Message.mID))
                                        {
                                            throw new CorrException();
                                        }

                                        newUserCorrespondence.mMessages.Add(сurrentChange.Message);
                                        newUserCorrespondence.FromCache = false;

                                        // обновляем номер версии
                                        if (newUserCorrespondence.VersionNum < сurrentChange.VersionNum)
                                        {
                                            newUserCorrespondence.VersionNum = сurrentChange.VersionNum;
                                        }

                                        break;

                                    #region //

                                    //case "del":
                                    //    if (!newUserCorrespondence.Delete(сurrentChange.Message.mID))
                                    //    {
                                    //        throw new CorrException();
                                    //    }

                                    //    if (newUserCorrespondence.VersionNum < сurrentChange.VersionNum)
                                    //    {
                                    //        newUserCorrespondence.VersionNum = сurrentChange.VersionNum;
                                    //    }

                                    //    break;

                                    //case "restore":
                                    //    if (newUserCorrespondence.MessageIsInList(сurrentChange.Message.mID))
                                    //    {
                                    //        throw new CorrException();
                                    //    }

                                    //    newUserCorrespondence.mMessages.Add(сurrentChange.Message);

                                    //    if (newUserCorrespondence.VersionNum < сurrentChange.VersionNum)
                                    //    {
                                    //        newUserCorrespondence.VersionNum = сurrentChange.VersionNum;
                                    //    }

                                    //    break;

                                    case "read":
                                        if (!newUserCorrespondence.MessageIsInList(сurrentChange.Message.mID))
                                        {
                                            throw new CorrException();
                                        }

                                        newUserCorrespondence.SeekMessage(сurrentChange.Message.mID).mMesIsRead = true;

                                        if (newUserCorrespondence.VersionNum < сurrentChange.VersionNum)
                                        {
                                            newUserCorrespondence.VersionNum = сurrentChange.VersionNum;
                                            newUserCorrespondence.FromCache = false;
                                        }

                                        break;

                                    #endregion
                                }
                            }
                        }

                        // удаляем сообщения с id = 0;
                        bool completeOperation;

                        do
                        {
                            MessageCover newMessageCover = null;

                            foreach (var val in newUserCorrespondence.mMessages)
                            {
                                if (val.mID == 0)
                                {
                                    newMessageCover = val;

                                    break;
                                }
                            }

                            if (newMessageCover != null)
                            {
                                newUserCorrespondence.mMessages.Remove(newMessageCover);
                                newUserCorrespondence.FromCache = false;
                                completeOperation = true;
                            }
                            else
                            {
                                completeOperation = false;
                            }
                        }
                        while (completeOperation);

                        try
                        {
                            //bool result;

                            if (messCorrespondence == null)
                            {
                                DataModel.Data.MessageCorrespondence = newMessCorrespondence;
                                //result = Cache.Cache.SaveToCache(newMessCorrespondence, string.Empty, "MessageCorrespondence");
                            }
                            //else
                                //result = true;
                            /*
                            if (result)
                            {
                                DebugHelper.WriteLogEntry("MessageCorrespondence сохранены в кэш.");
                            }
                            else
                            {
                                DebugHelper.WriteLogEntry("MessageCorrespondence не сохранены в кэш.");
                            }
                            */
                        }
                        catch (IOException newException)
                        {
                            DebugHelper.WriteLogEntry("Ошибка сохранения данных MessageCorrespondence в кэш: " + newException.Message);
                        }
                    }

                    return newUserCorrespondence;
                }
                else
                {
                    // переписки с пользователем не нашли
                    throw new CorrException();
                }
            }
            catch (CorrException) // прогрузка всей цепочки целиком
            {
                if (!isRefresh)
                {
                    return null;
                }

                //WiFi
                if (_iDataLogic.GetOnlyWIFI() == "1")
                    if (!CoreHelper.TurnWiFi(true)) throw new VKException(ExceptionMessage.NoConnection);

                // восстановить сессию
                if (restoreSession)
                {
                    AutoLogin();
                }

                MessResponse newMessResponse = null;
                MessageUser mU = new MessageUser();

                mU.mUserID = toUserId;

                // т.к. это список цепочек то список может быть не пустой, но гаранитровать мы этого не можем
                if (newMessCorrespondence == null)
                {
                    newMessCorrespondence = new MessCorrespondence();
                }

                // если вызывается прогрузка цепочки, то необходимо удалить старую версию и заменить ее новой...
                // если цепочка была - очищаем
                // если цепочик не было - добавляем
                if (!newMessCorrespondence.UserIsInList(toUserId))
                {
                    newUserCorrespondence = new MessUserCorrespondence(mU);

                    newMessCorrespondence.mUserCorrespondences.Add(newUserCorrespondence);
                }
                else
                {
                    newMessCorrespondence.SeekCorrespondence(toUserId, out newUserCorrespondence);

                    newUserCorrespondence.mMessages.Clear();

                    newUserCorrespondence.FromCache = false;

                    newUserCorrespondence.VersionNum = 0;
                }

                ErrorResponse newErrorResponseData = null;

                // размер блока которым мы гшрузим цепочку
                int messBlockSize = 50;

                // количество скачанных сообщений
                int messCount = 0;

                // общее количество сообщений
                int messOnServerCount = messBlockSize;

                while (messCount < messOnServerCount)
                {
                    newMessResponse = null;
                    newErrorResponseData = null;

                    // поставить обработчик!!!
                    try
                    {
                        newMessResponse = _iCommunicationLogic.LoadMessages("message", _iDataLogic.GetUid(), _iDataLogic.GetSessionKey(), Convert.ToString(messCount), Convert.ToString(messCount + messBlockSize), toUserId, out newErrorResponseData);
                    }
                    catch (TimeoutException ex)
                    {
                        DebugHelper.WriteLogEntry("BaseLogic.RefreshUserCorrespondence Timeout Exception message: " + ex.Message);
                        DebugHelper.WriteLogEntry("BaseLogic.RefreshUserCorrespondence Timeout Exception stack trace: " + ex.StackTrace);

                        throw new VKException(ExceptionMessage.NoConnection);
                    }
                    catch (WebException ex)
                    {
                        DebugHelper.WriteLogEntry("BaseLogic.RefreshUserCorrespondence Web Exception message: " + ex.Message);
                        DebugHelper.WriteLogEntry("BaseLogic.RefreshUserCorrespondence Web Exception stack trace: " + ex.StackTrace);

                        throw new VKException(ExceptionMessage.NoConnection);
                    }
                    catch (OutOfMemoryException ex)
                    {
                        DebugHelper.WriteLogEntry("BaseLogic.RefreshUserCorrespondence OutOfMemory Exception message: " + ex.Message);
                        DebugHelper.WriteLogEntry("BaseLogic.RefreshUserCorrespondence OutOfMemory Exception stack trace: " + ex.StackTrace);

                        throw;
                    }
                    catch (Exception ex)
                    {
                        DebugHelper.WriteLogEntry("BaseLogic.RefreshUserCorrespondence Unknown Exception message: " + ex.Message);
                        DebugHelper.WriteLogEntry("BaseLogic.RefreshUserCorrespondence Unknown Exception stack trace: " + ex.StackTrace);
                    }

                    if (newMessResponse == null) // ищем ошибку
                    {
                        if (newErrorResponseData != null)
                        {
                            if (newErrorResponseData.error_code == "1")
                            {
                                if (!restoreSession)
                                {
                                    RefreshUserCorrespondence(toUserId, isRefresh, true, null);
                                }
                                else
                                {
                                    throw new VKException(ExceptionMessage.ServerUnavalible);
                                }
                            }
                            else if (newErrorResponseData.error_code == "2")
                            {
                                throw new VKException(ExceptionMessage.AccountBloked);
                            }
                            else
                            {
                                throw new VKException(ExceptionMessage.UnknownError);
                            }
                        }
                        else
                        {
                            throw new VKException(ExceptionMessage.UnknownError);
                        }
                    }
                    else
                    {
                        messOnServerCount = newMessResponse.mCount;

                        // учитываем изменения ts
                        if (newUserCorrespondence.VersionNum < newMessResponse.VersionNum)
                        {
                            newUserCorrespondence.VersionNum = newMessResponse.VersionNum;         
                        }

                        foreach (MessageCover CurrentMessage in newMessResponse.mMessages)
                        {
                            newUserCorrespondence.mMessages.Add(CurrentMessage);
                        }

                        newUserCorrespondence.FromCache = false;

                        messCount += messBlockSize;
                    }
                }

                try
                {
                    bool result;

                    if (messCorrespondence == null)
                    {
                        DataModel.Data.MessageCorrespondence = newMessCorrespondence;
                        //result = Cache.Cache.SaveToCache(newMessCorrespondence, string.Empty, "MessageCorrespondence");
                    }

                    //else
                        //result = true;

                    /*
                    if (result)
                    {
                        DebugHelper.WriteLogEntry("MessageCorrespondence сохранены в кэш.");
                    }
                    else
                    {
                        DebugHelper.WriteLogEntry("MessageCorrespondence не сохранены в кэш.");
                    }
                    */
                }
                catch (IOException newException)
                {
                    DebugHelper.WriteLogEntry("Ошибка сохранения данных MessageCorrespondence в кэш: " + newException.Message);
                }

                return newUserCorrespondence;
            }
        }

        /// <summary>
        /// Отправляет сообщение пользователю
        /// </summary>
        /// <param name="uid">ID пользователя, которому отправляется сообщение</param>
        /// <param name="text">Текст сообщения</param>
        /// <param name="restoreSession">Возобновить сессию</param>
        public bool SendMessage(int uid, string text, bool restoreSession)
        {
            DebugHelper.WriteLogEntry("BaseLogic.SendMessage");

            ErrorResponse newErrorResponse = null;

            //WiFi
            if (_iDataLogic.GetOnlyWIFI() == "1")
                if (!CoreHelper.TurnWiFi(true)) throw new VKException(ExceptionMessage.NoConnection);

            if (restoreSession)
            {
                AutoLogin();
            }

            bool result = false;

            try
            {
                result = _iCommunicationLogic.SendMessage(_iDataLogic.GetUid(), _iDataLogic.GetSessionKey(), uid, text, out newErrorResponse);
            }
            catch (VKException)
            {
                throw;
            }
            catch (TimeoutException ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.SendMessage TimeoutException Message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.SendMessage TimeoutException StackTrace: " + ex.StackTrace);

                throw new VKException(ExceptionMessage.NoConnection);
            }
            catch (WebException ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.SendMessage WebException Message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.SendMessage WebException StackTrace: " + ex.StackTrace);

                throw new VKException(ExceptionMessage.NoConnection);
            }
            catch (Exception ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.SendMessage Exception Message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.SendMessage Exception StackTrace: " + ex.StackTrace);
            }

            if (!result) // result = true только если от сервера пришло подтверждение, что сообщение отправлено
            {
                if (newErrorResponse != null) // ловим ошибку
                {
                    if (newErrorResponse.error_code == "1")
                    {
                        if (!restoreSession)
                        {
                            return SendMessage(uid, text, true);
                        }
                        else
                        {
                            throw new VKException(ExceptionMessage.ServerUnavalible);
                        }
                    }
                    else if (newErrorResponse.error_code == "2")
                    {
                        throw new VKException(ExceptionMessage.AccountBloked);
                    }
                    else if (newErrorResponse.error_code == "3")
                    {
                        throw new VKException(ExceptionMessage.OperationIsProhibitedByPrivacy);
                    }
                    else
                    {
                        throw new VKException(ExceptionMessage.UnknownError);
                    }
                }
                //else
                //{
                //    throw new VKException(ExceptionMessage.UnknownError);
                //}
                // закомменчано т.к. часть логики похоже перекрывается...
            }

            return result;
        }

        public void SaveSendMessageToCache(int uid, string text)
        {
            try
            {
                MessUserCorrespondence newUserCorrespondence = null;
                
                //MessCorrespondence newMessCorrespondence = Cache.Cache.LoadFromCache<MessCorrespondence>(string.Empty, "MessageCorrespondence");
                MessCorrespondence newMessCorrespondence = DataModel.Data.MessageCorrespondence;

                if (newMessCorrespondence == null)
                {
                    newMessCorrespondence = new MessCorrespondence();

                    // заводим пользователя
                    newUserCorrespondence = new MessUserCorrespondence();

                    newMessCorrespondence.mUserCorrespondences.Add(newUserCorrespondence);
                }
                else
                {
                    newMessCorrespondence.SeekCorrespondence(uid, out newUserCorrespondence);

                    if (newUserCorrespondence == null)
                    {
                        // заводим пользователя
                        newUserCorrespondence = new MessUserCorrespondence();

                        newMessCorrespondence.mUserCorrespondences.Add(newUserCorrespondence);
                    }
                }

                newUserCorrespondence.mMesUser.mUserID = uid;

                MessageCover newMessageCover = new MessageCover();

                newUserCorrespondence.mMessages.Add(newMessageCover);

                newMessageCover.mID = 0;
                newMessageCover.mTime = DateTime.Now.ToLocalTime();
                newMessageCover.mIOType = MessageIOType.Outbox;

                newMessageCover.mData = new MessageData();

                newMessageCover.mData.mText = text;

                // Обновляем в MessageShortCorrespondence
                MessShortCorrespondence shortCorr = DataModel.Data.MessageShortCorrespondence;
                MessShort mess = shortCorr.UserIsInList(new MessageUser {mUserID = uid}) ?? new MessShort();
                mess.mID = 0;
                mess.mIsRead = true;
                mess.mLastMessageText = text;
                mess.mTime = newMessageCover.mTime;
                mess.mType = MessageIOType.Outbox;
                mess.mUserID = uid;
                if (shortCorr.UserIsInList(new MessageUser { mUserID = uid }) == null)
                {
                    User u = DataModel.Data.FriendsListResponseData.GetUserByID(uid.ToString());
                    mess.mUserName = u.FullName;
                    mess.mUserPhotoURL = u.Photo100px;
                }

                // сохранить все в кэш
                try
                {
                    bool result;

                    DataModel.Data.MessageCorrespondence = newMessCorrespondence;
                    DataModel.Data.MessageShortCorrespondence = shortCorr;
                    /*
                    result = Cache.Cache.SaveToCache(newMessCorrespondence, string.Empty, "MessageCorrespondence");

                    if (result)
                    {
                        DebugHelper.WriteLogEntry("MessageCorrespondence сохранены в кэш.");
                    }
                    else
                    {
                        DebugHelper.WriteLogEntry("MessageCorrespondence не сохранены в кэш.");
                    }
                     */
                }
                catch (IOException newException)
                {
                    DebugHelper.WriteLogEntry("Ошибка сохранения данных MessageCorrespondence в кэш: " + newException.Message);
                }
            }
            catch (Exception)
            {
                //
            }

            //try
            //    {
            //        newMessCorrespondence = Cache.Cache.LoadFromCache<MessCorrespondence>(string.Empty, "MessageCorrespondence");

            //        // если в кэше нет нужных данных, переходим к загрузке с сервера
            //        if (newMessCorrespondence == null)
            //        {
            //            throw new CorrException();
            //        }
            //    }
            //    catch (IOException ex)
            //    {
            //        DebugHelper.WriteLogEntry("BaseLogic.RefreshUserCorrespondence IO Exception message: " + ex.Message);
            //        DebugHelper.WriteLogEntry("BaseLogic.RefreshUserCorrespondence IO Exception stack trace: " + ex.StackTrace);

            //        // если ошибка кэша, переходим к загрузке с сервера
            //        throw new CorrException();
            //    }

            //    MessageUser mU = new MessageUser();

            //    mU.mUserID = toUserId;

            //    // есть ли среди данных переписка с пользователем?
            //    if (newMessCorrespondence.UserIsInList(toUserId))
            //    {
            //        // вытаскиваем из всех цепочек одну
            //        newMessCorrespondence.SeekCorrespondence(toUserId, out newUserCorrespondence);

            //        if (newUserCorrespondence == null)
            //        {
            //            throw new CorrException();
            //        }
        }

        #endregion

        #endregion

        #region 3 (друзья, новости, комментарии)

        #region загрузка списков друзей, новостей, комментариев

        /// <summary>
        /// Получение имени пользователя по его ID (информация поднимается из кэша...)
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public string GetFriendName(string uid)
        {
            string result = string.Empty;

            try
            {
                //FriendsListResponse newFriendsListResponse = Cache.Cache.LoadFromCache<FriendsListResponse>(string.Empty, "FriendsListResponse");
                FriendsListResponse newFriendsListResponse = DataModel.Data.FriendsListResponseData;

                foreach (User newUser in newFriendsListResponse.Users)
                {
                    if (newUser.Uid.Equals(uid))
                    {
                        return newUser.LastName + " " + newUser.FirstName;
                    }
                }
            }
            catch
            {
                //
            }

            return result;
        }

        public FriendsListResponse LoadFriendsList(bool isRefresh, bool restoreSession)
        {
            DebugHelper.WriteLogEntry("BaseLogic.LoadFriendsList");

            try
            {
                if (isRefresh)
                {
                    throw new Exception();
                }

                //try
                //{
                //    FriendsListResponse newFriendsListResponse = Cache.Cache.LoadFromCache<FriendsListResponse>(string.Empty, "FriendsListResponse");

                //    return newFriendsListResponse;
                //}
                //catch (IOException)
                //{
                //    return null;
                //}

                return DataModel.Data.FriendsListResponseData;
            }
            catch
            {
                //WiFi
                if (_iDataLogic.GetOnlyWIFI() == "1")
                {
                    if (!CoreHelper.TurnWiFi(true))
                    {
                        throw new VKException(ExceptionMessage.NoConnection);
                    }
                }

                if (restoreSession)
                {
                    AutoLogin();
                }

                ErrorResponse newErrorResponseLoad = null;

                FriendsListResponse newFriendsListResponse = null;

                // выполняем запрос на получение списка друзей
                try
                {
                    newFriendsListResponse = _iCommunicationLogic.LoadFriendsListData(_iDataLogic.GetUid(), _iDataLogic.GetSessionKey(), out newErrorResponseLoad);

                    //_iDataLogic.SetToken(string.Empty);
                }
                catch (VKException)
                {
                    throw;
                }
                catch (TimeoutException ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.LoadFriendsList TimeoutException Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.LoadFriendsList TimeoutException StackTrace: " + ex.StackTrace);

                    throw new VKException(ExceptionMessage.NoConnection);
                }
                catch (WebException ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.LoadFriendsList WebException Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.LoadFriendsList WebException StackTrace: " + ex.StackTrace);

                    throw new VKException(ExceptionMessage.NoConnection);
                }
                catch (OutOfMemoryException ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.LoadFriendsList OutOfMemoryException Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.LoadFriendsList OutOfMemoryException StackTrace: " + ex.StackTrace);

                    throw;
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.LoadFriendsList Exception Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.LoadFriendsList Exception StackTrace: " + ex.StackTrace);
                }

                if (newFriendsListResponse == null)
                {
                    if (newErrorResponseLoad != null)
                    {
                        if (newErrorResponseLoad.error_code == "1")
                        {
                            if (!restoreSession)
                            {
                                return LoadFriendsList(isRefresh, true);
                            }
                            else
                            {
                                throw new VKException(ExceptionMessage.ServerUnavalible);
                            }
                        }
                        else if (newErrorResponseLoad.error_code == "2")
                        {
                            throw new VKException(ExceptionMessage.AccountBloked);
                        }
                        else
                        {
                            throw new VKException(ExceptionMessage.UnknownError);
                        }
                    }
                    else
                    {
                        //throw new VKException(ExceptionMessage.UnknownError);
                    }
                }

                ////сохраняем в кэш
                //try
                //{
                //    DebugHelper.WriteLogEntry("(Кэш) Сохранение данных FriendsListResponse...");

                //    bool result;

                //    result = Cache.Cache.SaveToCache(newFriendsListResponse, string.Empty, "FriendsListResponse");

                //    if (result)
                //    {
                //        DebugHelper.WriteLogEntry("(Кэш) Данные FriendsListResponse успешно сохранены.");
                //    }
                //    else
                //    {
                //        DebugHelper.WriteLogEntry("(Кэш) Данные FriendsListResponse не сохранены.");
                //    }
                //}
                //catch (IOException)
                //{
                //    DebugHelper.WriteLogEntry("(Кэш) В процессе сохранения данных FriendsListResponse произошла ошибка.");
                //}

                if (newFriendsListResponse != null)
                {
                    DataModel.Data.FriendsListResponseData = newFriendsListResponse;
                }

                return newFriendsListResponse;
            }
        }

        public FriendsListResponse LoadFriendsOnlineList(bool restoreSession)
        {
            DebugHelper.WriteLogEntry("BaseLogic.LoadFriendsOnlineList");

            //WiFi
            if (_iDataLogic.GetOnlyWIFI() == "1")
                if (!CoreHelper.TurnWiFi(true)) throw new VKException(ExceptionMessage.NoConnection);

            if (restoreSession)
            {
                AutoLogin();
            }

            FriendsListResponse newFriendsOnlineListResponse = null;

            ErrorResponse newErrorResponseLoad = null;

            // выполняем запрос на получение списка друзей
            try
            {
                newFriendsOnlineListResponse = _iCommunicationLogic.LoadFriendsOnlineListData(_iDataLogic.GetUid(), _iDataLogic.GetSessionKey(), out newErrorResponseLoad);
            }
            catch (VKException)
            {
                throw;
            }
            catch (TimeoutException ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.LoadFriendsList TimeoutException Message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.LoadFriendsList TimeoutException StackTrace: " + ex.StackTrace);

                throw new VKException(ExceptionMessage.NoConnection);
            }
            catch (WebException ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.LoadFriendsList WebException Message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.LoadFriendsList WebException StackTrace: " + ex.StackTrace);

                throw new VKException(ExceptionMessage.NoConnection);
            }
            catch (OutOfMemoryException ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.LoadFriendsList OutOfMemoryException Message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.LoadFriendsList OutOfMemoryException StackTrace: " + ex.StackTrace);

                throw;
            }
            catch (Exception ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.LoadFriendsList Exception Message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.LoadFriendsList Exception StackTrace: " + ex.StackTrace);
            }

            if (newFriendsOnlineListResponse == null)
            {
                if (newErrorResponseLoad != null)
                {
                    if (newErrorResponseLoad.error_code == "1")
                    {
                        if (!restoreSession)
                        {
                            return LoadFriendsOnlineList(true);
                        }
                        else
                        {
                            throw new VKException(ExceptionMessage.ServerUnavalible);
                        }
                    }
                    else if (newErrorResponseLoad.error_code == "2")
                    {
                        throw new VKException(ExceptionMessage.AccountBloked);
                    }
                    else
                    {
                        throw new VKException(ExceptionMessage.UnknownError);
                    }
                }
                else
                {
                    //throw new VKException(ExceptionMessage.UnknownError);
                }
            }

            return newFriendsOnlineListResponse;
        }

        /// <summary>
        /// грузим обновления комментариев ДРУЗЕЙ пользователя
        /// </summary>
        /// <param name="count"></param>
        /// <param name="isRefresh"></param>
        /// <param name="restoreSession"></param>
        /// <returns></returns>
        public ActivityResponse LoadActivityDataList(int count, bool isRefresh, bool restoreSession)
        {
            DebugHelper.WriteLogEntry("BaseLogic.LoadActivityDataList");

            try
            {
                if (isRefresh)
                {
                    throw new IOException();
                }

                //try
                //{
                //    ActivityResponse cacheActivityResponse = Cache.Cache.LoadFromCache<ActivityResponse>(string.Empty, "ActivityResponse");
                //    return cacheActivityResponse;
                //}
                //catch (IOException)
                //{
                //    return null;
                //}

                return DataModel.Data.ActivityResponseData;
            }
            catch (IOException)
            {
                //WiFi
                if (_iDataLogic.GetOnlyWIFI() == "1")
                    if (!CoreHelper.TurnWiFi(true)) throw new VKException(ExceptionMessage.NoConnection);

                if (restoreSession)
                {
                    AutoLogin();
                }

                ErrorResponse loadActivityDataListDataErrorResponse = null;

                ActivityResponse loadActivityDataListDataActivityResponse = null;
                //FriendsListResponse loadFriendsListDataFriendsListResponse = null;

                //считываем требуемое количество обновлений
                try
                {
                    loadActivityDataListDataActivityResponse = _iCommunicationLogic.LoadActivityDataListData(_iDataLogic.GetUid(), _iDataLogic.GetSessionKey(), "0", Convert.ToString(count - 1), out loadActivityDataListDataErrorResponse);
                }
                catch (VKException)
                {
                    throw;
                }
                catch (TimeoutException ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.LoadActivityDataList TimeoutException Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.LoadActivityDataList TimeoutException StackTrace: " + ex.StackTrace);

                    throw new VKException(ExceptionMessage.NoConnection);
                }
                catch (WebException ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.LoadActivityDataList WebException Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.LoadActivityDataList WebException StackTrace: " + ex.StackTrace);

                    throw new VKException(ExceptionMessage.NoConnection);
                }
                catch (OutOfMemoryException ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.LoadActivityDataList OutOfMemoryException Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.LoadActivityDataList OutOfMemoryException StackTrace: " + ex.StackTrace);

                    throw;
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.LoadActivityDataList Exception Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.LoadActivityDataList Exception StackTrace: " + ex.StackTrace);
                }

                if (loadActivityDataListDataActivityResponse != null)
                {
                    //// фотки юзверей берем из списка друзей
                    //try
                    //{
                    //    loadFriendsListDataFriendsListResponse = LoadFriendsList(isRefresh, false);
                    //}
                    //catch (VKException)
                    //{
                    //    throw;
                    //}
                    //catch (OutOfMemoryException)
                    //{
                    //    throw;
                    //}
                    //catch (Exception)
                    //{
                    //    //
                    //}

                    //if (loadFriendsListDataFriendsListResponse != null)
                    //{
                    //    foreach (ActivityData newActivityData in loadActivityDataListDataActivityResponse.arActivityDatas)
                    //    {
                    //        foreach (User newUser in loadFriendsListDataFriendsListResponse.Users)
                    //        {
                    //            if (newActivityData.adDataSender.psUserID.ToString() == newUser.Uid)
                    //            {
                    //                newActivityData.adDataSender.psUserPhotoURL = newUser.Photo100px;
                    //            }
                    //        }
                    //    }
                    //}
                }
                else
                {
                    if (loadActivityDataListDataErrorResponse != null)
                    {
                        if (loadActivityDataListDataErrorResponse.error_code == "1")
                        {
                            if (!restoreSession)
                            {
                                return LoadActivityDataList(count, isRefresh, true);
                            }
                            else
                            {
                                throw new VKException(ExceptionMessage.ServerUnavalible);
                            }
                        }
                        else if (loadActivityDataListDataErrorResponse.error_code == "2")
                        {
                            throw new VKException(ExceptionMessage.AccountBloked);
                        }
                        else
                        {
                            throw new VKException(ExceptionMessage.UnknownError);
                        }
                    }
                    else
                    {
                        //throw new VKException(ExceptionMessage.UnknownError);
                    }
                }

                ////сохраняем в кэш
                //try
                //{
                //    DebugHelper.WriteLogEntry("(Кэш) Сохранение данных ActivityResponse...");

                //    bool result;

                //    result = Cache.Cache.SaveToCache(loadActivityDataListDataActivityResponse, string.Empty, "ActivityResponse");

                //    if (result)
                //    {
                //        DebugHelper.WriteLogEntry("(Кэш) Данные ActivityResponse успешно сохранены.");
                //    }
                //    else
                //    {
                //        DebugHelper.WriteLogEntry("(Кэш) Данные ActivityResponse не сохранены.");
                //    }
                //}
                //catch (IOException)
                //{
                //    DebugHelper.WriteLogEntry("(Кэш) В процессе сохранения данных ActivityResponse произошла ошибка.");
                //}

                if (loadActivityDataListDataActivityResponse != null)
                {
                    DataModel.Data.ActivityResponseData = loadActivityDataListDataActivityResponse;
                }

                return loadActivityDataListDataActivityResponse;
            }
        }

        /// <summary>
        /// Функция для записи нового статуса в кэш статусов пользователя
        /// </summary>
        /// <param name="newStatus">Статус, который необходимо записать</param>
        /// <param name="emptyStatus">Значение, которое необходимо присвоить пустому статусу</param>
        /// <param name="MessageI">Значение, которое надо присвоить временно владельцу статуса</param>
        public void UpdateCacheOfUserActivities(string newStatus, string emptyStatus, string MessageI)
        {
            ActivityResponse newActivityResponse = null;

            try
            {
                //newActivityResponse = Cache.Cache.LoadFromCache<ActivityResponse>(string.Empty, "UserActivityResponse");
                newActivityResponse = DataModel.Data.UserActivityResponseData;

                if (newActivityResponse == null)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                newActivityResponse = new ActivityResponse();
            }

            bool isNeed = true;

            if (newStatus == string.Empty)
            {
                foreach (ActivityData ad in newActivityResponse.arActivityDatas)
                {
                    if (ad.adText == emptyStatus)
                    {
                        isNeed = false;
                        break;
                    }
                }

                if (isNeed) newStatus = emptyStatus;
            }
            else
            {
                foreach (ActivityData ad in newActivityResponse.arActivityDatas)
                {
                    if (ad.adText == emptyStatus)
                    {
                        newActivityResponse.arActivityDatas.Remove(ad);
                        break;
                    }
                }
            }

            if (isNeed)
            {

                var newPostSender = new PostSender
                                        {
                                            psUserID = Convert.ToInt32(_iDataLogic.GetUid()),
                                            psUserName = MessageI
                                        };

                var newActivityData = new ActivityData
                                          {
                                              adDataSender = newPostSender,
                                              adStatusID = 0,
                                              adTime = DateTime.Now,
                                              adText = newStatus
                                          };
                newActivityResponse.arActivityDatas.Add(newActivityData);

                //try
                //{
                //    bool result = Cache.Cache.SaveToCache(newActivityResponse, string.Empty, "UserActivityResponse");
                //    DebugHelper.WriteLogEntry(result
                //                                  ? "Новый статус пользователя сохранен в кэш."
                //                                  : "Новый статус пользователя не сохранены в кэш.");
                //}
                //catch (IOException newException)
                //{
                //    DebugHelper.WriteLogEntry("Ошибка сохранения статуса пользователя в кэш: " +
                //                              newException.Message);
                //}

                if (newActivityResponse != null)
                {
                    DataModel.Data.UserActivityResponseData = newActivityResponse;
                }
            }
        }

        /// <summary>
        /// грузим обновления комментариев ПОЛЬЗОВАТЕЛЯ
        /// </summary>
        /// <param name="count"></param>
        /// <param name="isRefresh"></param>
        /// <param name="restoreSession"></param>
        /// <returns></returns>
        public ActivityResponse LoadUserActivityDataList(int count, bool isRefresh, bool restoreSession)
        {
            DebugHelper.WriteLogEntry("BaseLogic.LoadUserActivityDataList");

            try
            {
                if (isRefresh)
                {
                    throw new Exception();
                }

                //try
                //{
                //    ActivityResponse cacheActivityResponse = Cache.Cache.LoadFromCache<ActivityResponse>(string.Empty, "UserActivityResponse");
                //    return cacheActivityResponse;
                //}
                //catch (IOException)
                //{
                //    return null;
                //}

                return DataModel.Data.UserActivityResponseData;
            }
            catch (Exception)
            {
                //WiFi
                if (_iDataLogic.GetOnlyWIFI() == "1")
                    if (!CoreHelper.TurnWiFi(true)) throw new VKException(ExceptionMessage.NoConnection);

                if (restoreSession)
                {
                    AutoLogin();
                }

                ErrorResponse newErrorResponse = null;
                ActivityResponse newActivityResponse = null;

                // считываем требуемое количество обновлений
                try
                {
                    newActivityResponse = _iCommunicationLogic.LoadUserActivity(_iDataLogic.GetUid(), _iDataLogic.GetSessionKey(), "0", Convert.ToString(count - 1), out newErrorResponse);
                }
                catch (VKException)
                {
                    throw;
                }
                catch (TimeoutException ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.LoadUserActivityDataList TimeoutException Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.LoadUserActivityDataList TimeoutException StackTrace: " + ex.StackTrace);

                    throw new VKException(ExceptionMessage.NoConnection);
                }
                catch (WebException ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.LoadUserActivityDataList WebException Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.LoadUserActivityDataList WebException StackTrace: " + ex.StackTrace);

                    throw new VKException(ExceptionMessage.NoConnection);
                }
                catch (OutOfMemoryException ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.LoadUserActivityDataList OutOfMemoryException Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.LoadUserActivityDataList OutOfMemoryException StackTrace: " + ex.StackTrace);

                    throw;
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.LoadUserActivityDataList Exception Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.LoadUserActivityDataList Exception StackTrace: " + ex.StackTrace);
                }

                if (newActivityResponse == null) // ищем ошибки
                {
                    if (newErrorResponse != null)
                    {
                        if (newErrorResponse.error_code == "1")
                        {
                            if (!restoreSession)
                            {
                                return LoadUserActivityDataList(count, isRefresh, true);
                            }
                            else
                            {
                                throw new VKException(ExceptionMessage.ServerUnavalible);
                            }
                        }
                        else if (newErrorResponse.error_code == "2")
                        {
                            throw new VKException(ExceptionMessage.AccountBloked);
                        }
                        else
                        {
                            throw new VKException(ExceptionMessage.UnknownError);
                        }
                    }
                    else
                    {
                        //throw new VKException(ExceptionMessage.UnknownError);
                    }
                }

                ////сохраняем в кэш
                //try
                //{
                //    DebugHelper.WriteLogEntry("(Кэш) Сохранение данных UserActivityResponse...");

                //    bool result;

                //    result = Cache.Cache.SaveToCache(newActivityResponse, string.Empty, "UserActivityResponse");

                //    if (result)
                //    {
                //        DebugHelper.WriteLogEntry("(Кэш) Данные UserActivityResponse успешно сохранены.");
                //    }
                //    else
                //    {
                //        DebugHelper.WriteLogEntry("(Кэш) Данные UserActivityResponse не сохранены.");
                //    }
                //}
                //catch (IOException)
                //{
                //    DebugHelper.WriteLogEntry("(Кэш) В процессе сохранения данных UserActivityResponse произошла ошибка.");
                //}

                if (newActivityResponse != null)
                {
                    DataModel.Data.UserActivityResponseData = newActivityResponse;
                }

                return newActivityResponse;
            }
        }

        /// <summary>
        /// Загрузка списка комментариев к фотографиям пользователя
        /// </summary>
        /// <param name="count"></param>
        /// <param name="isRefresh"></param>
        /// <param name="restoreSession"></param>
        /// <returns></returns>
        public PhotosCommentsResponse LoadPhotosComments(int count, bool isRefresh, bool restoreSession)
        {
            DebugHelper.WriteLogEntry("BaseLogic.LoadPhotosComments");

            try
            {
                if (isRefresh)
                {
                    throw new IOException();
                }

                //try
                //{
                //    PhotosCommentsResponse newPhotosCommentsRespounse = Cache.Cache.LoadFromCache<PhotosCommentsResponse>(string.Empty, "PhotosCommentsResponse");
                //    return newPhotosCommentsRespounse;
                //}
                //catch (IOException)
                //{
                //    return null;
                //}

                return DataModel.Data.PhotosCommentsResponseData;
            }
            catch (IOException)
            {
                //WiFi
                if (_iDataLogic.GetOnlyWIFI() == "1")
                    if (!CoreHelper.TurnWiFi(true)) throw new VKException(ExceptionMessage.NoConnection);

                if (restoreSession)
                {
                    AutoLogin();
                }

                ErrorResponse newErrorResponse = null;

                PhotosCommentsResponse newPhotosCommentsResponse = null;

                //считываем требуемое количество комментариев к фотографиям пользователя
                try
                {
                    newPhotosCommentsResponse = _iCommunicationLogic.LoadPhotosCommentsData(_iDataLogic.GetUid(), _iDataLogic.GetSessionKey(), "0", Convert.ToString(count - 1), "-1", out newErrorResponse);
                }
                catch (VKException)
                {
                    throw;
                }
                catch (TimeoutException ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.LoadPhotosComments TimeoutException Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.LoadPhotosComments TimeoutException StackTrace: " + ex.StackTrace);

                    throw new VKException(ExceptionMessage.NoConnection);
                }
                catch (WebException ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.LoadPhotosComments WebException Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.LoadPhotosComments WebException StackTrace: " + ex.StackTrace);

                    throw new VKException(ExceptionMessage.NoConnection);
                }
                catch (OutOfMemoryException ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.LoadPhotosComments OutOfMemoryException Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.LoadPhotosComments OutOfMemoryException StackTrace: " + ex.StackTrace);

                    throw;
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.LoadPhotosComments Exception Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.LoadPhotosComments Exception StackTrace: " + ex.StackTrace);
                }

                if (newPhotosCommentsResponse == null)
                {
                    if (newErrorResponse != null)
                    {
                        if (newErrorResponse.error_code == "1")
                        {
                            if (!restoreSession)
                            {
                                return LoadPhotosComments(count, isRefresh, true);
                            }
                            else
                            {
                                throw new VKException(ExceptionMessage.ServerUnavalible);
                            }
                        }
                        else if (newErrorResponse.error_code == "2")
                        {
                            throw new VKException(ExceptionMessage.AccountBloked);
                        }
                        else
                        {
                            throw new VKException(ExceptionMessage.UnknownError);
                        }
                    }
                    else
                    {
                        //throw new VKException(ExceptionMessage.UnknownError);
                    }
                }

                ////сохраняем в кэш
                //try
                //{
                //    DebugHelper.WriteLogEntry("(Кэш) Сохранение данных PhotosCommentsResponse...");

                //    bool result;

                //    result = Cache.Cache.SaveToCache(newPhotosCommentsResponse, string.Empty, "PhotosCommentsResponse");

                //    if (result)
                //    {
                //        DebugHelper.WriteLogEntry("(Кэш) Данные PhotosCommentsResponse успешно сохранены.");
                //    }
                //    else
                //    {
                //        DebugHelper.WriteLogEntry("(Кэш) Данные PhotosCommentsResponse не сохранены.");
                //    }
                //}
                //catch (IOException)
                //{
                //    DebugHelper.WriteLogEntry("(Кэш) В процессе сохранения данных PhotosCommentsResponse произошла ошибка.");
                //}

                if (newPhotosCommentsResponse != null)
                {
                    DataModel.Data.PhotosCommentsResponseData = newPhotosCommentsResponse;
                }

                return newPhotosCommentsResponse;
            }
        }

        /// <summary>
        /// Загрузка списка комменарием для конкретного фото
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="photoID"></param>
        /// <param name="count"></param>
        /// <param name="isRefresh"></param>
        /// <param name="restoreSession"></param>
        /// <returns></returns>
        public PhotosCommentsResponse LoadCommentsToPhoto(int uid, int photoID, int count, bool isRefresh, bool restoreSession, PhotosCommentsResponseHistory photosCommentsRespounseHistory)
        {
            DebugHelper.WriteLogEntry("BaseLogic.LoadCommentsToPhoto");

            // т.к. "общий" кэш все равно нужен в работе
            PhotosCommentsResponseHistory newPhotosCommentsRespounseHistory;

            try
            {
                if (photosCommentsRespounseHistory == null)
                {
                    //newPhotosCommentsRespounseHistory = Cache.Cache.LoadFromCache<PhotosCommentsResponseHistory>(string.Empty, "PhotosCommentsResponseHistory");
                    newPhotosCommentsRespounseHistory = DataModel.Data.PhotosCommentsResponseHistoryData;
                }
                else
                {
                    newPhotosCommentsRespounseHistory = photosCommentsRespounseHistory;
                }

                if (newPhotosCommentsRespounseHistory == null)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                newPhotosCommentsRespounseHistory = new PhotosCommentsResponseHistory();
            }

            try
            {
                if (isRefresh)
                {
                    throw new Exception();
                }

                // ищем нужный айтем
                PhotosCommentsResponse newPhotosCommentsResponse = newPhotosCommentsRespounseHistory.GetItem(photoID);

                if (newPhotosCommentsResponse != null)
                {
                    return newPhotosCommentsResponse; // выводим, если нашли
                }
                else
                {
                    throw new Exception(); // обращаемся к серверу, если нет
                }
            }
            catch (Exception)
            {
                if (!isRefresh) return null;

                //WiFi
                if (_iDataLogic.GetOnlyWIFI() == "1")
                {
                    if (!CoreHelper.TurnWiFi(true))
                    {
                        throw new VKException(ExceptionMessage.NoConnection);
                    }
                }

                // восстанавливаем сессию
                if (restoreSession)
                {
                    AutoLogin();
                }

                ErrorResponse newErrorResponse = null;
                PhotosCommentsResponse newPhotosCommentsResponse = null;

                // считываем требуемое количество комментариев к фотографии
                try
                {
                    //throw new NullReferenceException();

                    newPhotosCommentsResponse = _iCommunicationLogic.LoadPhotosCommentsData(_iDataLogic.GetUid(), _iDataLogic.GetSessionKey(), "0", Convert.ToString(count - 1), uid.ToString() + "_" + photoID.ToString(), out newErrorResponse);
                }
                catch (VKException)
                {
                    throw;
                }
                catch (TimeoutException ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.LoadCommentsToPhoto TimeoutException Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.LoadCommentsToPhoto TimeoutException StackTrace: " + ex.StackTrace);

                    throw new VKException(ExceptionMessage.NoConnection);
                }
                catch (WebException ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.LoadCommentsToPhoto WebException Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.LoadCommentsToPhoto WebException StackTrace: " + ex.StackTrace);

                    throw new VKException(ExceptionMessage.NoConnection);
                }
                catch (OutOfMemoryException ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.LoadCommentsToPhoto OutOfMemoryException Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.LoadCommentsToPhoto OutOfMemoryException StackTrace: " + ex.StackTrace);

                    throw;
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.LoadCommentsToPhoto Exception Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.LoadCommentsToPhoto Exception StackTrace: " + ex.StackTrace);
                }

                if (newPhotosCommentsResponse == null)
                {
                    if (newErrorResponse != null)
                    {
                        if (newErrorResponse.error_code == "1")
                        {
                            if (!restoreSession)
                            {
                                return LoadCommentsToPhoto(uid, photoID, count, isRefresh, true, null);
                            }
                            else
                            {
                                throw new VKException(ExceptionMessage.ServerUnavalible);
                            }
                        }
                        else if (newErrorResponse.error_code == "2")
                        {
                            throw new VKException(ExceptionMessage.AccountBloked);
                        }
                        else
                        {
                            throw new VKException(ExceptionMessage.UnknownError);
                        }
                    }
                    else
                    {
                        //throw new VKException(ExceptionMessage.UnknownError);
                    }
                }

                newPhotosCommentsResponse.pcrPhotoID = photoID; // устанавливаем ID фото

                newPhotosCommentsRespounseHistory.DelItem(photoID); // удаляем данные по фото, на случай если они есть
                newPhotosCommentsRespounseHistory.AddItem(newPhotosCommentsResponse); // добавляем полученные данные                

                ////сохраняем в кэш всю историю
                //try
                //{
                //    DebugHelper.WriteLogEntry("(Кэш) Сохранение данных PhotosCommentsResponseHistory...");

                //    bool result;

                //    if (photosCommentsRespounseHistory == null)
                //    {
                //        result = Cache.Cache.SaveToCache(newPhotosCommentsRespounseHistory, string.Empty, "PhotosCommentsResponseHistory");
                //    }
                //    else
                //    {
                //        result = true;
                //    }

                //    if (result)
                //    {
                //        DebugHelper.WriteLogEntry("(Кэш) Данные PhotosCommentsResponseHistory успешно сохранены.");
                //    }
                //    else
                //    {
                //        DebugHelper.WriteLogEntry("(Кэш) Данные PhotosCommentsResponseHistory не сохранены.");
                //    }
                //}
                //catch (Exception)
                //{
                //    DebugHelper.WriteLogEntry("(Кэш) В процессе сохранения данных PhotosCommentsResponseHistory произошла ошибка.");
                //}

                if (photosCommentsRespounseHistory == null)
                {
                    //result = Cache.Cache.SaveToCache(newPhotosCommentsRespounseHistory, string.Empty, "PhotosCommentsResponseHistory");
                    DataModel.Data.PhotosCommentsResponseHistoryData = newPhotosCommentsRespounseHistory;
                }

                return newPhotosCommentsResponse;
            }
        }

        public UpdatesPhotosResponse LoadUpdatesPhotos(int count, bool isRefresh, bool restoreSession)
        {
            DebugHelper.WriteLogEntry("BaseLogic.LoadUpdatesPhotos");

            try
            {
                if (isRefresh)
                {
                    throw new IOException();
                }

                //// загружаем данные из кэша
                //try
                //{
                //    UpdatesPhotosResponse newUpdatesPhotosResponse = Cache.Cache.LoadFromCache<UpdatesPhotosResponse>(string.Empty, "UpdatesPhotosResponse");
                //    return newUpdatesPhotosResponse;
                //}
                //catch (IOException)
                //{
                //    return null;
                //}

                return DataModel.Data.UpdatesPhotosResponseData;
            }
            catch (IOException)
            {
                //WiFi
                if (_iDataLogic.GetOnlyWIFI() == "1")
                    if (!CoreHelper.TurnWiFi(true)) throw new VKException(ExceptionMessage.NoConnection);

                // восстанавливаем сессию
                if (restoreSession)
                {
                    AutoLogin();
                }

                ErrorResponse newErrorResponse = null;

                UpdatesPhotosResponse newUpdatesPhotosResponse = null;

                // запрашиваем данные с сервера
                try
                {
                    newUpdatesPhotosResponse = _iCommunicationLogic.LoadUpdatesPhotosData(_iDataLogic.GetUid(), _iDataLogic.GetSessionKey(), "0", Convert.ToString(count - 1), out newErrorResponse);
                }
                catch (VKException)
                {
                    throw;
                }
                catch (TimeoutException ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.LoadUpdatesPhotos TimeoutException Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.LoadUpdatesPhotos TimeoutException StackTrace: " + ex.StackTrace);

                    throw new VKException(ExceptionMessage.NoConnection);
                }
                catch (WebException ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.LoadUpdatesPhotos WebException Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.LoadUpdatesPhotos WebException StackTrace: " + ex.StackTrace);

                    throw new VKException(ExceptionMessage.NoConnection);
                }
                catch (OutOfMemoryException ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.LoadUpdatesPhotos OutOfMemoryException Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.LoadUpdatesPhotos OutOfMemoryException StackTrace: " + ex.StackTrace);

                    throw;
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteLogEntry("BaseLogic.LoadUpdatesPhotos Exception Message: " + ex.Message);
                    DebugHelper.WriteLogEntry("BaseLogic.LoadUpdatesPhotos Exception StackTrace: " + ex.StackTrace);
                }

                // обрабатываем данные и ищем ошибки
                if (newUpdatesPhotosResponse == null)
                {
                    if (newErrorResponse != null)
                    {
                        if (newErrorResponse.error_code == "1")
                        {
                            if (!restoreSession)
                            {
                                return LoadUpdatesPhotos(count, isRefresh, true);
                            }
                            else
                            {
                                throw new VKException(ExceptionMessage.ServerUnavalible);
                            }
                        }
                        else if (newErrorResponse.error_code == "2")
                        {
                            throw new VKException(ExceptionMessage.AccountBloked);
                        }
                        else
                        {
                            throw new VKException(ExceptionMessage.UnknownError);
                        }
                    }
                    else
                    {
                        //throw new VKException(ExceptionMessage.UnknownError);
                    }
                }

                //// сохраняем данные в кэш
                //try
                //{
                //    DebugHelper.WriteLogEntry("(Кэш) Сохранение данных UpdatesPhotosResponse...");

                //    bool result;

                //    result = Cache.Cache.SaveToCache(newUpdatesPhotosResponse, string.Empty, "UpdatesPhotosResponse");

                //    if (result)
                //    {
                //        DebugHelper.WriteLogEntry("(Кэш) Данные UpdatesPhotosResponse успешно сохранены.");
                //    }
                //    else
                //    {
                //        DebugHelper.WriteLogEntry("(Кэш) Данные UpdatesPhotosResponse не сохранены.");
                //    }
                //}
                //catch (IOException)
                //{
                //    DebugHelper.WriteLogEntry("(Кэш) В процессе сохранения данных UpdatesPhotosResponse произошла ошибка.");
                //}

                if (newUpdatesPhotosResponse != null)
                {
                    DataModel.Data.UpdatesPhotosResponseData = newUpdatesPhotosResponse;
                }

                // возвращаем то, что успели набрать
                return newUpdatesPhotosResponse;
            }
        }

        public int GetLastUserPhotoID(bool restoreSession)
        {
            //WiFi
            if (_iDataLogic.GetOnlyWIFI() == "1")
                if (!CoreHelper.TurnWiFi(true)) throw new VKException(ExceptionMessage.NoConnection);

            if (restoreSession)
            {
                try
                {
                    AutoLogin();
                }
                catch
                {
                    //
                }
            }

            ErrorResponse newErrorResponse = null;
            UpdatesPhotosResponse newUpdatesPhotosResponse = null;

            // запрашиваем данные с сервера
            try
            {
                newUpdatesPhotosResponse = _iCommunicationLogic.LoadUserPhotosData(_iDataLogic.GetUid(), _iDataLogic.GetSessionKey(), "0", "1", out newErrorResponse);
            }
            catch (VKException)
            {
                //throw;
            }
            catch (TimeoutException ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.LoadUpdatesPhotos TimeoutException Message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.LoadUpdatesPhotos TimeoutException StackTrace: " + ex.StackTrace);

                //throw new VKException(ExceptionMessage.NoConnection);
            }
            catch (WebException ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.LoadUpdatesPhotos WebException Message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.LoadUpdatesPhotos WebException StackTrace: " + ex.StackTrace);

                //throw new VKException(ExceptionMessage.NoConnection);
            }
            catch (OutOfMemoryException ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.LoadUpdatesPhotos OutOfMemoryException Message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.LoadUpdatesPhotos OutOfMemoryException StackTrace: " + ex.StackTrace);

                //throw;
            }
            catch (Exception ex)
            {
                DebugHelper.WriteLogEntry("BaseLogic.LoadUpdatesPhotos Exception Message: " + ex.Message);
                DebugHelper.WriteLogEntry("BaseLogic.LoadUpdatesPhotos Exception StackTrace: " + ex.StackTrace);
            }

            if (newUpdatesPhotosResponse != null)
            {
                if (newUpdatesPhotosResponse.uprPhotoDatas.Count > 0)
                {
                    return newUpdatesPhotosResponse.uprPhotoDatas[0].pdPhotoID;
                }
            }
            else
            {
                if (newErrorResponse != null)
                {
                    if (newErrorResponse.error_code == "1")
                    {
                        if (!restoreSession)
                        {
                            return GetLastUserPhotoID(true);
                        }
                        else
                        {
                            //throw new VKException(ExceptionMessage.ServerUnavalible);
                        }
                    }
                    else if (newErrorResponse.error_code == "2")
                    {
                        //throw new VKException(ExceptionMessage.AccountBloked);
                    }
                    else
                    {
                        //throw new VKException(ExceptionMessage.UnknownError);
                    }
                }
                else
                {
                    //throw new VKException(ExceptionMessage.UnknownError);
                }
            }

            return 0;
        }

        // этот метод выбирает данные из кэша
        public string GetPhotoLargeURLByPhotoID(int photoID)
        {
            try
            {
                //UpdatesPhotosResponse newUserPhotosResponse = Cache.Cache.LoadFromCache<UpdatesPhotosResponse>(string.Empty, "UserPhotosResponse");
                UpdatesPhotosResponse newUserPhotosResponse = DataModel.Data.UserPhotosResponseData;

                if (newUserPhotosResponse == null)
                {
                    return string.Empty;
                }

                foreach (var val in newUserPhotosResponse.uprPhotoDatas)
                {
                    if (val.pdPhotoID == photoID)
                    {
                        return val.pdPhotoURL604px;
                    }
                }

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        // этот метод просто перезагружает все фотки
        public void ReloadUserPhotos(bool isRefresh, bool restoreSession)
        {
            try
            {
                if (isRefresh)
                {
                    throw new Exception();
                }
            }
            catch (Exception) // если кэша нет... лезем в сеть
            {
                // WiFi
                if (_iDataLogic.GetOnlyWIFI() == "1")
                {
                    if (!CoreHelper.TurnWiFi(true))
                    {
                        throw new VKException(ExceptionMessage.NoConnection);
                    }
                }

                // восстанавливаем сессию
                if (restoreSession)
                {
                    AutoLogin();
                }

                UpdatesPhotosResponse totalUpdatesPhotosResponse = new UpdatesPhotosResponse();

                int blockSize = 50;
                int countGet = 0;

                bool doLoading = true;

                while (doLoading)
                {
                    ErrorResponse newErrorResponse = null;
                    UpdatesPhotosResponse newUpdatesPhotosResponse = null;

                    try
                    {
                        newUpdatesPhotosResponse = _iCommunicationLogic.LoadUserPhotosData(_iDataLogic.GetUid(), _iDataLogic.GetSessionKey(), countGet.ToString(), (countGet + blockSize).ToString(), out newErrorResponse);
                    }
                    catch (VKException)
                    {
                        throw;
                    }
                    catch (TimeoutException ex)
                    {
                        DebugHelper.WriteLogEntry("BaseLogic.GetPhotoLargeURLByPhotoID TimeoutException Message: " + ex.Message);
                        DebugHelper.WriteLogEntry("BaseLogic.GetPhotoLargeURLByPhotoID TimeoutException StackTrace: " + ex.StackTrace);

                        throw new VKException(ExceptionMessage.NoConnection);
                    }
                    catch (WebException ex)
                    {
                        DebugHelper.WriteLogEntry("BaseLogic.GetPhotoLargeURLByPhotoID WebException Message: " + ex.Message);
                        DebugHelper.WriteLogEntry("BaseLogic.GetPhotoLargeURLByPhotoID WebException StackTrace: " + ex.StackTrace);

                        throw new VKException(ExceptionMessage.NoConnection);
                    }
                    catch (OutOfMemoryException ex)
                    {
                        DebugHelper.WriteLogEntry("BaseLogic.GetPhotoLargeURLByPhotoID OutOfMemoryException Message: " + ex.Message);
                        DebugHelper.WriteLogEntry("BaseLogic.GetPhotoLargeURLByPhotoID OutOfMemoryException StackTrace: " + ex.StackTrace);

                        throw;
                    }
                    catch (Exception ex)
                    {
                        DebugHelper.WriteLogEntry("BaseLogic.GetPhotoLargeURLByPhotoID Exception Message: " + ex.Message);
                        DebugHelper.WriteLogEntry("BaseLogic.GetPhotoLargeURLByPhotoID Exception StackTrace: " + ex.StackTrace);
                    }

                    if (newUpdatesPhotosResponse != null)
                    {
                        foreach (var val in newUpdatesPhotosResponse.uprPhotoDatas)
                        {
                            totalUpdatesPhotosResponse.uprPhotoDatas.Add(val);
                        }

                        if (newUpdatesPhotosResponse.uprPhotoDatas.Count < blockSize) // т.е. в очередном запросе пришло меньше чем просили...
                        {
                            doLoading = false;
                        }

                        countGet += newUpdatesPhotosResponse.uprPhotoDatas.Count;
                    }
                    else
                    {
                        if (newErrorResponse != null)
                        {
                            if (newErrorResponse.error_code == "1")
                            {
                                if (!restoreSession)
                                {
                                    ReloadUserPhotos(isRefresh, true);

                                    return;
                                }
                                else
                                {
                                    throw new VKException(ExceptionMessage.ServerUnavalible);
                                }
                            }
                            else if (newErrorResponse.error_code == "2")
                            {
                                throw new VKException(ExceptionMessage.AccountBloked);
                            }
                            else
                            {
                                throw new VKException(ExceptionMessage.UnknownError);
                            }
                        }
                        else
                        {
                            //throw new VKException(ExceptionMessage.UnknownError);
                        }
                    }
                }

                //// сохраняем в кэш
                //try
                //{
                //    DebugHelper.WriteLogEntry("(Кэш) Сохранение данных UserPhotosResponse...");

                //    bool result;

                //    result = Cache.Cache.SaveToCache(totalUpdatesPhotosResponse, string.Empty, "UserPhotosResponse");

                //    if (result)
                //    {
                //        DebugHelper.WriteLogEntry("(Кэш) Данные UserPhotosResponse успешно сохранены.");
                //    }
                //    else
                //    {
                //        DebugHelper.WriteLogEntry("(Кэш) Данные UserPhotosResponse не сохранены.");
                //    }
                //}
                //catch (Exception)
                //{
                //    DebugHelper.WriteLogEntry("(Кэш) В процессе сохранения данных UserPhotosResponse произошла ошибка.");
                //}

                if (totalUpdatesPhotosResponse != null)
                {
                    DataModel.Data.UserPhotosResponseData = totalUpdatesPhotosResponse;
                }
            }
        }

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// Событие для сигнализирования отправки запроса к серверу
        /// </summary>
        event LogRequestEventHandler LogRequestEvent;

        /// <summary>
        /// Событие для сигналищирования получения ответа от сервера
        /// </summary>
        event LogResponseEventHandler LogResponseEvent;

        /// <summary>
        /// Event для сигнализирования форме о завершении загрузки изображения
        /// </summary>
        event AfterLoadImageEventHandler AfterLoadImageEvent;

        #endregion

        #region Event handlers

#if DEBUG

        private void LogResponse(object sender, LogResponseEventArgs e)
        {
            string response = null;

            if (e.Stream != null)
            {
                byte[] buffer = new byte[e.Stream.Length];

                e.Stream.Read(buffer, 0, (int)e.Stream.Length);

                response = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

                e.Stream.Flush();
                e.Stream.Close();
            }

            DebugHelper.WriteLogEntry(String.Format("Log Response: {0}; Stream: {1}", e.Info, response));
        }

        private void LogRequest(object sender, LogRequestEventArgs e)
        {
            string request = null;

            if (e.Stream != null)
            {
                byte[] buffer = new byte[e.Stream.Length];

                e.Stream.Read(buffer, 0, (int)e.Stream.Length);

                request = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            }

            DebugHelper.WriteLogEntry(String.Format("Log Request: {0}; Stream: {1}", e.Info, request));
        }

#endif

        #endregion

        private System.Windows.Forms.Timer timerKeepAwake;

        private static void TimerKeepAwakeTick(object sender, EventArgs eventArgs)
        {
            CoreHelper.KeepDeviceAwake();
            DebugHelper.WriteLogEntry("TimerKeepAwakeTick");
        }
    }
}
