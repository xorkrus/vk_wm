using System;

using System.Collections.Generic;
using System.Text;

namespace Galssoft.VKontakteWM.Components.Common.ResponseClasses
{
    /// <summary>
    /// Список обновлений статуса
    /// </summary>
    public class ShortActivityResponse
    {
        /// <summary>
        /// ID статуса
        /// </summary>
        public List<int> sadStatusID = new List<int>();
    }

    /// <summary>
    /// Обновление
    /// </summary>
    public class ActivityData
    {
        /// <summary>
        /// ID статуса
        /// </summary>
        public int adStatusID { get; set; }

        /// <summary>
        /// Текст обновления
        /// </summary>
        public string adText { get; set; }
        
        /// <summary>
        /// Дата обновления статуса (unixtime)
        /// </summary>
        public DateTime adTime { get; set; }

        /// <summary>
        /// Пользователь обновивший статус
        /// </summary>
        public PostSender adDataSender;

        /// <summary>
        /// Инициализировать пустое обновление
        /// </summary>
        public ActivityData()
        {
            adStatusID = 0;

            adText = string.Empty;

            adTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        }
    }

    /// <summary>
    /// Список обновлений статуса
    /// </summary>
    public class ActivityResponse
    {
        /// <summary>
        /// Общее количество обновлений
        /// </summary>
        public int arPostCount { get; set; }

        /// <summary>
        /// Обновления
        /// </summary>
        public List<ActivityData> arActivityDatas = new List<ActivityData>();

        /// <summary>
        /// Текущая версия обновлений (Timestamp)
        /// </summary>
        public int arTimeStamp { get; set; }

        /// <summary>
        /// Инициализировать новый список обновлений статуса
        /// </summary>
        public ActivityResponse()
        {
            arPostCount = 0;

            arTimeStamp = 0;
        }
    }
}
