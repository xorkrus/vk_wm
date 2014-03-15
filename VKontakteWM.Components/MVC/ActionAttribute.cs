using System;

namespace Galssoft.VKontakteWM.Components.MVC
{
    /// <summary>
    /// Attribute is used to bind events in View and Controller objects
    /// </summary>
    public class ActionAttribute : Attribute
    {
        private Type type;

        public ActionAttribute(Type type)
        {
            this.type = type;
        }

        public Type Type
        {
            get
            {
                return this.type;
            }
        }
    }
}