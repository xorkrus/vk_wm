using System;

using System.Collections.Generic;
using System.Text;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    public class NewsItems
    {
        public List<StatusUpdatesListViewItem> Statuses; // { get; set; }
        public List<PhotosUpdatesListViewItem> Photos; // { get; set; }    

        public NewsItems()
        {
            Statuses = new List<StatusUpdatesListViewItem>();
            Photos = new List<PhotosUpdatesListViewItem>();
        }
    }

    //public enum StatusUpdatesListViewItemType
    //{
    //    Activity = 0,
    //    Photo = 1
    //}

    public class StatusUpdatesListViewItem : IComparable<StatusUpdatesListViewItem>
    {
        //public StatusUpdatesListViewItemType ItemType { get; set; }

        // Activity
        public string Uid { get; set; } // это ID статуса

        public string UserID { get; set; } // это ID пользователя

        //public string UserPhoto { get; set; }

        //public bool IsUserPhotoLoaded { get; set; }

        public string UserName { get; set; }

        public string UserStatus { get; set; }

        public string Group { get; set; }

        public DateTime StatusSetDate { get; set; }

        //// Photo
        ////public string Uid { get; set; }

        //public string Photo { get; set; }

        //public bool IsPhotoLoaded { get; set; }


        //// компаратор
        //public int CompareTo(StatusUpdatesListViewItem newStatusUpdateListViewItem)
        //{
        //    int val = 0;

        //    switch (ItemType)
        //    {
        //        case StatusUpdatesListViewItemType.Activity:
        //            val = -StatusSetDate.CompareTo(newStatusUpdateListViewItem.StatusSetDate);                    
        //            break;

        //        case StatusUpdatesListViewItemType.Photo:
        //            val = -Uid.CompareTo(newStatusUpdateListViewItem.Uid);
        //            break;                
        //    }            

        //    return val;
        //}



        public int CompareTo(StatusUpdatesListViewItem newStatusUpdateListViewItem)
        {
            return -StatusSetDate.CompareTo(newStatusUpdateListViewItem.StatusSetDate);       
        }
    }

    public class PhotosUpdatesListViewItem : IComparable<PhotosUpdatesListViewItem>
    {
        public string Uid { get; set; } // это ID фото

        public string UserID { get; set; } // это ID пользователя

        public string Photo { get; set; }

        public string LargePhotoURL { get; set; }

        public bool IsPhotoLoaded { get; set; }

        public int CompareTo(PhotosUpdatesListViewItem newPhotosUpdatesListViewItem)
        {
            return -Uid.CompareTo(newPhotosUpdatesListViewItem.Uid);
        }
    }

}
