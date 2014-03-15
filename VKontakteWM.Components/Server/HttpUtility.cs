using System;
using System.IO;
using System.Text;
using System.Net;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.SystemHelpers;
using System.Security.Cryptography;
using Galssoft.VKontakteWM.Components.Configuration;

namespace Galssoft.VKontakteWM.Components.Server
{
	public sealed class HttpUtility
	{
		/// <summary>
		/// Prepare HttpWebRequest with default parameters
		/// </summary>
		/// <param name="url">URL for request. </param>
		/// <returns>Prepared HttpWebRequest.</returns>
		public static HttpWebRequest PrepareHttpWebRequest(string url)
		{
			return PrepareHttpWebRequest(url, "GET");
		}

		/// <summary>
		/// Prepare HttpWebRequest with default parameters
		/// </summary>
		/// <param name="url">URL for request. </param>
		/// <param name="method">Method type - GET, PUT, POST. </param>
		/// <returns>Prepared HttpWebRequest.</returns>
		public static HttpWebRequest PrepareHttpWebRequest(string url, string method)
		{
            if (String.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            if (String.IsNullOrEmpty(method))
            {
                throw new ArgumentNullException("method");
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = method.ToUpper();

            //if (StreamCompressPlugin.IsAvailable)
            //{
            request.Headers["Accept-Encoding"] = "gzip, deflate";
            //}
           
            request.UserAgent = PlatformDetection.GetCurrentClientVersion();
            request.Timeout = SystemConfiguration.ServerConnectionTimeOutShort;
            request.AllowAutoRedirect = false;
            return request;            
		}

        /// <summary>
        /// Checks if response is gzipped and returns stream (even ungzipped)
        /// </summary>
        /// <param name="webResponse">Web responce to stream</param>
        /// <returns>Stream</returns>
        public static Stream PrepareResponseStream(WebResponse webResponse)
        {
            WebResponse httpWebResponse = new CompressedWebResponse(webResponse);
            return httpWebResponse.GetResponseStream();
        }

        /// <summary>
        /// Формирует MD5-хэш, передаваемой строки
        /// </summary>
        /// <param name="input">Строка для формирования MD5-хэша </param>
        /// <returns>Сформированный MD5-хэш </returns>
        public static string GetMd5Hash(string input)
        {

            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();
            // Convert the input string to a byte array and compute the hash.
            byte[] data = (input != null) ? md5Hasher.ComputeHash(Encoding.Default.GetBytes(input)) : new byte[0];
            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();
            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public static void CopyStream(Stream input, Stream output)
        {
            try
            {
                int bufferSize = 4096;
                byte[] buffer = new byte[bufferSize];
                while (true)
                {
                    int read = input.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                    {
                        input.Flush();
                        input.Close();
                        return;
                    }
                    output.Write(buffer, 0, read);
                }
            }
            catch (Exception ex)
            {
                DebugHelper.WriteLogEntry(ex, "Streem copy error");
                output = null;
            }
        }

	    /// <summary>
	    ///FIXME Возвращает текстовое описание ошибки по ее коду (Это пойдет в сборку локализации)
	    /// </summary>
        ///<param name="errorCode">Номер ошибки </param>
	    ///<returns>Описание ошибки </returns>
	    public static string GetErrorDescription(int errorCode)
        {
            string res = "";
            switch (errorCode)
            { 
                case 0:
                    res = "Неопознанная ошибка на сервере.";
                    break;
                case 1:
                    res = "Текущая сессия истекла.";
                    break;
                case 2:
                    res = "Требуется ввод captcha.";
                    break;
                case 3:
                    res = "Сервер временно недоступен.";
                    break;
                case 4:
                    res = "Сервер временно недоступен.";
                    break;
                case 5:
                    res = "Неудалось подключится к серверу, проверьте настройки подключения.";
                    break;
                case 10:
                    res = "Сервер временно недоступен.";
                    break;
                case 11:
                    res = "Операция прошла не успешно.";
                    break;
                case 12:
                    res = "Операция запрещена приватностью.";
                    break;
                case 100:
                    res = "Сервер временно недоступен.";
                    break;
                case 101:
                    res = "Сервер временно недоступен.";
                    break;
                case 102:
                    res = "Сервер временно недоступен.";
                    break;
                case 103:
                    res = "Сервер временно недоступен.";
                    break;
                case 104:
                    res = "Сервер временно недоступен.";
                    break;
                case 110:
                    res = "Неверный логин или пароль.";
                    break;
                case 200:
                    res = "Неверный логин или пароль.";
                    break;
                case 210:
                    res = "Сервер временно недоступен.";
                    break;
                case 401:
                    res = "Неверный логин или пароль.";
                    break;
                case 453:
                    res = "Сервер временно недоступен.";
                    break;
                case 900:
                    res = "Сервер временно недоступен.";
                    break;
                case 9999:
                    res = "Сервер временно недоступен.";
                    break;
            }
            return res;
        }
	}
}
