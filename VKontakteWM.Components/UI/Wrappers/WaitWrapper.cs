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
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;

namespace Galssoft.VKontakteWM.Components.UI.Wrappers
{
    public class WaitWrapper : IDisposable
    {
        #region Fields

        /// <summary>
        /// Timer to show waiting cursor
        /// </summary>
        private Timer _showCursorTimer = null;

        /// <summary>
        /// Prevoius user's cursor before changing to waiting cursor
        /// </summary>
        private Cursor _current;

        #endregion

        #region Constructors

        public WaitWrapper()
            : this(false)
        { }

        /// <param name="showWithDelay">Show waiting cursor with delay in 0.5 sec</param>
        public WaitWrapper(bool showWithDelay)
        {
            _current = Cursor.Current;

            if (showWithDelay)
            {
                _showCursorTimer = new Timer();
                _showCursorTimer.Interval = 500;
                _showCursorTimer.Tick += new EventHandler(ShowCursorTimer_Tick);
                _showCursorTimer.Enabled = true;
            }
            else
            {
                Cursor.Current = Cursors.WaitCursor;
            }
        }

        void ShowCursorTimer_Tick(object sender, EventArgs e)
        {
            _showCursorTimer.Enabled = false;
            _current = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
        }

        #endregion

        #region Methods
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                if (_showCursorTimer != null)
                {
                    _showCursorTimer.Enabled = false;
                    _showCursorTimer.Dispose();
                    _showCursorTimer = null;
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteLogEntry(e.ToString());
            }
            Cursor.Current = _current;
        }

        #endregion

    }
}