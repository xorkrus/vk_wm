/*	 This file was created by GALS Soft	company
 *	 http://www.galssoft.com
 *	 
 *	 Project name is Odnoklassniki Mobile
 *	 Platform is .Net Compact Framework 2.0 for Windows Mobile 5.0+ 
 * 
 *	 Copyright (c) 2009-2010 GALS Soft
 */

using System.Drawing;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.GDI;

namespace Galssoft.VKontakteWM.Components.UI.CompoundControls
{
    public class KineticControlsSetting
    {
        public bool ListGroupTextShadow = true;
        public int BackgroundHeaderPixMargin = 5;
        public int GroupPixHeight = 18;
        public int GroupPixTopMargin = 1;
        public int ItemFontPixHeight = 11;
        public int ListItemPixHeight = 15;
        public int ListItemPixSize = 36;
        public int PrimaryTextIndentPix = 10;
        public int PrimaryTextHeight = 15;
        public int PrimaryIconPixHeight = 50;
        public int PrimaryIconPixWidth = 50;
        public int ScrollPixMargin = 3;
        public int ScrollPixWidth = 3;
        public int SecondaryItemFontPixHeight = 15;
        public int SecondaryIconPixWidth = 32;
        public int SecondaryTextLinesCount = 2;
        public int TextPixMargin = 3;
        public int ThirdIconHeight = 50;
        public int ThirdIconHeightSmall = 25;
        public FontGdi PrimaryTextFontGdi = FontCache.CreateFont("Tahoma", 13, FontStyle.Bold, true);
        public FontGdi PrimaryText2FontGdi = FontCache.CreateFont("Tahoma", 13, FontStyle.Regular, true);
        public FontGdi SecondaryTextFontGdi = FontCache.CreateFont("Tahoma", 13, FontStyle.Regular, true);
        public FontGdi ThirdTextFontGdi = FontCache.CreateFont("Tahoma", 13, FontStyle.Bold, true);
        public FontGdi GroupFont = FontCache.CreateFont("Tahoma", 13, FontStyle.Regular, true);
        public FontGdi SecondaryListFont = FontCache.CreateFont("Tahoma", 11, FontStyle.Bold, true);
        public Color ListItemTextColor = Color.Black;
        public Color ListItemTextColor2 = Color.Red;
        public Color ListItemTextColor3 = Color.Brown;
        public Color ListItemSelectedTextColor = Color.Black;
        public Color ListItemSeparatorColor = Color.FromArgb(163, 163, 163);
        public Color ListGroupBackgroundColor = Color.FromArgb(177, 183, 190);
        public Color ListGroupBackground2Color = Color.FromArgb(200, 204, 209);
        public Color ListItemBackgroundColorOdd = SystemColors.Menu;
        public Color ListItemBackgroundColorEven = SystemColors.Menu;
        public Color ListGroupTextColor = Color.White;

        private BrushGdi _listItemSeparator;
        private BrushGdi _listItemBackgroundOdd;
        private BrushGdi _listItemBackgroundEven;
        private BrushGdi _listGroupBackground;
        private BrushGdi _listGroupBackground2;

        /// <summary>
        /// Перерасчет Размеров и расположения UI контролов на форме в зависимости от расширения
        /// </summary>
        public void RecalcDPIScaling()
        {
            BackgroundHeaderPixMargin = UISettings.CalcPix(BackgroundHeaderPixMargin);
            GroupPixHeight = UISettings.CalcPix(GroupPixHeight);
            GroupPixTopMargin = UISettings.CalcPix(GroupPixTopMargin);
            ItemFontPixHeight = UISettings.CalcPix(ItemFontPixHeight);
            ListItemPixSize = UISettings.CalcPix(ListItemPixSize);
            ListItemPixHeight = UISettings.CalcPix(ListItemPixHeight);
            PrimaryTextIndentPix = UISettings.CalcPix(PrimaryTextIndentPix);
            PrimaryTextHeight = UISettings.CalcPix(PrimaryTextHeight);
            PrimaryIconPixHeight = UISettings.CalcPix(PrimaryIconPixHeight);
            PrimaryIconPixWidth = UISettings.CalcPix(PrimaryIconPixWidth);
            ScrollPixMargin = UISettings.CalcPix(ScrollPixMargin);
            ScrollPixWidth = UISettings.CalcPix(ScrollPixWidth);
            SecondaryItemFontPixHeight = UISettings.CalcPix(SecondaryItemFontPixHeight);
            SecondaryIconPixWidth = UISettings.CalcPix(SecondaryIconPixWidth);
            //SecondaryTextLinesCount = UISettings.CalcPix(SecondaryTextLinesCount);
            TextPixMargin = UISettings.CalcPix(TextPixMargin);
            ThirdIconHeight = UISettings.CalcPix(ThirdIconHeight);
            ThirdIconHeightSmall = UISettings.CalcPix(ThirdIconHeightSmall);
        }

        public BrushGdi ListItemSeparator
        {
            get
            {
                return _listItemSeparator ??
                       (_listItemSeparator =
                        ListItemSeparatorColor != Color.Empty
                            ? Gdi.CreateSolidBrush(ListItemSeparatorColor)
                            : BrushGdi.Empty);
            }
        }

        public BrushGdi ListItemBackgroundOdd
        {
            get
            {
                return _listItemBackgroundOdd ??
                       (_listItemBackgroundOdd =
                        ListItemBackgroundColorOdd != Color.Empty
                            ? Gdi.CreateSolidBrush(ListItemBackgroundColorOdd)
                            : BrushGdi.Empty);
            }
        }

        public BrushGdi ListItemBackgroundEven
        {
            get
            {
                return _listItemBackgroundEven ??
                       (_listItemBackgroundEven =
                        ListItemBackgroundColorEven != Color.Empty
                            ? Gdi.CreateSolidBrush(ListItemBackgroundColorEven)
                            : BrushGdi.Empty);
            }
        }

        public BrushGdi ListGroupBackground
        {
            get
            {
                return _listGroupBackground ??
                       (_listGroupBackground =
                        ListGroupBackgroundColor != Color.Empty
                            ? Gdi.CreateSolidBrush(ListGroupBackgroundColor)
                            : BrushGdi.Empty);
            }
        }

        public BrushGdi ListGroupBackground2
        {
            get
            {
                return _listGroupBackground2 ??
                       (_listGroupBackground2 =
                        ListGroupBackground2Color != Color.Empty
                            ? Gdi.CreateSolidBrush(ListGroupBackground2Color)
                            : BrushGdi.Empty);
            }
        }
    }
}