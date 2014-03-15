/*	 This file was created by GALS Soft	company
 *	 http://www.galssoft.com
 *	 
 *	 Project name is Odnoklassniki Mobile
 *	 Platform is .Net Compact Framework 2.0 for Windows Mobile 5.0+ 
 * 
 *	 Copyright (c) 2009-2010 GALS Soft
 */

using System;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;

namespace Galssoft.VKontakteWM.Components.GDI
{
    public class BrushGdi : IDisposable
    {
        protected IntPtr	m_hbrush;
        private bool		m_stock = false;

        public static BrushGdi Empty = new BrushGdi(IntPtr.Zero);

        protected BrushGdi()
        {
        }

        public BrushGdi(IntPtr hbrush)
        {
            m_hbrush = hbrush;
        }

        public BrushGdi(IntPtr hbrush, bool stock)
        {
            m_hbrush = hbrush;
            m_stock = stock;
        }

        ~BrushGdi()
        {
            Release();
        }

        public void Dispose()
        {
            Release();
            GC.SuppressFinalize(this);
        }

        public static explicit operator IntPtr(BrushGdi brush)
        {
            if (brush == null)
                return IntPtr.Zero;
            else
                return brush.m_hbrush;
        }

        public static implicit operator BrushGdi(IntPtr hbrush)
        {
            return new BrushGdi(hbrush);
        }

        private void Release()
        {
            if (m_hbrush != IntPtr.Zero)
            {
                if (!m_stock)
                    Win32.DeleteObject(m_hbrush);
                m_hbrush = IntPtr.Zero;
            }
        }
    }
}