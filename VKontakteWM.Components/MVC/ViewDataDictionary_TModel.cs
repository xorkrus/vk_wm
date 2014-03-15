// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

namespace Galssoft.VKontakteWM.Components.MVC
{
    /// <summary>
    /// A generic version of the ViewDataDictionary class
    /// </summary>
    /// <typeparam name="TModel">The type of the model data.</typeparam>
    public class ViewDataDictionary<TModel> : ViewDataDictionary where TModel : class 
    {
       
        public ViewDataDictionary() :
            base() {
            }

        public ViewDataDictionary(TModel model) :
            base(model) {
            }

        public ViewDataDictionary(ViewDataDictionary viewDataDictionary) :
            base(viewDataDictionary) {
            }

        public new TModel Model {
            get {
                return (TModel)base.Model;
            }
            set {
                SetModel(value);
            }
        }

        protected override void SetModel(object value) {
            TModel model = value as TModel;
            base.SetModel(value);
        }
    }
}