using System;
using System.Collections.Generic;
using System.Text;

namespace Galssoft.VKontakteWM.Components.ResponseClasses
{
    public class EventsGetResponse
    {
        public List<Event> events = new List<Event>();
    }

    public class Event
    {
        public EventType type { get; set; }

        public int number { get; set; }
    }
}
