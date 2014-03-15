/*	 This file was created by GALS Soft	company
 *	 http://www.galssoft.com
 *	 
 *	 Project name is Odnoklassniki Mobile
 *	 Platform is .Net Compact Framework 2.0 for Windows Mobile 5.0+ 
 * 
 *	 Copyright (c) 2009-2010 GALS Soft
 */

using System;
using System.Runtime.InteropServices;
using System.Drawing;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.GDI;

namespace Galssoft.VKontakteWM.Components.Common.Configuration
{
    /// <summary>
    /// Set of settings for fonts and lists according to current screen DPI
    /// </summary>
    public class UISettings
    {
        /// <summary>
        /// Current settings for fonts and lists
        /// </summary>
        public static UISettings Global
        {
            get { return _settingsGlobal; }
        }
        private static UISettings _settingsGlobal = null;

        #region External Win32 functions

        private const int LOGPIXELSX = 88;
        //private const int LOGPIXELSY = 90;

        [DllImport("coredll")]
        extern static IntPtr GetDC(IntPtr hwnd);

        [DllImport("coredll")]
        extern static int GetDeviceCaps(IntPtr hdc, int index);

        #endregion

        #region Constructors and destructor

        /// <summary>
        /// Initialize settings for fonts and lists according to current DPI
        /// </summary>
        public static void InitSettings() // Form form
        {
            //Чем плох Gdi.GetScreenDpi(IntPtr hWnd) ???
            IntPtr hdc = GetDC(IntPtr.Zero);
            _screenDPI = GetDeviceCaps(hdc, LOGPIXELSX);

            _settingsGlobal = GetDefaultSettingsForPlatform(_screenDPI);
            _settingsGlobal.AdjustScreenValuesForCurrentDPI();
        }

        private UISettings()
        { }

        #endregion

        #region Private vars

        private static int _screenDPI = 0;

        #endregion

        #region Default sizes

        //Sizes
        private const int LabelFontSize = 11;

        //Fonts
        private const string DefaultFontName = "Calibri";

        //Colors
        public readonly Color TextColor = SystemColors.ControlText;
        public readonly Color SelectedTextColor = SystemColors.ActiveCaptionText;

        #endregion

        #region Calculated sizes in pixels

        public int LabelFontSizePx { get; protected set; }

        #endregion

        #region Public properties

        public readonly bool UseClearType = true;

        public static int ScreenDPI
        {
            get { return _screenDPI; }
        }

        // Fonts

        /// <summary>
        /// Default GDI font for text labels and textboxes
        /// </summary>
        public FontGdi TextFontGdi
        {
            get { return FontCache.CreateFont(DefaultFontName, LabelFontSize, FontStyle.Regular, UseClearType); }
        }

        /// <summary>
        /// Default font for text labels and textboxes
        /// </summary>
        public Font TextFont
        {
            get { return Font.FromHfont((IntPtr)TextFontGdi); }
        }

        #endregion

        #region Calculate settings for current DPI

        public static int CalcPix(int points)
        {
            if (ScreenDPI == 96)
                return points;
            else
                return Win32.MulDivEx(points, ScreenDPI, 96);
        }

        protected void AdjustScreenValuesForCurrentDPI()
        {
            //LabelFontSizePx = CalcPix(LabelFontSize);
        }

        public static UISettings GetDefaultSettingsForPlatform(int screenDPI)
        {
            UISettings retVal = new UISettings();

            retVal.AdjustScreenValuesForCurrentDPI();
            return retVal;
        }

        #endregion
    }
}