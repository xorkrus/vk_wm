/*	 This file was created by GALS Soft	company
 *	 http://www.galssoft.com
 *	 
 *	 Project name is Odnoklassniki Mobile
 *	 Platform is .Net Compact Framework 2.0 for Windows Mobile 5.0+ 
 * 
 *	 Copyright (c) 2009-2010 GALS Soft
 */

using System;

namespace Galssoft.VKontakteWM.Components.GDI
{
    public struct FontGdi
    {
        private IntPtr	m_hfont;

        public FontGdi(IntPtr hfont)
        {
            m_hfont = hfont;
        }

        public static FontGdi Empty
        {
            get { return (FontGdi) IntPtr.Zero; }
        }

        public bool IsEmpty
        {
            get { return (m_hfont == IntPtr.Zero); }
        }

        public override bool Equals(object obj)
        {
            return ((IntPtr) this == (IntPtr) obj);
        }

        public override int GetHashCode()
        {
            return m_hfont.GetHashCode();
        }


        public static bool operator !=(FontGdi a, FontGdi b)
        {
            return (a.m_hfont != b.m_hfont);
        }

        public static bool operator ==(FontGdi a, FontGdi b)
        {
            return (a.m_hfont == b.m_hfont);
        }

        public static explicit operator IntPtr(FontGdi font)
        {
            return font.m_hfont;
        }

        public static implicit operator FontGdi(IntPtr hfont)
        {
            return new FontGdi(hfont);
        }
    }
}