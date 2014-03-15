using System;
using System.IO;

namespace Galssoft.VKontakteWM.Components.Server
{
    public class LogRequestEventArgs : EventArgs
    {
        public LogRequestEventArgs(string info, Stream stream)
        {
            Info = info;
            Stream = stream;
        }

        public string Info;
        public Stream Stream;
    }
}
