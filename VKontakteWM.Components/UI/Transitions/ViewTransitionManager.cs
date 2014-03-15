/*	 This file was created by GALS Soft	company
 *	 http://www.galssoft.com
 *	 
 *	 Project name is Odnoklassniki Mobile
 *	 Platform is .Net Compact Framework 2.0 for Windows Mobile 5.0+ 
 * 
 *	 Copyright (c) 2009-2010 GALS Soft
 */

using Galssoft.VKontakteWM.Components.MVC;

namespace Galssoft.VKontakteWM.Components.UI.Transitions
{
    internal static class ViewTransitionManager
    {
        #region Constructors

        static ViewTransitionManager()
        {
            _slideTransition = new ViewTransitionSlide();
            _basicTransition = new ViewTransitionBasic();
            _flipTransition = new ViewTransitionFlip();
        }

        internal static void Initialize(TransitionControl transitionControl)
        {
            _masterTransitionControl = transitionControl;
        }

        #endregion

        #region Private vars

        private static TransitionControl _masterTransitionControl;

        static IView _fromView, _toView;

        static ViewTransitionSlide _slideTransition;
        static ViewTransitionBasic _basicTransition;
        static ViewTransitionFlip _flipTransition;

        static IViewTransition _currentTransition;

        #endregion

        #region Public properties

        public static IView ToView
        {
            get { return _toView; }
        }

        public static IView FromView
        {
            get { return _fromView; }
        }

        #endregion

        internal static void Execute(IView from, IView to, TransitionType type)
        {
            _fromView = from;
            _toView = to;

            switch (type)
            {
                case TransitionType.Basic:
                    _currentTransition = _basicTransition;
                    break;

                case TransitionType.SlideLeft:
                    _slideTransition.SlideDirection = TransitionType.SlideLeft;
                    _currentTransition = _slideTransition;
                    break;

                case TransitionType.SlideRight:
                    _slideTransition.SlideDirection = TransitionType.SlideRight;
                    _currentTransition = _slideTransition;
                    break;

                case TransitionType.FlipLeft:
                    _flipTransition.FlipDirection = TransitionType.FlipLeft;
                    _currentTransition = _flipTransition;
                    break;

                case TransitionType.FlipRight:
                    _flipTransition.FlipDirection = TransitionType.FlipRight;
                    _currentTransition = _flipTransition;
                    break;
            }

            if (!_currentTransition.IsTransitionAvailable())
                _currentTransition = _basicTransition;

            _currentTransition.Execute();
        }

        internal static TransitionControl TransitionCanvas
        {
            get
            {
                return _masterTransitionControl;
            }
        }

        internal static void OnTransitionStart()
        {
            _toView.OnBeforeActivate();

            _fromView.Visible = false;
            _masterTransitionControl.Visible = true;
        }

        internal static void OnTransitionEnd()
        {
            _masterTransitionControl.Visible = false;
            _masterTransitionControl.ReleaseOffscreenBitmap();

            _toView.Visible = true;
            _toView.Focus();

            ViewManager.OnViewTransitionCompleted(_fromView, _toView);
        }
    }
}