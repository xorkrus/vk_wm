using System;
using System.Drawing;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.Properties;
using Microsoft.WindowsCE.Forms;

namespace Galssoft.VKontakteWM.CustomControlls
{
    public partial class FilterControl : UIViewBase
    {
        #region Constructors

        public FilterControl(int width, int height)
        {
            Size = new Size(width, height);
            InitializeComponent();
        }

        #endregion

        #region Private Methods

        private Components.GDI.IImage PrepareBackground()
        {
            return ImagingHelper.CropIImage(MasterForm.SkinManager.GetImage("Header"), 
                new Rectangle(Location.X, Location.Y, UISettings.CalcPix(10), UISettings.CalcPix(25)));
        }

        private void TextBoxGotFocus(object sender, EventArgs e)
        {
            (new InputPanel()).Enabled = true;

            if (_filter.Text == Resources.FilterText)
            {
                _filter.Text = string.Empty;
                _filter.TextColor = Color.Black;
                _filter.OnInvalidate();
                ClearButtonVisibleChange(false);
            }
        }

        private void TextBoxLostFocus(object sender, EventArgs e)
        {
            (new InputPanel()).Enabled = false;

            if (_filter.Text == string.Empty)
            {
                _filter.TextColor = Color.DarkGray;
                _filter.Text = Resources.FilterText;

                if (_filter.Width == DefaultFilterWidth)
                    _filter.Width = WideFilterWidth;
                
                _filter.OnInvalidate();
                ClearButtonVisibleChange(false);
            }
        }

        private void ClearClick(object sender, EventArgs e)
        {
            _filter.TextColor = Color.DarkGray;
            _filter.Text = Resources.FilterText;

            if (_filter.Width == DefaultFilterWidth)
                _filter.Width = WideFilterWidth;

            _filter.Focus(true);
            _filter.OnInvalidate();
            ClearButtonVisibleChange(false);
        }

        private void FilterControlResize(object sender, EventArgs e)
        {
            DefaultFilterWidth = Width - UISettings.CalcPix(33);
            WideFilterWidth = Width - UISettings.CalcPix(16);
            _filter.Focus(false);
        }

        private void TextBoxTextChanged(object sender, EventArgs e)
        {
            if (_filter.Text != string.Empty || _filter.Text != Resources.FilterText)
            {
                _filter.Width = DefaultFilterWidth;
            }
        }

        #endregion

        #region Public Methods

        //inputPanelEnabledChanged

        public void ClearButtonVisibleChange(bool needShow)
        {
            _clear.Visible = needShow;
        }

        #endregion
    }
}
