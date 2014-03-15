/*	 This file was created by GALS Soft	company
 *	 http://www.galssoft.com
 *	 
 *	 Project name is Odnoklassniki Mobile
 *	 Platform is .Net Compact Framework 2.0 for Windows Mobile 5.0+ 
 * 
 *	 Copyright (c) 2009-2010 GALS Soft
 */

using System.Drawing;
using Galssoft.VKontakteWM.Components.GDI;

namespace Galssoft.VKontakteWM.Components.UI.Transitions
{
    internal class ViewTransitionBasic : IViewTransition
    {
        public virtual void DrawScreenOn(Gdi mem, Rectangle rect) {}

        public virtual void Execute()
        {
            if (ViewTransitionManager.FromView != ViewTransitionManager.ToView)
            {
                OnTransitionStart();
                OnTransitionEnd();
            }
            else
            {
                OnTransitionEnd();
            }
        }

        public virtual bool IsTransitionAvailable()
        {
            return true;
        }

        protected virtual void OnTransitionStart()
        {
            ViewTransitionManager.OnTransitionStart();
        }

        protected virtual void OnTransitionEnd()
        {
            ViewTransitionManager.OnTransitionEnd();
        }

    }
}