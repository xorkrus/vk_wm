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

namespace Galssoft.VKontakteWM.Components.UI.WebBrowser
{
    public delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    public static class WebBrowserAPI
    {
        public const int DTM_BROWSERDISPATCH = WM_USER + 124;
        public const int DTM_CLEAR = WM_USER + 113;
        public const int DTM_DOCUMENTDISPATCH = WM_USER + 123;
        public const int DTM_IMAGEFAIL = WM_USER + 109;
        public const int DTM_SETIMAGE = WM_USER + 103;
        public const int DTM_ENABLECONTEXTMENU = WM_USER + 110;
        public const int DTM_NAVIGATE = WM_USER + 120;
        public const int DTM_ZOOMLEVEL = WM_USER + 116;
        public const int DTM_STOP = WM_USER + 125;

        //DTM_NAVIGATE flags
        public const int NAVIGATEFLAG_REFRESH = 0x0020;
        public const int NAVIGATEFLAG_RELATIVE = 0x0040;
        public const int NAVIGATEFLAG_ENTERED = 0x0080;
        public const int NAVIGATEFLAG_IGNORETARGET = 0x0200;
        public const int NAVIGATEFLAG_GETFROMCACHE = 0x0400;
        public const int NAVIGATEFLAG_NOCACHE = 0x1000;
        public const int NAVIGATEFLAG_RESYNCHRONIZE = 0x2000;
        public const int NAVIGATEFLAG_RELOAD = 0x4000;

        public const int NM_HOTSPOT = WM_USER + 101;
        public const int NM_INLINE_IMAGE = WM_USER + 102;
        //protected const int NM_INLINE_SOUND =        (WM_USER + 103);
        //protected const int NM_TITLE =               (WM_USER + 104);
        //protected const int NM_META =                (WM_USER + 105);
        //protected const int NM_BASE =                (WM_USER + 106);
        //public const int NM_CONTEXTMENU = WM_USER + 107;
        //protected const int NM_INLINE_XML =          (WM_USER + 108);
        public const int NM_BEFORENAVIGATE = WM_USER + 109;
        public const int NM_DOCUMENTCOMPLETE = WM_USER + 110;
        public const int NM_NAVIGATECOMPLETE = WM_USER + 111;
        //protected const int NM_TITLECHANGE =         (WM_USER + 112);

        public const int HS_NOSELECTION = 0x0200;

        public const int DTM_ANCHOR = 1129;
        public const int DTM_ANCHORW = 1130;

        public const int GW_HWNDNEXT = 2;
        public const int GW_CHILD = 5;

        public const int SIF_POS = 0x0004;
        public const int SIF_RANGE = 0x0001;
        public const int SIF_PAGE = 0x0002;
        public const int SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS);
        public const int SB_VERT = 0x1;
        public const int GWL_WNDPROC = (-4);
        public const int SB_BOTTOM = 0x7;
        public const int WM_VSCROLL = 0x115;
        public const int WM_USER = 0x0400;
        public const int WM_NOTIFY = 0x004E;
        public const int WS_CHILD = 0x40000000;
        public const int WS_TABSTOP = 0x00010000;
        public const int WS_VISIBLE = 0x10000000;
        public const int SW_INVALIDATE = 0x0002;
        public const int SW_ERASE = 0x0004;

        public const string HtmlClassName = "PIEHTML";

        [StructLayout(LayoutKind.Sequential)]
        public struct NmHdr
        {
            public IntPtr hwndFrom;
            public uint idFrom;
            public uint code;
            public static NmHdr GetStruct(IntPtr p)
            {
                return (NmHdr)Marshal.PtrToStructure(p, typeof(NmHdr));
            }
        }

        public struct SCROLLINFO
        {
            internal uint cbSize;
            internal uint fMask;
            internal int nMin;
            internal int nMax;
            internal int nPage;
            internal int nPos;
            internal uint nTrackPos;
        }

        //public struct HtmlContextMessage
        //{
        //    internal WebBrowserAPI.NmHdr hdr;
        //    internal System.Drawing.Point pt;
        //    internal uint uTypeFlags;
        //    internal IntPtr szLinkHREF;
        //    internal IntPtr szLinkName;
        //    internal int dwUnused1;
        //    internal int dwImageCookie;
        //    internal int dwUnused2;

        //    internal const int HTMLCONTEXT_BACKGROUND = 0x00;
        //    internal const int HTMLCONTEXT_LINK = 0x01;
        //    internal const int HTMLCONTEXT_IMAGE = 0x02;
        //    internal const int HTMLCONTEXT_IMAGENOTLOADED = 0x04;

        //    internal static HtmlContextMessage GetStruct(IntPtr p)
        //    {
        //        return (HtmlContextMessage)Marshal.PtrToStructure(p, typeof(HtmlContextMessage));
        //    }

        //    internal string linkHREF
        //    {
        //        get
        //        {
        //            return (szLinkHREF != IntPtr.Zero ? Marshal.PtrToStringUni(szLinkHREF) : null);
        //        }
        //    }
        //    internal string linkName
        //    {
        //        get
        //        {
        //            return (szLinkName != IntPtr.Zero ? Marshal.PtrToStringUni(szLinkName) : null);
        //        }
        //    }
        //}

        public struct INLINEIMAGEINFO
        {
            public UInt32 dwCookie;
            public int iOrigHeight;
            public int iOrigWidth;
            public IntPtr hbm;
            public bool bOwnBitmap;
        }

        public struct HtmlViewMessage
        {
            internal NmHdr hdr;
            internal IntPtr szTarget;
            internal IntPtr szData;
            internal int dwCookieFlags;
            internal IntPtr szExInfo;

            internal static HtmlViewMessage GetStruct(IntPtr p)
            {
                return (HtmlViewMessage)Marshal.PtrToStructure(p, typeof(HtmlViewMessage));
            }

            internal string target
            {
                get
                {
                    return (szTarget != IntPtr.Zero ? Marshal.PtrToStringUni(szTarget) : null);
                }
            }

            internal string targetStr
            {
                get { return GetStringFromLPSTR(szTarget); }
            }

            public string Url
            {
                get { return targetStr; }
            }

            internal string data
            {
                get
                {
                    return (szData != IntPtr.Zero ? Marshal.PtrToStringUni(szData) : null);
                }
            }

            internal string exInfo
            {
                get
                {
                    return (szExInfo != IntPtr.Zero ? Marshal.PtrToStringUni(szExInfo) : null);
                }
            }

            private string GetStringFromLPSTR(IntPtr ptr)
            {
                if (ptr != IntPtr.Zero)
                {
                    byte[] bytes = new byte[1024];
                    Marshal.Copy(ptr, bytes, 0, bytes.Length);
                    string value = System.Text.Encoding.ASCII.GetString(bytes, 0, bytes.Length);

                    return value.Substring(0, value.IndexOf('\0'));
                }
                return null;
            }
        }

        static WebBrowserAPI()
        {
            //load htmlview module
            IntPtr module = LoadLibrary("htmlview.dll");

            //init htmlcontrol
            IntPtr inst = GetModuleHandle(null);
            int result = InitHTMLControl(inst);
        }


        [DllImport("coredll", SetLastError = true)]
        public static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("coredll.dll", EntryPoint = "CreateWindowExW", SetLastError = true)]
        public static extern IntPtr CreateWindowEx(int dwExStyle, string lpClassName,
                                                   string lpWindowName, int dwStyle, int x, int y, int nWidth,
                                                   int nHeight,
                                                   IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport("coredll.dll")]
        public static extern bool DestroyWindow(IntPtr hwnd);

        [DllImport("coredll", SetLastError = true)]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("coredll", SetLastError = true)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr newWndProc);

        [DllImport("coredll", SetLastError = true)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, WndProcDelegate newWndProc);

        [DllImport("coredll.dll")]
        private static extern IntPtr GetModuleHandle(string t);

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);

        [DllImport("coredll.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, char[] lpClassName, int nMaxCount);

        [DllImport("htmlview.dll", EntryPoint = "InitHTMLControl", SetLastError = true)]
        private static extern int InitHTMLControl(IntPtr hinst);

        [DllImport("coredll.dll")]
        private static extern IntPtr LoadLibrary(string path);

        [DllImport("coredll.dll")]
        public static extern IntPtr SendMessage(IntPtr hwnd, int msg, int lParam, ref IntPtr wParam);

        [DllImport("coredll.dll")]
        public static extern IntPtr SendMessage(IntPtr hwnd, int msg, int lParam, string wParam);

        [DllImport("coredll.dll")]
        public static extern IntPtr SendMessage(IntPtr hwnd, int msg, int lParam, int wParam);

        [DllImport("coredll.dll")]
        public static extern int ScrollWindowEx(IntPtr hWnd, int dx, int dy, int scrollRect, int clipRect, IntPtr hrgn, Rectangle updateRect, int flags);

        [DllImport("coredll.dll")]
        public static extern IntPtr GetClientRect(IntPtr hwnd, ref Rectangle updateRect);

        [DllImport("coredll.dll")]
        public static extern int SetScrollInfo(IntPtr hwnd, int n, ref SCROLLINFO lpScrollInfo, bool fRedraw);

        [DllImport("coredll.dll")]
        public static extern int GetScrollInfo(IntPtr hwnd, int n, ref SCROLLINFO lpScrollInfo);

        public static void InitializeHtmlView()
        {
        }

        public static IntPtr FindChildWindowByParent(string strChildClassName, IntPtr hWndTopLevel)
        {
            bool bFound = false;
            IntPtr hwndCur = IntPtr.Zero;
            IntPtr hwndCopyOfCur = IntPtr.Zero;
            char[] chArWindowClass = new char[32];

            do
            {
                // Is the current child null?
                if (IntPtr.Zero == hwndCur)
                {
                    // get the first child
                    hwndCur = GetWindow(hWndTopLevel, GW_CHILD);
                }
                else
                {
                    hwndCopyOfCur = hwndCur;
                    // at this point hwndcur may be a parent of other windows
                    hwndCur = GetWindow(hwndCur, GW_CHILD);

                    // in case it's not a parent, does it have "brothers"?
                    if (IntPtr.Zero == hwndCur)
                        hwndCur = GetWindow(hwndCopyOfCur, GW_HWNDNEXT);
                }

                //if we found a window (child or "brother"), let's see if it's the one we were looking for
                if (IntPtr.Zero != hwndCur)
                {
                    GetClassName(hwndCur, chArWindowClass, 256);
                    string strWndClass = new string(chArWindowClass);
                    strWndClass = strWndClass.Substring(0, strWndClass.IndexOf('\0'));

                    bFound = (strWndClass.ToLower() == strChildClassName.ToLower());
                }
                else
                    break;
            }
            while (!bFound);

            return hwndCur;
        }

    }
}