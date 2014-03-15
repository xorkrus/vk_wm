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
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.UI.Transitions;
using Microsoft.WindowsCE.Forms;

namespace Galssoft.VKontakteWM.Components.UI
{
    public static class ViewManager
    {
        private static FormBase _masterForm;
        private static IView _currentView;
        private static InputPanel _inputPanel;
        private static MainMenu _prevMenu;

        public static void Initialize(FormBase masterForm)
        {
            _masterForm = masterForm;
            _inputPanel = new InputPanel();

            ViewTransitionManager.Initialize(masterForm.TransitionControl);
        }

        #region Public Properties

        /// <summary>
        /// The Current View
        /// </summary>
        public static IView CurrentView
        {
            get { return _currentView; }
        }

        #endregion

        #region Methods
        
        public static void ShowMenu(MainMenu menu)
        {
            _prevMenu = _masterForm.Menu;
            _masterForm.Menu = menu;
        }

        public static void HideMenu()
        {
            _masterForm.Menu = _prevMenu;
            _prevMenu = null;
        }

        /// <summary>
        /// Show view onto the screen
        /// </summary>
        /// <param name="view">View to activate</param>
        public static void ActivateView(IView view)
        {
            ActivateView(view, CalculateTransitionType(_currentView, view));
            
        }

        /// <summary>
        /// Show view onto the screen
        /// </summary>
        /// <param name="view">View to activate</param>
        /// <param name="type">Type of a transition</param>
        public static void ActivateView(IView view, TransitionType type)
        {
            if (_masterForm.InvokeRequired)
            {
                _masterForm.Invoke(new Action<IView>(ActivateView), new object[] { view, type });
                return;
            }

            var from = _currentView != null ? _currentView : view;
            var to = view;

            AddViewToForm(to);

            ViewTransitionManager.Execute(from, to, type);
        }

        /// <summary>
        /// Add visual part of the IView to the MasterForm 
        /// </summary>
        /// <param name="view">IView</param>
        /// <exception cref="NotSupportedException">Object for IView must be based on System.Windows.Forms.Control. In other cases will be NotSupportedException exception.</exception>
        private static void AddViewToForm(IView view)
        {
            if (!(view is Control))
                throw new NotSupportedException("IView must be based on System.Windows.Forms.Control class or any inherited.");
		
            Control c = (Control)view;
            c.Visible = false;
            c.ClientSize = new Size(_masterForm.ClientSize.Width, _masterForm.ClientSize.Height);
            c.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                       | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom;
            c.Location = new Point(0, 0);

            if (_masterForm.Controls.Contains(c))
                c.BringToFront();
            else
                _masterForm.Controls.Add(c);

            if (_currentView != null)
                ((ViewBase)_currentView).SuspendLayout();

            _masterForm.Menu = view.ViewMenu;
        }

        /// <summary>
        /// Remove visual part of the IView from the MasterForm 
        /// </summary>
        /// <param name="view">IView</param>
        /// <exception cref="NotSupportedException">Object for IView must be based on System.Windows.Forms.Control. In other cases will be NotSupportedException exception.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If MasterForm does not contain IView then there will be ArgumentOutOfRangeException exception.</exception>
        private static void RemoveViewFromForm(IView view)
        {
            if (!(view is Control))
                throw new NotSupportedException("IView must be based on System.Windows.Forms.Control class or any inherited.");
		
            Control c = (Control)view;
            if (_masterForm.Controls.Contains(c))
                _masterForm.Controls.Remove(c);
            else
                throw new ArgumentOutOfRangeException("MsaterForm does not contain specified IView.");
        }

        /// <summary>
        /// Calculate default transition according to view properties
        /// </summary>
        /// <param name="from">Current view</param>
        /// <param name="to">View to display</param>
        /// <returns>Calculated trasition type or Basic</returns>
        private static TransitionType CalculateTransitionType(IView from, IView to)
        {
            return to.GetTransition(from);
        }

        /// <summary>
        /// Change the form's caption 
        /// </summary>
        public static void RefreshTitle()
        {
            _masterForm.Text = CurrentView.Title;
        }

        #endregion

        #region Event Handlers

        internal static void OnViewTransitionCompleted(IView fromView, IView toView)
        {
            _currentView = toView;

            if (fromView != null)
                fromView.OnAfterDeactivate();

            toView.OnAfterActivate();

            if (fromView != null)
                ((ViewBase) fromView).ResumeLayout();

            RefreshTitle();
        }

        #endregion

    }
}