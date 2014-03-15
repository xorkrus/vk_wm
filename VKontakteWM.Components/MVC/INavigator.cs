namespace Galssoft.VKontakteWM.Components.MVC
{
    public interface INavigator
    {
        //void Navigate(string name);
        void Navigate(string name, params object[] parameters);
        //void Navigate(Controller controller);
        void Navigate(Controller controller, params object[] parameters);
        //DialogResult NavigateDialog(string name);
        //DialogResult NavigateDialog(Controller controller);
        //DialogResult NavigateDialog(Controller controller, params object[] parameters);
        void GoBack();
        void GoForward();
        bool CanGoBack();
        bool CanGoForward();
    }
}