using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Net;
using Galssoft.VKontakteWM.Components.Common.ResponseClasses;
using Galssoft.VKontakteWM.Components.ResponseClasses;
using Galssoft.VKontakteWM.Components.SystemHelpers;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Galssoft.VKontakteWM.Components.Server
{
    public sealed class ParsingHelper
    {
        /// <summary>
        /// Функция для получения валидной строки для сообщений, комментариев и статусов
        /// </summary>
        /// <param name="nonActualString">Исходная строка</param>
        /// <returns>Валидная срока</returns>
        public static string GetActualString(string nonActualString)
        {
            int badIndex = nonActualString.IndexOf("&");
            while (badIndex != -1)
            {
                nonActualString = nonActualString.Remove(badIndex, 1);
                badIndex = nonActualString.IndexOf("&");
            }

            badIndex = nonActualString.IndexOf("+");
            while (badIndex != -1)
            {
                nonActualString = nonActualString.Remove(badIndex, 1);
                badIndex = nonActualString.IndexOf("+");
            }
            return nonActualString;
        }

        /// <summary>
        /// Парсинг ответа на запрос изменений в сообщениях
        /// </summary>
        /// <param name="serverResponse">Поток с ответом сервера</param>
        /// <param name="UserID">ID пользователя</param>
        /// <param name="errorResponse">Объект для хранения кода ошибки</param>
        public static MessChangesResponse ParseMessageChanges(Stream serverResponse, int UserID, out ErrorResponse errorResponse)
        {
            MessChangesResponse newMessChangesResponse = new MessChangesResponse();

            errorResponse = null;

            JsonTextReader newJsonTextReader = new JsonTextReader(new StreamReader(serverResponse));

            try
            {
                while (newJsonTextReader.Read())
                {
                    //<...> сообщения

                    if (newJsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(newJsonTextReader.Value, "h"))
                    {
                        newJsonTextReader.Read();

                        //открываем очередь чтения списка изменений
                        if (newJsonTextReader.TokenType == JsonToken.StartArray)
                        {
                            newJsonTextReader.Read();

                            while (newJsonTextReader.TokenType != JsonToken.EndArray)
                            {

                                // открываем очередь чтения одного изменения
                                if (newJsonTextReader.TokenType == JsonToken.StartArray)
                                {


                                    MessChange newMessageChange = new MessChange();
                                    newMessChangesResponse.MessList.Add(newMessageChange);

                                    int ChangeIndex = 0;

                                    newJsonTextReader.Read();

                                    while (newJsonTextReader.TokenType != JsonToken.EndArray)
                                    {

                                        //
                                        switch (ChangeIndex)
                                        {
                                            case 0:
                                                newMessageChange.VersionNum = Convert.ToInt32(newJsonTextReader.Value);
                                                break;
                                            case 1:
                                                newMessageChange.ActionType = (string)newJsonTextReader.Value;
                                                break;
                                        }


                                        //открываем очередь чтения сообщения
                                        if ((newJsonTextReader.TokenType == JsonToken.StartArray) && (ChangeIndex == 2))
                                        {
                                            //номер элемента в массиве конкретного сообщения
                                            int index = 0;
                                            newJsonTextReader.Read();
                                            while (newJsonTextReader.TokenType != JsonToken.EndArray)
                                            {
                                                switch (index)
                                                {
                                                    case 0:
                                                        newMessageChange.Message.mID = Convert.ToInt32(newJsonTextReader.Value);
                                                        break;

                                                    case 1:
                                                        newMessageChange.Message.mTime = newMessageChange.Message.mTime.AddSeconds(Convert.ToInt32(newJsonTextReader.Value));
                                                        newMessageChange.Message.mTime = newMessageChange.Message.mTime.ToLocalTime();
                                                        break;
                                                    case 5:
                                                        if (newMessageChange.Message.mIOType == MessageIOType.Inbox) 
                                                            newMessageChange.Message.mMesIsRead = Convert.ToBoolean(newJsonTextReader.Value);
                                                        break;
                                                }
                                                //Чтение текста сообщения
                                                if ((newJsonTextReader.TokenType == JsonToken.StartArray) && (index == 2))
                                                {
                                                    int iindex = 0;
                                                    newJsonTextReader.Read();

                                                    while (newJsonTextReader.TokenType != JsonToken.EndArray)
                                                    {


                                                        switch (iindex)
                                                        {
                                                            case 0:
                                                                newMessageChange.Message.mData.mText = (string)newJsonTextReader.Value;

                                                                newMessageChange.Message.mData.mText = Regex.Replace(newMessageChange.Message.mData.mText, "<br>", "\n", RegexOptions.IgnoreCase);
                                                                //newMessageChange.Message.mData.mText = Regex.Replace(newMessageChange.Message.mData.mText, "<[^>]*?>", string.Empty, RegexOptions.IgnoreCase);
                                                                newMessageChange.Message.mData.mText = Regex.Replace(newMessageChange.Message.mData.mText, "&quot;", @"""", RegexOptions.IgnoreCase);
                                                                newMessageChange.Message.mData.mText = Regex.Replace(newMessageChange.Message.mData.mText, "&#39;", "'", RegexOptions.IgnoreCase);
                                                                newMessageChange.Message.mData.mText = Regex.Replace(newMessageChange.Message.mData.mText, "&lt;", "<", RegexOptions.IgnoreCase);
                                                                newMessageChange.Message.mData.mText = Regex.Replace(newMessageChange.Message.mData.mText, "&gt;", ">", RegexOptions.IgnoreCase);
                                                                newMessageChange.Message.mData.mText = Regex.Replace(newMessageChange.Message.mData.mText, "&amp;", "&", RegexOptions.IgnoreCase);

                                                                break;

                                                            case 1:
                                                                newMessageChange.Message.mData.mDataType = (MessageDataType)Convert.ToInt32(newJsonTextReader.Value);
                                                                break;

                                                            case 2:
                                                                newMessageChange.Message.mData.mElementName = (string)newJsonTextReader.Value;
                                                                break;

                                                            case 3:
                                                                //т.к. последовательность полей зависти от типов данных,
                                                                //для явно определяемых типов: граффити, видеозапись, аудиозапись
                                                                //выбираем необходимые поля вручную
                                                                switch (newMessageChange.Message.mData.mDataType)
                                                                {
                                                                    case MessageDataType.Audio:
                                                                        newMessageChange.Message.mData.mElementURL = (string)newJsonTextReader.Value;
                                                                        break;

                                                                    case MessageDataType.Photo:
                                                                    case MessageDataType.Graffiti:
                                                                    case MessageDataType.Video:
                                                                        newMessageChange.Message.mData.mElementThumbImageURL = (string)newJsonTextReader.Value;
                                                                        break;
                                                                }
                                                                break;

                                                            case 4:
                                                                switch (newMessageChange.Message.mData.mDataType)
                                                                {
                                                                    case MessageDataType.Audio:
                                                                        newMessageChange.Message.mData.mElementOwnerID = Convert.ToInt32(newJsonTextReader.Value);
                                                                        break;

                                                                    case MessageDataType.Photo:
                                                                    case MessageDataType.Graffiti:
                                                                    case MessageDataType.Video:
                                                                        newMessageChange.Message.mData.mElementURL = (string)newJsonTextReader.Value;
                                                                        break;
                                                                }
                                                                break;

                                                            case 5:
                                                                switch (newMessageChange.Message.mData.mDataType)
                                                                {
                                                                    case MessageDataType.Audio:
                                                                        newMessageChange.Message.mData.mElementID = Convert.ToInt32(newJsonTextReader.Value);
                                                                        break;

                                                                    case MessageDataType.Photo:
                                                                    case MessageDataType.Graffiti:
                                                                    case MessageDataType.Video:
                                                                        newMessageChange.Message.mData.mElementOwnerID = Convert.ToInt32(newJsonTextReader.Value);
                                                                        break;
                                                                }
                                                                break;

                                                            case 6:
                                                                switch (newMessageChange.Message.mData.mDataType)
                                                                {
                                                                    case MessageDataType.Audio:
                                                                        newMessageChange.Message.mData.mElementSize = Convert.ToInt32(newJsonTextReader.Value);
                                                                        break;

                                                                    case MessageDataType.Photo:
                                                                    case MessageDataType.Graffiti:
                                                                    case MessageDataType.Video:
                                                                        newMessageChange.Message.mData.mElementID = Convert.ToInt32(newJsonTextReader.Value);
                                                                        break;
                                                                }
                                                                break;
                                                        }

                                                        iindex++;

                                                        newJsonTextReader.Read();

                                                    }
                                                }
                                                //Чтение информации об отправителе
                                                if ((newJsonTextReader.TokenType == JsonToken.StartArray) && (index == 3))
                                                {
                                                    int iindex = 0;
                                                    newJsonTextReader.Read();

                                                    while (newJsonTextReader.TokenType != JsonToken.EndArray)
                                                    {

                                                        switch (iindex)
                                                        {
                                                            case 0:
                                                                newMessageChange.Message.mMesSender.mUserID = Convert.ToInt32(newJsonTextReader.Value);
                                                                //если отправитель сообщения - это я, то сообщение исходящее
                                                                if (newMessageChange.Message.mMesSender.mUserID == UserID) newMessageChange.Message.mIOType = MessageIOType.Outbox;
                                                                else newMessageChange.Message.mIOType = MessageIOType.Inbox;
                                                                break;

                                                            case 1:
                                                                newMessageChange.Message.mMesSender.mUserName = (string)newJsonTextReader.Value;
                                                                newMessageChange.Message.mMesSender.mUserName = Regex.Replace(newMessageChange.Message.mMesSender.mUserName, "\t", " ", RegexOptions.IgnoreCase);

                                                                break;

                                                            case 2:
                                                                newMessageChange.Message.mMesSender.mUserPhotoURL = (string)newJsonTextReader.Value;
                                                                break;

                                                            case 3:
                                                                newMessageChange.Message.mMesSender.mSmallUserPhotoName = (string)newJsonTextReader.Value;
                                                                break;

                                                            case 4:
                                                                newMessageChange.Message.mMesSender.mUserSex = Convert.ToInt32(newJsonTextReader.Value);
                                                                break;

                                                            case 5:
                                                                newMessageChange.Message.mMesSender.mUserIsOnline = Convert.ToInt32(newJsonTextReader.Value);
                                                                break;
                                                        }

                                                        newJsonTextReader.Read();
                                                        iindex++;

                                                    }


                                                }

                                                //Чтение информации о получателе
                                                if ((newJsonTextReader.TokenType == JsonToken.StartArray) && (index == 4))
                                                {
                                                    int iindex = 0;
                                                    newJsonTextReader.Read();

                                                    while (newJsonTextReader.TokenType != JsonToken.EndArray)
                                                    {
                                                        switch (iindex)
                                                        {
                                                            case 0:
                                                                newMessageChange.Message.mMesReceiver.mUserID = Convert.ToInt32(newJsonTextReader.Value);
                                                                break;

                                                            case 1:
                                                                newMessageChange.Message.mMesReceiver.mUserName = (string)newJsonTextReader.Value;
                                                                newMessageChange.Message.mMesReceiver.mUserName = Regex.Replace(newMessageChange.Message.mMesReceiver.mUserName, "\t", " ", RegexOptions.IgnoreCase);
                                                                break;

                                                            case 2:
                                                                newMessageChange.Message.mMesReceiver.mUserPhotoURL = (string)newJsonTextReader.Value;
                                                                break;

                                                            case 3:
                                                                newMessageChange.Message.mMesReceiver.mSmallUserPhotoName = (string)newJsonTextReader.Value;
                                                                break;

                                                            case 4:
                                                                newMessageChange.Message.mMesReceiver.mUserSex = Convert.ToInt32(newJsonTextReader.Value);
                                                                break;

                                                            case 5:
                                                                newMessageChange.Message.mMesReceiver.mUserIsOnline = Convert.ToInt32(newJsonTextReader.Value);
                                                                break;
                                                        }
                                                        iindex++;
                                                        newJsonTextReader.Read();

                                                    }


                                                }


                                                newJsonTextReader.Read();
                                                index++;



                                            }



                                        }
                                        newJsonTextReader.Read();
                                        ChangeIndex++;
                                    }


                                }


                                newJsonTextReader.Read();
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

                return newMessChangesResponse;
            }
            catch (Exception)
            {
                throw;

            }

        }

        /// <summary>
        /// Парсинг ответа на запрос отправки сообщения
        /// </summary>
        /// <param name="serverResponse">Поток с ответом сервера</param>
        /// <param name="errorResponse">Объект для хранения кода ошибки</param>
        public static bool SendMessageResponse(Stream serverResponse, out ErrorResponse errorResponse)
        {
            errorResponse = null;

            bool result = false;

            JsonTextReader newJsonTextReader = new JsonTextReader(new StreamReader(serverResponse));

            try
            {
                while (newJsonTextReader.Read())
                {
                    //ответ
                    if (newJsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(newJsonTextReader.Value, "ok"))
                    {
                        errorResponse = new ErrorResponse();

                        newJsonTextReader.Read();

                        int error_code = Convert.ToInt32(newJsonTextReader.Value);

                        switch (error_code)
                        {
                            //операция прошла успешно
                            case 1:                                
                                result = true;
                                break;

                            //операция прошла не успешно
                            case 0:                                
                                result = false;
                                break;

                            //истекшая сессия
                            case -1:                                
                                errorResponse.error_code = "1";
                                break;

                            //флуд-контроль
                            case -2:                                
                                errorResponse.error_code = "2";
                                break;

                            //операция запрещена настройками приватности
                            case -3:                                
                                errorResponse.error_code = "3";
                                break;
                        }
                    }
                }
            }            
            finally
            {
                //
            }

            return result;
        }
        /// <summary>
        /// Парсинг ответа на запрос загрузки сообщений
        /// </summary>
        /// <param name="serverResponse">Поток с ответом сервера</param>
        /// <param name="UserID">ID пользователя</param>
        /// <param name="errorResponse">Объект для хранения кода ошибки</param>
        public static MessResponse ParseMessages(Stream serverResponse, int UserID, out ErrorResponse errorResponse)
        {
            MessResponse newMessResponse = new MessResponse();

            errorResponse = null;

            JsonTextReader newJsonTextReader = new JsonTextReader(new StreamReader(serverResponse));

            try
            {
                while (newJsonTextReader.Read())
                {
                    //<...> сообщения
                    if (newJsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(newJsonTextReader.Value, "n"))
                    {
                        newJsonTextReader.Read();

                        newMessResponse.mCount = Convert.ToInt32(newJsonTextReader.Value);
                    }
                    if (newJsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(newJsonTextReader.Value, "h"))
                    {
                        newJsonTextReader.Read();

                        newMessResponse.VersionNum = Convert.ToInt32(newJsonTextReader.Value);
                    }

                    if (newJsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(newJsonTextReader.Value, "d"))
                    {
                        newJsonTextReader.Read();

                        //открыли массив 1-ого порядка (список сообщений)
                        if (newJsonTextReader.TokenType == JsonToken.StartArray)
                        {
                            newJsonTextReader.Read();

                            while (newJsonTextReader.TokenType != JsonToken.EndArray) //открываем очередь чтения списка сообщений
                            {
                                //читаем содержимое списка сообщений
                                //открыли массив 2-ого порядка (отдельное сообщение)

                                if (newJsonTextReader.TokenType == JsonToken.StartArray)
                                {
                                    MessageCover newMessageCover = new MessageCover();
                                    newMessResponse.mMessages.Add(newMessageCover);

                                    int index = 0; //номер элемента в массиве конкретного сообщения

                                    newJsonTextReader.Read();

                                    while (newJsonTextReader.TokenType != JsonToken.EndArray) //открываем очередь чтения сообщения
                                    {
                                        switch (index)
                                        {
                                            case 0:
                                                newMessageCover.mID = Convert.ToInt32(newJsonTextReader.Value);
                                                break;

                                            case 1:
                                                newMessageCover.mTime = newMessageCover.mTime.AddSeconds(Convert.ToInt32(newJsonTextReader.Value));
                                                newMessageCover.mTime = newMessageCover.mTime.ToLocalTime();
                                                break;
                                            case 5:
                                                if (newMessageCover.mIOType == MessageIOType.Inbox)
                                                    newMessageCover.mMesIsRead = Convert.ToBoolean(newJsonTextReader.Value);
                                                break;
                                        }
                                        //Чтение текста сообщения
                                        if ((newJsonTextReader.TokenType == JsonToken.StartArray) && (index == 2))
                                        {
                                            int iindex = 0;
                                            newJsonTextReader.Read();

                                            while (newJsonTextReader.TokenType != JsonToken.EndArray)
                                            {
                                                switch (iindex)
                                                {
                                                    case 0:
                                                        newMessageCover.mData.mText = (string)newJsonTextReader.Value;

                                                        newMessageCover.mData.mText = Regex.Replace(newMessageCover.mData.mText, "<br>", "\n", RegexOptions.IgnoreCase);                                             

                                                        newMessageCover.mData.mText = Regex.Replace(newMessageCover.mData.mText, "&quot;", @"""", RegexOptions.IgnoreCase);
                                                        newMessageCover.mData.mText = Regex.Replace(newMessageCover.mData.mText, "&#39;", "'", RegexOptions.IgnoreCase);
                                                        newMessageCover.mData.mText = Regex.Replace(newMessageCover.mData.mText, "&lt;", "<", RegexOptions.IgnoreCase);
                                                        newMessageCover.mData.mText = Regex.Replace(newMessageCover.mData.mText, "&gt;", ">", RegexOptions.IgnoreCase);
                                                        newMessageCover.mData.mText = Regex.Replace(newMessageCover.mData.mText, "&amp;", "&", RegexOptions.IgnoreCase);

                                                        break;

                                                    case 1:
                                                        newMessageCover.mData.mDataType = (MessageDataType)Convert.ToInt32(newJsonTextReader.Value);
                                                        break;

                                                    case 2:
                                                        newMessageCover.mData.mElementName = (string)newJsonTextReader.Value;
                                                        break;

                                                    case 3:
                                                        //т.к. последовательность полей зависти от типов данных,
                                                        //для явно определяемых типов: граффити, видеозапись, аудиозапись
                                                        //выбираем необходимые поля вручную
                                                        switch (newMessageCover.mData.mDataType)
                                                        {
                                                            case MessageDataType.Audio:
                                                                newMessageCover.mData.mElementURL = (string)newJsonTextReader.Value;
                                                                break;

                                                            case MessageDataType.Photo:
                                                            case MessageDataType.Graffiti:
                                                            case MessageDataType.Video:
                                                                newMessageCover.mData.mElementThumbImageURL = (string)newJsonTextReader.Value;
                                                                break;
                                                        }
                                                        break;

                                                    case 4:
                                                        switch (newMessageCover.mData.mDataType)
                                                        {
                                                            case MessageDataType.Audio:
                                                                newMessageCover.mData.mElementOwnerID = Convert.ToInt32(newJsonTextReader.Value);
                                                                break;

                                                            case MessageDataType.Photo:
                                                            case MessageDataType.Graffiti:
                                                            case MessageDataType.Video:
                                                                newMessageCover.mData.mElementURL = (string)newJsonTextReader.Value;
                                                                break;
                                                        }
                                                        break;

                                                    case 5:
                                                        switch (newMessageCover.mData.mDataType)
                                                        {
                                                            case MessageDataType.Audio:
                                                                newMessageCover.mData.mElementID = Convert.ToInt32(newJsonTextReader.Value);
                                                                break;

                                                            case MessageDataType.Photo:
                                                            case MessageDataType.Graffiti:
                                                            case MessageDataType.Video:
                                                                newMessageCover.mData.mElementOwnerID = Convert.ToInt32(newJsonTextReader.Value);
                                                                break;
                                                        }
                                                        break;

                                                    case 6:
                                                        switch (newMessageCover.mData.mDataType)
                                                        {
                                                            case MessageDataType.Audio:
                                                                newMessageCover.mData.mElementSize = Convert.ToInt32(newJsonTextReader.Value);
                                                                break;

                                                            case MessageDataType.Photo:
                                                            case MessageDataType.Graffiti:
                                                            case MessageDataType.Video:
                                                                newMessageCover.mData.mElementID = Convert.ToInt32(newJsonTextReader.Value);
                                                                break;
                                                        }
                                                        break;
                                                }

                                                iindex++;

                                                newJsonTextReader.Read();

                                            }
                                        }
                                        //Чтение информации об отправителе
                                        if ((newJsonTextReader.TokenType == JsonToken.StartArray) && (index == 3))
                                        {
                                            int iindex = 0;
                                            newJsonTextReader.Read();

                                            while (newJsonTextReader.TokenType != JsonToken.EndArray)
                                            {

                                                switch (iindex)
                                                {
                                                    case 0:
                                                        newMessageCover.mMesSender.mUserID = Convert.ToInt32(newJsonTextReader.Value);
                                                        //если отправитель сообщения - это я, то сообщение исходящее
                                                        if (newMessageCover.mMesSender.mUserID == UserID)
                                                        {
                                                            newMessageCover.mIOType = MessageIOType.Outbox;
                                                        }
                                                        else
                                                        {
                                                            newMessageCover.mIOType = MessageIOType.Inbox;
                                                        }

                                                        break;

                                                    case 1:
                                                        newMessageCover.mMesSender.mUserName = (string)newJsonTextReader.Value;
                                                        newMessageCover.mMesSender.mUserName = Regex.Replace(newMessageCover.mMesSender.mUserName, "\t", " ", RegexOptions.IgnoreCase);

                                                        break;

                                                    case 2:
                                                        newMessageCover.mMesSender.mUserPhotoURL = (string)newJsonTextReader.Value;
                                                        break;

                                                    case 3:
                                                        newMessageCover.mMesSender.mSmallUserPhotoName = (string)newJsonTextReader.Value;
                                                        break;

                                                    case 4:
                                                        newMessageCover.mMesSender.mUserSex = Convert.ToInt32(newJsonTextReader.Value);
                                                        break;

                                                    case 5:
                                                        newMessageCover.mMesSender.mUserIsOnline = Convert.ToInt32(newJsonTextReader.Value);
                                                        break;
                                                }

                                                newJsonTextReader.Read();
                                                iindex++;

                                            }


                                        }

                                        //Чтение информации о получателе
                                        if ((newJsonTextReader.TokenType == JsonToken.StartArray) && (index == 4))
                                        {
                                            int iindex = 0;
                                            newJsonTextReader.Read();

                                            while (newJsonTextReader.TokenType != JsonToken.EndArray)
                                            {
                                                switch (iindex)
                                                {
                                                    case 0:
                                                        newMessageCover.mMesReceiver.mUserID = Convert.ToInt32(newJsonTextReader.Value);
                                                        break;

                                                    case 1:
                                                        newMessageCover.mMesReceiver.mUserName = (string)newJsonTextReader.Value;
                                                        newMessageCover.mMesReceiver.mUserName = Regex.Replace(newMessageCover.mMesReceiver.mUserName, "\t", " ", RegexOptions.IgnoreCase);
                                                        break;

                                                    case 2:
                                                        newMessageCover.mMesReceiver.mUserPhotoURL = (string)newJsonTextReader.Value;
                                                        break;

                                                    case 3:
                                                        newMessageCover.mMesReceiver.mSmallUserPhotoName = (string)newJsonTextReader.Value;
                                                        break;

                                                    case 4:
                                                        newMessageCover.mMesReceiver.mUserSex = Convert.ToInt32(newJsonTextReader.Value);
                                                        break;

                                                    case 5:
                                                        newMessageCover.mMesReceiver.mUserIsOnline = Convert.ToInt32(newJsonTextReader.Value);
                                                        break;
                                                }
                                                iindex++;
                                                newJsonTextReader.Read();

                                            }


                                        }


                                        newJsonTextReader.Read();
                                        index++;
                                    }
                                }


                                newJsonTextReader.Read();
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

                return newMessResponse;
            }
            catch (Exception)
            {
                throw;

            }

        }
      
        public static RawEventsGetResponse EventsMFCParse(Stream serverResponse, out ErrorResponse errorResponse)
        {
            RawEventsGetResponse response = new RawEventsGetResponse();

            errorResponse = null;

            serverResponse.Position = 0;

            JsonTextReader jsonTextReader = new JsonTextReader(new StreamReader(serverResponse));

            try
            {
                while (jsonTextReader.Read())
                {
                    // MessagesCount
                    if (jsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(jsonTextReader.Value, "nm"))
                    {
                        jsonTextReader.Read();

                        response.MessagesCount = Convert.ToInt32(jsonTextReader.Value);

                        continue;
                    }

                    // FriendsCount
                    if (jsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(jsonTextReader.Value, "nf"))
                    {
                        jsonTextReader.Read();

                        response.FriendsCount = Convert.ToInt32(jsonTextReader.Value);

                        continue;
                    }

                    if (jsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(jsonTextReader, "ok"))
                    {
                        errorResponse = new ErrorResponse();

                        jsonTextReader.Read();

                        int error_code = Convert.ToInt32(jsonTextReader.Value);

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

                return response;
            }
            catch (Exception)
            {     
                throw;
            }            
        }

        public static int SpecialEventsParse(Stream serverResponse, out ErrorResponse errorResponse)
        {
            errorResponse = new ErrorResponse();
            int c = 0;

            serverResponse.Position = 0;
            JsonTextReader jsonTextReader = new JsonTextReader(new StreamReader(serverResponse));
            try
            {
                while (jsonTextReader.Read())
                {
                    // FriendsPhotosCount
                    if (jsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(jsonTextReader.Value, "n"))
                    {
                        jsonTextReader.Read();
                        c = Convert.ToInt32(jsonTextReader.Value);
                        break;
                    }

                    if (jsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(jsonTextReader, "ok"))
                    {
                        jsonTextReader.Read();

                        int error_code = Convert.ToInt32(jsonTextReader.Value);

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

                        return -1;
                    }
                }
                return c;
            }
            catch (Exception)
            {
                throw;
            }   
        }

        public static User ProfileJSONParse(Stream serverResponse, out ErrorResponse errorResponse)
        {
            errorResponse = null;

            User usr = new User();
            string day, month, year;
            day = month = year = string.Empty;

            JsonTextReader jsonTextReader = new JsonTextReader(new StreamReader(serverResponse));

            try
            {
                while (jsonTextReader.Read())
                {
                    // FirstName
                    if (jsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(jsonTextReader.Value, "fn"))
                    {
                        jsonTextReader.Read();
                        usr.FirstName = jsonTextReader.Value.ToString();
                        continue;
                    }

                    // LastName
                    if (jsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(jsonTextReader.Value, "ln"))
                    {
                        jsonTextReader.Read();
                        usr.LastName = jsonTextReader.Value.ToString();
                        continue;
                    }

                    // BirthDate
                    if ((jsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(jsonTextReader.Value, "bd")) ||
                        (jsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(jsonTextReader.Value, "bm")) ||
                        (jsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(jsonTextReader.Value, "by")))
                    {
                        switch (jsonTextReader.Value.ToString())
                        {
                            case "bd":
                                jsonTextReader.Read();
                                day = jsonTextReader.Value.ToString();
                                break;
                            case "bm":
                                jsonTextReader.Read();
                                month = jsonTextReader.Value.ToString();
                                break;
                            case "by":
                                jsonTextReader.Read();
                                year = jsonTextReader.Value.ToString();
                                break;
                        }

                        if (day != "0" && day != string.Empty && month != "0" && month != string.Empty && year != "0" && year != string.Empty)
                        {                            
                            try
                            {
                                usr.Birthday = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day));
                            }
                            catch
                            {
                                usr.Birthday = new DateTime(0);
                            }

                            //usr.Birthday = (Convert.ToDateTime(month + "/" + day + "/" + year)).ToString("D",new CultureInfo("ru-RU"));
                        }

                        continue;
                    }

                    // Photo
                    if (jsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(jsonTextReader.Value, "bp"))
                    {
                        jsonTextReader.Read();

                        if (jsonTextReader.Value.ToString() != "0")
                            usr.Photo200px = jsonTextReader.Value.ToString();

                        continue;
                    }

                    // Status
                    if (jsonTextReader.TokenType == JsonToken.PropertyName &&
                        string.Equals(jsonTextReader.Value, "actv"))
                    {
                        while (jsonTextReader.Read())
                        {
                            if (jsonTextReader.TokenType == JsonToken.PropertyName &&
                                string.Equals(jsonTextReader.Value, "5"))
                            {
                                jsonTextReader.Read();
                                if (jsonTextReader.Value.ToString() != "")
                                    usr.Status = jsonTextReader.Value.ToString();
                                break;
                            }
                        }
                        continue;
                    }

                    // Sex
                    if (jsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(jsonTextReader.Value, "sx"))
                    {
                        jsonTextReader.Read();
                        switch (jsonTextReader.Value.ToString())
                        {
                            case "1":
                                usr.Sex = "женский";
                                break;
                            case "2":
                                usr.Sex = "мужской";
                                break;
                            default:
                                usr.Sex = "";
                                break;
                        }
                        continue;
                    }

                    // Town
                    if (jsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(jsonTextReader.Value, "cin"))
                    {
                        jsonTextReader.Read();
                        if (jsonTextReader.Value != null)
                            usr.Town = jsonTextReader.Value.ToString();
                        continue;
                    }

                    //if (string.Equals(jsonTextReader.Value, "fr") || string.Equals(jsonTextReader.Value, "fro") || string.Equals(jsonTextReader.Value, "frm") || string.Equals(jsonTextReader.Value, "ph") || string.Equals(jsonTextReader.Value, "phw") || string.Equals(jsonTextReader.Value, "pr") || string.Equals(jsonTextReader.Value, "wa"))
                    //{
                    //    int i = 5;

                    //    if (i < 10)
                    //    {
                    //        i++;
                    //    }
                    //}

                    // MobilePhone
                    if (jsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(jsonTextReader.Value, "mo"))
                    {
                        jsonTextReader.Read();
                        usr.MobilePhone = jsonTextReader.Value.ToString();
                        continue;
                    }

                    // Upload Info For Album
                    if (jsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(jsonTextReader.Value, "ph"))
                    {
                        int depth = 0;

                        while (jsonTextReader.Read())
                        {
                            if (Configuration.SystemConfiguration.Aid == null   || Configuration.SystemConfiguration.photoHash == null  || Configuration.SystemConfiguration.photoRHash == null || Configuration.SystemConfiguration.photoUploadAddress == null)
                            {
                                if (jsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(jsonTextReader.Value, "url"))
                                {
                                    jsonTextReader.Read();
                                    Configuration.SystemConfiguration.photoUploadAddress = jsonTextReader.Value.ToString();
                                    continue;
                                }

                                if (jsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(jsonTextReader.Value, "hash"))
                                {
                                    jsonTextReader.Read();
                                    Configuration.SystemConfiguration.photoHash = jsonTextReader.Value.ToString();
                                    continue;
                                }

                                if (jsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(jsonTextReader.Value, "rhash"))
                                {
                                    jsonTextReader.Read();
                                    Configuration.SystemConfiguration.photoRHash = jsonTextReader.Value.ToString();
                                    continue;
                                }

                                if (jsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(jsonTextReader.Value, "aid"))
                                {
                                    jsonTextReader.Read();
                                    Configuration.SystemConfiguration.Aid = jsonTextReader.Value.ToString();
                                    continue;
                                }

                                //if (jsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(jsonTextReader.Value, "n"))
                                //{
                                //    break; // если это не наш профиль, то структура массива несколько другая и приводит к зависаниям
                                //}

                                if (jsonTextReader.TokenType == JsonToken.StartArray || jsonTextReader.TokenType == JsonToken.StartObject)
                                {
                                    depth++;
                                }
                                if (jsonTextReader.TokenType == JsonToken.EndArray || jsonTextReader.TokenType == JsonToken.EndObject)
                                {
                                    depth--;

                                    if (depth == 0)
                                    {
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        continue;
                    }

                    // Upload Info For Avatar
                    if (jsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(jsonTextReader.Value, "pr"))
                    {
                        int depth = 0;

                        while(jsonTextReader.Read())
                        {
                            if (Configuration.SystemConfiguration.avatarUploadAddress == null || Configuration.SystemConfiguration.avatarHash == null || Configuration.SystemConfiguration.avatarRHash == null)
                            {
                                //if (jsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(jsonTextReader.Value, "pa"))
                                //{
                                //    break; // если это не наш профиль, то структура массива несколько другая и приводит к зависаниям
                                //}

                                if (jsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(jsonTextReader.Value, "url"))
                                {
                                    jsonTextReader.Read();
                                    Configuration.SystemConfiguration.avatarUploadAddress = jsonTextReader.Value.ToString();
                                    continue;
                                }
                                if (jsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(jsonTextReader.Value, "hash"))
                                {
                                    jsonTextReader.Read();
                                    Configuration.SystemConfiguration.avatarHash = jsonTextReader.Value.ToString();
                                    continue;
                                }
                                if (jsonTextReader.TokenType == JsonToken.PropertyName &&
                                    string.Equals(jsonTextReader.Value, "rhash"))
                                {
                                    jsonTextReader.Read();
                                    Configuration.SystemConfiguration.avatarRHash = jsonTextReader.Value.ToString();
                                    continue;
                                }

                                if (jsonTextReader.TokenType == JsonToken.StartArray || jsonTextReader.TokenType == JsonToken.StartObject)
                                {
                                    depth++;
                                }
                                if (jsonTextReader.TokenType == JsonToken.EndArray || jsonTextReader.TokenType == JsonToken.EndObject)
                                {
                                    depth--;

                                    if (depth == 0)
                                    {
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        continue;
                    }

                    if (jsonTextReader.TokenType == JsonToken.PropertyName && string.Equals(jsonTextReader.Value, "ok"))
                    {
                        errorResponse = new ErrorResponse();

                        jsonTextReader.Read();

                        int error_code = Convert.ToInt32(jsonTextReader.Value);

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
                return usr;
            }
            catch (Exception)
            {
                return null;
                //throw;
            }
        }
    }
}
