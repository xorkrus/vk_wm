using System;
using Microsoft.Win32;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;

namespace Galssoft.VKontakteWM.Components.SystemHelpers
{
    public sealed class RegistryUtility
    {
        /// <summary>
        /// Запись значения параметра в реестр
        /// </summary>
        /// <param name="keyName">Название параметра </param>
        /// <param name="value"></param>
        public static void SetValue(string keyName, string path, string value, RegistryValueKind registryValueKind)
        {
            RegistryKey regKey = Registry.CurrentUser.CreateSubKey("software\\" + path);

            if (regKey == null)
            {
                DebugHelper.WriteLogEntry("Не удалось сохранить значение " + keyName + " в реестр");
            }
            else
            {
                regKey.SetValue(keyName, value, registryValueKind);
                regKey.Close();
            }
        }



        /// <summary>
        /// Записывает в реестр бинарные данные
        /// </summary>
        /// <param name="keyName">Имя ключа</param>
        /// <param name="path">Путь к ключу (HKCU\\software\\)</param>
        /// <param name="value">Значение</param>
        /// <param name="registryValueKind">Тип</param>
        public static void SetBinaryValue(string keyName, string path, object value, RegistryValueKind registryValueKind)
        {
            RegistryKey regKey = Registry.CurrentUser.CreateSubKey("software\\" + path);

            if (regKey == null)
            {
                DebugHelper.WriteLogEntry("Не удалось сохранить значение " + keyName + " в реестр");
            }
            else
            {
                regKey.SetValue(keyName, value, registryValueKind);
                regKey.Close();
            }
        }

        /// <summary>
        /// Получение значения параметра из реестра
        /// </summary>
        /// <param name="keyName">Название параметра </param>
        /// <returns></returns>
        public static string GetValue(string keyName, string path)
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("software\\" + path);

            if (regKey == null)
            {
                return "";
            }
            else
            {
                //Если существует параметр
                if (regKey.GetValue(keyName) != null)
                {
                    string res = regKey.GetValue(keyName).ToString();
                    return res;
                }
                regKey.Close();
                return "";
            }
        }

        /// <summary>
        /// Получение бинарного значения из реестра
        /// </summary>
        /// <param name="keyName">Имя ключа</param>
        /// <param name="path">Путь к ключу (HKCU\\software\\)</param>
        /// <returns></returns>
        public static object GetBinaryValue(string keyName, string path)
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("software\\" + path);

            if (regKey == null)
            {
                return null;
            }
            else
                //Если существует путь
                return regKey.GetValue(keyName);
        }

        /// <summary>
        /// Удаляет ветку из реестра
        /// </summary>
        /// <param name="path">Путь к удаляемой ветке</param>
        public static void ClearRegistryKey(string path)
        {
            //string subKey = "software\\" + path;
            //RegistryKey rk = Registry.CurrentUser;
            //RegistryKey sk1 = rk.OpenSubKey(subKey);
            try
            {
                Registry.CurrentUser.DeleteSubKey("software\\" + path, false);
                //if (sk1 != null)
                //rk.DeleteSubKeyTree(subKey);
            }
            catch (Exception e)
            {
                DebugHelper.WriteLogEntry("Regestry subkey delete exception: " + e.Message);
            }

        }
    }
}
