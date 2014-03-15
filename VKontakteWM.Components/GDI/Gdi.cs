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
using System.Drawing.Imaging;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;

namespace Galssoft.VKontakteWM.Components.GDI
{
    /// <summary>
    /// Extension for standard Graphics object in .net compact framework
    /// </summary>
    public class Gdi : IDisposable
    {
        #region Enums

        public enum PenMode : int
        {
            Solid = 0,
            Dash = 1,
            Null = 5,
        }

        #endregion

        #region Private properties

        private IntPtr _hdc;
        private IntPtr _hwnd;
        private bool _fontChanged = false;
        private IntPtr _oldFont;
        private Color _textColor = Color.Black;
        private IntPtr _oldPen;
        private PenGdi _pen;
        private IntPtr _oldBrush;
        private FontGdi _font;
        private BrushGdi _brush = new BrushGdi(IntPtr.Zero);
        private bool _ownDc = true;			// True means we'll release DC in desctructor
        private Win32.TextAlign _TextAlign = Win32.TextAlign.TA_LEFT;

        #endregion

        #region Constructors and Destructors

        private Gdi()
        {
        }

        public Gdi(IntPtr hwnd)
        {
            Init(hwnd, Rectangle.Empty);
        }

        public Gdi(IntPtr hwnd, Rectangle clipRect)
        {
            Init(hwnd, clipRect);
        }

        public Gdi(Control control)
        {
            control.Capture = true;
            IntPtr hwnd = Win32.GetHwnd(control);
            control.Capture = false;
            Init(hwnd, Rectangle.Empty);
        }

        public static Gdi FromHdc(IntPtr hdc, Rectangle clipRect)
        {
            Gdi gdi = new Gdi();
            gdi.InitWithHdc(hdc, clipRect, false);
            return gdi;
        }

        private void Init(IntPtr hwnd, Rectangle clipRect)
        {
            _hwnd = hwnd;
            InitWithHdc(Win32.GetDC(hwnd), clipRect, true);
        }

        private void InitWithHdc(IntPtr hdc, Rectangle clipRect, bool ownDc)
        {
            _hdc = hdc;
            this.Transparent = true;				// Use transparent drawing by default
            _ownDc = ownDc;

            if (!clipRect.IsEmpty)					// If there is a clip rectangle, us it.
            {
                int l, t, r, b;
                l = Convert.ToInt16(clipRect.Left);
                t = Convert.ToInt16(clipRect.Top);
                r = Convert.ToInt16(clipRect.Right);
                b = Convert.ToInt16(clipRect.Bottom);
                IntPtr hrgn = Win32.CreateRectRgn(l, t, r, b);
                int i = Win32.SelectClipRgn(_hdc, hrgn);
                Win32.DeleteObject(hrgn);
            }
        }

        ~Gdi()
        {
            Release();								// Make sure we release the DC
        }

        public void Dispose()
        {
            Release();								// Release the DC
            GC.SuppressFinalize(this);				// Don't call the destructor
        }

        /// <summary>
        /// Release all of our unmanaged resources.
        /// </summary>
        private void Release()
        {
            if (_hdc != IntPtr.Zero)
            {
                if (_fontChanged)
                    Win32.SelectObject(_hdc, _oldFont);
                if (_oldPen != IntPtr.Zero)
                    Win32.SelectObject(_hdc, _oldPen);

                if (_ownDc)
                    Win32.ReleaseDC(_hwnd, _hdc);
                _hdc = IntPtr.Zero;
            }
        }

        #endregion

        #region Properties

        public Color BackColor
        {
            set { Win32.SetBkColor(_hdc, Win32.Color2ColorRef(value)); }
            get { return Color.FromArgb(Win32.GetBkColor(_hdc)); }
        }

        public BrushGdi Brush
        {
            set
            {
                if ((IntPtr)value == IntPtr.Zero)
                {
                    if (_oldBrush != IntPtr.Zero)
                        Win32.SelectObject(_hdc, _oldBrush);
                    _oldBrush = IntPtr.Zero;
                }
                else
                {
                    IntPtr br = Win32.SelectObject(_hdc, (IntPtr)value);
                    if (_oldBrush == IntPtr.Zero)
                        _oldBrush = br;
                }
                _brush = value;
            }
            get { return _brush; }
        }

        public FontGdi Font
        {
            set
            {
                if (value.IsEmpty && _fontChanged)
                {
                    Win32.SelectObject(_hdc, (IntPtr)_oldFont);
                    _oldFont = IntPtr.Zero;
                    _font = FontGdi.Empty;
                }

                IntPtr oldFont = Win32.SelectObject(_hdc, (IntPtr)value);
                if (!_fontChanged)
                {
                    _oldFont = oldFont;
                    _fontChanged = true;
                    _font = value;
                }
            }
            get { return _font; }
        }

        /// <summary>
        /// The pen that you want to use for drawing.
        /// <br/>
        /// This method does not dispose of the pen that you created. You'll need
        /// to do that yourself after you've finished using it.	PenGdi.Empty - Release the last pen from this DC.
        /// </summary>
        public PenGdi Pen
        {
            set
            {
                if (value.IsEmpty())
                {
                    if (_oldPen != IntPtr.Zero)
                        Win32.SelectObject(_hdc, _oldPen);
                    _oldPen = IntPtr.Zero;
                    _pen = null;
                    return;
                }

                IntPtr oldPen = Win32.SelectObject(_hdc, (IntPtr)value);
                if (_oldPen == IntPtr.Zero)
                    _oldPen = oldPen;
                _pen = value;
            }
        }

        /// <summary>
        /// Color to draw text
        /// </summary>
        public Color TextColor
        {
            get { return _textColor; }
            set
            {
                if (value == _textColor)
                    return;

                Win32.SetTextColor(_hdc, GetWin32Color(value));
                _textColor = value;
            }
        }

        /// <summary>
        /// Color for the text shadow
        /// </summary>
        protected readonly int Win32ShadowColor = GetWin32Color(Color.Gray);

        /// <summary>
        /// Calculate color in win32 format
        /// </summary>
        /// <param name="color"></param>
        /// <returns>Integer value for color</returns>
        private static int GetWin32Color(Color color)
        {
            return (color.B << 16) + (color.G << 8) + color.R;
        }

        public Win32.TextAlign TextAlign
        {
            get
            {
                return _TextAlign;
            }
            set
            {
                if (_TextAlign == value)
                    return;

                Win32.SetTextAlign(_hdc, (uint)value);
                _TextAlign = value;
            }
        }

        /// <summary>
        /// Control whether text drawing is transparent or opaque.
        /// </summary>
        public bool Transparent
        {
            set
            {
                Win32.BkgMode mode = (value) ? Win32.BkgMode.TRANSPARENT : Win32.BkgMode.OPAQUE;
                Win32.SetBkMode(_hdc, mode);
            }
        }

        #endregion

        #region GDI+ extension methods

        public IntPtr CreateCompatibleDC()
        {
            return Win32.CreateCompatibleDC(_hdc);
        }

        public static void ReleaseDC(IntPtr hdc)
        {
            Win32.DeleteDC(hdc);
        }

        //public int GetDpi()
        //{
        //    return Win32.GetDeviceCaps(_hdc, (int)Win32.LogPixels.LOGPIXELSX);
        //}

        //public static int GetScreenDpi(IntPtr hWnd)
        //{
        //    using(Gdi g = new Gdi(hWnd))
        //    {
        //        return g.GetDpi();
        //    }
        //}

        public IntPtr ClipRegion
        {
            set { Win32.SelectClipRgn(_hdc, value); }
        }

        public static PenGdi CreatePen(Color c)
        {
            return new PenGdi(Win32.CreatePen(0, 1, Win32.Color2ColorRef(c)));
        }

        public static PenGdi CreatePen(Color color, PenMode mode)
        {
            return new PenGdi(Win32.CreatePen((int)mode, 1, Win32.Color2ColorRef(color)));
        }

        public static BrushGdi CreateSolidBrush(Color c)
        {
            return new BrushGdi(Win32.CreateSolidBrush(Win32.Color2ColorRef(c)));
        }

        public bool DeleteObject(IntPtr obj)
        {
            return Win32.DeleteObject(obj);
        }

        public BrushGdi GetStockBrush(Win32.StockObjects type)
        {
            return new BrushGdi(Win32.GetStockObject(type), true);
        }

        public Size GetTextExtent(string text)
        {
            if (text == null || text.Length == 0)
                return Size.Empty;

            Size size = Size.Empty;
            int fit;
            Win32.GetTextExtentExPoint(_hdc, text, text.Length, 10000, out fit, null, ref size);
            return size;
        }

        /// <summary>
        /// GetTextExtent
        /// </summary>
        /// <param name="text">The text that we want to measure</param>
        /// <param name="width">The maximum width that we'll allow</param>
        /// <param name="extents">An array of character positions for the string</param>
        /// <param name="maxFit">The maximum number of characters that will fit in width.</param>
        /// <returns>The width and height of the string.</returns>
        public Size GetTextExtent(string text, int width, out int[] extents, out int maxFit)
        {
            Size size = Size.Empty;
            extents = new int[text.Length];
            Win32.GetTextExtentExPoint(_hdc, text, text.Length, width, out maxFit, extents, ref size);
            return size;
        }

        //public int TextHeightForWidth(string text, int width)
        //{
        //    // example from http://www.mobilepractices.com/2007/12/multi-line-graphicsmeasurestring.html

        //    Win32.RECT rect = new Win32.RECT(0, 0, width, 0);

        //    //var hFont = Font.ToHfont();
        //    //var oldHFont = Api.SelectObject(hDc, hFont);

        //    int flags = Win32.DT_CALCRECT | Win32.DT_WORDBREAK;

        //    DrawText(hDc, text, text.Length, ref bounds, flags);

        //    Api.SelectObject(hDc, oldHFont);

        //    Api.DeleteObject(hFont);

        //    g.ReleaseHdc(hDc);

        //    return bounds.Bottom - bounds.Top;
        //}

        public Size GetDrawTextSize(string text, int width, Win32.DT format)
        {
            Win32.RECT rect = new Win32.RECT(0, 0, width, 1);
            Win32.DrawText(_hdc, text, text.Length, ref rect, format | Win32.DT.CALCRECT);

            return new Size(rect.Width, rect.Height);
        }

        /// <summary>
        /// This method includes overloads for the type-safe objects that can be
        /// selected into this DC.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public IntPtr SelectObject(IntPtr obj)
        {
            return Win32.SelectObject(_hdc, obj);
        }

        public PenGdi SelectObject(PenGdi pen)
        {
            return Win32.SelectObject(_hdc, (IntPtr)pen);
        }

        public BrushGdi SelectObject(BrushGdi brush)
        {
            return Win32.SelectObject(_hdc, (IntPtr)brush);
        }

        /// <summary>
        /// Create snapshot for the control
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static Bitmap CreateControlSnapshot(Control control)
        {
            Bitmap bmp = null;

            try
            {
                using (Graphics gx = control.CreateGraphics())
                {
                    Rectangle rect = control.Bounds;
                    bmp = new Bitmap(rect.Width, rect.Height);

                    using (var gxComp = Graphics.FromImage(bmp))
                    {
                        gxComp.Clear(Color.Black);

                        var hdcBmp = gxComp.GetHdc();
                        var hdcControl = gx.GetHdc();

                        using (var g = FromHdc(hdcBmp, Rectangle.Empty))
                        {
                            g.BitBlt(0, 0, rect.Width, rect.Height, hdcControl,
                                     0, 0, //rect.Left, rect.Top,
                                     TernaryRasterOperations.SRCCOPY);
                        }

                        gxComp.ReleaseHdc(hdcBmp);
                        gx.ReleaseHdc(hdcControl);
                    }

                    return bmp;
                }
            }
            catch (Exception)
            {
                if (bmp != null)
                    bmp.Dispose();

                return null;
            }
        }

        /// <summary>
        /// Поворот Битмапа на 90, 180, 270 градусов
        /// </summary>
        /// <param name="rotationAngle">Угол - 90, 180, 270 градусов</param>
        /// <param name="originalBitmap"></param>
        /// <param name="rotatedBitmap"></param>
        public static void RotateImage(int rotationAngle, Bitmap originalBitmap, Bitmap rotatedBitmap)
        {
            // It should be faster to access values stored on the stack
            // compared to calling a method (in this case a property) to 
            // retrieve a value. That's why we store the width and height
            // of the bitmaps here so that when we're traversing the pixels
            // we won't have to call more methods than necessary.

            int newWidth = rotatedBitmap.Width;
            int newHeight = rotatedBitmap.Height;

            int originalWidth = originalBitmap.Width;
            int originalHeight = originalBitmap.Height;

            // We're going to use the new width and height minus one a lot so lets 
            // pre-calculate that once to save some more time
            int newWidthMinusOne = newWidth - 1;
            int newHeightMinusOne = newHeight - 1;

            // To grab the raw bitmap data into a BitmapData object we need to
            // "lock" the data (bits that make up the image) into system memory.
            // We lock the source image as ReadOnly and the destination image
            // as WriteOnly and hope that the .NET Framework can perform some
            // sort of optimization based on this.
            // Note that this piece of code relies on the PixelFormat of the 
            // images to be 32 bpp (bits per pixel). We're not interested in 
            // the order of the components (red, green, blue and alpha) as 
            // we're going to copy the entire 32 bits as they are.
            System.Drawing.Imaging.BitmapData originalData = originalBitmap.LockBits(
                new Rectangle(0, 0, originalWidth, originalHeight),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppRgb);
            System.Drawing.Imaging.BitmapData rotatedData = rotatedBitmap.LockBits(
                new Rectangle(0, 0, rotatedBitmap.Width, rotatedBitmap.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppRgb);

            // We're not allowed to use pointers in "safe" code so this
            // section has to be marked as "unsafe". Cool!
            unsafe
            {
                // Grab int pointers to the source image data and the 
                // destination image data. We can think of this pointer
                // as a reference to the first pixel on the first row of the 
                // image. It's actually a pointer to the piece of memory 
                // holding the int pixel data and we're going to treat it as
                // an array of one dimension later on to address the pixels.
                int* originalPointer = (int*)originalData.Scan0.ToPointer();
                int* rotatedPointer = (int*)rotatedData.Scan0.ToPointer();

                // There are nested for-loops in all of these case statements
                // and one might argue that it would have been neater and more
                // tidy to have the switch statement inside the a single nested
                // set of for loops, doing it this way saves us up to three int 
                // to int comparisons per pixel. 
                //
                switch (rotationAngle)
                {
                    case 90:
                        for (int y = 0; y < originalHeight; ++y)
                        {
                            int destinationX = newWidthMinusOne - y;
                            for (int x = 0; x < originalWidth; ++x)
                            {
                                int sourcePosition = (x + y * originalWidth);
                                int destinationY = x;
                                int destinationPosition =
                                        (destinationX + destinationY * newWidth);
                                rotatedPointer[destinationPosition] =
                                    originalPointer[sourcePosition];
                            }
                        }
                        break;
                    case 180:
                        for (int y = 0; y < originalHeight; ++y)
                        {
                            int destinationY = (newHeightMinusOne - y) * newWidth;
                            for (int x = 0; x < originalWidth; ++x)
                            {
                                int sourcePosition = (x + y * originalWidth);
                                int destinationX = newWidthMinusOne - x;
                                int destinationPosition = (destinationX + destinationY);
                                rotatedPointer[destinationPosition] =
                                    originalPointer[sourcePosition];
                            }
                        }
                        break;
                    case 270:
                        for (int y = 0; y < originalHeight; ++y)
                        {
                            int destinationX = y;
                            for (int x = 0; x < originalWidth; ++x)
                            {
                                int sourcePosition = (x + y * originalWidth);
                                int destinationY = newHeightMinusOne - x;
                                int destinationPosition =
                                    (destinationX + destinationY * newWidth);
                                rotatedPointer[destinationPosition] =
                                    originalPointer[sourcePosition];
                            }
                        }
                        break;
                }

                // We have to remember to unlock the bits when we're done.
                originalBitmap.UnlockBits(originalData);
                rotatedBitmap.UnlockBits(rotatedData);
            }
        }

        public static Bitmap CreateSnapshot(byte alpha, Point startPosition)
        {
            Rectangle rect = Screen.PrimaryScreen.Bounds;
            rect = new Rectangle(startPosition.X, startPosition.Y, rect.Width - startPosition.X, rect.Height - startPosition.Y);

            var result = new Bitmap(rect.Width, rect.Height);

            IntPtr deviceContext = Win32.GetDC(IntPtr.Zero);

            try
            {
                using (Graphics resultGx = Graphics.FromImage(result))
                {
                    var resultHdc = resultGx.GetHdc();

                    Win32.BitBlt(resultHdc, 0, 0, rect.Width, rect.Height, deviceContext, startPosition.X, startPosition.Y, TernaryRasterOperations.SRCCOPY);
                    resultGx.ReleaseHdc(resultHdc);
                }

            }
            finally
            {
                Win32.ReleaseDC(IntPtr.Zero, deviceContext);
            }

            if (alpha < 100)
            {
                //result = AdjustBrightness(result, (decimal)alpha);  
                Bitmap b = new Bitmap(rect.Width, result.Height);
                using (Graphics gxSrc = Graphics.FromImage(b))
                using (Graphics resultGx = Graphics.FromImage(result))
                {
                    gxSrc.Clear(Color.Black);
                    IntPtr hdcDst = resultGx.GetHdc();
                    IntPtr hdcSrc = gxSrc.GetHdc();
                    BlendFunction blendFunction = new BlendFunction();
                    blendFunction.BlendOp = 0; // Only supported blend operation
                    blendFunction.BlendFlags = 0; // Documentation says put 0 here
                    blendFunction.SourceConstantAlpha = 50;    // Constant alpha factor
                    blendFunction.AlphaFormat = 0;              // Don't look for per pixel alpha
                    Win32.AlphaBlend(hdcDst, 0, 0, rect.Width, rect.Height,
                                     hdcSrc, 0, 0, rect.Width,
                                     rect.Height, blendFunction);
                    gxSrc.ReleaseHdc(hdcSrc);      // Required cleanup to GetHdc()
                    resultGx.ReleaseHdc(hdcDst);    // Required cleanup to GetHdc()
                }
            }
            return result;
        }

        /// <summary>
        /// Quick copy graphics 
        /// </summary>
        /// <param name="gxSrc"></param>
        /// <param name="gxDest"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private static void CopyGraphics(Graphics gxSrc, Graphics gxDest, int width, int height)
        {
            IntPtr destDc = gxDest.GetHdc();
            IntPtr srcDc = gxSrc.GetHdc();
            BitBlt(destDc, 0, 0, width, height, srcDc, 0, 0, TernaryRasterOperations.SRCCOPY);
            gxSrc.ReleaseHdc(srcDc);
            gxDest.ReleaseHdc(destDc);
        }

        private static Color GetTransparentColor(Image image)
        {
            return ((Bitmap)image).GetPixel(image.Width - 1, image.Height - 1);
        }

        #endregion

        #region Drawing extension methods

        public static Bitmap CreateAlphaBitmap(byte alpha, Bitmap original)
        {
            if (alpha < 100)
            {
                Bitmap b = new Bitmap(original.Width, original.Height);
                using (Graphics gxSrc = Graphics.FromImage(b))
                using (Graphics resultGx = Graphics.FromImage(original))
                {
                    gxSrc.Clear(Color.Black);
                    IntPtr hdcDst = resultGx.GetHdc();
                    IntPtr hdcSrc = gxSrc.GetHdc();
                    BlendFunction blendFunction = new BlendFunction();
                    blendFunction.BlendOp = 0; // Only supported blend operation
                    blendFunction.BlendFlags = 0; // Documentation says put 0 here
                    blendFunction.SourceConstantAlpha = 50;    // Constant alpha factor
                    blendFunction.AlphaFormat = 0;              // Don't look for per pixel alpha
                    Win32.AlphaBlend(hdcDst, 0, 0, original.Width, original.Height,
                                                                    hdcSrc, 0, 0, original.Width,
                                                                    original.Height, blendFunction);
                    gxSrc.ReleaseHdc(hdcSrc);      // Required cleanup to GetHdc()
                    resultGx.ReleaseHdc(hdcDst);    // Required cleanup to GetHdc()
                }
                b.Dispose();
            }
            return original;
        }

        /// <summary>
        /// Shade image
        /// </summary>
        /// <param name="original">Bitmap</param>
        /// <param name="percent">Darkening in percents, 0 - is full black</param>
        /// <returns></returns>
        public static unsafe Bitmap AdjustBrightness(Bitmap original, decimal percent)
        {
            percent /= 100;
            byte[] newvalues = new byte[256];
            for (int i = 0; i < 256; ++i)
            {
                newvalues[i] = (byte)Math.Round(Math.Min(i * percent, (decimal)255));
                if (newvalues[i] == 0)
                {
                    newvalues[i] = 10;
                }
            }

            Bitmap b = new Bitmap(original);
            var bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                                    ImageLockMode.ReadWrite,
                                    PixelFormat.Format24bppRgb);
            int stride = bmData.Stride;
            IntPtr Scan0 = bmData.Scan0;
            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                int nOffset = stride - b.Width * 3;
                int nWidth = b.Width * 3;
                for (int y = 0; y < b.Height; ++y)
                {
                    for (int x = 0; x < nWidth; ++x)
                    {
                        p[0] = newvalues[p[0]];
                        ++p;
                    }
                    p += nOffset;
                }
            }

            b.UnlockBits(bmData);

            return b;
        }

        /// <summary>
        /// Draws the text. If the text is wider than the width you provide, we'll show
        /// as much of the text as possible with "..." at the very end.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="width"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void DrawClipped(string text, int width, int x, int y)
        {
            int[] extents;
            int maxChars;
            Size size = GetTextExtent(text, width, out extents, out maxChars);

            if (maxChars == text.Length)
                ExtTextOut(x, y, text);
            else
            {
                int xEllipse = width - GetTextExtent("...").Width;
                int numChars;
                for (numChars = maxChars; numChars > 0; numChars--)
                {
                    if (extents[numChars - 1] <= xEllipse)
                    {
                        xEllipse = extents[numChars - 1];
                        break;
                    }
                }
                ExtTextOut(x, y, text.Substring(0, numChars));
                ExtTextOut(x + xEllipse, y, "...");
            }
        }

        public int DrawText(string text, Win32.RECT rect, Win32.DT format)
        {
            if (text != null)
                return Win32.DrawText(_hdc, text, text.Length, ref rect, format);
            else
                return 0;
        }

        /// <summary>
        ///  Draws a single line of text into the window, without clipping or wrapping.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool ExtTextOut(int x, int y, string text)
        {
            return Win32.ExtTextOut(_hdc, x, y, 0, IntPtr.Zero, text,
                                    Convert.ToUInt16(text.Length), IntPtr.Zero);
        }

        /// <summary>
        /// Draw text with shadow
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="text"></param>
        /// <param name="useShadow"></param>
        /// <returns></returns>
        public bool ExtTextOut(int x, int y, string text, bool useShadow)
        {
            Color textColor = TextColor;
            ushort len = Convert.ToUInt16(text.Length);

            // draw shadow
            Win32.SetTextColor(_hdc, Win32ShadowColor);
            Win32.ExtTextOut(_hdc, x + 1, y + 1, 0, IntPtr.Zero, text, len, IntPtr.Zero);

            // draw text
            Win32.SetTextColor(_hdc, GetWin32Color(TextColor));
            return Win32.ExtTextOut(_hdc, x, y, 0, IntPtr.Zero, text, len, IntPtr.Zero);
        }

        public bool ExtTextOut(int x, int y, int width, string text)
        {
            Size size = GetTextExtent(text);
            return ExtTextOut(x + (width - size.Width) / 2, y, text);
        }

        public int FillRect(Win32.RECT rect, BrushGdi hbrush)
        {
            return Win32.FillRect(_hdc, ref rect, (IntPtr)hbrush);
        }

        public int FillRect(int x, int y, int w, int h, BrushGdi hbrush)
        {
            Win32.RECT rect = new Win32.RECT(x, y, w, h);
            return Win32.FillRect(_hdc, ref rect, (IntPtr)hbrush);
        }

        public int FillRect(Rectangle rect, BrushGdi hbrush)
        {
            return FillRect(new Win32.RECT(rect), hbrush);
        }

        public int FillRect(Rectangle rect, Color color)
        {
            using (BrushGdi br = CreateSolidBrush(color))
            {
                return FillRect(rect, br);
            }
        }

        public bool RoundRect(int left, int top, int right, int bottom, int roundW, int roundH)
        {
            return Win32.RoundRect(_hdc, left, top, right, bottom, roundW, roundH);
        }

        public void FrameRect(Rectangle rect)
        {
            BrushGdi brOld = Brush;
            Brush = GetStockBrush(Win32.StockObjects.NullBrush);
            DrawRectangle(rect);
            Brush = brOld;
        }

        /// <summary>
        /// Gradient fill
        /// </summary>
        public bool GradientFill(Rectangle rc, Color startColor, Color endColor, FillDirection fillDir)
        {
            return VKontakteWM.Components.GDI.GradientFill.Fill(_hdc, rc, startColor, endColor, fillDir);
        }

        public bool DrawLine(int x1, int y1, int x2, int y2)
        {
            Point[] points = new Point[2];
            points[0] = new Point(x1, y1);
            points[1] = new Point(x2, y2);
            return Polyline(points);
        }

        public void DrawLine(int x1, int y1, int x2, int y2, Color color)
        {
            PenGdi pen = CreatePen(Color.Gray);
            PenGdi oldPen = SelectObject(pen);

            DrawLine(x1, y1, x2, y2);

            SelectObject(oldPen);
            pen.Dispose();
        }

        public void DrawRectangle(Rectangle rect)
        {
            Win32.Rectangle(_hdc, rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        public bool Polygon(Point[] points)
        {
            return Win32.Polygon(_hdc, points, Convert.ToInt16(points.Length));
        }

        public bool Polyline(Point[] points)
        {
            return Win32.Polyline(_hdc, points, Convert.ToInt16(points.Length));
        }

        public bool CopyImageTo(Bitmap bmp, int nxDest, int dyDest, int nWidth, int nHeight, int nxSrc, int nySrc)
        {
            bool b;
            using (Graphics gxDest = Graphics.FromImage(bmp))
            {
                IntPtr hdcDest = gxDest.GetHdc();
                b = Win32.BitBlt(hdcDest, nxDest, dyDest, nWidth, nHeight, _hdc, nxSrc, nySrc, TernaryRasterOperations.SRCCOPY);
                gxDest.ReleaseHdc(hdcDest);
            }
            return b;
        }

        public bool BitBlt(int nxDest, int dyDest, int nWidth, int nHeight, Gdi src, int nxSrc, int nySrc, TernaryRasterOperations rop)
        {
            return Win32.BitBlt(_hdc, nxDest, dyDest, nWidth, nHeight, src._hdc, nxSrc, nySrc, rop);
        }

        public bool BitBlt(int nxDest, int dyDest, int nWidth, int nHeight, IntPtr hdcSrc, int nxSrc, int nySrc, TernaryRasterOperations rop)
        {
            return Win32.BitBlt(_hdc, nxDest, dyDest, nWidth, nHeight, hdcSrc, nxSrc, nySrc, rop);
        }

        public static bool BitBlt(IntPtr hdsDst, int nxDest, int dyDest, int nWidth, int nHeight, IntPtr hdcSrc, int nxSrc, int nySrc, TernaryRasterOperations rop)
        {
            return Win32.BitBlt(hdsDst, nxDest, dyDest, nWidth, nHeight, hdcSrc, nxSrc, nySrc, rop);
        }

        public bool StretchBlt(int nxDest, int dyDest, int nWidth, int nHeight, Gdi src, int nxSrc, int nySrc, int nSWidth, int nSHeight, TernaryRasterOperations rop)
        {
            return Win32.StretchBlt(_hdc, nxDest, dyDest, nWidth, nHeight, src._hdc, nxSrc, nySrc, nSWidth, nSHeight, rop);
        }

        public bool StretchBlt(int nxDest, int dyDest, int nWidth, int nHeight, IntPtr hdcSrc, int nxSrc, int nySrc, int nSWidth, int nSHeight, TernaryRasterOperations rop)
        {
            return Win32.StretchBlt(_hdc, nxDest, dyDest, nWidth, nHeight, hdcSrc, nxSrc, nySrc, nSWidth, nSHeight, rop);
        }

        /// <summary>
        /// Draw an image
        /// </summary>
        public bool DrawImage(Bitmap bmp, int x, int y)
        {
            bool b = false;
            using (Graphics gxSrc = Graphics.FromImage(bmp))
            {
                IntPtr hdcSrc = gxSrc.GetHdc();
                b = Win32.BitBlt(_hdc, x, y, bmp.Width, bmp.Height, hdcSrc, 0, 0, TernaryRasterOperations.SRCCOPY);
                gxSrc.ReleaseHdc(hdcSrc);
            }
            return b;
        }

        public bool DrawImage(Bitmap bmp, int x, int y, int maxwidth, int maxheight)
        {
            bool b = false;
            using (Graphics gxSrc = Graphics.FromImage(bmp))
            {
                IntPtr hdcSrc = gxSrc.GetHdc();
                b = Win32.BitBlt(_hdc, x, y,
                                 Math.Min(bmp.Width, maxwidth), Math.Min(bmp.Height, maxheight),
                                 hdcSrc, 0, 0, TernaryRasterOperations.SRCCOPY);
                gxSrc.ReleaseHdc(hdcSrc);
            }
            return b;
        }

        public bool DrawImage(Bitmap bmp, int x, int y, int maxwidth, int maxheight, int srcx, int srcy)
        {
            bool b = false;
            using (Graphics gxSrc = Graphics.FromImage(bmp))
            {
                IntPtr hdcSrc = gxSrc.GetHdc();
                b = Win32.BitBlt(_hdc, x, y,
                                 Math.Min(bmp.Width, maxwidth), Math.Min(bmp.Height, maxheight),
                                 hdcSrc, srcx, srcy, TernaryRasterOperations.SRCCOPY);
                gxSrc.ReleaseHdc(hdcSrc);
            }
            return b;
        }

        /// <summary>
        /// Draw an image
        /// </summary>
        public bool DrawStretchImage(Bitmap bmp, int x, int y, int width, int height)
        {
            bool b = false;
            using (Graphics gxSrc = Graphics.FromImage(bmp))
            {
                IntPtr hdcSrc = gxSrc.GetHdc();
                b = Win32.StretchBlt(_hdc, x, y, width, height, hdcSrc, 0, 0, bmp.Width, bmp.Height, TernaryRasterOperations.SRCCOPY);
                gxSrc.ReleaseHdc(hdcSrc);
            }
            return b;
        }

        public static Bitmap StretchImage(Bitmap sourceBmp, int width, int height)
        {
            if (width <= 0)
                width = sourceBmp.Width;
            if (height <= 0)
                height = sourceBmp.Height;

            Bitmap stretchedBmp = new Bitmap(width, height);
            using (Graphics gxSrc = Graphics.FromImage(sourceBmp))
            using (Graphics gxDst = Graphics.FromImage(stretchedBmp))
            {
                IntPtr hdcSrc = gxSrc.GetHdc();
                IntPtr hdcDst = gxDst.GetHdc();
                Win32.StretchBlt(hdcDst, 0, 0, width, height, hdcSrc, 0, 0, sourceBmp.Width, sourceBmp.Height, TernaryRasterOperations.SRCCOPY);
                gxDst.ReleaseHdc(hdcDst);
                gxSrc.ReleaseHdc(hdcSrc);
            }
            return stretchedBmp;
        }

        /// <summary>
        /// Tile background image to specified size
        /// </summary>
        public static Bitmap PrepareBackground(Bitmap bmp, int width, int height)
        {
            if (bmp == null)
                return null;

            if (bmp.Height < height || bmp.Width < width)
                return Gdi.TileImage(bmp, width, height);
            else
                return bmp;
        }

        /// <summary>
        /// Tile image to new size
        /// </summary>
        /// <param name="sourceBmp">Source image</param>
        /// <param name="width">Multiply source image to this new width</param>
        /// <param name="height">Multiply source image to this new height</param>
        /// <returns>Tiled image with new size</returns>
        public static Bitmap TileImage(Bitmap sourceBmp, int width, int height)
        {
            if (width <= 0)
                width = sourceBmp.Width;
            if (height <= 0)
                height = sourceBmp.Height;

            Bitmap tiledBmp = new Bitmap(width, height);
            using (Graphics gxSrc = Graphics.FromImage(sourceBmp))
            using (Graphics gxDst = Graphics.FromImage(tiledBmp))
            {
                ////TODO: (apih) for test purposes
                //gxDst.Clear(Color.Magenta);

                int x = 0;
                IntPtr hdcSrc = gxSrc.GetHdc();
                IntPtr hdcDst = gxDst.GetHdc();
                while (x < width)
                {
                    BitBlt(hdcDst, x, 0, sourceBmp.Width, sourceBmp.Height, hdcSrc, 0, 0, TernaryRasterOperations.SRCCOPY);
                    x += sourceBmp.Width;
                }
                int y = 0;
                while (y < height)
                {
                    BitBlt(hdcDst, 0, y, width, sourceBmp.Height, hdcDst, 0, 0, TernaryRasterOperations.SRCCOPY);
                    y += sourceBmp.Height;
                }
                gxDst.ReleaseHdc(hdcDst);
                gxSrc.ReleaseHdc(hdcSrc);
            }
            return tiledBmp;
        }

        /// <summary>
        /// Draw an image with full transparency
        /// </summary>
        /// <param name="image"></param>
        /// <param name="transp">Transparency index</param>
        /// <param name="x">X coord to draw the image</param>
        /// <param name="y">Y coord to draw the image</param>
        /// <returns></returns>
        public int DrawAlpha(Bitmap image, byte transp, int x, int y)
        {
            using (Graphics gxSrc = Graphics.FromImage(image))
            {
                IntPtr hdcSrc = gxSrc.GetHdc();
                return DrawAlpha(hdcSrc, transp, x, y, image.Width, image.Height);
            }
        }

        /// <summary>
        /// Draw an image with full transparency
        /// </summary>
        /// <param name="gdi"></param>
        /// <param name="transp">Transparency index</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public int DrawAlpha(Gdi gdi, byte transp, int x, int y, int width, int height)
        {
            return DrawAlpha(gdi._hdc, transp, x, y, width, height);
        }

        /// <summary>
        /// Draw an image with full transparency
        /// </summary>
        /// <param name="hdc"></param>
        /// <param name="transp">Constant alpha factor</param>
        /// <param name="x">X coord to draw the image</param>
        /// <param name="y">Y coord to draw the image</param>
        /// <returns></returns>
        private int DrawAlpha(IntPtr hdc, byte transp, int x, int y, int width, int height)
        {
            var blendFunction = new BlendFunction
            {
                BlendOp = ((byte)BlendOperation.AC_SRC_OVER),		// Only supported blend operation
                BlendFlags = 0,										// Documentation says put 0 here
                SourceConstantAlpha = transp,						// Constant alpha factor
                AlphaFormat = (byte)0
            };

            return Win32.AlphaBlend(_hdc, x, y, width, height, hdc, 0, 0, width, height, blendFunction);
        }


        //public Int32 BltAlpha(int left, int top, int width, int height, Image source, byte transp)
        //{
        //  using (Bitmap bmp = new Bitmap(source))
        //  {
        //    // Get DC handle and create a compatible one
        //    IntPtr memdc = CreateCompatibleDC();

        //    // Select our bitmap in to DC, recording what was there before
        //    IntPtr hBitmap = bmp.GetHbitmap();
        //    IntPtr oldObject = Win32.SelectObject(memdc, hBitmap);

        //    var blendFunction = new BlendFunction
        //    {
        //      BlendOp = ((byte)Win32.BlendOperation.AC_SRC_OVER),
        //      BlendFlags = 0,
        //      SourceConstantAlpha = transp,
        //      AlphaFormat = 0
        //    };

        //    Int32 rv = Win32.AlphaBlend(m_hdc, left, top, width, height,
        //                                memdc, 0, 0, width, height, blendFunction);

        //    // Select our bitmap object back out of the DC
        //    Win32.SelectObject(memdc, oldObject);

        //    // Delete our bitmap
        //    Win32.DeleteObject(hBitmap);

        //    // Delete memory DC and release our DC handle
        //    Win32.DeleteDC(memdc);

        //    return rv;
        //  }
        //}

        public bool TransparentImage(int xDestOrg, int yDestOrg, int dxDest, int dyDest, IntPtr hdcSrc, int xSrcOrg, int ySrcOrg, int dxSrc, int dySrc, Color color)
        {
            return Win32.TransparentImage(_hdc, xDestOrg, yDestOrg, dxDest, dyDest, hdcSrc, xSrcOrg, ySrcOrg, dxSrc, dySrc,
                                          Win32.Color2ColorRef(color));
        }

        public void DrawImageAlphaChannel(IImage image, int x, int y)
        {
            ImageInfo imageInfo = new ImageInfo();
            image.GetImageInfo(out imageInfo);
            Rectangle rc = new Rectangle(x, y, (int)imageInfo.Width + x, (int)imageInfo.Height + y);
            //Rectangle rcSrc = new Rectangle();
            image.Draw(_hdc, ref rc, IntPtr.Zero);
        }

        public void DrawImageAlphaChannel(IImage image, Rectangle dest)
        {
            Rectangle rc = new Rectangle(dest.X, dest.Y, dest.Width + dest.X, dest.Height + dest.Y);
            image.Draw(_hdc, ref rc, IntPtr.Zero);
        }

        public void DrawImageAlphaChannelTiled(IImage image, int x, int y, bool horizontal, int maxwidth)
        {
            ImageInfo imageInfo = new ImageInfo();
            image.GetImageInfo(out imageInfo);

            if (horizontal)
            {
                int width = Math.Max((int)imageInfo.Width, maxwidth);
                Rectangle rc = new Rectangle(x, y, x + width, (int)imageInfo.Height + y);
                image.Draw(_hdc, ref rc, IntPtr.Zero);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /*
		/// <summary>
		/// Draws gradient filled rounded rectangle with transparency
		/// </summary>
		/// <param name="gx">Destination graphics</param>
		/// <param name="rc">Destination rectangle</param>
		/// <param name="startColorValue">Starting color for gradient</param>
		/// <param name="endColorValue">End color for gradient</param>
		/// <param name="borderColor">The color of the border</param>
		/// <param name="size">The size of the rounded corners</param>
		/// <param name="transparency">Transparency constant</param>
		public void DrawGradientRoundedRectangleAlpha(Rectangle rc, Color startColorValue, Color endColorValue, Color borderColor, Size size, byte transparency)
		{
			// Prepare image for gradient
			Bitmap gradientImage = new Bitmap(rc.Width, rc.Height);
			// Create temporary graphics
			Graphics gxGradient = Graphics.FromImage(gradientImage);
			// This is our rectangle
			Rectangle roundedRect = new Rectangle(0, 0, rc.Width, rc.Height);
			// Fill in gradient
			GradientFill.Fill(
				gxGradient,
				roundedRect,
				startColorValue,
				endColorValue,
				FillDirection.TopToBottom);

			// Prepare the copy of the screen graphics
			Bitmap tempBitmap = new Bitmap(240, 320);
			Graphics tempGx = Graphics.FromImage(tempBitmap);
			// Copy from the screen's graphics to the temp graphics
			CopyGraphics(gx, tempGx, 240, 320);
			// Draw the gradient image with transparency on the temp graphics
			tempGx.DrawAlpha(gradientImage, transparency, rc.X, rc.Y);
			// Cut out the transparent image 
			gxGradient.DrawImage(tempBitmap, new Rectangle(0, 0, rc.Width, rc.Height), rc, GraphicsUnit.Pixel);
			// Prepare for imposing
			roundedRect.Width--;
			roundedRect.Height--;
			// Impose the rounded rectangle with transparent color
			Bitmap borderImage = ImposeRoundedRectangle(roundedRect, borderColor, size);
			// Draw the transparent rounded rectangle
			ImageAttributes attrib = new ImageAttributes();
			attrib.SetColorKey(Color.Green, Color.Green);
			gxGradient.DrawImage(borderImage, new Rectangle(0, 0, rc.Width, rc.Height), 0, 0, borderImage.Width, borderImage.Height, GraphicsUnit.Pixel, attrib);
			// OK... now are ready to draw the final image on the original graphics
			gx.DrawImageTransparent(gradientImage, rc);

			// Clean up
			attrib.Dispose();
			tempGx.Dispose();
			tempBitmap.Dispose();
			gradientImage.Dispose();
			gxGradient.Dispose();
		}

		/// <summary>
		/// Draws the image with transparency
		/// </summary>
		/// <param name="gx">Destination graphics</param>
		/// <param name="image">The image to draw</param>
		/// <param name="destRect">Desctination rectangle</param>
		public static void DrawImageTransparent(this Graphics gx, Image image, Rectangle destRect)
		{
			ImageAttributes imageAttr = new ImageAttributes();
			Color transpColor = GetTransparentColor(image);
			imageAttr.SetColorKey(transpColor, transpColor);
			gx.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttr);
			imageAttr.Dispose();
		}

		/// <summary>
		/// Returns the rectangle filled with gradient colors
		/// </summary>
		/// <param name="rc">Destination rectangle</param>
		/// <param name="startColorValue">Starting color for gradient</param>
		/// <param name="endColorValue">End color for gradient</param>
		/// <param name="fillDirection">The direction of the gradient</param>
		/// <returns>Image of the rectanle</returns>
		public static Bitmap GetGradientRectangle(Rectangle rc, Color startColorValue, Color endColorValue, FillDirection fillDirection)
		{
			Bitmap outputImage = new Bitmap(rc.Width, rc.Height);
			// Create temporary graphics
			Graphics gx = Graphics.FromImage(outputImage);

			GradientFill.Fill(
			  gx,
			  rc,
			  startColorValue,
			  endColorValue,
			  fillDirection);

			return outputImage;
		}

		/// <summary>
		/// Fills the rectagle with gradient colors
		/// </summary>
		/// <param name="gx">Destination graphics</param>
		/// <param name="rc">Desctination rectangle</param>
		/// <param name="startColorValue">Starting color for gradient</param>
		/// <param name="endColorValue">End color for gradient</param>
		/// <param name="fillDirection">The direction of the gradient</param>
		public static void FillGradientRectangle(this Graphics gx, Rectangle rc, Color startColorValue, Color endColorValue, FillDirection fillDirection)
		{
			GradientFill.Fill(
			  gx,
			  rc,
			  startColorValue,
			  endColorValue,
			  fillDirection);
		}

		/// <summary>
		/// Draws gradient filled rounded rectangle
		/// </summary>
		/// <param name="gx">Destination graphics</param>
		/// <param name="rc">Desctination rectangle</param>
		/// <param name="startColorValue">Starting color for gradient</param>
		/// <param name="endColorValue">End color for gradient</param>
		/// <param name="borderColor">The color of the border</param>
		/// <param name="size">The size of the rounded corners</param>
		public static void DrawGradientRoundedRectangle(this Graphics gx, Rectangle rc, Color startColorValue, Color endColorValue, Color borderColor, Size size)
		{
			Bitmap bitmap = GetGradiendRoundedRectangle(new Rectangle(0, 0, rc.Width, rc.Height), startColorValue, endColorValue, borderColor, size);
			gx.DrawImageTransparent(bitmap, rc);
		}

		/// <summary>
		/// Returns the image of the gradient filled rounded rectangle
		/// </summary>     
		/// <param name="rc">Desctination rectangle</param>
		/// <param name="startColorValue">Starting color for gradient</param>
		/// <param name="endColorValue">End color for gradient</param>
		/// <param name="borderColor">The color of the border</param>
		/// <param name="size">The size of the rounded corners</param>
		/// <returns>Bitmap image</returns>
		public static Bitmap GetGradiendRoundedRectangle(Rectangle rc, Color startColorValue, Color endColorValue, Color borderColor, Size size)
		{
			Bitmap outputImage = new Bitmap(rc.Width, rc.Height);
			// Create temporary graphics
			Graphics gx = Graphics.FromImage(outputImage);

			GradientFill.Fill(
				gx,
				rc,
				startColorValue,
				endColorValue,
				FillDirection.TopToBottom);

			Rectangle roundedRect = rc;
			roundedRect.Width--;
			roundedRect.Height--;

			Bitmap borderImage = ImposeRoundedRectangle(roundedRect, borderColor, size);

			ImageAttributes attrib = new ImageAttributes();
			attrib.SetColorKey(Color.Green, Color.Green);

			gx.DrawImage(borderImage, rc, 0, 0, borderImage.Width, borderImage.Height, GraphicsUnit.Pixel, attrib);

			// Clean up
			attrib.Dispose();
			gx.Dispose();

			return outputImage;
		}

		/// <summary>
		/// Returns the image of the gradient filled rounded rectangle
		/// </summary>     
		/// <param name="rc">Desctination rectangle</param>
		/// <param name="startColorValue">Starting color for gradient</param>
		/// <param name="endColorValue">End color for gradient</param>
		/// <param name="borderColor">The color of the border</param>
		/// <param name="size">The size of the rounded corners</param>
		/// <returns>Bitmap image</returns>
		public static Bitmap GradiendRoundedRectangle2(Rectangle rc, Color startColorValue, Color endColorValue, Color borderColor, Size size)
		{
			Bitmap outputImage = new Bitmap(rc.Width, rc.Height);
			Graphics gx = Graphics.FromImage(outputImage);

			Rectangle rectTopMiddle = new Rectangle(0, 0, rc.Width, rc.Height / 2);

			Rectangle rectMiddleBottom = new Rectangle(0, rc.Height / 2, rc.Width, rc.Height / 2);


			GradientFill.Fill(
				gx,
				rectTopMiddle,
				endColorValue,
				startColorValue,
				FillDirection.TopToBottom);

			GradientFill.Fill(
			   gx,
			   rectMiddleBottom,
			   startColorValue,
			   endColorValue,
			   FillDirection.TopToBottom);


			Rectangle roundedRect = rc;
			roundedRect.Width--;
			roundedRect.Height--;

			Bitmap borderImage = ImposeRoundedRectangle(roundedRect, Color.Green, size);

			ImageAttributes attrib = new ImageAttributes();
			attrib.SetColorKey(Color.Green, Color.Green);
			gx.DrawImage(borderImage, rc, 0, 0, borderImage.Width, borderImage.Height, GraphicsUnit.Pixel, attrib);

			attrib.Dispose();
			gx.Dispose();

			return outputImage;
		}

		private static Bitmap ImposeRoundedRectangle(Rectangle rc, Color borderColor, Size size)
		{

			Bitmap bitmap = new Bitmap(rc.Width + 1, rc.Height + 1);
			//Create temp graphics
			Graphics gxTemp = Graphics.FromImage(bitmap);
			gxTemp.Clear(Color.White);
			DrawRoundedRectangle(gxTemp, borderColor, Color.Green, rc, size);
			gxTemp.Dispose();
			return bitmap;
		}

		/// <summary>
		/// Draws rounded rectangle
		/// </summary>
		/// <param name="gx">Destination graphics</param>
		/// <param name="pen">The pen to draw</param>
		/// <param name="backColor"></param>
		/// <param name="rc"></param>
		/// <param name="size"></param>
		public static void DrawRoundedRectangle(this Graphics gx, Color borderColor, Color backColor, Rectangle rc, Size size)
		{
			Pen borderPen = new Pen(borderColor);
			DrawRoundedRect(gx, borderPen, backColor, rc, size);
			borderPen.Dispose();
		}

		*/

        public void DrawRectandleAlpha(Color borderColor, Color backColor, Rectangle rc, byte transparency)
        {
            using (Bitmap tempBitmap = new Bitmap(rc.Width, rc.Height))
            using (Graphics tempGraphics = Graphics.FromImage(tempBitmap))
            {
                using (Brush backColorBrush = new SolidBrush(backColor))
                {
                    tempGraphics.FillRectangle(backColorBrush, new Rectangle(0, 0, rc.Width, rc.Height));
                }

                using (Pen borderPen = new Pen(borderColor))
                {
                    tempGraphics.DrawRectangle(borderPen, new Rectangle(0, 0, rc.Width, rc.Height));
                }

                DrawAlpha(tempBitmap, transparency, rc.X, rc.Y);
            }
        }

        /*
        public static void DrawRoundedRectangleAlpha(this Graphics gx, Color borderColor, Color backColor, Rectangle rc, Size size, byte transparency)
        {
            Pen borderPen = new Pen(borderColor);
            ////Create temp bitmap
            //Bitmap tempBitmap = new Bitmap(rc.Width, rc.Height);
            //Graphics tempGraphics = Graphics.FromImage(tempBitmap);
            //DrawRoundedRect(tempGraphics, borderPen, backColor, new Rectangle(0, 0, rc.Width, rc.Width), size);
            //gx.DrawAlpha(tempBitmap, transparency, rc.X, rc.Y);

            ////Clean up
            //borderPen.Dispose();
            //tempGraphics.Dispose();
            //tempBitmap.Dispose();

            // Prepare image for gradient
            Bitmap roundedImage = new Bitmap(rc.Width, rc.Height);
            // Create temporary graphics
            Graphics gxRounded = Graphics.FromImage(roundedImage);
            // This is our rectangle
            Rectangle roundedRect = new Rectangle(0, 0, rc.Width, rc.Height);
            // Draw rounded rect
            DrawRoundedRect(gxRounded, borderPen, backColor, new Rectangle(0, 0, rc.Width, rc.Width), size);

            // Prepare the copy of the screen graphics
            Bitmap tempBitmap = new Bitmap(240, 320);
            Graphics tempGx = Graphics.FromImage(tempBitmap);
            // Copy from the screen's graphics to the temp graphics
            CopyGraphics(gx, tempGx, 240, 320);
            // Draw the gradient image with transparency on the temp graphics
            tempGx.DrawAlpha(roundedImage, transparency, rc.X, rc.Y);
            // Cut out the transparent image 
            gxRounded.DrawImage(tempBitmap, new Rectangle(0, 0, rc.Width, rc.Height), rc, GraphicsUnit.Pixel);
            // Prepare for imposing
            roundedRect.Width--;
            roundedRect.Height--;
            // Impose the rounded rectangle with transparent color
            Bitmap borderImage = ImposeRoundedRectangle(roundedRect, borderColor, size);
            // Draw the transparent rounded rectangle
            ImageAttributes attrib = new ImageAttributes();
            attrib.SetColorKey(Color.Green, Color.Green);
            gxRounded.DrawImage(borderImage, new Rectangle(0, 0, rc.Width, rc.Height), 0, 0, borderImage.Width, borderImage.Height, GraphicsUnit.Pixel, attrib);
            // OK... now are ready to draw the final image on the original graphics
            gx.DrawImageTransparent(roundedImage, rc);

            // Clean up
            attrib.Dispose();
            tempGx.Dispose();
            tempBitmap.Dispose();
            roundedImage.Dispose();
            gxRounded.Dispose();
        }


        public static void DrawRoundedRect(Graphics g, Pen p, Color backColor, Rectangle rc, Size size)
        {
            Point[] points = new Point[8];

            //prepare points for poligon
            points[0].X = rc.Left + size.Width / 2;
            points[0].Y = rc.Top + 1;
            points[1].X = rc.Right - size.Width / 2;
            points[1].Y = rc.Top + 1;

            points[2].X = rc.Right;
            points[2].Y = rc.Top + size.Height / 2;
            points[3].X = rc.Right;
            points[3].Y = rc.Bottom - size.Height / 2;

            points[4].X = rc.Right - size.Width / 2;
            points[4].Y = rc.Bottom;
            points[5].X = rc.Left + size.Width / 2;
            points[5].Y = rc.Bottom;

            points[6].X = rc.Left + 1;
            points[6].Y = rc.Bottom - size.Height / 2;
            points[7].X = rc.Left + 1;
            points[7].Y = rc.Top + size.Height / 2;

            //prepare brush for background
            Brush fillBrush = new SolidBrush(backColor);

            //draw side lines and circles in the corners
            g.DrawLine(p, rc.Left + size.Width / 2, rc.Top,
             rc.Right - size.Width / 2, rc.Top);

            g.FillEllipse(fillBrush, rc.Right - size.Width, rc.Top,
             size.Width, size.Height);

            g.DrawEllipse(p, rc.Right - size.Width, rc.Top,
            size.Width, size.Height);


            g.DrawLine(p, rc.Right, rc.Top + size.Height / 2,
             rc.Right, rc.Bottom - size.Height / 2);

            g.FillEllipse(fillBrush, rc.Right - size.Width, rc.Bottom - size.Height,
            size.Width, size.Height);

            g.DrawEllipse(p, rc.Right - size.Width, rc.Bottom - size.Height,
             size.Width, size.Height);



            g.DrawLine(p, rc.Right - size.Width / 2, rc.Bottom,
             rc.Left + size.Width / 2, rc.Bottom);

            g.FillEllipse(fillBrush, rc.Left, rc.Bottom - size.Height,
            size.Width, size.Height);

            g.DrawEllipse(p, rc.Left, rc.Bottom - size.Height,
             size.Width, size.Height);



            g.DrawLine(p, rc.Left, rc.Bottom - size.Height / 2,
             rc.Left, rc.Top + size.Height / 2);
            g.FillEllipse(fillBrush, rc.Left, rc.Top,
             size.Width, size.Height);

            g.DrawEllipse(p, rc.Left, rc.Top,
            size.Width, size.Height);

            //fill the background and remove the internal arcs  
            g.FillPolygon(fillBrush, points);
            //dispose the brush
            fillBrush.Dispose();
        } 
        */

        #endregion
    }
}