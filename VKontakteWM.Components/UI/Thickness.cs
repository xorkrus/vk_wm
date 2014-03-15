/*	 This file was created by GALS Soft	company
 *	 http://www.galssoft.com
 *	 
 *	 Project name is Odnoklassniki Mobile
 *	 Platform is .Net Compact Framework 2.0 for Windows Mobile 5.0+ 
 * 
 *	 Copyright (c) 2009-2010 GALS Soft
 */

using Galssoft.VKontakteWM.Components.Common.Configuration;

namespace Galssoft.VKontakteWM.Components.UI
{
    public struct Thickness
    {
        public Thickness(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Bottom = bottom;
            Right = right;
        }

        public static Thickness From(float leftInches, float topInches, float rightInches, float bottomInches)
        {
            Thickness ret = new Thickness();
            ret.Left = UISettings.CalcPix((int)leftInches);
            ret.Right = UISettings.CalcPix((int)rightInches);
            ret.Top = UISettings.CalcPix((int)topInches);
            ret.Bottom = UISettings.CalcPix((int)bottomInches);
            return ret;
        }

        public int Left; 
        public int Top;
        public int Right;
        public int Bottom;

        public static Thickness operator +(Thickness one, Thickness two)
        {
            return new Thickness(one.Left + two.Left, one.Top + two.Top, one.Right + two.Right, one.Bottom + two.Bottom);
        }

        public static readonly Thickness Empty = new Thickness();
    }
}