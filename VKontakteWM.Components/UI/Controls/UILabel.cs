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
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.GDI;
using System;
using System.Collections.Generic;

namespace Galssoft.VKontakteWM.Components.UI.Controls
{
    public class UILabel : UIControl
    {
        private struct LineBreak
        {
            string text;
            int myIndex;

            public int Index
            {
                get { return myIndex; }
                set { myIndex = value; }
            }

            public string Text
            {
                get { return text; }
                set { text = value; }
            }
            SizeF mySize;

            public SizeF Size
            {
                get { return mySize; }
                set { mySize = value; }
            }

            public float Width
            {
                get
                {
                    return mySize.Width;
                }
                set
                {
                    mySize.Width = value;
                }
            }
            public float Height
            {
                get
                {
                    return mySize.Height;
                }
                set
                {
                    mySize.Height = value;
                }
            }
        }

        List<LineBreak> _lineBreaks = new List<LineBreak>();
        float _totalHeight = 0;
        //SolidBrush myForeBrush = new SolidBrush(Color.Black);
        //SolidBrush myBackBrush = new SolidBrush(Color.Transparent);

        public UILabel()
        {
        }

        public UILabel(string text)
            : this()
        {
            Text = text;
        }

        public UILabel(string text, FontGdi font)
            : this(text)
        {
            Font = font;
        }

        public UILabel(string text, FontGdi font, Thickness margin)
            : this(text, font)
        {
            Margin = margin;
        }

        public UILabel(string text, FontGdi font, Thickness margin, Color foreColor)
            : this(text, font, margin)
        {
            ForeColor = foreColor;
        }

        //#region Defaults Properties

        //public Color DefaultFontColor
        //{
        //    get { return Color.Black; }
        //}
        //#endregion

        #region Public properties

        ///// <summary>
        ///// Title for the label
        ///// </summary>
        //public string Text
        //{
        //    get
        //    {
        //        return _text;
        //    }
        //    set
        //    {
        //        _text = value;
        //        _multiLinesText = _text.Split('\n');
        //    }
        //}
        //private string _text = null;
        //private string[] _multiLinesText;

        /// <summary>
        /// Title for the label
        /// </summary>
        public string Text
        {
            get { return _text; }
            set
            {
                if (value == null)
                    value = string.Empty;

                bool fireEvent = _text != value;

                _text = value;

                if (fireEvent)
                    TextChanged();
            }
        }
        private string _text = String.Empty;

        protected void AutoEllipsisChanged()
        {
            OnInvalidate();
        }

        protected void MultilineChanged()
        {
            OnInvalidate();
        }

        protected void HorizontalTextAlignmentChanged()
        {
            OnInvalidate();
        }

        protected void VerticalTextAlignmentChanged()
        {
            OnInvalidate();
        }

        protected void TextChanged()
        {
            MeasureUnpadded(Size, false);
            OnInvalidate();
        }

        protected virtual void FontChanged()
        {
            MeasureUnpadded(Size, false);
            OnInvalidate();
        }


        /// <summary>
        /// Title for the button
        /// </summary>
        public Win32.TextAlign TextAlign
        {
            get { return _textAlign; }
            set { _textAlign = value; }
        }
        private Win32.TextAlign _textAlign = Win32.TextAlign.TA_CENTER;

        public HorizontalAlignment HorizontalTextAlignment
        {
            get { return _horizontalTextAlignment; }
            set
            {
                bool fireEvent = _horizontalTextAlignment != value;

                _horizontalTextAlignment = value;

                if (fireEvent)
                    HorizontalTextAlignmentChanged();
            }
        }
        private HorizontalAlignment _horizontalTextAlignment = HorizontalAlignment.Left;

        public VerticalAlignment VerticalTextAlignment
        {
            get { return _verticalTextAlignment; }
            set
            {
                bool fireEvent = _verticalTextAlignment != value;

                _verticalTextAlignment = value;

                if (fireEvent)
                    VerticalTextAlignmentChanged();
            }
        }
        private VerticalAlignment _verticalTextAlignment = VerticalAlignment.Center;


        public FontGdi Font
        {
            get
            {
                return _fontGdi;
            }
            set
            {
                if (_fontGdi == FontGdi.Empty)
                    _fontGdi = UISettings.Global.TextFontGdi;
                else
                    _fontGdi = value;
                _font = System.Drawing.Font.FromHfont((IntPtr)_fontGdi);
            }
        }
        private FontGdi _fontGdi = UISettings.Global.TextFontGdi;

        private Font FontFromGdi
        {
            get
            {
                if (_font == null)
                    _font = System.Drawing.Font.FromHfont((IntPtr)_fontGdi);
                return _font;
            }
        }
        private Font _font;

        public bool AutoEllipsis
        {
            get { return _autoEllipsis; }
            set
            {
                bool fireEvent = _autoEllipsis != value;

                _autoEllipsis = value;

                if (fireEvent)
                    AutoEllipsisChanged();
            }
        }
        private bool _autoEllipsis = true;


        /// <summary>
        /// WordWrap for the label text
        /// </summary>
        public bool Multiline
        {
            get { return _multiline; }
            set
            {
                bool fireEvent = _multiline != value;

                _multiline = value;

                if (fireEvent)
                    MultilineChanged();
            }
        }
        private bool _multiline = false;

        #endregion

        #region Drawing

        //protected override void OnRender(Gdi gMem, Rectangle clipRect)
        //{
        //    if (!string.IsNullOrEmpty(Text))
        //    {
        //        if (!WordWrap)
        //        {
        //            gMem.Font = Font;
        //            gMem.TextColor = FontColor;
        //            Size textSize = gMem.GetTextExtent(Text);
        //            switch (_textAlign)
        //            {
        //                case Win32.TextAlign.TA_RIGHT:
        //                    gMem.ExtTextOut(clipRect.Left + clipRect.Width - textSize.Width,
        //                                    clipRect.Top + 0, Text);
        //                    break;
        //                case Win32.TextAlign.TA_LEFT:
        //                    gMem.ExtTextOut(clipRect.Left,
        //                                    clipRect.Top + 0, Text);
        //                    break;
        //                case Win32.TextAlign.UNKNOWN:
        //                case Win32.TextAlign.TA_CENTER:
        //                    gMem.ExtTextOut(clipRect.Left + (clipRect.Width - textSize.Width) / 2,
        //                                    clipRect.Top + 0, Text);
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            gMem.Font = Font;
        //            gMem.TextColor = FontColor;
        //            int topoffset = 0;
        //            foreach (string s in _multiLinesText)
        //            {
        //                Size textSize = gMem.GetTextExtent(s);
        //                switch (_textAlign)
        //                {
        //                    case Win32.TextAlign.TA_RIGHT:
        //                        gMem.ExtTextOut(clipRect.Left + clipRect.Width - textSize.Width,
        //                                        clipRect.Top + topoffset, s);
        //                        break;
        //                    case Win32.TextAlign.TA_LEFT:
        //                        gMem.ExtTextOut(clipRect.Left,
        //                                        clipRect.Top + topoffset, s);
        //                        break;
        //                    case Win32.TextAlign.UNKNOWN:
        //                    case Win32.TextAlign.TA_CENTER:
        //                        gMem.ExtTextOut(clipRect.Left + (clipRect.Width - textSize.Width) / 2,
        //                                        clipRect.Top + topoffset, s);
        //                        break;
        //                }
        //                topoffset += textSize.Height;
        //            }
        //        }
        //    }
        //}

        static int FindWhitespace(string text, int startIndex)
        {
            int space = text.IndexOf(' ', startIndex);
            int linebreak = text.IndexOf('\n', startIndex);
            if (space == -1)
                return linebreak;
            else if (linebreak == -1)
                return space;
            else
                return Math.Min(space, linebreak);
        }

        LineBreak FitString(string text, int startIndex, int width, Graphics gMeatsure)
        {
            LineBreak lineBreak = new LineBreak();
            int currentIndex = FindWhitespace(text, startIndex);
            if (currentIndex == -1)
                currentIndex = text.Length;
            int lastIndex = startIndex;
            SizeF dims = new SizeF(0, 0);

            lineBreak.Size = new SizeF(0, 0);
            while (currentIndex != -1 && (dims = gMeatsure.MeasureString(text.Substring(startIndex, currentIndex - startIndex), FontFromGdi)).Width <= width)
            {
                // record the width/height while succesfully fit
                lineBreak.Width = dims.Width;
                lineBreak.Height = dims.Height;

                // done
                if (currentIndex == text.Length)
                {
                    lastIndex = currentIndex;
                    currentIndex = -1;
                }
                else if (text[currentIndex] == '\n')
                {
                    lastIndex = currentIndex + 1;
                    currentIndex = -1;
                }
                else
                {
                    // get next word
                    lastIndex = currentIndex + 1;
                    currentIndex = FindWhitespace(text, lastIndex);
                    // end of string
                    if (currentIndex == -1)
                        currentIndex = text.Length;
                }
            }

            if (lastIndex == startIndex)
            {
                // the string was either too long or we are at the end of the string
                if (currentIndex == -1)
                {
                    throw new Exception("Somehow executing unreachable code while drawing text.");
                }

                currentIndex = lastIndex + 1;
                while ((dims = gMeatsure.MeasureString(text.Substring(startIndex, currentIndex - startIndex), FontFromGdi)).Width <= width)
                {
                    lineBreak.Size = new SizeF(dims.Width, dims.Height);
                    currentIndex++;
                }
                lineBreak.Size = new SizeF(dims.Width, dims.Height);
                lineBreak.Width = Math.Min(lineBreak.Width, width);
                lineBreak.Text = text.Substring(startIndex, currentIndex - startIndex);
                lineBreak.Index = currentIndex;
                return lineBreak;
            }
            else
            {
                // return the index we're painting to
                lineBreak.Text = text.Substring(startIndex, lastIndex - startIndex);
                lineBreak.Index = lastIndex;
                return lineBreak;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            MeasureUnpadded(Size, false);
            base.OnResize(e);
        }

        public bool MeasureUnpadded(Size bounds, bool boundsChange)
        {
            try
            {
                _onParamBatchChangeInternal = true;

                using (Bitmap bmpMeasure = new Bitmap(1, 1))
                using (Graphics gMeasure = Graphics.FromImage(bmpMeasure))
                {
                    _lineBreaks.Clear();

                    ////ClientWidth = 0;
                    //Width = 0;
                    ////ClientHeight = 0;
                    //Height = 0;
                    _totalHeight = 0;

                    if (string.IsNullOrEmpty(Text))
                        return false;

                    // limit to one line. trim anything after line feed.
                    string processingText = Text;
                    if (!Multiline)
                    {
                        int index = processingText.IndexOf('\n');
                        if (index != -1)
                            processingText = processingText.Substring(0, index);
                    }
                    LineBreak lineBreak = FitString(processingText, 0, bounds.Width, gMeasure);
                    // simulate nonmultiline by capping the bounds to the height of the first string... hackish but easy!
                    if (!Multiline)
                    {
                        bounds.Height = (int)Math.Ceiling(lineBreak.Height);
                    }

                    while (lineBreak.Index != processingText.Length)
                    {
                        LineBreak nextBreak = FitString(processingText, lineBreak.Index, bounds.Width, gMeasure);
                        // see if this line needs ellipsis
                        if (AutoEllipsis && lineBreak.Height + nextBreak.Height + _totalHeight > bounds.Height)
                        {
                            string text = lineBreak.Text;
                            int ellipsisStart = text.Length - 3;
                            if (ellipsisStart < 0)
                                ellipsisStart = 0;
                            text = text.Substring(0, ellipsisStart) + "...";
                            lineBreak.Width = gMeasure.MeasureString(text, FontFromGdi).Width;
                            lineBreak.Text = text;
                            break;
                        }

                        _lineBreaks.Add(lineBreak);
                        ////ClientWidth = (int)Math.Ceiling(Math.Max(ClientWidth, lineBreak.Width));
                        //Width = (int)Math.Ceiling(Math.Max(Width, lineBreak.Width));
                        _totalHeight += lineBreak.Height;
                        lineBreak = nextBreak;
                    }
                    _lineBreaks.Add(lineBreak);
                    _totalHeight += lineBreak.Height;

                    //if (HorizontalAlignment == HorizontalAlignment.Stretch && bounds.Width != Int32.MaxValue)
                    //    //ClientWidth = bounds.Width;
                    //    Width = bounds.Width;
                    //else
                    //    //ClientWidth = (int)Math.Ceiling(Math.Max(ClientWidth, lineBreak.Width));
                    //    Width = (int)Math.Ceiling(Math.Max(Width, lineBreak.Width));
                    //if (VerticalAlignment == VerticalAlignment.Stretch && bounds.Height != Int32.MaxValue)
                    //    //ClientHeight = bounds.Height;
                    //    Height = bounds.Height;
                    //else
                    //    //ClientHeight = (int)Math.Ceiling(Math.Min(_totalHeight, bounds.Height));
                    //    Height = (int)Math.Ceiling(Math.Min(_totalHeight, bounds.Height));
                    return false;
                }
            }
            finally
            {
                _onParamBatchChangeInternal = false;
            }
        }

        protected override void OnRender(Gdi graphics, Rectangle clipRect)
        {
            //graphics.DrawText(_message,
            //  new Win32.RECT(_textRectangle.X,
            //                 _textRectangle.Y,
            //                 _textRectangle.Width,
            //                 _textRectangle.Height),
            //  Win32.DT.LEFT | Win32.DT.TOP | Win32.DT.WORDBREAK);


            clipRect.Intersect(this.Rectangle);
            if (clipRect.IsEmpty)
                return;

            //base.OnRender(graphics, clipRect);

            //SetupDefaultClip(g);
            float topAdjust = 0;
            switch (VerticalTextAlignment)
            {
                case VerticalAlignment.Top:
                    topAdjust = 0;
                    break;
                case VerticalAlignment.Bottom:
                    //topAdjust = ClientHeight - _totalHeight;
                    topAdjust = Height - _totalHeight;
                    break;
                case VerticalAlignment.Center:
                    //topAdjust = (ClientHeight - _totalHeight) / 2;
                    topAdjust = (Height - _totalHeight) / 2;
                    break;
            }

            if (BackColor != Color.Transparent)
            {
                graphics.FillRect(
                    //new Rectangle(e.Origin.X, e.Origin.Y, ClientWidth, ClientHeigh),
                    clipRect,
                    BackColor);
            }

            foreach (LineBreak lineBreak in _lineBreaks)
            {
                float leftAdjust = 0;
                switch (HorizontalTextAlignment)
                {
                    case HorizontalAlignment.Left:
                        leftAdjust = 0;
                        break;
                    case HorizontalAlignment.Right:
                        //leftAdjust = ClientWidth - lineBreak.Width;
                        leftAdjust = Width - lineBreak.Width;
                        break;
                    case HorizontalAlignment.Center:
                        //leftAdjust = (ClientWidth - lineBreak.Width) / 2.0f;
                        leftAdjust = (Width - lineBreak.Width) / 2.0f;
                        break;
                }

                Rectangle drawRect = new Rectangle(this.Left + Margin.Left + (int)leftAdjust,
                this.Top + Margin.Top + (int)topAdjust, (int)lineBreak.Width, (int)lineBreak.Height);
                if (clipRect.IntersectsWith(drawRect))
                {
                    graphics.Font = Font;
                    graphics.TextColor = ForeColor;
                    //graphics.
                    //TODO: chhange all this code to only one single DrawText
                    graphics.ExtTextOut(drawRect.Left, drawRect.Top, drawRect.Width,
                        lineBreak.Text);
                    //	new Galssoft.WM.Components.SystemHelpers.Win32.RECT(drawRect), Galssoft.WM.Components.SystemHelpers.Win32.DT.LEFT);
                    //Font, myForeBrush, e.Origin.X + leftAdjust, e.Origin.Y + topAdjust);
                }
                topAdjust += lineBreak.Height;
                if (topAdjust > Height)
                    break;
            }
        }

        #endregion
    }
}