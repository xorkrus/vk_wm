// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

namespace Galssoft.VKontakteWM.Components.MVC
{
    /// <summary>
    /// The generic version of the Controller class.
    /// </summary>
    /// <typeparam name="TModel">The type of the model data.</typeparam>
    public abstract class Controller<TModel> : Controller where TModel : class
    {
        #region fields

        protected new IView<TModel> view;

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of the Controller.
        /// </summary>
        /// <param name="view">An instance of the view.</param>
        public Controller(IView<TModel> view)
            : base(view)
        {
            this.view = view;
            //мое художество
            //исходник искать в public static class ViewFormExtension
            //this.view.GetViewData<TModel>();
            view.ViewData = getViewData(view);
        }

        public new ViewDataDictionary<TModel> ViewData
        {
            get { return view.ViewData; }
        }

        private ViewDataDictionary<TModel> getViewData<TModel>(IView<TModel> view) where TModel : class
        {
            if (view.ViewData == null)
            {
                view.ViewData = new ViewDataDictionary<TModel>();
            }
            return view.ViewData;
        }

        #endregion

        #region overrides

        /// <summary>
        /// Override of the OnInitalize from the Controller class.
        /// </summary>
        /// <param name="parameters">An array of the parameters</param>
        protected override void OnInitialize(params object[] parameters)
        {
            base.OnInitialize(parameters);
        }

        #endregion
    }
}