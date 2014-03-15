using System;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    public class WallPostListViewItem : IComparable<WallPostListViewItem>
    {
        public string Avatar { get; set; }

        public bool IsAvatarLoaded { get; set; }

        public string UserName { get; set; }

        public string Status { get; set; }

        public string StatusChangeDate { get; set; }

        public int CompareTo(WallPostListViewItem obj)
        {
            return UserName.CompareTo(obj.UserName);
        }
    }
}
