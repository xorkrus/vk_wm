// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System;

namespace Galssoft.VKontakteWM.Components.MVC
{
    /// <summary>
    /// Class to manage navigation between the views and controllers. 
    /// Maintains cache of the loaded controllers
    /// </summary>
    public static class NavigationService
    {
        #region fields

        private static Dictionary<string, Controller> controllerCache;
        private static NavigationHistory history;

        #endregion

        static NavigationService()
        {
            history = new NavigationHistory();
            controllerCache = new Dictionary<string, Controller>();
        }


        /// <summary>
        /// Gets the controllers cache.
        /// </summary>
        public static Dictionary<string, Controller> Controllers
        {
            get
            {
                return controllerCache;
            }
        }


        /// <summary>
        /// Adds controller to the cache
        /// </summary>
        /// <param name="controller">Controller instance.</param>
        public static void Add(Controller controller)
        {
            if (!controllerCache.ContainsKey(controller.Name))
            {
                controllerCache.Add(controller.Name, controller);
            }
            else
            {
                throw new ArgumentException("Controller already exists.");
            }
        }

        /// <summary>
        /// Removes controller from the cache
        /// </summary>
        /// <param name="controller">Controller instance.</param>
        public static void Remove(Controller controller)
        {
            if (controllerCache.ContainsKey(controller.Name))
            {
                controllerCache.Remove(controller.Name);
            }
            else
            {
                throw new ArgumentException("Controller doesn't exist.");
            }
        }


        /// <summary>
        /// Removes controller from the cache
        /// </summary>
        /// <param name="name">Controller name.</param>
        public static void Remove(string name)
        {
            if (controllerCache.ContainsKey(name))
            {
                controllerCache.Remove(name);
            }
            else
            {
                throw new ArgumentException("Controller doesn't exist.");
            }
        }


        ///// <summary>
        ///// Navigates to the controller
        ///// </summary>
        ///// <param name="name">controller name</param>
        //public static void Navigate(string name)
        //{
        //    if (controllerCache.ContainsKey(name))
        //    {
        //        Controller controller = controllerCache[name];

        //        Controller currentController = history.GetCurrent();              

        //        if (currentController != controller)
        //        {
        //            controller.Activate();
        //            if (currentController != null)
        //            {
        //                currentController.Deactivate();
        //            }

        //            history.Add(controller);
        //        }                

        //    }
        //    else
        //    {
        //        throw new MissingMemberException("Controller does not exist");
        //    }
        //}

        /// <summary>
        /// Navigates to the view/controller with the parameters
        /// </summary>
        /// <param name="name">The name of the controller.</param>
        /// <param name="parameters">An array of the parameters</param>
        public static void Navigate(string name, params object[] parameters)
        {
            if (controllerCache.ContainsKey(name))
            {
                Controller controller = controllerCache[name];

                if (parameters != null)
                {
                    controller.Initialize(parameters);
                }
				
                Controller currentController = history.GetCurrent();

                controller.Activate();
                if (currentController != null)
                {
                    currentController.Deactivate();
                }

                history.Add(controller);
            }
            else
            {
                throw new MissingMemberException("Controller does not exist");
            }
        }

        ///// <summary>
        ///// Navigates to the view/controller in a modal fashion.
        ///// </summary>
        ///// <param name="controller">The name of the controller.</param>
        ///// <returns>The DialogResult from the displayed view.</returns>
        //public static DialogResult NavigateFormDialog(Controller controller)
        //{
        //    string name = controller.Name;


        //    if (!controllerCache.ContainsKey(name))
        //    {
        //        controllerCache.Add(name, controller);
        //    }
        //    history.Add(controller);
        //    DialogResult result = controller.View.ShowDialog();
        //    history.GoBack();
        //    return result;
        //}

        //        /// <summary>
        ///// Navigates to the view/controller in a modal fashion for Control realization.
        ///// </summary>
        ///// <param name="controller">The name of the controller.</param>
        ///// <returns>The DialogResult from the displayed view.</returns>
        //public static void NavigateControlDialog(Controller controller)
        //{
        //    string name = controller.Name;


        //    if (!controllerCache.ContainsKey(name))
        //    {
        //        controllerCache.Add(name, controller);
        //    }
        //    history.Add(controller);
        //    controller.View.ShowDialog();
        //}

        ///// <summary>
        ///// Navigates to the controller.
        ///// </summary>
        ///// <param name="controller">An instance of the controller.</param>
        //public static void Navigate(Controller controller)
        //{
        //    string name = controller.Name;

        //    if (!controllerCache.ContainsKey(name))
        //    {
        //        controllerCache.Add(name, controller);
        //    }
        //    else
        //    {
        //        controllerCache.Remove(name);
        //        controllerCache.Add(name, controller);
        //    }

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
        /// <param name="controller">An instance of the contoller.</param>
        /// <param name="parameters">An array of the parameters.</param>
        public static void Navigate(Controller controller, params object[] parameters)
        {
            string name = controller.GetType().Name;

            if (!controllerCache.ContainsKey(name))
            {
                controllerCache.Add(name, controller);
            }
            else
            {
                controllerCache.Remove(name);
                controllerCache.Add(name, controller);
            }

            controller.Initialize(parameters);
            controller.Activate();
            
            Controller currentController = history.GetCurrent();
            if (currentController != null)
            {
                currentController.Deactivate();
            }
            history.Add(controller);
        }

        /// <summary>
        /// Displays the previous view/controller from the cache.
        /// Hides the currently shown view.
        /// </summary>
        public static void GoBack()
        {
            Controller currentController = history.GetCurrent();
            history.GoBack();
            history.GetCurrent().Activate();
            currentController.Deactivate();
        }

        /// <summary>
        /// Displays the next controller from the cache.
        /// Hides the currently shown view.
        /// </summary>
        public static void GoForward()
        {
            Controller currentController = history.GetCurrent();
            history.GoForward();
            history.GetCurrent().Activate();
            currentController.Deactivate();
        }

        /// <summary>
        /// Get the current controller from the cache.
        /// </summary>
        public static Controller Current()
        {
            Controller currentController = history.GetCurrent();
            return currentController;
        }
    }
}