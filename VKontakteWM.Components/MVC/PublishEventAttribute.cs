using System;

namespace Galssoft.VKontakteWM.Components.MVC
{
    /// <summary>
    /// The attribute used to decorate events that the view or controller wants to publish.
    /// Using only 'void OnSomeDoing(object sender, EventArgs e)' type function.
    /// </summary>
    public class PublishEventAttribute : Attribute
    {
        private string targetName;

        /// <summary>
        /// Initializes a new instance of the PublishEventAttribute
        /// </summary>
        /// <param name="targetName"></param>
        public PublishEventAttribute(string targetName)
        {
            this.targetName = targetName;
        }

        /// <summary>
        /// Gets or sets the event handler name.
        /// </summary>
        public string TargetName
        {
            get
            {
                return targetName;
            }
            set
            {
                targetName = value;
            }
        }
    }
}