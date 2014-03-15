using System;
using System.Drawing;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    public class AfterLoadFriendAvatarEventArgs : EventArgs
    {
        public AfterLoadFriendAvatarEventArgs(string imageFileName, bool imageLast)
        {
            ImageFileName = imageFileName;
            ImageLast = imageLast;
        }

        public string ImageFileName { get; private set; }

        public bool ImageLast { get; private set; }

        public bool Result;

        public int ImageHeight { get; set; }
    }
}
