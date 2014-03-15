/*	 This file was created by GALS Soft	company
 *	 http://www.galssoft.com
 *	 
 *	 Project name is Odnoklassniki Mobile
 *	 Platform is .Net Compact Framework 2.0 for Windows Mobile 5.0+ 
 * 
 *	 Copyright (c) 2009-2010 GALS Soft
 */

using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32;
using Microsoft.WindowsMobile.Status;

namespace Galssoft.VKontakteWM.Components.Common.SystemHelpers
{
    public class CoreHelper
    {
        #region PowerState enum

        public enum PowerState
        {
            On = 0x00010000,
            Off = 0x00020000,
            Unattended = 0x00400000,
            UserIdle = 0x01000000,
            Suspend = 0x00200000,
            BackLightOn = 0x02000000
        }

        public enum PowerRequirement
        {
            POWER_NAME = 0x00000001,
            POWER_FORCE = 0x00001000,
            POWER_DUMPDW = 0x00002000
        }

        private enum DevicePowerState : int
        {
            Unspecified = -1,
            D0 = 0, // Full On: full power, full functionality 
            D1, // Low Power On: fully functional at low power/performance 
            D2, // Standby: partially powered with automatic wake 
            D3, // Sleep: partially powered with device initiated wake 
            D4, // Off: unpowered 
        }

        #endregion

        #region PocketPC Power Management

        [DllImport("coredll.dll", SetLastError = true)]
        private static extern int SetSystemPowerState(string psState, int stateFlags, int options);

        [DllImport("coredll.dll", SetLastError = true)]
        private static extern int ReleasePowerRequirement(int hPowerReq);

        [DllImport("coredll.dll", SetLastError = true)]
        private static extern int SetDevicePower(string pvDevice, int dwDeviceFlags, DevicePowerState DeviceState);

        public static void SetDevicePowerState(PowerState state)
        {
            SetSystemPowerState(null, (int)state, 0);
        }

        public static void KeepDeviceAwake()
        {
            try
            {
                SystemIdleTimerReset();
            }
            catch (Exception e)
            {
                DebugHelper.WriteLogEntry(e.ToString());
                //Do nothing - not supported on the desktop
            }
        }

        public static bool TurnWiFi(bool on)
        {
            if (on)
            {
                string driver = WiFiFindDriverKey();
                SetDevicePower(driver, (int)PowerRequirement.POWER_NAME, DevicePowerState.D0);
            }
            else
            {
                string driver = WiFiFindDriverKey();
                SetDevicePower(driver, (int)PowerRequirement.POWER_NAME, DevicePowerState.D4);
            }

            //Проверка наличия WiFi
            //Ждем 5 сек пока WiFi законектится
            int cnt = 0;
            while (cnt < 10)
            {
                //if (SystemState.WiFiStateConnected)
                {
                    DebugHelper.WriteLogEntry("WiFi turned on");
                    return true;
                }
                Thread.Sleep(1000);
                cnt++;
            }
            DebugHelper.WriteLogEntry("WiFi can't be turned on");
            return false;
        }

        /// <summary>
        /// Utilities.WiFi.FindDriverKey() is simply a function that returns the whole registry key name of the key containing the NDIS MINIPORT class GUID defined in the SDK’s pm.h:
        /// </summary>
        private static string WiFiFindDriverKey()
        {
            string ret = string.Empty;

            //#define PMCLASS_NDIS_MINIPORT      TEXT("{98C5250D-C29A-4985-AE5F-AFE5367E5006}") 
            //(From "c:\Program Files (x86)\Windows Mobile 6 SDK\PocketPC\Include\Armv4i\pm.h") 
            string WiFiDriverClass = "{98C5250D-C29A-4985-AE5F-AFE5367E5006}";

            foreach (string tmp in Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\Power\\State", false).GetValueNames())
            {
                if (tmp.IndexOf(WiFiDriverClass) > -1)
                {
                    ret = tmp;
                    break;
                }
            }

            return ret;
        }

        [DllImport("coredll.dll")]
        private static extern void SystemIdleTimerReset();

        #endregion

        #region Free Memory

        public static int GetMemoryLoad()
        {
            MEMORYSTATUS memStatus = new MEMORYSTATUS();
            GlobalMemoryStatus(ref memStatus);

            return memStatus.dwMemoryLoad;
        }

        /// <summary>
        /// This function tries to free up memory for an application. If necessary, the shell closes down other applications by sending WM_CLOSE messages.
        /// </summary>
        /// <remarks>If a large memory allocation fails in your application, call SHCloseApps and attempt to allocate memory again.</remarks>
        /// <param name="needed">Specifies, in kilobytes, the amount of memory to be freed</param>
        /// <returns>This function returns TRUE if it is successful and FALSE if it fail</returns>
        public static bool FreeProgramMemoryIfNeeded(int needed)
        {
            bool result = true;
            needed *= 1024;

            MEMORYSTATUS memStatus = new MEMORYSTATUS();
            GlobalMemoryStatus(ref memStatus);

            if (memStatus.dwAvailPhys < needed)
            {
                if (SHCloseApps(needed) != 0)
                {
                    //result = false;
                    System.Threading.Thread.Sleep(1000);
                    result = SHCloseApps(needed) == 0;
                }
            }
            return result;
        }

        public static bool FreeVirtualMemoryIfNeeded(int needed)
        {
            bool result = true;

            MEMORYSTATUS memStatus = new MEMORYSTATUS();
            GlobalMemoryStatus(ref memStatus);

            if (memStatus.dwTotalVirtual < needed)
            {
                if (SHCloseApps(needed) != 0)
                    result = false;
            }
            return result;
        }

        //Required P/Invoke declarations
        [DllImport("aygshell.dll")]
        public static extern int SHCloseApps(int dwMemSought);

        [DllImport("coredll.dll")]
        public static extern void GlobalMemoryStatus(ref MEMORYSTATUS lpBuffer);

        public struct MEMORYSTATUS
        {
            // do not change positions
            public int dwLength;
            public int dwMemoryLoad;
            public int dwTotalPhys;
            public int dwAvailPhys;
            public int dwTotalPageFile;
            public int dwAvailPageFile;
            public int dwTotalVirtual;
            public int dwAvailVirtual;
        }

        #endregion

        #region SIP management

        [DllImport("coredll.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("coredll.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("coredll.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static void SetSipButton(bool show)
        {
            IntPtr hSipWindow = FindWindow("MS_SIPBUTTON", "MS_SIPBUTTON");
            if (hSipWindow != IntPtr.Zero)
            {
                IntPtr hSipButton = GetWindow(hSipWindow, 5);
                if (hSipButton != IntPtr.Zero) ShowWindow(hSipButton, (show) ? 1 : 0);
            }
        }

        #endregion

        //[DllImport("coredll.dll")]
        //public static extern int ShowWindow(IntPtr hWnd, int nCmdShow);

        //public const int SW_MINIMIZED = 6;

    }
}