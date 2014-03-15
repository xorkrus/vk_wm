/*	 This file was created by GALS Soft	company
 *	 http://www.galssoft.com
 *	 
 *	 Project name is Odnoklassniki Mobile
 *	 Platform is .Net Compact Framework 2.0 for Windows Mobile 5.0+ 
 * 
 *	 Copyright (c) 2009-2010 GALS Soft
 */

using System;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.GDI;

namespace Galssoft.VKontakteWM.Components.UI.Controls
{
    public class UIButton : UIControl
    {
        public UIButton()
            : this(ButtonStyle.Standard)
        {
        }

        public UIButton(ButtonStyle style)
        {
            Style = style;
            base.Focusable = true;
        }

        #region Defaults Properties

        public FontGdi DefaultButtonFont
        {
            get { return FontCache.CreateFont("Calibri", 13, FontStyle.Bold, true); }
        }

        public Color DefaultFontColor
        {
            get { return Color.Black; }
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
        private string _text = null;

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

        public FontGdi PressedFont
        {
            get
            {
                if (_pressedFont != FontGdi.Empty)
                    return _pressedFont;
                else
                    return Font;
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
                else
                    return FontColor;
            }
            set { _pressedFontColor = value; }
        }
        private Color _pressedFontColor = Color.Empty;

        public FontGdi SelectedFont
        {
            get
            {
                if (_selectedFont != FontGdi.Empty)
                    return _selectedFont;
                else
                    return Font;
            }
            set { _selectedFont = value; }
        }
        private FontGdi _selectedFont = FontGdi.Empty;

        public Color SelectedFontColor
        {
            get
            {
                if (_selectedFontColor != Color.Empty)
                    return _selectedFontColor;
                else
                    return FontColor;
            }
            set { _selectedFontColor = value; }
        }
        private Color _selectedFontColor = Color.Empty;

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
        private int _verticalTextMargin = 0;

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
                OnInvalidate();
                OnClick(EventArgs.Empty);
            }
        }

        #endregion

        #region Drawing

        protected override void OnRender(Gdi gMem, Rectangle clipRect)
        {
            switch (Style)
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
                    if (_pressed || _selected)
                        gMem.FillRect(clipRect, BackColor);
                    else
                        gMem.FillRect(clipRect, ForeColor);
                    break;
            }

            if (!string.IsNullOrEmpty(Text))
            {
                FontGdi font = Font;
                if (_pressed)
                    font = PressedFont;
                else if (_selected)
                    font = SelectedFont;

                Color color = FontColor;
                if (_pressed)
                    color = PressedFontColor;
                else if (_selected)
                    color = SelectedFontColor;

                gMem.Font = font;
                gMem.TextColor = color;
                Size textSize = gMem.GetTextExtent(Text);

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

                gMem.ExtTextOut(clipRect.Left + (clipRect.Width - textSize.Width) / 2,
                                clipRect.Top + topAdjust, textSize.Width, Text);
            }
        }

        #endregion

        #region IDisposable Members


        #endregion

    }
}