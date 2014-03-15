using System;

using System.Collections.Generic;
using System.Text;

namespace Galssoft.VKontakteWM.Components.Common.ResponseClasses
{
    /// <summary>
    /// Список сообщений (сокращение)
    /// </summary>
    public class ShortWallResponse
    {
        /// <summary>
        /// ID сообщения
        /// </summary>
        public List<int> swrMessageID = new List<int>();
    }

    /// <summary>
    /// Типы сообщений
    /// </summary>
    public enum WallDataType
    {
        PlainText = 0,
        Photo = 1,
        Graffiti = 2,
        Video = 3,
        Audio = 4
    }

    /// <summary>
    /// Сообщение
    /// </summary> 
    public class WallData
    {
        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string wdText { get; set; }

        /// <summary>
        /// Тип сообщения
        /// </summary>
        public WallDataType wdWallDataType { get; set; }

        /// <summary>
        /// Название элемента
        /// </summary>
        public string wdElementName { get; set; }

        /// <summary>
        /// URL уменьшенного изображения для элемента
        /// </summary>
        public string wdElementThumbImageURL { get; set; }

        /// <summary>
        /// URL исходного элемента
        /// </summary>
        public string wdElementURL { get; set; }

        /// <summary>
        /// ID владельца элемента
        /// </summary>
        public int wdElementOwnerID { get; set; }

        /// <summary>
        /// ID элемента
        /// </summary>
        public int wdElementID { get; set; }

        /// <summary>
        /// Hазмер элемента
        /// </summary>
        public int wdElementSize { get; set; }

        /// <summary>
        /// Инициализировать пустое сообщения
        /// </summary>
        public WallData()
        {
            wdText = string.Empty;

            wdWallDataType = (WallDataType)0;

            wdElementName = string.Empty;

            wdElementThumbImageURL = string.Empty;

            wdElementURL = string.Empty;

            wdElementOwnerID = 0;

            wdElementID = 0;

            wdElementSize = 0;
        }
    }

    /// <summary>
    /// Информация об отправителе
    /// </summary>
    public class PostSender
    {
        /// <summary>
        /// ID пользователя
        /// </summary>
        public int psUserID { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string psUserName { get; set; }

        /// <summary>
        /// URL c фотографией пользователя (ширина 100 px)
        /// </summary>
        public string psUserPhotoURL { get; set; }

        /// <summary>
        /// Название файла для уменьшенной фотографии (50 px)
        /// </summary>
        public string psSmallUserPhotoName { get; set; }

        /// <summary>
        /// Пол пользователя
        /// 1 = женский
        /// 2 = мужской
        /// </summary>
        public int psUserSex { get; set; }

        /// <summary>
        /// Online статус пользователя
        /// </summary>
        public int psUserIsOnline { get; set; }

        /// <summary>
        /// Инициализировать нового отправителя
        /// </summary>
        public PostSender()
        {
            psUserID = 0;

            psUserName = string.Empty;

            psUserPhotoURL = string.Empty;

            psSmallUserPhotoName = string.Empty;

            psUserSex = 0;

            psUserIsOnline = 0;
        }
    }

    /// <summary>
    /// Информация о получателе
    /// </summary>
    public class PostReceiver
    {
        /// <summary>
        /// ID пользователя
        /// </summary>
        public int prUserID { get; set; }

        /// <summary>
        /// Инициализировать нового получателя
        /// </summary>
        public PostReceiver()
        {
            prUserID = 0;
        }
    }

    /// <summary>
    /// Конверт сообщения
    /// </summary>
    public class WallPost
    {
        /// <summary>
        /// ID сообщения
        /// </summary>
        public int wpID { get; set; }

        /// <summary>
        /// время отправки сообщения (unixtime)
        /// </summary>
        public int wpTime { get; set; }

        /// <summary>
        /// Собщение
        /// </summary>
        public WallData wpWallData;

        /// <summary>
        /// Отправитель
        /// </summary>
        public PostSender wpPostSender;

        /// <summary>
        /// Получатель
        /// </summary>
        public PostReceiver wpPostReceiver;

        /// <summary>
        /// Инициализировать новый конверт сообщения
        /// </summary>
        public WallPost()
        {
            wpID = 0;

            wpTime = 0;            
        }
    }

    /// <summary>
    /// Список сообщений
    /// </summary>
    public class WallResponse
    {
        /// <summary>
        /// количество сообщений на стене
        /// </summary>
        public int wrPostCount { get; set; }

        /// <summary>
        /// Конверты сообщений
        /// </summary>
        public List<WallPost> wrWallPosts = new List<WallPost>();

        /// <summary>
        /// Текущая версия стены (Timestamp)
        /// </summary>
        public int wrTimeStamp { get; set; }

        /// <summary>
        /// Инициализировать новый список сообщений
        /// </summary>
        public WallResponse()
        {
            wrPostCount = 0;

            wrTimeStamp = 0;
        }
    }
    
}
