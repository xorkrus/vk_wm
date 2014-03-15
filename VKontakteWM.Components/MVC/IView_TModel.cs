namespace Galssoft.VKontakteWM.Components.MVC
{
    /// <summary>
    /// Generic verion of the IView interface
    /// </summary>
    /// <typeparam name="TModel">Strongly typed model data</typeparam>
    public interface IView<TModel> : IView where TModel : class
    {                      
        // Generic version of the View data 
        new ViewDataDictionary<TModel> ViewData { get; set; }
        /// <summary>
        /// Strongly typed model object
        /// </summary>
        new TModel Model { get; set; }

    }
}