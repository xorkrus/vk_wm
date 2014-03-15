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
using System.Windows.Forms;

namespace Galssoft.VKontakteWM.Components.UI
{
    partial class UIViewBase
    {
        #region Release graphics

        protected static void ReleaseBitmap(IDisposable bitmap)
        {
            if (bitmap != null)
                bitmap.Dispose();
        }

        private void ReleaseGrapicsResources()
        {
            ReleaseBitmap(BackgroundImage);
            _backgroundImage = null;

            if (_canvas != null)
                _canvas.Dispose();
            _canvas = null;
        }

        #endregion

        /// <summary>
        /// Tile background image to client size
        /// </summary>
        protected Bitmap PrepareBackground(Bitmap bmp)
        {
            return Gdi.PrepareBackground(bmp, ClientSize.Width, ClientSize.Height);
        }

        /// <summary>
        /// Draw background in list
        /// </summary>
        /// <param name="gMem">Graphics</param>
        /// <param name="rListRect">Client rectangle to fill background</param>
        protected virtual void DrawBackground(Gdi gMem, Rectangle rListRect)
        {
            if (BackgroundImage != null)
                gMem.DrawImage(BackgroundImage, rListRect.X, rListRect.Y, rListRect.Width, rListRect.Height, rListRect.X, rListRect.Y);
            else
                gMem.FillRect(rListRect, BackColor);
        }





        public void UIInvalidate()
        {
            UIInvalidate(new Rectangle(0, 0, ClientSize.Width, ClientSize.Height));
        }

        public static void UIInvalidate(UIElementBase sender)
        {
            //Point origin = Point.Empty;
            //UIElementBase parent = sender;
            //while (!(parent is UIViewBase) && parent != null)
            //{
            //origin.X += parent.Left;
            //origin.Y += parent.Top;
            //parent = parent.Parent;
            //}
            //UIViewBase host = parent as UIViewBase;
            //if (host != null)
            //host.UIInvalidate(new Rectangle(origin.X, origin.Y, sender.Width, sender.Height));
        }

        private void UIInvalidate(Rectangle rect)
        {
            //PrepareDirtyRegion();
            //myDirtyRegion.Union(rect);
            Invalidate(rect);

            //// tell all child controls that are transparent and in this rect that they need to repaint
            //foreach (Control control in Controls)
            //{
            //    WindowlessControlHost wc = control as WindowlessControlHost;
            //    if (wc == null || wc.BackColor != Color.Transparent)
            //        continue;
            //    Rectangle intersect = new Rectangle(wc.Left, wc.Top, wc.Width, wc.Height);
            //    intersect.Intersect(rect);
            //    if (intersect.Width == 0 || intersect.Height == 0)
            //        continue;
            //    intersect.X -= wc.Left;
            //    intersect.Y -= wc.Top;
            //    wc.WindowlessInvalidate(intersect);
            //}
        }


        #region Events

        protected override void OnPaint(PaintEventArgs e)
        {
            IntPtr hdc = e.Graphics.GetHdc();
            try
            {
                using (Gdi gMem = Gdi.FromHdc(hdc, e.ClipRectangle)) //	 Rectangle.Empty
                {
                    OnRender(gMem, e.ClipRectangle);
                }
            }
            finally
            {
                e.Graphics.ReleaseHdc(hdc);
            }
        }

        protected virtual void OnRender(Gdi graphics, Rectangle clipRect)
        {
            IntPtr ptrSrc = OffscreenBuffer.OffScreenGraphics.GetHdc();

            using (Gdi gr = Gdi.FromHdc(ptrSrc, Rectangle.Empty))
            {
                DrawBackground(gr, clipRect);

                // Pass the graphics to the canvas to render
                if (Canvas != null)
                {
                    Canvas.Render(gr, clipRect);
                }
            }

            graphics.BitBlt(clipRect.Left, clipRect.Top, Width, Height, ptrSrc, clipRect.Left, clipRect.Top, TernaryRasterOperations.SRCCOPY);

            OffscreenBuffer.OffScreenGraphics.ReleaseHdc(ptrSrc);

            InvokeFirstRender();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // do nothing
        }

        #endregion
    }
}