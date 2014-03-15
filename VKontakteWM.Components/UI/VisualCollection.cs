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

namespace Galssoft.VKontakteWM.Components.UI
{
    public sealed class VisualCollection : List<UIElementBase>
    {
        private Dictionary<string, int> nameList;
        private Canvas canvas;

        private VisualCollection()
        {
            this.nameList = new Dictionary<string, int>();
        }

        public VisualCollection(Canvas canvas) : this()
        {
            this.canvas = canvas;
        }

        public T Add<T>(Func<T> build)
        {
            T element = build();
            this.Add(element as UIElementBase);
            return element;
        }

        public new void Add(UIElementBase element)
        {
            if (element.Name != "")
            {
                nameList.Add(element.Name, this.Count);
            }
            element.Parent = canvas;
            element.Invalidate += new EventHandler(element_Invalidate);
            base.Add(element);

            if (element is IInitializeAfterConstructor)
                ((IInitializeAfterConstructor)element).Initialize();
        }

        public new void Remove(UIElementBase element)
        {
            if (element.Name != "")
            {
                nameList.Remove(element.Name);
            }
            element.Parent = null;
            element.Invalidate -= new EventHandler(element_Invalidate);
            base.Remove(element);
        }

        void element_Invalidate(object sender, EventArgs e)
        {
            // Pass invalidation event to the canvas  
            if (canvas != null)
                canvas.InvalidateChild(sender as UIElementBase);
        }

        public UIElementBase this[string name]
        {
            get
            {
                return this[nameList[name]];
            }
        }
    }
}