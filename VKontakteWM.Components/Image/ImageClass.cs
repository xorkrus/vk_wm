using System;

using System.Collections.Generic;
using System.Text;

using System.IO;
using OpenNETCF.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Galssoft.VKontakteWM.Components.ImageClass
{
    public static class ImageClass
    {
        public static bool ShrinkToMaxLinearSize(string fileName, uint maxLinearSize)
        {
            IImagingFactory newIImagingFactory = new ImagingFactoryClass();
            IImage newIImage = null;
            IImage newIImageThumb = null;
            ImageInfo newImageInfo;

            uint imageThumbWidth = 0;
            uint imageThumbHeight = 0;

            if (!File.Exists(fileName))
            {
                return false;
            }

            try
            {
                using (var newStreamOnFile = new StreamOnFile(fileName))
                {
                    newIImagingFactory.CreateImageFromStream(newStreamOnFile, out newIImage);

                    newIImage.GetImageInfo(out newImageInfo);

                    uint imageWidth = newImageInfo.Width;
                    uint imageHeight = newImageInfo.Height;

                    if (imageWidth > maxLinearSize || imageHeight > maxLinearSize)
                    {
                        float wa = (float)imageWidth / (float)maxLinearSize;
                        float ha = (float)imageHeight / (float)maxLinearSize;

                        float a = (float)imageWidth / (float)imageHeight;

                        if (wa > ha)
                        {
                            imageThumbWidth = maxLinearSize;
                            imageThumbHeight = (uint)(maxLinearSize / a);
                        }
                        else
                        {
                            imageThumbHeight = maxLinearSize;
                            imageThumbWidth = (uint)(maxLinearSize * a);
                        }

                        newIImage.GetThumbnail(imageThumbWidth, imageThumbHeight, out newIImageThumb);

                        Marshal.FinalReleaseComObject(newIImage);
                    }
                }
            }
            catch
            {
                return false;
            }

            try
            {
                File.Delete(fileName);

                IBitmapImage newIBitmapImage;
                newIImagingFactory.CreateBitmapFromImage(newIImageThumb, imageThumbWidth, imageThumbHeight, newImageInfo.PixelFormat, InterpolationHint.InterpolationHintBilinear, out newIBitmapImage);
                Bitmap newBitmap = ImageUtils.IBitmapImageToBitmap(newIBitmapImage);

                newBitmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static bool GetImageSize(IImage inputIImage, out Size resultSize)
        {
            resultSize = new Size(0, 0);

            IImagingFactory newIImagingFactory = new ImagingFactoryClass();
            ImageInfo newImageInfo;

            try
            {
                inputIImage.GetImageInfo(out newImageInfo);

                resultSize = new Size((int)newImageInfo.Width, (int)newImageInfo.Height);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
