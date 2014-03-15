using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace Galssoft.VKontakteWM.Notification.ServiceClasses
{
    public enum SoundFileType { WAV, MP3, MIDI, WMA }

    public class SoundPlayer : IDisposable
    {
        [DllImport("aygshell.dll")]
        static extern uint SndOpen(string pszSoundFile, ref IntPtr phSound);

        [DllImport("aygshell.dll")]
        static extern uint SndPlayAsync(IntPtr hSound, uint dwFlags);

        [DllImport("aygshell.dll")]
        static extern uint SndClose(IntPtr hSound);

        [DllImport("aygshell.dll")]
        static extern uint SndStop(int SoundScope, IntPtr hSound);

        [DllImport("aygshell.dll")]
        static extern uint SndPlaySync(string pszSoundFile, uint dwFlags);

        const int SND_SCOPE_PROCESS = 0x1;
        IntPtr sound = IntPtr.Zero;
        Thread soundThread = null;

        string _filePath = string.Empty;
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        public SoundPlayer(string filePath)
        {
            _filePath = filePath;
        }

        public SoundPlayer(byte[] soundBytes, SoundFileType fileType)
        {
            string fileExtension = ".wav";
            switch (fileType)
            {
                case SoundFileType.WAV:
                    fileExtension = ".wav";
                    break;
                case SoundFileType.MP3:
                    fileExtension = ".mp3";
                    break;
                case SoundFileType.MIDI:
                    fileExtension = ".mid";
                    break;
                //case SoundFileType.WMA:
                //    fileExtension = ".wma";
                //    break;
                default:
                    throw new Exception("Audio format is not supported.");
                    break;
            }

            _filePath = Path.GetTempPath() + "tempSound" + fileExtension;
            using (FileStream fs = new FileStream(_filePath, FileMode.Create, FileAccess.ReadWrite))
            {
                fs.Write(soundBytes, 0, soundBytes.Length);
            }
        }

        public void Play()
        {
            SndPlaySync(_filePath, 0);
        }

        public void PlayLooping()
        {
            soundThread = new Thread(Playing);
            soundThread.Start();
        }

        private void Playing()
        {
            while (true)
            {
                SndPlaySync(_filePath, 0);
            }
        }

        public void PlayAsync()
        {
            SndOpen(_filePath, ref sound);
            SndPlayAsync(sound, 0);
        }

        public void Stop()
        {
            if (soundThread != null)
            {
                SndStop(SND_SCOPE_PROCESS, IntPtr.Zero);
                soundThread.Abort();
                soundThread = null;
            }
            if (sound != IntPtr.Zero)
            {
                SndStop(SND_SCOPE_PROCESS, IntPtr.Zero);
                SndClose(sound);
                sound = IntPtr.Zero;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Stop();
        }

        #endregion
    }
}
