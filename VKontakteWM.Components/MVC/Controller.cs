using System.Collections.Generic;
using System.Reflection;
using System;

namespace Galssoft.VKontakteWM.Components.MVC
{
    /// <summary>
    /// Represents the method that will handle an event of the state change in the model.
    /// </summary>
    /// <param name="key">The key identifier.</param>
    public delegate void StateChangedEventHandler(string key);

    /// <summary>
    /// Represents the Controller class.
    /// </summary>
    public abstract class Controller
    {
        #region fields

        protected IView view;
        private MethodInfo[] controllerMethods;
        private MethodInfo[] viewMethods;
        private string name;        

        #endregion

        #region constructors       

        /// <summary>
        /// Initializes a new instance of the System.Moblie.Mvc.Controller class with the IView.
        /// </summary>
        /// <param name="view">The instance of the view</param>
        public Controller(IView view)
        {
            this.name = this.GetType().Name;
            this.view = view;
            this.view.ViewStateChanged += new StateChangedEventHandler(view_StateChanged);
            this.controllerMethods = this.GetMethods(this.GetType());
            this.viewMethods = this.GetMethods(view.GetType());
            this.HookEvents(view, this, this.controllerMethods);
            this.HookEvents(this, view, this.viewMethods);
        }

        #endregion

        /// <summary>
        /// Gets an instance of the Navigator.
        /// </summary>
        public Navigator Navigator
        {
            get
            {
                return Navigator.Current;
            }
        }

        #region public and virtual methods

        public virtual void Activate()
        {
			
        }

        public virtual void Deactivate()
        {
        }

        /// <summary>
        /// Initializes the Controller with the parameters
        /// </summary>
        /// <param name="parameters">An array of parameters</param>
        public void Initialize(params object[] parameters)
        {
            OnInitialize(parameters);
        }

        /// <summary>
        /// Notifies the overriding class of the Initialize method been called.
        /// </summary>
        /// <param name="parameters">An array of parameters</param>
        protected virtual void OnInitialize(params object[] parameters)
        {

        }

        /// <summary>
        /// Assignes the ViewDataDictionary to the View.
        /// </summary>
        /// <param name="viewData">The instance of the ViewDataDictionary.</param>
        protected virtual void SetViewData(ViewDataDictionary viewData)
        {
            this.view.ViewData = viewData;
        }

        /// <summary>
        /// Notifies the overriding class of the state change in the View.
        /// </summary>
        /// <param name="key">The key that is changed.</param>
        protected virtual void OnViewStateChanged(string key)
        {

        }

        #endregion

        #region event handlers

        void view_StateChanged(string key)
        {
            OnViewStateChanged(key);
        }

        #endregion

        #region properties

        public ViewDataDictionary ViewData
        {
            get
            {
                return view.ViewData;
            }
            set
            {
                if (view != null)
                {
                    view.ViewData = value;
                }

            }
        }

        public object Model
        {
            get
            {
                return view.ViewData.Model;
            }
            set
            {
                view.ViewData.Model = value;
            }
        }

        /// <summary>
        /// Gets or sets the Controller name.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        /// <summary>
        /// Gets the view instance.
        /// </summary>
        public IView View
        {
            get
            {
                return this.view;
            }
            set
            {
                if (this.view != null)
                {
                    this.view.ViewStateChanged -= new StateChangedEventHandler(view_StateChanged);
                }

                this.view = value;
                if (this.view != null)
                {
                    this.view.ViewStateChanged += new StateChangedEventHandler(view_StateChanged);
                    this.viewMethods = this.GetMethods(view.GetType());
                    this.HookEvents(view, this, this.controllerMethods);
                    this.HookEvents(this, view, this.viewMethods);
                }
            }
        }

        #endregion


        #region helper methods

        // Used to hook into published events by the view or the controller
        private void HookEvents(object source, object target, MethodInfo[] tagetMethods)
        {
            EventInfo[] events = source.GetType().GetEvents(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            foreach (EventInfo eventInfo in events)
            {
                object[] attribs = eventInfo.GetCustomAttributes(false);
                if (attribs.Length > 0 && attribs[0] is PublishEventAttribute)
                {
                    PublishEventAttribute eventAttrib = attribs[0] as PublishEventAttribute;

                    //Вместо
                    //MethodInfo method = tagetMethods.FirstOrDefault<MethodInfo>(m =>  m.Name == eventAttrib.TargetName);
                    MethodInfo method = null;
                    foreach (MethodInfo info in tagetMethods)
                    {
                        if (info.Name == eventAttrib.TargetName)
                        {
                            method = info;
                            break;
                        }
                    }

                    if (method != null)
                    {
                        EventHandler ev = delegate(object sender, EventArgs e) { method.Invoke(target, new object[] { sender, e }); };
                        eventInfo.AddEventHandler(source, ev);
                    }
                    
                    //if (method != null)
                    //{
                    //    var deleg = Delegate.CreateDelegate(eventInfo.EventHandlerType, target, method);
                    //    eventInfo.AddEventHandler(source, deleg);
                    //}
                }
            }
        }

        // Returns all methods from the type
        private MethodInfo[] GetMethods(Type type)
        {
            return type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        private MethodInfo[] GetValidActionMethods(MethodInfo[] allMethods)
        {
            List<MethodInfo> list = new List<MethodInfo>();

            foreach (MethodInfo methodInfo in allMethods)
            {
                object[] attribs = methodInfo.GetCustomAttributes(false);
                if (attribs.Length > 0 && attribs[0] is ActionAttribute)
                {
                    list.Add(methodInfo);
                }
            }
            return list.ToArray();
        }

        #endregion

    }
}