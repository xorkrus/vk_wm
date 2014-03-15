/*	 This file was created by GALS Soft	company
 *	 http://www.galssoft.com
 *	 
 *	 Project name is Odnoklassniki Mobile
 *	 Platform is .Net Compact Framework 2.0 for Windows Mobile 5.0+ 
 * 
 *	 Copyright (c) 2009-2010 GALS Soft
 */

using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.IO;
using Microsoft.WindowsCE.Forms;

namespace Galssoft.VKontakteWM.Components.Common.SystemHelpers
{
    public class DeviceInfo
    {
        // Fields
        public string Device = "?";
        public string DevLink = "";
        public string FreeSpace = "Root:";
        public string Man = "";
        private const int SPI_GETOEMINFO = 0x102;

        // Methods
        public DeviceInfo()
        {
            this.DevLink = this.GetLink();
            this.Man = this.GetManufacturer();
            if (this.GetHTCdevice())
            {
                this.Man = "HTC";
            }
            else if (this.GetVoxtelDev())
            {
                this.Man = "VOXTEL";
            }
            else if (this.GetAsusDev())
            {
                this.Man = "ASUS";
            }
            else if (this.GetGlofishDev())
            {
                this.Man = "ETEN";
            }
            else
            {
                this.Man = this.GetManufacturer();
                if (this.Man.StartsWith("Generic Manufacturer"))
                {
                    this.Man = "Unknown";
                }
            }
            string directoryName = @"\";
            DiskFreeSpace space = new DiskFreeSpace();
            try
            {
                if (!GetDiskFreeSpaceEx(directoryName, ref space.FreeBytesAvailable, ref space.TotalBytes, ref space.TotalFreeBytes))
                {
                    this.FreeSpace = this.FreeSpace + "?/?";
                }
                else
                {
                    this.FreeSpace = this.FreeSpace + Convert.ToString((long)(space.FreeBytesAvailable / 0x400L)) + "kb/" + Convert.ToString((space.TotalBytes / 0x400L) + "kb");
                }
            }
            catch
            {
                this.FreeSpace = this.FreeSpace + "N/A";
            }
            DirectoryInfo info = new DirectoryInfo(@"\");
            foreach (DirectoryInfo info2 in info.GetDirectories())
            {
                if ((info2.Attributes & FileAttributes.Temporary) == FileAttributes.Temporary)
                {
                    space = new DiskFreeSpace();
                    try
                    {
                        if (!GetDiskFreeSpaceEx(info2.FullName, ref space.FreeBytesAvailable, ref space.TotalBytes, ref space.TotalFreeBytes))
                        {
                            this.FreeSpace = this.FreeSpace + " " + info2.Name + ":?/?";
                        }
                        else
                        {
                            string freeSpace = this.FreeSpace;
                            this.FreeSpace = freeSpace + " " + info2.Name + ":" + Convert.ToString((long)(space.FreeBytesAvailable / 0x400L)) + "kb/" + Convert.ToString((long)(space.TotalBytes / 0x400L)) + "kb";
                        }
                    }
                    catch
                    {
                        this.FreeSpace = this.FreeSpace + " " + info2.Name + ":N/A";
                    }
                }
            }
        }

        private bool GetAsusDev()
        {
            string name = @"SOFTWARE\WIDCOMM\BTConfig\General";
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(name);
                this.Device = key.GetValue("DeviceName").ToString();
                key.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }

        [DllImport("coredll")]
        private static extern bool GetDiskFreeSpaceEx(string directoryName, ref long freeBytesAvailable, ref long totalBytes, ref long totalFreeBytes);
        private bool GetGlofishDev()
        {
            string name = @"ControlPanel\ETEN\SystemInfo\System";
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(name);
                this.Device = "Glofish-" + key.GetValue("Model").ToString();
                key.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }

        private bool GetHTCdevice()
        {
            string name = @"SOFTWARE\HTC\Camera\Captparam\ExifInfo";
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(name);
                this.Device = key.GetValue("Model").ToString();
                key.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public string GetLink()
        {
            string str;
            string name = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings";
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(name);
                str = key.GetValue("UAProf").ToString();
                key.Close();
            }
            catch
            {
                str = "NoLink";
            }
            return str;
        }

        private string GetManufacturer()
        {
            string str = "?";
            string name = @"Drivers\USB\FunctionDrivers";
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(name);
                foreach (string str3 in key.GetSubKeyNames())
                {
                    key = Registry.LocalMachine.OpenSubKey(name + @"\" + str3);
                    try
                    {
                        str = key.GetValue("Manufacturer").ToString();
                    }
                    catch
                    {
                    }
                    break;
                }
                key.Close();
            }
            catch
            {
            }
            return str;
        }

        public string getScreenOrientation()
        {
            return SystemSettings.ScreenOrientation.ToString();
        }

        public string GetStorageCardNames()
        {
            List<string> list = new List<string>();
            DirectoryInfo info = new DirectoryInfo(@"\");
            foreach (DirectoryInfo info2 in info.GetDirectories())
            {
                if ((info2.Attributes & FileAttributes.Temporary) == FileAttributes.Temporary)
                {
                    list.Add(info2.Name);
                }
            }
            if (list.Count > 0)
            {
                return list[0];
            }
            return "";
        }

        private bool GetVoxtelDev()
        {
            string name = @"ControlPanel\OEMINFORMATION";
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(name);
                this.Device = key.GetValue("TF_VERSION").ToString();
                key.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }

        [DllImport("Coredll")]
        private static extern bool SystemParametersInfo(int uiAction, int uiParam, byte[] pvParam, int fWinIni);

        // Properties
        public static string DeviceID
        {
            get
            {
                bool flag;
                byte[] array = new byte[0x80];
                int length = array.Length;
                BitConverter.GetBytes(length).CopyTo(array, 0);
                try
                {
                    flag = SystemParametersInfo(0x102, length, array, 0);
                }
                catch
                {
                    flag = false;
                }
                if (flag)
                {
                    return Encoding.Unicode.GetString(array, 0, length).ToString().Trim('\0');
                }
                return "Error_get_device";
            }
        }

        // Nested Types
        [StructLayout(LayoutKind.Sequential)]
        public struct DiskFreeSpace
        {
            public long FreeBytesAvailable;
            public long TotalBytes;
            public long TotalFreeBytes;
        }
    }
}