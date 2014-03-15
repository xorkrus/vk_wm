using System.Security.Cryptography;
using Galssoft.VKontakteWM.Common;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;

namespace Galssoft.VKontakteWM.Cryptography
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
                Globals.BaseLogic.IDataLogic.SetCSPBlobValue(keyData);

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
                byte[] keyData = Globals.BaseLogic.IDataLogic.GetCSPBlobValue();
                if (keyData != null)
                    _rsa.ImportCspBlob(keyData);

                return _rsa.Decrypt(encryptedData, false);
            }
            catch (CryptographicException e)
            {
                //перезапуск 
                _rsa = new RSACryptoServiceProvider();
                DebugHelper.WriteLogEntry(e.ToString());
                return null;
            }
        }
    }
}
