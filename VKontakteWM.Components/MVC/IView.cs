using System.Windows.Forms;
using System.Drawing;
using Galssoft.VKontakteWM.Components.UI.Transitions;

namespace Galssoft.VKontakteWM.Components.MVC
{
    /// <summary>
    /// Non-generic IView interface
    /// </summary>
    public interface IView
    {
        #region Common form's methods
		
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        string Title { get; }

        /// <summary>
        /// Gets the view menu.
        /// </summary>
        /// <value>The view menu.</value>
        MainMenu ViewMenu { get; }

        /// <summary>
        /// Shows or Hides the view
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// Sets the focus to the view
        /// </summary>
        bool Focus();

        ///// <summary>
        ///// Restart Positioning to the view
        ///// </summary>
        //void RestartPosition();

        /// <summary>
        /// Initialize part of construction
        /// </summary>
        void Load();

        /// <summary>
        /// Awake from sleep and reinitialize visual components	to display on the screen
        /// </summary>
        void Activate();

        /// <summary>
        /// Go to sleep and release resources
        /// </summary>
        void Deactivate();

        /// <summary>
        /// Notifies the view that it will be activated
        /// </summary>
        void OnBeforeActivate();

        /// <summary>
        /// Notifies the view that it was deactivated
        /// </summary>
        void OnAfterDeactivate();

        /// <summary>
        /// Notifies that the view was activated
        /// </summary>
        void OnAfterActivate();

        /// <summary>
        /// Gets a screenshot of the view for transitions
        /// </summary>
        /// <returns></returns>
        Bitmap CreateScreenShot();

        /// <summary>
        /// Gets a transition type for transitions
        /// </summary>
        /// <returns></returns>
        TransitionType GetTransition(IView from);

        #endregion

        #region MVC part

        /// <summary>
        /// Event indicating that something has been changed in the View
        /// </summary>
        event StateChangedEventHandler ViewStateChanged;
        
        /// <summary>
        /// The View data
        /// </summary>
        ViewDataDictionary ViewData { get; set; }
        
        /// <summary>
        /// Model
        /// </summary>
        object Model { get; set; }
        
        /// <summary>
        /// Method to tell the View to update itself
        /// </summary>
        /// <param name="key"></param>
        void UpdateView(string key);

        #endregion
    }
}