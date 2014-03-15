namespace Galssoft.VKontakteWM.Components.Common.ResponseClasses
{
    public class RawEventsGetResponse
    {
        public int MessagesCount { get; set; }

        public int CommentsCount { get; set; }

        public int FriendsCount { get; set; }

        public int WallCount { get; set; }

        public int FriendsPhotosCount { get; set; }

        public int FriendsNewsCount { get; set; }

        public RawEventsGetResponse()
        {
            MessagesCount = CommentsCount = FriendsCount = WallCount = FriendsPhotosCount = FriendsNewsCount = 0;
        }
    }

}
