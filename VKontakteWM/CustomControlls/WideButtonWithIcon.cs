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
    public enum WideButtonWithIconType
    {
        Write = 0,
        Photo = 1,
        Open = 2
    }

    class WideButtonWithIcon : UIControl
    {        
        public WideButtonWithIcon(WideButtonWithIconType iconType)
        {
            Style = ButtonStyle.AlphaChannel;

            base.Focusable = true;

            _iconType = iconType;

            SetImages();

            _lines = new List<string>();            
        }

        #region Private vars

        IImage _leftPart;
        IImage _rightPart;
        IImage _centerPart;
        IImage _icon;

        Size _leftPartSize;
        Size _rightPartSize;              
        Size _iconSize;

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

        public WideButtonWithIconType IconType
        {
            get { return _iconType; }
            set { _iconType = value; }
        }
        private WideButtonWithIconType _iconType;

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
            _leftPart = MasterForm.SkinManager.GetImage("IconButtonLeft");
            _rightPart = MasterForm.SkinManager.GetImage("IconButtonRight");
            _centerPart = MasterForm.SkinManager.GetImage("IconButtonCenter");

            switch (_iconType)
            {
                case WideButtonWithIconType.Open:
                    _icon = MasterForm.SkinManager.GetImage("IconButtonIconOpen");
                    break;

                case WideButtonWithIconType.Photo:
                    _icon = MasterForm.SkinManager.GetImage("IconButtonIconPhoto");
                    break;

                case WideButtonWithIconType.Write:
                    _icon = MasterForm.SkinManager.GetImage("IconButtonIconWrite");
                    break;
            }            

            _leftPartPressed = MasterForm.SkinManager.GetImage("IconButtonLeftPressed");
            _rightPartPressed = MasterForm.SkinManager.GetImage("IconButtonRightPressed");
            _centerPartPressed = MasterForm.SkinManager.GetImage("IconButtonCenterPressed");            

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

            if (_icon != null)
            {
                _icon.GetImageInfo(out ii);
                _iconSize = new Size((int)ii.Width, (int)ii.Height);
            }

            //if (_centerPart != null)
            //{
            //    _centerPart.GetImageInfo(out ii);
            //}

            //if (_leftPartPressed != null)
            //{
            //    _leftPartPressed.GetImageInfo(out ii);
            //}

            //if (_rightPartPressed != null)
            //{
            //    _rightPartPressed.GetImageInfo(out ii);
            //}            

            //if (_centerPartPressed != null)
            //{
            //    _centerPartPressed.GetImageInfo(out ii);
            //}
        }

        protected override void OnRelocate(EventArgs e)
        {
            base.OnRelocate(e);

            if (_leftPart != null && _rightPart != null && _centerPart != null)
            {
                var location = new Point(Location.X + _leftPartSize.Width, Location.Y + (_leftPartSize.Height - _leftPartSize.Height) / 2);

                //_label.Location = location;
            }
            else
            {
                //_label.Location = Location;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (_leftPart != null && _rightPart != null && _centerPart != null)
            {
                var size = new Size(Size.Width - _leftPartSize.Width - _rightPartSize.Width, Size.Height);
                var location = new Point(Location.X + _leftPartSize.Width, Location.Y + (_leftPartSize.Height - _leftPartSize.Height) / 2);

                //_label.Location = location;
                //_label.Size = size;
            }
            else
            {
                //_label.Location = Location;
                //_label.Size = Size;
            }

            _lines.Clear();
        }

        protected override void OnRender(Gdi graphics, Rectangle clipRect)
        {
            if (_leftPart != null && _rightPart != null && _centerPart != null)
            {
                var rect = new Rectangle(clipRect.Left + _leftPartSize.Width, clipRect.Top, clipRect.Width - _rightPartSize.Width - _leftPartSize.Width, clipRect.Height);

                int iconIndentWidth = 0;
                int iconIndentHeight = 0;

                switch(_iconType)
                {
                    case WideButtonWithIconType.Write:
                    case WideButtonWithIconType.Photo:
                        iconIndentWidth = 7;
                        iconIndentHeight = 6;
                        break;
                    case WideButtonWithIconType.Open:
                        iconIndentWidth = 5;
                        iconIndentHeight = 7;
                        break;
                }

                if (!_pressed)
                {
                    graphics.DrawImageAlphaChannel(_leftPart, clipRect.Left, clipRect.Top);
                    graphics.DrawImageAlphaChannel(_rightPart, rect.Right, clipRect.Top);
                    graphics.DrawImageAlphaChannel(_centerPart, rect);
                    graphics.DrawImageAlphaChannel(_icon, clipRect.Left + UISettings.CalcPix(iconIndentWidth), clipRect.Top + UISettings.CalcPix(iconIndentHeight));
                }
                else
                {
                    graphics.DrawImageAlphaChannel(_leftPartPressed, clipRect.Left, clipRect.Top);
                    graphics.DrawImageAlphaChannel(_rightPartPressed, rect.Right, clipRect.Top);
                    graphics.DrawImageAlphaChannel(_centerPartPressed, rect);
                    graphics.DrawImageAlphaChannel(_icon, clipRect.Left + UISettings.CalcPix(iconIndentWidth), clipRect.Top + UISettings.CalcPix(iconIndentHeight + 1));
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

                    int pressIndent = 0;

                    if (_pressed)
                    {
                        pressIndent = UISettings.CalcPix(1);
                    }

                    //if (_dropShadow && !_pressed)
                    if (_dropShadow)
                    {
                        graphics.TextColor = FontColorShadow;

                        //graphics.ExtTextOut(clipRect.Left + (clipRect.Width - textSize.Width) / 2 + UISettings.CalcPix(1), clipRect.Top + topAdjust + UISettings.CalcPix(1), textSize.Width, Text);
                        graphics.ExtTextOut(clipRect.Left + UISettings.CalcPix(22 + 5), clipRect.Top + topAdjust + UISettings.CalcPix(1) + pressIndent, textSize.Width, Text);
                    }

                    graphics.TextColor = color;

                    //graphics.ExtTextOut(clipRect.Left + (clipRect.Width - textSize.Width) / 2, clipRect.Top + topAdjust, textSize.Width, Text);
                    graphics.ExtTextOut(clipRect.Left + UISettings.CalcPix(22 + 5), clipRect.Top + topAdjust + pressIndent, textSize.Width, Text);
                }
            }
        }

        #endregion
    }
}
