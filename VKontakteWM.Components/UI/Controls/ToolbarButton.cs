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
    public class ToolbarButton
    {
        private readonly UIButton _button;
        private EventHandler _click;
        private MouseEventHandler _mouseClick;

        internal ToolbarButton()
            : this(ButtonStyle.Standard)
        {

        }

        internal ToolbarButton(ButtonStyle style)
        {
            _button = new UIButton(style);
            _button.AutoSize = false;
            _button.VerticalTextAlignment = VerticalAlignment.Bottom;
            _button.Click += ButtonClick;
            _button.MouseUp += ButtonMouseUp;
        }

        /// <summary>
        /// Переопределние MouseUp для того чтобы в качестве sender не передавался GraphicsButton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonMouseUp(object sender, MouseEventArgs e)
        {
            if (_mouseClick != null)
                _mouseClick(this, e);
        }

        /// <summary>
        /// Переопределение Click для того, что бы в качестве sender не передавался GraphicsButton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClick(object sender, EventArgs e)
        {
            if (_click != null)
                _click(this, e);
        }

        #region Public properties

        public event EventHandler Click
        {
            add
            {
                _click += value;
            }
            remove
            {
                _click -= value;
            }

        }

        public event MouseEventHandler MouseClick
        {
            add { _mouseClick += value; }
            remove { _mouseClick -= value; }
        }

        /// <summary>
        /// Надпись на кнопке
        /// </summary>
        public string Text
        {
            get
            {
                return _button.Text;
            }
            set
            {
                _button.Text = value;
            }
        }

        /// <summary>
        /// Текст надписи на кнопке
        /// </summary>
        public Color FontColor
        {
            get
            {
                return _button.FontColor;
            }
            set
            {
                _button.FontColor = value;
            }
        }

        /// <summary>
        /// Надпись на кнопке
        /// </summary>
        public FontGdi Font
        {
            get
            {
                return _button.Font;
            }
            set
            {
                _button.Font = value;
            }
        }

        /// <summary>
        /// Цвет надписи на кнопке в нажатом состоянии
        /// </summary>
        public Color PressedFontColor
        {
            get
            {
                return _button.PressedFontColor;
            }
            set
            {
                _button.PressedFontColor = value;
            }
        }

        /// <summary>
        /// Фонт надписи на кнопке в нажатом состоянии
        /// </summary>
        public FontGdi PressedFont
        {
            get
            {
                return _button.PressedFont;
            }
            set
            {
                _button.PressedFont = value;
            }
        }

        /// <summary>
        /// Цвет надписи на кнопке в выбранном состоянии
        /// </summary>
        public Color SelectedFontColor
        {
            get
            {
                return _button.SelectedFontColor;
            }
            set
            {
                _button.SelectedFontColor = value;
            }
        }

        /// <summary>
        /// Фонт надписи на кнопке в выбранном состоянии
        /// </summary>
        public FontGdi SelectedFont
        {
            get
            {
                return _button.SelectedFont;
            }
            set
            {
                _button.SelectedFont = value;
            }
        }

        /// <summary>
        /// Отступ от краев при написании текста
        /// </summary>
        public int VerticalTextMargin
        {
            get
            {
                return _button.VerticalTextMargin;
            }
            set
            {
                _button.VerticalTextMargin = value;
            }
        }

        /// <summary>
        /// Реализация через ImadeData для ненажатого состояния кнопки
        /// </summary>
        public ImageData Button
        {
            get
            {
                return _button.Button;
            }
            set
            {
                _button.Button = value;
            }
        }

        /// <summary>
        /// Реализация через ImadeData для нажатого состояния кнопки
        /// </summary>
        public ImageData ButtonPressed
        {
            get
            {
                return _button.ButtonPressed;
            }
            set
            {
                _button.ButtonPressed = value;
            }
        }

        /// <summary>
        /// Реализация через ImadeData для выбранного состояния кнопки
        /// </summary>
        public ImageData ButtonSelected
        {
            get
            {
                return _button.ButtonSelected;
            }
            set
            {
                _button.ButtonSelected = value;
            }
        }

        /// <summary>
        /// Цвет в нажатом состоянии
        /// </summary>
        public Color BackgroundColor
        {
            get
            {
                return _button.BackColor;
            }
            set
            {
                _button.BackColor = value;
            }
        }

        /// <summary>
        /// Цвет в ненажатом состоянии
        /// </summary>
        public Color ForeColor
        {
            get
            {
                return _button.ForeColor;
            }
            set
            {
                _button.ForeColor = value;
            }
        }

        /// <summary>
        /// Стиль прорисовки
        /// </summary>
        public ButtonStyle ButtonStyle
        {
            get
            {
                return _button.Style;
            }
            set
            {
                _button.Style = value;
            }
        }

        /// <summary>
        /// Реализация через IImage для ненажатого состояния кнопки
        /// </summary>
        public IImage TransparentButton
        {
            get
            {
                return _button.TransparentButton;
            }
            set
            {
                _button.TransparentButton = value;
            }
        }

        /// <summary>
        /// Реализация через IImage для нажатого состояния кнопки
        /// </summary>
        public IImage TransparentButtonPressed
        {
            get
            {
                return _button.TransparentButtonPressed;
            }
            set
            {
                _button.TransparentButtonPressed = value;
            }
        }

        /// <summary>
        /// Реализация через IImage для выбранного состояния кнопки
        /// </summary>
        public IImage TransparentButtonSelected
        {
            get
            {
                return _button.TransparentButtonSelected;
            }
            set
            {
                _button.TransparentButtonSelected = value;
            }
        }

        #endregion

        #region Internal properties

        /// <summary>
        /// Для прямого доступа к кнопке во внутренних классах
        /// </summary>
        internal UIButton Instance
        {
            get { return _button; }
        }

        #endregion

        #region Actions

        #endregion

        #region Drawing

        #endregion
    }
}