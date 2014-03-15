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
using Galssoft.VKontakteWM.Components.GDI;

namespace Galssoft.VKontakteWM.Components.UI.CompoundControls
{
    public class KineticPanel : KineticControl
    {
        #region Vars

        private Canvas _canvas;
        private bool _focusRemoved = false;
        protected List<UIElementBase> _mouseDownObjects = new List<UIElementBase>();

        #endregion

        #region Constructors

        public KineticPanel(KineticControlScrollType scrollType) : base(scrollType)
        {
            _canvas = new Canvas(this);
        }

        public KineticPanel(KineticControlScrollType scrollType, bool retainOffsetBufferOnDispose)
            : base(scrollType, retainOffsetBufferOnDispose)
        {
            _canvas = new Canvas(this);
        }

        #endregion

        #region Fields

        public Canvas Canvas
        {
            get { return _canvas; }
        }

        public int BottomPadding { get; set; }

        public override int ItemCount
        {
            get { return 1; }
        }

        #endregion

        #region Actions

        protected override void OnMouseDown(MouseEventArgs e)
        {
            SuspendLayout();

            base.OnMouseDown(e);

            Point p = new Point(e.X, e.Y + CurrentScrollPosition);
            _focusRemoved = false;

            foreach (UIElementBase element in Canvas.Children)
            {
                if (element.HitTest(p))
                {
                    _mouseDownObjects.Add(element);
                    element.OnMouseDown(e);
                }
            }

            ResumeLayout(false);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            SuspendLayout();

            bool scrolling = _IsScrolling;
            bool dragging = _IsDragging;

            base.OnMouseUp(e);

            if (!_focusRemoved)
            {
                _focusRemoved = true;

                if (!scrolling && !dragging)
                {
                    Point p = new Point(e.X, e.Y + CurrentScrollPosition);
                    foreach (UIElementBase element in _mouseDownObjects)
                    {
                        if (element.HitTest(p))
                        {
                            element.OnMouseUp(e);
                        }
                        else
                            element.Focus(false);
                    }
                }
            }

            _mouseDownObjects.Clear();

            ResumeLayout(false);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            SuspendLayout();

            base.OnMouseMove(e);

            if (!_focusRemoved && (_IsDragging || _IsScrolling))
            {
                _focusRemoved = true;

                foreach (UIElementBase element in Canvas.Children)
                {
                    element.Focus(false);
                }

            }

            ResumeLayout(false);
        }

        protected override void OnSelectionChanged(int index)
        {
            if (index < 0 && !_focusRemoved)
            {
                _focusRemoved = true;

                SuspendLayout();
                foreach (UIElementBase element in Canvas.Children)
                {
                    element.Focus(false);
                }
                ResumeLayout(false);
            }

            base.OnSelectionChanged(index);
        }

        #endregion

        #region Custom Drawing

        public void InvalidateWithShift(Rectangle rect)
        {
            Invalidate(new Rectangle(rect.X, rect.Y - CurrentScrollPosition, rect.Width, rect.Height));
        }

        protected override void DrawScreenOn(Gdi mem, Rectangle rect, int position)
        {
            if (rect.Bottom > ContentRectangle.Top || rect.Top < ContentRectangle.Bottom)
            {
                DrawBackground(mem, rect, position);

                // Pass the graphics to the canvas to render
                if (Canvas != null)
                {
                    Canvas.Render(mem, rect);
                }
            }
        }

        public void CalculateHeight()
        {
            CalculateItemsSize();
        }

        protected override void CalculateItemsSize()
        {
            StartPositions.Clear();

            if (Canvas.Children.Count == 0)
            {
                _ActiveListHeight = 0;
                _ActiveListWidth = 0;
                return;
            }

            StartPositions.Add(0);

            UIElementBase element = Canvas.Children[0];
            int max = element.Bottom;
            for (int i = 1; i < Canvas.Children.Count; i++)
            {
                element = Canvas.Children[i];
                max = Math.Max(max, element.Bottom);
            }

            int v = max + BottomPadding;
            StartPositions.Add(v);

            _ActiveListHeight = v;
        }

        #endregion
    }
}