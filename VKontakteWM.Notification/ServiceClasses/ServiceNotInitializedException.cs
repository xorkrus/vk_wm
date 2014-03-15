using System;

namespace Galssoft.VKontakteWM.Notification.ServiceClasses
{
    /// <summary>
    /// Exception thrown by ServiceApplication if it was not initialized
    /// </summary>
    public class ServiceNotInitializedException : Exception
    {

        public ServiceNotInitializedException()
            : base("ServiceApplication was not initialized! Call Init() method before use.")
        {

        }
    }

}
