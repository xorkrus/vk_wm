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

namespace Galssoft.VKontakteWM.Components.UI.CompoundControls
{
    public abstract class KineticControl : KineticControlBase
    {
        public KineticControl(KineticControlScrollType scrollType)
            : base(scrollType)
        { }

        public KineticControl(KineticControlScrollType scrollType, bool retainOffsetBmpOnDispose)
            : base(scrollType, retainOffsetBmpOnDispose)
        { }

        /// <summary>
        /// Image for list background
        /// </summary>
        public Bitmap BackgroundImage
        {
            get { return _backgroundImage; }
            set
            {
                Bitmap image = PrepareBackground(value);
                if (_backgroundImage != image)
                {
                    ReleaseBitmap(_backgroundImage);
                    _backgroundImage = image;
                }
            }
        }

        private Bitmap _backgroundImage = null;
        private IImage _backgroundGImage = null;

        /// <summary>
        /// Image for list background
        /// </summary>
        public IImage BackgroundIImage
        {
            get { return _backgroundGImage; }
            set
            {
                _backgroundGImage = value;
                if (_backgroundGImage != null)
                {
                    ImageInfo ii;
                    value.GetImageInfo(out ii);
                    Bitmap img = new Bitmap((int)ii.Width, (int)ii.Height);
                    using (Graphics gr = Graphics.FromImage(img))
                    {
                        IntPtr hdc = gr.GetHdc();
                        Rectangle rect = new Rectangle(0, 0, img.Width, img.Height);
                        _backgroundGImage.Draw(hdc, ref rect, IntPtr.Zero);
                        gr.ReleaseHdc(hdc);
                    }
                    BackgroundImage = img;
                }
                else
                {
                    if (BackgroundImage != null)
                        BackgroundImage.Dispose();
                    BackgroundImage = null;
                }
            }
        }

        /// <summary>
        /// Tile background image to client size
        /// </summary>
        protected Bitmap PrepareBackground(Bitmap bmp)
        {
            return Gdi.PrepareBackground(bmp, ClientSize.Width, ClientSize.Height);
        }

        /// <summary>
        /// Title of the list
        /// </summary>
        public virtual string Title
        {
            get
            {
                return String.Empty;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // prepare background
            if (BackgroundIImage != null)
                BackgroundIImage = BackgroundIImage;

            //if (ShouldReloadOnResize())
            //    SetItems(BuildNativeControlItems(), _SelectedIndex, true);
        }

        #region Drawing Methods

        /// <summary>
        /// Draw background in list
        /// </summary>
        /// <param name="gMem">Graphics</param>
        /// <param name="rListRect">Client rectangle to fill background</param>
        /// <param name="offset">Offset for the beginning of the item list</param>
        protected virtual void DrawBackground(Gdi gMem, Rectangle rListRect, int offset)
        {
            if (BackgroundImage != null)
                gMem.DrawImage(BackgroundImage, 0, 0, rListRect.Width, rListRect.Height);
            else
                if (BackColor != Color.Empty)
                    gMem.FillRect(rListRect, BackColor);
        }

        #endregion

        /// <summary>
        /// Display list from the first item
        /// </summary>
        public void ScrollToStart()
        {
            ScrollTo(MinScrollPosition);
        }

        /// <summary>
        /// Display list to the latest item
        /// </summary>
        public void ScrollToEnd()
        {
            ScrollTo(MaxScrollPosition);
        }

        public virtual void Deactivate()
        {
        }

        protected static void ReleaseBitmap(IDisposable bitmap)
        {
            if (bitmap != null)
                bitmap.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            ReleaseBitmap(BackgroundImage);
            _backgroundImage = null;
            base.Dispose(disposing);
        }

    }
}