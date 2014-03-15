// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

using System.Windows.Forms;
using System;

namespace Galssoft.VKontakteWM.Components.MVC
{
    public class ViewBase : UserControl
    {
        #region fields

        private ViewDataDictionary _viewData;

        #endregion

        #region constructor

        public ViewBase()
        {
            
        }

        #endregion


        // Raises the ViewStateChanged event
        protected void OnViewStateChanged(string key)
        {
            if (this.ViewStateChanged != null)
            {
                this.ViewStateChanged(key);
            }
        }

        protected virtual void OnUpdateView(string key)
        {

        }

        private void SetViewData(ViewDataDictionary viewData)
        {
            this._viewData = viewData;
        }

        #region IView Members

        /// <summary>
        /// The state change event of the view
        /// </summary>
        public event StateChangedEventHandler ViewStateChanged;

        /// <summary>
        /// Gets or sets the ViewData. Supports assigning the ViewData from a different thread.
        /// </summary>
        public ViewDataDictionary ViewData
        {
            get
            {
                if (_viewData == null)
                {
                    _viewData = new ViewDataDictionary();
                }
                return _viewData;
            }
            set
            {
                if (!this.InvokeRequired)
                {
                    _viewData = value;
                }
                else
                {
                    this.Invoke(new Action<ViewDataDictionary>(SetViewData), value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Model.
        /// </summary>
        public object Model
        {
            get
            {
                return _viewData.Model;
            }
            set
            {
                _viewData.Model = value;
            }
        }

        /// <summary>
        /// Used to notify the view of the updated model data.
        /// </summary>
        /// <param name="key">The key to indentify the data.</param>
        public void UpdateView(string key)
        {
            if (!this.InvokeRequired)
            {
                this.OnUpdateView(key);
            }
            else
            {
                this.Invoke(new Action<string>(OnUpdateView), key);
            }
        }

        #endregion
    }
}