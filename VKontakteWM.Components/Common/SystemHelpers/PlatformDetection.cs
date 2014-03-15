using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using Galssoft.VKontakteWM.Components.Configuration;

namespace Galssoft.VKontakteWM.Components.SystemHelpers
{
	/// This code is based on a blog post from Andrew Arnott 
	/// http://blog.nerdbank.net/search/label/Smart%20devices
	/// </summary>
	public class PlatformDetection
	{
		#region SystemParametersInfoActions enum

		public enum SystemParametersInfoActions : uint
		{
			SPI_GETPLATFORMTYPE = 257, // this is used elsewhere for Smartphone/PocketPC detection 
			SPI_GETOEMINFO = 258
		}

		#endregion

		private const string MicrosoftEmulatorOemValue = "Microsoft DeviceEmulator";

		[DllImport("Coredll.dll", EntryPoint = "SystemParametersInfoW", CharSet = CharSet.Unicode)]
		private static extern int SystemParametersInfo4Strings(uint uiAction, uint uiParam, StringBuilder pvParam,
															   uint fWinIni);
		private static string GetPlatformType()
		{
			StringBuilder platformType = new StringBuilder(50);
			if (SystemParametersInfo4Strings((uint)SystemParametersInfoActions.SPI_GETPLATFORMTYPE, (uint)platformType.Capacity,
											 platformType, 0) == 0)
				throw new Exception("Error getting platform type.");
			return platformType.ToString();
		}

		public static string GetOemInfo()
		{
			StringBuilder oemInfo = new StringBuilder(50);

			if (SystemParametersInfo4Strings((uint)SystemParametersInfoActions.SPI_GETOEMINFO,
											 (uint)oemInfo.Capacity, oemInfo, 0) == 0)
				throw new Exception("Error getting OEM info.");
			return oemInfo.ToString();
		}

		/// <summary>
		/// Platform-OS-ProductName/ClientVersionMajor.Minor.Rev
		/// </summary>
		public static string GetCurrentClientVersion()
		{
			string info;

			try
			{
				info = GetOemInfo();
			}
			catch (Exception)
			{
				info = "Unknown";
			}

			Version assemblyVersion = Assembly.GetCallingAssembly().GetName().Version;

			return string.Format("{3} {2}, {0}, {1}", info, GetOSVersion(),
				string.Format("{0}.{1}.{2}", assemblyVersion.Major, assemblyVersion.Minor, assemblyVersion.Build),
				SystemConfiguration.ApplicationName);
		}

		public static string GetOSVersion()
		{
			return Environment.OSVersion.ToString();//.Version.Major.ToString() + Environment.OSVersion.Version.Minor.ToString() + Environment.OSVersion.Version.Build.ToString() + Environment.OSVersion.Version.Revision.ToString();
		}

		public static bool IsWM5()
		{
			return Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor < 2;//5.2 - WM6+, 5.1 - WM5
		}

		public static bool IsSmartphone()
		{
			return GetPlatformType() == "SmartPhone";
		}

		public static bool IsPocketPC()
		{
			return GetPlatformType() == "PocketPC";
		}

		public static bool IsEmulator()
		{
			return GetOemInfo() == MicrosoftEmulatorOemValue;
		}
	}
}
