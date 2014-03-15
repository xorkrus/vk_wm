using Galssoft.VKontakteWM.Components;
using Galssoft.VKontakteWM.Components.GDI;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    public class EventButton
    {
        public EventButton(string text, int count, EventType eventType, IImage icon)
        {
            Count = count;
            Text = text;
            Event = eventType;
            Icon = icon;
        }

        public EventType Event { get; set; }
        public IImage Icon { get; set; }
        public string Text { get; set; }
        public int Count { get; set; }
    }
}
