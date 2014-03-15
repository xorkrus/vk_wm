/*	 This file was created by GALS Soft	company
 *	 http://www.galssoft.com
 *	 
 *	 Project name is Odnoklassniki Mobile
 *	 Platform is .Net Compact Framework 2.0 for Windows Mobile 5.0+ 
 * 
 *	 Copyright (c) 2009-2010 GALS Soft
 */

using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace Galssoft.VKontakteWM.Components.UI
{
    partial class UIViewBase
    {
        protected List<UIElementBase> _mouseDownObjects = new List<UIElementBase>();
        protected List<UIElementBase> _mouseFocusObjects = new List<UIElementBase>();


        protected override void OnMouseDown(MouseEventArgs e)
        {
            foreach (UIElementBase element in Canvas.Children)
            {
                if (element.HitTest(new Point(e.X, e.Y)))
                {
                    _mouseDownObjects.Add(element);
                    element.Focus(true);
                    element.OnMouseDown(e);
                }
                else if (_mouseFocusObjects.Contains(element))
                {
                    element.Focus(false);
                }
            }

            _mouseFocusObjects.Clear();
            _mouseFocusObjects.AddRange(_mouseDownObjects);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            foreach (UIElementBase element in Canvas.Children)
            {
                if (element.HitTest(new Point(e.X, e.Y)))
                {
                    element.OnMouseMove(e);
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            while (_mouseDownObjects.Count > 0)
            {
                UIElementBase element = _mouseDownObjects[0];
                if (element.HitTest(new Point(e.X, e.Y)))
                {
                    element.OnMouseUp(e);
                }
                else
                    element.Focus(false);

                if (_mouseDownObjects.Contains(element))
                    _mouseDownObjects.Remove(element);
            }

            //foreach (UIElementBase element in _mouseDownObjects)
            //{
            //    if (element.HitTest(new Point(e.X, e.Y)))
            //    {
            //        element.OnMouseUp(e);
            //    }
            //    else
            //        element.Focus(false);
            //}

            //_mouseDownObjects.Clear();
        }

    }
}