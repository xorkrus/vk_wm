using System;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    public class FriendListViewItem : IComparable<FriendListViewItem>
    {
        public string Uid { get; set; }

        public string Group { get; set; }

        public string Avatar { get; set; }

        public bool IsAvatarLoaded { get; set; }

        //public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsOnline { get; set; }

        public string Telephone { get; set; }

        public DateTime Birthday { get; set; }

        public int CompareTo(FriendListViewItem newFriendListViewItem)
        {
            return LastName.CompareTo(newFriendListViewItem.LastName);
        }
    }
}
