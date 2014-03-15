/*	 This file was created by GALS Soft	company
 *	 http://www.galssoft.com
 *	 
 *	 Project name is LiveJournal Mobile
 *	 Platform is .Net Compact Framework 2.0 for Windows Mobile 5.0+ 
 * 
 *	 Copyright (c) 2009-2010 GALS Soft
 */

using System.Security.Cryptography;
using Galssoft.VKontakteWM.Components.SystemHelpers;
using Microsoft.Win32;
using Galssoft.VKontakteWM.Components.Configuration;

namespace Galssoft.VKontakteWM.Components.Common.SystemHelpers
{
	public class CryptoServiceProvider
	{
		private static RSACryptoServiceProvider _rsa = new RSACryptoServiceProvider();

		/// <summary>
		/// Шифрует массив байт
		/// </summary>
		/// <param name="originalData">Массив байт для шифрации</param>
		/// <returns>Зашифрованный массив байт</returns>
		public static byte[] RSAEncrypt(byte[] originalData)
		{
			try
			{
				byte[] keyData = _rsa.ExportCspBlob(true);				

                RegistryUtility.SetBinaryValue("CSPBlob", SystemConfiguration.CommonRegKey, (object)keyData, RegistryValueKind.Binary);

				return _rsa.Encrypt(originalData, false);
			}
			catch (CryptographicException e)
			{
				DebugHelper.WriteLogEntry(e.ToString());

				return null;
			}
		}

		/// <summary>
		/// Дешифрует массив байт
		/// </summary>
		/// <param name="encryptedData">Массив байт для дешифрации</param>
		/// <returns>Дешифрованный массив байт</returns>
		public static byte[] RSADecrypt(byte[] encryptedData)
		{
			try
			{				
                byte[] keyData = (byte[])RegistryUtility.GetBinaryValue("CSPBlob", SystemConfiguration.CommonRegKey);

                if (keyData != null)
                {
                    _rsa.ImportCspBlob(keyData);
                }
				
				return _rsa.Decrypt(encryptedData, false);
			}
			catch (CryptographicException e)
			{
                // перезапуск 
                _rsa = new RSACryptoServiceProvider();

				DebugHelper.WriteLogEntry(e.ToString());

				return null;
			}
		}

	}
}
