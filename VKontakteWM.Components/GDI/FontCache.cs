/*	 This file was created by GALS Soft	company
 *	 http://www.galssoft.com
 *	 
 *	 Project name is Odnoklassniki Mobile
 *	 Platform is .Net Compact Framework 2.0 for Windows Mobile 5.0+ 
 * 
 *	 Copyright (c) 2009-2010 GALS Soft
 */

using System;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Microsoft.Win32;

namespace Galssoft.VKontakteWM.Components.GDI
{
    public class FontCache
    {
        static protected FontCache m_cache = new FontCache();
        static protected Hashtable m_fontFamilies = new Hashtable();

        //
        // FontCache constructor
        //
        // Don't allow an instance of this class to be created. The only access is via the static
        // methods.
        //
        private FontCache()
        {
        }

        //
        // FontVariation class
        //
        // Used to keep track of all the different variations.
        //
        protected class FontVariation
        {
            private const int c_maxSize = (int)(FontStyle.Bold | FontStyle.Regular | FontStyle.Italic | FontStyle.Strikeout | FontStyle.Underline);
            private IntPtr[] m_fonts = new IntPtr[c_maxSize];

            public IntPtr this[FontStyle style]
            {
                get { return m_fonts[(int) style]; }
                set { m_fonts[(int) style] = value; }
            }
        }

        protected class FontFamilyItem
        {
            internal Hashtable SizeList = new Hashtable();
			
            public FontVariation this[float val]
            {
                get
                {
                    if ( !SizeList.ContainsKey(val) )
                        SizeList.Add(val, new FontVariation());
                    return (FontVariation)SizeList[val];
                }
            }
        }

        private FontFamilyItem this[string Family] 
        {
            get 
            { 
                if ( !m_fontFamilies.ContainsKey(Family) )
                {
                    m_fontFamilies.Add(Family, new FontFamilyItem());
                }
                return (FontFamilyItem) m_fontFamilies[Family];
            }
        }

        public static FontGdi CreateFont(string Family, float Size, FontStyle Style)
        {
            return CreateFont(Family, Size, Style, true);
        }

        public static FontGdi CreateFont(string Family, float Size, FontStyle Style, bool forceClearType)
        {
            IntPtr hFont = m_cache[Family][Size][Style];
            if ( hFont != IntPtr.Zero )
                return new FontGdi(hFont);

            //
            // ClearType: Check to see if this computer has ClearType turned on so we can
            // display with ClearType fonts if it does.
            //
            bool useCleartype = forceClearType;
            if (!forceClearType)
            {
                object clearType = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Shell", "ClearType", null);
                useCleartype = (clearType is int && (int) clearType == 1);
            }

            Win32.LOGFONT lf = new Win32.LOGFONT();
            lf.lfCharSet = Win32.ANSI_CHARSET;
            lf.lfClipPrecision = 0;
            lf.lfEscapement = 0;
            //lf.lfFaceName = Family.PadRight(32, ' ').ToCharArray();
            lf.lfHeight = UISettings.CalcPix((int)Size);// (Size / 8 * 13);
            lf.lfItalic = (Style & FontStyle.Italic) == FontStyle.Italic? (byte)1: (byte)0;
            lf.lfOrientation = 0;
            lf.lfOutPrecision = 0;
            lf.lfPitchAndFamily = Win32.FF_DONTCARE;
            if (useCleartype)
                lf.lfQuality = Win32.CLEARTYPE_QUALITY;
            else
                lf.lfQuality = Win32.DEFAULT_QUALITY;
            lf.lfStrikeOut = (Style & FontStyle.Strikeout) == FontStyle.Strikeout? (byte)1: (byte)0;
            lf.lfUnderline = (Style & FontStyle.Underline) == FontStyle.Underline? (byte)1: (byte)0;
            lf.lfWeight = (Style & FontStyle.Bold) == FontStyle.Bold? Win32.FW_BOLD: Win32.FW_NORMAL;
            lf.lfWidth = 0;
            IntPtr pLF = Win32.LocalAlloc(Win32.LocalAllocFlags.LPTR, 92);
            Marshal.StructureToPtr(lf, pLF, false);
            if ( Family.Length > 32 ) Family = Family.Substring(0, 32);
            Marshal.Copy(Family.ToCharArray(), 0, (IntPtr) ((int)pLF + 28), Family.Length);
            hFont = Win32.CreateFontIndirect(pLF);
            Marshal.PtrToStructure(pLF, lf);
            Win32.LocalFree(pLF);
            m_cache[Family][Size][Style] = hFont;
            return new FontGdi(hFont);
        }

        public static FontGdi CreateFont(Font fromFont)
        {
            return CreateFont(fromFont.Name, fromFont.Size, fromFont.Style);
        }
    }
}