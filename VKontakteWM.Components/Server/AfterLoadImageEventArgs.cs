namespace Galssoft.VKontakteWM.Components.Server
{
    public class AfterLoadImageEventArgs
    {
        public AfterLoadImageEventArgs(string imageFilename, bool imageLast, int imageLinearSize)
        {
            ImageFilename = imageFilename;
            ImageLast = imageLast;
            ImageLinearSize = imageLinearSize;
        }

        public string ImageFilename { get; private set; }

        public bool ImageLast { get; private set; }

        public int ImageLinearSize { get; private set; }

        public bool Result;
    }
}
