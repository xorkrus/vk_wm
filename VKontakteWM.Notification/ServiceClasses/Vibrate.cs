using System.Runtime.InteropServices;

namespace Galssoft.VKontakteWM.Notification.ServiceClasses
{
    public static class VibrateLED
    {
        [DllImport("Coredll")]
        extern static bool NLedSetDevice(int deviceId, ref NLED_SETTINGS_INFO info);
        [DllImport("Coredll")]
        extern static bool NLedGetDeviceInfo(int nInfoId, ref NLED_COUNT_INFO info);
        [DllImport("Coredll")]
        extern static bool NLedGetDeviceInfo(int nInfoId, ref NLED_SETTINGS_INFO info);
        [DllImport("Coredll")]
        extern static bool NLedGetDeviceInfo(int nInfoId, ref NLED_SUPPORTS_INFO info);

        public const int NLED_COUNT_INFO_ID = 0;
        public const int NLED_SUPPORTS_INFO_ID = 1;
        public const int NLED_SETTINGS_INFO_ID = 2;

        struct NLED_COUNT_INFO
        {
            public int cLeds;
        }

        struct NLED_SETTINGS_INFO
        {
            public int LedNum;
            public int OffOnBlink;
            public int TotalCycleTime;
            public int OnTime;
            public int OffTime;
            public int MetaCycleOn;
            public int MetaCycleOff;
        }

        struct NLED_SUPPORTS_INFO
        {
            public uint LedNum;
            public int lCycleAdjust;
            public bool fAdjustTotalCycleTime;
            public bool fAdjustOnTime;
            public bool fAdjustOffTime;
            public bool fMetaCycleOn;
            public bool fMetaCycleOff;
        }

        private static void LedOn(int id)
        {
            NLED_SETTINGS_INFO settings = new NLED_SETTINGS_INFO();
            settings.LedNum = id;
            settings.OffOnBlink = 1;
            NLedSetDevice(id, ref settings);
        }

        private static void LedOff(int id)
        {
            NLED_SETTINGS_INFO settings = new NLED_SETTINGS_INFO();
            settings.LedNum = id;
            settings.OffOnBlink = 0;
            NLedSetDevice(id, ref settings);
        }

        private static int _vibrateLedID = -1;

        public static void Vibrate()
        {
            if (_vibrateLedID < 0)
                _vibrateLedID = GetVibrateLNum();

            LedOn(_vibrateLedID);
            System.Threading.Thread.Sleep(150);
            LedOff(_vibrateLedID);
        }

        private static int GetVibrateLNum()
        {
            NLED_COUNT_INFO nci = new NLED_COUNT_INFO();
            int wCount = 0,
                VibrLed = 0;
            NLED_SUPPORTS_INFO sup = new NLED_SUPPORTS_INFO();
            if (NLedGetDeviceInfo(0, ref nci))
                wCount = (int)nci.cLeds;
            for (int i = wCount - 1; i > -1; i--)
            {
                sup.LedNum = (uint)i;
                NLedGetDeviceInfo(1, ref sup);

                if (sup.lCycleAdjust == -1)
                {
                    VibrLed = i;
                    break;
                }
            }
            return VibrLed;
        }


    }
}
