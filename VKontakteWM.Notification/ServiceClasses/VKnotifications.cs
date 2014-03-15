using System;
using System.Runtime.InteropServices;

namespace Galssoft.VKontakteWM.Notification.ServiceClasses
{
    /// <summary>
    /// Class wraps P/Invokes to the windows system functions
    /// </summary>
    public class VKnotifications
    {
        [DllImport("coredll.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(IntPtr lpClassName, string lpWindowName);

        [DllImport("coredll.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    }
}
