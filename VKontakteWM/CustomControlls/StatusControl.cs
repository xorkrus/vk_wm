using System;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Forms;
using System.Collections.Generic;

namespace Galssoft.VKontakteWM.CustomControlls
{
    class StatusControl : UIControl
    {
        public StatusControl()
        {
            _label = new UILabel();
            _label.Visible = true;

            Style = ButtonStyle.AlphaChannel;
            base.Focusable = true;
            SetImages();

            _lines = new List<string>();

            //_text = string.Empty;
            //_textToPrint = string.Empty;

            _textOld = string.Empty;
        }

        #region Private vars

        UILabel _label;

        IImage _leftPart;
        IImage _rightPart;
        IImage _centerPart;
        Size _leftPartSize;
        Size _rightPartSize;
        Size _centerPartSize;

        IImage _leftPartPressed;
        IImage _rightPartPressed;
        IImage _centerPartPressed;
        Size _leftPartSizePressed;
        Size _rightPartSizePressed;
        Size _centerPartSizePressed;
        #endregion

        #region Defaults Properties

        public FontGdi DefaultButtonFont
        {
            get { return FontCache.CreateFont("Calibri", 11, FontStyle.Bold, true); }
        }

        public Color DefaultFontColor
        {
            get { return Color.Black; }
        }

        public Color DefaultFontColorUnactive
        {
            get { return Color.Gray; }
        }


        public Color DefaultFontColorInvert
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

        public string TextSubstitute
        {
            get { return _textSubstitute; }
            set { _textSubstitute = value; }
        }
        private string _textSubstitute;

        public FontGdi Font
        {
            get
            {
                if (_font != FontGdi.Empty)
                    return _font;
                else
                    return DefaultButtonFont;
            }
            set { _font = value; }
        }
        private FontGdi _font = FontGdi.Empty;

        public Color FontColor
        {
            get
            {
                if (_fontColor != Color.Empty)
                    return _fontColor;
                else
                    return DefaultFontColor;
            }
            set { _fontColor = value; }
        }
        private Color _fontColor = Color.Empty;

        public Color FontColorUnactive
        {
            get
            {
                if (_fontColorUnactive != Color.Empty)
                    return _fontColorUnactive;
                else
                    return DefaultFontColorUnactive;
            }
            set { _fontColorUnactive = value; }
        }
        private Color _fontColorUnactive = Color.Empty;

        public Color FontColorInvert
        {
            get
            {
                if (_fontColorInvert != Color.Empty)
                    return _fontColorInvert;
                else
                    return DefaultFontColorInvert;
            }
            set { _fontColorInvert = value; }
        }
        private Color _fontColorInvert = Color.Empty;

        public bool Pressed
        {
            get { return _pressed; }
            internal set { _pressed = value; }
        }
        protected bool _pressed = false;

        public bool Selected
        {
            get { return _selected; }
            internal set { _selected = value; }
        }
        protected bool _selected = false;

        /// <summary>
        /// Style to draw the button
        /// </summary>
        public ButtonStyle Style
        {
            get { return _style; }
            set { _style = value; }
        }
        private ButtonStyle _style;

        public IImage TransparentButton
        {
            get { return _transparentButton; }
            set
            {
                _transparentButton = value;

                if (_autosize)
                {
                    ImageInfo img;
                    value.GetImageInfo(out img);
                    this.Size = new Size((int)img.Width, (int)img.Height);
                }
            }
        }
        private IImage _transparentButton = null;

        public IImage TransparentButtonPressed
        {
            get { return _transparentButtonPressed; }
            set { _transparentButtonPressed = value; }
        }
        private IImage _transparentButtonPressed = null;

        public IImage TransparentButtonSelected
        {
            get { return _transparentButtonSelected; }
            set { _transparentButtonSelected = value; }
        }
        private IImage _transparentButtonSelected = null;

        public ImageData Button
        {
            get { return _button; }
            set
            {
                _button = value;

                if (_autosize)
                    this.Size = new Size(value.ImageSize.Width, value.ImageSize.Height);
            }
        }
        private ImageData _button = null;

        public ImageData ButtonPressed
        {
            get { return _buttonPressed; }
            set { _buttonPressed = value; }
        }
        private ImageData _buttonPressed = null;

        public ImageData ButtonSelected
        {
            get { return _buttonSelected; }
            set { _buttonSelected = value; }
        }
        private ImageData _buttonSelected = null;

        public bool Stretch
        {
            get { return _stretch; }
            set
            {
                if (Style == ButtonStyle.AlphaChannel)
                    _stretch = false;
                else
                    _stretch = value;
            }
        }
        private bool _stretch = false;

        public bool AutoSize
        {
            get { return _autosize; }
            set { _autosize = value; }
        }
        private bool _autosize = true;

        private List<string> _lines;
        private int _totalHeight;
        private string _textToPrint;
        private string _textOld;

        #endregion

        #region Actions

        public override void Focus(bool setFocus)
        {
            if (setFocus)
            {
                _selected = false;
                _pressed = true;
            }
            else
            {
                _selected = false;
                _pressed = false;
            }
            OnInvalidate();
        }

        public override void OnMouseDown(MouseEventArgs e)
        {
            _selected = true;
            OnInvalidate();
        }

        public override void OnMouseUp(MouseEventArgs e)
        {
            if (_selected)
            {
                _selected = false;
                _pressed = false;
                OnClick(EventArgs.Empty);

                OnInvalidate();
            }
        }

        #endregion

        #region Drawing

        public void SetImages()
        {
            _leftPart = MasterForm.SkinManager.GetImage("Status1");
            _rightPart = MasterForm.SkinManager.GetImage("Status3");
            _centerPart = MasterForm.SkinManager.GetImage("Status2");
            ImageInfo ii;
            if (_leftPart != null)
            {
                _leftPart.GetImageInfo(out ii);
                _leftPartSize = new Size((int)ii.Width, (int)ii.Height);
            }
            if (_rightPart != null)
            {
                _rightPart.GetImageInfo(out ii);
                _rightPartSize = new Size((int)ii.Width, (int)ii.Height);
            }
            if (_centerPart != null)
            {
                _centerPart.GetImageInfo(out ii);
                _centerPartSize = new Size((int)ii.Width, (int)ii.Height);
            }

            _leftPartPressed = MasterForm.SkinManager.GetImage("Status1Pressed");
            _rightPartPressed = MasterForm.SkinManager.GetImage("Status3Pressed");
            _centerPartPressed = MasterForm.SkinManager.GetImage("Status2Pressed");
            if (_leftPartPressed != null)
            {
                _leftPartPressed.GetImageInfo(out ii);
                _leftPartSizePressed = new Size((int)ii.Width, (int)ii.Height);
            }
            if (_rightPartPressed != null)
            {
                _rightPartPressed.GetImageInfo(out ii);
                _rightPartSizePressed = new Size((int)ii.Width, (int)ii.Height);
            }
            if (_centerPartPressed != null)
            {
                _centerPartPressed.GetImageInfo(out ii);
                _centerPartSizePressed = new Size((int)ii.Width, (int)ii.Height);
            }
        }

        protected override void OnRelocate(EventArgs e)
        {
            base.OnRelocate(e);
            if (_leftPart != null && _rightPart != null && _centerPart != null)
            {
                Point location = new Point(Location.X + _leftPartSize.Width,
                                            Location.Y + (_leftPartSize.Height - _leftPartSize.Height) / 2);

                _label.Location = location;
            }
            else
            {
                _label.Location = Location;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (_leftPart != null && _rightPart != null && _centerPart != null)
            {
                Size size = new Size(Size.Width - _leftPartSize.Width - _rightPartSize.Width, Size.Height);
                Point location = new Point(Location.X + _leftPartSize.Width, Location.Y + (_leftPartSize.Height - _leftPartSize.Height) / 2);

                _label.Location = location;
                _label.Size = size;
            }
            else
            {
                _label.Location = Location;
                _label.Size = Size;
            }

            _lines.Clear();
        }

        protected override void OnRender(Gdi graphics, Rectangle clipRect)
        {
            if (_leftPart != null && _rightPart != null && _centerPart != null)
            {
                Rectangle rect = new Rectangle(clipRect.Left + _leftPartSize.Width,
                                                clipRect.Top,
                                                clipRect.Width - _rightPartSize.Width - _leftPartSize.Width,
                                                clipRect.Height);

                if (!_pressed)
                {
                    graphics.DrawImageAlphaChannel(_leftPart, clipRect.Left, clipRect.Top);
                    graphics.DrawImageAlphaChannel(_rightPart, rect.Right, clipRect.Top);
                    graphics.DrawImageAlphaChannel(_centerPart, rect);
                }
                else
                {
                    graphics.DrawImageAlphaChannel(_leftPartPressed, clipRect.Left, clipRect.Top);
                    graphics.DrawImageAlphaChannel(_rightPartPressed, rect.Right, clipRect.Top);
                    graphics.DrawImageAlphaChannel(_centerPartPressed, rect);
                }

                #region отрисовка текста комментария

                graphics.Font = Font;
                graphics.TextAlign = Galssoft.VKontakteWM.Components.Common.SystemHelpers.Win32.TextAlign.TA_LEFT;

                if (string.IsNullOrEmpty(_text))
                {
                    _text = string.Empty;

                    if (string.IsNullOrEmpty(_textSubstitute))
                    {
                        _textToPrint = string.Empty;
                    }
                    else
                    {
                        _textToPrint = _textSubstitute;
                    }

                    graphics.TextColor = FontColorUnactive;
                }
                else
                {
                    _textToPrint = _text;

                    graphics.TextColor = FontColor;
                }

                if (_pressed)
                {
                    graphics.TextColor = FontColorInvert;
                }

                if (!_textOld.Equals(_textToPrint))
                {
                    _textOld = _textToPrint;

                    _lines.Clear();
                }

                int topIndent = 0;

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

                            Size size = graphics.GetTextExtent(text, clipRect.Width - _leftPartSize.Width - _rightPartSize.Width, out extents, out maxChars);

                            totalHeight += size.Height * 11 / 10;

                            bool isSpace = false; //признак пробела
                            bool isOver = false; //признак необходимости новой строки

                            if (text.Length > maxChars)
                            {
                                isOver = true;

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
                                isOver = false;

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

                            //добавляем точечки
                            if ((_lines.Count + 1) < 2)
                            {
                                _lines.Add(outtext);
                            }
                            else
                            {
                                if (isOver)
                                {
                                    outtext = outtext.Remove(outtext.Length - 3, 3);

                                    if (outtext.EndsWith(" "))
                                    {
                                        outtext = outtext.Remove(outtext.Length - 1, 1);
                                    }

                                    outtext += "...";
                                }

                                _lines.Add(outtext);

                                break;
                            }
                        }
                        while (text.Length > maxChars);

                        break;
                    }

                    _totalHeight = totalHeight;
                }

                #endregion

                totalHeight = _totalHeight;
                averageHeight = totalHeight / _lines.Count;

                foreach (string line in _lines)
                {
                    //для отображения используется прямоугольник 4/5 от ширины контрола
                    //отступ: 4 зазора от левого края
                    //graphics.ExtTextOut(clipRect.Left + (clipRect.Width - clipRect.Width * 4 / 5) / 4, clipRect.Top + topIndent + (clipRect.Height - totalHeight) / 2, line);

                    graphics.ExtTextOut(clipRect.Left + _leftPartSize.Width, clipRect.Top + topIndent + (clipRect.Height - totalHeight) / 2, line);

                    topIndent += averageHeight;
                }

                #endregion

            }

            /*            switch (Style)
                        {
                            case ButtonStyle.AlphaChannel:
                                IImage img1 = null;
                                if (_selected && !_pressed)
                                    img1 = TransparentButtonSelected;
                                if (_pressed || (_selected && img1 == null))
                                    img1 = TransparentButtonPressed;
                                if (!_pressed && !_selected)
                                    img1 = TransparentButton;

                                if (img1 != null)
                                    gMem.DrawImageAlphaChannel(img1, clipRect);
                                break;
                            case ButtonStyle.TransparentBackground:
                                ImageData img2 = null;
                                if (_selected && !_pressed)
                                    img2 = ButtonSelected;
                                if (_pressed || (_selected && img2 == null))
                                    img2 = ButtonPressed;
                                if (!_pressed && !_selected)
                                    img2 = Button;

                                if (img2 != null)
                                {
                                    int imgWidth = img2.ImageSize.Width;
                                    int imgHeight = img2.ImageSize.Height;
                                    gMem.TransparentImage(clipRect.Left, clipRect.Top,
                                                          clipRect.Width,
                                                          clipRect.Height,
                                                          img2.ImageHandle,
                                                          img2.ImageOffset.X,
                                                          img2.ImageOffset.Y,
                                                          imgWidth,
                                                          imgHeight,
                                                          img2.TransparentColor);
                                }
                                break;
                            default:
                                if (_pressed)
                                    gMem.FillRect(clipRect, BackColor);
                                else
                                    gMem.FillRect(clipRect, ForeColor);
                                break;
                        }

                        if (!string.IsNullOrEmpty(Text))
                        {
                            gMem.Font = Font;
                            gMem.TextColor = FontColor;
                            Size textSize = gMem.GetTextExtent(Text);
                            gMem.ExtTextOut(clipRect.Left + (clipRect.Width - textSize.Width) / 2,
                                            clipRect.Top + (clipRect.Height - textSize.Height) / 2, textSize.Width, Text);
                        }*/
        }

        #endregion

        #region IDisposable Members


        #endregion
    }
}
