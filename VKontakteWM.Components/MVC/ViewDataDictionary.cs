using System.Collections.Generic;
using System;

namespace Galssoft.VKontakteWM.Components.MVC
{
    /// <summary>
    /// The dictionary to store the model data.
    /// </summary>
    public class ViewDataDictionary : Dictionary<string, object>
    {
        private object _model;

        #region constructors

        /// <summary>
        /// Initializes a new instance of the ViewDataDictionary class.
        /// </summary>
        public ViewDataDictionary()
            : base(StringComparer.OrdinalIgnoreCase)
        {           
        }
        
        /// <summary>
        /// Initializes a new instance of the ViewDataDictionary class.
        /// </summary>
        /// <param name="model">An instance of the model data class.</param>
        public ViewDataDictionary(object model)
            : base(StringComparer.OrdinalIgnoreCase)
        {
            Model = model;
        }

        /// <summary>
        /// Initializes a new instance of the ViewDataDictionary class.
        /// </summary>
        /// <param name="viewDataDictionary">An instance of the existing ViewDataDictionary.</param>
        public ViewDataDictionary(ViewDataDictionary viewDataDictionary)
            : base(viewDataDictionary, StringComparer.OrdinalIgnoreCase)
        {
            Model = viewDataDictionary.Model;
        }

        #endregion

       
        /// <summary>
        /// Gets or sets the model data.
        /// </summary>
        public object Model
        {
            get
            {
                return _model;
            }
            set
            {
                SetModel(value);
            }
        }       

        protected virtual void SetModel(object value)
        {
            _model = value;
        }

        // If a user tries to index into the dictionary using a ViewDataDictionary reference, we don't throw an exception if
        // the key doesn't exist.  If he uses an IDictionary or Dictionary reference, its implementation throws.
        public new object this[string key]
        {
            get
            {
                object value;
                TryGetValue(key, out value);
                return value;
            }
            set
            {
                base[key] = value;
            }
        }                 
    }
}