using System;

using System.Collections.Generic;
using System.Text;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    public class PhotoCommentsUpdatesViewItem : IComparable<PhotoCommentsUpdatesViewItem>
    {
        public string Uid { get; set; } // ID - комментария

        public string UserID { get; set; } // ID - пользователя

        public string Photo { get; set; }

        public bool IsPhotoLoaded { get; set; }

        public string SenderName { get; set; }

        public string Comment { get; set; }

        public string Group { get; set; }

        public DateTime CommentSetDate { get; set; }

        public string LargePhotoURL { get; set; }

        public string PhotoID { get; set; }

        public int CompareTo(PhotoCommentsUpdatesViewItem newPhotoCommentsUpdatesViewItem)
        {
            return -CommentSetDate.CompareTo(newPhotoCommentsUpdatesViewItem.CommentSetDate);
        }
    }
}
