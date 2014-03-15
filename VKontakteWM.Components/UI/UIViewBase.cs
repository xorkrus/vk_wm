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
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.MVC;

namespace Galssoft.VKontakteWM.Components.UI
{
    /// <summary>
    /// Basic graphic view for drawing our controls 
    /// </summary>
    public partial class UIViewBase : ViewBase
    {
        #region Constructors and destructors

        public UIViewBase()
        {
            _canvas = new Canvas(this);

            SuspendLayout();
            base.AutoScroll = false;
            InitializeComponent();
            ResumeLayout(false);
        }

        #endregion

        #region First Render Event

        private bool _firstRenderInvoked;

        private event EventHandler _firstRenderComplete;
        protected event EventHandler FirstRenderComplete
        {
            add
            {
                _firstRenderComplete += value;
            }
            remove
            {
                _firstRenderComplete -= value;
            }
        }

        private void InvokeFirstRender()
        {
            if (!_firstRenderInvoked)
            {
                _firstRenderInvoked = true;
                if (_firstRenderComplete != null)
                    _firstRenderComplete.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        #region	Private vars

        private Size _oldViewSize = Size.Empty;
        private bool _suspendLayout = false;

        private Bitmap _backgroundImage = null;
        private Canvas _canvas;

        #endregion

        #region Public properties

        /// <summary>
        /// Windowless control collection
        /// </summary>
        public Canvas Canvas
        {
            get { return _canvas; }
        }

        /// <summary>
        /// Имеется ли на View нерисуемые контролы (Forms.Controls)
        /// </summary>
        public virtual bool HaveWinFormsControls
        {
            get
            {
                return false;
            }
        }

        public bool LayoutIsSuspended
        {
            get { return _suspendLayout; }
        }

        /// <summary>
        /// Image for list background
        /// </summary>
        public Bitmap BackgroundImage
        {
            get { return _backgroundImage; }
            set
            {
                Bitmap image = null;
                if (value != null)
                    image = PrepareBackground(value);
                if (_backgroundImage != image)
                {
                    ReleaseBitmap(_backgroundImage);
                    _backgroundImage = image;
                }
            }
        }

        public IImage BackgroundIImage
        {
            get
            {
                return _backgroundIImage;
            }
            set
            {
                if (value != null)
                {
                    _backgroundIImage = value;
                    ImageInfo ii;
                    _backgroundIImage.GetImageInfo(out ii);
                    Bitmap btm = new Bitmap((int)ii.Width, (int)ii.Height);
                    using (Graphics gr = Graphics.FromImage(btm))
                    {
                        Rectangle r = new Rectangle(0, 0, btm.Width, btm.Height);
                        IntPtr hdcSrc = gr.GetHdc();
                        _backgroundIImage.Draw(hdcSrc, ref r, IntPtr.Zero);
                        gr.ReleaseHdc(hdcSrc);
                        BackgroundImage = btm;
                    }
                }
                else
                {
                    _backgroundIImage = null;
                    if (BackgroundImage != null)
                        BackgroundImage.Dispose();
                    BackgroundImage = null;
                }
            }
        }
        private IImage _backgroundIImage = null;

        #endregion

        #region Public Functions

        public Bitmap GetImageFromRectangle(Rectangle rectangle)
        {
            Bitmap destBitmap = new Bitmap(rectangle.Width, rectangle.Height);
            using (Graphics destGraphics = Graphics.FromImage(destBitmap))
            {
                if (BackgroundImage != null)
                {
                    using (Graphics srcGraphics = Graphics.FromImage(BackgroundImage))
                    {
                        IntPtr srcPtr = srcGraphics.GetHdc();
                        IntPtr destPtr = destGraphics.GetHdc();

                        Win32.BitBlt(destPtr, 0, 0, rectangle.Width, rectangle.Height,
                                     srcPtr, rectangle.X, rectangle.Y, TernaryRasterOperations.SRCCOPY);

                        destGraphics.ReleaseHdc(destPtr);
                        srcGraphics.ReleaseHdc(srcPtr);
                    }
                }
                else
                {
                    destGraphics.FillRegion(new SolidBrush(BackColor), new Region(rectangle));
                }
            }
            return destBitmap;
        }

        #endregion

        #region Private Functions


        #endregion

        #region IView components

        public virtual void Activate()
        {
            DebugHelper.WriteTraceEntry(GetType().Name + " Activate called.");
            if (this is IView)
            {
                ViewManager.ActivateView((IView)this);
            }
            DebugHelper.WriteTraceEntry(GetType().Name + " Activate completed.");

            DebugHelper.FlushTraceBuffer();
        }

        public virtual void Deactivate()
        {
            DebugHelper.WriteTraceEntry(GetType().Name + " Deactivate called.");
        }

        public virtual Rectangle GetStaticRectangleForTransition(IView from)
        {
            return Rectangle.Empty;
        }

        #endregion

        #region Actions

        public new void SuspendLayout()
        {
            _suspendLayout = true;

            _canvas.SuspendLayout();
            base.SuspendLayout();
        }

        public new void ResumeLayout(bool performLayout)
        {
            _suspendLayout = false;

            base.ResumeLayout(performLayout);
            _canvas.ResumeLayout();
        }

        protected override void OnResize(EventArgs e)
        {
            OffscreenBuffer.TryReAllocateOffsceenBuffer();
            base.OnResize(e);

            if (BackgroundIImage != null)
                BackgroundIImage = BackgroundIImage;

            if (Canvas.Children.Count == 0)
                return;

            // resize is called whenever the control is moved or resized
            Size size = Size;
            if (size == _oldViewSize)
            {
                // the control was moved
                // this should mean that it was repositioned by the host
                // if the control has transparencies, invalidate it.
                //if (BackColor == Color.Transparent)
                //	Canvas.inva Invalidate();
            }
            else
            {
                _oldViewSize = size;

                // the control was resized for whatever reason
                // we must repaint everything.
                //sInvalidate();
            }
        }

        #endregion

    }
}