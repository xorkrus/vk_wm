/*	 This file was created by GALS Soft	company
 *	 http://www.galssoft.com
 *	 
 *	 Project name is Odnoklassniki Mobile
 *	 Platform is .Net Compact Framework 2.0 for Windows Mobile 5.0+ 
 * 
 *	 Copyright (c) 2009-2010 GALS Soft
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.GDI;

namespace Galssoft.VKontakteWM.Components.UI.CompoundControls
{
    public abstract partial class KineticControlBase : Control, IDisposable
    {
        #region Public enums

        public enum KineticControlScrollType
        {
            Vertical,
            Horizontal
        }

        public enum KineticControlScrollAction
        {
            ScrollingForTime, //Прокручивать после отпускания с уменьщающейся скоростью
            ArrangeTopLeft //Выравнивать на первый видимый элемент и не прокучивать со скоростью.
        }

        #endregion

        #region Data Memebers

        public readonly int SCROLL_THRESHOLD = 12;
        public readonly int SCROLL_BEGIN_THRESHOLD = 10;
        public const int SCROLL_TO_PERIOD = 300;
        public const int REFRESH_RATE = 11;
        public const double FRICTION_COEFF = 0.002F;

        private readonly Timer _ScrollTimer = new Timer();
        private readonly Timer _ScrollToTimer = new Timer();

        // Scrolling support
        protected bool _IsDragging = false;
        protected bool _IsScrolling = false;
        private bool _IsScrollingOnButtonDown = false;

        protected KineticControlScrollType ScrollType
        {
            get { return _scrollType; }
        }
        private readonly KineticControlScrollType _scrollType;

        protected int CurrentScrollPosition
        {
            get { return _currentScrollPosition; }
        }
        private int _currentScrollPosition;

        protected int LastMouseX
        {
            get { return _LastMouseX; }
        }
        private int _LastMouseX;

        protected int LastMouseY
        {
            get { return _LastMouseY; }
        }
        private int _LastMouseY;

        private int _MouseStartTime;
        private int _MouseEndTime;
        private double _ScrollVelocity = 0;

        // Scroll To
        private int _ScrollStartTime = 0;
        private int _ScrollStartPosition = 0;
        private int _ScrollChange = 0;
        private double _ScrollDuration = 0.0;

        protected Point MouseDownPosition
        {
            get { return _MouseDownPosition; }
        }
        private Point _MouseDownPosition = new Point(-1, -1);

        protected int _SelectedIndex = -2;

        private readonly Win32.WindowProcCallback _delegate;
        private readonly IntPtr _wndprocReal; // the original wndproc of

        private static Bitmap _offScreenBmp;
        private static Graphics _offScreenGraphics;
        protected Rectangle ContentRectangle;

        protected int _ActiveListWidth = 0;
        protected int _ActiveListHeight = 0;
        protected List<int> StartPositions = new List<int>();

        #endregion

        #region Events

        private event EventHandler _onScrollToEnding;
        public event EventHandler OnScrollToEnding
        {
            add
            {
                _onScrollToEnding += value;
            }
            remove
            {
                _onScrollToEnding -= value;
            }
        }

        #endregion

        public bool AllowScrollOverContent
        {
            get { return _allowScrollOverContent; }
            set { _allowScrollOverContent = value; }
        }
        private bool _allowScrollOverContent = true;

        public KineticControlScrollAction ScrollAction
        {
            get { return _scrollAction; }
            set { _scrollAction = value; }
        }
        private KineticControlScrollAction _scrollAction = KineticControlScrollAction.ScrollingForTime;

        public abstract int ItemCount
        {
            get;
        }

        protected int MaxScrollPosition
        {
            get
            {
                if (_scrollType == KineticControlScrollType.Vertical)
                    return _ActiveListHeight > ContentRectangle.Height ? _ActiveListHeight - ContentRectangle.Height : 0;
                else
                    return _ActiveListWidth > ContentRectangle.Width ? _ActiveListWidth - ContentRectangle.Width : 0;
            }
        }

        protected static int MinScrollPosition
        {
            get
            {
                return 0;
            }
        }

        public KineticControlBase(KineticControlScrollType scrollType)
        {
            InitializeComponent();

            ContentRectangle = ClientRectangle;

            _ScrollTimer.Tick += OnScrollTick;
            _ScrollToTimer.Tick += OnScrollToTick;

            _scrollType =scrollType;
            SCROLL_THRESHOLD = UISettings.CalcPix(SCROLL_THRESHOLD);
            SCROLL_BEGIN_THRESHOLD = UISettings.CalcPix(SCROLL_BEGIN_THRESHOLD);

            _delegate = WnProc;
            _wndprocReal = Win32.SetWindowLong(Handle, Win32.GWL_WNDPROC,
                                               Marshal.GetFunctionPointerForDelegate(_delegate));
        }

        /// <param name="retainOffsetBmpOnDispose">Not to dispose offset buffer after Dispose() method</param>
        /// <remarks>
        /// It's very dangerous to retain offset buffer after Dispose(). May be leak!
        /// </remarks>
        public KineticControlBase(KineticControlScrollType scrollType, bool retainOffsetBufferOnDispose)
            : this(scrollType)
        {
            _retainOffsetBufferOnDispose = retainOffsetBufferOnDispose;
        }


        //protected virtual void OnCustomizeControlSettings(ListViewSettings settings) { }

        private int WnProc(IntPtr hwnd, uint msg, uint wParam, IntPtr lParam)
        {
            bool handled = false;
            if (msg == Win32.WM_PAINT)
            {
                handled = OnPaint(hwnd);
            }

            return !handled ? Win32.CallWindowProc(_wndprocReal, hwnd, msg, wParam, lParam) : 0;
        }

        #region Scrolling

        public void ScrollTo(int toPos)
        {
            ScrollTo(toPos, SCROLL_TO_PERIOD);
        }

        private void ScrollBar(int posPoint)
        {
            _IsScrolling = true;
            _ScrollVelocity = 20;

            int maxScrolled;
            double pct;
            if (_scrollType == KineticControlScrollType.Vertical)
            {
                maxScrolled = _ActiveListHeight <= ContentRectangle.Height ? 0 : _ActiveListHeight - ContentRectangle.Height;
                pct = (double)(posPoint - ContentRectangle.Top) / (double)ContentRectangle.Height;                
            }
            else
            {
                maxScrolled = _ActiveListWidth <= ContentRectangle.Width ? 0 : _ActiveListWidth - ContentRectangle.Width;
                pct = (double)(posPoint - ContentRectangle.Left) / (double)ContentRectangle.Width;
            }
            
            _currentScrollPosition = (int)(maxScrolled * pct);
            _currentScrollPosition = Math.Min(_currentScrollPosition, maxScrolled);
            _currentScrollPosition = Math.Max(_currentScrollPosition, 0);
        }

        public void ScrollTo(int position, int duration)
        {
            int minScrolled = 0;
            int maxScrolled;

            if (_scrollType == KineticControlScrollType.Vertical)
                maxScrolled = _ActiveListHeight <= ContentRectangle.Height ? 0 : _ActiveListHeight - ContentRectangle.Height;
            else
                maxScrolled = _ActiveListWidth <= ContentRectangle.Width ? 0 : _ActiveListWidth - ContentRectangle.Width;

            if (position < minScrolled)
                position = minScrolled;

            if (position > maxScrolled)
                position = maxScrolled;

            _ScrollStartPosition = _currentScrollPosition;
            _ScrollChange = position - _ScrollStartPosition;
            _ScrollDuration = duration;
            _ScrollStartTime = Environment.TickCount;
            
            if(duration == 0)
            {
                _IsScrolling = false;
                _IsScrollingOnButtonDown = false;
                _ScrollToTimer.Enabled = false;
                _currentScrollPosition = _ScrollChange + _ScrollStartPosition;
                if (_onScrollToEnding != null)
                    _onScrollToEnding(this, EventArgs.Empty);
            }
            else
            {
                _ScrollToTimer.Interval = REFRESH_RATE;
                _ScrollToTimer.Enabled = true;                
            }
        }

        protected void ScrollItemIntoView(int index)
        {
            Rectangle r = GetItemRectangle(index);
            if (r == Rectangle.Empty)
            {
                _currentScrollPosition = 0;
            }
            else if (_IsScrolling)
            {
                if (_scrollType == KineticControlScrollType.Vertical)
                    _currentScrollPosition = Math.Max(0, Math.Min(StartPositions[index], _ActiveListHeight - ContentRectangle.Height));
                else
                    _currentScrollPosition = Math.Max(0, Math.Min(StartPositions[index], _ActiveListWidth - ContentRectangle.Width));
            }
            else
            {
                if (_scrollType == KineticControlScrollType.Vertical)
                {
                    if (r.Top < _currentScrollPosition)
                        _currentScrollPosition = Math.Min(r.Top - index > 0 ? ShiftForScrollIntoView : 0, _ActiveListHeight - ContentRectangle.Height);

                    if (r.Bottom > _currentScrollPosition + ContentRectangle.Height)
                        _currentScrollPosition = Math.Max(0, r.Bottom - ContentRectangle.Height);                    
                }
                else
                {
                    if (r.Left < _currentScrollPosition)
                        _currentScrollPosition = Math.Min(r.Left - index > 0 ? ShiftForScrollIntoView : 0, _ActiveListWidth - ContentRectangle.Width);

                    if (r.Right > _currentScrollPosition + ContentRectangle.Width)
                        _currentScrollPosition = Math.Max(0, r.Right - ContentRectangle.Width); 
                }
            }

            RefreshControl();
        }

        public void ScrollItemAnimated(int index)
        {
            int pos = _currentScrollPosition;
            Rectangle r = GetItemRectangle(index);
            if (r == Rectangle.Empty)
            {
                pos = 0;
            }
            else if (_IsScrolling)
            {
                if (_scrollType == KineticControlScrollType.Vertical)
                    pos = Math.Max(0, Math.Min(StartPositions[index], _ActiveListHeight - ContentRectangle.Height));
                else
                    pos = Math.Max(0, Math.Min(StartPositions[index], _ActiveListWidth- ContentRectangle.Width));
            }
            else
            {
                if (_scrollType == KineticControlScrollType.Vertical)
                {
                    if (r.Top < pos)
                        pos = Math.Min(r.Top - index > 0 ? ShiftForScrollIntoView : 0, _ActiveListHeight - ContentRectangle.Height);

                    if (r.Bottom > pos + ContentRectangle.Height)
                        pos = Math.Max(0, r.Bottom - ContentRectangle.Height);                    
                }
                else
                {
                    if (r.Left < pos)
                        pos = Math.Min(r.Left - index > 0 ? ShiftForScrollIntoView : 0, _ActiveListWidth - ContentRectangle.Width);

                    if (r.Right > pos + ContentRectangle.Width)
                        pos = Math.Max(0, r.Right - ContentRectangle.Width);
                }
            }

            ScrollTo(pos, 2000);
        }


        /// <summary>
        /// Make minor changes for scrolling to item 
        /// </summary>
        /// <returns></returns>
        protected virtual int ShiftForScrollIntoView
        {
            get { return 0; }
        }

        protected void ResetScrollPosition()
        {
            _currentScrollPosition = 0;
        }

        #endregion

        #region Control Event Handlers

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        protected bool OnPaint(IntPtr hWnd)
        {
            var ps = new Win32.PAINTSTRUCT();

            if (OffScreenGraphics == null)
                return false;

            var hdc = Win32.BeginPaint(hWnd, ref ps);
            var hdcMem = OffScreenGraphics.GetHdc();

            try
            {
                using (Gdi g = Gdi.FromHdc(hdc, ps.rcPaint))
                {
                    using (Gdi gMem = Gdi.FromHdc(hdcMem, Rectangle.Empty))
                    {
                        Rectangle rect = ps.rcPaint;

                        try
                        {
                            DrawScreenOn(gMem, rect, _currentScrollPosition);
                        }
                        catch (COMException ex)
                        {
                            throw;
                        }
                        catch (Exception)
                        {
                            throw;
                        }

                        g.BitBlt(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top, gMem, rect.Left, rect.Top,
                                 TernaryRasterOperations.SRCCOPY);
                    }
                }
            }
            finally
            {
                OffScreenGraphics.ReleaseHdc(hdcMem);
                Win32.EndPaint(hWnd, ref ps);
            }

            return true;
        }

        protected abstract void DrawScreenOn(Gdi mem, Rectangle rect, int position);

        public void DrawRender(Gdi g)
        {
            Rectangle rect = new Rectangle(0, 0, Width, Height);
            IntPtr hdcMem = OffScreenGraphics.GetHdc();

            using (Gdi gMem = Gdi.FromHdc(hdcMem, Rectangle.Empty))
            {
                DrawScreenOn(gMem, rect, _currentScrollPosition);

                g.BitBlt(Location.X, Location.Y, rect.Width, rect.Height, gMem, 0, 0, TernaryRasterOperations.SRCCOPY);
            }

            OffScreenGraphics.ReleaseHdc(hdcMem);
        }

        public Bitmap CreateScreenShot(Size size)
        {
            Bitmap snapShotBmp = null;
            Graphics snapShotGraphics = null;
            var hdcMem = IntPtr.Zero;

            try
            {
                snapShotBmp = new Bitmap(size.Width, size.Height);
                snapShotGraphics = Graphics.FromImage(snapShotBmp);

                if (size != ClientSize)
                    snapShotGraphics.Clear(BackColor);

                hdcMem = snapShotGraphics.GetHdc();

                using (var gMem = Gdi.FromHdc(hdcMem, Rectangle.Empty))
                {
                    DrawScreenOn(gMem, ClientRectangle, _currentScrollPosition);
                }
            }
            catch (Exception ex)
            {
                DebugHelper.WriteLogEntry(ex.ToString());
                if (snapShotBmp != null)
                {
                    snapShotBmp.Dispose();
                    snapShotBmp = null;
                }
            }
            finally
            {
                if (snapShotGraphics != null)
                {
                    //tr
                    snapShotGraphics.ReleaseHdc(hdcMem);
                    snapShotGraphics.Dispose();
                }
            }

            return snapShotBmp;
        }

        public Bitmap CreateScreenShot()
        {
            return CreateScreenShot(ClientSize);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _LastMouseX = _MouseDownPosition.X = e.X;
            _LastMouseY = _MouseDownPosition.Y = e.Y;
            _MouseStartTime = Environment.TickCount;

            //if (_LastMouseX > (ContentRectangle.Right - Settings.SecondaryIconPixWidth) && !_IsScrolling && !_IsDragging)
            //{
            //    _MouseDownItemYPos = _LastMouseY - ContentRectangle.Top;
            //    RefreshControl();
            //}

            if (_IsScrolling)
            {
                if (CurrentScrollPosition < 0 || CurrentScrollPosition > MaxScrollPosition)
                    _IsDragging = true;

                _ScrollToTimer.Enabled = false;
                _ScrollTimer.Enabled = false;
                _ScrollVelocity = 0;
                _IsScrolling = false;
                _IsScrollingOnButtonDown = true;
            }

            //if (MouseDown != null)
            //{
            //    MouseDown(e);
            //}
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            var pt = new Point(e.X, e.Y);

            if (_IsScrolling)
            {
                _IsScrolling = false;
                RefreshControl();
                return;
            }

            // They scrolled the screen up too far
            if (_currentScrollPosition <= MinScrollPosition)
            {
                //_IsDragging = false;

                if (AllowScrollOverContent)
                {
                    ScrollTo(MinScrollPosition);
                    SignalListEnd();
                }
                else
                    _currentScrollPosition = MinScrollPosition;
            }

            // They scrolled the screen down too far
            if (_currentScrollPosition >= MaxScrollPosition)
            {
                //_IsDragging = false;
                if (AllowScrollOverContent)
                {
                    ScrollTo(MaxScrollPosition);
                    SignalListEnd();
                }
                else
                    _currentScrollPosition = MaxScrollPosition;
            }

            // now we're scrolling
            if (_IsDragging && ScrollAction == KineticControlScrollAction.ScrollingForTime)
            {
                _ScrollTimer.Interval = REFRESH_RATE;
                _ScrollTimer.Enabled = true;
                _IsScrolling = true;
                _IsDragging = false;
                return;                    
            }
            else
            {
                //Уравнивать всегда надо иначе может быть зазор в SCROLL_THRESHOLD
                if (ScrollAction == KineticControlScrollAction.ArrangeTopLeft)
                {
                    //_IsDragging = false;

                    int pos = _scrollType == KineticControlScrollType.Vertical
                                  ? _currentScrollPosition - ContentRectangle.Top
                                  : _currentScrollPosition - ContentRectangle.Left;

                    int item = GetPixelToItem(pos);
                    int nWidth = 0;

                    if (ItemCount == 1)
                    {
                        ScrollTo(StartPositions[0]);
                    }
                    else if (ItemCount > 1)
                    {
                        if (item == -1)
                            item = 0;
                        else if (item == -2)
                            item = ItemCount - 1;
                        else if (ItemCount == item - 1)
                        {
                            nWidth = StartPositions[item] - StartPositions[item - 1];
                        }
                        else
                        {
                            nWidth = StartPositions[item + 1] - StartPositions[item];
                        }

                        if (pos - nWidth / 2 >= StartPositions[item] && ItemCount != item - 1)
                        {
                            ScrollTo(StartPositions[item + 1]);
                        }
                        else
                        {
                            ScrollTo(StartPositions[item]);
                        }
                    }
                }

                if (!_IsDragging)
                {
                    if (_IsScrollingOnButtonDown)
                    {
                        _IsScrollingOnButtonDown = false;
                        RefreshControl();
                        return;
                    }

                    MouseSelectItemInternal(pt, true);
                }

                _IsDragging = false;

                //Защищенный вызов. вызывается после проверки на промежуток времени.
                //Блокируется множественные нажатия одновременно
                //if (TickCounter.CheckTickDelay())
                //{
                //    TickCounter.CreateTickStamp();
                //    MouseSelectItemInternal(pt, true);
                //    TickCounter.CreateTickStamp();
                //}
            }

            Win32.UpdateWindow(Handle);

            base.OnMouseUp(e);

            //return;
        }

        protected void MouseSelectItemInternal(Point pt, bool select)
        {
            int pos = 0;
            bool inside = _scrollType == KineticControlScrollType.Vertical
                              ? pt.Y >= ContentRectangle.Top
                              : pt.X >= ContentRectangle.Left;

            if (inside)
            {
                pos = _scrollType == KineticControlScrollType.Vertical
                          ? pt.Y + _currentScrollPosition - ContentRectangle.Top
                          : pt.X + _currentScrollPosition - ContentRectangle.Left;

                SelectItemInternal(GetPixelToItem(pos));

                Win32.InvalidateRect(Handle, ref ContentRectangle, false);
                if (select)
                    OnItemSelected(_SelectedIndex, pt);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;
            int t = Environment.TickCount;

            if (_IsScrolling)
            {
                if (_scrollType == KineticControlScrollType.Vertical)
                    ScrollBar(y);
                else
                    ScrollBar(x);

                RefreshControl();
                return;
            }

            int z = _scrollType == KineticControlScrollType.Vertical
                        ? y - _MouseDownPosition.Y
                        : x - _MouseDownPosition.X;

            if (Math.Abs(z) > SCROLL_THRESHOLD)
            {
                SelectItemInternal(-1);
                _IsDragging = true;
            }              

            // SCROLL
            if (Math.Abs(z) > SCROLL_BEGIN_THRESHOLD)
            {
                if (_scrollType == KineticControlScrollType.Vertical)
                    _currentScrollPosition = _currentScrollPosition - y + _LastMouseY;
                else
                    _currentScrollPosition = _currentScrollPosition - x + _LastMouseX;
            }

            if (!AllowScrollOverContent)
            {
                if (_currentScrollPosition < MinScrollPosition)
                    _currentScrollPosition = MinScrollPosition;
                else if (_currentScrollPosition > MaxScrollPosition)
                    _currentScrollPosition = MaxScrollPosition;
            }
            _LastMouseY = y;
            _LastMouseX = x;

            if (_scrollType == KineticControlScrollType.Vertical)
                _ScrollVelocity = (double)(_LastMouseY - _MouseDownPosition.Y) / (t - _MouseStartTime);
            else
                _ScrollVelocity = (double)(_LastMouseX - _MouseDownPosition.X) / (t - _MouseStartTime);

            _MouseEndTime = t;

            RefreshControl();

            base.OnMouseMove(e);

            //return;
        }

        /// <summary>
        /// Not to dispose offset buffer after Dispose() method
        /// </summary>
        /// <remarks>
        /// It's very dangerous to retain offset buffer after Dispose(). May be leak!
        /// </remarks>
        private bool _retainOffsetBufferOnDispose = false;

        public static Graphics OffScreenGraphics
        {
            get
            {
                if (_offScreenGraphics == null)
                    TryReAllocateOffsceenBuffer();

                return _offScreenGraphics;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            ContentRectangle = ClientRectangle;
            TryReAllocateOffsceenBuffer();
        }

        private static void TryReAllocateOffsceenBuffer()
        {
            OffscreenBuffer.TryReAllocateOffsceenBuffer(ref _offScreenBmp, ref _offScreenGraphics, Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
        }

        public static void ReleaseOffscreenBuffer()
        {
            OffscreenBuffer.ReleaseOffscreenBuffer(ref _offScreenBmp, ref _offScreenGraphics);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (_IsScrolling)
            {
                _ScrollTimer.Enabled = false;
                _IsScrolling = false;
                _ScrollVelocity = 0;
            }

            if (_currentScrollPosition < 0)
            {
                e.Handled = true;
                ScrollTo(0);
                return;
            }

            base.OnKeyDown(e);
        }


        protected override void OnKeyUp(KeyEventArgs e)
        {
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)Keys.Escape)
            {
                e.Handled = true;
                OnBackKeyClicked();
            }
        }

        protected virtual void OnBackKeyClicked()
        { }

        #endregion

        #region Outgoing Event Notifications

        protected virtual void OnSelectionChanged(int index)
        {
        }

        protected virtual void OnItemSelected(int index, Point mousePosition)
        {
        }

        #endregion

        #region Helper Methods

        protected void SelectItemInternal(int index)
        {
            int old = _SelectedIndex;

            _SelectedIndex = index;

            if (old != _SelectedIndex)
            {
                if (_SelectedIndex >= 0)
                {
                    if (IsItemVisible(_SelectedIndex))
                        RefreshControl();
                    else
                        ScrollItemIntoView(_SelectedIndex);
                }

                OnSelectionChanged(index);
            }
        }

        protected int GetPixelToItem(int point)
        {
            if (ItemCount == 0)
                return -1;

            if (_scrollType == KineticControlScrollType.Vertical)
            {
                if (point >= _ActiveListHeight - 1)
                    return -2;
            }
            else
            {
                if (point >= _ActiveListWidth - 1)
                    return -2;
            }

            if (_scrollType == KineticControlScrollType.Vertical)
                point = Math.Min((_ActiveListHeight - 1) - 1, point);
            else
                point = Math.Min((_ActiveListWidth - 1) - 1, point);
            point = Math.Max(0, point);

            int guess = 0;
            if (StartPositions != null && StartPositions.Count > 0)
                for (int i = 0; i < StartPositions.Count; i++)
                {
                    if (point >= StartPositions[i])
                        guess = i;
                }

            int max = ItemCount;
            if (guess > max)
                guess = max;

            //while (point < StartPositions[guess] && guess > 0)
            //{
            //    guess--;
            //}

            //while (point >= StartPositions[guess + 1] && guess < max)
            //{
            //    guess++;
            //}

            return guess;
        }

        protected abstract void CalculateItemsSize();

        private void OnScrollTick(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                _ScrollTimer.Enabled = false;
                _IsScrolling = false;
                _ScrollVelocity = 0;

                return;
            }

            int t = Environment.TickCount;

            const int minScrolled = 0;
            int maxScrolled;
            if (_scrollType == KineticControlScrollType.Vertical)
                maxScrolled = _ActiveListHeight <= ContentRectangle.Height ? 0 : _ActiveListHeight - ContentRectangle.Height;
            else
                maxScrolled = _ActiveListWidth <= ContentRectangle.Width ? 0 : _ActiveListWidth - ContentRectangle.Width;

            // Time
            int dt = t - _MouseEndTime;

            // _ScrollVelocity
            if (_currentScrollPosition <= minScrolled)
            {
                if (!AllowScrollOverContent)
                {
                    _currentScrollPosition = minScrolled;
                    _ScrollVelocity= 0;
                }
                else
                {
                    _ScrollVelocity = (double)(_currentScrollPosition - minScrolled) / 2 / dt;
                }
            }
            else if (_currentScrollPosition >= maxScrolled)
            {
                if (!AllowScrollOverContent)
                {
                    _currentScrollPosition = maxScrolled;
                    _ScrollVelocity= 0;
                }
                else
                {
                    _ScrollVelocity = (double)(_currentScrollPosition - maxScrolled) / 2 / dt;
                }
            }
            else
            {
                double dv = _ScrollVelocity * FRICTION_COEFF * dt;
                if (Math.Abs(dv) > Math.Abs(_ScrollVelocity))
                    _ScrollVelocity = 0;
                else
                    _ScrollVelocity = _ScrollVelocity - dv;
            }

            // Displacement
            double s = _ScrollVelocity * dt;
            if (s < 0 && s > -1 && _currentScrollPosition < minScrolled)
                s = -1;
            else if (s > 0 && s < 1 && _currentScrollPosition > maxScrolled)
                s = 1;

            // We're done scrolling
            if ((int)s == 0)
            {
                _ScrollTimer.Enabled = false;
                _IsScrolling = false;
                _ScrollVelocity = 0;
                _IsScrollingOnButtonDown = false;
            }

            _currentScrollPosition = _currentScrollPosition - (int)s;
            _MouseEndTime = t;

            Win32.InvalidateRect(Handle, ref ContentRectangle, false);
        }

        private void OnScrollToTick(object sender, EventArgs e)
        {
            double scrollTime = (Environment.TickCount - _ScrollStartTime);

            if (scrollTime < _ScrollDuration)
            {
                _IsScrolling = true;

                double dur = scrollTime / _ScrollDuration;
                dur -= 1;
                double multiplier = Math.Pow(dur, 3);
                multiplier += 1;
                double amount = _ScrollChange * multiplier;

                _currentScrollPosition = _ScrollStartPosition + (int)amount;
            }
            else
            {
                _IsScrolling = false;
                _IsScrollingOnButtonDown = false;
                _ScrollToTimer.Enabled = false;
                _currentScrollPosition = _ScrollChange + _ScrollStartPosition;
                if (_onScrollToEnding != null)
                    _onScrollToEnding(this, EventArgs.Empty);
            }

            Win32.InvalidateRect(Handle, ref ContentRectangle, false);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            StopTimers();
            base.OnHandleDestroyed(e);
        }

        private void StopTimers()
        {
            StopScrollTimer();
            StopScrollToTimer();
        }

        private void StopScrollToTimer()
        {
            if (_ScrollToTimer != null)
                _ScrollToTimer.Enabled = false;
        }

        private void StopScrollTimer()
        {
            if (_ScrollTimer != null)
                _ScrollTimer.Enabled = false;
            _IsScrolling = false;
            _ScrollVelocity = 0;
        }

        protected void RefreshControl()
        {
            Win32.InvalidateRect(Handle, ref ContentRectangle, false);
            Win32.UpdateWindow(Handle);
        }

        protected bool IsItemVisible(int index)
        {
            Rectangle r = GetItemRectangle(index);

            if (r == Rectangle.Empty)
            {
                return false;
            }

            if (_scrollType == KineticControlScrollType.Vertical)
            {
                return r.Top >= _currentScrollPosition && r.Top <= _currentScrollPosition + ContentRectangle.Height || r.Bottom >= _currentScrollPosition && r.Bottom <= _currentScrollPosition + ContentRectangle.Height;
            }
            else
            {
                return r.Left >= _currentScrollPosition && r.Left <= _currentScrollPosition + ContentRectangle.Width || r.Right >= _currentScrollPosition && r.Right <= _currentScrollPosition + ContentRectangle.Width;
            }
        }

        Rectangle GetItemRectangle(int itemIndex)
        {
            if (itemIndex < 0 || itemIndex >= ItemCount)
            {
                return Rectangle.Empty;
            }

            if (_scrollType == KineticControlScrollType.Vertical)
            {
                try
                {
                    return new Rectangle(ContentRectangle.Left, StartPositions[itemIndex], ContentRectangle.Width, StartPositions[itemIndex + 1] - StartPositions[itemIndex]);
                }
                catch
                {
                    return Rectangle.Empty;
                }
            }
            else
            {
                try
                {
                    return new Rectangle(StartPositions[itemIndex], ContentRectangle.Top, StartPositions[itemIndex + 1] - StartPositions[itemIndex], ContentRectangle.Height);
                }
                catch
                {
                    return Rectangle.Empty;
                }
            }
        }


        protected virtual void SignalListEnd() { }

        public static char GetAlternativeKey(char keyChar)
        {
            switch (keyChar)
            {
                case '1':
                    return 'w';

                case '2':
                    return 'e';

                case '3':
                    return 'r';

                case '4':
                    return 's';

                case '5':
                    return 'd';

                case '6':
                    return 'f';

                case '7':
                    return 'x';

                case '8':
                    return 'c';

                case '9':
                    return 'v';

                default:
                    return ' ';
            }
        }

        #endregion
    }
}