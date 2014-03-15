/*	 This file was created by GALS Soft	company
 *	 http://www.galssoft.com
 *	 
 *	 Project name is Odnoklassniki Mobile
 *	 Platform is .Net Compact Framework 2.0 for Windows Mobile 5.0+ 
 * 
 *	 Copyright (c) 2009-2010 GALS Soft
 */

using System;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.GDI;
using System.Drawing;

namespace Galssoft.VKontakteWM.Components.UI.Controls
{
    public class UITextControl : UIControl, IInitializeAfterConstructor
    {
        #region Constructor and destructor

        public UITextControl()
        {
            base.Focusable = true;

            base.SuspendLayout();

            _textbox = new TextBox();
            _textbox.TextChanged += new EventHandler(TextBox_TextChanged);
            _textbox.LostFocus += new EventHandler(TextBox_LostFocus);
            _textbox.BorderStyle = BorderStyle.None;
            _textbox.Visible = false;

            _label = new UILabel();
            _label.Visible = true;


            base.ResumeLayout();
            base.Remeasure();
        }

        #region IInitializeAfterConstructor Members

        void IInitializeAfterConstructor.Initialize()
        {
            Control host = FindHost();
            if (host != null)
                host.Controls.Add(_textbox);
            else
                throw new NotSupportedException("UITextControl can't find Control host");
        }

        #endregion

        public override void Dispose()
        {
            Control host = FindHost();
            //if (host != null)
                //host.Controls.Remove(_textbox);

            base.Dispose();
        }

        #endregion

        #region Private vars

        TextBox _textbox;
        UILabel _label;

        IImage _leftBorder;
        IImage _rightBorder;
        IImage _topBorder;
        IImage _bottomBorder;
        Size _leftBorderSize;
        Size _rightBorderSize;
        Size _topBorderSize;
        Size _bottomBorderSize;

        #endregion

        public FontGdi Font
        {
            get
            {
                return _label.Font;
            }
            set
            {
                _label.Font = value;
                _textbox.Font = System.Drawing.Font.FromHfont((IntPtr)value);
            }
        }

        public Color TextColor
        {
            get
            {
                return _label.ForeColor;
            }
            set
            {
                _label.ForeColor = value;
            }
        }

        public char PasswordChar //Добавил поддержку PasswordChar (Николай Шестаков, 21.06.2010)
        {
            get 
            {
                return _textbox.PasswordChar;
            }

            set
            {
                _textbox.PasswordChar = value;
            }
        }

        public TextBox TextBox
        {
            get { return _textbox; }
            set { _textbox = value; }
        }

        bool _isEditing = false;
        public bool IsEditing
        {
            get
            {
                return _isEditing;
            }
            set
            {
                if (value != _isEditing)
                {
                    _isEditing = value;
                    OnEditingChanged();
                }
            }
        }

        public override void Focus(bool setFocus)
        {
            base.Focus(setFocus);

            if (!setFocus)
            {
                if (IsEditing)
                {
                    IsEditing = false;
                }
            }
        }

        public void SetImages(IImage leftBorder, IImage rightBorder, IImage topBorder, IImage bottomBorder)
        {
            _leftBorder = leftBorder;
            _rightBorder = rightBorder;
            _topBorder = topBorder;
            _bottomBorder = bottomBorder;
            ImageInfo ii;
            if (_leftBorder != null)
            {
                _leftBorder.GetImageInfo(out ii);
                _leftBorderSize = new Size((int)ii.Width, (int)ii.Height);
            }
            if (_rightBorder != null)
            {
                _rightBorder.GetImageInfo(out ii);
                _rightBorderSize = new Size((int)ii.Width, (int)ii.Height);
            }
            if (_topBorder != null)
            {
                _topBorder.GetImageInfo(out ii);
                _topBorderSize = new Size((int)ii.Width, (int)ii.Height);
            }
            if (_bottomBorder != null)
            {
                _bottomBorder.GetImageInfo(out ii);
                _bottomBorderSize = new Size((int)ii.Width, (int)ii.Height);
            }
        }

        void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (_textbox.PasswordChar == 0) //Добавил поддержку PasswordChar (Николай Шестаков, 21.06.2010)
            {
                if (_label.Text != _textbox.Text)
                    _label.Text = Text; // this triggers a remeasure
            }
            else
            {
                string s = new string(_textbox.PasswordChar, _textbox.TextLength);
                if (_label.Text != s)
                    _label.Text = s;   // this triggers a remeasure
            }
            
        }

        void TextBox_LostFocus(object sender, EventArgs e)
        {
            if (IsEditing)
                IsEditing = false;
        }

        public override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (!IsEditing)
                IsEditing = true;
        }

        protected override void OnKeyPress(Control sender, KeyPressEventArgs e)
        {
            if (!e.Handled && e.KeyChar == '\r')
            {
                IsEditing = !IsEditing;
                e.Handled = true;
            }

            base.OnKeyPress(sender, e);
        }

        protected void OnEditingChanged()
        {
            if (IsEditing)
            {
                _textbox.Location = _label.Location;
                _textbox.Size = _label.Size;
                _textbox.Visible = true;
                _label.Visible = false;
                _textbox.Focus();
            }
            else
            {
                _label.Visible = true;
                _textbox.Visible = false;
            }
        }

        public string Text
        {
            get
            {
                return _textbox.Text;
            }
            set
            {
                _label.Text = _textbox.Text = value;
            }
        }

        protected override void OnRender(Gdi graphics, Rectangle clipRect)
        {
            if (_leftBorder != null && _rightBorder != null && _topBorder != null && _bottomBorder != null)
            {
                Rectangle rect = new Rectangle(clipRect.Left + _leftBorderSize.Width,
                                               clipRect.Top + _topBorderSize.Height,
                                               clipRect.Width - _leftBorderSize.Width - _rightBorderSize.Width,
                                               _leftBorderSize.Height - _topBorderSize.Height - _bottomBorderSize.Height);

                if (_leftBorder != null)
                    graphics.DrawImageAlphaChannel(_leftBorder, clipRect.Left, clipRect.Top);
                if (_rightBorder != null)
                    graphics.DrawImageAlphaChannel(_rightBorder, rect.Right,
                                                   clipRect.Top);
                if (_topBorder != null)
                    graphics.DrawImageAlphaChannel(_topBorder,
                                                   new Rectangle(rect.Left, clipRect.Top,
                                                                 rect.Width, _topBorderSize.Height));
                if (_bottomBorder != null)
                    graphics.DrawImageAlphaChannel(_bottomBorder,
                                                   new Rectangle(rect.Left,
                                                                 rect.Bottom,
                                                                 rect.Width, _topBorderSize.Height));

                if (BackColor != Color.Empty)
                    graphics.FillRect(rect, BackColor);
            }
            else
            {
                //if (BackColor != Color.Empty)
                //    graphics.FillRect(clipRect, BackColor);
            }

            if (_textbox.Visible)
            {
                //
            }
            else
            {
                _label.Render(graphics, clipRect);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (_leftBorder != null && _rightBorder != null && _topBorder != null && _bottomBorder != null)
            {
                Size size = new Size(Size.Width - _leftBorderSize.Width - _rightBorderSize.Width, Size.Height);
                Point location = new Point(Location.X + _leftBorderSize.Width, Location.Y + (_leftBorderSize.Height - _textbox.Size.Height) / 2);

                _textbox.Location = location;
                _label.Location = _textbox.Location;
                _textbox.Size = size;
                _label.Size = _textbox.Size;
            }
            else
            {
                _textbox.Location = Location;
                _label.Location = Location;
                _textbox.Size = Size;
                _label.Size = Size;
            }
        }

        protected override void OnRelocate(EventArgs e)
        {
            base.OnRelocate(e);
            if (_leftBorder != null && _rightBorder != null && _topBorder != null && _bottomBorder != null)
            {
                Point location = new Point(Location.X + _leftBorderSize.Width, Location.Y + (_leftBorderSize.Height - _textbox.Size.Height) / 2);

                _textbox.Location = location;
                _label.Location = _textbox.Location;
            }
            else
            {
                _textbox.Location = Location;
                _label.Location = Location;
            }
        }
    }
}