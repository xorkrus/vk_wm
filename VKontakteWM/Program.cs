using System;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Common;
using Galssoft.VKontakteWM.Components;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.Data;
using Galssoft.VKontakteWM.Components.Process;
using Galssoft.VKontakteWM.Components.Server;
using Galssoft.VKontakteWM.Forms;
using Microsoft.WindowsCE.Forms;
using Galssoft.VKontakteWM.Components.Configuration;
using Galssoft.VKontakteWM.Properties;
using Galssoft.VKontakteWM.Components.Cache;
using Galssoft.VKontakteWM.Components.SystemHelpers;

namespace Galssoft.VKontakteWM
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            // add global exception handler
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            DebugHelper.WriteTraceEntry("-------------------- Start programm --------------------");

            Process[] proc = Process.GetProcesses();
            int i = 0;
            foreach (Process process in proc)
            {
                if (process.ProcessName == "VKontakteWM.exe")
                {
                    i++;
                    if (i > 1)
                    {
                        Application.Exit();
                        return;
                    }
                }
            }

            SystemConfiguration.Init("ВКонтакте", "sysconfig.xml");

            if (PlatformDetection.IsSmartphone() || !(Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1)) //5.2 - WM6+, 5.1 - WM5))
            {
                MessageBox.Show(Resources.PlatformSupport, SystemConfiguration.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);

                return;
            }

            Cache.InitPath(SystemConfiguration.AppInstallPath + "\\Cache");

            ////make sure we have enough memory to run
            CoreHelper.FreeProgramMemoryIfNeeded(4000);
            // make sure device does not go to sleep during startup
            CoreHelper.KeepDeviceAwake();

            MobileDevice.Hibernate += MobileDevice_Hibernate;

            // Инициализация синглтона бизнес-логики
            Globals.Init(new BaseLogic(new DataLogic(), new CommunicationLogic()));

            // init master form
            MasterForm form = new MasterForm();

            form.Closing += form_Closing;
            form.Initialize();

            Application.Run(form);
        }

        static void MobileDevice_Hibernate(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        static void form_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DebugHelper.LogMemoryState();

            DebugHelper.WriteTraceEntry("-------------------- End programm --------------------");
            DebugHelper.FlushTraceBuffer();
        }

        private static void OnUnhandledException(Object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                DebugHelper.WriteReleaseLogEntry("Fatal Exception Handler: " + ex.Message + "\n" + ex.StackTrace + "\n\n");

                if (ex is OutOfMemoryException)
                {
                    DebugHelper.WriteReleaseLogEntry("!!! OUT OF MEMORY EXCEPTION !!!\n" + ex.StackTrace + "\n\n");

                    MessageBox.Show(Resources.NotEnoughMemory, Resources.InsufficientMemory,
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                }
                else
                {
                    MessageBox.Show(string.Format(Resources.FatalErrorInfo, ex.Message),
                        Resources.FatalError);
                }

                DebugHelper.FlushTraceBuffer();
            }
        }

    }
}