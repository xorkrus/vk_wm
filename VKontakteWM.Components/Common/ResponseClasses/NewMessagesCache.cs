using System;

using System.Collections.Generic;
using System.Text;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using System.Text.RegularExpressions;
using Galssoft.VKontakteWM.Components.Data;

namespace Galssoft.VKontakteWM.Components.Common.ResponseClasses
{
    /// <summary>
    /// "Черновик" сообщения
    /// </summary>
    public class DraftMessage
    {
        /// <summary>
        /// ID получателя
        /// </summary>
        public int dmUserID  { get; set; }

        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string dmText { get; set; }

        /// <summary>
        /// Инициализация нового "черновика" сообщения
        /// </summary>
        public DraftMessage()
        {
            dmUserID = 0;

            dmText = string.Empty;
        }
    }

    /// <summary>
    /// Список "черновиков" сообщения
    /// </summary>
    public class DraftMessagesData
    {
        /// <summary>
        /// "Черновики" сообщения
        /// </summary>
        public List<DraftMessage> dmdDraftMessages = new List<DraftMessage>();
    }

    /// <summary>
    /// Функции для работы с "черновиками" сообшения
    /// </summary>
    public static class DraftMessagesDataIO
    {
        /// <summary>
        /// Загрузка "черновика" сообщения
        /// </summary>
        /// <param name="uid">ID получателя</param>
        /// <returns>Текст сообщения</returns>
        public static string GetDraftMessage(int uid)
        {
            DraftMessagesData newDraftMessagesData = null;

            //// загрузка данных из кэша
            //try
            //{
            //    DebugHelper.WriteLogEntry("(Кэш) Загрузка newDraftMessagesData...");

            //    newDraftMessagesData = Cache.Cache.LoadFromCache<DraftMessagesData>(string.Empty, "DraftMessagesData");

            //    DebugHelper.WriteLogEntry("(Кэш) newDraftMessagesData успешно загружено.");
            //}
            //catch (Exception newExeption)
            //{
            //    DebugHelper.WriteLogEntry("(Кэш) В процессе загрузки newDraftMessagesData произошла ошибка: " + newExeption.Message);

            //    return string.Empty;
            //}

            newDraftMessagesData = DataModel.Data.DraftMessagesData;

            if (newDraftMessagesData == null)
            {
                newDraftMessagesData = new DraftMessagesData();
            }

                foreach (DraftMessage newDraftMessage in newDraftMessagesData.dmdDraftMessages)
                {
                    if (newDraftMessage.dmUserID == uid)
                    {
                        string text = newDraftMessage.dmText;

                        // обратная обработка text
                        text = Regex.Replace(text, "<nl>", "\r\n");

                        return text;
                    }
                }            

            return string.Empty;
        }

        /// <summary>
        /// Сохранение "черновика" сообщения
        /// </summary>
        /// <param name="text">Текст сообщения</param>
        /// <param name="uid">ID получателя</param>
        /// <returns>Результат выполнения операции сохранения "черновика" сообщения</returns>
        public static bool SetDraftMessage(string text, int uid)
        {
            DraftMessagesData newDraftMessagesData = null;

            // прямая обработка text
            text = Regex.Replace(text, "\r\n", "<nl>");

            //// загрузка данных из кэша
            //try
            //{
            //    DebugHelper.WriteLogEntry("(Кэш) Загрузка newDraftMessagesData...");

            //    newDraftMessagesData = Cache.Cache.LoadFromCache<DraftMessagesData>(string.Empty, "DraftMessagesData");

            //    DebugHelper.WriteLogEntry("(Кэш) newDraftMessagesData успешно загружено.");
            //}
            //catch (Exception newExeption)
            //{
            //    newDraftMessagesData = new DraftMessagesData();

            //    DebugHelper.WriteLogEntry("(Кэш) В процессе загрузки newDraftMessagesData произошла ошибка: " + newExeption.Message);
            //}

            newDraftMessagesData = DataModel.Data.DraftMessagesData;

            if (newDraftMessagesData == null)
            {
                newDraftMessagesData = new DraftMessagesData();
            }

            bool isInCache = false;            

            foreach (DraftMessage newDraftMessage in newDraftMessagesData.dmdDraftMessages)
            {
                if (newDraftMessage.dmUserID == uid)
                {
                    newDraftMessage.dmText = text;

                    isInCache = true;
                }
            }

            if (!isInCache)
            {
                DraftMessage newDraftMessage = new DraftMessage();

                newDraftMessage.dmUserID = uid;
                newDraftMessage.dmText = text;

                newDraftMessagesData.dmdDraftMessages.Add(newDraftMessage);
            }

            ////сохранение данных в кэш
            //try
            //{
            //    DebugHelper.WriteLogEntry("(Кэш) Сохранение newDraftMessagesData...");

            //    bool result;

            //    result = Cache.Cache.SaveToCache(newDraftMessagesData, string.Empty, "DraftMessagesData");

            //    if (result)
            //    {
            //        DebugHelper.WriteLogEntry("(Кэш) newDraftMessagesData успешно сохранено.");
            //    }
            //    else
            //    {
            //        DebugHelper.WriteLogEntry("(Кэш) newDraftMessagesData не сохранено.");
            //    }
            //}
            //catch (Exception newException)
            //{
            //    DebugHelper.WriteLogEntry("(Кэш) В процессе сохранения newDraftMessagesData произошла ошибка: " + newException.Message);

            //    return false;
            //}

            if (newDraftMessagesData != null)
            {
                DataModel.Data.DraftMessagesData = newDraftMessagesData;
            }

            return true;
        }

        /// <summary>
        /// Удаление "черновика сообщения"
        /// </summary>
        /// <param name="uid">ID получателя</param>
        /// <returns>Результат выполнения операции удаления "черновика" сообщения</returns>
        public static bool DeleteDraftMessage(int uid)
        {
            DraftMessagesData newDraftMessagesData = null;

            ////загрузка данных из кэша
            //try
            //{
            //    DebugHelper.WriteLogEntry("(Кэш) Загрузка newDraftMessagesData...");

            //    newDraftMessagesData = Cache.Cache.LoadFromCache<DraftMessagesData>(string.Empty, "DraftMessagesData");

            //    DebugHelper.WriteLogEntry("(Кэш) newDraftMessagesData успешно загружено.");
            //}
            //catch (Exception newExeption)
            //{
            //    DebugHelper.WriteLogEntry("(Кэш) В процессе загрузки newDraftMessagesData произошла ошибка: " + newExeption.Message);

            //    return false;
            //}

            newDraftMessagesData = DataModel.Data.DraftMessagesData;

            if (newDraftMessagesData == null)
            {
                newDraftMessagesData = new DraftMessagesData();
            }

            bool isInCache = false;
            DraftMessage deletedDraftMessage = null;

            foreach (DraftMessage newDraftMessage in newDraftMessagesData.dmdDraftMessages)
            {
                if (newDraftMessage.dmUserID == uid)
                {
                    deletedDraftMessage = newDraftMessage;

                    isInCache = true;
                }
            }

            if (isInCache)
            {
                if (newDraftMessagesData.dmdDraftMessages.Remove(deletedDraftMessage))
                {
                    ////сохранение данных в кэш
                    //try
                    //{
                    //    DebugHelper.WriteLogEntry("(Кэш) Сохранение newDraftMessagesData...");

                    //    bool result;

                    //    result = Cache.Cache.SaveToCache(newDraftMessagesData, string.Empty, "DraftMessagesData");

                    //    if (result)
                    //    {
                    //        DebugHelper.WriteLogEntry("(Кэш) newDraftMessagesData успешно сохранено.");
                    //    }
                    //    else
                    //    {
                    //        DebugHelper.WriteLogEntry("(Кэш) newDraftMessagesData не сохранено.");
                    //    }

                    //    return result;
                    //}
                    //catch (Exception newException)
                    //{
                    //    DebugHelper.WriteLogEntry("(Кэш) В процессе сохранения newDraftMessagesData произошла ошибка: " + newException.Message);

                    //    return false;
                    //}

                    if (newDraftMessagesData != null)
                    {
                        DataModel.Data.DraftMessagesData = newDraftMessagesData;                        
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }

}
