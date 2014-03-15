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
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.GDI;

namespace Galssoft.VKontakteWM.Components.UI.Transitions
{
    internal class ViewTransitionFlip : ViewTransitionBasic
    {
        private const double _transitionDuration = 700;
        private readonly Timer _SequenceTimer = new Timer();

        private Bitmap _FromBitmap;
        private Graphics _FromGraphics;
        private Bitmap _ToBitmap;
        private Graphics _ToGraphics;
        private Bitmap _StretchBitmap;
        private Graphics _StretchGraphics;

        private double _transitionPct;
        private int _transitionStartTime;

        private int _CanvasMiddle;
        private List<Point> _FlipInTopPath;
        private List<Point> _FlipInLeftPath;
        private List<Point> _FlipOutTopPath;
        private List<Point> _FlipOutLeftPath;

        public ViewTransitionFlip()
        {
            _SequenceTimer.Interval = 11;
            _SequenceTimer.Tick += CalculateAnimationSequence;
        }

        public TransitionType FlipDirection { get; set; }

        public override void Execute()
        {
            if (ViewTransitionManager.FromView != ViewTransitionManager.ToView)
                OnTransitionStart();
            else
                OnTransitionEnd();
        }

        public override void DrawScreenOn(Gdi mem, Rectangle rect)
        {
            IntPtr fromDC = _FromGraphics.GetHdc();
            IntPtr toDC = _ToGraphics.GetHdc();
            IntPtr stretchDC = _StretchGraphics.GetHdc();
			
            mem.FillRect(rect, Color.Gainsboro);

            double percent = 2 * (_transitionPct > 0.5 ? _transitionPct - 0.5 : _transitionPct);

            if (FlipDirection == TransitionType.FlipLeft)
            {
                if (_transitionPct <= 0.5)
                    FlipOut(mem, rect, fromDC, stretchDC, percent);
                else
                    FlipIn(mem, rect, toDC, stretchDC, percent);
            }
            else
            {
                if (_transitionPct <= 0.5)
                    FlipIn(mem, rect, fromDC, stretchDC, 1 - percent);
                else
                    FlipOut(mem, rect, toDC, stretchDC, 1 - percent);
            }

            _StretchGraphics.ReleaseHdc(stretchDC);
            _FromGraphics.ReleaseHdc(fromDC);
            _ToGraphics.ReleaseHdc(toDC);
        }

        private void FlipOut(Gdi mem, Rectangle canvas, IntPtr dc, IntPtr stretchDC, double transitionPct)
        {
			
            int margin = (int)((double)_CanvasMiddle * transitionPct);

            using (Gdi stretch = Gdi.FromHdc(stretchDC, Rectangle.Empty))
            {
                stretch.FillRect(canvas, Color.Gainsboro);
                stretch.StretchBlt(0, 0, canvas.Width - margin*2, canvas.Height,
                                   dc,
                                   0, 0, canvas.Width, canvas.Height, TernaryRasterOperations.SRCCOPY);
            }

            var currLine = BuildPath(_FlipOutLeftPath[margin].X, _FlipOutLeftPath[margin].Y, _FlipOutTopPath[margin].X,
                                     _FlipOutTopPath[margin].Y);

            for (int i = 0; i < currLine.Count - 1; i++)
            {
                mem.StretchBlt(currLine[i].X, currLine[i].Y, 1, canvas.Height - currLine[i].Y*2,
                               stretchDC,
                               i, 0, 1, canvas.Height,
                               TernaryRasterOperations.SRCCOPY);
            }
        }

        private void FlipIn(Gdi mem, Rectangle canvas, IntPtr dc, IntPtr stretchDC, double transitionPct)
        {

            var margin = (int)((double)_CanvasMiddle * transitionPct);

            using (Gdi stretch = Gdi.FromHdc(stretchDC, Rectangle.Empty))
            {
                stretch.FillRect(canvas, Color.Gainsboro);
                stretch.StretchBlt(0, 0, margin * 2, canvas.Height,
                                   dc,
                                   0, 0, canvas.Width, canvas.Height, TernaryRasterOperations.SRCCOPY);
            }

            List<Point> currLine = BuildPath(_FlipInLeftPath[margin].X, _FlipInLeftPath[margin].Y,
                                             _FlipInTopPath[margin].X, _FlipInTopPath[margin].Y);

            for (int i = 0; i < currLine.Count - 1; i++)
            {
                mem.StretchBlt(currLine[i].X, currLine[i].Y, 1, canvas.Height - currLine[i].Y * 2,
                               stretchDC,
                               currLine.Count - i - 1, 0, 1, canvas.Height,
                               TernaryRasterOperations.SRCCOPY);
            }
        }

        private static List<Point> BuildPath(int x0, int y0, int x1, int y1)
        {
            var points = new List<Point>();
            int dx = x1 - x0;
            int dy = y1 - y0;

            points.Add(new Point(x0, y0));
            if (dx != 0)
            {
                float m = dy/(float) dx;
                float b = y0 - m*x0;
                dx = (x1 > x0) ? 1 : -1;
                while (x0 != x1)
                {
                    x0 += dx;
                    y0 = (int) Math.Round(m*x0 + b);
                    points.Add(new Point(x0, y0));
                }
            }
            return points;
        }

        private void CalculateAnimationSequence(object sender, EventArgs e)
        {
            double scrollTime = (Environment.TickCount - _transitionStartTime);

            if (scrollTime < _transitionDuration)
            {
                double dur = scrollTime/_transitionDuration;

                dur -= 1.0;
                dur = Math.Pow(dur, 3);
                dur += 1.0;

                _transitionPct = dur;
                ViewTransitionManager.TransitionCanvas.RefreshControl();
            }
            else
            {
                _SequenceTimer.Enabled = false;
                OnTransitionEnd();
            }
        }

        protected override void OnTransitionStart()
        {
            _FromBitmap = null;
            try
            {
                _FromBitmap = ViewTransitionManager.FromView.CreateScreenShot();
            }
            catch (OutOfMemoryException)
            {
            }

            _ToBitmap = null;
            try
            {
                _ToBitmap = ViewTransitionManager.ToView.CreateScreenShot();
            }
            catch (OutOfMemoryException)
            {
            }

            if (_FromBitmap == null || _ToBitmap == null || _StretchBitmap == null)
            {
                base.OnTransitionStart();
                OnTransitionEnd();
                return;
            }

            _FromGraphics = Graphics.FromImage(_FromBitmap);
            _ToGraphics = Graphics.FromImage(_ToBitmap);
            _StretchGraphics = Graphics.FromImage(_StretchBitmap);

            _transitionPct = 0;

            _CanvasMiddle = ViewTransitionManager.TransitionCanvas.ClientSize.Width / 2;
            _FlipInTopPath = BuildPath(_CanvasMiddle, 0, 0, 0);
            _FlipInLeftPath = BuildPath(_CanvasMiddle, 60, ViewTransitionManager.TransitionCanvas.ClientSize.Width, 0);
            _FlipOutTopPath = BuildPath(ViewTransitionManager.TransitionCanvas.ClientSize.Width, 0, 0, 0);
            _FlipOutLeftPath = BuildPath(0, 0, _CanvasMiddle, 60);

            ViewTransitionManager.TransitionCanvas.CurrentTransition = this;

            base.OnTransitionStart();

            Application.DoEvents();

            _transitionStartTime = Environment.TickCount;
            _SequenceTimer.Enabled = true;
        }

        protected override void OnTransitionEnd()
        {
            ViewTransitionManager.TransitionCanvas.CurrentTransition = null;

            if (_FromGraphics != null)
            {
                _FromGraphics.Dispose();
                _FromGraphics = null;
            }

            if (_FromBitmap != null)
            {
                _FromBitmap.Dispose();
                _FromBitmap = null;
            }

            if (_ToGraphics != null)
            {
                _ToGraphics.Dispose();
                _ToGraphics = null;
            }

            if (_ToBitmap != null)
            {
                _ToBitmap.Dispose();
                _ToBitmap = null;
            }

            if (_StretchGraphics != null)
            {
                _StretchGraphics.Dispose();
                _StretchGraphics = null;
            }

            if (_StretchBitmap != null)
            {
                _StretchBitmap.Dispose();
                _StretchBitmap = null;
            }

            base.OnTransitionEnd();
        }

        public override bool IsTransitionAvailable()
        {
            try
            {
                _StretchBitmap = new Bitmap(ViewTransitionManager.TransitionCanvas.ClientRectangle.Width, ViewTransitionManager.TransitionCanvas.ClientRectangle.Height);
            }
            catch (OutOfMemoryException e)
            {
                DebugHelper.WriteLogEntry(e.ToString());
                _StretchBitmap = null;
            }

            return _StretchBitmap != null && ViewTransitionManager.TransitionCanvas.AllocateOffscreenBitmap();
        }
    }
}