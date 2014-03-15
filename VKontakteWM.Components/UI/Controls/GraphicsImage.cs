/*	 This file was created by GALS Soft	company
 *	 http://www.galssoft.com
 *	 
 *	 Project name is Odnoklassniki Mobile
 *	 Platform is .Net Compact Framework 2.0 for Windows Mobile 5.0+ 
 * 
 *	 Copyright (c) 2009-2010 GALS Soft
 */

using System;
using System.Drawing;
using Galssoft.VKontakteWM.Components.GDI;

namespace Galssoft.VKontakteWM.Components.UI.Controls
{
    public class GraphicsImage : UIControl
    {
        public GraphicsImage()
        {
        }

        public GraphicsImage(IImage alphaChannelImage, bool stretch)
        {
            AlphaChannelImage = alphaChannelImage;
            _stretch = stretch;
            //left = location.Left;
            //top = location.Top;
            //width = location.Width;
            //height = location.Height;
        }

        #region Public properties

        public bool Stretch
        {
            get { return _stretch; }
            set { _stretch = value; }
        }
        private bool _stretch = false;

        /// <summary>
        /// Image with alpha channel
        /// </summary>
        public IImage AlphaChannelImage
        {
            get { return _alphaChannelImage; }
            set 
            { 
                _alphaChannelImage = value;

                ImageInfo img;
                value.GetImageInfo(out img);
                this.Size = new Size((int)img.Width, (int)img.Height);
            }
        }
        private IImage _alphaChannelImage = null;

        /// <summary>
        /// Image with transparent color
        /// </summary>
        public ImageData TransparentImage
        {
            get { return _transparentImage; }
            set 
            { 
                _transparentImage = value;

                this.Size = new Size(value.ImageSize.Width, value.ImageSize.Height);
            }
        }
        private ImageData _transparentImage = null;

        #endregion
		
        #region Actions

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }

        #endregion

        #region Drawing

        protected override void OnRender(Gdi gMem, Rectangle clipRect)
        {
            if (AlphaChannelImage != null)
            {
                if (Stretch)
                    gMem.DrawImageAlphaChannel(AlphaChannelImage, clipRect);
                else
                    gMem.DrawImageAlphaChannel(AlphaChannelImage, clipRect.Left, clipRect.Top);
            }
            else if (TransparentImage != null)
            {
                int imgWidth = TransparentImage.ImageSize.Width;
                int imgHeight = TransparentImage.ImageSize.Height;
                int destWidth = imgWidth;
                int destHeight = imgHeight;

                if (Stretch)
                {
                    destWidth = clipRect.Width;
                    destHeight = clipRect.Height;
                }

                gMem.TransparentImage(clipRect.Left, clipRect.Top,
                                      destWidth,
                                      destHeight,
                                      TransparentImage.ImageHandle,
                                      TransparentImage.ImageOffset.X,
                                      TransparentImage.ImageOffset.Y,
                                      imgWidth,
                                      imgHeight,
                                      TransparentImage.TransparentColor);
            }
        }

        #endregion

        #region IDisposable Members

        #endregion

    }
}