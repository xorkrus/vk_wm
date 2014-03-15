using System;

namespace Galssoft.VKontakteWM.Components.MVC
{
    public class StateArgs : EventArgs
    {
        private ViewDataDictionary data;

        public StateArgs(ViewDataDictionary data)
        {
            this.data = data;
        }

        public ViewDataDictionary Data
        {
            get
            {
                return data;
            }
        }
    }
}