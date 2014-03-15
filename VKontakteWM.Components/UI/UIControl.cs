/*	 This file was created by GALS Soft	company
 *	 http://www.galssoft.com
 *	 
 *	 Project name is Odnoklassniki Mobile
 *	 Platform is .Net Compact Framework 2.0 for Windows Mobile 5.0+ 
 * 
 *	 Copyright (c) 2009-2010 GALS Soft
 */

namespace Galssoft.VKontakteWM.Components.UI
{
    public class UIControl : UIElementBase
    {
        protected virtual void ResumeLayout()
        {
		
        }

        protected virtual void Remeasure()
        {

        }

        protected virtual void SuspendLayout()
        {

        }

        public HorizontalAlignment HorizontalAlignment
        {
            get { return _horizontalAlignment; }
            set
            {
                bool fireEvent = _horizontalAlignment != value;

                _horizontalAlignment = value;

                if (fireEvent)
                    HorizontalAlignmentChanged();
            }
        }
        private HorizontalAlignment _horizontalAlignment = HorizontalAlignment.Left;

        public VerticalAlignment VerticalAlignment
        {
            get { return _verticalAlignment; }
            set
            {
                bool fireEvent = _verticalAlignment != value;

                _verticalAlignment = value;

                if (fireEvent)
                    VerticalAlignmentChanged();
            }
        }
        private VerticalAlignment _verticalAlignment = VerticalAlignment.Center;

        protected virtual void HorizontalAlignmentChanged()
        {

        }

        protected virtual void VerticalAlignmentChanged()
        {

        }

        private Thickness _margin;
        public Thickness Margin
        {
            get { return _margin; }
            set
            {
                _margin = value;
                //Remeasure();
            }
        }


    }
}