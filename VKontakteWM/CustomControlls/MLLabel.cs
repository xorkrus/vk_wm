using System;
using System.Drawing;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Forms;
using System.Collections.Generic;
using Galssoft.VKontakteWM.Components.Common.Configuration;

namespace Galssoft.VKontakteWM.CustomControlls
{
    class MLLabel : UIControl
    {
        public MLLabel()
        {
            _lines = new List<string>();
            _textOld = string.Empty;
        }

        #region Defaults Properties

        public FontGdi DefaultButtonFont
        {
            get { return FontCache.CreateFont("Tahoma", 12, FontStyle.Regular); }
        }

        public Color DefaultFontColor
        {
            get { return Color.Black; }
        }

        public Color DefaultFontColorShadow
        {
            get { return Color.White; }
        }

        #endregion

        #region Public properties

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
        private string _text;

        public FontGdi Font
        {
            get
            {
                if (_font != FontGdi.Empty)
                {
                    return _font;
                }

                return DefaultButtonFont;
            }
            set
            {
                _font = value;
            }
        }
        private FontGdi _font = FontGdi.Empty;

        public HorizontalAlignment HorizontalTextAlignment
        {
            get { return _horizontalTextAlignment; }
            set { _horizontalTextAlignment = value; }
        }
        private HorizontalAlignment _horizontalTextAlignment = HorizontalAlignment.Center;

        public VerticalAlignment VerticalTextAlignment
        {
            get { return _verticalTextAlignment; }
            set { _verticalTextAlignment = value; }
        }
        private VerticalAlignment _verticalTextAlignment = VerticalAlignment.Center;

        public bool DropShadow
        {
            get { return _dropShadow; }
            set { _dropShadow = value; }
        }
        private bool _dropShadow = false;

        public Color FontColor
        {
            get
            {
                if (_fontColor != Color.Empty)
                {
                    return _fontColor;
                }

                return DefaultFontColor;
            }
            set
            {
                _fontColor = value;
            }
        }
        private Color _fontColor = Color.Empty;

        public Color FontColorShadow
        {
            get
            {
                if (_fontColorShadow != Color.Empty)
                {
                    return _fontColorShadow;
                }

                return DefaultFontColorShadow;
            }
            set
            {
                _fontColorShadow = value;
            }
        }
        private Color _fontColorShadow = Color.Empty;

        private List<string> _lines;
        private int _totalHeight;
        private string _textToPrint;
        private string _textOld;

        #endregion

        #region Drawing

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            _lines.Clear();
        }

        protected override void OnRender(Gdi graphics, Rectangle clipRect)
        {
            graphics.Font = Font;
            graphics.TextAlign = Galssoft.VKontakteWM.Components.Common.SystemHelpers.Win32.TextAlign.TA_LEFT;

            if (string.IsNullOrEmpty(_text))
            {
                _text = string.Empty;
            }

            _textToPrint = _text.Replace("<br>", "\n");

            if (!_textOld.Equals(_textToPrint))
            {
                _textOld = _textToPrint;

                _lines.Clear();
            }

            int totalHeight = 0;
            int averageHeight = 0;

            #region распил текста на строчки выполняется только если _lines пустой

            if (_lines.Count == 0)
            {
                //пилим по переносам строки...
                string[] lines = _textToPrint.Split('\n');

                foreach (string line in lines)
                {
                    int ipreviouscol = 0;
                    string text = line;
                    string outtext = string.Empty;
                    int icol = text.Length;
                    int maxChars;

                    do
                    {
                        text = line.Substring(ipreviouscol);

                        int[] extents;

                        Size size = graphics.GetTextExtent(text, clipRect.Width, out extents, out maxChars);

                        totalHeight += size.Height;

                        if (totalHeight > clipRect.Height)
                        {
                            totalHeight -= size.Height;

                            break;
                        }

                        bool isSpace = false; //признак пробела

                        if (text.Length > maxChars)
                        {
                            //ищем последний с позиции в начало maxChars пробел
                            int iSpace = text.LastIndexOf(' ', maxChars, maxChars);

                            if (iSpace > -1)
                            {
                                isSpace = true;

                                icol = iSpace;
                            }
                            else
                            {
                                isSpace = false;

                                icol = maxChars;
                            }

                            outtext = text.Substring(0, icol);
                        }
                        else
                        {
                            outtext = text;
                        }

                        //учитываем пробел в конце строки
                        if (isSpace)
                        {
                            ipreviouscol += (icol + 1);
                        }
                        else
                        {
                            ipreviouscol += icol;
                        }

                        _lines.Add(outtext);
                    }
                    while (text.Length > maxChars);
                }

                _totalHeight = totalHeight;
            }

            #endregion

            totalHeight = _totalHeight;

            if (_lines.Count != 0)
            {
                averageHeight = totalHeight / _lines.Count;
            }

            int iter = 0;

            foreach (string line in _lines)
            {
                int leftIndent = 0;
                int topIndent = 0;

                Size size = graphics.GetTextExtent(line);

                switch (HorizontalTextAlignment)
                {
                    case HorizontalAlignment.Left:
                        leftIndent = clipRect.Left;
                        break;

                    case HorizontalAlignment.Center:
                    case HorizontalAlignment.Stretch:
                        leftIndent = clipRect.Left + (clipRect.Width - size.Width) / 2;
                        break;

                    case HorizontalAlignment.Right:
                        leftIndent = clipRect.Left + (clipRect.Width - size.Width);
                        break;
                }

                switch (VerticalTextAlignment)
                {
                    case VerticalAlignment.Top:
                        topIndent = clipRect.Top;
                        break;

                    case VerticalAlignment.Center:
                    case VerticalAlignment.Stretch:
                        topIndent = clipRect.Top + (clipRect.Height - totalHeight) / 2;
                        break;

                    case VerticalAlignment.Bottom:
                        topIndent = clipRect.Top + (clipRect.Height - totalHeight);
                        break;
                }

                //тень
                if (_dropShadow)
                {
                    graphics.TextColor = FontColorShadow;
                    graphics.ExtTextOut(leftIndent + UISettings.CalcPix(1), topIndent + averageHeight * iter + UISettings.CalcPix(1), line);
                }

                graphics.TextColor = FontColor;
                graphics.ExtTextOut(leftIndent, topIndent + averageHeight * iter, line);

                iter++;
            }
        }

        #endregion
    }
}
