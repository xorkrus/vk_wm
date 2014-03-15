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
    public class Canvas : UIElementBase
    {        
        private VisualCollection _children;
        private Size _hostSize;
        private bool _layouted;

        public Control Host
        {
            get { return _host; }
        }
        private Control _host;
		  
        public Canvas(Control host)
        {
            _children = new VisualCollection(this);
            _host = host;
            _hostSize = _host.Size;
            _host.Resize += Host_Resize;
        }

        public void SuspendLayout()
        {
            _layouted = true;
        }

        public void ResumeLayout()
        {
            _layouted = false;
        }

        public Bitmap GetImageFromRectangle(Rectangle rectangle)
        {
            if (_host != null && _host is UIViewBase)
            {
                return ((UIViewBase)_host).GetImageFromRectangle(rectangle);
            }
            else
            {
                return null;
            }
        }

        private static bool IsAnchored(AnchorStyles anchor, AnchorStyles desiredAnchor)
        {
            return ((anchor & desiredAnchor) == desiredAnchor);
        }

        void Host_Resize(object sender, EventArgs e)
        {
            if (!_layouted)
            {
                Size sizeDiff = new Size(_host.Size.Width - _hostSize.Width, _host.Size.Height - _hostSize.Height);
                for (int i = 0; i < _children.Count; i++)
                {
                    UIElementBase element = _children[i];
                    Rectangle rec = new Rectangle(element.Left, element.Top, element.Width, element.Height);
                    if (IsAnchored(element.Anchor, AnchorStyles.Right))
                    {
                        if (IsAnchored(element.Anchor, AnchorStyles.Left))
                        {
                            rec.Width += sizeDiff.Width;
                        }
                        else
                        {
                            rec.X += sizeDiff.Width;
                        }
                    }
                    if (IsAnchored(element.Anchor, AnchorStyles.Bottom))
                    {
                        if (IsAnchored(element.Anchor, AnchorStyles.Top))
                        {
                            rec.Height += sizeDiff.Height;
                        }
                        else
                        {
                            rec.Y += sizeDiff.Height;
                        }
                    }
                    if (element.Anchor != 0)
                    {
                        if (rec.X != element.Left || rec.Y != element.Top)
                            element.Location = rec.Location;
                        if (rec.Width != element.Width || rec.Height != element.Height)
                            element.Size = rec.Size;
                    }
                }
            }
            _hostSize = _host.Size;
        }

        /// <summary>
        /// Перерасчет Размеров и расположения UI контролов на форме в зависимости от расширения
        /// </summary>
        public void RecalcDPIScaling()
        {
            if (UISettings.ScreenDPI <= 96)
                return;

            for (int i = 0; i < _children.Count; i++)
            {
                UIElementBase element = _children[i];
                Point loc = new Point(UISettings.CalcPix(element.Left), UISettings.CalcPix(element.Top));
                Size size = new Size(UISettings.CalcPix(element.Width), UISettings.CalcPix(element.Height));
                element.Location = loc;
                element.Size = size;
            }
        }

        public T AddElement<T>(Func<T> buildFunc) where T : UIElementBase
        {
            if (buildFunc == null)
                throw new ArgumentNullException("buildFunc");

            T instance = buildFunc();
            this._children.Add(instance as UIElementBase);
            return instance;
        }

        internal void InvalidateChild(UIElementBase element)
        {
            if (this._host != null)
            {
                Host.Invalidate(element.Rectangle);
            }           
        }

        //protected override void OnRender(Gdi graphics, Rectangle rect)
        //{
        //    //base.OnRender(graphics, rect);
			
        //    //// Draw a back color
        //    //if (this.BackgroundColor != null)
        //    //{
        //    //    //using (Brush brush = new SolidBrush(BackgroundColor))
        //    //    //{
        //    //    //     graphics.FillRectangle(brush, new Rectangle(this.Left, this.Top, this.Height, this.Width));
        //    //    //}
        //    //    graphics.FillRect(Rectangle, BackgroundColor);
        //    //}

        //    // Pass the graphics objects to all UI elements
        //    for (int i = 0; i < _children.Count; i++)
        //    {
        //        UIElementBase element = _children[i];
        //        if (element.Visible)
        //        {
        //            //int y = 0;
        //            //if (!(Host is Form))
        //            //{
        //            //    y += Host.Top;
        //            //}

        //            //Rectangle elementRect = new Rectangle(Host.Left + element.Left,
        //            //            y + element.Top,
        //            //            element.Width,
        //            //            element.Height);

        //            if (IsVisibleOnRect(element.Rectangle, rect))
        //            {
        //                //element.Render(graphics, elementRect);
        //                element.Render(graphics, element.Rectangle);
        //            }
        //        }
        //    }
        //}

        protected override void OnRender(Gdi graphics, Rectangle rect)
        {
            //base.OnRender(graphics, rect);
			
            //// Draw a back color
            //if (this.BackgroundColor != null)
            //{
            //    //using (Brush brush = new SolidBrush(BackgroundColor))
            //    //{
            //    //     graphics.FillRectangle(brush, new Rectangle(this.Left, this.Top, this.Height, this.Width));
            //    //}
            //    graphics.FillRect(Rectangle, BackgroundColor);
            //}

            // Pass the graphics objects to all UI elements
            if (!_layouted)
                for (int i = 0; i < _children.Count; i++)
                {
                    UIElementBase element = _children[i];
                    if (element.Visible)
                    {
                        if (element.Rectangle.IntersectsWith(rect))
                        {
                            element.Render(graphics, element.Rectangle);
                        }
                    }
                }
        }

        public void Render(Gdi gMem, Rectangle rClip, int yListOffset)
        {
            // Pass the graphics objects to all UI elements
            for (int i = 0; i < _children.Count; i++)
            {
                UIElementBase element = _children[i];
                if (element.Visible)
                {
                    int y = -yListOffset;
                    if (!(Host is Form))
                    {
                        y += Host.Top;
                    }

                    Rectangle elementRect = new Rectangle(Host.Left + element.Left,
                                                          y + element.Top,
                                                          element.Width,
                                                          element.Height);

                    if (IsVisibleOnRect(elementRect, rClip))
                    {
                        element.Render(gMem, elementRect);
                    }
                }
            }
        }

        protected bool IsVisibleOnRect(Rectangle elementRect, Rectangle paintRect)
        {
            return elementRect.IntersectsWith(paintRect);
        }

        public VisualCollection Children
        {
            get
            {
                return this._children;
            }
            set
            {
                this._children = value;
            }
        }

        #region IDisposable Members

        public override void Dispose()
        {
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Dispose();
            }
            base.Dispose();
        }

        #endregion
    }
}