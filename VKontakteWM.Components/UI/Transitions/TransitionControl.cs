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
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.GDI;

namespace Galssoft.VKontakteWM.Components.UI.Transitions
{
    internal class TransitionControl : Control
    {
        private readonly Win32.WindowProcCallback _delegate;
        private readonly IntPtr _wndprocReal; // the original wndproc of


        internal IViewTransition CurrentTransition { get; set; }

        private Bitmap _offScreenBmp;
        private Graphics _offScreenGraphics;

        Rectangle _ContentRectangle = Rectangle.Empty;

        public TransitionControl()
        {
            _delegate = WnProc;
            _wndprocReal = Win32.SetWindowLong(Handle, Win32.GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(_delegate));
        }

        private int WnProc(IntPtr hwnd, uint msg, uint wParam, IntPtr lParam)
        {
            bool handled = false;
            if (msg == Win32.WM_PAINT)
            {
                handled = OnPaint(hwnd);
            }

            return !handled ? Win32.CallWindowProc(_wndprocReal, hwnd, msg, wParam, lParam) : 0;
        }

        bool OnPaint(IntPtr hWnd)
        {
            if (CurrentTransition == null)
                return false;

            var ps = new Win32.PAINTSTRUCT();

            IntPtr hdc = Win32.BeginPaint(hWnd, ref ps);

            IntPtr hdcMem = _offScreenGraphics.GetHdc();

            using (Gdi g = Gdi.FromHdc(hdc, ps.rcPaint))
            {
                using (Gdi gMem = Gdi.FromHdc(hdcMem, Rectangle.Empty))
                {
                    Rectangle rect = ps.rcPaint;

                    try
                    {
                        CurrentTransition.DrawScreenOn(gMem, ps.rcPaint);
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                    g.BitBlt(rect.Left, rect.Top, rect.Width, rect.Height, hdcMem, rect.Left, rect.Top,
                             TernaryRasterOperations.SRCCOPY);
                }
            }

            _offScreenGraphics.ReleaseHdc(hdcMem);
            Win32.EndPaint(hWnd, ref ps);
            return true;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        { }

        public bool AllocateOffscreenBitmap()
        {
            if (_offScreenGraphics != null)
            {
                ReleaseOffscreenBitmap();
            }

            _ContentRectangle = ClientRectangle;

            try
            {
                _offScreenBmp = new Bitmap(ClientSize.Width, ClientSize.Height);
                _offScreenGraphics = Graphics.FromImage(_offScreenBmp);
            }
            catch (OutOfMemoryException e)
            {
                DebugHelper.WriteLogEntry(e.ToString());
                _offScreenGraphics = null;
                return false;
            }

            return _offScreenGraphics != null;
        }

        public void ReleaseOffscreenBitmap()
        {
            if (_offScreenGraphics != null)
            {
                _offScreenGraphics.Dispose();
                _offScreenBmp.Dispose();
                _offScreenGraphics = null;
            }
        }


        public void RefreshControl()
        {
            _ContentRectangle = ClientRectangle;
            Win32.InvalidateRect(Handle, ref _ContentRectangle, false);
            Win32.UpdateWindow(Handle);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ReleaseOffscreenBitmap();
            }

            base.Dispose(disposing);
        }
    }
}