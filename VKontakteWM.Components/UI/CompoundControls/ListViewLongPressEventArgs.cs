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

namespace Galssoft.VKontakteWM.Components.UI.CompoundControls
{
    public class ListViewLongPressEventArgs : EventArgs
    {
        public ListViewLongPressEventArgs(Point point, int index, object item)
        {
            _clickCoordinates = point;
            _index = index;
            _item = item;
        }

        /// <summary>
        /// Coordoinates of the click
        /// </summary>
        public Point ClickCoordinates
        {
            get { return _clickCoordinates; }
        }

        private Point _clickCoordinates;

        /// <summary>
        /// Index of the selected item in the list
        /// </summary>
        public int Index
        {
            get { return _index; }
        }

        private int _index;

        /// <summary>
        /// Selected item
        /// </summary>
        public object ItemData
        {
            get { return _item; }
        }

        private object _item;
    }
}