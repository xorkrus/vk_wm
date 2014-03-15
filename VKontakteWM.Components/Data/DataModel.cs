using System.IO;
using Galssoft.VKontakteWM.Components.Common.ResponseClasses;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;

namespace Galssoft.VKontakteWM.Components.Data
{
    /// <summary>
    ///Синглтон для оперативного доступа к данным мимо дискового кэша
    /// </summary>
    public class DataModel
    {
        private static DataModel _data;

        public static DataModel Data
        {
            get
            {
                if (_data == null)
                {
                    _data = new DataModel();
                }

                return _data;
            }
        }        

        private MessCorrespondence _messageCorrespondence;

        private MessShortCorrespondence _messageShortCorrespondence;

        private PhotosCommentsResponseHistory _photosCommentsResponseHistory;

        private FriendsListResponse _friendsListResponse;

        private ActivityResponse _activityResponse;

        private UpdatesPhotosResponse _updatesPhotosResponse;

        private PhotosCommentsResponse _photosCommentsResponse;

        private UpdatesPhotosResponse _userPhotosResponse;

        private FriendsListResponse _friendsListAdditionalResponse;

        private ActivityResponse _userActivityResponse;

        private DraftMessagesData _draftMessages;

        /// <summary>
        /// Хранит все цепочки сообщений
        /// </summary>
        public MessCorrespondence MessageCorrespondence
        {
            get
            {
                if (_messageCorrespondence != null) return _messageCorrespondence;

                try
                {
                    _messageCorrespondence = Cache.Cache.LoadFromCache<MessCorrespondence>(string.Empty, "MessageCorrespondence");
                    return _messageCorrespondence;
                }
                catch (IOException newException)
                {
                    DebugHelper.WriteLogEntry("Ошибка сохранения данных MessageCorrespondence в кэш: " + newException.Message);
                    return null;
                }
            }

            set 
            {
                _messageCorrespondence = value;

                try
                {
                    bool result;

                    if (value == null)
                        result = Cache.Cache.DeleteEntryFromCache(string.Empty, "MessageCorrespondence");
                    else
                        result = Cache.Cache.SaveToCache(value, string.Empty, "MessageCorrespondence");

                    if (result)
                    {
                        DebugHelper.WriteLogEntry("MessageCorrespondence сохранен/удален.");
                    }
                    else
                    {
                        DebugHelper.WriteLogEntry("MessageCorrespondence не сохранен/удален.");
                    }
                }
                catch (IOException newException)
                {
                    DebugHelper.WriteLogEntry("Ошибка сохранения/удаления MessageCorrespondence в кэше: " + newException.Message);
                }
            }
        }
       
        public PhotosCommentsResponseHistory PhotosCommentsResponseHistoryData
        {
            get
            {
                if (_photosCommentsResponseHistory != null)
                {
                    return _photosCommentsResponseHistory;
                }

                try
                {
                    _photosCommentsResponseHistory = Cache.Cache.LoadFromCache<PhotosCommentsResponseHistory>(string.Empty, "PhotosCommentsResponseHistory");

                    return _photosCommentsResponseHistory;
                }
                catch (IOException newException)
                {
                    DebugHelper.WriteLogEntry("Ошибка сохранения данных PhotosCommentsResponseHistory в кэш: " + newException.Message);

                    return null;
                }
            }

            set
            {
                _photosCommentsResponseHistory = value;

                try
                {
                    bool result;

                    if (value == null)
                    {
                        result = Cache.Cache.DeleteEntryFromCache(string.Empty, "PhotosCommentsResponseHistory");
                    }
                    else
                    {
                        result = Cache.Cache.SaveToCache(value, string.Empty, "PhotosCommentsResponseHistory");
                    }

                    if (result)
                    {
                        DebugHelper.WriteLogEntry("PhotosCommentsResponseHistory сохранен/удален.");
                    }
                    else
                    {
                        DebugHelper.WriteLogEntry("PhotosCommentsResponseHistory не сохранен/удален.");
                    }
                }
                catch (IOException newException)
                {
                    DebugHelper.WriteLogEntry("Ошибка сохранения/удаления PhotosCommentsResponseHistory в кэше: " + newException.Message);
                }
            }
        }

        /// <summary>
        /// Хранит сообщения, необходимые для формирования списка цепочек сообщений
        /// </summary>
        public MessShortCorrespondence MessageShortCorrespondence
        {
            get
            {
                if (_messageShortCorrespondence != null) return _messageShortCorrespondence;

                try
                {
                    _messageShortCorrespondence = Cache.Cache.LoadFromCache<MessShortCorrespondence>(string.Empty, "MessageShortCorrespondence");
                    return _messageShortCorrespondence;
                }
                catch (IOException newException)
                {
                    DebugHelper.WriteLogEntry("Ошибка сохранения данных MessageShortCorrespondence в кэш: " + newException.Message);
                    return null;
                }
            }

            set 
            {
                _messageShortCorrespondence = value;

                try
                {
                    bool result;

                    if (value == null)
                        result = Cache.Cache.DeleteEntryFromCache(string.Empty, "MessageShortCorrespondence");
                    else
                        result = Cache.Cache.SaveToCache(value, string.Empty, "MessageShortCorrespondence");

                    if (result)
                    {
                        DebugHelper.WriteLogEntry("MessageShortCorrespondence сохранен/удален.");
                    }
                    else
                    {
                        DebugHelper.WriteLogEntry("MessageShortCorrespondence не сохранен/удален.");
                    }
                }
                catch (IOException newException)
                {
                    DebugHelper.WriteLogEntry("Ошибка сохранения/удаления MessageShortCorrespondence в кэше: " + newException.Message);
                }
            }
        }

        public FriendsListResponse FriendsListResponseData
        {
            get
            {
                if (_friendsListResponse != null)
                {
                    return _friendsListResponse;
                }

                try
                {
                    _friendsListResponse = Cache.Cache.LoadFromCache<FriendsListResponse>(string.Empty, "FriendsListResponse");

                    return _friendsListResponse;
                }
                catch (IOException newException)
                {
                    DebugHelper.WriteLogEntry("Ошибка сохранения данных FriendsListResponse в кэш: " + newException.Message);

                    return null;
                }
            }

            set
            {
                _friendsListResponse = value;

                try
                {
                    bool result;

                    if (value == null)
                    {
                        result = Cache.Cache.DeleteEntryFromCache(string.Empty, "FriendsListResponse");
                    }
                    else
                    {
                        result = Cache.Cache.SaveToCache(value, string.Empty, "FriendsListResponse");
                    }

                    if (result)
                    {
                        DebugHelper.WriteLogEntry("FriendsListResponse сохранен/удален.");
                    }
                    else
                    {
                        DebugHelper.WriteLogEntry("FriendsListResponse не сохранен/удален.");
                    }
                }
                catch (IOException newException)
                {
                    DebugHelper.WriteLogEntry("Ошибка сохранения/удаления FriendsListResponse в кэше: " + newException.Message);
                }
            }
        }

        public ActivityResponse ActivityResponseData
        {
            get
            {
                if (_activityResponse != null)
                {
                    return _activityResponse;
                }

                try
                {
                    _activityResponse = Cache.Cache.LoadFromCache<ActivityResponse>(string.Empty, "ActivityResponse");

                    return _activityResponse;
                }
                catch (IOException newException)
                {
                    DebugHelper.WriteLogEntry("Ошибка сохранения данных ActivityResponse в кэш: " + newException.Message);

                    return null;
                }
            }

            set
            {
                _activityResponse = value;

                try
                {
                    bool result;

                    if (value == null)
                    {
                        result = Cache.Cache.DeleteEntryFromCache(string.Empty, "ActivityResponse");
                    }
                    else
                    {
                        result = Cache.Cache.SaveToCache(value, string.Empty, "ActivityResponse");
                    }

                    if (result)
                    {
                        DebugHelper.WriteLogEntry("ActivityResponse сохранен/удален.");
                    }
                    else
                    {
                        DebugHelper.WriteLogEntry("ActivityResponse не сохранен/удален.");
                    }
                }
                catch (IOException newException)
                {
                    DebugHelper.WriteLogEntry("Ошибка сохранения/удаления ActivityResponse в кэше: " + newException.Message);
                }
            }
        }

        public UpdatesPhotosResponse UpdatesPhotosResponseData
        {
            get
            {
                if (_updatesPhotosResponse != null)
                {
                    return _updatesPhotosResponse;
                }

                try
                {
                    _updatesPhotosResponse = Cache.Cache.LoadFromCache<UpdatesPhotosResponse>(string.Empty, "UpdatesPhotosResponse");

                    return _updatesPhotosResponse;
                }
                catch (IOException newException)
                {
                    DebugHelper.WriteLogEntry("Ошибка сохранения данных UpdatesPhotosResponse в кэш: " + newException.Message);

                    return null;
                }
            }

            set
            {
                _updatesPhotosResponse = value;

                try
                {
                    bool result;

                    if (value == null)
                    {
                        result = Cache.Cache.DeleteEntryFromCache(string.Empty, "UpdatesPhotosResponse");
                    }
                    else
                    {
                        result = Cache.Cache.SaveToCache(value, string.Empty, "UpdatesPhotosResponse");
                    }

                    if (result)
                    {
                        DebugHelper.WriteLogEntry("UpdatesPhotosResponse сохранен/удален.");
                    }
                    else
                    {
                        DebugHelper.WriteLogEntry("UpdatesPhotosResponse не сохранен/удален.");
                    }
                }
                catch (IOException newException)
                {
                    DebugHelper.WriteLogEntry("Ошибка сохранения/удаления UpdatesPhotosResponse в кэше: " + newException.Message);
                }
            }
        }       

        public PhotosCommentsResponse PhotosCommentsResponseData
        {
            get
            {
                if (_photosCommentsResponse != null)
                {
                    return _photosCommentsResponse;
                }

                try
                {
                    _photosCommentsResponse = Cache.Cache.LoadFromCache<PhotosCommentsResponse>(string.Empty, "PhotosCommentsResponse");

                    return _photosCommentsResponse;
                }
                catch (IOException newException)
                {
                    DebugHelper.WriteLogEntry("Ошибка сохранения данных PhotosCommentsResponse в кэш: " + newException.Message);

                    return null;
                }
            }

            set
            {
                _photosCommentsResponse = value;

                try
                {
                    bool result;

                    if (value == null)
                    {
                        result = Cache.Cache.DeleteEntryFromCache(string.Empty, "PhotosCommentsResponse");
                    }
                    else
                    {
                        result = Cache.Cache.SaveToCache(value, string.Empty, "PhotosCommentsResponse");
                    }

                    if (result)
                    {
                        DebugHelper.WriteLogEntry("PhotosCommentsResponse сохранен/удален.");
                    }
                    else
                    {
                        DebugHelper.WriteLogEntry("PhotosCommentsResponse не сохранен/удален.");
                    }
                }
                catch (IOException newException)
                {
                    DebugHelper.WriteLogEntry("Ошибка сохранения/удаления PhotosCommentsResponse в кэше: " + newException.Message);
                }
            }
        }

        public UpdatesPhotosResponse UserPhotosResponseData
        {
            get
            {
                if (_userPhotosResponse != null)
                {
                    return _userPhotosResponse;
                }

                try
                {
                    _userPhotosResponse = Cache.Cache.LoadFromCache<UpdatesPhotosResponse>(string.Empty, "UserPhotosResponse");

                    return _userPhotosResponse;
                }
                catch (IOException newException)
                {
                    DebugHelper.WriteLogEntry("Ошибка сохранения данных PhotosCommentsResponse в кэш: " + newException.Message);

                    return null;
                }
            }

            set
            {
                _userPhotosResponse = value;

                try
                {
                    bool result;

                    if (value == null)
                    {
                        result = Cache.Cache.DeleteEntryFromCache(string.Empty, "UserPhotosResponse");
                    }
                    else
                    {
                        result = Cache.Cache.SaveToCache(value, string.Empty, "UserPhotosResponse");
                    }

                    if (result)
                    {
                        DebugHelper.WriteLogEntry("UserPhotosResponse сохранен/удален.");
                    }
                    else
                    {
                        DebugHelper.WriteLogEntry("UserPhotosResponse не сохранен/удален.");
                    }
                }
                catch (IOException newException)
                {
                    DebugHelper.WriteLogEntry("Ошибка сохранения/удаления UserPhotosResponse в кэше: " + newException.Message);
                }
            }
        }

        public FriendsListResponse FriendsListAdditionalResponseData
        {
            get
            {
                if (_friendsListAdditionalResponse != null)
                {
                    return _friendsListAdditionalResponse;
                }

                try
                {
                    _friendsListAdditionalResponse = Cache.Cache.LoadFromCache<FriendsListResponse>(string.Empty, "FriendsListAdditionalResponse");

                    return _friendsListAdditionalResponse;
                }
                catch (IOException newException)
                {
                    DebugHelper.WriteLogEntry("Ошибка сохранения данных FriendsListAdditionalResponse в кэш: " + newException.Message);

                    return null;
                }
            }

            set
            {
                _friendsListAdditionalResponse = value;

                try
                {
                    bool result;

                    if (value == null)
                    {
                        result = Cache.Cache.DeleteEntryFromCache(string.Empty, "FriendsListAdditionalResponse");
                    }
                    else
                    {
                        result = Cache.Cache.SaveToCache(value, string.Empty, "FriendsListAdditionalResponse");
                    }

                    if (result)
                    {
                        DebugHelper.WriteLogEntry("FriendsListAdditionalResponse сохранен/удален.");
                    }
                    else
                    {
                        DebugHelper.WriteLogEntry("FriendsListAdditionalResponse не сохранен/удален.");
                    }
                }
                catch (IOException newException)
                {
                    DebugHelper.WriteLogEntry("Ошибка сохранения/удаления FriendsListAdditionalResponse в кэше: " + newException.Message);
                }
            }
        }

        public ActivityResponse UserActivityResponseData
        {
            get
            {
                if (_userActivityResponse != null)
                {
                    return _userActivityResponse;
                }

                try
                {
                    _userActivityResponse = Cache.Cache.LoadFromCache<ActivityResponse>(string.Empty, "UserActivityResponse");

                    return _userActivityResponse;
                }
                catch (IOException newException)
                {
                    DebugHelper.WriteLogEntry("Ошибка сохранения данных UserActivityResponse в кэш: " + newException.Message);

                    return null;
                }
            }

            set
            {
                _userActivityResponse = value;

                try
                {
                    bool result;

                    if (value == null)
                    {
                        result = Cache.Cache.DeleteEntryFromCache(string.Empty, "UserActivityResponse");
                    }
                    else
                    {
                        result = Cache.Cache.SaveToCache(value, string.Empty, "UserActivityResponse");
                    }

                    if (result)
                    {
                        DebugHelper.WriteLogEntry("UserActivityResponse сохранен/удален.");
                    }
                    else
                    {
                        DebugHelper.WriteLogEntry("UserActivityResponse не сохранен/удален.");
                    }
                }
                catch (IOException newException)
                {
                    DebugHelper.WriteLogEntry("Ошибка сохранения/удаления UserActivityResponse в кэше: " + newException.Message);
                }
            }
        }

        //private DraftMessagesData _draftMessages;

        //Cache.Cache.DeleteEntryFromCache(string.Empty, "DraftMessagesData");

        public DraftMessagesData DraftMessagesData
        {
            get
            {
                if (_draftMessages != null)
                {
                    return _draftMessages;
                }

                try
                {
                    _draftMessages = Cache.Cache.LoadFromCache<DraftMessagesData>(string.Empty, "DraftMessagesData");

                    return _draftMessages;
                }
                catch (IOException newException)
                {
                    DebugHelper.WriteLogEntry("Ошибка сохранения данных DraftMessagesData в кэш: " + newException.Message);

                    return null;
                }
            }

            set
            {
                _draftMessages = value;

                try
                {
                    bool result;

                    if (value == null)
                    {
                        result = Cache.Cache.DeleteEntryFromCache(string.Empty, "DraftMessagesData");
                    }
                    else
                    {
                        result = Cache.Cache.SaveToCache(value, string.Empty, "DraftMessagesData");
                    }

                    if (result)
                    {
                        DebugHelper.WriteLogEntry("DraftMessagesData сохранен/удален.");
                    }
                    else
                    {
                        DebugHelper.WriteLogEntry("DraftMessagesData не сохранен/удален.");
                    }
                }
                catch (IOException newException)
                {
                    DebugHelper.WriteLogEntry("Ошибка сохранения/удаления DraftMessagesData в кэше: " + newException.Message);
                }
            }
        }
    }
}
