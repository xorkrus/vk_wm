/*	 This file was created by GALS Soft	company
 *	 http://www.galssoft.com
 *	 
 *	 Project name is Odnoklassniki Mobile
 *	 Platform is .Net Compact Framework 2.0 for Windows Mobile 5.0+ 
 * 
 *	 Copyright (c) 2009-2010 GALS Soft
 */

using System.Drawing;

namespace Galssoft.VKontakteWM.Components.UI.Controls
{
    class UIPanel : UIControl
    {
        public UIPanel()
        {
            InitializeControls();
        }

        private void InitializeControls()
        {

        }

        protected void LayoutControl(UIControl control, Rectangle rect)
        {
            switch (control.HorizontalAlignment)
            {
                case HorizontalAlignment.Stretch:
                case HorizontalAlignment.Left:
                    control.Left = rect.Left;
                    break;
                case HorizontalAlignment.Center:
                    control.Left = (rect.Left + rect.Right - control.Width) / 2;
                    break;
                case HorizontalAlignment.Right:
                    control.Left = rect.Right - control.Width;
                    break;
            }
            switch (control.VerticalAlignment)
            {
                case VerticalAlignment.Stretch:
                case VerticalAlignment.Top:
                    control.Top = rect.Top;
                    break;
                case VerticalAlignment.Center:
                    control.Top = (rect.Top + rect.Bottom - control.Height) / 2;
                    break;
                case VerticalAlignment.Bottom:
                    control.Top = rect.Bottom - control.Height;
                    break;
            }
        }
    }
}