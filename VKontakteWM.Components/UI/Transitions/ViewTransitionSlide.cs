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
using Galssoft.VKontakteWM.Components.GDI;

namespace Galssoft.VKontakteWM.Components.UI.Transitions
{
    internal class ViewTransitionSlide : ViewTransitionBasic
    {
        private readonly Timer _SequenceTimer = new Timer();
        private const double _transitionDuration = 1000;
        private double _transitionPct;
        private int _transitionStartTime;

        private Bitmap _ToBitmap;
        private Graphics _ToGraphics;

        private Bitmap _FromBitmap;
        private Graphics _fromGraphics;

        public ViewTransitionSlide()
        {
            _SequenceTimer.Interval = 11;
            _SequenceTimer.Tick += CalculateAnimationSequence;
        }

        public TransitionType SlideDirection { get; set; }

        public override void Execute()
        {
            if (ViewTransitionManager.FromView != ViewTransitionManager.ToView)
                OnTransitionStart();
            else
                OnTransitionEnd();
        }

        public override void DrawScreenOn(Gdi mem, Rectangle rect)
        {
            var fromDC = _fromGraphics.GetHdc();
            var toDC = _ToGraphics.GetHdc();

            var toWidth = (int)(ViewTransitionManager.TransitionCanvas.Width * _transitionPct);

            if (SlideDirection == TransitionType.SlideLeft)
                toWidth = ViewTransitionManager.TransitionCanvas.Width - toWidth;

            int fromWidth = ViewTransitionManager.TransitionCanvas.Width - toWidth;

            if (SlideDirection == TransitionType.SlideLeft)
            {
                mem.BitBlt(0, 0, toWidth, _FromBitmap.Height,
                           fromDC, fromWidth, 0, TernaryRasterOperations.SRCCOPY);

                mem.BitBlt(toWidth, 0, fromWidth, _ToBitmap.Height,
                           toDC, 0, 0, TernaryRasterOperations.SRCCOPY);
            }
            else
            {
                mem.BitBlt(0, 0, toWidth, _FromBitmap.Height,
                           toDC, fromWidth, 0, TernaryRasterOperations.SRCCOPY);

                mem.BitBlt(toWidth, 0, fromWidth, _ToBitmap.Height,
                           fromDC, 0, 0, TernaryRasterOperations.SRCCOPY);
            }

            _fromGraphics.ReleaseHdc(fromDC);
            _ToGraphics.ReleaseHdc(toDC);
        }

        private void CalculateAnimationSequence(object sender, EventArgs e)
        {
            double scrollTime = (Environment.TickCount - _transitionStartTime);

            if (scrollTime < _transitionDuration)
            {
                double dur = scrollTime / _transitionDuration;

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
            base.OnTransitionStart();

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

            if (_FromBitmap == null || _ToBitmap == null)
            {
                base.OnTransitionStart();
                OnTransitionEnd();
                return;
            }

            _fromGraphics = Graphics.FromImage(_FromBitmap);
            _ToGraphics = Graphics.FromImage(_ToBitmap);

            _transitionPct = 0;

            ViewTransitionManager.TransitionCanvas.CurrentTransition = this;

            Application.DoEvents();

            _transitionStartTime = Environment.TickCount;
            _SequenceTimer.Enabled = true;
        }

        protected override void OnTransitionEnd()
        {
            ViewTransitionManager.TransitionCanvas.CurrentTransition = null;

            if (_fromGraphics != null)
            {
                _fromGraphics.Dispose();
                _fromGraphics = null;
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

            base.OnTransitionEnd();
        }

        public override bool IsTransitionAvailable()
        {
            return ViewTransitionManager.TransitionCanvas.AllocateOffscreenBitmap();
        }
    }
}