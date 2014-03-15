using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using Galssoft.VKontakteWM.Components.Common.Localization;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.Configuration;
using Galssoft.VKontakteWM.Components.ResponseClasses;
using Galssoft.VKontakteWM.Components.Common.ResponseClasses;

using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Galssoft.VKontakteWM.Components.Server
{
    public class CommunicationLogic : ICommunicationLogic
    {
        #region Public methods <- все здесь

        #region 1 (автор., инф., загруз. изобр.)

        #region авторизация

        /// <summary>
        /// Авторизация пользователя по логину и паролю
        /// </summary>
        /// <param name="login">Логин пользователя </param>
        /// <param name="pass">Пароль </param>
        /// <param name="isRemember">Флаг, указывающий на необходимость получения токена </param>
        /// <param name="errorResponse"></param>
        /// <returns>Объект с результатом аутентификации </returns>
        public AuthLoginResponse AuthLogin(string login, string pass, bool isRemember)
        {
            var authLoginResponse = new AuthLoginResponse();

            // формирование запроса
            var uri = new Uri
                          {
                              Address = SystemConfiguration.ServerConnectionToLogin,
                              Method = "/auth?login=force&"
                          };

            uri.Parameters.Add("site=1");
            uri.Parameters.Add("email=" + login);
            uri.Parameters.Add("pass=" + pass);

            var request = HttpUtility.PrepareHttpWebRequest(uri.GetUri());

            try
            {
                using (WebResponse newHttpWebResponse = request.GetResponse())
                {
                    bool logging = (LogRequestEvent != null);

                    if (logging)
                    {
                        OnLogRequest(new LogRequestEventArgs(String.Format("AuthLogin requested: Address: {0} Header: {1}", request.Address, request.Headers), null));
                    }

                    // прием ответа
                    WebHeaderCollection whc = newHttpWebResponse.Headers;

                    // парсинг ответа
                    if (whc != null)
                    {
                        try
                        {
                            string location = whc["Location"];

                            if (location.IndexOf("sid=-1") != -1)
                            {
                                throw new VKException(ExceptionMessage.IncorrectLoginOrPassword);
                            }
                            else if (location.IndexOf("sid=-2") != -1)
                            {
                                throw new VKException(ExceptionMessage.IncorrectLoginOrPassword);
                            }
                            else if (location.IndexOf("sid=-3") != -1)
                            {
                                throw new VKException(ExceptionMessage.IncorrectLoginOrPassword);
                            }
                            else if (location.IndexOf("sid=-4") != -1)
                            {
                                throw new VKException(ExceptionMessage.IncorrectLoginOrPassword);
                            }

                            authLoginResponse.session_key = location.Substring(location.IndexOf("sid=") + "sid=".Length);
                        }
                        catch (ArgumentException)
                        {
                            authLoginResponse.session_key = string.Empty;
                        }

                        try
                        {
                            string cookie = whc["Set-Cookie"];

                            int startId = cookie.IndexOf("remixmid=") + "remixmid=".Length;
                            int idLength = 0;

                            cookie = cookie.Remove(0, startId);

                            for (int i = 0; i < cookie.Length; i++)
                            {
                                if (cookie[i] != ';')
                                {
                                    idLength++;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            authLoginResponse.uid = cookie.Substring(0, idLength);
                        }
                        catch (ArgumentException)
                        {
                            authLoginResponse.uid = string.Empty;
                        }

                        if (isRemember)
                        {
                            try
                            {
                                string cookie = whc["Set-Cookie"];                                

                                int startId = cookie.IndexOf("remixpassword=") + "remixpassword=".Length;
                                int rmxpasLength = 0;

                                cookie = cookie.Remove(0, startId);

                                for (int i = 0; i < cookie.Length; i++)
                                {
                                    if (cookie[i] != ';')
                                    {
                                        rmxpasLength++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                authLoginResponse.auth_token = cookie.Substring(0, rmxpasLength);

                            }
                            catch (ArgumentException)
                            {
                                authLoginResponse.auth_token = string.Empty;
                            }
                        }

                        return authLoginResponse;
                    }
                }
            }
            catch (ObjectDisposedException ex)
            {
                DebugHelper.WriteLogEntry(ex, "AuthLogin ObjectDisposedException");

                return null;
            }
            finally
            {
                request.Abort();
            }

            return null;
        }

        /// <summary>
        /// Авторизация пользователя по токену
        /// </summary>
        /// <param name="token">Токен</param>
        /// <param name="errorResponse"></param>
        /// <returns>Объект с результатом аутентификации </returns>
        public AuthLoginResponse AuthLoginByToken(string token, string uid)
        {
            var authLoginResponse = new AuthLoginResponse();

            authLoginResponse.uid = uid;
            authLoginResponse.auth_token = token;

            // формирование запроса
            Uri uri = new Uri();
            uri.Address = SystemConfiguration.ServerConnectionToLogin;
            uri.Method = "/auth?login=auto&site=1";

            var request = HttpUtility.PrepareHttpWebRequest(uri.GetUri());

            try
            {
                request.Headers["Cookie"] = "remixpassword=" + token;

                bool logging = (LogRequestEvent != null);

                if (logging)
                {
                    OnLogRequest(new LogRequestEventArgs(String.Format("AuthLoginByToken requested: Address: {0} Header: {1}", request.Address, request.Headers), null));
                }

                // прием ответа

                using (WebResponse newHttpWebResponse = request.GetResponse())
                {
                    WebHeaderCollection whc = newHttpWebResponse.Headers;

                    if (whc != null)
                    {
                        try
                        {
                            string loc = whc["Location"];

                            string tmpStr = loc.Substring(loc.IndexOf("sid=") + "sid=".Length);

                            if (tmpStr == "-1")
                            {
                                throw new VKException(ExceptionMessage.IncorrectLoginOrPassword);
                            }
                            else if (tmpStr == "-2")
                            {
                                throw new VKException(ExceptionMessage.IncorrectLoginOrPassword);
                            }
                            else if (tmpStr == "-3")
                            {
                                throw new VKException(ExceptionMessage.IncorrectLoginOrPassword);
                            }
                            else if (tmpStr == "-4")
                            {
                                throw new VKException(ExceptionMessage.IncorrectLoginOrPassword);
                            }
                            else
                            {
                                authLoginResponse.session_key = tmpStr;
                            }
                        }
                        catch (ArgumentException)
                        {
                            authLoginResponse.session_key = string.Empty;
                        }

                        return authLoginResponse;
                    }
                }
            }
            catch (ObjectDisposedException ex)
            {
                DebugHelper.WriteLogEntry(ex, "AuthLoginByToken ObjectDisposedException");

                return null;
            }
            finally
            {
                request.Abort();
            }

            return null;
        }

        /// <summary>
        /// Уничтожение текущей сессии
        /// </summary>
        /// <param name="sid">Идентификатор сессии</param>
        /// <param name="errorResponse">Ошибки</param>
        /// <returns>удачное или неудачное завершение операции</returns>
        public bool ExpireSession(string sid, out ErrorResponse errorResponse)
        {
            errorResponse = new ErrorResponse();

            var uri = new Uri();
            uri.Address = SystemConfiguration.ServerConnectionToLogin;
            uri.Method = "/auth?login=logout&site=2";
            uri.Parameters.Add("sid=" + sid);

            try
            {
                HttpWebRequest request = HttpUtility.PrepareHttpWebRequest(uri.GetUri());
                request.AllowAutoRedirect = false;
                request.Timeout = SystemConfiguration.ServerConnectionTimeOutShort;

                WebHeaderCollection whc = request.GetResponse().Headers;
                request.Abort();

                string response = whc["Set-Cookie"];

                if (response.IndexOf("remixpassword=deleted") == -1)
                {
                    errorResponse.error_code = "2";
                    return false;
                }
                return true;
            }
            catch (ObjectDisposedException ex)
            {
                DebugHelper.WriteLogEntry(ex, "ExpireSession ObjectDisposedException");
                return false;
            }
        }

        #endregion

        #region Info

        /// <summary>
        /// Информация о пользователе
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="sessionKey"></param>
        /// <param name="errorResponse"></param>
        /// <returns></returns>
        public User UserGetInfo(string uid, string sessionKey, out ErrorResponse errorResponse)
        {
            errorResponse = null;
            User userGetInfoResponse = null;

            //Формирование запроса
            var uri = new Uri
                          {
                              Address = SystemConfiguration.ServerConnectionToApiCalls,
                              Method = "/data?act=profile&"
                          };

            uri.Parameters.Add("id=" + uid);
            uri.Parameters.Add("sid=" + sessionKey);

            //HttpWebRequest newHttpWebRequest = HttpUtility.PrepareHttpWebRequest(newUri.GetUri());

            //WebResponse newHttpWebResponse = newHttpWebRequest.GetResponse();

            //Stream newStream;

            //bool logging = (LogRequestEvent != null);

            //if (logging)
            //{
            //    newStream = new MemoryStream(2048);

            //    HttpUtility.CopyStream(HttpUtility.PrepareResponseStream(newHttpWebResponse), newStream);

            //    newStream.Flush();
            //    newStream.Position = 0;

            //    OnLogRequest(new LogRequestEventArgs(string.Format("LoadFriendsListData requested: Address: {0} Header: {1}", newHttpWebRequest.Address, newHttpWebRequest.Headers), null));
            //}
            //else
            //{
            //    newStream = HttpUtility.PrepareResponseStream(newHttpWebResponse);
            //}

            //newHttpWebRequest.Abort();

            var request = HttpUtility.PrepareHttpWebRequest(uri.GetUri());
            //WebResponse newHttpWebResponse = null;
            Stream newStream = null;
            try
            {
                using (WebResponse newHttpWebResponse = request.GetResponse())
                {
                    bool logging = (LogRequestEvent != null);

                    if (logging)
                    {
                        newStream = new MemoryStream(2048);

                        HttpUtility.CopyStream(HttpUtility.PrepareResponseStream(newHttpWebResponse), newStream);
                        newStream.Flush();
                        newStream.Position = 0;

                        OnLogRequest(new LogRequestEventArgs(String.Format("UserGetInfo requested: Address: {0} Header: {1}", request.Address, request.Headers), null));
                    }
                    else
                    {
                        newStream = HttpUtility.PrepareResponseStream(newHttpWebResponse);
                    }

                    userGetInfoResponse = ParsingHelper.ProfileJSONParse(newStream, out errorResponse);

                    if (userGetInfoResponse != null)
                    {
                        userGetInfoResponse.Uid = uid;
                    }

                    return userGetInfoResponse;
                }                
            }
            catch (ObjectDisposedException ex)
            {
                DebugHelper.WriteLogEntry(ex, "UserGetInfo ObjectDisposedException");

                return null;
            }
            finally
            {
                request.Abort();

                if (newStream != null)
                {
                    newStream.Close();
                }

                //if (newHttpWebResponse != null)
                //    newHttpWebResponse.Close();
            }
        }

        /// <summary>
        /// Список событий
        /// </summary>
        /// <param name="Uid"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public RawEventsGetResponse GetEvents(string Uid, string sessionId)
        {
            RawEventsGetResponse getEventsResponse = new RawEventsGetResponse();

            ErrorResponse errorResponse = null;

            Uri uri = new Uri
            {
                Address = SystemConfiguration.ServerConnectionToApiCalls,
                Method = "/data?act=history&"
            };

            uri.Parameters.Add("sid=" + sessionId);

            HttpWebRequest newHttpWebRequest = HttpUtility.PrepareHttpWebRequest(uri.GetUri());
            //WebResponse newHttpWebResponse = null;
            Stream newStream = null;

            try
            {
                using (WebResponse newHttpWebResponse = newHttpWebRequest.GetResponse())
                {
                    bool logging = (LogRequestEvent != null);

                    if (logging)
                    {
                        newStream = new MemoryStream(2048);

                        HttpUtility.CopyStream(HttpUtility.PrepareResponseStream(newHttpWebResponse), newStream);

                        newStream.Flush();
                        newStream.Position = 0;

                        OnLogRequest(new LogRequestEventArgs(string.Format("GetEvents requested: Address: {0} Header: {1}", newHttpWebRequest.Address, newHttpWebRequest.Headers), null));
                    }
                    else
                    {
                        newStream = HttpUtility.PrepareResponseStream(newHttpWebResponse);
                    }

                    getEventsResponse = ParsingHelper.EventsMFCParse(newStream, out errorResponse);

                    if (logging)
                    {
                        newStream.Position = 0;
                        OnLogResponse(new LogResponseEventArgs("GetEvents responsed: ", newStream));
                        newStream.Flush();
                    }

                    return getEventsResponse;
                }                
            }
            catch (ObjectDisposedException ex)
            {
                DebugHelper.WriteLogEntry(ex, "GetEvents ObjectDisposedException");
                return null;
            }
            finally
            {
                newHttpWebRequest.Abort();

                if (newStream != null)
                {
                    newStream.Close();
                }
            }
        }

        /// <summary>
        /// Редактирование текущего статуса
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="sid">Идентификатор сессии</param>
        /// <param name="newStatus">Тест нового статуса</param>
        /// <param name="action">Действие со статусом (ST_REFRESH - обновить, ST_CLEAR - очистить, ST_DELETE - удалить)</param>
        /// <returns>Статус операции (прошла: 1 - успешно, 0 - неуспешно)</returns>
        public bool SetStatus(string uid, string sid, string newStatus, StatusActionType actionType, out ErrorResponse errorResponse)
        {
            errorResponse = null;

            // подготовка строки запроса для редактирования            
            Uri uri = new Uri();
            uri.Address = SystemConfiguration.ServerConnectionToApiCalls;

            switch (actionType)
            {
                case StatusActionType.Refresh:
                    uri.Method = "/data?act=set_activity&";
                    uri.Parameters.Add("text=" + newStatus);
                    break;

                case StatusActionType.Clear:
                    uri.Method = "/data?act=clear_activity&";
                    break;

                case StatusActionType.Delete:
                    uri.Method = "/data?act=del_activity&";
                    break;

                default:
                    return false;
            }

            uri.Parameters.Add("id=" + uid);
            uri.Parameters.Add("sid=" + sid);

            var request = HttpUtility.PrepareHttpWebRequest(uri.GetUri());            
            Stream webResponseStream = null;

            try
            {
                bool logging = (LogRequestEvent != null);

                if (logging)
                {
                    OnLogRequest(new LogRequestEventArgs(String.Format("SetStatus requested: Address: {0} Header: {1}", request.Address, request.Headers), null));
                }

                // получение и разбор ответа
                using (WebResponse webResponse = request.GetResponse())
                {
                    webResponseStream = HttpUtility.PrepareResponseStream(webResponse);

                    var response = new StreamReader(webResponseStream).ReadToEnd();

                    if (response.IndexOf("\"ok\":0") != -1)
                    {
                        errorResponse = new ErrorResponse();

                        errorResponse.error_code = "3";

                        return false;
                    }
                    if (response.IndexOf("\"ok\":-1") != -1)
                    {
                        errorResponse = new ErrorResponse();

                        errorResponse.error_code = "1";

                        return false;
                    }
                    if (response.IndexOf("\"ok\":-2") != -1)
                    {
                        errorResponse = new ErrorResponse();

                        errorResponse.error_code = "2";

                        return false;
                    }

                    if (logging)
                    {
                        OnLogResponse(new LogResponseEventArgs(String.Format("SetStatus responsed: {0}", response), null));
                    }        
   
                    return true;
                }                
            }
            catch (ObjectDisposedException ex)
            {
                DebugHelper.WriteLogEntry(ex, "SetStatus ObjectDisposedException");

                return false;
            }
            finally
            {
                request.Abort();

                if (webResponseStream != null)
                {
                    webResponseStream.Close();
                }
            }
        }

        #endregion

        #region загрузка изображений

        /// <summary>
        /// Загрузка фотографии в альбом
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="sid">Идентификатор сессии</param>
        /// <param name="hash">Хэш для загрузки фотографии</param>
        /// <param name="rhash">Обратный хэш для загрузки фотографий</param>
        /// <param name="aid">Идентификатор альбома</param>
        /// <param name="UplAddress">Адрес для загрузки фото</param>
        /// <param name="file">Файл с фотографией</param>
        /// <param name="errorResponse">Ошибки</param>
        /// <returns>Удачное или неудачное завершение операции</returns>
        public bool UploadPhoto(string uid, string sid, string hash, string rhash, string aid, string UplAddress, byte[] file, out ErrorResponse errorResponse)
        {
            errorResponse = null;

            var uri = new Uri()
                          {
                              Address = UplAddress,
                              Method = "/upload.php?act=lphotos&"
                          };

            uri.Parameters.Add("id=" + uid);
            uri.Parameters.Add("oid=" + uid);
            uri.Parameters.Add("sid=" + sid);

            HttpWebRequest newHttpWebRequest = HttpUtility.PrepareHttpWebRequest(uri.GetUri(), "POST");
            HttpWebRequest newHttpWebRequest2 = null;

            try
            {
                string boundary = "----------" + DateTime.Now.Ticks.ToString("x");

                newHttpWebRequest.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);

                string header = string.Format("--{0}", boundary);
                string footer = "\r\n" + header + "--";

                var contents = new StringBuilder();

                contents.Append(header + "\r\n");
                contents.Append("Content-Disposition: form-data; name=\"back\"\r\n\r\n");
                contents.Append(@"http://durov.ru/" + "\r\n");
                contents.Append(header + "\r\n");
                contents.Append("Content-Disposition: form-data; name=\"hash\"\r\n\r\n");
                contents.Append(hash + "\r\n");
                contents.Append(header + "\r\n");
                contents.Append("Content-Disposition: form-data; name=\"aid\"\r\n\r\n");
                contents.Append(aid + "\r\n");
                contents.Append(header + "\r\n");
                contents.Append("Content-Disposition: form-data; name=\"rhash\"\r\n\r\n");
                contents.Append(rhash + "\r\n");
                contents.Append(header + "\r\n");
                contents.Append(string.Format("Content-Disposition: form-data; name=\"file1\"; filename=\"{0}\"\r\n", DateTime.Now.Ticks.ToString("x") + ".jpeg"));
                contents.Append("Content-Type: image/jpeg" + "\r\n");

                byte[] head = Encoding.UTF8.GetBytes(contents + "\r\n");

                using (var Str = new MemoryStream())
                {
                    Str.Write(head, 0, head.Length);

                    var buffer = new byte[2048];

                    using (var img = new MemoryStream(file))
                    {
                        img.Position = 0;

                        for (int i = 0; i < img.Length; )
                        {
                            int k = img.Read(buffer, 0, buffer.Length);

                            if (k > 0)
                            {
                                Str.Write(buffer, 0, k);
                            }

                            i += k;
                        }

                        img.Close();
                    }

                    byte[] newb = Encoding.UTF8.GetBytes(footer);

                    Str.Write(newb, 0, newb.Length);

                    newHttpWebRequest.ContentLength = Str.Length;

                    bool logging = (LogRequestEvent != null);

                    if (logging)
                    {
                        OnLogRequest(new LogRequestEventArgs(String.Format("UploadPhotos requested: Address: {0} Header: {1}", newHttpWebRequest.Address, newHttpWebRequest.Headers), null));
                    }

                    Str.Position = 0;

                    using (Stream dataStream = newHttpWebRequest.GetRequestStream())
                    {
                        for (int i = 0; i < Str.Length; )
                        {
                            int k = Str.Read(buffer, 0, buffer.Length);

                            if (k > 0)
                            {
                                dataStream.Write(buffer, 0, k);
                            }

                            i += k;
                        }

                        dataStream.Flush();
                        dataStream.Close();
                    }

                    Str.Flush();
                    Str.Close();
                }

                string str;

                using (var response = (HttpWebResponse)newHttpWebRequest.GetResponse())
                {
                    WebHeaderCollection whc = response.Headers;

                    newHttpWebRequest.Abort();

                    str = whc["Location"];
                }
                
                //Do not use HttpWebRequest twice! It can be reason of strange behavior!
                newHttpWebRequest2 = HttpUtility.PrepareHttpWebRequest(str);

                using (var response = (HttpWebResponse)newHttpWebRequest2.GetResponse())
                {
                    //
                }

                return true;
            }
            catch (ObjectDisposedException ex)
            {
                DebugHelper.WriteLogEntry(ex, "UploadPhoto ObjectDisposedException");

                return false;
            }
            finally
            {
                newHttpWebRequest.Abort();

                if (newHttpWebRequest2 != null)
                {
                    newHttpWebRequest2.Abort();
                }
            }
        }

        /// <summary>
        /// Смена аватара
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="sid">Идентификатор сессии</param>
        /// <param name="address">Адрес для загрузки аватара</param>
        /// <param name="hash">Хэш для загрузки аватара</param>
        /// <param name="rhash">Обратный хэш для загрузки аватара</param>
        /// <param name="file">Файл с аватаром</param>
        /// <param name="errorResponse">Ошибки</param>
        /// <returns>Удачное или неудачное завершение операции</returns>
        public bool ChangeAvatar(string uid, string sid, string address, string hash, string rhash, byte[] file, out ErrorResponse errorResponse)
        {
            errorResponse = null;

            var uri = new Uri()
            {
                Address = address,
                Method = "/upload.php?act=lprofile"
            };
            try
            {
                HttpWebRequest newHttpWebRequest = HttpUtility.PrepareHttpWebRequest(uri.GetUri(), "POST");

                string boundary = "----------" + DateTime.Now.Ticks.ToString("x");

                newHttpWebRequest.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);

                string header = string.Format("--{0}", boundary);
                string footer = "\r\n" + header + "--";

                var contents = new StringBuilder();

                contents.Append(header + "\r\n");
                contents.Append("Content-Disposition: form-data; name=\"id\"\r\n\r\n");
                contents.Append(uid + "\r\n");
                contents.Append(header + "\r\n");
                contents.Append("Content-Disposition: form-data; name=\"sid\"\r\n\r\n");
                contents.Append(sid + "\r\n");
                contents.Append(header + "\r\n");
                contents.Append("Content-Disposition: form-data; name=\"back\"\r\n\r\n");
                contents.Append(@"http://durov.ru/" + "\r\n");
                contents.Append(header + "\r\n");
                contents.Append("Content-Disposition: form-data; name=\"hash\"\r\n\r\n");
                contents.Append(hash + "\r\n");
                contents.Append(header + "\r\n");
                contents.Append("Content-Disposition: form-data; name=\"rhash\"\r\n\r\n");
                contents.Append(rhash + "\r\n");
                contents.Append(header + "\r\n");
                contents.Append(string.Format("Content-Disposition: form-data; name=\"photo\"; filename=\"{0}\"\r\n", DateTime.Now.Ticks.ToString("x") + ".jpeg"));
                contents.Append("Content-Type: image/jpeg" + "\r\n");

                byte[] head = Encoding.UTF8.GetBytes(contents + "\r\n");

                using (var Str = new MemoryStream())
                {
                    Str.Write(head, 0, head.Length);

                    var buffer = new byte[2048];

                    using (var img = new MemoryStream(file))
                    {
                        img.Position = 0;

                        for (int i = 0; i < img.Length; )
                        {
                            int k = img.Read(buffer, 0, buffer.Length);

                            if (k > 0)
                            {
                                Str.Write(buffer, 0, k);
                            }

                            i += k;
                        }

                        img.Close();
                    }

                    byte[] newb = Encoding.UTF8.GetBytes(footer);

                    Str.Write(newb, 0, newb.Length);

                    newHttpWebRequest.ContentLength = Str.Length;

                    bool logging = (LogRequestEvent != null);

                    if (logging)
                    {
                        OnLogRequest(new LogRequestEventArgs(String.Format("UploadPhotos requested: Address: {0} Header: {1}", newHttpWebRequest.Address, newHttpWebRequest.Headers), null));
                    }

                    Str.Position = 0;

                    using (Stream dataStream = newHttpWebRequest.GetRequestStream())
                    {
                        for (int i = 0; i < Str.Length; )
                        {
                            int k = Str.Read(buffer, 0, buffer.Length);

                            if (k > 0)
                            {
                                dataStream.Write(buffer, 0, k);
                            }

                            i += k;
                        }

                        dataStream.Flush();
                        dataStream.Close();
                    }

                    Str.Flush();
                    Str.Close();
                }

                var response = (HttpWebResponse)newHttpWebRequest.GetResponse();

                WebHeaderCollection whc = response.Headers;

                newHttpWebRequest.Abort();

                string str = whc["Location"];

                //Do not use HttpWebRequest twice! It can be reason of strange behavior!
                var newHttpWebRequest2 = HttpUtility.PrepareHttpWebRequest(str);
                newHttpWebRequest2.GetResponse();

                newHttpWebRequest2.Abort();

                return true;
            }
            catch (ObjectDisposedException ex)
            {
                DebugHelper.WriteLogEntry(ex, "ChangeAvatar ObjectDisposedException");
                return false;
            }
        }

        #endregion

        #endregion

        #region 2 (сообщ.)

        #region сообщения

        /// <summary>
        /// Загружает сообщения 
        /// </summary>
        /// <param name="mtype">Тип запроса: inbox (все входящие), oubox (все исходящие), message (переписка с пользователем)</param>
        /// <param name="uid">ID пользователя</param>
        /// <param name="sid">ID сессии</param>
        /// <param name="from">Номер начального элемента</param>
        /// <param name="to">Номер конечного элемента</param>
        /// <param name="userid">ID пользователя, переписка с которым запрашивается</param>
        /// <returns>Сообщения в классе MessResponse</returns>
        public MessResponse LoadMessages(string mtype, string uid, string sid, string from, string to, int userid, out ErrorResponse newErrorResponse)
        {
            // формируем запрос
            Uri newUri = new Uri
            {
                Address = SystemConfiguration.ServerConnectionToApiCalls,
                Method = "/data?act=" + mtype + "&"
            };

            newUri.Parameters.Add("sid=" + sid);
            newUri.Parameters.Add("from=" + from);
            newUri.Parameters.Add("to=" + to);

            if (mtype == "message")
            {
                newUri.Parameters.Add("id=" + Convert.ToString(userid));
            }

            HttpWebRequest newHttpWebRequest = HttpUtility.PrepareHttpWebRequest(newUri.GetUri());
            //WebResponse newHttpWebResponse = null;
            Stream newStream = null;
            try
            {
                using (WebResponse newHttpWebResponse = newHttpWebRequest.GetResponse())
                {
                    bool logging = (LogRequestEvent != null);

                    if (logging)
                    {
                        newStream = new MemoryStream(2048);

                        HttpUtility.CopyStream(HttpUtility.PrepareResponseStream(newHttpWebResponse), newStream);

                        newStream.Flush();
                        newStream.Position = 0;

                        OnLogRequest(new LogRequestEventArgs(string.Format("LoadMessages requested: Address: {0} Header: {1}", newHttpWebRequest.Address, newHttpWebRequest.Headers), null));
                    }
                    else
                    {
                        newStream = HttpUtility.PrepareResponseStream(newHttpWebResponse);
                    }

                    MessResponse newMessResponse = ParsingHelper.ParseMessages(newStream, Convert.ToInt32(uid), out newErrorResponse);

                    if (logging)
                    {
                        newStream.Position = 0;
                        OnLogResponse(new LogResponseEventArgs("LoadMessages", newStream));
                        newStream.Flush();
                    }

                    return newMessResponse;
                }                
            }
            catch (ObjectDisposedException ex)
            {
                DebugHelper.WriteLogEntry(ex, "LoadMessages ObjectDisposedException");

                newErrorResponse = null;

                return null;
            }
            finally
            {
                newHttpWebRequest.Abort();

                if (newStream != null)
                {
                    newStream.Close();
                }
            }
        }

        /// <summary>
        /// Загружает изменения в личных сообщениях 
        /// </summary>
        /// <param name="mtype">Тип запроса: inbox (все входящие), oubox (все исходящие), message (переписка с пользователем)</param>
        /// <param name="uid">ID пользователя</param>
        /// <param name="sid">ID сессии</param>
        /// <param name="ts">Номер версии</param>
        /// <param name="userid">ID пользователя, переписка с которым запрашивается</param>
        /// <returns>Изменения в сообщениях в классе MessChangesResponse</returns>
        public MessChangesResponse LoadMessChanges(string mtype, string uid, string sid, int userid, int ts, out ErrorResponse newErrorResponse)
        {
            //формируем запрос
            Uri newUri = new Uri
            {
                Address = SystemConfiguration.ServerConnectionToApiCalls,
                Method = "/data?act=" + mtype + "&"
            };

            newUri.Parameters.Add("sid=" + sid);
            newUri.Parameters.Add("ts=" + Convert.ToString(ts));

            if (mtype == "message")
            {
                newUri.Parameters.Add("id=" + Convert.ToString(userid));
            }

            HttpWebRequest newHttpWebRequest = HttpUtility.PrepareHttpWebRequest(newUri.GetUri());
            //WebResponse newHttpWebResponse = null;
            Stream newStream = null;
            try
            {
                using (WebResponse newHttpWebResponse = newHttpWebRequest.GetResponse())
                {
                    bool logging = (LogRequestEvent != null);

                    if (logging)
                    {
                        newStream = new MemoryStream(2048);

                        HttpUtility.CopyStream(HttpUtility.PrepareResponseStream(newHttpWebResponse), newStream);

                        newStream.Flush();
                        newStream.Position = 0;

                        OnLogRequest(new LogRequestEventArgs(string.Format("LoadMessChanges requested: Address: {0} Header: {1}", newHttpWebRequest.Address, newHttpWebRequest.Headers), null));
                    }
                    else
                    {
                        newStream = HttpUtility.PrepareResponseStream(newHttpWebResponse);
                    }

                    MessChangesResponse MessChanges = ParsingHelper.ParseMessageChanges(newStream, Convert.ToInt32(uid), out newErrorResponse);

                    if (logging)
                    {
                        newStream.Position = 0;
                        OnLogResponse(new LogResponseEventArgs("LoadMessChanges", newStream));
                        newStream.Flush();
                    }

                    return MessChanges;
                }
            }
            catch (ObjectDisposedException ex)
            {
                DebugHelper.WriteLogEntry(ex, "LoadMessChanges ObjectDisposedException");
                newErrorResponse = null;
                return null;
            }
            finally
            {
                newHttpWebRequest.Abort();
                if (newStream != null)
                    newStream.Close();
                //if (newHttpWebResponse != null)
                //    newHttpWebResponse.Close();
            }
        }

        /// <summary>
        /// Отправляет сообщение
        /// </summary>
        /// <param name="uid">ID отправителя</param>
        /// <param name="sid">ID сессии</param>
        /// <param name="userid">ID получателя</param>
        /// <param name="messText">Текст сообщения</param>
        /// <returns>Успешность выполнения операции</returns>
        public bool SendMessage(string uid, string sid, int userid, string messText, out ErrorResponse newErrorResponse)
        {
            //newErrorResponse = null;
            // формируем запрос
            Uri newUri = new Uri
            {
                Address = SystemConfiguration.ServerConnectionToApiCalls,
                Method = "/data?act=add_message&"
            };

            newUri.Parameters.Add("id=" + Convert.ToString(userid));
            newUri.Parameters.Add("sid=" + sid);
            newUri.Parameters.Add("message=" + messText);

            HttpWebRequest newHttpWebRequest = HttpUtility.PrepareHttpWebRequest(newUri.GetUri());
            //WebResponse newHttpWebResponse = null;
            Stream newStream = null;
            bool result = false;
            newErrorResponse = null;
            try
            {
                using (WebResponse newHttpWebResponse = newHttpWebRequest.GetResponse())
                {
                    bool logging = (LogRequestEvent != null);

                    if (logging)
                    {
                        newStream = new MemoryStream(2048);

                        HttpUtility.CopyStream(HttpUtility.PrepareResponseStream(newHttpWebResponse), newStream);

                        newStream.Flush();
                        newStream.Position = 0;

                        OnLogRequest(
                            new LogRequestEventArgs(
                                string.Format("SendMessage requested: Address: {0} Header: {1}", newHttpWebRequest.Address,
                                              newHttpWebRequest.Headers), null));
                    }
                    else
                    {
                        newStream = HttpUtility.PrepareResponseStream(newHttpWebResponse);
                    }

                    result = ParsingHelper.SendMessageResponse(newStream, out newErrorResponse);

                    if (logging)
                    {
                        newStream.Position = 0;
                        OnLogResponse(new LogResponseEventArgs("SendMessage", newStream));
                        newStream.Flush();
                    }
                }
            }
            catch (ObjectDisposedException ex)
            {
                DebugHelper.WriteLogEntry(ex, "SendMessage ObjectDisposedException");
            }
            finally
            {
                newHttpWebRequest.Abort();
                if (newStream != null)
                    newStream.Close();
                //if (newHttpWebResponse != null)
                //    newHttpWebResponse.Close();
            }
            return result;
        }

        #endregion

        #endregion

        #region 3 (обновл., друзья, новости, комментарии, загр. изображ.)

        #region загрузка кр. инф. об обновлениях

        public ShortActivityResponse LoadShortActivityResponseData(string uid, string sid, string from, string to, out ErrorResponse newErrorResponse)
        {
            ShortActivityResponse newShortActivityData = new ShortActivityResponse();

            newErrorResponse = null;

            //формируем запрос
            Uri newUri = new Uri
            {
                Address = SystemConfiguration.ServerConnectionToApiCalls,
                Method = "/data?act=updates_activity&"
            };

            newUri.Parameters.Add("sid=" + sid);
            newUri.Parameters.Add("from=" + from);
            newUri.Parameters.Add("to=" + to);

            HttpWebRequest newHttpWebRequest = HttpUtility.PrepareHttpWebRequest(newUri.GetUri());
            Stream newStream = null;

            try
            {
                newHttpWebRequest.AllowAutoRedirect = false;
                newHttpWebRequest.Timeout = SystemConfiguration.ServerConnectionTimeOutShort;

                using (WebResponse newHttpWebResponse = newHttpWebRequest.GetResponse())
                {
                    bool logging = (LogRequestEvent != null);

                    if (logging)
                    {
                        newStream = new MemoryStream(2048);

                        HttpUtility.CopyStream(HttpUtility.PrepareResponseStream(newHttpWebResponse), newStream);

                        newStream.Flush();
                        newStream.Position = 0;

                        OnLogRequest(new LogRequestEventArgs(string.Format("LoadShortActivityResponseData requested: Address: {0} Header: {1}", newHttpWebRequest.Address, newHttpWebRequest.Headers), null));
                    }
                    else
                    {
                        newStream = HttpUtility.PrepareResponseStream(newHttpWebResponse);
                    }

                    using (JsonTextReader newJsonTextReader = new JsonTextReader(new StreamReader(newStream)))
                    {
                        try
                        {
                            #region парсинг

                            while (newJsonTextReader.Read())
                            {
                                //<...> обновления
                                if (newJsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(newJsonTextReader.Value, "d"))
                                {
                                    newJsonTextReader.Read();

                                    //открыли массив 1-ого порядка (список обновлений)
                                    if (newJsonTextReader.TokenType == JsonToken.StartArray)
                                    {
                                        while (newJsonTextReader.Read()) //открываем очередь чтения списка обновлений
                                        {
                                            //читаем содержимое списка обновлений

                                            //открыли массив 2-ого порядка (отдельное обновление)
                                            if (newJsonTextReader.TokenType == JsonToken.StartArray)
                                            {
                                                while (newJsonTextReader.Read()) //открываем очередь чтения обновления
                                                {
                                                    //читаем содержимое обновления
                                                    if (newJsonTextReader.TokenType == JsonToken.String)
                                                    {
                                                        string value = (string)newJsonTextReader.Value;

                                                        int position = value.IndexOf('_');

                                                        string strID = value.Remove(0, position + 1);

                                                        newShortActivityData.sadStatusID.Add(Convert.ToInt32(strID));

                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                //ошибки
                                if (newJsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(newJsonTextReader.Value, "ok"))
                                {
                                    newErrorResponse = new ErrorResponse();

                                    newJsonTextReader.Read();

                                    int error_code = Convert.ToInt32(newJsonTextReader.Value);

                                    switch (error_code)
                                    {
                                        case -1:
                                            //истекшая сессия
                                            newErrorResponse.error_code = "1";
                                            break;

                                        case -2:
                                            //флуд-контроль
                                            newErrorResponse.error_code = "2";
                                            break;
                                    }

                                    return null;
                                }
                            }

                            return newShortActivityData;

                            #endregion
                        }
                        finally
                        {
                            if (logging)
                            {
                                newStream.Position = 0;
                                OnLogResponse(new LogResponseEventArgs("LoadShortActivityResponseData", newStream));
                                newStream.Flush();
                            }

                        }
                    }
                }
            }
            catch (ObjectDisposedException ex)
            {
                DebugHelper.WriteLogEntry(ex, "LoadShortActivityResponseData ObjectDisposedException");
                newErrorResponse = null;
                return null;
            }
            finally
            {
                newHttpWebRequest.Abort();
                if (newStream != null)
                    newStream.Close();
                //if (newHttpWebResponse != null)
                //    newHttpWebResponse.Close();
            }
        }

        public ShortWallResponse LoadShortWallResponseData(string uid, string sid, string from, string to, out ErrorResponse newErrorResponse)
        {
            ShortWallResponse newShortWallResponse = new ShortWallResponse();

            newErrorResponse = null;

            //формируем запрос
            Uri newUri = new Uri
            {
                Address = SystemConfiguration.ServerConnectionToApiCalls,
                Method = "/data?act=wall&"
            };

            newUri.Parameters.Add("id=" + uid);
            newUri.Parameters.Add("sid=" + sid);
            newUri.Parameters.Add("from=" + from);
            newUri.Parameters.Add("to=" + to);

            HttpWebRequest newHttpWebRequest = HttpUtility.PrepareHttpWebRequest(newUri.GetUri());
            //WebResponse newHttpWebResponse = null;
            Stream newStream = null;
            try
            {
                newHttpWebRequest.AllowAutoRedirect = false;
                newHttpWebRequest.Timeout = SystemConfiguration.ServerConnectionTimeOutShort;

                using (WebResponse newHttpWebResponse = newHttpWebRequest.GetResponse())
                {
                    bool logging = (LogRequestEvent != null);

                    if (logging)
                    {
                        newStream = new MemoryStream(2048);

                        HttpUtility.CopyStream(HttpUtility.PrepareResponseStream(newHttpWebResponse), newStream);

                        newStream.Flush();
                        newStream.Position = 0;

                        OnLogRequest(new LogRequestEventArgs(string.Format("LoadShortWallResponseData requested: Address: {0} Header: {1}", newHttpWebRequest.Address, newHttpWebRequest.Headers), null));
                    }
                    else
                    {
                        newStream = HttpUtility.PrepareResponseStream(newHttpWebResponse);
                    }

                    using (JsonTextReader newJsonTextReader = new JsonTextReader(new StreamReader(newStream)))
                    {
                        try
                        {
                            #region парсинг

                            while (newJsonTextReader.Read())
                            {
                                //<...> сообщения
                                if (newJsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(newJsonTextReader.Value, "d"))
                                {
                                    newJsonTextReader.Read();

                                    //открыли массив 1-ого порядка (список сообщений)
                                    if (newJsonTextReader.TokenType == JsonToken.StartArray)
                                    {
                                        while (newJsonTextReader.Read()) //открываем очередь чтения списка сообщений
                                        {
                                            //читаем содержимое списка сообщений

                                            //открыли массив 2-ого порядка (отдельное сообщение)
                                            if ((newJsonTextReader.TokenType == JsonToken.StartArray))
                                            {
                                                int depth = 0; //это чтобы корректно выходить из циклов
                                                bool valueIsRead = false; //это чтобы не читать 2 раза

                                                while (newJsonTextReader.Read()) //открываем очередь чтения сообщения
                                                {
                                                    //читаем содержимое сообщения
                                                    if (newJsonTextReader.TokenType == JsonToken.Integer)
                                                    {
                                                        if (!valueIsRead)
                                                        {
                                                            newShortWallResponse.swrMessageID.Add(Convert.ToInt32(newJsonTextReader.Value));

                                                            valueIsRead = true;
                                                        }
                                                    }
                                                    else if (newJsonTextReader.TokenType == JsonToken.StartArray)
                                                    {
                                                        depth++;
                                                    }
                                                    else if (newJsonTextReader.TokenType == JsonToken.EndArray)
                                                    {
                                                        if (depth > 0)
                                                        {
                                                            depth--;
                                                        }
                                                        else
                                                        {
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                //ошибки
                                if (newJsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(newJsonTextReader.Value, "ok"))
                                {
                                    newErrorResponse = new ErrorResponse();

                                    newJsonTextReader.Read();

                                    int error_code = Convert.ToInt32(newJsonTextReader.Value);

                                    switch (error_code)
                                    {
                                        case -1:
                                            //истекшая сессия
                                            newErrorResponse.error_code = "1";
                                            break;

                                        case -2:
                                            //флуд-контроль
                                            newErrorResponse.error_code = "2";
                                            break;
                                    }

                                    return null;
                                }
                            }

                            #endregion

                            return newShortWallResponse;
                        }
                        finally
                        {
                            if (logging)
                            {
                                newStream.Position = 0;
                                OnLogResponse(new LogResponseEventArgs("LoadShortWallResponseData", newStream));
                                newStream.Flush();
                            }
                        }
                    }
                }
            }
            catch (ObjectDisposedException ex)
            {
                DebugHelper.WriteLogEntry(ex, "LoadShortWallResponseData ObjectDisposedException");
                newErrorResponse = null;
                return null;
            }
            finally
            {
                newHttpWebRequest.Abort();
                if (newStream != null)
                    newStream.Close();
                //if (newHttpWebResponse != null)
                //    newHttpWebResponse.Close();
            }
        }

        public ShortUpdatesPhotosResponse LoadShortUpdatesPhotosResponse(string sid, string from, string to, out ErrorResponse newErrorResponse)
        {
            ShortUpdatesPhotosResponse newShortUpdatesPhotosResponse = new ShortUpdatesPhotosResponse();

            newErrorResponse = null;

            //формируем запрос
            Uri newUri = new Uri
            {
                Address = SystemConfiguration.ServerConnectionToApiCalls,
                Method = "/data?act=updates_photos&"
            };

            newUri.Parameters.Add("sid=" + sid);
            newUri.Parameters.Add("from=" + from);
            newUri.Parameters.Add("to=" + to);

            HttpWebRequest newHttpWebRequest = HttpUtility.PrepareHttpWebRequest(newUri.GetUri());
            //WebResponse newHttpWebResponse = null;
            Stream newStream = null;
            try
            {
                newHttpWebRequest.AllowAutoRedirect = false;
                newHttpWebRequest.Timeout = SystemConfiguration.ServerConnectionTimeOutShort;

                using (WebResponse newHttpWebResponse = newHttpWebRequest.GetResponse())
                {
                    bool logging = (LogRequestEvent != null);

                    if (logging)
                    {
                        newStream = new MemoryStream(2048);

                        HttpUtility.CopyStream(HttpUtility.PrepareResponseStream(newHttpWebResponse), newStream);

                        newStream.Flush();
                        newStream.Position = 0;

                        OnLogRequest(new LogRequestEventArgs(string.Format("LoadShortUpdatesPhotosResponse requested: Address: {0} Header: {1}", newHttpWebRequest.Address, newHttpWebRequest.Headers), null));
                    }
                    else
                    {
                        newStream = HttpUtility.PrepareResponseStream(newHttpWebResponse);
                    }

                    using (JsonTextReader newJsonTextReader = new JsonTextReader(new StreamReader(newStream)))
                    {
                        try
                        {
                            #region парсинг

                            while (newJsonTextReader.Read())
                            {
                                //<...> обновления
                                if (newJsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(newJsonTextReader.Value, "d"))
                                {
                                    newJsonTextReader.Read();

                                    //открыли массив 1-ого порядка (список обновлений)
                                    if (newJsonTextReader.TokenType == JsonToken.StartArray)
                                    {
                                        while (newJsonTextReader.Read()) //открываем очередь чтения списка обновлений
                                        {
                                            //читаем содержимое списка обновлений

                                            //открыли массив 2-ого порядка (отдельное обновление)
                                            if (newJsonTextReader.TokenType == JsonToken.StartArray)
                                            {
                                                while (newJsonTextReader.Read()) //открываем очередь чтения обновления
                                                {
                                                    //читаем содержимое обновления
                                                    if (newJsonTextReader.TokenType == JsonToken.String)
                                                    {
                                                        string value = (string)newJsonTextReader.Value;

                                                        int position = value.IndexOf('_');

                                                        string strID = value.Remove(0, position + 1);

                                                        newShortUpdatesPhotosResponse.suprPhotoID.Add(Convert.ToInt32(strID));

                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                //ошибки
                                if (newJsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(newJsonTextReader.Value, "ok"))
                                {
                                    newErrorResponse = new ErrorResponse();

                                    newJsonTextReader.Read();

                                    int error_code = Convert.ToInt32(newJsonTextReader.Value);

                                    switch (error_code)
                                    {
                                        case -1:
                                            //истекшая сессия
                                            newErrorResponse.error_code = "1";
                                            break;

                                        case -2:
                                            //флуд-контроль
                                            newErrorResponse.error_code = "2";
                                            break;
                                    }

                                    return null;
                                }
                            }

                            return newShortUpdatesPhotosResponse;

                            #endregion
                        }
                        finally
                        {
                            if (logging)
                            {
                                newStream.Position = 0;
                                OnLogResponse(new LogResponseEventArgs("LoadShortUpdatesPhotosResponse", newStream));
                                newStream.Flush();
                            }

                        }
                    }
                }
            }
            catch (ObjectDisposedException ex)
            {
                DebugHelper.WriteLogEntry(ex, "LoadShortUpdatesPhotosResponse ObjectDisposedException");
                newErrorResponse = null;
                return null;
            }
            finally
            {
                newHttpWebRequest.Abort();
                if (newStream != null)
                    newStream.Close();
                //if (newHttpWebResponse != null)
                //    newHttpWebResponse.Close();
            }
        }

        public ShortPhotosCommentsRespounse LoadShortPhotosCommentsRespounse(string uid, string sid, string from, string to, string parent, out ErrorResponse newErrorResponse)
        {
            ShortPhotosCommentsRespounse newShortPhotosCommentsRespounse = new ShortPhotosCommentsRespounse();

            newErrorResponse = null;

            //формируем запрос
            Uri newUri = new Uri
            {
                Address = SystemConfiguration.ServerConnectionToApiCalls,
                Method = "/data?act=photos_comments&"
            };

            newUri.Parameters.Add("id=" + uid);
            newUri.Parameters.Add("sid=" + sid);
            newUri.Parameters.Add("from=" + from);
            newUri.Parameters.Add("to=" + to);
            newUri.Parameters.Add("parent=" + parent);

            HttpWebRequest newHttpWebRequest = HttpUtility.PrepareHttpWebRequest(newUri.GetUri());
            //WebResponse newHttpWebResponse = null;
            Stream newStream = null;
            try
            {
                newHttpWebRequest.AllowAutoRedirect = false;
                newHttpWebRequest.Timeout = SystemConfiguration.ServerConnectionTimeOutShort;

                using (WebResponse newHttpWebResponse = newHttpWebRequest.GetResponse())
                {
                    bool logging = (LogRequestEvent != null);

                    if (logging)
                    {
                        newStream = new MemoryStream(2048);

                        HttpUtility.CopyStream(HttpUtility.PrepareResponseStream(newHttpWebResponse), newStream);

                        newStream.Flush();
                        newStream.Position = 0;

                        OnLogRequest(new LogRequestEventArgs(string.Format("LoadShortPhotosCommentsRespounse requested: Address: {0} Header: {1}", newHttpWebRequest.Address, newHttpWebRequest.Headers), null));
                    }
                    else
                    {
                        newStream = HttpUtility.PrepareResponseStream(newHttpWebResponse);
                    }

                    using (JsonTextReader newJsonTextReader = new JsonTextReader(new StreamReader(newStream)))
                    {
                        try
                        {
                            #region парсинг

                            while (newJsonTextReader.Read())
                            {
                                //<...> комментарии
                                if (newJsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(newJsonTextReader.Value, "d"))
                                {
                                    newJsonTextReader.Read();

                                    //открыли массив 1-ого порядка (список комментариев)
                                    if (newJsonTextReader.TokenType == JsonToken.StartArray)
                                    {
                                        while (newJsonTextReader.Read()) //открываем очередь чтения списка комментариев
                                        {
                                            //читаем содержимое списка комментариев

                                            //открыли массив 2-ого порядка (отдельный комментарий)
                                            if ((newJsonTextReader.TokenType == JsonToken.StartArray))
                                            {
                                                int depth = 0; //это чтобы корректно выходить из циклов
                                                bool valueIsRead = false; //это чтобы не читать 2 раза

                                                while (newJsonTextReader.Read()) //открываем очередь чтения комментария
                                                {
                                                    //читаем содержимое комменария
                                                    if (newJsonTextReader.TokenType == JsonToken.Integer)
                                                    {
                                                        if (!valueIsRead)
                                                        {
                                                            newShortPhotosCommentsRespounse.spcrCommentIDs.Add(Convert.ToInt32(newJsonTextReader.Value));

                                                            valueIsRead = true;
                                                        }
                                                    }
                                                    else if (newJsonTextReader.TokenType == JsonToken.StartArray)
                                                    {
                                                        depth++;
                                                    }
                                                    else if (newJsonTextReader.TokenType == JsonToken.EndArray)
                                                    {
                                                        if (depth > 0)
                                                        {
                                                            depth--;
                                                        }
                                                        else
                                                        {
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                //ошибки
                                if (newJsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(newJsonTextReader.Value, "ok"))
                                {
                                    newErrorResponse = new ErrorResponse();

                                    newJsonTextReader.Read();

                                    int error_code = Convert.ToInt32(newJsonTextReader.Value);

                                    switch (error_code)
                                    {
                                        case -1:
                                            //истекшая сессия
                                            newErrorResponse.error_code = "1";
                                            break;

                                        case -2:
                                            //флуд-контроль
                                            newErrorResponse.error_code = "2";
                                            break;
                                    }

                                    return null;
                                }
                            }

                            #endregion

                            return newShortPhotosCommentsRespounse;
                        }
                        finally
                        {
                            if (logging)
                            {
                                newStream.Position = 0;
                                OnLogResponse(new LogResponseEventArgs("LoadShortPhotosCommentsRespounse", newStream));
                                newStream.Flush();
                            }
                        }
                    }
                }
            }
            catch (ObjectDisposedException ex)
            {
                DebugHelper.WriteLogEntry(ex, "LoadShortPhotosCommentsRespounse ObjectDisposedException");
                newErrorResponse = null;
                return null;
            }
            finally
            {
                newHttpWebRequest.Abort();
                if (newStream != null)
                    newStream.Close();
                //if (newHttpWebResponse != null)
                //    newHttpWebResponse.Close();
            }
        }

        #endregion

        #region загрузка списков друзей, новостей, комментариев, новых фотографий

        public FriendsListResponse LoadFriendsListData(string uid, string sid, out ErrorResponse newErrorResponse)
        {
            FriendsListResponse newFriendsListResponse = new FriendsListResponse();

            newErrorResponse = null;

            // формируем запрос
            Uri newUri = new Uri
            {
                Address = SystemConfiguration.ServerConnectionToApiCalls,
                Method = "/data?act=friends&"
            };

            newUri.Parameters.Add("id=" + uid);
            newUri.Parameters.Add("sid=" + sid);

            HttpWebRequest newHttpWebRequest = HttpUtility.PrepareHttpWebRequest(newUri.GetUri());
            //WebResponse newHttpWebResponse = null;
            Stream newStream = null;
            try
            {
                using (WebResponse newHttpWebResponse = newHttpWebRequest.GetResponse())
                {
                    bool logging = (LogRequestEvent != null);

                    if (logging)
                    {
                        newStream = new MemoryStream(2048);

                        HttpUtility.CopyStream(HttpUtility.PrepareResponseStream(newHttpWebResponse), newStream);

                        newStream.Flush();
                        newStream.Position = 0;

                        OnLogRequest(new LogRequestEventArgs(string.Format("LoadFriendsListData requested: Address: {0} Header: {1}", newHttpWebRequest.Address, newHttpWebRequest.Headers), null));
                    }
                    else
                    {
                        newStream = HttpUtility.PrepareResponseStream(newHttpWebResponse);
                    }

                    using (JsonTextReader newJsonTextReader = new JsonTextReader(new StreamReader(newStream)))
                    {
                        try
                        {
                            #region парсинг

                            while (newJsonTextReader.Read())
                            {
                                //открыли массив 1-ого порядка (список друзей)
                                if (newJsonTextReader.TokenType == JsonToken.StartArray)
                                {
                                    while (newJsonTextReader.Read()) //открываем очередь чтения списка друзей
                                    {
                                        //читаем содержимое списка друзей

                                        //открыли массив 1-ого порядка (профиль друг)
                                        if (newJsonTextReader.TokenType == JsonToken.StartArray)
                                        {
                                            User newUser = new User();
                                            newFriendsListResponse.Users.Add(newUser);

                                            int index = 0;

                                            while (newJsonTextReader.Read()) //открываем очередь чтения профиля друга
                                            {
                                                //читаем содержимое профиля друга
                                                if (newJsonTextReader.TokenType == JsonToken.Integer || newJsonTextReader.TokenType == JsonToken.String)
                                                {
                                                    switch (index)
                                                    {
                                                        case 0:
                                                            newUser.Uid = newJsonTextReader.Value.ToString();
                                                            break;

                                                        case 1:
                                                            newUser.FullName = newJsonTextReader.Value.ToString();

                                                            string[] fullName = newUser.FullName.Split(' ');

                                                            newUser.FirstName = fullName[0];
                                                            newUser.LastName = fullName[1];
                                                            break;

                                                        case 2:
                                                            newUser.Photo100px = (string)newJsonTextReader.Value;
                                                            break;

                                                        case 3:
                                                            newUser.IsOnline = newJsonTextReader.Value.ToString();
                                                            break;
                                                    }
                                                }
                                                //если до конца массива, то прерываем очередь чтения профиля друга
                                                else if (newJsonTextReader.TokenType == JsonToken.EndArray)
                                                {
                                                    break;
                                                }

                                                index++;
                                            }
                                        }
                                        //если до конца массива, то прерываем очередь чтения (список друзей)
                                        else if (newJsonTextReader.TokenType == JsonToken.EndArray)
                                        {
                                            break;
                                        }
                                    }
                                }

                                //ошибки
                                if (newJsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(newJsonTextReader.Value, "ok"))
                                {
                                    newErrorResponse = new ErrorResponse();

                                    newJsonTextReader.Read();

                                    int error_code = Convert.ToInt32(newJsonTextReader.Value);

                                    switch (error_code)
                                    {
                                        case -1:
                                            //истекшая сессия
                                            newErrorResponse.error_code = "1";
                                            break;

                                        case -2:
                                            //флуд-контроль
                                            newErrorResponse.error_code = "2";
                                            break;
                                    }

                                    return null;
                                }
                            }

                            #endregion

                            newFriendsListResponse.LastUpdate = DateTime.Now;
                            return newFriendsListResponse;
                        }
                        finally
                        {
                            if (logging)
                            {
                                newStream.Position = 0;
                                OnLogResponse(new LogResponseEventArgs("LoadFriendsListData", newStream));
                                newStream.Flush();
                            }
                        }
                    }
                }
            }
            catch (ObjectDisposedException ex)
            {
                DebugHelper.WriteLogEntry(ex, "LoadFriendsListData ObjectDisposedException");
                newErrorResponse = null;
                return null;
            }
            finally
            {
                newHttpWebRequest.Abort();
                if (newStream != null)
                    newStream.Close();
                //if (newHttpWebResponse != null)
                //    newHttpWebResponse.Close();
            }
        }

        public FriendsListResponse LoadFriendsOnlineListData(string uid, string sid, out ErrorResponse newErrorResponse)
        {
            var newFriendsListResponse = new FriendsListResponse();

            newErrorResponse = null;

            //формируем запрос
            var newUri = new Uri
            {
                Address = SystemConfiguration.ServerConnectionToApiCalls,
                Method = "/data?act=friends_online&"
            };

            newUri.Parameters.Add("id=" + uid);
            newUri.Parameters.Add("sid=" + sid);

            HttpWebRequest newHttpWebRequest = HttpUtility.PrepareHttpWebRequest(newUri.GetUri());
            //WebResponse newHttpWebResponse = null;
            Stream newStream = null;
            try
            {
                using (WebResponse newHttpWebResponse = newHttpWebRequest.GetResponse())
                {
                    bool logging = (LogRequestEvent != null);

                    if (logging)
                    {
                        newStream = new MemoryStream(2048);

                        HttpUtility.CopyStream(HttpUtility.PrepareResponseStream(newHttpWebResponse), newStream);

                        newStream.Flush();
                        newStream.Position = 0;

                        OnLogRequest(new LogRequestEventArgs(string.Format("LoadFriendsOnlineListData requested: Address: {0} Header: {1}", newHttpWebRequest.Address, newHttpWebRequest.Headers), null));
                    }
                    else
                    {
                        newStream = HttpUtility.PrepareResponseStream(newHttpWebResponse);
                    }

                    using (var newJsonTextReader = new JsonTextReader(new StreamReader(newStream)))
                    {
                        try
                        {
                            #region парсинг

                            while (newJsonTextReader.Read())
                            {
                                //открыли массив 1-ого порядка (список друзей)
                                if (newJsonTextReader.TokenType == JsonToken.StartArray)
                                {
                                    while (newJsonTextReader.Read()) //открываем очередь чтения списка друзей
                                    {
                                        //читаем содержимое списка друзей

                                        //открыли массив 1-ого порядка (профиль друг)
                                        if (newJsonTextReader.TokenType == JsonToken.StartArray)
                                        {
                                            User newUser = new User();
                                            newFriendsListResponse.Users.Add(newUser);

                                            int index = 0;

                                            while (newJsonTextReader.Read()) //открываем очередь чтения профиля друга
                                            {
                                                //читаем содержимое профиля друга
                                                if (newJsonTextReader.TokenType == JsonToken.Integer || newJsonTextReader.TokenType == JsonToken.String)
                                                {
                                                    switch (index)
                                                    {
                                                        case 0:
                                                            newUser.Uid = newJsonTextReader.Value.ToString();
                                                            break;

                                                        case 1:
                                                            //fullName = newJsonTextReader.Value.ToString().Split(' ');
                                                            //newUser.FirstName = fullName[0];
                                                            //newUser.LastName = fullName[1]; 

                                                            newUser.FullName = newJsonTextReader.Value.ToString();
                                                            break;

                                                        case 2:
                                                            newUser.Photo100px = (string)newJsonTextReader.Value;
                                                            break;

                                                        case 3:
                                                            newUser.IsOnline = newJsonTextReader.Value.ToString();
                                                            break;
                                                    }
                                                }
                                                //если до конца массива, то прерываем очередь чтения профиля друга
                                                else if (newJsonTextReader.TokenType == JsonToken.EndArray)
                                                {
                                                    break;
                                                }

                                                index++;
                                            }
                                        }
                                        //если до конца массива, то прерываем очередь чтения (список друзей)
                                        else if (newJsonTextReader.TokenType == JsonToken.EndArray)
                                        {
                                            break;
                                        }
                                    }
                                }

                                //ошибки
                                if (newJsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(newJsonTextReader.Value, "ok"))
                                {
                                    newErrorResponse = new ErrorResponse();

                                    newJsonTextReader.Read();

                                    int error_code = Convert.ToInt32(newJsonTextReader.Value);

                                    switch (error_code)
                                    {
                                        case -1:
                                            //истекшая сессия
                                            newErrorResponse.error_code = "1";
                                            break;

                                        case -2:
                                            //флуд-контроль
                                            newErrorResponse.error_code = "2";
                                            break;
                                    }

                                    return null;
                                }
                            }

                            #endregion

                            return newFriendsListResponse;
                        }
                        finally
                        {
                            if (logging)
                            {
                                newStream.Position = 0;
                                OnLogResponse(new LogResponseEventArgs("LoadFriendsListData", newStream));
                                newStream.Flush();
                            }
                        }
                    }
                }
            }
            catch (ObjectDisposedException ex)
            {
                DebugHelper.WriteLogEntry(ex, "LoadFriendsOnlineListData ObjectDisposedException");
                newErrorResponse = null;
                return null;
            }
            finally
            {
                newHttpWebRequest.Abort();
                if (newStream != null)
                    newStream.Close();
                //if (newHttpWebResponse != null)
                //    newHttpWebResponse.Close();
            }
        }

        public ActivityResponse LoadActivityDataListData(string uid, string sid, string from, string to, out ErrorResponse errorResponse)
        {
            ActivityResponse activityResponse = new ActivityResponse();

            errorResponse = null;

            //формируем запрос
            Uri newUri = new Uri
            {
                Address = SystemConfiguration.ServerConnectionToApiCalls,
                Method = "/data?act=updates_activity&"
            };

            newUri.Parameters.Add("sid=" + sid);
            newUri.Parameters.Add("from=" + from);
            newUri.Parameters.Add("to=" + to);

            HttpWebRequest newHttpWebRequest = HttpUtility.PrepareHttpWebRequest(newUri.GetUri());
            //WebResponse newHttpWebResponse = null;
            Stream newStream = null;
            try
            {
                using (WebResponse newHttpWebResponse = newHttpWebRequest.GetResponse())
                {
                    bool logging = (LogRequestEvent != null);

                    if (logging)
                    {
                        newStream = new MemoryStream(2048);

                        HttpUtility.CopyStream(HttpUtility.PrepareResponseStream(newHttpWebResponse), newStream);

                        newStream.Flush();
                        newStream.Position = 0;

                        OnLogRequest(new LogRequestEventArgs(string.Format("LoadActivityDataListData requested: Address: {0} Header: {1}", newHttpWebRequest.Address, newHttpWebRequest.Headers), null));
                    }
                    else
                    {
                        newStream = HttpUtility.PrepareResponseStream(newHttpWebResponse);
                    }

                    using (JsonTextReader newJsonTextReader = new JsonTextReader(new StreamReader(newStream)))
                    {
                        try
                        {
                            #region парсинг

                            while (newJsonTextReader.Read())
                            {
                                //<...> обновления
                                if (newJsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(newJsonTextReader.Value, "d"))
                                {
                                    newJsonTextReader.Read();

                                    //открыли массив 1-ого порядка (список обновлений)
                                    if (newJsonTextReader.TokenType == JsonToken.StartArray)
                                    {
                                        while (newJsonTextReader.Read()) //открываем очередь чтения списка обновлений
                                        {
                                            //читаем содержимое списка обновлений

                                            //открыли массив 2-ого порядка (отдельное обновление)
                                            if (newJsonTextReader.TokenType == JsonToken.StartArray)
                                            {
                                                ActivityData activityData = new ActivityData();
                                                activityResponse.arActivityDatas.Add(activityData);

                                                PostSender activityDataSender = new PostSender();
                                                activityData.adDataSender = activityDataSender;

                                                int index = 0;

                                                while (newJsonTextReader.Read()) //открываем очередь чтения обновления
                                                {
                                                    //читаем содержимое обновления
                                                    if (newJsonTextReader.TokenType == JsonToken.Integer || newJsonTextReader.TokenType == JsonToken.String)
                                                    {
                                                        switch (index)
                                                        {
                                                            case 0:
                                                                string fullID = (string)newJsonTextReader.Value;

                                                                int iindex = fullID.IndexOf("_");

                                                                fullID = fullID.Substring(iindex + 1, fullID.Length - iindex - 1);

                                                                try
                                                                {
                                                                    activityData.adStatusID = Convert.ToInt32(fullID);
                                                                }
                                                                catch
                                                                {
                                                                    activityData.adStatusID = 0;
                                                                }

                                                                break;

                                                            case 1:
                                                                activityData.adDataSender.psUserID = Convert.ToInt32(newJsonTextReader.Value);
                                                                break;

                                                            case 2:
                                                                break;

                                                            case 3:
                                                                activityData.adDataSender.psUserName = (string)newJsonTextReader.Value;
                                                                break;

                                                            case 4:
                                                                int timestamp = Convert.ToInt32(newJsonTextReader.Value);

                                                                activityData.adTime = activityData.adTime.AddSeconds(timestamp);

                                                                activityData.adTime = activityData.adTime.ToLocalTime();
                                                                break;

                                                            case 5:
                                                                string rawText = (string)newJsonTextReader.Value;

                                                                rawText = Regex.Replace(rawText, "&quot;", @"""", RegexOptions.IgnoreCase);
                                                                rawText = Regex.Replace(rawText, "&#39;", "'", RegexOptions.IgnoreCase);
                                                                rawText = Regex.Replace(rawText, "&lt;", "<", RegexOptions.IgnoreCase);
                                                                rawText = Regex.Replace(rawText, "&gt;", ">", RegexOptions.IgnoreCase);
                                                                rawText = Regex.Replace(rawText, "&amp;", "&", RegexOptions.IgnoreCase);

                                                                activityData.adText = rawText;
                                                                break;
                                                        }
                                                    }
                                                    //если до конца массива, то прерываем очередь чтения обновления
                                                    else if (newJsonTextReader.TokenType == JsonToken.EndArray)
                                                    {
                                                        break;
                                                    }

                                                    index++;
                                                }
                                            }
                                            //если до конца массива, то прерываем очередь чтения (список обновлений)
                                            else if (newJsonTextReader.TokenType == JsonToken.EndArray)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }

                                //ошибки
                                if (newJsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(newJsonTextReader.Value, "ok"))
                                {
                                    errorResponse = new ErrorResponse();

                                    newJsonTextReader.Read();

                                    int error_code = Convert.ToInt32(newJsonTextReader.Value);

                                    switch (error_code)
                                    {
                                        case -1:
                                            //истекшая сессия
                                            errorResponse.error_code = "1";
                                            break;

                                        case -2:
                                            //флуд-контроль
                                            errorResponse.error_code = "2";
                                            break;
                                    }

                                    return null;
                                }
                            }

                            #endregion

                            return activityResponse;
                        }
                        finally
                        {
                            if (logging)
                            {
                                newStream.Position = 0;
                                OnLogResponse(new LogResponseEventArgs("LoadActivityDataListData", newStream));
                                newStream.Flush();
                            }
                        }
                    }
                }
            }
            catch (ObjectDisposedException ex)
            {
                DebugHelper.WriteLogEntry(ex, "LoadActivityDataListData ObjectDisposedException");
                errorResponse = null;
                return null;
            }
            finally
            {
                newHttpWebRequest.Abort();
                if (newStream != null)
                    newStream.Close();
                //if (newHttpWebResponse != null)
                //    newHttpWebResponse.Close();
            }
        }

        public PhotosCommentsResponse LoadPhotosCommentsData(string uid, string sid, string from, string to, string parent, out ErrorResponse newErrorResponse)
        {
            PhotosCommentsResponse newPhotosCommentsRespounse = new PhotosCommentsResponse();

            newErrorResponse = null;

            // формируем запрос
            Uri newUri = new Uri
            {
                Address = SystemConfiguration.ServerConnectionToApiCalls,
                Method = "/data?act=photos_comments&"
            };

            newUri.Parameters.Add("id=" + uid);
            newUri.Parameters.Add("sid=" + sid);
            newUri.Parameters.Add("from=" + from);
            newUri.Parameters.Add("to=" + to);
            newUri.Parameters.Add("parent=" + parent);

            HttpWebRequest newHttpWebRequest = HttpUtility.PrepareHttpWebRequest(newUri.GetUri());
            //WebResponse newHttpWebResponse = null;
            Stream newStream = null;
            try
            {
                using (WebResponse newHttpWebResponse = newHttpWebRequest.GetResponse())
                {
                    bool logging = (LogRequestEvent != null);

                    if (logging)
                    {
                        newStream = new MemoryStream(2048);

                        HttpUtility.CopyStream(HttpUtility.PrepareResponseStream(newHttpWebResponse), newStream);

                        newStream.Flush();
                        newStream.Position = 0;

                        OnLogRequest(new LogRequestEventArgs(string.Format("LoadPhotosCommentsData requested: Address: {0} Header: {1}", newHttpWebRequest.Address, newHttpWebRequest.Headers), null));
                    }
                    else
                    {
                        newStream = HttpUtility.PrepareResponseStream(newHttpWebResponse);
                    }

                    using (JsonTextReader newJsonTextReader = new JsonTextReader(new StreamReader(newStream)))
                    {
                        try
                        {
                            #region парсинг

                            while (newJsonTextReader.Read())
                            {
                                // кол-во комментариев
                                if (newJsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(newJsonTextReader.Value, "n"))
                                {
                                    newJsonTextReader.Read();

                                    newPhotosCommentsRespounse.pcrCount = Convert.ToInt32(newJsonTextReader.Value);
                                }

                                // timestamp
                                if (newJsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(newJsonTextReader.Value, "h"))
                                {
                                    newJsonTextReader.Read();

                                    newPhotosCommentsRespounse.pcrTimeStamp = Convert.ToInt32(newJsonTextReader.Value);
                                }

                                // автор
                                if (newJsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(newJsonTextReader.Value, "author"))
                                {
                                    newJsonTextReader.Read();

                                    if (newJsonTextReader.TokenType == JsonToken.StartArray)
                                    {
                                        User newUser = new User();
                                        newPhotosCommentsRespounse.pcrAuthor = newUser;

                                        int index = 0;

                                        while (newJsonTextReader.Read())
                                        {
                                            if (newJsonTextReader.TokenType == JsonToken.Integer || newJsonTextReader.TokenType == JsonToken.String)
                                            {
                                                switch (index)
                                                {
                                                    case 0:
                                                        newUser.Uid = Convert.ToString(newJsonTextReader.Value);
                                                        break;

                                                    case 1:
                                                        newUser.FullName = Convert.ToString(newJsonTextReader.Value);
                                                        break;
                                                }

                                                index++;
                                            }
                                        }
                                    }

                                }

                                // <...> комментарии
                                if (newJsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(newJsonTextReader.Value, "d"))
                                {
                                    newJsonTextReader.Read();

                                    // открыли массив 1-ого порядка (список комментариев)
                                    if (newJsonTextReader.TokenType == JsonToken.StartArray)
                                    {
                                        while (newJsonTextReader.Read()) // открываем очередь чтения списка комментариев
                                        {
                                            // читаем содержимое списка комментариев

                                            // открыли массив 2-ого порядка (отдельный комментарий)
                                            if (newJsonTextReader.TokenType == JsonToken.StartArray)
                                            {
                                                CommentPost newCommentPost = new CommentPost();
                                                newPhotosCommentsRespounse.pcrComments.Add(newCommentPost);

                                                WallData newWallData = new WallData();
                                                newCommentPost.cpWallData = newWallData;

                                                PostSender newPostSender = new PostSender();
                                                newCommentPost.cpPostSender = newPostSender;

                                                PostReceiver newPostReceiver = new PostReceiver();
                                                newCommentPost.cpPostReceiver = newPostReceiver;

                                                PhotoData newPhotoData = new PhotoData();
                                                newCommentPost.cpPhotoData = newPhotoData;

                                                int index = 0;

                                                while (newJsonTextReader.Read()) // открываем очередь чтения сообщения
                                                {
                                                    // читаем содержимое сообщения
                                                    if (newJsonTextReader.TokenType == JsonToken.Integer)
                                                    {
                                                        switch (index)
                                                        {
                                                            case 0:
                                                                newCommentPost.cpID = Convert.ToInt32(newJsonTextReader.Value);
                                                                break;

                                                            case 1:
                                                                newCommentPost.cpTime = newCommentPost.cpTime.AddSeconds(Convert.ToInt32(newJsonTextReader.Value));

                                                                newCommentPost.cpTime = newCommentPost.cpTime.ToLocalTime();
                                                                break;
                                                        }
                                                    }

                                                    // открыли массив 3-его порядка (содержание комментария, отправитель, получатель)
                                                    else if (newJsonTextReader.TokenType == JsonToken.StartArray)
                                                    {
                                                        int iindex = 0;

                                                        while (newJsonTextReader.Read()) // открываем очередь чтения содержания сообщения
                                                        {
                                                            // чтение содержания сообщения

                                                            if (newJsonTextReader.TokenType == JsonToken.Integer || newJsonTextReader.TokenType == JsonToken.String)
                                                            {
                                                                switch (index)
                                                                {
                                                                    case 2:
                                                                        string rawText = (string)newJsonTextReader.Value;

                                                                        if (parent.Equals("-1"))
                                                                        {
                                                                            rawText = Regex.Replace(rawText, "<br>", " ", RegexOptions.IgnoreCase);
                                                                        }
                                                                        else
                                                                        {
                                                                            rawText = Regex.Replace(rawText, "<br>", "\n", RegexOptions.IgnoreCase);
                                                                        }

                                                                        //rawText = Regex.Replace(rawText, "<[^>]*?>", string.Empty, RegexOptions.IgnoreCase);
                                                                        rawText = Regex.Replace(rawText, "&quot;", @"""", RegexOptions.IgnoreCase);
                                                                        rawText = Regex.Replace(rawText, "&#39;", "'", RegexOptions.IgnoreCase);
                                                                        rawText = Regex.Replace(rawText, "&lt;", "<", RegexOptions.IgnoreCase);
                                                                        rawText = Regex.Replace(rawText, "&gt;", ">", RegexOptions.IgnoreCase);
                                                                        rawText = Regex.Replace(rawText, "&amp;", "&", RegexOptions.IgnoreCase);

                                                                        newWallData.wdText = rawText;

                                                                        break;

                                                                    case 3:
                                                                        switch (iindex)
                                                                        {
                                                                            case 0:
                                                                                newPostSender.psUserID = Convert.ToInt32(newJsonTextReader.Value);
                                                                                break;

                                                                            case 1:
                                                                                rawText = (string)newJsonTextReader.Value;

                                                                                rawText = Regex.Replace(rawText, "\t", " ", RegexOptions.IgnoreCase);

                                                                                newPostSender.psUserName = rawText;
                                                                                break;

                                                                            case 2:
                                                                                newPostSender.psUserPhotoURL = (string)newJsonTextReader.Value;
                                                                                break;

                                                                            case 3:
                                                                                newPostSender.psSmallUserPhotoName = (string)newJsonTextReader.Value;
                                                                                break;

                                                                            case 4:
                                                                                newPostSender.psUserSex = Convert.ToInt32(newJsonTextReader.Value);
                                                                                break;

                                                                            case 5:
                                                                                newPostSender.psUserIsOnline = Convert.ToInt32(newJsonTextReader.Value);
                                                                                break;
                                                                        }

                                                                        break;

                                                                    case 4:
                                                                        switch (iindex)
                                                                        {
                                                                            case 0:
                                                                                newPostReceiver.prUserID = Convert.ToInt32(newJsonTextReader.Value);
                                                                                break;
                                                                        }

                                                                        break;

                                                                    case 7:
                                                                        switch (iindex)
                                                                        {
                                                                            case 0:
                                                                                newPhotoData.pdPhotoID = Convert.ToInt32(newJsonTextReader.Value);
                                                                                break;

                                                                            case 1:
                                                                                newPhotoData.pdPhotoURL130px = (string)newJsonTextReader.Value;
                                                                                break;
                                                                        }

                                                                        break;
                                                                }
                                                            }

                                                            // если до конца массива, то прерываем очередь чтения (списка сообщений)
                                                            else if (newJsonTextReader.TokenType == JsonToken.EndArray)
                                                            {
                                                                break;
                                                            }

                                                            iindex++;
                                                        }
                                                    }

                                                    // если до конца массива, то прерываем очередь чтения (сообщения)
                                                    else if (newJsonTextReader.TokenType == JsonToken.EndArray)
                                                    {
                                                        break;
                                                    }

                                                    index++;
                                                }
                                            }

                                            // если до конца массива, то прерываем очередь чтения (списка сообщений)
                                            else if (newJsonTextReader.TokenType == JsonToken.EndArray)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }

                                //ошибки
                                if (newJsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(newJsonTextReader.Value, "ok"))
                                {
                                    newErrorResponse = new ErrorResponse();

                                    newJsonTextReader.Read();

                                    int error_code = Convert.ToInt32(newJsonTextReader.Value);

                                    switch (error_code)
                                    {
                                        case -1:
                                            //истекшая сессия
                                            newErrorResponse.error_code = "1";
                                            break;

                                        case -2:
                                            //флуд-контроль
                                            newErrorResponse.error_code = "2";
                                            break;
                                    }

                                    return null;
                                }
                            }

                            #endregion

                            return newPhotosCommentsRespounse;
                        }
                        finally
                        {
                            if (logging)
                            {
                                newStream.Position = 0;
                                OnLogResponse(new LogResponseEventArgs("LoadPhotosCommentsData", newStream));
                                newStream.Flush();
                            }
                        }
                    }
                }
            }
            catch (ObjectDisposedException ex)
            {
                DebugHelper.WriteLogEntry(ex, "LoadPhotosCommentsData ObjectDisposedException");
                newErrorResponse = null;
                return null;
            }
            finally
            {
                newHttpWebRequest.Abort();
                if (newStream != null)
                    newStream.Close();
                //if (newHttpWebResponse != null)
                //    newHttpWebResponse.Close();
            }
        }

        // список фотографий друзей пользователя
        public UpdatesPhotosResponse LoadUpdatesPhotosData(string uid, string sid, string from, string to, out ErrorResponse newErrorResponse)
        {
            UpdatesPhotosResponse newUpdatesPhotosResponse = new UpdatesPhotosResponse();

            newErrorResponse = null;

            //формируем запрос
            Uri newUri = new Uri
            {
                Address = SystemConfiguration.ServerConnectionToApiCalls,
                Method = "/data?act=updates_photos&"
            };

            newUri.Parameters.Add("sid=" + sid);
            newUri.Parameters.Add("from=" + from);
            newUri.Parameters.Add("to=" + to);

            HttpWebRequest newHttpWebRequest = HttpUtility.PrepareHttpWebRequest(newUri.GetUri());
            //WebResponse newHttpWebResponse = null;
            Stream newStream = null;
            try
            {
                using (WebResponse newHttpWebResponse = newHttpWebRequest.GetResponse())
                {
                    bool logging = (LogRequestEvent != null);

                    if (logging)
                    {
                        newStream = new MemoryStream(2048);

                        HttpUtility.CopyStream(HttpUtility.PrepareResponseStream(newHttpWebResponse), newStream);

                        newStream.Flush();
                        newStream.Position = 0;

                        OnLogRequest(new LogRequestEventArgs(string.Format("LoadUpdatesPhotosData requested: Address: {0} Header: {1}", newHttpWebRequest.Address, newHttpWebRequest.Headers), null));
                    }
                    else
                    {
                        newStream = HttpUtility.PrepareResponseStream(newHttpWebResponse);
                    }

                    using (JsonTextReader newJsonTextReader = new JsonTextReader(new StreamReader(newStream)))
                    {
                        try
                        {
                            #region парсинг

                            while (newJsonTextReader.Read())
                            {
                                if (newJsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(newJsonTextReader.Value, "n"))
                                {
                                    newJsonTextReader.Read();

                                    newUpdatesPhotosResponse.uprPostCount = Convert.ToInt32(newJsonTextReader.Value);
                                }

                                //<...>
                                if (newJsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(newJsonTextReader.Value, "d"))
                                {
                                    newJsonTextReader.Read();

                                    //открыли массив 1-ого порядка (список)
                                    if (newJsonTextReader.TokenType == JsonToken.StartArray)
                                    {
                                        while (newJsonTextReader.Read()) //открываем очередь чтения списка
                                        {
                                            //читаем содержимое списка

                                            //открыли массив 2-ого порядка
                                            if (newJsonTextReader.TokenType == JsonToken.StartArray)
                                            {
                                                PhotoData newPhotoData = new PhotoData();
                                                newUpdatesPhotosResponse.uprPhotoDatas.Add(newPhotoData);

                                                int index = 0;

                                                while (newJsonTextReader.Read()) //открываем очередь чтения
                                                {
                                                    //читаем содержимое
                                                    if (newJsonTextReader.TokenType == JsonToken.String)
                                                    {
                                                        switch (index)
                                                        {
                                                            case 0:
                                                                string[] value = ((string)newJsonTextReader.Value).Split('_');

                                                                newPhotoData.pdUserID = Convert.ToInt32(value[0]);
                                                                newPhotoData.pdPhotoID = Convert.ToInt32(value[1]);
                                                                break;

                                                            case 1:
                                                                newPhotoData.pdPhotoURL130px = (string)newJsonTextReader.Value;
                                                                break;

                                                            case 2:
                                                                newPhotoData.pdPhotoURL604px = (string)newJsonTextReader.Value;
                                                                break;
                                                        }
                                                    }
                                                    //если до конца массива, то прерываем очередь чтения
                                                    else if (newJsonTextReader.TokenType == JsonToken.EndArray)
                                                    {
                                                        break;
                                                    }

                                                    index++;
                                                }
                                            }
                                        }
                                    }
                                }

                                // ошибки
                                if (newJsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(newJsonTextReader.Value, "ok"))
                                {
                                    newErrorResponse = new ErrorResponse();

                                    newJsonTextReader.Read();

                                    int error_code = Convert.ToInt32(newJsonTextReader.Value);

                                    switch (error_code)
                                    {
                                        case -1:
                                            // истекшая сессия
                                            newErrorResponse.error_code = "1";
                                            break;

                                        case -2:
                                            // флуд-контроль
                                            newErrorResponse.error_code = "2";
                                            break;
                                    }

                                    return null;
                                }
                            }

                            return newUpdatesPhotosResponse;

                            #endregion
                        }
                        finally
                        {
                            if (logging)
                            {
                                newStream.Position = 0;
                                OnLogResponse(new LogResponseEventArgs("LoadUpdatesPhotosData", newStream));
                                newStream.Flush();
                            }
                        }
                    }
                }
            }
            catch (ObjectDisposedException ex)
            {
                DebugHelper.WriteLogEntry(ex, "LoadUpdatesPhotosData ObjectDisposedException");
                newErrorResponse = null;
                return null;
            }
            finally
            {
                newHttpWebRequest.Abort();
                if (newStream != null)
                    newStream.Close();
                //if (newHttpWebResponse != null)
                //    newHttpWebResponse.Close();
            }
        }

        public ActivityResponse LoadUserActivity(string uid, string sid, string from, string to, out ErrorResponse errorResponse)
        {
            ActivityResponse activityResponse = new ActivityResponse();

            errorResponse = null;

            //формируем запрос
            Uri newUri = new Uri
            {
                Address = SystemConfiguration.ServerConnectionToApiCalls,
                Method = "/data?act=activity&"
            };

            newUri.Parameters.Add("id=" + uid);
            newUri.Parameters.Add("sid=" + sid);
            newUri.Parameters.Add("from=" + from);
            newUri.Parameters.Add("to=" + to);

            HttpWebRequest newHttpWebRequest = HttpUtility.PrepareHttpWebRequest(newUri.GetUri());
            //WebResponse newHttpWebResponse = null;
            Stream newStream = null;
            try
            {
                using (WebResponse newHttpWebResponse = newHttpWebRequest.GetResponse())
                {
                    bool logging = (LogRequestEvent != null);

                    if (logging)
                    {
                        newStream = new MemoryStream(2048);

                        HttpUtility.CopyStream(HttpUtility.PrepareResponseStream(newHttpWebResponse), newStream);

                        newStream.Flush();
                        newStream.Position = 0;

                        OnLogRequest(new LogRequestEventArgs(string.Format("LoadUserActivity requested: Address: {0} Header: {1}", newHttpWebRequest.Address, newHttpWebRequest.Headers), null));
                    }
                    else
                    {
                        newStream = HttpUtility.PrepareResponseStream(newHttpWebResponse);
                    }

                    using (JsonTextReader newJsonTextReader = new JsonTextReader(new StreamReader(newStream)))
                    {
                        try
                        {
                            #region парсинг

                            while (newJsonTextReader.Read())
                            {
                                //<...> обновления
                                if (newJsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(newJsonTextReader.Value, "d"))
                                {
                                    newJsonTextReader.Read();

                                    //открыли массив 1-ого порядка (список обновлений)
                                    if (newJsonTextReader.TokenType == JsonToken.StartArray)
                                    {
                                        while (newJsonTextReader.Read()) //открываем очередь чтения списка обновлений
                                        {
                                            //читаем содержимое списка обновлений

                                            //открыли массив 2-ого порядка (отдельное обновление)
                                            if (newJsonTextReader.TokenType == JsonToken.StartArray)
                                            {
                                                ActivityData activityData = new ActivityData();
                                                activityResponse.arActivityDatas.Add(activityData);

                                                PostSender activityDataSender = new PostSender();
                                                activityData.adDataSender = activityDataSender;

                                                int index = 0;

                                                while (newJsonTextReader.Read()) //открываем очередь чтения обновления
                                                {
                                                    //читаем содержимое обновления
                                                    if (newJsonTextReader.TokenType == JsonToken.Integer || newJsonTextReader.TokenType == JsonToken.String)
                                                    {
                                                        switch (index)
                                                        {
                                                            case 0:
                                                                string[] fullID = ((string)newJsonTextReader.Value).Split('_');

                                                                activityDataSender.psUserID = Convert.ToInt32(fullID[0]);
                                                                activityData.adStatusID = Convert.ToInt32(fullID[1]);
                                                                break;

                                                            case 1:
                                                                //activityData.adDataSender.psUserID = Convert.ToInt32(newJsonTextReader.Value);
                                                                break;

                                                            case 2:
                                                                break;

                                                            case 3:
                                                                activityData.adDataSender.psUserName = (string)newJsonTextReader.Value;
                                                                break;

                                                            case 4:
                                                                int timestamp = Convert.ToInt32(newJsonTextReader.Value);

                                                                activityData.adTime = activityData.adTime.AddSeconds(timestamp);

                                                                activityData.adTime = activityData.adTime.ToLocalTime();
                                                                break;

                                                            case 5:
                                                                string rawText = (string)newJsonTextReader.Value;

                                                                rawText = Regex.Replace(rawText, "&quot;", @"""", RegexOptions.IgnoreCase);
                                                                rawText = Regex.Replace(rawText, "&#39;", "'", RegexOptions.IgnoreCase);
                                                                rawText = Regex.Replace(rawText, "&lt;", "<", RegexOptions.IgnoreCase);
                                                                rawText = Regex.Replace(rawText, "&gt;", ">", RegexOptions.IgnoreCase);
                                                                rawText = Regex.Replace(rawText, "&amp;", "&", RegexOptions.IgnoreCase);

                                                                activityData.adText = rawText;
                                                                break;
                                                        }
                                                    }
                                                    //если до конца массива, то прерываем очередь чтения обновления
                                                    else if (newJsonTextReader.TokenType == JsonToken.EndArray)
                                                    {
                                                        break;
                                                    }

                                                    index++;
                                                }
                                            }
                                            //если до конца массива, то прерываем очередь чтения (список обновлений)
                                            else if (newJsonTextReader.TokenType == JsonToken.EndArray)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }

                                //ошибки
                                if (newJsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(newJsonTextReader.Value, "ok"))
                                {
                                    errorResponse = new ErrorResponse();

                                    newJsonTextReader.Read();

                                    int error_code = Convert.ToInt32(newJsonTextReader.Value);

                                    switch (error_code)
                                    {
                                        case -1:
                                            //истекшая сессия
                                            errorResponse.error_code = "1";
                                            break;

                                        case -2:
                                            //флуд-контроль
                                            errorResponse.error_code = "2";
                                            break;
                                    }

                                    return null;
                                }
                            }

                            #endregion

                            return activityResponse;
                        }
                        finally
                        {
                            if (logging)
                            {
                                newStream.Position = 0;
                                OnLogResponse(new LogResponseEventArgs("LoadUserActivity", newStream));
                                newStream.Flush();
                            }
                        }
                    }
                }
            }
            catch (ObjectDisposedException ex)
            {
                DebugHelper.WriteLogEntry(ex, "LoadUserActivity ObjectDisposedException");
                errorResponse = null;
                return null;
            }
            finally
            {
                newHttpWebRequest.Abort();
                if (newStream != null)
                    newStream.Close();
                //if (newHttpWebResponse != null)
                //    newHttpWebResponse.Close();
            }
        }

        // список фотографий пользователя
        public UpdatesPhotosResponse LoadUserPhotosData(string uid, string sid, string from, string to, out ErrorResponse newErrorResponse)
        {
            UpdatesPhotosResponse newUpdatesPhotosResponse = new UpdatesPhotosResponse();

            newErrorResponse = null;

            //формируем запрос
            Uri newUri = new Uri
            {
                Address = SystemConfiguration.ServerConnectionToApiCalls,
                Method = "/data?act=photos&"
            };

            newUri.Parameters.Add("id=" + uid);
            newUri.Parameters.Add("sid=" + sid);
            newUri.Parameters.Add("from=" + from);
            newUri.Parameters.Add("to=" + to);

            HttpWebRequest newHttpWebRequest = HttpUtility.PrepareHttpWebRequest(newUri.GetUri());
            //WebResponse newHttpWebResponse = null;
            Stream newStream = null;
            try
            {
                using (WebResponse newHttpWebResponse = newHttpWebRequest.GetResponse())
                {
                    bool logging = (LogRequestEvent != null);

                    if (logging)
                    {
                        newStream = new MemoryStream(2048);

                        HttpUtility.CopyStream(HttpUtility.PrepareResponseStream(newHttpWebResponse), newStream);

                        newStream.Flush();
                        newStream.Position = 0;

                        OnLogRequest(new LogRequestEventArgs(string.Format("LoadUserPhotosData requested: Address: {0} Header: {1}", newHttpWebRequest.Address, newHttpWebRequest.Headers), null));
                    }
                    else
                    {
                        newStream = HttpUtility.PrepareResponseStream(newHttpWebResponse);
                    }

                    using (JsonTextReader newJsonTextReader = new JsonTextReader(new StreamReader(newStream)))
                    {
                        try
                        {
                            #region парсинг

                            while (newJsonTextReader.Read())
                            {
                                // открыли массив 1-ого порядка (список)
                                if (newJsonTextReader.TokenType == JsonToken.StartArray)
                                {
                                    while (newJsonTextReader.Read()) // открываем очередь чтения списка
                                    {
                                        // читаем содержимое списка

                                        // открыли массив 2-ого порядка
                                        if (newJsonTextReader.TokenType == JsonToken.StartArray)
                                        {
                                            PhotoData newPhotoData = new PhotoData();
                                            newUpdatesPhotosResponse.uprPhotoDatas.Add(newPhotoData);

                                            int index = 0;

                                            while (newJsonTextReader.Read()) //открываем очередь чтения
                                            {
                                                //читаем содержимое
                                                if (newJsonTextReader.TokenType == JsonToken.String)
                                                {
                                                    switch (index)
                                                    {
                                                        case 0:
                                                            string[] value = ((string)newJsonTextReader.Value).Split('_');

                                                            newPhotoData.pdUserID = Convert.ToInt32(value[0]);
                                                            newPhotoData.pdPhotoID = Convert.ToInt32(value[1]);
                                                            break;

                                                        case 1:
                                                            newPhotoData.pdPhotoURL130px = (string)newJsonTextReader.Value;
                                                            break;

                                                        case 2:
                                                            newPhotoData.pdPhotoURL604px = (string)newJsonTextReader.Value;
                                                            break;
                                                    }
                                                }
                                                //если до конца массива, то прерываем очередь чтения
                                                else if (newJsonTextReader.TokenType == JsonToken.EndArray)
                                                {
                                                    break;
                                                }

                                                index++;
                                            }
                                        }
                                    }
                                }

                                // ошибки
                                if (newJsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(newJsonTextReader.Value, "ok"))
                                {
                                    newErrorResponse = new ErrorResponse();

                                    newJsonTextReader.Read();

                                    int error_code = Convert.ToInt32(newJsonTextReader.Value);

                                    switch (error_code)
                                    {
                                        case -1:
                                            // истекшая сессия
                                            newErrorResponse.error_code = "1";
                                            break;

                                        case -2:
                                            // флуд-контроль
                                            newErrorResponse.error_code = "2";
                                            break;
                                    }

                                    return null;
                                }
                            }

                            return newUpdatesPhotosResponse;

                            #endregion
                        }
                        finally
                        {
                            if (logging)
                            {
                                newStream.Position = 0;
                                OnLogResponse(new LogResponseEventArgs("LoadUserPhotosData", newStream));
                                newStream.Flush();
                            }

                        }
                    }
                }
            }
            catch (ObjectDisposedException ex)
            {
                DebugHelper.WriteLogEntry(ex, "LoadUserPhotosData ObjectDisposedException");
                newErrorResponse = null;
                return null;
            }
            finally
            {
                newHttpWebRequest.Abort();
                if (newStream != null)
                    newStream.Close();
                //if (newHttpWebResponse != null)
                //    newHttpWebResponse.Close();
            }
        }

        #endregion

        #region загр. изображ. в кэш с сервера

        public struct LoadImageInfo
        {
            public string ImageName;

            public AfterLoadImageEventHandler Event;

            public int ImageLinearSize;

            public object SortKey;

            public string SortType;
        }

        /// <summary>
        /// Загружает файл в кэш
        /// </summary>
        /// <param name="uri">адрес</param>
        /// <param name="fileName">Имя для сохранения</param>
        /// <param name="isRefresh">Указывает на необходимость обновления с сайта, если true - данные из кэша не считываются</param>
        /// <param name="afterLoadImageEvent">Делегат, обрабатывающий событие загрузки картинки </param>
        public bool LoadImage(string uri, string fileName, bool isRefresh, AfterLoadImageEventHandler afterLoadImageEvent, int imageLinearSize, object sortKey, string sortType)
        {
            try
            {
                if (uri.Equals("0"))
                {
                    return false;
                }

                //файл в кэше?
                if (File.Exists(SystemConfiguration.AppInstallPath + "//Cache//Files//" + fileName))
                {
                    return true;
                }

                throw new Exception();
            }
            catch (Exception)
            {
                HttpWebRequest request = HttpUtility.PrepareHttpWebRequest(uri);

                try
                {
                    bool logging = (LogResponseEvent != null);

                    if (logging)
                    {
                        OnLogRequest(new LogRequestEventArgs(String.Format("LoadImage requested: Address: {0} Header: {1}", request.Address, request.Headers), null));
                    }

                    LoadImageInfo newLoadImageInfo = new LoadImageInfo();

                    foreach (var val in _images)
                    {
                        if (val.Key.Address.Equals(request.Address))
                        {
                            return false;
                        }
                    }

                    newLoadImageInfo.ImageName = fileName;
                    newLoadImageInfo.Event = afterLoadImageEvent;
                    newLoadImageInfo.ImageLinearSize = imageLinearSize;
                    newLoadImageInfo.SortKey = sortKey;
                    newLoadImageInfo.SortType = sortType;

                    _images.Add(request, newLoadImageInfo);

                    return false;
                }
                catch (ObjectDisposedException ex)
                {
                    DebugHelper.WriteLogEntry(ex, "LoadImage ObjectDisposedException");
                    return false;
                }
                finally
                {
                    //request.Abort();
                }
            }
        }

        public void LoadImagesInDictionary()
        {
            string fileName = string.Empty;
            string fullFileName = string.Empty;
            AfterLoadImageEventHandler afterLoadImageEvent;

            // делаем "слепок" с текущей очереди загружаемых изображений            
            List<KeyValuePair<HttpWebRequest, LoadImageInfo>> images = new List<KeyValuePair<HttpWebRequest, LoadImageInfo>>(_images);

            // и очищаем ее
            _images = new Dictionary<HttpWebRequest, LoadImageInfo>();

            // сортируем очередь по признаку SortKey
            images.Sort(
                delegate(KeyValuePair<HttpWebRequest, LoadImageInfo> firstPair, KeyValuePair<HttpWebRequest, LoadImageInfo> nextPair)
                {
                    if (firstPair.Value.SortType == "int" && nextPair.Value.SortType == "int")
                    {
                        return -((int)firstPair.Value.SortKey).CompareTo((int)nextPair.Value.SortKey);
                    }
                    else if (firstPair.Value.SortType == "DateTime" && nextPair.Value.SortType == "DateTime")
                    {
                        return -((DateTime)firstPair.Value.SortKey).CompareTo((DateTime)nextPair.Value.SortKey);
                    }
                    else if (firstPair.Value.SortType == "string" && nextPair.Value.SortType == "string")
                    {
                        return ((string)firstPair.Value.SortKey).CompareTo((string)nextPair.Value.SortKey);
                    }

                    else if (firstPair.Value.SortType == "int" && nextPair.Value.SortType == "DateTime")
                    {
                        return 1;
                    }
                    else if (firstPair.Value.SortType == "int" && nextPair.Value.SortType == "string")
                    {
                        return -1;
                    }
                    else if (firstPair.Value.SortType == "DateTime" && nextPair.Value.SortType == "int")
                    {
                        return -1;
                    }

                    else if (firstPair.Value.SortType == "DateTime" && nextPair.Value.SortType == "string")
                    {
                        return -1;
                    }
                    else if (firstPair.Value.SortType == "DateTime" && nextPair.Value.SortType == "int")
                    {
                        return 1;
                    }
                    else if (firstPair.Value.SortType == "DateTime" && nextPair.Value.SortType == "string")
                    {
                        return 1;
                    }

                    return 0;
                }
            );

            int count = 0;

            foreach (KeyValuePair<HttpWebRequest, LoadImageInfo> kvp in images)
            {
                try
                {
                    using (WebResponse webResponse = kvp.Key.GetResponse())
                    {
                        using (Stream s = HttpUtility.PrepareResponseStream(webResponse))
                        {
                            using (Bitmap bmp = new Bitmap(s))
                            {
                                fileName = kvp.Value.ImageName;
                                fullFileName = SystemConfiguration.AppInstallPath + "//Cache//Files//" + fileName;

                                if (File.Exists(fullFileName))
                                {
                                    continue;
                                }

                                afterLoadImageEvent = kvp.Value.Event;

                                bmp.Save(SystemConfiguration.AppInstallPath + "//Cache//Files//" + fileName, System.Drawing.Imaging.ImageFormat.Jpeg);

                                bmp.Dispose();

                                if (afterLoadImageEvent != null) // уменьшить потом
                                {
                                    if (count < images.Count - 1)
                                    {
                                        afterLoadImageEvent.Invoke(this, new AfterLoadImageEventArgs(fileName, false, kvp.Value.ImageLinearSize));
                                    }
                                    else
                                    {
                                        afterLoadImageEvent.Invoke(this, new AfterLoadImageEventArgs(fileName, true, kvp.Value.ImageLinearSize));
                                    }
                                }
                                else // уменьшения потом не будет т.к. оно по afterLoadImageEvent
                                {
                                    if (kvp.Value.ImageLinearSize > 0)
                                    {
                                        if (kvp.Value.SortType.Equals("string"))
                                        {
                                            ImageHelper.SaveSquareImage(fullFileName, fullFileName, kvp.Value.ImageLinearSize);
                                        }
                                        else
                                        {                                     
                                            ImageHelper.CustomSaveScaledImage(fullFileName, fullFileName, kvp.Value.ImageLinearSize, OpenNETCF.Drawing.RotationAngle.Zero);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    kvp.Key.Abort();

                    count++;
                }
                catch
                {
                    //
                }
            }
        }

        public void ClearImagesInDictionary()
        {
            _images = new Dictionary<HttpWebRequest, LoadImageInfo>();
        }

        #endregion

        #region Добавление комментария к фотографии

        /// <summary>
        /// Функция добавления комментариев к фотографиям
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="ts">Текущее состояние photos_comments</param>
        /// <param name="message">Текст сообщения</param>
        /// <param name="parent">{id владльца фотографии}_{id фотографии}</param>
        /// <param name="sid">Идентификатор сессии</param>
        /// <param name="errorResponse"></param>
        /// <returns></returns>
        public HttpWebResponse AddPhotosComments(string uid, string message, string parent, string sid, out ErrorResponse errorResponse)
        {
            //// коррекция parent
            //string[] parentData = parent.Split('_');

            //if (parentData[0].Equals("0"))
            //{
            //    parent = uid + "_" + parentData[1];
            //}

            errorResponse = null;

            var uri = new Uri
                          {
                              Address = SystemConfiguration.ServerConnectionToApiCalls,
                              Method = "/data?act=add_photos_comments&"
                          };

            uri.Parameters.Add("id=" + uid);
            uri.Parameters.Add("message=" + message);
            uri.Parameters.Add("parent=" + parent);
            uri.Parameters.Add("sid=" + sid);

            HttpWebRequest request = HttpUtility.PrepareHttpWebRequest(uri.GetUri());
            //WebResponse response = null;            
            try
            {
                using (WebResponse newHttpWebResponse = request.GetResponse())
                {
                    string responseStr = new StreamReader(HttpUtility.PrepareResponseStream(newHttpWebResponse)).ReadToEnd();

                    if (responseStr.IndexOf("\"ok\":0") != -1)
                        errorResponse = new ErrorResponse { error_code = "0" };
                    else if (responseStr.IndexOf("\"ok\":-1") != -1)
                        errorResponse = new ErrorResponse { error_code = "-1" };
                    else if (responseStr.IndexOf("\"ok\":-2") != -1)
                        errorResponse = new ErrorResponse { error_code = "-2" };
                    else if (responseStr.IndexOf("\"ok\":-3") != -1)
                        errorResponse = new ErrorResponse { error_code = "-3" };

                    if (errorResponse != null)
                        return null;
                    return (HttpWebResponse)newHttpWebResponse;
                }
            }
            catch (ObjectDisposedException ex)
            {
                DebugHelper.WriteLogEntry(ex, "AddPhotosComments ObjectDisposedException");
                return null;
            }
            finally
            {
                request.Abort();
                //response.Close();
            }
        }

        #endregion

        #endregion

        #endregion

        #region private

        /// <summary>
        /// Хранит очередь загружаемых изображений
        /// </summary>
        private Dictionary<HttpWebRequest, LoadImageInfo> _images = new Dictionary<HttpWebRequest, LoadImageInfo>();

        /// <summary>
        /// Вызывается при по завершению загрузки изображения
        /// </summary>
        /// <param name="ar"></param>
        private void AfterLoadImage(IAsyncResult ar)
        {
            Bitmap bmp;

            string fileName;
            AfterLoadImageEventHandler afterLoadImageEvent;

            int retryCount = 0;
            bool requestDone = false;

            HttpWebRequest request = (HttpWebRequest)ar.AsyncState;

            //int workerThreads;
            //int completionPortThreads;

            //System.Threading.ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);

            //while (!requestDone)
            //{
            try
            {
                WebResponse webResponse = request.EndGetResponse(ar);
                request.Abort();

                requestDone = true;

                using (Stream s = HttpUtility.PrepareResponseStream(webResponse))
                {
                    using (bmp = new Bitmap(s))
                    {
                        LoadImageInfo loadImageInfo = _images[request];

                        fileName = loadImageInfo.ImageName;

                        afterLoadImageEvent = loadImageInfo.Event;

                        _images.Remove(request);

                        bmp.Save(SystemConfiguration.AppInstallPath + @"\Cache\Files\" + fileName, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }

                //afterLoadImageEvent.Invoke(this, new AfterLoadImageEventArgs(fileName));
            }
            catch (IOException)
            {
                throw;
            }
            catch (Exception ex)
            {
                //if (retryCount < 50)
                //{
                //    //int workerThreads;
                //    //int completionPortThreads;

                //    //System.Threading.ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);

                //    System.Threading.Thread.Sleep(1000);

                //    retryCount++;
                //}
                //else
                //{
                requestDone = true;

                LoadImageInfo loadImageInfo = _images[request];

                fileName = string.Empty;

                afterLoadImageEvent = loadImageInfo.Event;

                _images.Remove(request);

                DebugHelper.FlushTraceBuffer();

                DebugHelper.WriteLogEntry(ex, null);

                //afterLoadImageEvent.Invoke(this, new AfterLoadImageEventArgs(fileName));
                //}
            }

            //afterLoadImageEvent.Invoke(this, new AfterLoadImageEventArgs(fileName));
            //}

            //afterLoadImageEvent.Invoke(this, new AfterLoadImageEventArgs(fileName));            
        }

        #endregion

        #region Events

        public event LogRequestEventHandler LogRequestEvent;

        public event LogResponseEventHandler LogResponseEvent;

        /// <summary>
        /// Event для сигнализирования форме о завершении загрузки изображения
        /// </summary>
        public event AfterLoadImageEventHandler AfterLoadImageEvent;

        #endregion

        #region Protected members

        protected void OnLogResponse(LogResponseEventArgs e)
        {
            if (LogResponseEvent != null) LogResponseEvent(this, e);
        }

        protected void OnLogRequest(LogRequestEventArgs e)
        {
            if (LogRequestEvent != null) LogRequestEvent(this, e);
        }

        #endregion
    }
}
