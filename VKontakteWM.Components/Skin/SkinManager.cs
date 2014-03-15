/*	 This file was created by GALS Soft	company
 *	 http://www.galssoft.com
 *	 
 *	 Project name is VKontakte Mobile
 *	 Platform is .Net Compact Framework 2.0 for Windows Mobile 5.0+ 
 * 
 *	 Copyright (c) 2009-2010 GALS Soft
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.GDI;

namespace Galssoft.VKontakteWM.Components.Skin
{
    /// <summary>
    /// Класс управления скинами - наборами картинок для приложения
    /// </summary>
    public class SkinManager : IDisposable
    {
        private class EntryPoint
        {
            public int Start { get; set; }
            public int Lenght { get; set; }
        }

        private bool _skinInitialized;
        private string _skinName;
        private string _skinFolderPath;
        private string _subDataPrefix;
        private bool _haveFolder;
        private bool _haveFile;
        private Dictionary<string, IImage> _imageCache;
        private readonly IImagingFactory _imagingFactory;
        private Dictionary<string, EntryPoint> _imageList;

        private static string _wrongDPI;
        private static string _wrongVersion;

        public SkinManager(string skinFolderPath, int dpi)
        {
            if (string.IsNullOrEmpty(skinFolderPath))
                skinFolderPath = Configuration.SystemConfiguration.AppInstallPath;

            _imageCache = new Dictionary<string, IImage>();
            _skinFolderPath = skinFolderPath;
            _subDataPrefix = dpi < 192 ? "96" : "192";
            _skinInitialized = false;
            _imagingFactory = (IImagingFactory)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("327ABDA8-072B-11D3-9D7B-0000F81EF32E")));
            _imageList = new Dictionary<string, EntryPoint>();
        }

        /// <summary>
        /// Инициализация ресурсных строк
        /// </summary>
        public static void InitialiseStrings(string wrongDPI, string wrongVersion)
        {
            _wrongDPI = wrongDPI;
            _wrongVersion = wrongVersion;
        }

        /// <summary>
        /// Получение доступных скинов
        /// </summary>
        public string[] Skins
        {
            get
            {
                List<string> skins = new List<string>();
                string[] files = Directory.GetFiles(_skinFolderPath, "*.skn");
                foreach (string file in files)
                {
                    if (CheckSkinFile(file))
                    {
                        string fileName = (new FileInfo(file)).Name;
                        string skinname = fileName.Substring(fileName.LastIndexOf('.'));
                        skins.Add(skinname);
                    }
                }

                string[] directories = Directory.GetDirectories(_skinFolderPath);
                foreach (string directory in directories)
                {
                    string dir = (new DirectoryInfo(directory)).Name;
                    if (!skins.Contains(dir))
                        skins.Add(dir);
                }

                return skins.ToArray();
            }
        }

        public bool Initialized
        {
            get { return _skinInitialized; }
        }

        /// <summary>
        /// Используемое мэнеджером DPI
        /// </summary>
        public string SubDataPrefix
        {
            get
            {
                return _subDataPrefix;
            }
        }

        /// <summary>
        /// загрузка скина
        /// </summary>
        /// <param name="skinName">Название скина</param>
        /// <returns>True если скин удачно загрузился</returns>
        public bool LoadSkin(string skinName)
        {
            if (string.IsNullOrEmpty(skinName))
                return false;

            _haveFolder = false;
            _haveFile = false;
            _imageList = new Dictionary<string, EntryPoint>();
            ReleaseImages();

            _skinInitialized = false;
            _skinName = skinName;
            string skinFilePath = _skinFolderPath + "\\" + skinName + ".skn";
            if (File.Exists(skinFilePath))
            {
                if (!CheckSkinFile(skinFilePath))
                {
                    _skinInitialized = false;
                    return false;
                }
                _skinInitialized = true;
                ParseSkinFile(skinFilePath);
                _haveFile = true;
            }

            if (Directory.Exists(_skinFolderPath + "\\" + skinName))
            {
                _skinInitialized = true;
                _haveFolder = true;
            }

            return _skinInitialized;
        }

        /// <summary>
        /// Получение IImage по имени
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns></returns>
        public IImage GetImage(string imageName)
        {
            IImage img = GetImageFromCache(imageName);
            if (img == null)
            {
                img = GetImageFromFileSystem(imageName);
                _imageCache.Add(imageName, img);
            }

            return img;
        }

        /// <summary>
        /// COM освобождение IImage
        /// </summary>
        /// <param name="imageName"></param>
        public void ReleaseImage(string imageName)
        {
            if (_imageCache.ContainsKey(imageName))
            {
                Marshal.ReleaseComObject(_imageCache[imageName]);
                _imageCache.Remove(imageName);
            }
        }

        private IImage GetImageFromFileSystem(string imageName)
        {
            IImage img = null;
            if (_haveFolder)
                img = GetImageFromSkinFolder(imageName);
            if (img == null && _haveFile)
                img = GetImageFromSkinFile(imageName);
            return img;
        }

        private bool CheckSkinFile(string path)
        {
            try
            {
                using (FileStream skinFile = File.OpenRead(path))
                {
                    byte[] arr = new byte[sizeof(Int32)];
                    skinFile.Read(arr, 0, arr.Length);
                    byte[] ver = new byte[sizeof(Int32)];
                    skinFile.Read(ver, 0, ver.Length);
                    int version = BitConverter.ToInt32(ver, 0);
                    int dpi = BitConverter.ToInt32(arr, 0);

                    if ((dpi == 192 && UISettings.ScreenDPI <= 96) ||
                        (dpi == 96 && UISettings.ScreenDPI >= 192))
                    {
                        MessageBox.Show(/*_wrongDPI*/"Фиговое DPI");
                        return false;
                    }
                    if (version != 1)
                    {
                        MessageBox.Show(/*_wrongVersion*/"Фиговая версия");
                        return false;
                    }
                }
            }
            catch (OutOfMemoryException)
            {
                throw;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private void ParseSkinFile(string path)
        {
            try
            {
                using (FileStream skinFile = File.OpenRead(path))
                {
                    byte[] arr = new byte[sizeof(Int32)];
                    skinFile.Read(arr, 0, arr.Length);
                    int dpi = BitConverter.ToInt32(arr, 0);
                    byte[] ver = new byte[sizeof(Int32)];
                    skinFile.Read(ver, 0, ver.Length);
                    int version = BitConverter.ToInt32(ver, 0);

                    while (skinFile.CanRead)
                    {
                        byte[] array = new byte[sizeof(Int32)];
                        int z = skinFile.Read(array, 0, array.Length);
                        //z == 0 EndOfFile
                        if (z == 0)
                            break;
                        int s = BitConverter.ToInt32(array, 0);
                        array = new byte[s];
                        skinFile.Read(array, 0, s);
                        System.Text.ASCIIEncoding enc = new ASCIIEncoding();
                        string name = enc.GetString(array, 0, array.Length);
                        array = new byte[sizeof(Int32)];
                        skinFile.Read(array, 0, array.Length);
                        s = BitConverter.ToInt32(array, 0);
                        array = new byte[s];

                        long stIndex = skinFile.Position;
                        skinFile.Read(array, 0, s);

                        EntryPoint pt = new EntryPoint()
                        {
                            Lenght = s,
                            Start = (int)stIndex
                        };

                        _imageList.Add(name, pt);
                    }
                }
            }
            catch (OutOfMemoryException)
            {
                throw;
            }
            catch (Exception)
            {
                _skinInitialized = false;
                throw;
            }
        }

        private IImage GetImageFromSkinFile(string imageName)
        {
            //Если не получилось получить файл со скина
            // throw new Exception
            if (!_imageList.ContainsKey(imageName))
                return null;

            try
            {
                using (FileStream skinFile = File.OpenRead(_skinFolderPath + "\\" + _skinName + ".skn"))
                {
                    EntryPoint pt = _imageList[imageName];
                    skinFile.Position = pt.Start;
                    byte[] array = new byte[pt.Lenght];
                    skinFile.Read(array, 0, pt.Lenght);
                    skinFile.Close();
                    IImage img;

                    IntPtr bytes = Marshal.AllocHGlobal(array.Length);
                    Marshal.Copy(array, 0, bytes, array.Length);
                    _imagingFactory.CreateImageFromBuffer(bytes, (uint)array.Length,
                                                          BufferDisposalFlag.BufferDisposalFlagNone, out img);
                    return img;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private IImage GetImageFromSkinFolder(string imageName)
        {
            try
            {
                string skinFolderPath = _skinFolderPath + "\\" + _skinName + "\\" + _subDataPrefix;
                string[] files = null;
                if (Directory.Exists(skinFolderPath))
                    files = Directory.GetFiles(skinFolderPath, imageName + ".*");
                if (files != null && files.Length > 0)
                {
                    IImage img;
                    _imagingFactory.CreateImageFromFile(files[0], out img);
                    return img;
                }
                else
                {
                    skinFolderPath = _skinFolderPath + "\\" + _skinName;
                    files = Directory.GetFiles(skinFolderPath, imageName + ".*");
                    if (files.Length > 0)
                    {
                        IImage img;
                        _imagingFactory.CreateImageFromFile(files[0], out img);
                        return img;
                    }
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

        private IImage GetImageFromCache(string imageName)
        {
            if (_imageCache.ContainsKey(imageName))
            {
                IImage img = _imageCache[imageName];
                if (img == null)
                    _imageCache.Remove(imageName);
                return img;
            }
            return null;
        }

        private void ReloadImageCache()
        {
            Dictionary<string, IImage> imageCacheTemp = new Dictionary<string, IImage>();

            foreach (KeyValuePair<string, IImage> kvp in _imageCache)
            {
                Marshal.ReleaseComObject(kvp.Value);
                imageCacheTemp.Add(kvp.Key, GetImageFromFileSystem(kvp.Key));
            }

            _imageCache.Clear();
            _imageCache = imageCacheTemp;
        }

        private void ReleaseImages()
        {
            foreach (KeyValuePair<string, IImage> kvp in _imageCache)
            {
                if (kvp.Value != null)
                    Marshal.ReleaseComObject(kvp.Value);
            }
            _imageCache.Clear();
        }

        #region IDisposable Members

        public void Dispose()
        {
            ReleaseImages();
        }

        #endregion
    }
}