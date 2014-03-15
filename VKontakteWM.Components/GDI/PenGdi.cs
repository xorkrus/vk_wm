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
    public class PenGdi : IDisposable
    {
        private IntPtr	m_hpen;

        public static PenGdi Empty = new PenGdi(IntPtr.Zero);

        public PenGdi(IntPtr hpen)
        {
            m_hpen = hpen;
        }

        ~PenGdi()
        {
            Release();
        }

        public void Dispose()
        {
            Release();
            GC.SuppressFinalize(this);
        }

        public bool IsEmpty()
        {
            return (m_hpen == IntPtr.Zero);
        }

        public static explicit operator IntPtr(PenGdi pen)
        {
            return pen.m_hpen;
        }

        public static implicit operator PenGdi(IntPtr hpen)
        {
            return new PenGdi(hpen);
        }

        private void Release()
        {
            if (m_hpen != IntPtr.Zero)
            {
                Win32.DeleteObject(m_hpen);
                m_hpen = IntPtr.Zero;
            }
        }
    }
}