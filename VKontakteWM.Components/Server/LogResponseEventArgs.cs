using System.IO;

namespace Galssoft.VKontakteWM.Components.Server
{
    public class LogResponseEventArgs
    {
        public LogResponseEventArgs(string info, Stream stream)
        {
            Info = info;
            Stream = stream;
        }

        public string Info;
        public Stream Stream;
    }
}