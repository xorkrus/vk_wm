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
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.GDI;

namespace Galssoft.VKontakteWM.Components.UI
{
    public abstract class UIElementBase: IDisposable
    {
        #region events

        public event MouseEventHandler MouseMove;
        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseUp;
        public event EventHandler Click;
        public event EventHandler Invalidate;
        public event EventHandler Resize;
        public event EventHandler Relocate;

        #endregion

        #region fields

        private byte _opacity;
        private bool _visible;
        private string _name;

        protected int height;
        protected int width;
        protected int left;
        protected int top;

        protected AnchorStyles anchor;

        private Color _backColor = Color.Transparent;
        private Color _foreColor = UISettings.Global.TextColor;

        private UIElementBase _parent;

        #endregion

        #region constructor

        public UIElementBase()
        {
            this._visible = true;
            this._name = "";
            this._opacity = 127;
           
        }

        #endregion

        #region public and protected methods

        /// <summary>
        /// Detects a hit test.
        /// </summary>
        /// <param name="point">a point to detect</param>
        /// <returns>True: if the UIElement was hit.</returns>
        public bool HitTest(Point point)
        {
            if (Focusable)
                return this.Rectangle.Contains(point);
            else
                return false;
        }

        /// <summary>
        /// Renders the UIElement.
        /// </summary>
        /// <param name="graphics">Graphics object.</param>
        public void Render(Gdi graphics, Rectangle rect)
        {
            this.OnRender(graphics, rect);
        }

        /// <summary>
        /// Render method to implement by inheritors
        /// </summary>
        /// <param name="graphics">Graphics object.</param>
        /// <param name="clipRect">Rectangle in which to paint</param>
        protected void OnRender(Graphics graphics, Rectangle clipRect)
        {
            IntPtr hdc = graphics.GetHdc();
            try
            {
                using (Gdi gMem = Gdi.FromHdc(hdc, Rectangle.Empty))  //e.ClipRectangle
                {
                    OnRender(gMem, clipRect);
                }
            }
            finally
            {
                graphics.ReleaseHdc(hdc);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphics">Graphics object.</param>
        /// <param name="clipRect">Rectangle in which to paint</param>
        protected virtual void OnRender(Gdi graphics, Rectangle clipRect)
        {

        }

        public void OnInvalidate(UIElementBase element)
        {
            if (this.Invalidate != null)
            {
                this.Invalidate(element, null);
            }
        }

        public void OnInvalidate()
        {
            if (this.Invalidate != null)
            {
                this.Invalidate(this, null);
            }
        }      

        public void OnClick(EventArgs e)
        {
            if (this.Click != null)
            {
                this.Click(this, e);
            }
        }

        protected virtual void OnKeyDown(System.Windows.Forms.Control sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter && this.Equals(sender))
            {
                e.Handled = true;
                //OnMouseDown();
            }
        }

        protected virtual void OnKeyUp(System.Windows.Forms.Control sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter && this.Equals(sender))
            {
                e.Handled = true;
                //OnMouseUp();
            }
        }

        protected virtual void OnKeyPress(System.Windows.Forms.Control sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (!e.Handled && e.KeyChar == '\r' && this.Equals(sender))
            {
                //InvokeClick();
                e.Handled = true;
            }
        }

        public virtual void OnMouseDown(MouseEventArgs e)
        {
            if (this.MouseDown != null)
            {
                this.MouseDown(this, e);
            }
        }

        public virtual void OnMouseUp(MouseEventArgs e)
        {
            if (this.MouseUp != null)
            {
                this.MouseUp(this, e);
            }
        }

        public virtual void OnMouseMove(MouseEventArgs e)
        {
            if (this.MouseMove != null)
            {
                this.MouseMove(this, e);
            }
        }

        protected virtual void OnResize(EventArgs e)
        {
            if (this.Resize != null)
            {
                this.Resize(this, e);
            }
        }

        protected virtual void OnRelocate(EventArgs e)
        {
            if (this.Relocate != null)
            {
                this.Relocate(this, e);
            }
        }

        #endregion

        #region properties

        public Point Location
        {
            get
            {
                return new Point(Left, Top);
            }
            set
            {
                Left = value.X;
                Top = value.Y;
            }
        }

        public AnchorStyles Anchor
        {
            get
            {
                return anchor;
            }
            set
            {
                anchor = value;
            }
        }

        public int Left
        {
            get
            {
                return left;
            }
            set
            {
                int oldvalue = left;
                left = value;

                if (!_onParamBatchChangeInternal && oldvalue != value)
                    OnRelocate(EventArgs.Empty);
            }
        }

        public int Top
        {
            get
            {
                return top;
            }
            set
            {
                int oldvalue = top;
                top = value;

                if (!_onParamBatchChangeInternal && oldvalue != value)
                    OnRelocate(EventArgs.Empty);
            }
        }

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle(this.left, this.top, this.width, this.height);
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                int oldvalue = height;
                height = value;

                if (!_onParamBatchChangeInternal && oldvalue != value)
                    OnResize(EventArgs.Empty);
            }
        }

        public int Bottom
        {
            get { return Top + Height - 1; }
        }

        public int Right
        {
            get { return Left + Width - 1; }
        }

        public virtual Size Size
        {
            get { return new Size(Width, Height); }
            set 
            {
                _onParamBatchChangeInternal = true;
                bool b = value.Width == Width && value.Height == Height;

                Width = value.Width;
                Height = value.Height;

                _onParamBatchChangeInternal = false;

                if (!b)
                    OnResize(EventArgs.Empty);
            }
        }

        protected bool _onParamBatchChangeInternal = false;

        public UIElementBase Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }

        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                int oldvalue = width;
                width = value;
				
                if (!_onParamBatchChangeInternal && oldvalue != value)
                    OnResize(EventArgs.Empty);
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public byte Opacity
        {
            get
            {
                return _opacity;
            }
            set
            {
                _opacity = value;
            }
        }

        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
                OnInvalidate();
            }
        }

        public Color ForeColor
        {
            get { return _foreColor; }
            set
            {
                bool fireEvent = _foreColor != value;

                _foreColor = value;

                if (fireEvent)
                    ForeColorChanged();
            }
        }

        public Color BackColor
        {
            get { return _backColor; }
            set
            {
                bool fireEvent = _backColor != value;

                _backColor = value;

                if (fireEvent)
                    BackColorChanged();
            }
        }

        protected void BackColorChanged()
        {
            OnInvalidate();
        }

        protected void ForeColorChanged()
        {
            OnInvalidate();
        }

        #endregion

        protected Control FindHost()
        {
            UIElementBase parent = this.Parent;
            while (parent != null)
            {
                if (parent is Canvas)
                {
                    return ((Canvas)parent).Host;
                }
                parent = parent.Parent;
            }
            return null;
        }

        public virtual void Focus(bool setFocus)
        {
        }

        private bool _focusable = false;
        public virtual bool Focusable
        {
            get { return _focusable; }
            protected set { _focusable = value; }
        }


        #region IDisposable Members

        //public abstract void Dispose();
        public virtual void Dispose()
        {
            _parent = null;
        }

        #endregion
    }
}