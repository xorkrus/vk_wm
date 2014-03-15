using System;

using System.Collections.Generic;
using System.Text;
using Galssoft.VKontakteWM.Components.Common.ResponseClasses;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    public class MessagesListViewItem : IComparable<MessagesListViewItem>
    {
        public string Uid { get; set; }

        //public string UserPhoto { get; set; }

        //public bool IsUserPhotoLoaded { get; set; }

        public string UserName { get; set; }

        public string MessageText { get; set; }        

        public string MessageWroteDateString { get; set; }

        public DateTime MessageWroteDate { get; set; } 

        public bool IsMessageInbox { get; set; }

        public int CompareTo(MessagesListViewItem newMessageListViewItem)
        {
            int val = MessageWroteDate.CompareTo(newMessageListViewItem.MessageWroteDate);

            //val = -val;

            return val;
        }
    }
}
