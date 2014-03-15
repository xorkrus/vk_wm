using System;

using System.Collections.Generic;
using System.Text;
using Galssoft.VKontakteWM.Components.GDI;

using OpenNETCF.Drawing;
using OpenNETCF.Drawing.Imaging;

using IBitmapImage = OpenNETCF.Drawing.Imaging.IBitmapImage;
using IImage = OpenNETCF.Drawing.Imaging.IImage;
using IImagingFactory = Galssoft.VKontakteWM.Components.GDI.IImagingFactory;
using ImageInfo = OpenNETCF.Drawing.Imaging.ImageInfo;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;

namespace Galssoft.VKontakteWM.Components.Common.SystemHelpers
{
    public static class ImageHelper
    {
        private static readonly IImagingFactory ImagingFactory = (IImagingFactory)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("327ABDA8-072B-11D3-9D7B-0000F81EF32E")));

        public static readonly int ImageMaxLinearSize = 800;
        private static readonly GDI.InterpolationHint DefaultInterpolation = Galssoft.VKontakteWM.Components.GDI.InterpolationHint.InterpolationHintBilinear;

        public static void GetImageSize(string file, out Size size)
        {
            GDI.IImage image;
            GDI.ImageInfo newImageInfo;
                      
            ImagingFactory.CreateImageFromFile(file, out image);

            image.GetImageInfo(out newImageInfo);

            size = new Size((int)newImageInfo.Width, (int)newImageInfo.Height);

            Marshal.FinalReleaseComObject(image);
        }

        /// <summary>
        /// Читает угол поворота из EXIF
        /// </summary>
        /// <param name="file"></param>
        /// <param name="exifRotationAngle"></param>
        public static void ReadRotationAngleFromEXIF(string file, out RotationAngle exifRotationAngle)
        {
            exifRotationAngle = RotationAngle.Zero;

            using (StreamOnFile newStreamOnFile = new StreamOnFile(file))
            {
                IImageDecoder newIImageDecoder;

                OpenNETCF.Drawing.Imaging.IImagingFactory newImagingFactory = new OpenNETCF.Drawing.Imaging.ImagingFactoryClass();

                int r = newImagingFactory.CreateImageDecoder(newStreamOnFile, DecoderInitFlag.DecoderInitFlagNone, out newIImageDecoder);

                // Ниже используется своя функция обработки EXIF, так как на Philips попытка проверить
                // jpeg из-за некорректных данных рушится в COM. Это случается из-за параметра DecoderInitFlag.DecoderInitFlagNone
                // ImageProperty[] items = ImageUtils.GetAllProperties(newIImageDecoder);
                ImageProperty[] items = GetAllPropertiesInternal(newIImageDecoder);
                if (items.Length == 0)
                {
                    newImagingFactory.CreateImageDecoder(newStreamOnFile, DecoderInitFlag.DecoderInitFlagBuiltIn1st, out newIImageDecoder);
                    items = GetAllPropertiesInternal(newIImageDecoder);
                }

                foreach (ImageProperty item in items)
                {
                    if (item.Id == ImageTag.TAG_ORIENTATION)
                    {
                        try
                        {
                            switch (Convert.ToInt16(item.GetValue()))
                            {
                                case 3:
                                    exifRotationAngle = RotationAngle.Clockwise180;
                                    break;

                                case 6:
                                    exifRotationAngle = RotationAngle.Clockwise90;
                                    break;

                                case 8:
                                    exifRotationAngle = RotationAngle.Clockwise270;
                                    break;

                                default:
                                    throw new Exception();
                                    break;
                            }
                        }
                        catch (Exception)
                        {
                            exifRotationAngle = RotationAngle.Zero;
                        }
                    }
                }
            }
        }

        private static ImageProperty[] GetAllPropertiesInternal(IImageDecoder decoder)
        {
            uint num;
            uint num2;
            if (decoder == null)
            {
                throw new ArgumentNullException();
            }
            List<ImageProperty> list = new List<ImageProperty>(ImageUtils.GetAllTags(decoder).Length);
            decoder.GetPropertySize(out num, out num2);

            // Если драйвер возвращает заведомо мало информации, то значит она битая и вытащить
            // оттуда всё равно ничего не вытащить. Число 30 взято исходя из опытных расчётов.
            // Выходим из процедуры, иначе вывалится ошибка COM.
            if (num < num2 * 30)
                return list.ToArray();

            IntPtr pItems = Marshal.AllocHGlobal((int)num);
            PropertyItem[] itemArray = new PropertyItem[num2];
            decoder.GetAllPropertyItems(num, num2, pItems);
            IntPtr ptr = pItems;
            for (uint i = 0; i < num2; i++)
            {
                itemArray[i] = (PropertyItem)Marshal.PtrToStructure(ptr, typeof(PropertyItem));
                ptr = (IntPtr)(ptr.ToInt32() + Marshal.SizeOf(typeof(PropertyItem)));
            }
            foreach (PropertyItem item in itemArray)
            {
                list.Add(ImageUtils.LoadProperty(item));
            }
            Marshal.FreeHGlobal(pItems);
            return list.ToArray();
        }

        //public static ImageProperty LoadProperty(PropertyItem item)
        //{
        //    ImageProperty property = new ImageProperty();
        //    property.Id = item.Id;
        //    property.Len = item.Len;
        //    property.Type = item.Type;
        //    if (property.Len > 0)
        //    {
        //        byte[] destination = new byte[item.Len];
        //        Marshal.Copy(item.Value, destination, 0, item.Len);
        //        property.FromByteArray(destination);
        //    }
        //    return property;
        //}



        //

        /// <summary>
        /// Вписывает в квадрат прямоугольник с соотношением сторон
        /// </summary>
        /// <param name="iSize"></param>
        /// <param name="maxLinearSize"></param>
        /// <param name="oSize"></param>
        public static void GetSizeByMaxLinearSize(Size iSize, int maxLinearSize, out Size oSize)
        {
            oSize = new Size(0, 0);

            int imageWidth = iSize.Width;
            int imageHeight = iSize.Height;

            float wa = (float)imageWidth / (float)maxLinearSize;
            float ha = (float)imageHeight / (float)maxLinearSize;

            float a = (float)imageWidth / (float)imageHeight;

            if (wa > ha)
            {
                oSize.Width = maxLinearSize;
                oSize.Height = (int)(maxLinearSize / a);
            }
            else
            {
                oSize.Height = maxLinearSize;
                oSize.Width = (int)(maxLinearSize * a);
            }
        }

        public static void GetSizeByMinLinearSize(Size iSize, int minLinearSize, out Size oSize)
        {
            oSize = new Size(0, 0);

            int imageWidth = iSize.Width;
            int imageHeight = iSize.Height;

            float wa = (float)imageWidth / (float)minLinearSize;
            float ha = (float)imageHeight / (float)minLinearSize;

            float a = (float)imageWidth / (float)imageHeight;

            if (wa < ha)
            {
                oSize.Width = minLinearSize;
                oSize.Height = (int)(minLinearSize / a);
            }
            else
            {
                oSize.Height = minLinearSize;
                oSize.Width = (int)(minLinearSize * a);
            }
        }

        /// <summary>
        /// Сохраняет данные IImage в файл
        /// </summary>
        /// <param name="image"></param>
        /// <param name="file"></param>
        public static void SaveImageFromMemory(GDI.IImage image, string file)
        {
            GDI.ImageInfo newImageInfo;
            GDI.IBitmapImage newIBitmapImage;
            Bitmap newBitmap;

            image.GetImageInfo(out newImageInfo);

            ImagingFactory.CreateBitmapFromImage(image, newImageInfo.Width, newImageInfo.Height, PixelFormatID.PixelFormat24bppRGB, DefaultInterpolation, out newIBitmapImage);

            newBitmap = ImageUtils.IBitmapImageToBitmap((IBitmapImage)newIBitmapImage);

            newBitmap.Save(file, ImageFormat.Jpeg);

            Marshal.FinalReleaseComObject(newIBitmapImage);
            newBitmap.Dispose();
        }

        /// <summary>
        /// Сохраняет в файл отмаштабированноое изображение
        /// </summary>
        /// <param name="iFile"></param>
        /// <param name="oFile"></param>
        /// <param name="iSize"></param>
        public static void SaveScaledImage(string iFile, string oFile, Size iSize)
        {
            GDI.IImage image;
            GDI.IBitmapImage newIBitmapImage;
            Bitmap newBitmap;

            ImagingFactory.CreateImageFromFile(iFile, out image);

            ImagingFactory.CreateBitmapFromImage(image, (uint)iSize.Width, (uint)iSize.Height, PixelFormatID.PixelFormat24bppRGB, DefaultInterpolation, out newIBitmapImage);

            newBitmap = ImageUtils.IBitmapImageToBitmap((IBitmapImage)newIBitmapImage);

            newBitmap.Save(oFile, ImageFormat.Jpeg);

            Marshal.FinalReleaseComObject(image);
            Marshal.FinalReleaseComObject(newIBitmapImage);
            newBitmap.Dispose();
        }

        /// <summary>
        /// Сохраняет в файл отмаштабированноое и развернутое на заданный угол изображение
        /// </summary>
        /// <param name="iFile"></param>
        /// <param name="oFile"></param>
        /// <param name="maxLinearSize"></param>
        /// <param name="angle"></param>
        public static void SaveScaledImage(string iFile, string oFile, int maxLinearSize, RotationAngle angle)
        {
            GDI.IImage image;
            GDI.IBitmapImage newIBitmapImage;
            Bitmap newBitmap;
            GDI.ImageInfo newImageInfo;

            ImagingFactory.CreateImageFromFile(iFile, out image);

            image.GetImageInfo(out newImageInfo);

            Size oSize;

            GetSizeByMaxLinearSize(new Size((int)newImageInfo.Width, (int)newImageInfo.Height), maxLinearSize, out oSize);

            ImagingFactory.CreateBitmapFromImage(image, (uint)oSize.Width, (uint)oSize.Height, PixelFormatID.PixelFormat24bppRGB, DefaultInterpolation, out newIBitmapImage);

            newBitmap = ImageUtils.IBitmapImageToBitmap((IBitmapImage)newIBitmapImage);

            if (angle != RotationAngle.Zero)
            {
                newBitmap = ImageUtils.Rotate(newBitmap, angle);
            }

            Marshal.FinalReleaseComObject(image);
            Marshal.FinalReleaseComObject(newIBitmapImage);

            newBitmap.Save(oFile, ImageFormat.Jpeg);

            newBitmap.Dispose();

            //throw new Exception();
        }

        public static void SaveSquareImage(string iFile, string oFile, int squareSize)
        {
            GDI.IImage image;
            GDI.IBitmapImage newIBitmapImage;
            Bitmap newBitmap;
            GDI.ImageInfo newImageInfo;
            Image resultImage;

            ImagingFactory.CreateImageFromFile(iFile, out image);

            image.GetImageInfo(out newImageInfo);

            Size oSize;

            GetSizeByMinLinearSize(new Size((int)newImageInfo.Width, (int)newImageInfo.Height), squareSize, out oSize);

            ImagingFactory.CreateBitmapFromImage(image, (uint)oSize.Width, (uint)oSize.Height, PixelFormatID.PixelFormat24bppRGB, DefaultInterpolation, out newIBitmapImage);

            newBitmap = ImageUtils.IBitmapImageToBitmap((IBitmapImage)newIBitmapImage);

            resultImage = new Bitmap(squareSize, squareSize);

            using (Graphics g = Graphics.FromImage(resultImage))
            {
                g.DrawImage(newBitmap, 0, 0);
            }            

            Marshal.FinalReleaseComObject(image);
            Marshal.FinalReleaseComObject(newIBitmapImage);
            
            resultImage.Save(oFile, ImageFormat.Jpeg);

            newBitmap.Dispose();
            resultImage.Dispose();
        }

        public static void CustomSaveScaledImage(string iFile, string oFile, int maxLinearSize, RotationAngle angle)
        {
            GDI.IImage image;
            GDI.IBitmapImage newIBitmapImage;
            Bitmap newBitmap;
            GDI.ImageInfo newImageInfo;

            ImagingFactory.CreateImageFromFile(iFile, out image);

            image.GetImageInfo(out newImageInfo);

            Size oSize;

            if (!(newImageInfo.Width <= maxLinearSize && newImageInfo.Height <= maxLinearSize))
                GetSizeByMaxLinearSize(new Size((int) newImageInfo.Width, (int) newImageInfo.Height), maxLinearSize, out oSize);
            else
                oSize = new Size((int)newImageInfo.Width, (int)newImageInfo.Height);

            ImagingFactory.CreateBitmapFromImage(image, (uint)oSize.Width, (uint)oSize.Height, PixelFormatID.PixelFormat24bppRGB, DefaultInterpolation, out newIBitmapImage);

            newBitmap = ImageUtils.IBitmapImageToBitmap((IBitmapImage)newIBitmapImage);

            if (angle != RotationAngle.Zero)
            {
                newBitmap = ImageUtils.Rotate(newBitmap, angle);
            }

            Marshal.FinalReleaseComObject(image);
            Marshal.FinalReleaseComObject(newIBitmapImage);

            newBitmap.Save(oFile, ImageFormat.Jpeg);

            newBitmap.Dispose();
        }

        /// <summary>
        /// Заключительная обработка избражения: вращение на заданный угол
        /// </summary>
        /// <param name="file"></param>
        /// <param name="angle"></param>
        public static void PostProcessImageFile(string file, RotationAngle angle)
        {
            GDI.IImage image;
            GDI.IBitmapImage newIBitmapImage;
            Bitmap newBitmap;
            GDI.ImageInfo newImageInfo;

            if (angle != RotationAngle.Zero)
            {
                ImagingFactory.CreateImageFromFile(file, out image);

                image.GetImageInfo(out newImageInfo);

                ImagingFactory.CreateBitmapFromImage(image, newImageInfo.Width, newImageInfo.Height, PixelFormatID.PixelFormat24bppRGB, DefaultInterpolation, out newIBitmapImage);

                newBitmap = ImageUtils.IBitmapImageToBitmap((IBitmapImage)newIBitmapImage);

                newBitmap = ImageUtils.Rotate(newBitmap, angle);

                Marshal.FinalReleaseComObject(image);

                newBitmap.Save(file, ImageFormat.Jpeg);

                Marshal.FinalReleaseComObject(image);
                Marshal.FinalReleaseComObject(newIBitmapImage);
                newBitmap.Dispose();
            }
        }

        /// <summary>
        /// Предварительная обработка изображения: мастабирование и вращение на угол, полученный из EXIF
        /// </summary>
        /// <param name="iFile"></param>
        /// <param name="oFile"></param>
        /// <param name="maxLinearSize"></param>
        public static void PreProcessImageFile(string iFile, string oFile, int maxLinearSize)
        {
            GDI.IImage image;
            GDI.ImageInfo newImageInfo;
            GDI.IBitmapImage newIBitmapImage;
            Bitmap newBitmap;

            ImagingFactory.CreateImageFromFile(iFile, out image);

            image.GetImageInfo(out newImageInfo);

            if (maxLinearSize > ImageMaxLinearSize)
            {
                maxLinearSize = ImageMaxLinearSize;
            }

            if ((int)Math.Max(newImageInfo.Width, newImageInfo.Height) > maxLinearSize)
            {
                Size oSize;

                GetSizeByMaxLinearSize(new Size((int)newImageInfo.Width, (int)newImageInfo.Height), maxLinearSize, out oSize);

                ImagingFactory.CreateBitmapFromImage(image, (uint)oSize.Width, (uint)oSize.Height, PixelFormatID.PixelFormat24bppRGB, DefaultInterpolation, out newIBitmapImage);
            }
            else
            {
                ImagingFactory.CreateBitmapFromImage(image, newImageInfo.Width, newImageInfo.Height, PixelFormatID.PixelFormat24bppRGB, DefaultInterpolation, out newIBitmapImage);
            }

            newBitmap = ImageUtils.IBitmapImageToBitmap((IBitmapImage)newIBitmapImage);

            RotationAngle angle;

            ImageHelper.ReadRotationAngleFromEXIF(iFile, out angle);

            if (angle != RotationAngle.Zero)
            {
                newBitmap = ImageUtils.Rotate(newBitmap, angle);
            }

            newBitmap.Save(oFile, ImageFormat.Jpeg);

            Marshal.FinalReleaseComObject(image);
            Marshal.FinalReleaseComObject(newIBitmapImage);
            newBitmap.Dispose();
        }

        /// <summary>
        /// Загрузка IImage из файла
        /// </summary>
        /// <param name="file"></param>
        /// <param name="image"></param>
        /// <param name="imageSize"></param>
        public static void LoadImageFromFile(string file, out GDI.IImage image, out Size imageSize)
        {
            GDI.ImageInfo newImageInfo;

            ImagingFactory.CreateImageFromFile(file, out image);

            image.GetImageInfo(out newImageInfo);

            imageSize = new Size((int)newImageInfo.Width, (int)newImageInfo.Height);
        }

        /// <summary>
        /// Загрузка IImage из файла
        /// </summary>
        /// <param name="file"></param>
        /// <param name="image"></param>
        public static void LoadImageFromFile(string file, out GDI.IImage image)
        {
            ImagingFactory.CreateImageFromFile(file, out image);
        }

        /// <summary>
        /// Загрузка IImage из файла в память (без привязки к файлу)
        /// </summary>
        /// <param name="file"></param>
        /// <param name="image"></param>
        /// <param name="imageSize"></param>
        public static void LoadImageFromFileIntoMemory(string file, out GDI.IImage image, out Size imageSize)
        {
            GDI.ImageInfo newImageInfo;

            FileStream newFileStream = new FileStream(file, FileMode.Open);
            MemoryStream newMemoryStream = new MemoryStream((int)newFileStream.Length);

            CopyStream(newFileStream, newMemoryStream);

            byte[] buffer = newMemoryStream.GetBuffer();

            IntPtr bytes = Marshal.AllocHGlobal(buffer.Length);
            Marshal.Copy(buffer, 0, bytes, buffer.Length);
            ImagingFactory.CreateImageFromBuffer(bytes, (uint)buffer.Length, GDI.BufferDisposalFlag.BufferDisposalFlagGlobalFree, out image);

            newFileStream.Close();

            image.GetImageInfo(out newImageInfo);

            imageSize = new Size((int)newImageInfo.Width, (int)newImageInfo.Height);            
        }

        /// <summary>
        /// Загрузка IImage из файла в память (без привязки к файлу)
        /// </summary>
        /// <param name="file"></param>
        /// <param name="image"></param>
        public static void LoadImageFromFileIntoMemory(string file, out GDI.IImage image)
        {
            FileStream newFileStream = new FileStream(file, FileMode.Open);
            MemoryStream newMemoryStream = new MemoryStream((int)newFileStream.Length);

            CopyStream(newFileStream, newMemoryStream);

            byte[] buffer = newMemoryStream.GetBuffer();

            IntPtr bytes = Marshal.AllocHGlobal(buffer.Length);
            Marshal.Copy(buffer, 0, bytes, buffer.Length);
            ImagingFactory.CreateImageFromBuffer(bytes, (uint)buffer.Length, GDI.BufferDisposalFlag.BufferDisposalFlagGlobalFree, out image);

            newFileStream.Close();
        }

        /// <summary>
        /// Копирование из потока в поток
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
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
    }
}
