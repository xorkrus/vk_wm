using System;

using System.Collections.Generic;
using System.Text;

//using System.Xml.Serialization;

namespace Galssoft.VKontakteWM.Components.Common.ResponseClasses
{
    public class MessagesGetDialogsResponse
    {
        public List<Message> messages = new List<Message>();
    }

    public class Message
    {
        public string body { get; set; }

        public string title { get; set; }

        public string date { get; set; }

        public string uid { get; set; }

        public string mid { get; set; }

        public string read_state { get; set; }

        public string _out { get; set; }
    }
}
