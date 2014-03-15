using Galssoft.VKontakteWM.Components.GDI;

namespace Galssoft.VKontakteWM.CustomControlls
{
    public enum ExtraActions
    {
        UserData,
        Settings,
        About,
        Help,
        Exit
    }

    public class ExtraButton
    {
        public ExtraButton(ExtraActions actionType, string text, IImage icon)
        {
            Action = actionType;
            Text = text;
            Icon = icon;
        }

        public ExtraActions Action { get; set; }
        public IImage Icon { get; set; }
        public string Text { get; set; }
    }
}
