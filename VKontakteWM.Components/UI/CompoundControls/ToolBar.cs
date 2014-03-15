/*	 This file was created by GALS Soft	company
 *	 http://www.galssoft.com
 *	 
 *	 Project name is Odnoklassniki Mobile
 *	 Platform is .Net Compact Framework 2.0 for Windows Mobile 5.0+ 
 * 
 *	 Copyright (c) 2009-2010 GALS Soft
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.UI.Controls;

namespace Galssoft.VKontakteWM.Components.UI.CompoundControls
{
    public class ToolBar : UIElementBase
    {
        //Размер кнопки по умолчанию
        private readonly int _defButtonSize = 8;

        private GraphicsImage _backgroundImage;
        private List<ToolbarButton> _buttons;
        private ToolbarButton _selected;
        private Size _buttonsSize;
        private int _dpiScaling;

        public ToolBar()
        {
            base.Focusable = true;
            _dpiScaling = UISettings.CalcPix(1);
            _defButtonSize = UISettings.CalcPix(_defButtonSize);
            _buttons = new List<ToolbarButton>();
            _buttonsSize = new Size(_defButtonSize, _defButtonSize);
        }

        public ToolBar(GraphicsImage backGroundImage)
            : this()
        {
            _backgroundImage = backGroundImage;
            _backgroundImage.Size = Size;
        }

        #region Public Properties

        /// <summary>
        /// Размер кнопок
        /// </summary>
        public Size ButtonsSize
        {
            get
            {
                return new Size(_buttonsSize.Width / _dpiScaling, _buttonsSize.Height / _dpiScaling);
            }
            set
            {
                _buttonsSize = new Size(value.Width * _dpiScaling, value.Height * _dpiScaling);
                ResizeButtons();
                RecalcButtonLayout();
            }
        }

        /// <summary>
        /// Подложка
        /// </summary>
        public GraphicsImage BackgroundImage
        {
            get
            {
                return _backgroundImage;
            }
            set
            {
                _backgroundImage = value;
            }
        }

        #endregion

        #region Public Functions

        public ToolbarButton AddButton()
        {
            return AddButton(ButtonStyle.Standard);
        }

        public int ButtonsCount()
        {
            return _buttons.Count;
        }

        public ToolbarButton GetButton(int index)
        {
            return _buttons[index];
        }

        public ToolbarButton AddButton(ButtonStyle style)
        {
            ToolbarButton b = new ToolbarButton();
            b.Instance.Size = _buttonsSize;
            b.Instance.Style = style;
            _buttons.Add(b);
            b.Instance.Invalidate += element_Invalidate;
            RecalcButtonLayout();
            return b;
        }

        public void RemoveButton(ToolbarButton button)
        {
            if (_buttons.Contains(button))
                _buttons.Remove(button);
            button.Instance.Invalidate -= element_Invalidate;
            RecalcButtonLayout();
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Изменение размера кнопок
        /// </summary>
        private void ResizeButtons()
        {
            foreach (ToolbarButton button in _buttons)
            {
                button.Instance.Size = _buttonsSize;
            }
        }

        /// <summary>
        /// Перерасчет расположения кнопок
        /// </summary>
        public void RecalcButtonLayout()
        {
            if (_buttonsSize == Size.Empty)
                return;
            if (_buttons.Count == 0)
                return;

            if (Width > Height)
            {
                int i = (Height - _buttonsSize.Height) / 2;

                //Использованно под кнопки
                int y = _buttonsSize.Width * _buttons.Count;
                //Осталось пустого пространства
                y = Width - y;
                if (y < 0)
                    y = 0;

                //Пустые отрезки
                float space = (float)y / (_buttons.Count + 1);

                float z = 0;
                for (int j = 0; j < _buttons.Count; j++)
                {
                    z += space;
                    _buttons[j].Instance.Location = new Point((int)z + Location.X, i + Location.Y);
                    z += _buttonsSize.Width;
                }
            }
            else
            {
                int i = (width - _buttonsSize.Width) / 2;

                //Использованно под кнопки
                int x = _buttonsSize.Height * _buttons.Count;
                //Осталось пустого пространства
                x = Height - x;
                if (x < 0)
                    x = 0;

                //Пустые отрезки
                float space = (float)x / (_buttons.Count + 1);

                float z = 0;
                for (int j = 0; j < _buttons.Count; j++)
                {
                    z += space;
                    _buttons[j].Instance.Location = new Point(i + Location.X, (int)z + Location.Y);
                    z += _buttonsSize.Height;
                }
            }
        }

        #endregion

        #region Actions

        protected List<ToolbarButton> _mouseDownObjects = new List<ToolbarButton>();

        public override void OnMouseDown(MouseEventArgs e)
        {
            foreach (ToolbarButton button in _buttons)
            {
                if (button.Instance.HitTest(new Point(e.X, e.Y)))
                {
                    if (!(button == _selected))
                    {
                        _mouseDownObjects.Add(button);
                        button.Instance.OnMouseDown(e);
                    }
                }
            }

            base.OnMouseDown(e);
        }

        public override void OnMouseUp(MouseEventArgs e)
        {
            if (_mouseDownObjects.Count > 0)
                foreach (ToolbarButton button in _mouseDownObjects)
                {
                    if (button.Instance.HitTest(new Point(e.X, e.Y)))
                    {
                        SelectButton(button);
                    }
                    else
                        button.Instance.Focus(false);
                }

            _mouseDownObjects.Clear();

            base.OnMouseUp(e);
        }

        public override void Focus(bool setFocus)
        {
            if (!setFocus)
            {
                foreach (ToolbarButton button in _mouseDownObjects)
                {
                    button.Instance.Focus(false);
                }
                _mouseDownObjects.Clear();
            }

            base.Focus(setFocus);
        }

        protected override void OnResize(EventArgs e)
        {
            //Перерасчет положения
            RecalcButtonLayout();
            base.OnResize(e);
        }

        protected override void OnRelocate(EventArgs e)
        {
            //Перерасчет положения
            RecalcButtonLayout();
            base.OnRelocate(e);
        }

        /// <summary>
        /// Выбор кнопки
        /// </summary>
        /// <param name="button"></param>
        public void SelectButton(ToolbarButton button)
        {
            if (!_buttons.Contains(button) || _selected == button)
                return;

            int selectedButton = -1;
            for (int i = 0; i < _buttons.Count; i++)
            {
                _buttons[i].Instance.Selected = false;
                _buttons[i].Instance.Pressed = _buttons[i] == button;
                if (_buttons[i] == button)
                {
                    selectedButton = i;
                }
            }

            foreach (ToolbarButton toolbarButton in _buttons)
            {
                OnInvalidate(toolbarButton.Instance);
            }

            if (selectedButton > -1)
            {
                _selected = button;
                _buttons[selectedButton].Instance.OnClick(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Выбор кнопки
        /// </summary>
        /// <param name="buttonID"></param>
        public void SelectButton(int buttonID)
        {
            if (buttonID < 0 || buttonID > _buttons.Count - 1)
                return;

            int pressedButton = -1;
            for (int i = 0; i < _buttons.Count; i++)
            {
                _buttons[i].Instance.Pressed = i == buttonID;
                if (i == buttonID)
                {
                    pressedButton = i;
                }
            }

            foreach (ToolbarButton toolbarButton in _buttons)
            {
                OnInvalidate(toolbarButton.Instance);
            }

            if (pressedButton > -1)
            {
                _selected = _buttons[pressedButton];
                _buttons[pressedButton].Instance.OnClick(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Убрать выбор кнопок
        /// </summary>
        public void DeselectButtons()
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                _buttons[i].Instance.Pressed = false;
                OnInvalidate(_buttons[i].Instance);
            }
        }

        #endregion

        #region Drawing

        void element_Invalidate(object sender, EventArgs e)
        {
            // Pass invalidation event to the canvas          
            OnInvalidate((UIElementBase)sender);
        }

        protected override void OnRender(Gdi gMem, Rectangle clipRect)
        {
            if (_backgroundImage != null)
                _backgroundImage.Render(gMem, clipRect);
            else
                gMem.FillRect(Rectangle, BackColor);

            if (_buttons.Count > 0)
            {
                for (int i = 0; i < _buttons.Count; i++)
                {
                    _buttons[i].Instance.Render(gMem, new Rectangle(_buttons[i].Instance.Location.X,
                        _buttons[i].Instance.Location.Y,
                        _buttonsSize.Width, _buttonsSize.Height));
                }
            }
        }

        #endregion

        #region IDisposable Members

        #endregion
    }
}