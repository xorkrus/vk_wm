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
using System.Runtime.InteropServices;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;

namespace Galssoft.VKontakteWM.Components.GDI
{
    public class ImageData : IDisposable
    {
        // Pointer to an external unmanaged resource.
        private bool disposed;
        private IntPtr _imageHandle = IntPtr.Zero;
        private readonly Size _ImageSize;
        private Color _TransparentColor = Color.Empty;

        public IntPtr ImageHandle
        {
            get { return _imageHandle; }
        }

        public Size ImageSize
        {
            get { return _ImageSize; }
        }

        public Color TransparentColor
        {
            get { return _TransparentColor; }
            set { _TransparentColor = value; }
        }

        // Track whether Dispose has been called.

        // The class constructor.
        public ImageData(string imagePath)
        {
            if(string.IsNullOrEmpty(imagePath))
                return;

            _imageHandle = Win32.SHLoadImageFile(imagePath);
            if (_imageHandle != IntPtr.Zero)
            {
                var bm = new Win32.BITMAP();
                Win32.GetObject(_imageHandle, Marshal.SizeOf(bm), ref bm);
                _ImageSize.Height = bm.bmHeight;
                _ImageSize.Width = bm.bmWidth;
                ViewportSize = ImageSize;
            }
        }

        public ImageData(string ImagePath, Color transparentColor) : this(ImagePath)
        {
            _TransparentColor = transparentColor;
        }


        public Point ImageOffset { get; set; }

        public Size ViewportSize { get; set; }

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        #endregion

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed.
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!disposed)
            {
                // Call the appropriate methods to clean up 
                // unmanaged resources here.
                // If disposing is false, 
                // only the following code is executed.
                Win32.DeleteObject(_imageHandle);
                _imageHandle = IntPtr.Zero;
            }
            disposed = true;
        }

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method 
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~ImageData()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
    }
}