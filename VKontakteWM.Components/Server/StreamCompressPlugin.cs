// --- Copyright (c) 2006-2008 Stefan Kyntchev ---
// This software is written and copyrighted by Stefan Kyntchev 
// and BeyondPod Team members. All rights are reserved.
// Author contact: support@beyondpod.mobi
// ------------------------------------------------------------------------
// This file is part of BeyondPod RSS Feed Reader and Podcast manager
// (www.codeplex.com/BeyondPod) 
// BeyondPod is free software: you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published by 
// the Free Software Foundation, either version 3 of the License, or 
// (at your option) any later version. 
//  
// BeyondPod is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
// GNU General Public License for more details. 
//  
// You should have received a copy of the GNU General Public License
// along with BeyondPod. If not, see <http://www.gnu.org/licenses/>
// ------------------------------------------------------------------------
// COPYRIGHT NOTICE: This copyright notice may NOT be removed, obscured or modified 
// without written consent from the author.
// --- End copyright notice --- 


using System;
using System.IO;
using System.Reflection;
using Galssoft.VKontakteWM.Components.SystemHelpers;

namespace Galssoft.VKontakteWM.Components.Server
{
	public static class StreamCompressPlugin
	{
		private static Assembly m_Assembly;
		private static Type m_gzip;
		private static Type m_inflate;
		private static Type m_CompressionMode;
        private static Logger Log;

		private static readonly Type[] m_ConstructorSignature = new Type[] { typeof(Stream) };
		private static readonly Type[] m_BuiltInConstructorSignature = new Type[2];

		static StreamCompressPlugin()
		{
			if (IsCompressionSupportedByTheFramework)
			{
				LoadBuiltInDecompression();
			}
			else
			{
				LoadSharpZipLibDecompression();
			}
		}

		static void LoadBuiltInDecompression()
		{
			try
			{
				m_Assembly = Assembly.Load("System");
				m_gzip = m_Assembly.GetType("System.IO.Compression.GZipStream");
				m_inflate = m_Assembly.GetType("System.IO.Compression.DeflateStream");
				m_CompressionMode = m_Assembly.GetType("System.IO.Compression.CompressionMode");

				m_BuiltInConstructorSignature[0] = typeof(Stream);
				m_BuiltInConstructorSignature[1] = m_CompressionMode;

				if (m_gzip == null || m_inflate == null || m_CompressionMode == null)
				{
                    Log.Write(LogEntryType.Error, "Unable to find GZipStream or DeflateStream types!");
					m_Assembly = null;
				}
			}
			catch (Exception ex)
			{
				m_Assembly = null;
                Log.Write(LogEntryType.Error, "Unable to load System.dll! Reason: " + ex.Message);
			}
		}

		static void LoadSharpZipLibDecompression()
		{
			string assemblyPath = Path.Combine("", "SharpZipLib.dll");
			try
			{
				m_Assembly = Assembly.LoadFrom(assemblyPath);
				m_gzip = m_Assembly.GetType("ICSharpCode.SharpZipLib.GZip.GZipInputStream");
				m_inflate = m_Assembly.GetType("ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream");

				if (m_gzip == null || m_inflate == null)
				{
                    Log.Write (LogEntryType.Error, "Unable to find GZipInputStream or InflaterInputStream types!");
					m_Assembly = null;
				}
			}
			catch (Exception ex)
			{
				m_Assembly = null;
				if (File.Exists(assemblyPath))	
                    Log.Write(LogEntryType.Error, "Unable to load SHarpZipLibAssembly! Reason: " + ex.Message);
			}
		}

		/// <summary>
		/// True если плаггин доступен
		/// </summary>
		public static bool IsAvailable
		{
			get
			{
				return IsCompressionSupportedByTheFramework || m_Assembly != null;
			}
		}

		/// <summary>
		/// Создает новый экземпляр GZip decompression stream
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static Stream CreateGZipInputStream(Stream input)
		{
			object[] pars;

			if (IsCompressionSupportedByTheFramework)
			{
				pars = new object[] { input, 0 };
				return m_gzip.GetConstructor(m_BuiltInConstructorSignature).Invoke(pars) as Stream;
			}
			else
			{
				pars = new object[] { input };
				return m_gzip.GetConstructor(m_ConstructorSignature).Invoke(pars) as Stream;
			}
		}

		/// <summary>
		/// Создает новый экземпляр inflate stream
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static Stream CreateInflaterInputStream(Stream input)
		{
			object[] pars;

			if (IsCompressionSupportedByTheFramework)
			{
				pars = new object[] { input, 0 };
				return m_inflate.GetConstructor(m_BuiltInConstructorSignature).Invoke(pars) as Stream;
			}
			else
			{
				pars = new object[] { input };
				return m_inflate.GetConstructor(m_ConstructorSignature).Invoke(pars) as Stream;
			}
		}

		static bool IsCompressionSupportedByTheFramework
		{
			get
			{
				return Environment.Version >= new Version(3, 5);
			}
		}
	}
}