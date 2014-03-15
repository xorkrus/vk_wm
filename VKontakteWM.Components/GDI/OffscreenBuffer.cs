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
using System.Windows.Forms;

namespace Galssoft.VKontakteWM.Components.GDI
{
    public static class OffscreenBuffer
    {
        private static Bitmap _offScreenBmp;
        private static Graphics _offScreenGraphics;

        /// <summary>
        /// Get offscreen buffer to draw 
        /// </summary>
        public static Graphics OffScreenGraphics
        {
            get
            {
                if (_offScreenGraphics == null)
                    TryReAllocateOffsceenBuffer();

                return _offScreenGraphics;
            }
        }

        public static Bitmap OffScreenBitmap
        {
            get
            {
                return _offScreenBmp;
            }
        }

        public static void TryReAllocateOffsceenBuffer()
        {
            TryReAllocateOffsceenBuffer(
                ref _offScreenBmp,
                ref _offScreenGraphics,
                Screen.PrimaryScreen.WorkingArea.Width,
                Screen.PrimaryScreen.WorkingArea.Height);
        }

        public static void TryReAllocateOffsceenBuffer(ref Bitmap offScreenBmp, ref Graphics offScreenGraphics, int clientWidth, int clientHeight)
        {
            if (offScreenBmp != null &&
                offScreenBmp.Width == clientWidth &&	//Screen.PrimaryScreen.WorkingArea.Width &&
                offScreenBmp.Height == clientHeight)	//Screen.PrimaryScreen.WorkingArea.Height)
                return;

            try
            {
                if (offScreenBmp != null)
                    ReleaseOffscreenBuffer(ref offScreenBmp, ref offScreenGraphics);

                offScreenBmp = new Bitmap(clientWidth, clientHeight);
                offScreenGraphics = Graphics.FromImage(offScreenBmp);
            }
            catch (OutOfMemoryException)
            {
                ReleaseOffscreenBuffer(ref offScreenBmp, ref offScreenGraphics);
            }
        }

        public static void ReleaseOffscreenBuffer(ref Bitmap offScreenBmp, ref Graphics offScreenGraphics)
        {
            if (offScreenGraphics != null)
            {
                offScreenGraphics.Dispose();
                offScreenGraphics = null;
            }

            if (offScreenBmp != null)
            {
                offScreenBmp.Dispose();
                offScreenBmp = null;
            }
        }

        public static void ReleaseOffscreenBuffer()
        {
            if (_offScreenGraphics != null)
            {
                _offScreenGraphics.Dispose();
                _offScreenGraphics = null;
            }

            if (_offScreenBmp != null)
            {
                _offScreenBmp.Dispose();
                _offScreenBmp = null;
            }
        }
    }
}