using System.Drawing;
using Galssoft.VKontakteWM.Components.GDI;
using System;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    public enum ImageDetailedListViewItemType
    {
        Photo = 0,
        Author = 1,
        Comment = 2
    }

    public class ImageDetailedListViewItem
    {    
        public string Uid { get; set; } // ID комментария

        public string UserID { get; set; } // ID пользователя

        public string Photo { get; set; }

        public int PhotoHeight { get; set; }  

        public bool IsPhotoLoaded { get; set; }        

        public string UserName { get; set; }        

        public string UserComment { get; set; }

        public DateTime CommentSetDate { get; set; }

        public string CommentWroteDateString { get; set; }

        public string Group { get; set; }

        public ImageDetailedListViewItemType Type { get; set; }

        public int CompareTo(ImageDetailedListViewItem newImageDetailedListViewItem)
        {
            return -CommentSetDate.CompareTo(newImageDetailedListViewItem.CommentSetDate);
        }
    }
}
