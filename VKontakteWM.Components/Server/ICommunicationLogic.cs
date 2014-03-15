using System.Drawing;
using System.IO;
using System.Net;
using Galssoft.VKontakteWM.Components.Common.ResponseClasses;
using Galssoft.VKontakteWM.Components.ResponseClasses;

namespace Galssoft.VKontakteWM.Components.Server
{
    public interface ICommunicationLogic
    {
        #region 1

        /// <summary>
        /// Авторизация пользователя по логину и паролю
        /// </summary>
        /// <param name="login">Логин пользователя </param>
        /// <param name="pass">Пароль </param>
        /// <param name="isRemember">Флаг, указывающий на необходимость получения токена </param>
        /// <param name="errorResponse">Объект с описанием ошибки</param>
        /// <returns>Объект с результатом аутентификации</returns>
        AuthLoginResponse AuthLogin(string login, string pass, bool isRemember);

        /// <summary>
        /// Авторизация пользователя по токену
        /// </summary>
        /// <param name="token">Токен </param>
        /// <param name="errorResponse">Объект с описанием ошибки</param>
        /// <returns>Объект с результатом аутентификации </returns>
        AuthLoginResponse AuthLoginByToken(string token, string uid);

        /// <summary>
        /// Функция добавления комментариев к фотографиям
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="ts">Текущее состояние photos_comments</param>
        /// <param name="message">Текст сообщения</param>
        /// <param name="parent">{id владльца фотографии}_{id фотографии}</param>
        /// <param name="sid">Идентификатор сессии</param>
        /// <param name="errorResponse">Объект с описанием ошибки</param>
        /// <returns></returns>
        HttpWebResponse AddPhotosComments(string uid, string message, string parent, string sid, out ErrorResponse errorResponse);

        User UserGetInfo(string uid, string sessionKey, out ErrorResponse errorResponse);

        RawEventsGetResponse GetEvents(string uid, string sessionId);

        bool UploadPhoto(string uid, string sid, string hash, string rhash, string aid, string UplAddress, byte[] file,
                          out ErrorResponse errorResponse);

        bool ChangeAvatar(string uid, string sid, string address, string hash, string rhash, byte[] file, 
                          out ErrorResponse errorResponse);

        /// <summary>
        /// Редактирование текущего статуса статуса
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="sid">Идентификатор сессии</param>
        /// <param name="newStatus">Текст статуса</param>
        /// <param name="actionType">Действие</param>
        /// <param name="errorResponse">Ошибки</param>
        /// <returns>Удачное или неудачное завершение операции</returns>
        bool SetStatus(string uid, string sid, string newStatus, StatusActionType actionType, out ErrorResponse errorResponse);

        /// <summary>
        /// Уничтожение текущей сессии
        /// </summary>
        /// <param name="sid">Идетификатор текущей сессии</param>
        /// <param name="errorResponse">Ошибки</param>
        /// <returns>Удачное или неудачное завершение операции</returns>
        bool ExpireSession(string sid, out ErrorResponse errorResponse);

        #endregion

        #region 2

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
        MessResponse LoadMessages(string mtype, string uid, string sid, string from, string to, int userid, out ErrorResponse newErrorResponse);

        /// <summary>
        /// Загружает изменения в личных сообщениях 
        /// </summary>
        /// <param name="mtype">Тип запроса: inbox (все входящие), oubox (все исходящие), message (переписка с пользователем)</param>
        /// <param name="uid">ID пользователя</param>
        /// <param name="sid">ID сессии</param>
        /// <param name="ts">Номер версии</param>
        /// <param name="userid">ID пользователя, переписка с которым запрашивается</param>
        /// <returns>Изменения в сообщениях в классе MessChangesResponse</returns>
        MessChangesResponse LoadMessChanges(string mtype, string uid, string sid, int userid, int ts, out ErrorResponse newErrorResponse);

        /// <summary>
        /// Отправляет сообщение
        /// </summary>
        /// <param name="uid">ID отправителя</param>
        /// <param name="sid">ID сессии</param>
        /// <param name="userid">ID получателя</param>
        /// <param name="messText">Текст сообщения</param>
        /// <returns>Успешность выполнения операции</returns>
        bool SendMessage(string uid, string sid, int userid, string messText, out ErrorResponse newErrorResponse);

        #endregion

        #region 3

        bool LoadImage(string uri, string fileName, bool isRefresh, AfterLoadImageEventHandler afterLoadImageEvent, int imageLinearSize, object sortKey, string sortType);

        void LoadImagesInDictionary();

        void ClearImagesInDictionary();

        ActivityResponse LoadActivityDataListData(string uid, string sid, string from, string to, out ErrorResponse errorResponse);

        FriendsListResponse LoadFriendsListData(string uid, string sid, out ErrorResponse newErrorResponse);

        FriendsListResponse LoadFriendsOnlineListData(string uid, string sid, out ErrorResponse newErrorResponse);

        PhotosCommentsResponse LoadPhotosCommentsData(string uid, string sid, string from, string to, string parent, out ErrorResponse newErrorResponse);

        UpdatesPhotosResponse LoadUpdatesPhotosData(string uid, string sid, string from, string to, out ErrorResponse newErrorResponse);

        UpdatesPhotosResponse LoadUserPhotosData(string uid, string sid, string from, string to, out ErrorResponse newErrorResponse);

        ShortActivityResponse LoadShortActivityResponseData(string uid, string sid, string from, string to, out ErrorResponse newErrorResponse);

        ShortWallResponse LoadShortWallResponseData(string uid, string sid, string from, string to, out ErrorResponse newErrorResponse);

        ShortUpdatesPhotosResponse LoadShortUpdatesPhotosResponse(string sid, string from, string to, out ErrorResponse newErrorResponse);

        ShortPhotosCommentsRespounse LoadShortPhotosCommentsRespounse(string uid, string sid, string from, string to, string parent, out ErrorResponse newErrorResponse);

        ActivityResponse LoadUserActivity(string uid, string sid, string from, string to, out ErrorResponse errorResponse);

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
    }

    #region делегаты

    public delegate void AfterLoadImageEventHandler(object sender, AfterLoadImageEventArgs e);

    public delegate void LogRequestEventHandler(object sender, LogRequestEventArgs e);

    public delegate void LogResponseEventHandler(object sender, LogResponseEventArgs e);

    #endregion
}
