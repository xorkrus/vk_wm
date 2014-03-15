using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Microsoft.WindowsCE.Forms;

namespace Galssoft.VKontakteWM.Notification.ServiceClasses
{
    /// <summary>
    /// Class for interprocess commnucation
    /// </summary>
    public class Interprocess
    {
        public const string ServiceName = "VKnotificationService";

        /// <summary>
        /// USER messages starts from 0x400
        /// </summary>
        public const int WM_USER = 0x400;
        /// <summary>
        /// QUIT message
        /// </summary>
        public const int WM_QUIT_SERVICE = WM_USER + 5001;
        /// <summary>
        /// SHOW_NFN message
        /// </summary>
        public const int WM_TIMER_TICK = WM_USER + 5002;

        //FIXME
        private static string ServiceExe = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + Path.DirectorySeparatorChar + "VKontakteWM.Notification.exe";

        /// <summary>
        /// Sends quit message to service
        /// </summary>
        public static void StopService()
        {
            IntPtr hwnd = VKnotifications.FindWindow(IntPtr.Zero, ServiceName);
            if (hwnd != IntPtr.Zero)
            {
                Message msg = Message.Create(hwnd, WM_QUIT_SERVICE, (IntPtr) 0, (IntPtr) 0);
                MessageWindow.SendMessage(ref msg);
            }
        }

        /// <summary>
        /// Sends timer tick message to service
        /// </summary>
        public static void TimerTick()
        {
            IntPtr hwnd = VKnotifications.FindWindow(IntPtr.Zero, ServiceName);
            Message msg = Message.Create(hwnd, WM_TIMER_TICK, (IntPtr)0, (IntPtr)0);
            MessageWindow.SendMessage(ref msg);
        }

        /// <summary>
        /// Starts service
        /// </summary>
        public static void StartService()
        {
            // Нотификатор отключён по просьбе пользователей - сильно садит батарейку и кушает много траффика
            return;

            Process proc = new Process();
            proc.StartInfo.FileName = ServiceExe;
            proc.StartInfo.UseShellExecute = true;
            try
            {
                proc.Start();
            }
            catch (System.Exception ex)
            {
                //throw new OKException(ExceptionMessage.ServiceStartError, String.Format(LocalizedMessages.Interprocess_StartService_FileNotFound, proc.StartInfo.FileName));
                DebugHelper.WriteLogEntry(ex, String.Format(Properties.Resources.Interprocess_StartService_FileNotFound, proc.StartInfo.FileName));
#if DEBUG
                MessageBox.Show(String.Format(Properties.Resources.Interprocess_StartService_FileNotFound, proc.StartInfo.FileName));
#endif
            }
        }

        /// <summary>
        /// Determines whether the service is running or not
        /// </summary>      
        public static bool IsServiceRunning
        {
            get
            {
                IntPtr hwnd = VKnotifications.FindWindow(IntPtr.Zero, ServiceName);
                if (hwnd.ToInt64() > 0)
                    return true;
                else
                    return false;
            }
        }
    }
}