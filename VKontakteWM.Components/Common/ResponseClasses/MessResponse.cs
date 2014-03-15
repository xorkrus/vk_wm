using System;
using System.Collections.Generic;
using Galssoft.VKontakteWM.Components.Common.Interfaces;

namespace Galssoft.VKontakteWM.Components.Common.ResponseClasses
{
    /// <summary>
    /// Типы содержания сообщений 
    /// </summary>
    public enum MessageDataType
    {
        PlainText = 0,
        Photo = 1,
        Graffiti = 2,
        Video = 3,
        Audio = 4
    }

    /// <summary>
    /// Типы сообщений по направлению
    /// </summary>
    public enum MessageIOType
    {

        Inbox = 1,
        Outbox = 2
    }

    /// <summary>
    /// Сообщение
    /// </summary> 
    public class MessageData
    {
        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string mText { get; set; }

        /// <summary>
        /// Тип сообщения
        /// </summary>
        public MessageDataType mDataType { get; set; }

        /// <summary>
        /// Название элемента
        /// </summary>
        public string mElementName { get; set; }

        /// <summary>
        /// URL уменьшенного изображения для элемента
        /// </summary>
        public string mElementThumbImageURL { get; set; }

        /// <summary>
        /// URL исходного элемента
        /// </summary>
        public string mElementURL { get; set; }

        /// <summary>
        /// ID владельца элемента
        /// </summary>
        public int mElementOwnerID { get; set; }

        /// <summary>
        /// ID элемента
        /// </summary>
        public int mElementID { get; set; }

        /// <summary>
        /// Hазмер элемента
        /// </summary>
        public int mElementSize { get; set; }

        /// <summary>
        /// Инициализация пустого сообщения
        /// </summary>
        public MessageData()
        {
            mText = string.Empty;
            mDataType = (MessageDataType)0;
            mElementName = string.Empty;
            mElementThumbImageURL = string.Empty;
            mElementURL = string.Empty;
            mElementOwnerID = 0;
            mElementID = 0;
            mElementSize = 0;
        }


    }

    /// <summary>
    /// Информация об отправителе или получателе сообщения
    /// </summary>
    public class MessageUser
    {
        /// <summary>
        /// ID пользователя
        /// </summary>
        public int mUserID { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string mUserName { get; set; }

        /// <summary>
        /// URL c фотографией пользователя (ширина 100 px)
        /// </summary>
        public string mUserPhotoURL { get; set; }

        /// <summary>
        /// Название файла для уменьшенной фотографии (50 px)
        /// </summary>
        public string mSmallUserPhotoName { get; set; }

        /// <summary>
        /// Пол пользователя
        /// 1 = женский
        /// 2 = мужской
        /// </summary>
        public int mUserSex { get; set; }

        /// <summary>
        /// Online статус пользователя
        /// </summary>
        public int mUserIsOnline { get; set; }

        /// <summary>
        /// Инициализировать нового пользователя
        /// </summary>
        public MessageUser()
        {
            mUserID = 0;

            mUserName = string.Empty;

            mUserPhotoURL = string.Empty;

            mSmallUserPhotoName = string.Empty;

            mUserSex = 0;

            mUserIsOnline = 0;
        }



    }

    /// <summary>
    /// Конверт сообщения
    /// </summary>
    public class MessageCover
    {
        /// <summary>
        /// ID сообщения
        /// </summary>
        public int mID { get; set; }

        /// <summary>
        /// Время отправки сообщения 
        /// </summary>
        public DateTime mTime { get; set; }

        /// <summary>
        /// Собщение
        /// </summary>
        public MessageData mData;

        /// <summary>
        /// Отправитель
        /// </summary>
        public MessageUser mMesSender;

        /// <summary>
        /// Получатель
        /// </summary>
        public MessageUser mMesReceiver;

        /// <summary>
        /// Прочитано сообщение или нет
        /// </summary>
        public bool mMesIsRead { get; set; }

        /// <summary>
        /// Входящее сообщение или исходящее
        /// </summary>
        public MessageIOType mIOType { get; set; }

        /// <summary>
        /// Инициализировать новый конверт сообщения
        /// </summary>
        public MessageCover()
        {
            mID = 0;
            mTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            mMesIsRead = true;
            mIOType = MessageIOType.Inbox;
            mMesSender = new MessageUser();
            mMesReceiver = new MessageUser();
            mData = new MessageData();
            mMesIsRead = true;

        }

    }

    /// <summary>
    /// Контейнер сообщений
    /// </summary>
    public class MessResponse
    {
        /// <summary>
        /// Общее количество сообщений
        /// </summary>
        public int mCount { get; set; }

        /// <summary>
        /// Конверты сообщений
        /// </summary>
        public List<MessageCover> mMessages = new List<MessageCover>();

        /// <summary>
        /// Номер версии 
        /// </summary>
        public int VersionNum { get; set; }

        /// <summary>
        /// Инициализировать новый список сообщений
        /// </summary>
        public MessResponse()
        {
            mCount = 0;
        }

    }

    /// <summary>
    /// Переписка с конкретным пользователем
    /// </summary>
    public class MessUserCorrespondence
    {
        /// <summary>
        /// ПОльзователь, с которым велась переписка
        /// </summary>
        public MessageUser mMesUser;

        /// <summary>
        /// Номер версии 
        /// </summary>
        public int VersionNum { get; set; }

        /// <summary>
        /// Есть ли данная копия в кэше
        /// </summary>
        public bool FromCache { get; set; }

        /// <summary>
        /// Конверты сообщений
        /// </summary>
        public List<MessageCover> mMessages = new List<MessageCover>();

        /// <summary>
        /// Инициализация переписки без параметров
        /// </summary>
        public MessUserCorrespondence()
        {
            mMesUser = new MessageUser();
            VersionNum = 0;
        }
        /// <summary>
        /// Инициализация переписки с существующим объектом для пользователя
        /// </summary>
        public MessUserCorrespondence(MessageUser usr)
        {
            mMesUser = usr;
            VersionNum = 0;
        }

        /// <summary>
        /// Проверка наличия сообщения в переписке
        /// </summary>
        /// <param name="MessID">ID сообщения</param>
        /// <returns>Есть сообщение (true), или нет(false)</returns> 
        public bool MessageIsInList(int messID)
        {
            foreach (MessageCover mCover in mMessages)
                if (mCover.mID == messID) return true;
            return false;
        }
        /// <summary>
        /// Поиск сообщения в переписке
        /// </summary>
        /// <param name="MessID">ID сообщения</param>
        /// <returns>Найденное сообщение</returns> 
        public MessageCover SeekMessage(int MessID)
        {
            foreach (MessageCover mCover in mMessages)
                if (mCover.mID == MessID) return mCover;
            return null;
        }
        /// <summary>
        /// Удаление сообщения из переписки
        /// </summary>
        /// <param name="MessID">ID сообщения</param>
        /// <returns>Успешность удаления</returns>  
        public bool Delete(int MessID)
        {

            MessageCover m = null;
            foreach (MessageCover iter in mMessages)
            {
                if (iter.mID == MessID)
                    m = iter;

            }
            return mMessages.Remove(m);
        }
    }

    /// <summary>
    /// Контейнер переписок
    /// </summary>
    public class MessCorrespondence : ISelfDeserializer
    {
        private string _folderName;
        private string _dataName;
        private List<string> _files;

        /// <summary>
        /// Переписки с пользователями
        /// </summary>
        public List<MessUserCorrespondence> mUserCorrespondences = new List<MessUserCorrespondence>();

        /// <summary>
        /// Инициализация контейнера переписок
        /// </summary>
        public MessCorrespondence()
        {
        }

        /// <summary>
        /// Проверка наличия переписки в контейнере для заданного пользователя
        /// </summary>
        /// <param name="UserID">ID пользователя</param>
        /// <returns>Есть переписка (true), или нет(false)</returns> 
        public bool UserIsInList(int UserID)
        {
            foreach (MessUserCorrespondence UserCorr in mUserCorrespondences)
            {
                if (UserCorr.mMesUser.mUserID == UserID)
                {
                    return true;
                }
            }

            if (!string.IsNullOrEmpty(_folderName) || !string.IsNullOrEmpty(_dataName))
            {
                if (_files != null && _files.Contains(UserID.ToString()))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Поиск переписки в контейнере для заданного пользователя
        /// </summary>
        /// <param name="UserID">ID пользователя</param>
        /// <param name="FoundCorrespondence">Найденная переписка</param>
        public void SeekCorrespondence(int UserID, out MessUserCorrespondence FoundCorrespondence)
        {
            FoundCorrespondence = null;

            foreach (MessUserCorrespondence UserCorr in mUserCorrespondences)
            {
                if (UserCorr.mMesUser.mUserID == UserID)
                {
                    FoundCorrespondence = UserCorr;

                    break;
                }
            }

            if (FoundCorrespondence == null)
            {
                if (!string.IsNullOrEmpty(_folderName) || !string.IsNullOrEmpty(_dataName))
                {
                    string folder = (string.IsNullOrEmpty(_folderName) ? "" : "\\") + _dataName;
                    if (_files != null && _files.Contains(UserID.ToString()))
                    {
                        MessUserCorrespondence corr = Cache.Cache.LoadFromCache<MessUserCorrespondence>(folder, UserID.ToString());
                        corr.FromCache = true;
                        mUserCorrespondences.Add(corr);
                        FoundCorrespondence = corr;
                    }
                }
            }
        }

        void ISelfDeserializer.Deserialize(string folderName, string dataName)
        {
            _folderName = folderName;
            _dataName = dataName;
            string folder = (string.IsNullOrEmpty(_folderName) ? "" : "\\") + _dataName;
            _files = new List<string>(Cache.Cache.GetEntryNames(folder));
        }

        void ISelfDeserializer.Serialize(string folderName, string dataName)
        {
            string folder = (string.IsNullOrEmpty(folderName) ? "" : "\\") + dataName;
            bool getFiles = false;
            foreach (MessUserCorrespondence messUserCorrespondence in mUserCorrespondences)
            {
                string dataname = messUserCorrespondence.mMesUser.mUserID.ToString();
                if (!messUserCorrespondence.FromCache)
                {
                    Cache.Cache.SaveToCache(messUserCorrespondence, folder, dataname);
                    messUserCorrespondence.FromCache = true;
                    getFiles = true;
                }
            }
            if (getFiles)
                _files = new List<string>(Cache.Cache.GetEntryNames(folder));
        }
    }

    /// <summary>
    /// "Короткое" сообщение
    /// </summary>
    public class MessShort
    {
        public int mID { get; set; }

        /// <summary>
        /// URL фотографии пользователя
        /// </summary>
        public string mUserPhotoURL { get; set; }
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string mUserName { get; set; }
        /// <summary>
        /// ID пользователя
        /// </summary>
        public int mUserID { get; set; }
        /// <summary>
        /// Текст последнего сообщения
        /// </summary>        
        public string mLastMessageText { get; set; }
        /// <summary>
        /// Время отправки сообщения 
        /// </summary>
        public DateTime mTime { get; set; }
        /// <summary>
        /// Входящее сообщение или исходящее
        /// </summary>
        public MessageIOType mType { get; set; }
        /// <summary>
        /// Прочитано сообщение или нет
        /// </summary>
        public bool mIsRead { get; set; }
        
        /// <summary>
        /// Инициализация "короткого" сообщения
        /// </summary>
        public MessShort()
        {
            mID = 0;
            mTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            mUserPhotoURL = string.Empty;
            mUserName = string.Empty;
            mLastMessageText = string.Empty;
            mType = MessageIOType.Inbox;
            mIsRead = true;

        }
    }


    /// <summary>
    /// Класс для работы с "короткими" сообщениями
    /// </summary>
    public class MessShortCorrespondence
    {
        /// <summary>
        /// Список "коротких" сообщений
        /// </summary>
        public List<MessShort> MessList = new List<MessShort>();

        /// <summary>
        /// Возвращает "короткое" сообщение, ассоциированное с пользователем
        /// </summary>
        /// <param name="usr">ID пользователя</param>
        /// <returns>"Короткое" сообщение</returns>
        public MessShort UserIsInList(MessageUser usr)
        {           
            foreach (MessShort newMessShort in MessList)
            {
                if (newMessShort.mUserID == usr.mUserID)
                {
                    return newMessShort;
                }
            }

            return null;            
        }
    }

    /// <summary>
    /// Класс, хранящий изменение в сообщении
    /// </summary>
    public class MessChange
    {
        /// <summary>
        /// Тип изменения 
        /// </summary>
        public string ActionType { get; set; }

        /// <summary>
        /// Конверт сообщения 
        /// </summary>
        public MessageCover Message;

        /// <summary>
        /// Номер версии 
        /// </summary>
        public int VersionNum { get; set; }

        /// <summary>
        /// Инициализация с конвертом сообщения
        /// </summary>
        public MessChange(MessageCover InMessage)
        {
            Message = InMessage;
        }
        /// <summary>
        /// Инициализация без параметров
        /// </summary>
        public MessChange()
        {
            Message = new MessageCover();
        }

    }

    /// <summary>
    /// Контейнер изменений в сообщениях
    /// </summary>
    public class MessChangesResponse
    {
        /// <summary>
        /// Список изменений
        /// </summary>
        public List<MessChange> MessList = new List<MessChange>();


    }
}
