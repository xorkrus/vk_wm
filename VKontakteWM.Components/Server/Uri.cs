using System;
using System.Collections.Generic;
using System.Text;
using Galssoft.VKontakteWM.Components.Configuration;

namespace Galssoft.VKontakteWM.Components.Server
{
    public sealed class Uri
    {
        /// <summary>
        /// Адрес API
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Метод предоставляемый API, например: /api/users/setStatus?
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Список передаваемых параметров, param=value
        /// </summary>
        public List<string> Parameters = new List<string>();

        /// <summary>
        /// Вычесляет сигнатуру запроса и добавляет ее к строке запроса
        /// </summary>
        /// <param name="useSessionSecretKey">Указывает на то, какой секретный ключ необходимо использовать (сессии или приложения)</param>
        /// <param name="sessionSecretKey">Секретный ключ сессии, необходимо передать, если useSessionSecretKey==true</param>
        /// <returns></returns>
        public string GetUri(/*bool useSessionSecretKey, string sessionSecretKey*/)
        {
            string res = this.Address + this.Method;
            string hash = "";

            bool firstParam = true;
            foreach (string param in this.Parameters)
            {
                if (firstParam)
                {
                    res += param;
                    firstParam = false;
                }
                else
                    res += "&" + param;

                hash += param;
            }
            /*
            if (!useSessionSecretKey)
            {
                //Использовать для подписи секретный ключ приложения
                hash += SystemConfiguration.SecretKey;
            }
            else
            {
                //Использовать для подписи секретный ключ сессии
                hash += sessionSecretKey;
            }
            //Сформировать MD5-хэш
            hash = HttpUtility.getMd5Hash(hash);
            */
            return res; //+"&sig="+hash;
        }
    }
}
