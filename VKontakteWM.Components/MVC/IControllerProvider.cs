using System;

namespace Galssoft.VKontakteWM.Components.MVC
{
    public interface IControllerProvider
    {
        Controller GetController(string name);
        Controller GetController(Type type);
        Controller GetController<T>();
        Controller GetController<T>(IView view);     
        void RegisterController(Controller controller);
    }
}