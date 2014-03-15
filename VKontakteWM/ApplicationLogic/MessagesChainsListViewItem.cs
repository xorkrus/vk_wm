using System;

using System.Collections.Generic;
using System.Text;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    public class MessagesChainsListViewItem : IComparable<MessagesChainsListViewItem>
    {
        public string Uid { get; set; }

        public int UserID { get; set; }

        //public string UserPhoto { get; set; }

        //public bool IsUserPhotoLoaded { get; set; }

        public string UserName { get; set; }

        public string MessageText { get; set; }

        public string MessageWroteDateString { get; set; }

        public DateTime MessageWroteDate { get; set; } //диржим для сравнения

        public bool IsMessageNew { get; set; }

        public bool IsMessageOutbox { get; set; }

        public int CompareTo(MessagesChainsListViewItem newMessagesChainsListViewItem)
        {
            int val = MessageWroteDate.CompareTo(newMessagesChainsListViewItem.MessageWroteDate);

            val = -val;

            return val;
        }
    }
}
