using Galssoft.VKontakteWM.Components.GDI;

namespace Galssoft.VKontakteWM.CustomControlls
{
    public enum UserDataActions
    {
        ClearPass,
        ClearCache
    }

    public class UserDataButton
    {
        public UserDataButton(UserDataActions actionType, string text, IImage icon)
        {
            Action = actionType;
            Text = text;
            Icon = icon;
        }

        public UserDataActions Action { get; set; }
        public IImage Icon { get; set; }
        public string Text { get; set; }
    }
}
