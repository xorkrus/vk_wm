using System;
using System.Collections.Generic;
using System.IO;
using Galssoft.VKontakteWM.Components.Common.Interfaces;
using Galssoft.VKontakteWM.Components.Configuration;
using System.Xml.Serialization;
using Galssoft.VKontakteWM.Components.Common.Localization;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;

namespace Galssoft.VKontakteWM.Components.Cache
{
    /// <summary>
    /// Класс Сохранения и загрузки файлов с кэш директорию
    /// </summary>
    [Serializable]
    public static class Cache
    {
        private static string _storagePath = string.Empty;
        public static int FileUsingWriteTicks;
        public static int FileUsingReadTicks;

        /// <summary>
        /// Задание пути к папке с кэшами
        /// </summary>
        /// <param name="storagePath"></param>
        public static void InitPath(string storagePath)
        {
            _storagePath = storagePath;

            if (!Directory.Exists(SystemConfiguration.AppInstallPath + @"\Cache"))
            {
                Directory.CreateDirectory(SystemConfiguration.AppInstallPath + @"\Cache");
            }

            if (!Directory.Exists(SystemConfiguration.AppInstallPath + @"\Cache\Files"))
            {
                Directory.CreateDirectory(SystemConfiguration.AppInstallPath + @"\Cache\Files");
            }

            if (!Directory.Exists(SystemConfiguration.AppInstallPath + @"\Cache\Files\Thumb"))
            {
                Directory.CreateDirectory(SystemConfiguration.AppInstallPath + @"\Cache\Files\Thumb");
            }
        }

        /// <summary>
        /// Загрузка из кэша.
        /// </summary>
        /// <param name="folderName">Название папки</param>
        /// <param name="dataName">Название объекта</param>
        /// <returns>Загружаемого объекта</returns>
        public static T LoadFromCache<T>(string folderName, string dataName) where T : class, new()
        {
            if (string.IsNullOrEmpty(_storagePath))
                throw new CacheException("Storage path not initialized.");

            if (string.IsNullOrEmpty(dataName))
                throw new CacheException("Data name is null or empty.");

            try
            {
                string fullpath = _storagePath + (string.IsNullOrEmpty(folderName) ? "" : "\\" + folderName);
                List<Type> interfaces = new List<Type>(typeof (T).GetInterfaces());
                T obj = null;

                if (interfaces.Contains(typeof(ISelfDeserializer)))
                {
                    obj = new T();
                    ((ISelfDeserializer) obj).Deserialize(folderName, dataName);
                    return obj;
                }
                else
                {
                    using (StreamReader reader = new StreamReader(fullpath + "\\" + dataName + ".xml"))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(T));
                        int sTick = Environment.TickCount;
                        obj = (T)serializer.Deserialize(reader);
                        sTick = Environment.TickCount - sTick;
                        FileUsingReadTicks += sTick;
                        return obj;
                    }
                }
            }
            catch (IOException)
            {
                throw;
            }
            catch (Exception e)
            {
                DebugHelper.WriteLogEntry("Cache loading exception: " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// Сохранение в кэш.
        /// Не сериализуйте пустые объекты!!!
        /// Или учтите что при сериализации они пропускаются.
        /// </summary>
        /// <param name="serializedObject">Загружаемый объект</param>
        /// <param name="folderName">Название папки</param>
        /// <param name="dataName">Название объекта</param>
        public static bool SaveToCache(object serializedObject, string folderName, string dataName)
        {
            //Не реализована сериализация serializedObject типа ArrayOf (массивы типа object[] и List<>)
            if (string.IsNullOrEmpty(_storagePath))
                throw new CacheException("Storage path not initialized.");

            if (string.IsNullOrEmpty(dataName))
                throw new CacheException("Data name is null or empty.");

            try
            {
                string fullpath = _storagePath + (string.IsNullOrEmpty(folderName) ? "" : "\\" + folderName);
                string fileName = fullpath + "\\" + dataName + ".xml";

                if (!Directory.Exists(fullpath))
                    Directory.CreateDirectory(fullpath);

                if (serializedObject is ISelfDeserializer)
                {
                    ((ISelfDeserializer) serializedObject).Serialize(folderName, dataName);
                }
                else
                {
                    using (StreamWriter writer = new StreamWriter(fileName, false))
                    {
                        XmlSerializer serializer = new XmlSerializer(serializedObject.GetType());
                        int sTick = Environment.TickCount;
                        serializer.Serialize(writer, serializedObject);
                        sTick = Environment.TickCount - sTick;
                        FileUsingWriteTicks += sTick;
                    }                    
                }

                return true;
            }
            catch (IOException)
            {
                throw;
            }
            catch (Exception e)
            {
                DebugHelper.WriteLogEntry("Cache saving exception: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Получение списка сериализованных объектов.
        /// </summary>
        /// <param name="folderName">Название поддиректории</param>
        public static string[] GetEntryNames(string folderName)
        {
            if (string.IsNullOrEmpty(_storagePath))
                throw new CacheException("Storage path not initialized.");

            string fullpath = _storagePath + (string.IsNullOrEmpty(folderName) ? "" : "\\" + folderName);

            FileInfo[] fileInfos;
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(fullpath);
                fileInfos = dirInfo.GetFiles("*.xml");
            }
            catch(DirectoryNotFoundException)
            {
                return new string[0];
            }
            catch (IOException)
            {
                throw;
            }
            catch (Exception e)
            {
                DebugHelper.WriteLogEntry("Cache getting entries exception: " + e.Message);
                throw;
            }

            string[] files = new string[fileInfos.Length];
            for (int i = 0; i < fileInfos.Length; i++)
            {
                files[i] = fileInfos[i].Name.Substring(0, fileInfos[i].Name.LastIndexOf('.'));
            }

            return files;
        }

        /// <summary>
        /// Удаление записи из кэша.
        /// </summary>
        /// <param name="folderName">Название поддиректории</param>
        /// <param name="dataName">Название объекта</param>
        public static bool DeleteEntryFromCache(string folderName, string dataName)
        {
            if (string.IsNullOrEmpty(_storagePath))
                throw new CacheException("Storage path not initialized.");

            if (string.IsNullOrEmpty(dataName))
                throw new CacheException("Data name is null or empty.");

            string fullpath = _storagePath + (string.IsNullOrEmpty(folderName) ? "" : "\\" + folderName) +
                "\\" + dataName + ".xml";
            string folderpath = _storagePath + (string.IsNullOrEmpty(folderName) ? "" : "\\" + folderName) +
                "\\" + dataName;

            try
            {
                if (File.Exists(fullpath))
                {
                    File.Delete(fullpath);
                }
                else if (Directory.Exists(folderpath))
                {
                    Directory.Delete(folderpath, true);
                }
                return true;
            }
            catch (IOException)
            {
                return false;                
            }
            catch (Exception e)
            {
                DebugHelper.WriteLogEntry("Cache deleting entry exception: " + e.Message);
                return false;
            }

        }

    }
}
