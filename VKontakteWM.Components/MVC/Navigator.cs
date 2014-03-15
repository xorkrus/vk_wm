namespace Galssoft.VKontakteWM.Components.MVC
{
    /// <summary>
    /// Navigator class implementation
    /// </summary>
    public class Navigator : INavigator
    {
        #region fields

        internal IControllerProvider controllerProvider;
        private NavigationHistory history;
        private static Navigator navigator;

        #endregion


        #region constructors

        //TODO: Why you use this Public constructors in case of the Singleton 

        /// <summary>
        /// Initializes Navigator instance
        /// </summary>
        public Navigator()
        {
            this.controllerProvider = new DefaultControllerProvider();
            this.history = new NavigationHistory();
        }

        /// <summary>
        /// Initializes Navigator instance
        /// </summary>
        /// <param name="controllerProvider">Instance of the IControllerProvider</param>
        public Navigator(IControllerProvider controllerProvider)
        {
            this.controllerProvider = controllerProvider;
            this.history = new NavigationHistory();
        }

        #endregion

        #region static methods

        /// <summary>
        /// Gets a singleton instance of the Navigator.
        /// </summary>
        public static Navigator Current
        {
            get
            {
                if (navigator == null)
                {
                    navigator = new Navigator();
                }

                return navigator;
            }
        }

        /// <summary>
        /// Sets the IControllerProvider to use.
        /// </summary>
        /// <param name="controllerProvider"></param>
        public static void SetControllerProvider(IControllerProvider controllerProvider)
        {
            if (navigator == null)
            {
                navigator = new Navigator(controllerProvider);
            }
        }

        #endregion

        #region helper methods

        private void ShowHide(Controller current, Controller next)
        {
            next.Activate();
            if (current != null && current != next)
            {
                current.Deactivate();
            }
        }

        #endregion

        #region INavigator Members

        /// <summary>
        /// Navigates to the controller
        /// </summary>
        /// <typeparam name="T">A type of the controller to navigate</typeparam>
        public void Navigate<T>()
        {
            Controller current = controllerProvider.GetController<T>();
            current.Initialize();
            ShowHide(history.GetCurrent(), current);
            history.Add(current);
        }

        /// <summary>
        /// Navigates to the controller
        /// </summary>
        /// <typeparam name="T">A type of the controller to navigate</typeparam>
        /// <param name="parameters">Parameters to pass to the controller</param>
        public void Navigate<T>(params object[] parameters)
        {
            Controller current = controllerProvider.GetController<T>();
            current.Initialize(parameters);
            ShowHide(history.GetCurrent(), current);
            history.Add(current);
        }

        /// <summary>
        /// Navigates to the controller
        /// </summary>
        /// <typeparam name="T">A type of the controller to navigate</typeparam>
        /// <param name="view">The IView to assign to the controller</param>
        public void Navigate<T>(IView view)
        {
            Controller current = controllerProvider.GetController<T>(view);
            current.Initialize();
            ShowHide(current, history.GetCurrent());
            history.Add(current);
        }

        /// <summary>
        /// Navigates to the controller
        /// </summary>
        /// <typeparam name="T">A type of the controller to navigate</typeparam>
        /// <param name="view">The IView to assign to the controller</param>
        /// <param name="parameters">Parameters to pass to the controller</param>
        public void Navigate<T>(IView view, params object[] parameters)
        {
            Controller current = controllerProvider.GetController<T>(view);
            current.Initialize(parameters);
            ShowHide(history.GetCurrent(), current);
            history.Add(current);
        }

        ///// <summary>
        ///// Navigates to the controller.
        ///// </summary>
        ///// <param name="name">The name of the controller.</param>
        //public void Navigate(string name)
        //{
        //    Controller controller = controllerProvider.GetController(name);
        //    controller.Initialize();
        //    Controller currentController = history.GetCurrent();

        //    controller.View.Show();
        //    if (currentController != null)
        //    {
        //        currentController.View.Hide();
        //    }

        //    history.Add(controller);
        //}

        /// <summary>
        /// Navigates to the controller.
        /// </summary>
        /// <param name="name">The name of the controller.</param>
        /// <param name="parameters">Parameters to pass to the controller</param>
        public void Navigate(string name, params object[] parameters)
        {
            Controller controller = controllerProvider.GetController(name);
            controller.Initialize(parameters);
            Controller currentController = history.GetCurrent();
			
            controller.Activate();
            if (currentController != null)
            {
                currentController.Deactivate();
            }

            history.Add(controller);
        }

        //public void Navigate(Controller controller)
        //{
        //    string name = controller.Name;
        //    controllerProvider.RegisterController(controller);

        //    Controller currentController = history.GetCurrent();

        //    controller.View.Show();
        //    if (currentController != null)
        //    {
        //        currentController.View.Hide();
        //    }

        //    history.Add(controller);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="parameters"></param>
        public void Navigate(Controller controller, params object[] parameters)
        {
            controllerProvider.RegisterController(controller);
            Controller current = controllerProvider.GetController(controller.GetType());
            current.Initialize(parameters);
            ShowHide(history.GetCurrent(), current);
            history.Add(current);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        public void Register(Controller controller)
        {
            // Register with Controller provider.
            controllerProvider.RegisterController(controller);
            history.Add(controller);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //public System.Windows.Forms.DialogResult NavigateDialog(string name)
        //{
        //    Controller controller = controllerProvider.GetController(name);
        //    Controller currentController = history.GetCurrent();
        //    controller.Initialize();

        //    if (currentController != null)
        //    {
        //        currentController.View.Hide();
        //    }

        //    history.Add(controller);
        //    history.GoBack();
        //    DialogResult result = controller.View.ShowDialog();
        //    return result;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="controller"></param>
        ///// <returns></returns>
        //public System.Windows.Forms.DialogResult NavigateDialog(Controller controller)
        //{
        //    string name = controller.Name;
        //    controllerProvider.RegisterController(controller);

        //    Controller currentController = history.GetCurrent();

        //    if (currentController != null)
        //    {
        //        currentController.View.Hide();
        //    }

        //    history.Add(controller);

        //    return controller.View.ShowDialog();
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="controller"></param>
        ///// <param name="parameters"></param>
        ///// <returns></returns>
        //public System.Windows.Forms.DialogResult NavigateDialog(Controller controller, params object[] parameters)
        //{
        //    string name = controller.Name;
        //    controllerProvider.RegisterController(controller);

        //    Controller currentController = history.GetCurrent();

        //    controller.Initialize(parameters);
        //    if (currentController != null)
        //    {
        //        currentController.View.Hide();
        //    }

        //    history.Add(controller);

        //    return controller.View.ShowDialog();
        //}

        /// <summary>
        /// Moves back to the controller in the navigation history
        /// </summary>
        public void GoBack()
        {
            Controller currentController = history.GetCurrent();
            history.GoBack();
            Controller controller = history.GetCurrent();
            controller.Activate();
            currentController.Deactivate();
        }

        /// <summary>
        /// Moves forward to the controller in the navigation history
        /// </summary>
        public void GoForward()
        {
            Controller currentController = history.GetCurrent();
            history.GoForward();
            history.GetCurrent().Activate();
            currentController.Deactivate();
        }

        /// <summary>
        /// Identifies if moving back is possible.
        /// </summary>
        /// <returns></returns>
        public bool CanGoBack()
        {
            return history.CanGoBack();
        }

        /// <summary>
        /// Identifies if moving forward is possible.
        /// </summary>
        /// <returns></returns>
        public bool CanGoForward()
        {
            return history.CanGoForward();
        }

        #endregion
    }
}