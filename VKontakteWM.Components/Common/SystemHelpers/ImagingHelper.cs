using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Galssoft.VKontakteWM.Components.GDI;
using OpenNETCF.Drawing;
using OpenNETCF.Drawing.Imaging;
using BufferDisposalFlag = OpenNETCF.Drawing.Imaging.BufferDisposalFlag;
using IBitmapImage = OpenNETCF.Drawing.Imaging.IBitmapImage;
using IImage = OpenNETCF.Drawing.Imaging.IImage;
using IImagingFactory = Galssoft.VKontakteWM.Components.GDI.IImagingFactory;
using ImageInfo = OpenNETCF.Drawing.Imaging.ImageInfo;

namespace Galssoft.VKontakteWM.Components.Common.SystemHelpers
{
    public static class ImagingHelper
    {
        private static readonly IImagingFactory ImagingFactory =
            (IImagingFactory) Activator.CreateInstance(Type.GetTypeFromCLSID(
                new Guid("327ABDA8-072B-11D3-9D7B-0000F81EF32E")));

        /// <summary>
        /// Создание изображения из файла
        /// </summary>
        /// <param name="originalPath"></param>ompa
        /// <returns></returns>
        public static IImage CreateIImageFromFile(string originalPath)
        {
            GDI.IImage originalImage;
            GDI.ImageInfo imageInfo;
            ImagingFactory.CreateImageFromFile(originalPath, out originalImage);
            originalImage.GetImageInfo(out imageInfo);
            var cashedImage = (GDI.IImage) ScaleIImage((IImage) originalImage, 800);
            Marshal.FinalReleaseComObject(originalImage);
            return (IImage) cashedImage;
        }

        /// <summary>
        /// Создание Bitmap из IImage
        /// </summary>
        /// <param name="img">Исходное изображение IImage</param>
        /// <returns>Bitmap</returns>
        public static Bitmap CreateBitmapFromIImage(IImage img)
        {
            GDI.IBitmapImage bmp;
            ImageInfo ii;
            img.GetImageInfo(out ii);
            ImagingFactory.CreateBitmapFromImage((GDI.IImage) img, ii.Width, ii.Height, PixelFormatID.PixelFormat24bppRGB, GDI.InterpolationHint.InterpolationHintBilinear, out bmp);
            return ImageUtils.IBitmapImageToBitmap((IBitmapImage) bmp);
        }

        /// <summary>
        /// Обрезка IImage прямоугольной областью
        /// </summary>
        /// <param name="img"></param>
        /// <param name="destRect"></param>
        /// <returns></returns>
        public static GDI.IImage CropIImage(GDI.IImage img, Rectangle destRect)
        {
            GDI.IImage retImg;
            
            var srcBmp = CreateBitmapFromIImage((IImage) img);
            var destBmp = new Bitmap(destRect.Width, destRect.Height);

            var gR = Graphics.FromImage(destBmp);
                gR.DrawImage(srcBmp, 0, 0, destRect, GraphicsUnit.Pixel);
            gR.Dispose();
            srcBmp.Dispose();

            var mStream = new MemoryStream();
            destBmp.Save(mStream, ImageFormat.Png);
            destBmp.Dispose();

            var bufSize = (uint) mStream.Length;
            byte[] buffer = mStream.GetBuffer();

            IntPtr bytes = Marshal.AllocHGlobal(buffer.Length);
            Marshal.Copy(buffer, 0, bytes, buffer.Length);
            ImagingFactory.CreateImageFromBuffer(bytes, (uint) buffer.Length, GDI.BufferDisposalFlag.BufferDisposalFlagGlobalFree, out retImg);

            return retImg;
        }

        /// <summary>
        /// Вращение изображения
        /// </summary>
        /// <param name="originalImage"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static IImage SaveRortatedIImage(IImage originalImage, RotationAngle angle)
        {
            GDI.IImage rThumbImage;
            ImageInfo imageInfo;
            GDI.IBitmapImage iBitmapImage;
            originalImage.GetImageInfo(out imageInfo);
            uint width = imageInfo.Width;
            uint height = imageInfo.Height;

            ImagingFactory.CreateBitmapFromImage((GDI.IImage) originalImage, width, height,
                                                 PixelFormatID.PixelFormat24bppRGB,
                                                 GDI.InterpolationHint.InterpolationHintDefault,
                                                 out iBitmapImage);
            Bitmap rotatedBitmap = ImageUtils.Rotate(ImageUtils.IBitmapImageToBitmap(iBitmapImage as IBitmapImage), angle);
            var mStream = new MemoryStream();
            rotatedBitmap.Save(mStream, ImageFormat.Jpeg);
            rotatedBitmap.Dispose();
            var bufSize = (uint) mStream.Length;
            byte[] buffer = mStream.GetBuffer();

            IntPtr bytes = Marshal.AllocHGlobal(buffer.Length);
            Marshal.Copy(buffer, 0, bytes, buffer.Length);
            ImagingFactory.CreateImageFromBuffer(bytes, (uint) buffer.Length, (GDI.BufferDisposalFlag) BufferDisposalFlag.BufferDisposalFlagGlobalFree, out rThumbImage);

            Marshal.FinalReleaseComObject(originalImage);
            return (IImage) rThumbImage;
        }

        /// <summary>
        /// Масштабирование изображения
        /// </summary>
        /// <param name="originalImage"></param>
        /// <param name="maxSize">Размер стороны квадрата в который нужно вписать изображение</param>
        /// <returns></returns>
        public static IImage ScaleIImage(IImage originalImage, uint maxSize)
        {
            IImage thumbImage;
            ImageInfo imageInfo;

            originalImage.GetImageInfo(out imageInfo);

            uint width = imageInfo.Width;
            uint height = imageInfo.Height;

            if (height > maxSize && width < maxSize ||
               (height > maxSize && width > maxSize && height > width))
            {
                width = (width * maxSize) / height;
                height = maxSize;
            }
            else if (height < maxSize && width > maxSize ||
                    (height > maxSize && width > maxSize && height < width))
            {
                height = (height * maxSize) / width;
                width = maxSize;
            }
            else if (height > maxSize && width > maxSize && height == width)
            {
                height = width = maxSize;
            }

            originalImage.GetThumbnail(width, height, out thumbImage);
            return thumbImage;
        }

        /// <summary>
        /// Сохранение изображения
        /// </summary>
        /// <param name="image"></param>
        /// <param name="fileName"></param>
        public static void SaveIImage(IImage image, string fileName)
        {
            ImageInfo imageInfo;
            GDI.IBitmapImage iBitmapImage;
            image.GetImageInfo(out imageInfo);
            uint width = imageInfo.Width;
            uint height = imageInfo.Height;
            ImagingFactory.CreateBitmapFromImage((GDI.IImage)image, width, height,
                                                 PixelFormatID.PixelFormat24bppRGB,
                                                 GDI.InterpolationHint.InterpolationHintDefault,
                                                 out iBitmapImage);
            Bitmap bitmap = ImageUtils.IBitmapImageToBitmap((IBitmapImage) iBitmapImage);
            bitmap.Save(fileName, ImageFormat.Jpeg);
            bitmap.Dispose();
        }
    }
}
