using System;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.Properties;
using Microsoft.WindowsCE.Forms;
using AnchorStyles = Galssoft.VKontakteWM.Components.UI.AnchorStyles;

namespace Galssoft.VKontakteWM.CustomControlls
{
    partial class FilterControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._filter = new UITextControl();
            this._clear = new UIButton(ButtonStyle.AlphaChannel);
            this._leftBorder = new GraphicsImage(MasterForm.SkinManager.GetImage("LeftFilterBorder"), false);
            this._rightBorder = new GraphicsImage(MasterForm.SkinManager.GetImage("RightFilterBorder"), false);
            this._centerImage = new GraphicsImage(MasterForm.SkinManager.GetImage("FilterCenter"), true);
            this._topBorder = new GraphicsImage();
            this._bottomBorder = new GraphicsImage();
            this._backgroundGradient = new GraphicsImage();

            inputPanel = new InputPanel();

            this.SuspendLayout();


            inputPanel.Enabled = false;

            //
            // element "FilterControl"
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F); 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Name = "FilterControl";
            this.Anchor = System.Windows.Forms.AnchorStyles.Left |
                          System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.Resize += new System.EventHandler(FilterControlResize);
            this.DefaultFilterWidth = this.Width - 33;
            this.WideFilterWidth = this.Width - 16;

            //
            // element "_backgroundGradient"
            //
            this._backgroundGradient.Location = new Point(0, 0);
            this._backgroundGradient.Size = this.Size;
            this._backgroundGradient.Stretch = true;
            this._backgroundGradient.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Right;
            this._backgroundGradient.Name = "_backgroundGradient";

            // 
            // element "_centerImage"
            //
            this._centerImage.Location = new Point(8, 2);
            this._centerImage.Size = new Size(this.Width - 16, 21);
            this._centerImage.Name = "_centerImage";
            this._centerImage.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left;

            //
            // element "_clear"
            //
            this._clear.TransparentButton = MasterForm.SkinManager.GetImage("FilterClear");
            this._clear.TransparentButtonPressed = MasterForm.SkinManager.GetImage("FilterClear");
            this._clear.Location = new Point(this.Width - 25, 4);
            this._clear.Size = new Size(17, 17);
            this._clear.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this._clear.Name = "_clear";
            this.ClearButtonVisibleChange(false);
            this._clear.Click += new System.EventHandler(ClearClick);

            //
            // element "_filter"
            //
            this._filter.Font = FontCache.CreateFont("Calibri", 16, System.Drawing.FontStyle.Regular);
            this._filter.TextColor = System.Drawing.Color.DarkGray;
            this._filter.Text = Resources.FilterText;
            this._filter.Name = "_filter";
            this._filter.Location = new Point(8, 5);
            this._filter.Size = new Size(this.WideFilterWidth, 16);
            this._filter.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            this._filter.TextBox.GotFocus += new System.EventHandler(TextBoxGotFocus);
            this._filter.TextBox.LostFocus += new System.EventHandler(TextBoxLostFocus);
            this._filter.TextBox.TextChanged += new EventHandler(TextBoxTextChanged);

            //
            // element "_leftBorder"
            //
            this._leftBorder.Location = new Point(0, 0);
            this._leftBorder.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            this._leftBorder.BackColor = Color.White;
            this._leftBorder.Opacity = 255;

            //
            // element "_rightBorder"
            //
            this._rightBorder.Location = new Point(this.Width - 8, 0);
            this._rightBorder.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this._rightBorder.BackColor = Color.White;
            this._rightBorder.Opacity = 255;

            //
            // element "_topBorder"
            //
            this._topBorder.Width = this.Width - 16;
            this._topBorder.Location = new Point(8, 0);
            this._topBorder.Stretch = true;
            this._topBorder.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            this._topBorder.BackColor = Color.White;
            this._topBorder.Opacity = 255;

            //
            // element "_bottomBorder"
            //
            this._bottomBorder.Width = this.Width - 16;
            this._bottomBorder.Location = new Point(8, 23);
            this._bottomBorder.Stretch = true;
            this._bottomBorder.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            this._bottomBorder.BackColor = Color.White;
            this._bottomBorder.Opacity = 255;

            // add to canvas
            this.Canvas.Children.Add(_backgroundGradient);
            this.Canvas.Children.Add(_centerImage);
            this.Canvas.Children.Add(_leftBorder);
            this.Canvas.Children.Add(_rightBorder);
            this.Canvas.Children.Add(_topBorder);
            this.Canvas.Children.Add(_bottomBorder);
            this.Canvas.Children.Add(_clear);
            this.Canvas.Children.Add(_filter);
            

            this.Canvas.RecalcDPIScaling();
            this.ResumeLayout(false);

            this._topBorder.AlphaChannelImage = MasterForm.SkinManager.GetImage("TopFilterBorder");
            this._topBorder.Width = this.Width - UISettings.CalcPix(16);
            this._bottomBorder.AlphaChannelImage = MasterForm.SkinManager.GetImage("BottomFilterBorder");
            this._bottomBorder.Width = this.Width - UISettings.CalcPix(16);
            this._backgroundGradient.AlphaChannelImage = PrepareBackground();
            this._backgroundGradient.Size = this.Size;
        }

        #endregion

        private int DefaultFilterWidth;
        private int WideFilterWidth;

        private GraphicsImage _backgroundGradient;
        private GraphicsImage _centerImage;
        private GraphicsImage _leftBorder;
        private GraphicsImage _rightBorder;
        private GraphicsImage _topBorder;
        private GraphicsImage _bottomBorder;
        public UITextControl _filter;
        private UIButton _clear;

        public String FilterText
        {
            get { return _filter.Text; }
            set { _filter.Text = value; }
        }

        public InputPanel inputPanel;
    }
}
