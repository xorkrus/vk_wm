using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Galssoft.VKontakteWM.Components.Common.Localization;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.Configuration;

namespace Galssoft.VKontakteWM.Notification.ServiceClasses
{
    public static class WakeupScheduler
    {
        [DllImport("Coredll", EntryPoint = "CeSetUserNotificationEx")]
        extern static IntPtr CeSetUserNotificationExNoType(IntPtr hNotification, ref UserNotificationTrigger pcnt, IntPtr pceun);

        private struct SystemTime
        {
            [DllImport("CoreDLL.dll")]
            static extern int FileTimeToSystemTime(ref long lpFileTime, ref SystemTime lpSystemTime);

            [DllImport("CoreDLL.dll")]
            static extern int FileTimeToLocalFileTime(ref long lpFileTime, ref long lpLocalFileTime);

            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;

            public static SystemTime FromDateTime(DateTime dateTime)
            {
                long fileTime = dateTime.ToFileTime();
                long localFileTime = 0;
                FileTimeToLocalFileTime(ref fileTime, ref localFileTime);
                SystemTime systemTime = new SystemTime();
                FileTimeToSystemTime(ref localFileTime, ref systemTime);
                return systemTime;
            }

            public static SystemTime FromDateTime2(DateTime dateTime)
            {
                SystemTime ret = new SystemTime();
                ret.wYear = (ushort)dateTime.Year;
                ret.wMonth = (ushort)dateTime.Month;
                ret.wDayOfWeek = (ushort)dateTime.DayOfWeek;
                ret.wDay = (ushort)dateTime.Day;
                ret.wHour = (ushort)dateTime.Hour;
                ret.wMinute = (ushort)dateTime.Minute;
                ret.wSecond = (ushort)dateTime.Second;
                ret.wMilliseconds = (ushort)dateTime.Millisecond;
                return ret;
            }
        }

        private struct UserNotificationTrigger
        {
            public int dwSize;
            public NotificationType dwType;
            public int dwEvent;
            public IntPtr lpszApplication;
            public IntPtr lpszArguments;
            public SystemTime stStartTime;
            public SystemTime stEndTime;
        }

        private enum NotificationType : int
        {
            CNT_EVENT = 1,			//@flag CNT_EVENT  | System event notification
            CNT_TIME = 2,			//@flag CNT_TIME   | Time-based notification
            CNT_PERIOD = 3,			//@flag CNT_PERIOD | Time-based notification is active for
        }

        private static string GetPowerStateExecutablePath()
        {
            //TODO: fix this please
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + Path.DirectorySeparatorChar + "WakeUp.exe";
            DebugHelper.WriteLogEntry(path);
            return path;
        }

        public static void ScheduleWakeUp(DateTime time)
        {
            var powerOn = GetPowerStateExecutablePath();
            if (File.Exists(powerOn))
            {
                DateTime powerOnTime = time;
                DebugHelper.WriteLogEntry(time.ToString());
                CeSetUserNotificationEx(powerOn, string.Empty, powerOnTime);
            }

        }

        ///// <summary>
        ///// Unregister an application given the path to the executable or the token that was provided
        ///// </summary>
        ///// <param name="applicationOrToken">The path to the application executable or the token, whichever was provided.</param>
        //public static void UnscheduleApplicationLaunch(string applicationOrToken)
        //{
        //    IntPtr handle = FindApplicationNotification(applicationOrToken);
        //    while (handle != IntPtr.Zero)
        //    {
        //        CeClearUserNotification(handle);
        //        handle = FindApplicationNotification(applicationOrToken);
        //    }

        //    //bool bret = UnscheduleCeRunAppAtTime(applicationOrToken, IntPtr.Zero);

        //    //int ret = ConnMgrUnregisterScheduledConnection(GetTokenFromApplication(applicationOrToken));
        //}

        private static void CeSetUserNotificationEx(string application, string arguments, DateTime start)
        {
            //UnscheduleApplicationLaunch(application);

            IntPtr appBuff = Marshal.StringToBSTR(application);
            IntPtr argsBuff = Marshal.StringToBSTR(arguments);

            var trigger = new UserNotificationTrigger
            {
                dwSize = Marshal.SizeOf(typeof(UserNotificationTrigger)),
                dwType = NotificationType.CNT_TIME,
                lpszApplication = appBuff,
                lpszArguments = argsBuff,
                stStartTime = SystemTime.FromDateTime2(start)
            };

            IntPtr handle = CeSetUserNotificationExNoType(IntPtr.Zero, ref trigger, IntPtr.Zero);
            Marshal.FreeBSTR(appBuff);
            Marshal.FreeBSTR(argsBuff);

            //TODO: fix this please

            if (handle == IntPtr.Zero)
                throw new ExternalException(string.Format("Unable to schedule application launch {0}: {1} {2}", start, application, arguments));
        }
    }
}
