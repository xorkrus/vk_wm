/*	 This file was created by GALS Soft	company
 *	 http://www.galssoft.com
 *	 
 *	 Project name is Odnoklassniki Mobile
 *	 Platform is .Net Compact Framework 2.0 for Windows Mobile 5.0+ 
 * 
 *	 Copyright (c) 2009-2010 GALS Soft
 */

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Diagnostics;
using Galssoft.VKontakteWM.Components.Configuration;

namespace Galssoft.VKontakteWM.Components.Common.SystemHelpers
{
    public class DebugHelper
    {
        /// <summary>
        /// The TraceLevel, 0 = none 
        /// </summary>
        public static int TraceLevel
        {
            get { return 1; }
        }

        private static StringBuilder _traceString;
        private static int _prevTick;

        private static string GetDataForLog()
        {
            DeviceInfo devInfo = new DeviceInfo();
            string deviceID = DeviceInfo.DeviceID;
            RegionInfo info2 = new RegionInfo(CultureInfo.CurrentCulture.LCID);
            string englishName = info2.EnglishName;
            string[] strArray = new string[4];
            strArray[0] = "Device:\"";
            char[] trimChars = new char[2];
            trimChars[1] = ' ';
            strArray[1] = deviceID.TrimEnd(trimChars);
            strArray[2] = " [";
            strArray[3] = devInfo.Device;
            strArray[4] = "]\"";
            string str3 = string.Concat(strArray);
            try
            {
                str3 = str3 + "\r\nDeviceName:" + Dns.GetHostName();
                str3 = str3 + "\r\nRegion:" + englishName;
                str3 = str3 + "\r\nProgrammPath:" + SystemConfiguration.AppInstallPath;
            }
            catch
            { }
            string str4 = Environment.Version.ToString();
            string str5 = Environment.OSVersion.Platform.ToString() + "_" + Environment.OSVersion.Version.ToString();
            string str6 = Screen.PrimaryScreen.Bounds.Width.ToString() + "x" + Screen.PrimaryScreen.Bounds.Height.ToString();
            string str7 = str3;
            return (str7 + "\r\nNET:" + str4 + " \r\nWindowsMobileOS:" + str5 + " \r\nResolution:" + str6 + " \r\nSRotate:" + devInfo.getScreenOrientation() + " \r\nClientVersion:" + SystemConfiguration.FullVersionString + " \r\n");
        }

        public static string GetTimeDurationAsString(int milliseconds)
        {
            TimeSpan ts = new TimeSpan(milliseconds * TimeSpan.TicksPerMillisecond);

            if (ts.TotalSeconds < 1)
                return ts.Milliseconds + " mSec.";

            if (ts.TotalMinutes < 1)
                return ts.Seconds + " sec.";

            if (ts.TotalHours < 1)
                return ts.Minutes + " min. " + ts.Seconds + " sec.";

            if (ts.Days < 1)
                return ts.Hours + "hr. " + ts.Minutes + " min. " + ts.Seconds + " sec.";

            return ts.ToString();
        }

        private static bool _logInitialized = false;

        private static void StartLog()
        {
            _logInitialized = true;

            try
            {
                FileInfo info = new FileInfo(SystemConfiguration.LogFilePath);
                bool exists = info.Exists;
                if (exists && (info.Length > 0x100000L))
                {
                    try
                    {
                        info.Delete();
                    }
                    catch (IOException)
                    {
                        //TODO: make a new file with next sequential number in name: filename%N+1%.txt
                    }
                    exists = false;
                }
                using (StreamWriter sw = File.AppendText(SystemConfiguration.LogFilePath))
                {
                    if (!exists)
                    {
                        string dataForLog = GetDataForLog();
                        sw.Write(dataForLog);
                    }
                    sw.WriteLine("----------------- " + DateTime.Now + "---------------------");
                    sw.WriteLine("Version: " + SystemConfiguration.FullVersionString + "\r\n\r\n");
#if DEBUG
                    sw.WriteLine("Flag: DEBUG");
#endif
#if TRACE
                    sw.WriteLine("Flag: TRACE");
#endif

                }
            }
            catch
            {
            }
        }

        [Conditional("DEBUG")]
        public static void WriteLogEntry(string entry)
        {
            if (!_logInitialized)
                StartLog();

            try
            {
                using (StreamWriter sw = File.AppendText(SystemConfiguration.LogFilePath))
                {
                    sw.WriteLine(string.Format("{0}: {1}", DateTime.Now, entry));
                }
            }
            catch (ThreadAbortException)
            {
                // Наш поток прервали при записи в файл (например, при загрузке постов нажали Отмену)
            }
            catch (Exception e)
            {
#if DEBUG
                //MessageBox.Show(e.ToString());
#endif
            }
        }

        public static void WriteReleaseLogEntry(string entry)
        {
            if (!_logInitialized)
                StartLog();

            try
            {
                using (StreamWriter sw = File.AppendText(SystemConfiguration.LogFilePath))
                {
                    sw.WriteLine(string.Format("{0}: {1}", DateTime.Now, entry));
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception e)
            {
            }
        }

        /// <summary>
        /// Вывод ошибки со стеком вызовов по заданному объекту исключения
        /// </summary>
        /// <param name="e">Исключение</param>
        /// <param name="message">Дополнительная информация</param>
        [Conditional("DEBUG")]
        public static void WriteLogEntry(Exception e, string message)
        {
            if (message == "")
                WriteLogEntry(String.Format("Exception: {0} Message: {1} Stack: {2}", e.GetType(), e.Message, e.StackTrace));
            else
                WriteLogEntry(String.Format("Exception: {0} Message: {1} ({2}) Stack: {3}", e.GetType(), e.Message, message, e.StackTrace));
        }

        [Conditional("TRACE")]
        public static void WriteTraceEntry(string text)
        {
            if (_traceString == null)
                _traceString = new StringBuilder(8192);
            if (_prevTick == 0)
                _prevTick = Environment.TickCount;
            _traceString.Append(string.Format("{0} {1}  Tick:{2}\r\n", DateTime.Now.ToString("mm.ss.fff"), text, Environment.TickCount - _prevTick));
            _prevTick = Environment.TickCount;
        }

        public static void LogMemoryState()
        {
            WriteLogEntry(MemoryStatus.GetMemoryInfo());
        }

        [Conditional("TRACE")]
        public static void FlushTraceBuffer()
        {
            if (_traceString != null && _traceString.Length > 0)
            {
                WriteLogEntry(_traceString.ToString());
                _traceString = null;
            }
        }
    }
}