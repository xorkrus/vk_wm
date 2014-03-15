using System;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.GDI;
using System.Drawing;

namespace Galssoft.VKontakteWM.Components.UI.CompoundControls
{
    public partial class SettingsListView
    {
        protected abstract class ExNativeItemData : NativeItemData
        {
            public SettingsListViewItems DataSource;
            public int WorkAreaWidth;
            public FontGdi Font;

            public abstract void DrawItemOn(Gdi g, Rectangle rItem);
            public abstract void ClickOn(int pos);
        }

        protected class CheckBoxNativeItemData : ExNativeItemData
        {
            private IImage _checkBoxEmpty;
            private IImage _checkBoxSelected;
            private Size _imageSize;

            public IImage CheckBoxEmpty
            {
                set
                {
                    _checkBoxEmpty = value;
                    if (_imageSize == Size.Empty && _checkBoxEmpty != null)
                    {
                        ImageInfo ii;
                        _checkBoxEmpty.GetImageInfo(out ii);
                        _imageSize = new Size((int)ii.Width, (int)ii.Height);
                    }
                }
            }
            public IImage CheckBoxSelected
            {
                set
                {
                    _checkBoxSelected = value;
                    if (_imageSize == Size.Empty && _checkBoxSelected != null)
                    {
                        ImageInfo ii;
                        _checkBoxSelected.GetImageInfo(out ii);
                        _imageSize = new Size((int)ii.Width, (int)ii.Height);
                    }
                }
            }

            public override void DrawItemOn(Gdi g, Rectangle rItem)
            {
                int x = (rItem.Width - _imageSize.Width) / 2;
                int y = (rItem.Height - _imageSize.Height) / 2;

                ImageInfo ii;
                _checkBoxSelected.GetImageInfo(out ii);

                if ((bool)DataSource.OptionValue)
                {
                    if (_checkBoxSelected != null)
                        g.DrawImageAlphaChannel(_checkBoxSelected, rItem.Right - (int)ii.Width - UISettings.CalcPix(10), rItem.Top + y);
                }
                else
                {
                    if (_checkBoxEmpty != null)
                        g.DrawImageAlphaChannel(_checkBoxEmpty, rItem.Right - (int)ii.Width - UISettings.CalcPix(10), rItem.Top + y);
                }
            }

            public override void ClickOn(int pos)
            {
                int x = (WorkAreaWidth - _imageSize.Width * 2) / 2 + UISettings.CalcPix(24);
                int x2 = (WorkAreaWidth + _imageSize.Width * 2) / 2 + UISettings.CalcPix(24);
                if (pos > x && pos < x2)
                {
                    DataSource.OptionValue = !((bool)DataSource.OptionValue);
                }
            }
        }

        protected class TwoItemsNativeItemData : ExNativeItemData
        {
            private IImage _itemLeft;
            private IImage _itemRight;
            private IImage _itemLeftSelected;
            private IImage _itemRightSelected;
            private Size _imageSize;

            public IImage ItemLeft
            {
                set
                {
                    _itemLeft = value;
                    if (_imageSize == Size.Empty && _itemLeft != null)
                    {
                        ImageInfo ii;
                        _itemLeft.GetImageInfo(out ii);
                        _imageSize = new Size((int)ii.Width, (int)ii.Height);
                    }
                }
            }
            public IImage ItemRight
            {
                set
                {
                    _itemRight = value;
                    if (_imageSize == Size.Empty && _itemRight != null)
                    {
                        ImageInfo ii;
                        _itemRight.GetImageInfo(out ii);
                        _imageSize = new Size((int)ii.Width, (int)ii.Height);
                    }
                }
            }
            public IImage ItemLeftSelected
            {
                set
                {
                    _itemLeftSelected = value;
                    if (_imageSize == Size.Empty && _itemLeftSelected != null)
                    {
                        ImageInfo ii;
                        _itemLeftSelected.GetImageInfo(out ii);
                        _imageSize = new Size((int)ii.Width, (int)ii.Height);
                    }
                }
            }
            public IImage ItemRightSelected
            {
                set
                {
                    _itemRightSelected = value;
                    if (_imageSize == Size.Empty && _itemRightSelected != null)
                    {
                        ImageInfo ii;
                        _itemRightSelected.GetImageInfo(out ii);
                        _imageSize = new Size((int)ii.Width, (int)ii.Height);
                    }
                }
            }

            public override void DrawItemOn(Gdi g, Rectangle rItem)
            {
                int x = rItem.Width / 2;
                int y = (rItem.Height - _imageSize.Height) / 2;

                Color color1;
                Color color2;

                g.Font = Font;

                if ((string)DataSource.OptionValue == DataSource.OptionValues[0])
                {
                    if (_itemLeftSelected != null)
                        g.DrawImageAlphaChannel(_itemLeftSelected, rItem.Left + x - _imageSize.Width, rItem.Top + y);
                    if (_itemRight != null)
                        g.DrawImageAlphaChannel(_itemRight, rItem.Left + x, rItem.Top + y);
                    color1 = Color.White;
                    color2 = Color.Gray;
                }
                else
                {
                    if (_itemLeft != null)
                        g.DrawImageAlphaChannel(_itemLeft, rItem.Left + x - _imageSize.Width, rItem.Top + y);
                    if (_itemRightSelected != null)
                        g.DrawImageAlphaChannel(_itemRightSelected, rItem.Left + x, rItem.Top + y);
                    color1 = Color.Gray;
                    color2 = Color.White;
                }

                g.TextAlign = Win32.TextAlign.TA_LEFT;
                g.TextColor = color1;
                Size textSize = g.GetTextExtent(DataSource.OptionValues[0]);
                y = (rItem.Height - textSize.Height) / 2;
                int left = x - _imageSize.Width;
                g.ExtTextOut(rItem.Left + left, rItem.Top + y, _imageSize.Width, DataSource.OptionValues[0]);
                g.TextColor = color2;
                left = x;
                g.ExtTextOut(rItem.Left + left, rItem.Top + y, _imageSize.Width, DataSource.OptionValues[1]);
            }

            public override void ClickOn(int pos)
            {
                int x = WorkAreaWidth / 2;
                int x2 = x - _imageSize.Width;
                if (pos > x2 && pos < x)
                {
                    DataSource.OptionValue = DataSource.OptionValues[0];
                }
                else if (pos > x && pos < WorkAreaWidth - x2)
                {
                    DataSource.OptionValue = DataSource.OptionValues[1];
                }
            }
        }

        protected class MultiItemsNativeItemData : ExNativeItemData
        {
            private IImage _selectButton;
            private Size _imageSize;

            public event EventHandler Select;

            public IImage SelectButton
            {
                set
                {
                    _selectButton = value;
                    ImageInfo ii;
                    _selectButton.GetImageInfo(out ii);
                    _imageSize = new Size((int)ii.Width, (int)ii.Height);
                }
            }

            public override void DrawItemOn(Gdi g, Rectangle rItem)
            {
                int y = (rItem.Height - _imageSize.Height) / 2;

                g.Font = Font;

                if (_selectButton != null)
                    g.DrawImageAlphaChannel(_selectButton, rItem.Right - _imageSize.Width - UISettings.CalcPix(15), rItem.Top + y);

                g.TextColor = Color.FromArgb(51, 153, 255);
                var textSize = g.GetTextExtent((string)DataSource.OptionValue);
                y = (rItem.Height - textSize.Height) / 2;
                int x = rItem.Width - UISettings.CalcPix(20) - _imageSize.Width;
                if (textSize.Width < x)
                    x = textSize.Width;

                g.DrawText((string)DataSource.OptionValue,
                    new Win32.RECT(rItem.Right - UISettings.CalcPix(20) - _imageSize.Width - x,
                                   rItem.Top + y,
                                   x,
                                   textSize.Height),
                    Win32.DT.LEFT | Win32.DT.TOP | Win32.DT.WORDBREAK | Win32.DT.SINGLELINE);
            }

            public override void ClickOn(int pos)
            {
                if (pos > WorkAreaWidth - UISettings.CalcPix(60) - _imageSize.Width)
                {
                    if (Select != null)
                        Select(this, EventArgs.Empty);
                }
            }
        }
    }
}
