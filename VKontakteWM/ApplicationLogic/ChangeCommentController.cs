using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Forms;
using System.Text;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    class ChangeCommentController : Controller
    {
        #region Constructors

        public ChangeCommentController()
            : base(new ChangeCommentView())
        {
            Name = "ChangeCommentController";
        }

        #endregion

        #region Controller implementations

        public override void Activate()
        {
            view.Activate();
        }

        public override void Deactivate()
        {
            view.Deactivate();
        }

        protected override void OnViewStateChanged(string key)
        {
            #region SetComment

            if (key == "SetComment")
            {
                StringBuilder newStringBuilder = new StringBuilder();

                string comment = (string)view.ViewData["CurrentComment"];

                comment = comment.Replace("\r", string.Empty);

                // пилим по переносам строки...
                string[] lines = comment.Split('\n');

                foreach (string line in lines)
                {
                    newStringBuilder.Append(line);
                    newStringBuilder.Append(" ");
                }

                comment = newStringBuilder.ToString();

                if (!string.IsNullOrEmpty(comment))
                {
                    comment = comment.Remove(comment.Length - 1, 1);
                }

                ViewData["CurrentComment"] = comment;

                MasterForm.Navigate<UploadPhotoController>("Comment", (string)ViewData["CurrentComment"]);
            }

            #endregion

            #region Cancel

            if (key == "Cancel")
            {
                MasterForm.Navigate<UploadPhotoController>();
            }

            #endregion
        }

        protected override void OnInitialize(params object[] parameters)
        {
            if ((parameters != null) && (parameters.Length > 0))
            {
                string param = parameters[0] as string;

                if ((param != null))
                {
                    ViewData["CurrentComment"] = param;

                    view.UpdateView("SetCurrentComment");
                }
            }

            base.OnInitialize(parameters);
        }

        #endregion
    }
}
