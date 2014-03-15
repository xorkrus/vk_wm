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
using System.Runtime.InteropServices;
using System.Text;
using Galssoft.VKontakteWM.Components.GDI;

namespace Galssoft.VKontakteWM.Components.Common.SystemHelpers
{
    /// <summary>
    /// Summary description for Win32.
    /// </summary>
    public class Win32
    {
        public delegate int EnumWindowsProc(IntPtr hwnd, IntPtr lParam);
        public delegate int WindowProcCallback(IntPtr hwnd, uint msg, uint wParam, IntPtr lParam);

        public static readonly string WC_SIPPREF = "SIPPREF";

        public static IntPtr GetHwnd(Control control)
        {
            control.Capture = true;
            IntPtr hwnd = GetCapture();
            control.Capture = false;

            return hwnd;
        }

        [DllImport("coredll.dll")]
        public static extern IntPtr GetModuleHandle(string t);

        /// <summary>
        /// Transfor .net color to GDI color
        /// </summary>
        /// <param name="color">.Net color</param>
        /// <returns>GDI color</returns>
        public static int Color2ColorRef(Color c)
        {
            return (c.B << 16) + (c.G << 8) + c.R;
        }

        public class LOGFONT
        {
            public int      lfHeight;
            public int      lfWidth;
            public int      lfEscapement;
            public int      lfOrientation;
            public int      lfWeight;
            public byte      lfItalic;
            public byte      lfUnderline;
            public byte      lfStrikeOut;
            public byte      lfCharSet;
            public byte      lfOutPrecision;
            public byte      lfClipPrecision;
            public byte      lfQuality;
            public byte      lfPitchAndFamily;
            //public char[]    lfFaceName;
        } 


        public struct NmHeader
        {
            public IntPtr		hwndFrom;
            public uint			idFrom;
            public Win32.DTN	code;
        }

        //
        // RECT Structure
        //
        public struct RECT
        {
            private int m_left;
            private int m_top;
            private int m_right;
            private int m_bottom;

            public RECT(int left, int top, int width, int height)
            {
                m_left = left;
                m_top = top;
                m_right = left + width;
                m_bottom = top + height;
            }

            public RECT(Rectangle rect)
            {
                m_left = rect.Left;
                m_top = rect.Top;
                m_right = rect.Right;
                m_bottom = rect.Bottom;
            }

            public int X
            {
                get { return m_left; }
                set
                {
                    int delta = value - m_left;
                    m_left += delta;
                    m_right += delta;
                }
            }

            public int Y
            {
                get { return m_top; }
                set
                {
                    int delta = value - m_top;
                    m_top += delta;
                    m_bottom += delta;
                }
            }

            public int Height
            {
                get { return m_bottom - m_top; }
            }

            public int Width
            {
                get { return m_right - m_left; }
            }

            public void Offset(int x, int y)
            {
                m_top += y;
                m_bottom += y;
                m_left += x;
                m_right += x;
            }
        }

        public const int TRANSPARENT        = 1;
        public const int OPAQUE             = 2;

        [Flags]
        public enum DT : uint
        {
            TOP				= 0x00000000,
            LEFT			= 0x00000000,
            CENTER			= 0x00000001,
            RIGHT			= 0x00000002,
            VCENTER			= 0x00000004,
            BOTTOM			= 0x00000008,
            WORDBREAK		= 0x00000010,
            SINGLELINE		= 0x00000020,
            NOCLIP			= 0x00000100,
            CALCRECT		= 0x00000400,
            NOPREFIX		= 0x00000800,
        }

        public enum DTS : uint
        {
            UPDOWN					= 0x0001, // use UPDOWN instead of MONTHCAL
            SHOWNONE				= 0x0002, // allow a NONE or checkbox selection
            SHORTDATEFORMAT			= 0x0000, // use the short date format (app must forward WM_WININICHANGE messages)
            LONGDATEFORMAT			= 0x0004, // use the long date format (app must forward WM_WININICHANGE messages)
            SHORTDATECENTURYFORMAT	= 0x000C,// short date format with century (app must forward WM_WININICHANGE messages)
            TIMEFORMAT				= 0x0009, // use the time format (app must forward WM_WININICHANGE messages)
            APPCANPARSE				= 0x0010, // allow user entered strings (app MUST respond to DTN_USERSTRING)
            RIGHTALIGN				= 0x0020, // right-align popup instead of left-align it
            NONEBUTTON				= 0x0080, // use NONE button instead of checkbox
        }

        public enum WS : uint
        {
            CHILD		= 0x40000000,
            VISIBLE		= 0x10000000,
            DISABLED	= 0x08000000,
            BORDER		= 0x00800000,
            DLGFRAME	= 0x00400000,
            VSCROLL		= 0x00200000,
            HSCROLL		= 0x00100000,
            TABSTOP		= 0x00010000,

            //
            // Additional edit styles
            //
            ES_LEFT			= 0x0000,
            ES_CENTER		= 0x0001,
            ES_RIGHT		= 0x0002,
            ES_MULTILINE	= 0x0004,
            ES_UPPERCASE	= 0x0008,
            ES_LOWERCASE	= 0x0010,
            ES_PASSWORD		= 0x0020,
            ES_AUTOVSCROLL	= 0x0040,
            ES_AUTOHSCROLL	= 0x0080,
            ES_NOHIDESEL	= 0x0100,
            ES_COMBOBOX		= 0x0200,
            ES_OEMCONVERT	= 0x0400,
            ES_READONLY		= 0x0800,
            ES_WANTRETURN	= 0x1000,

            //
            // Addition combo box styles
            //
            CBS_DROPDOWN			= 0x0002,
            CBS_DROPDOWNLIST		= 0x0003,
            CBS_AUTOHSCROLL			= 0x0040,
            CBS_OEMCONVERT			= 0x0080,
            CBS_SORT				= 0x0100,
            CBS_HASSTRINGS			= 0x0200,
            CBS_NOINTEGRALHEIGHT	= 0x0400,
            CBS_DISABLENOSCROLL		= 0x0800,
        }

        public enum WM : uint
        {
            Destroy			= 0x2,
            SetFocus		= 7,
            KillFocus		= 8,
            SetText			= 0x000C,
            GetText			= 0x000D,
            GetTextLength	= 0x000E,
            Notify			= 0x004E,
            Command			= 0x0111,
            MouseMove		= 0x0200,
            LButtonDown		= 0x0201,
            LButtonUp		= 0x0202,
            LButtonClick	= 0x0203,
        }

        public enum CB : uint
        {
            GETCURSEL		= 0x0147,
            GETDROPPEDSTATE	= 0x0157,
            GETEDITSEL		= 0x0140,
            SETCURSEL		= 0x014E,
            SETEDITSEL		= 0x0142,
            SHOWDROPDOWN	= 0x014F,
        }

        public enum StockObjects
        {
            WhiteBrush		= 0,
            LtGrayBrush		= 1,
            GrayBrush		= 2,
            DkGrayBrush		= 3,
            BlackBrush		= 4,
            NullBrush		= 5,
            WhitePen		= 6,
            BlackPen		= 7,
            NullPen			= 8,
            SystemFont		= 13,
            DefaultPalette	= 15,
        }

        public enum SW : uint
        {
            Hide		= 0,
            Show		= 5,
            ShowNa		= 8,
            ShowNormal	= 1,
            Restore		= 9
        }

        public enum SWP : uint
        {
            NOSIZE			= 0x0001,
            NOMOVE			= 0x0002,
            NOZORDER		= 0x0004,
            NOACTIVATE		= 0x0010,
            FRAMECHANGED	= 0x0020,  /* The frame changed: send WM_NCCALCSIZE */
            SHOWWINDOW		= 0x0040,
            HIDEWINDOW		= 0x0080,
            NOOWNERZORDER	= 0x0200,  /* Don't do owner Z ordering */

            //			DRAWFRAME		= SWP_FRAMECHANGED,
            //			NOREPOSITION	= SWP_NOOWNERZORDER,
        }

        public enum BkgMode : int
        {
            TRANSPARENT		= 1,
            OPAQUE			= 2,
        }

        public enum LogPixels
        {
            LOGPIXELSX    = 88,    /* Logical pixels/inch in X                 */
            LOGPIXELSY    = 90    /* Logical pixels/inch in Y                 */
        }

        public const int DT_TOP                     = 0x00000000;
        public const int DT_LEFT                    = 0x00000000;
        public const int DT_CENTER                  = 0x00000001;
        public const int DT_RIGHT                   = 0x00000002;
        public const int DT_VCENTER                 = 0x00000004;
        public const int DT_BOTTOM                  = 0x00000008;
        public const int DT_WORDBREAK               = 0x00000010;
        public const int DT_SINGLELINE              = 0x00000020;
        public const int DT_EXPANDTABS              = 0x00000040;
        public const int DT_TABSTOP                 = 0x00000080;
        public const int DT_NOCLIP                  = 0x00000100;
        public const int DT_EXTERNALLEADING         = 0x00000200;
        public const int DT_CALCRECT                = 0x00000400;
        public const int DT_NOPREFIX                = 0x00000800;
        public const int DT_INTERNAL                = 0x00001000;

        public const int DT_EDITCONTROL             = 0x00002000;
        public const int DT_PATH_ELLIPSIS           = 0x00004000;
        public const int DT_END_ELLIPSIS            = 0x00008000;
        public const int DT_MODIFYSTRING            = 0x00010000;
        public const int DT_RTLREADING              = 0x00020000;
        public const int DT_WORD_ELLIPSIS           = 0x00040000;
        public const int DT_NOFULLWIDTHCHARBREAK    = 0x00080000;

        public const int ANSI_CHARSET				 = 0x0;

        public const byte FF_DONTCARE         =(0<<4);  /* Don't care or don't know. */
        public const byte FF_ROMAN            =(1<<4);  /* Variable stroke width, serifed. */
        /* Times Roman, Century Schoolbook, etc. */
        public const byte FF_SWISS            =(2<<4);  /* Variable stroke width, sans-serifed. */
        /* Helvetica, Swiss, etc. */
        public const byte FF_MODERN           =(3<<4);  /* Constant stroke width, serifed or sans-serifed. */
        /* Pica, Elite, Courier, etc. */
        public const byte FF_SCRIPT           =(4<<4);  /* Cursive, etc. */
        public const byte FF_DECORATIVE       =(5<<4);  /* Old English, etc. */

        /* Font Weights */
        public const int FW_DONTCARE         =0;
        public const int FW_THIN             =100;
        public const int FW_EXTRALIGHT       =200;
        public const int FW_LIGHT            =300;
        public const int FW_NORMAL           =400;
        public const int FW_MEDIUM           =500;
        public const int FW_SEMIBOLD         =600;
        public const int FW_BOLD             =700;
        public const int FW_EXTRABOLD        =800;

        public const uint LMEM_ZEROINIT       = 0x0040;


        public const byte DEFAULT_QUALITY        = 0;
        public const byte DRAFT_QUALITY          = 1;
        public const byte PROOF_QUALITY          = 2;
        public const byte NONANTIALIASED_QUALITY = 3;
        public const byte ANTIALIASED_QUALITY    = 4;
        public const byte CLEARTYPE_QUALITY      = 5;
        public const uint WM_LBUTTONDOWN   = 0x0201;
        public const uint WM_LBUTTONUP	   = 0x0202;
	
        [DllImport("coredll")]
        public static extern IntPtr CreateSolidBrush(int color);

        [DllImport("coredll")]
        public static extern IntPtr CreatePatternBrush(IntPtr hbmp);

        [DllImport("coredll")]
        public static extern IntPtr CreatePen(int penStyle, int width, int color);

        [DllImport("coredll.dll") ]
        public static extern IntPtr CreateWindowEx(int dwExStyle, string lpClassName, string lpWindowName, WS style,
                                                   int x, int y, int width, int height, IntPtr hwndParent,
                                                   IntPtr hmenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport("coredll.dll", EntryPoint = "CreateWindowExW", SetLastError = true)]
        public static extern IntPtr CreateWindowEx(int dwExStyle, string lpClassName,
                                                   string lpWindowName, int dwStyle, int x, int y, int nWidth,
                                                   int nHeight,
                                                   IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport("coredll.dll")]
        public static extern bool DestroyWindow(IntPtr hwnd);

        [DllImport("coredll")]
        public static extern int DrawText(IntPtr hDC, string Text, int nLen, IntPtr pRect, uint uFormat);

        [DllImport("coredll.dll")]
        public static extern int DrawText(IntPtr hdc, string pString, int nCount, ref RECT rect, DT uFormat);

        [DllImport("coredll.dll")]
        public static extern bool ExtTextOut(IntPtr hdc, int X, int Y, uint fuOptions, 
                                             IntPtr lprc, string lpString, uint cbCount, IntPtr lpDx);
	
        [DllImport("coredll.dll")]
        public static extern int FillRect(IntPtr hdc, ref RECT rect, IntPtr hbrush);

        [ DllImport("coredll.dll") ]
        public static extern IntPtr GetCapture();

        [ DllImport("coredll.dll") ]
        public static extern IntPtr GetStockObject(StockObjects obj);

        [DllImport("coredll.dll")]
        public static extern bool GetTextExtentExPoint(IntPtr hdc, string lpString, int cchString,
                                                       int nMaxExtent, out int lpnFit, int[] alpDx, ref Size size);

        [DllImport("coredll")]
        public static extern bool MoveWindow(IntPtr hwnd, int x, int y, int width, int height, bool bRepaint);

        public enum EScrollWindow : uint
        {
            Default			  = 0,
            SW_SCROLLCHILDREN = 0x0001,  /* Scroll children within *lprcScroll. */
            SW_INVALIDATE     = 0x0002,  /* Invalidate after scrolling */
            SW_ERASE          = 0x0004,  /* If SW_INVALIDATE, don't send WM_ERASEBACKGROUND */
        }

        [ DllImport("coredll.dll") ]
        public static extern int ScrollWindowEx(IntPtr hwnd,
                                                int xAmount, int yAmount,
                                                IntPtr lpRect, ref Rectangle lpClipRect,
                                                IntPtr hrgnUpdate, out Rectangle prcUpdate, EScrollWindow flags);

        [DllImport("coredll")]
        public static extern bool Polygon(IntPtr hdc, Point[] points, int nCount);

        [DllImport("coredll.dll")]
        public static extern bool Polyline(IntPtr hdc, Point[] points, int cPoints);

        [DllImport("coredll.dll")]
        public static extern bool Rectangle(IntPtr hdc, int left, int top, int right, int bottom);

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("coredll.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
		
        [DllImport("coredll.dll")]
        public extern static int SetWindowText(IntPtr hwnd, string lpString);

        [DllImport("coredll.dll")]
        public static extern bool EnumWindows(IntPtr lpEnumFunc, uint lParam);

        [DllImport("coredll.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint processId); 

        [DllImport("coredll.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("coredll.dll")]
        public static extern int GetWindowRect(IntPtr hWnd,	ref RECT lpRect);
		
        [DllImport("coredll.dll")]
        public static extern bool ShowWindow(IntPtr hwnd, SW nCmdShow);

        [DllImport("coredll.dll")]
        public static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int cx, int cy, SWP flags);

        [DllImport("coredll.dll")]
        public static extern int GetWindowLong(IntPtr hwnd, GWL nIndex);

        [DllImport("coredll")]
        public static extern int GetUpdateRgn(IntPtr hwnd, IntPtr hrgn, bool bErase);

        [DllImport("coredll.dll")]
        public static extern int SetWindowLong(IntPtr hwnd, GWL nIndex, int dwNewLong);

        [DllImport("coredll.dll")]
        public static extern bool SetForegroundWindow(IntPtr hwnd);

        [DllImport("coredll")]
        public static extern IntPtr WindowFromPoint(IntPtr p);

        [DllImport("coredll")]
        public static extern bool EnableWindow(IntPtr hwnd, bool bEnable);

        [DllImport("coredll")]
        public static extern int GetBkColor(IntPtr hdc);

        [DllImport("coredll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("coredll")]
        public static extern int GetClipBox(IntPtr hdc, out RECT lprc);

        [DllImport("coredll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("coredll")]
        public static extern int GetRgnBox(IntPtr hrgn, out RECT lprc);

        [DllImport("coredll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, GW nCmd);

        [DllImport("coredll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("coredll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("coredll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("coredll")]
        public static extern bool DeleteDC(IntPtr hdc); 

        [DllImport("coredll")]
        public static extern int GetDeviceCaps(IntPtr hdc,int value);

        [DllImport("coredll")]
        public static extern int SelectClipRgn(IntPtr hDC, IntPtr hRgn);

        [DllImport("coredll")]
        public static extern int SetTextColor(IntPtr hDC, int cColor);

        [DllImport("coredll")]
        public static extern uint SetTextAlign(IntPtr hDC, uint align);

        [DllImport("coredll")]
        public static extern int SetBkColor(IntPtr hDC, int cColor);

        [DllImport("coredll")]
        public static extern int SetBkMode(IntPtr hDC, int nMode);

        [DllImport("coredll.dll")]
        public static extern int SetBkMode(IntPtr hdc, BkgMode mode);

        [DllImport("coredll")]
        public static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("coredll")]
        public static extern IntPtr LocalAlloc(LocalAllocFlags flags, uint size);

        [DllImport("coredll.dll")]
        public extern static IntPtr BeginPaint(IntPtr hwnd, ref PAINTSTRUCT ps);

        [DllImport("coredll.dll")]
        public extern static bool EndPaint(IntPtr hwnd, ref PAINTSTRUCT ps);

        public struct PAINTSTRUCT
        {
            private IntPtr hdc;
            public bool fErase;
            public Rectangle rcPaint;
            public bool fRestore;
            public bool fIncUpdate;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] rgbReserved;
        }

        [DllImport("coredll.dll")]
        public extern static bool UpdateWindow(IntPtr hwnd);

        [DllImport("coredll.dll")]
        public extern static bool InvalidateRect(IntPtr hwnd, ref Rectangle rcPaint, bool erase);

        public enum LocalAllocFlags
        {
            LPTR = 0x40,
        }

        [DllImport("coredll")]
        public static extern void LocalFree(IntPtr p);

        [DllImport("coredll")]
        public static extern bool DeleteObject(IntPtr hObj);

        [DllImport("coredll")]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObj);

        [DllImport("coredll")]
        public static extern IntPtr CreateFontIndirect(IntPtr pLogFont);

        [DllImport("coredll")]
        public static extern IntPtr GetFocus();

        [DllImport("coredll")]
        public static extern IntPtr SetFocus(IntPtr hwnd);

        [DllImport("coredll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("coredll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint uMsg, uint wParam, IntPtr lParam);
	
        [DllImport("coredll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint uMsg, uint wParam, uint lParam);

        [DllImport("coredll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint uMsg, uint wParam, string lParam);

        [DllImport("coredll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint uMsg, uint wParam, System.Text.StringBuilder lParam);

//		[DllImport("coredll")]
//		public static extern IntPtr SendMessage(IntPtr hWnd, uint uMsg, uint wParam, SystemTime lParam);
//
//		[DllImport("coredll.dll")]
//		public static extern bool FileTimeToSystemTime(FileTime fileTime, SystemTime systemTime);
//
//		[DllImport("coredll.dll")]
//		public static extern bool LocalFileTimeToFileTime(FileTime localFileTime, FileTime fileTime);
//
//		[DllImport("coredll.dll")]
//		public static extern bool SystemTimeToFileTime(SystemTime systemTime, FileTime fileTime);

        //
        //
        // Bitmap functions
        //
        [ DllImport("coredll.dll") ]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int width, int height);

        [ DllImport("coredll.dll") ]
        public static extern IntPtr CreateBitmap(int nWidth, int nHeight, uint cPlanes, uint cBitsPerPixel, byte[] pixels);

        [ DllImport("coredll.dll") ]
        public static extern bool BitBlt(IntPtr hdcDest, int nxDest, int dyDest, int nWidth, int nHeight, IntPtr hdcSrc, int nxSrc, int nySrc, TernaryRasterOperations rop);

        [ DllImport("coredll.dll") ]
        public static extern bool StretchBlt(IntPtr hdcDest, int nxDest, int dyDest, int nWidth, int nHeight, IntPtr hdcSrc, int nxSrc, int nySrc, int nSWidth, int nSHeight, TernaryRasterOperations rop);
		
        [ DllImport("coredll.dll") ]
        public static extern bool TransparentImage(IntPtr hdcDest, int xDestOrg,int  yDestOrg, int dxDest, int dyDest, IntPtr hdcSrc, int xSrcOrg, int ySrcOrg, int dxSrc, int dySrc, int colorref);

        [DllImport("coredll.dll")]
        public static extern Int32 AlphaBlend(IntPtr hdcDest, Int32 xDest, Int32 yDest, Int32 cxDest, Int32 cyDest,
                                              IntPtr hdcSrc, Int32 xSrc, Int32 ySrc, Int32 cxSrc, Int32 cySrc,
                                              BlendFunction blendFunction);

        [ DllImport("coredll.dll") ]
        public static extern IntPtr SHLoadDIBitmap(string szFileName);

        [DllImport("coredll.dll")]
        public static extern bool RoundRect(IntPtr HDC,int x1,int y1,int x2,int y2,int x3,int y3);

        //
        // Resource Functions
        //
        [DllImport("coredll.dll")]
        public static extern IntPtr FindResource(IntPtr hModule, int id, EResourceType type);

        [DllImport("coredll.dll")]
        public static extern bool FreeLibrary(IntPtr hLibModule);

        [DllImport("coredll.dll")]
        public static extern IntPtr LoadLibrary(string lpLibFileName);

        [DllImport("coredll.dll")]
        public static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("coredll.dll")]
        public static extern int LoadString(IntPtr hInstance, int uID, StringBuilder lpBuffer, int nBufferMax);

        [DllImport("coredll.dll")]
        public static extern int SizeofResource(IntPtr hModule, IntPtr hResInfo);

        public enum ProcessorType
        {
            Alpha_21064 = 0x5248,
            ARM_7TDMI = 0x11171,
            ARM720 = 0x720,
            ARM820 = 0x820,
            ARM920 = 0x920,
            Hitachi_SH3 = 0x2713,
            Hitachi_SH3E = 0x2714,
            Hitachi_SH4 = 0x2715,
            Intel_386 = 0x182,
            Intel_486 = 0x1e6,
            Intel_IA64 = 0x898,
            Intel_Pentium = 0x24a,
            Intel_PentiumII = 0x2ae,
            MIPS_R4000 = 0xfa0,
            Motorola_821 = 0x335,
            PPC_403 = 0x193,
            PPC_601 = 0x259,
            PPC_603 = 0x25b,
            PPC_604 = 0x25c,
            PPC_620 = 620,
            SHx_SH3 = 0x67,
            SHx_SH4 = 0x68,
            StrongARM = 0xa11
        }

        public enum ProcessorArchitecture : short
        {
            Alpha = 2,
            Alpha64 = 7,
            ARM = 5,
            IA64 = 6,
            Intel = 0,
            MIPS = 1,
            PPC = 3,
            SHX = 4,
            Unknown = -1
        }

        public enum EResourceType
        {
            Cursor = 1,
            Bitmap = 2,
            Icon = 3,
            Menu = 4,
            Dialog = 5,
            String = 6,
            FontDir = 7,
            Accelerator = 9,
            RcData = 10,
            MessageTable = 11,
        }

        //
        // Common Controls
        //
        public struct TInitCommonControlsEx
        {
            public int dwSize;
            public ICC dwICC;
        }

        [DllImport ("Commctrl.dll")]
        public static extern bool InitCommonControlsEx(ref TInitCommonControlsEx init);

        public enum ICC : uint
        {
            LISTVIEW_CLASSES = 0x00000001, // listview, header
            TREEVIEW_CLASSES = 0x00000002, // treeview, tooltips
            BAR_CLASSES      = 0x00000004, // toolbar, statusbar, trackbar, tooltips
            TAB_CLASSES      = 0x00000008, // tab, tooltips
            UPDOWN_CLASS     = 0x00000010, // updown
            PROGRESS_CLASS   = 0x00000020, // progress
            WIN95_CLASSES    = 0x000000FF,
            DATE_CLASSES     = 0x00000100, // month picker, date picker, time picker, updown
            COOL_CLASSES     = 0x00000400, // rebar (coolbar) control
        }

        public enum DTM : uint
        {
            GETSYSTEMTIME	= 0x1001,
            SETSYSTEMTIME	= 0x1002,
            GETMONTHCAL		= 0x1008,
            SETFORMAT		= 0x1032,
        }

        public enum DTN
        {
            First			= -760,
            DateTimeChange	= -760 + 1,
        }

        public enum EM
        {
            CanUndo		= 0x00C6,
            ReplaceSel	= 0x00C2,
            Undo		= 0x00C7,
        }

        public enum GDT : int
        {
            ERROR	= -1,
            VALID	= 0,
            NONE	= 1,
        }

        public enum GW
        {
            First	= 0,
            Last	= 1,
            Next	= 2,
            Prev	= 3,
            Owner	= 4,
            Child	= 5,
        }

        public enum GWL
        {
            WndProc		= -4,
            Style		= -16,
            ExStyle		= -20,
            UserData	= -21,
            ID			= -12,
        }

        /// <summary>
        /// Gradient fill
        /// </summary>
        public struct TRIVERTEX
        {
            public int x;
            public int y;
            public ushort Red;
            public ushort Green;
            public ushort Blue;
            public ushort Alpha;
            public TRIVERTEX(int x, int y, Color color)
                : this(x, y, color.R, color.G, color.B, color.A)
            {
            }
            public TRIVERTEX(
                int x, int y,
                ushort red, ushort green, ushort blue,
                ushort alpha)
            {
                this.x = x;
                this.y = y;
                Red = (ushort)(red << 8);
                Green = (ushort)(green << 8);
                Blue = (ushort)(blue << 8);
                Alpha = (ushort)(alpha << 8);
            }
        }
        public struct GRADIENT_RECT
        {
            public uint UpperLeft;
            public uint LowerRight;
            public GRADIENT_RECT(uint ul, uint lr)
            {
                UpperLeft = ul;
                LowerRight = lr;
            }
        }

        [DllImport("coredll.dll", SetLastError = true, EntryPoint = "GradientFill")]
        public extern static bool GradientFill(
            IntPtr hdc,
            TRIVERTEX[] pVertex,
            uint dwNumVertex,
            GRADIENT_RECT[] pMesh,
            uint dwNumMesh,
            uint dwMode);

        public const int GRADIENT_FILL_RECT_H = 0x00000000;
        public const int GRADIENT_FILL_RECT_V = 0x00000001;

        public enum TextAlign
        {
            UNKNOWN											= -1,
            TA_LEFT											= 0,
            TA_RIGHT                    = 2,
            TA_CENTER                   = 6,
        }

        public const int WM_PAINT = 0x000F;
        public const int WM_PRINT = 0x317;
        public const int WM_PRINTCLIENT = 0x318;
        public const int WM_ERASEBKGND = 0x14; 
        public const int WM_NOTIFY = 0x004E;
        public const int WS_CHILD = 0x40000000;
        public const int WS_VISIBLE = 0x10000000;
        public const int GWL_WNDPROC = (-4);
        public const int WM_DESTROY = 0x0002;
        public const int SWP_NOZORDER = 0x0004;
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;
        public const int WM_INPUTLANGCHANGE = 0x51;
        public const int WM_WININICHANGE = 0x1A;
        public const int WM_ACTIVATE = 0x06;

        [DllImport("coredll.dll")]
        public static extern IntPtr SetWindowLong(IntPtr hwnd, int nIndex, IntPtr dwNewLong);

        [DllImport("coredll.dll")]
        public static extern int CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hwnd, uint msg, uint wParam, IntPtr lParam);

        [DllImport("aygshell.dll", EntryPoint = "#75")]
        public static extern IntPtr SHLoadImageFile(string pszFileName);

        public struct BITMAP
        {
            public int bmType;
            public int bmWidth;
            public int bmHeight;
            public int bmWidthBytes;
            public ushort bmPlanes;
            public ushort bmBitsPixel;
            public IntPtr bmBits;
        }

        [DllImport("coredll.dll")]
        public static extern int GetObject(IntPtr hObj, int cb, ref BITMAP obj);

        [DllImport("aygshell.dll", EntryPoint = "Vibrate", SetLastError = true)]
        internal static extern int VibratePlay(
            int cvn,
            IntPtr rgvn,
            uint fRepeat,
            uint dwTimeout);

        [DllImport("aygshell.dll", SetLastError = true)]
        internal static extern int VibrateStop();

        public static Int32 MulDivEx(Int32 nNumber, Int32 nNumerator, Int32 nDenominator)
        {
            try
            {
                //MulDiv is not defined in some devices
                return MulDiv(nNumber, nNumerator, nDenominator);
            }
            catch (Exception)
            {
                return nNumber * nNumerator / nDenominator;
            }
        }

        [DllImport("coredll.dll")]
        public static extern Int32 MulDiv(Int32 nNumber, Int32 nNumerator, Int32 nDenominator);

        [DllImport("aygshell.dll")]
        public static extern int SHInitExtraControls();

    }
}