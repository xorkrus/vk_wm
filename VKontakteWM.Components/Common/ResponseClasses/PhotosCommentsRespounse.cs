using System;

using System.Collections.Generic;
using System.Text;

namespace Galssoft.VKontakteWM.Components.Common.ResponseClasses
{
    /// <summary>
    /// Комментарии к фотографиям (сокращенный вариант)
    /// </summary>
    public class ShortPhotosCommentsRespounse
    {
        /// <summary>
        /// ID сообщения
        /// </summary>
        public List<int> spcrCommentIDs = new List<int>();
    }

    ///// <summary>
    ///// Информация о фотографии
    ///// </summary>
    //public class PhotoData
    //{
    //    public int pdID { get; set; }

    //    public string pdURL { get; set; }

    //    public PhotoData()
    //    {
    //        pdID = 0;

    //        pdURL = string.Empty;
    //    }
    //}

    /// <summary>
    /// Конверт комментария
    /// </summary>
    public class CommentPost
    {
        /// <summary>
        /// ID сообщения
        /// </summary>
        public int cpID { get; set; }

        /// <summary>
        /// Время отправки сообщения (unixtime)
        /// </summary>
        public DateTime cpTime { get; set; }

        /// <summary>
        /// Сообщение
        /// </summary>
        public WallData cpWallData;

        /// <summary>
        /// Отправитель
        /// </summary>
        public PostSender cpPostSender;

        /// <summary>
        /// Получатель
        /// </summary>
        public PostReceiver cpPostReceiver;

        /// <summary>
        /// массив с информацией о фотографии
        /// </summary>
        public PhotoData cpPhotoData;     

        /// <summary>
        /// Инициализировать новый конверт сообщения
        /// </summary>
        public CommentPost()
        {
            cpID = 0;

            cpTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);  
        }
    }

    /// <summary>
    /// Комментарии к фотографиям
    /// </summary>
    public class PhotosCommentsResponse
    {
        /// <summary>
        /// Общее количество комментариев
        /// </summary>
        public int pcrCount { get; set; }

        /// <summary>
        /// Конверты комментариев
        /// </summary>
        public List<CommentPost> pcrComments = new List<CommentPost>();

        /// <summary>
        /// Текущая версия списка комментариев (TimeStamp)
        /// </summary>
        public int pcrTimeStamp { get; set; }

        /// <summary>
        /// PhotoID (используется только в PhotosCommentsResponseHistory)
        /// </summary>
        public int pcrPhotoID { get; set; }

        /// <summary>
        /// Автор фотографии
        /// </summary>
        public User pcrAuthor;

        /// <summary>
        /// Инициализировать новый список комментариев к фотографиям
        /// </summary>
        public PhotosCommentsResponse()
        {
            pcrCount = 0;

            pcrTimeStamp = 0;

            pcrPhotoID = 0;
        }
    }

    /// <summary>
    /// Набор PhotosCommentsResponse + PhotoID
    /// </summary>
    public class PhotosCommentsResponseHistory
    {
        public List<PhotosCommentsResponse> pcrhPhotosCommentsData = new List<PhotosCommentsResponse>();

        public PhotosCommentsResponse GetItem(int photoID)
        {
            try
            {
                foreach (var val in pcrhPhotosCommentsData)
                {
                    if (val.pcrPhotoID == photoID)
                    {
                        return val;
                    }
                }

                return null;
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        public void DelItem(int photoID)
        {
            try
            {
                foreach (var val in pcrhPhotosCommentsData)
                {
                    if (val.pcrPhotoID == photoID)
                    {
                        pcrhPhotosCommentsData.Remove(val);

                        return;
                    }
                }
            }
            catch (NullReferenceException)
            {
                return;
            }
        }

        public void AddItem(PhotosCommentsResponse val)
        {
            try
            {
                pcrhPhotosCommentsData.Add(val);
            }
            catch (NullReferenceException)
            {
                return;
            }
        }

        //public bool IsItemInList(int photoID)
        //{
        //    foreach (var val in pcrhPhotosCommentsData)
        //    {

        //    }

        //    return false;
        //}
    }
}
