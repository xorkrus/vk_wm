using System;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Forms;
using System.Collections.Generic;
using Galssoft.VKontakteWM.Components.Common.Configuration;

namespace Galssoft.VKontakteWM.CustomControlls
{
    class UploadPhotoControl : UIControl
    {
        public UploadPhotoControl()
        {
            _label = new UILabel { Visible = true };

            Style = ButtonStyle.AlphaChannel;
            base.Focusable = true;
            SetImages();

            _lines = new List<string>();
        }

        #region Private vars

        UILabel _label;

        IImage _leftPart;
        IImage _rightPart;
        IImage _centerPart;
        Size _leftPartSize;
        Size _rightPartSize;

        IImage _leftPartPressed;
        IImage _rightPartPressed;
        IImage _centerPartPressed;

        #endregion

        #region Defaults Properties

        public FontGdi DefaultButtonFont
        {
            get { return FontCache.CreateFont("Tahoma", 11, FontStyle.Regular, true); }
        }

        public Color DefaultFontColor
        {
            get { return Color.FromArgb(145, 145, 145); }
        }

        public FontGdi DefaultButtonPressedFont
        {
            get { return FontCache.CreateFont("Tahoma", 11, FontStyle.Regular, true); }
        }

        public Color DefaultPressedFontColor
        {
            get { return Color.White; }
        }

        public Color DefaultFontColorUnactive
        {
            get { return Color.White; }
        }

        public Color DefaultFontColorShadow
        {
            get { return Color.White; }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Title for the button
        /// </summary>
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

        public bool DropShadow
        {
            get { return _dropShadow; }
            set { _dropShadow = value; }
        }
        private bool _dropShadow = false;

        public FontGdi Font
        {
            get
            {
                if (_font != FontGdi.Empty)
                    return _font;
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
                return DefaultFontColor;
            }
            set { _fontColor = value; }
        }
        private Color _fontColor = Color.Empty;

        public FontGdi PressedFont
        {
            get
            {
                if (_pressedFont != FontGdi.Empty)
                    return _pressedFont;
                return DefaultButtonPressedFont;
            }
            set { _pressedFont = value; }
        }
        private FontGdi _pressedFont = FontGdi.Empty;

        public Color PressedFontColor
        {
            get
            {
                if (_pressedFontColor != Color.Empty)
                    return _pressedFontColor;
                return DefaultPressedFontColor;
            }
            set { _pressedFontColor = value; }
        }
        private Color _pressedFontColor = Color.Empty;

        public Color FontColorShadow
        {
            get
            {
                if (_fontColorShadow != Color.Empty)
                    return _fontColorShadow;
                return DefaultFontColorShadow;
            }
            set { _fontColorShadow = value; }
        }
        private Color _fontColorShadow = Color.Empty;

        public VerticalAlignment VerticalTextAlignment
        {
            get { return _verticalTextAlignment; }
            set { _verticalTextAlignment = value; }
        }
        private VerticalAlignment _verticalTextAlignment = VerticalAlignment.Center;

        public int VerticalTextMargin
        {
            get { return _verticalTextMargin; }
            set { _verticalTextMargin = value; }
        }
        private int _verticalTextMargin;

        public bool Pressed
        {
            get { return _pressed; }
            internal set { _pressed = value; }
        }
        private bool _pressed;

        /// <summary>
        /// Style to draw the button
        /// </summary>
        public ButtonStyle Style { get; set; }

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
                    Size = new Size((int)img.Width, (int)img.Height);
                }
            }
        }
        private IImage _transparentButton;

        public IImage TransparentButtonPressed { get; set; }

        public IImage TransparentButtonSelected { get; set; }

        public ImageData Button
        {
            get { return _button; }
            set
            {
                _button = value;

                if (_autosize)
                    Size = new Size(value.ImageSize.Width, value.ImageSize.Height);
            }
        }
        private ImageData _button;

        public ImageData ButtonPressed { get; set; }

        public ImageData ButtonSelected { get; set; }

        public bool Stretch
        {
            get { return _stretch; }
            set
            {
                _stretch = Style != ButtonStyle.AlphaChannel && value;
            }
        }
        private bool _stretch;

        public bool AutoSize
        {
            get { return _autosize; }
            set { _autosize = value; }
        }
        private bool _autosize = true;

        private List<string> _lines;

        #endregion

        #region Actions

        public override void Focus(bool setFocus)
        {
            if (setFocus)
            {
                //_selected = false;
                _pressed = true;
            }
            else
            {
                //_selected = false;
                _pressed = false;
            }
            OnInvalidate();
        }

        public override void OnMouseDown(MouseEventArgs e)
        {
            //_selected = true;
            OnInvalidate();
        }

        public override void OnMouseUp(MouseEventArgs e)
        {
            /*if (_selected)
            {
                _selected = false;
                
                
            }*/
            _pressed = false;
            OnClick(EventArgs.Empty);
            OnInvalidate();
        }

        #endregion

        #region Drawing

        public void SetImages()
        {
            _leftPart = MasterForm.SkinManager.GetImage("PhotoUpload1");
            _rightPart = MasterForm.SkinManager.GetImage("PhotoUpload3");
            _centerPart = MasterForm.SkinManager.GetImage("PhotoUpload2");

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
            }

            _leftPartPressed = MasterForm.SkinManager.GetImage("PhotoUpload1Pressed");
            _rightPartPressed = MasterForm.SkinManager.GetImage("PhotoUpload3Pressed");
            _centerPartPressed = MasterForm.SkinManager.GetImage("PhotoUpload2Pressed");

            if (_leftPartPressed != null)
            {
                _leftPartPressed.GetImageInfo(out ii);
            }
            if (_rightPartPressed != null)
            {
                _rightPartPressed.GetImageInfo(out ii);
            }
            if (_centerPartPressed != null)
            {
                _centerPartPressed.GetImageInfo(out ii);
            }
        }

        protected override void OnRelocate(EventArgs e)
        {
            base.OnRelocate(e);
            if (_leftPart != null && _rightPart != null && _centerPart != null)
            {
                var location = new Point(Location.X + _leftPartSize.Width,
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
                var size = new Size(Size.Width - _leftPartSize.Width - _rightPartSize.Width, Size.Height);
                var location = new Point(Location.X + _leftPartSize.Width, Location.Y + (_leftPartSize.Height - _leftPartSize.Height) / 2);

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
                var rect = new Rectangle(clipRect.Left + _leftPartSize.Width,
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

                if (!string.IsNullOrEmpty(Text))
                {
                    FontGdi font = Font;

                    if (_pressed)
                    {
                        font = PressedFont;
                    }

                    /*else if (_selected)
                        font = SelectedFont;*/

                    Color color = FontColor;

                    if (_pressed)
                    {
                        color = PressedFontColor;
                    }

                    /*else if (_selected)
                        color = SelectedFontColor;*/

                    graphics.Font = font;
                    graphics.TextColor = color;

                    Size textSize = graphics.GetTextExtent(Text);

                    int topAdjust = 0;

                    switch (VerticalTextAlignment)
                    {
                        case VerticalAlignment.Top:
                            topAdjust = 0 + _verticalTextMargin;
                            break;

                        case VerticalAlignment.Bottom:
                            topAdjust = Height - textSize.Height - _verticalTextMargin;
                            break;

                        case VerticalAlignment.Center:
                            topAdjust = (Height - textSize.Height) / 2;
                            break;
                    }

                    if (_dropShadow && !_pressed)
                    {
                        graphics.TextColor = FontColorShadow;

                        graphics.ExtTextOut(clipRect.Left + (clipRect.Width - textSize.Width) / 2 + UISettings.CalcPix(1), clipRect.Top + topAdjust + UISettings.CalcPix(1), textSize.Width, Text);
                    }

                    graphics.TextColor = color;

                    graphics.ExtTextOut(clipRect.Left + (clipRect.Width - textSize.Width) / 2, clipRect.Top + topAdjust, textSize.Width, Text);
                }
            }
        }

        #endregion
    }
}