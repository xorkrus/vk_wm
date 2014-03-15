using System;
using Galssoft.VKontakteWM.Common;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.Components.Common.ResponseClasses;
using Galssoft.VKontakteWM.Components.Common.Localization;
using Galssoft.VKontakteWM.Properties;
using Galssoft.VKontakteWM.Components.UI.Wrappers;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    class SendMessageController : Controller
    {
        #region Constructors

        public SendMessageController()
            : base(new MessageSendView())
        {
            Name = "SendMessageController";
        }

        #endregion

        public override void Activate()
        {
            view.Activate();
        }

        public override void Deactivate()
        {
            view.Deactivate();
        }

        /// <summary>
        /// This method indicates that something has been changed in the view.
        /// </summary>
        /// <param name="key">The string key to identify what has been changed.</param>
        protected override void OnViewStateChanged(string key)
        {
            #region Activate

            if (key == "Activate")
            {                
                int uid = Convert.ToInt32((string)ViewData["UserID"]);

                //string friendName = Globals.BaseLogic.GetFriendName(uid.ToString());
                string oldText = DraftMessagesDataIO.GetDraftMessage(uid);

                //view.ViewData["UserName"] = friendName;
                view.ViewData["MessageDraftInput"] = oldText;

                // проверяем: запретить ли смену пользователя?
                if (((string)ViewData["BackLink"]).Equals("ImageDetailedView"))
                {
                    ViewData["AFRHideButton"] = true;                    
                }
                else
                {
                    ViewData["AFRHideButton"] = false;
                }

                view.UpdateView("ActivateResponse");
            }

            #endregion

            #region SentMessage

            if (key == "SentMessage")
            {
                bool result = false;

                int uid = Convert.ToInt32((string)ViewData["UserID"]);
                string text = (string)ViewData["MessageDraftOutput"];

                try
                {                    
                    using (new WaitWrapper())
                    {
                        // проверка на Empty во вьюхе
                        result = Globals.BaseLogic.SendMessage(uid, text, false);
                    }
                }
                catch (VKException ex)
                {
                    string error = ExceptionTranslation.TranslateException(ex);

                    ViewData["SentMessageResponseMessage"] = error;
                    ViewData["MessageIsSent"] = "false";

                    view.UpdateView("SentMessageResponse");

                    if (ex.LocalizedMessage.Equals(ExceptionMessage.IncorrectLoginOrPassword))
                    {
                        Globals.BaseLogic.IDataLogic.SetToken(string.Empty);

                        view.UpdateView("GoToLogin");
                    }

                    return;
                }
                catch (OutOfMemoryException)
                {
                    ViewData["SentMessageResponseMessage"] = Resources.OutOfMemory;
                    ViewData["MessageIsSent"] = "false";

                    view.UpdateView("SentMessageResponse");
                    return;
                }

                using (new WaitWrapper(false))
                {
                    if (result)
                    {
                        DraftMessagesDataIO.DeleteDraftMessage(uid);
                        ViewData["MsgForChangedReceiver"] = string.Empty;

                        Globals.BaseLogic.SaveSendMessageToCache(uid, text);

                        ViewData["SentMessageResponseMessage"] =
                            Resources.MessagesList_Controller_Messages_MessageSentSuccessfully;
                        ViewData["MessageIsSent"] = "true";
                    }
                    else
                    {
                        ViewData["SentMessageResponseMessage"] =
                            Resources.MessagesList_Controller_Messages_MessageSentUnsuccessfully;
                        ViewData["MessageIsSent"] = "false";
                    }
                }

                view.UpdateView("SentMessageResponse");
            }

            #endregion

            #region Back
            if (key == "Back")
            {
                int uid = Convert.ToInt32((string)ViewData["UserID"]);
                string text = ((string)ViewData["MessageDraftOutput"]).Trim(); ;

                DraftMessagesDataIO.SetDraftMessage(text, uid);
                NavigateBack();
            }
            #endregion

            #region SendComplete
            if (key == "SendComplete")
            {
                NavigateBack();
            }
            #endregion

            #region ChangeReceiver

            if (key == "ChangeReceiver")
            {
                using (new WaitWrapper(false))
                {
                    MasterForm.Navigate<FriendsSearchListController>((string)ViewData["BackLink"]);
                }
            }

            #endregion

            if (key == "GoGoToLogin")
            {
                MasterForm.Navigate<LoginController>();
            }
        }

        private void NavigateBack()
        {
            switch ((string)ViewData["BackLink"])
            {
                case "FriendList":
                    MasterForm.Navigate<FriendsListController>();
                    break;
                //case "FriendSearch":
                    //MasterForm.Navigate<FriendsSearchListController>();
                    //break;
                case "MessagesList":
                    MasterForm.Navigate<MessagesListController>((string)ViewData["UserID"], (string)ViewData["UserName"]);
                    break;
                case "PhotoComments":
                    MasterForm.Navigate<PhotoCommentsUpdatesListController>();
                    break;
                case "FriendsStatus":
                    MasterForm.Navigate<StatusUpdatesListController>();
                    break;
                case "ImageDetailedView":
                    //MasterForm.Navigate<ImageCommentController>();
                    NavigationService.GoBack();
                    break;
                case "":
                    NavigationService.GoBack();
                    break;
            }
        }

        protected override void OnInitialize(params object[] parameters)
        {
            if (parameters != null)
            {
                if (parameters.Length > 0)
                {
                    string backLink = string.Empty;
                    int uid = 0;
                    string userName = string.Empty;

                    try
                    {
                        backLink = (string)parameters[0];
                    }
                    catch
                    {
                        NavigationService.GoBack();
                    }

                    try
                    {
                        uid = Convert.ToInt32(parameters[1].ToString());
                    }
                    catch
                    {
                        NavigationService.GoBack();
                    }

                    try
                    {
                        userName = (string)parameters[2];
                    }
                    catch
                    {
                        userName = Globals.BaseLogic.GetFriendName(uid.ToString());
                    }

                    ViewData["UserID"] = uid.ToString();
                    ViewData["UserName"] = userName;
                    ViewData["BackLink"] = backLink;
                }
            }

            base.OnInitialize(parameters);

            //if (parameters != null)
            //{
            //    if (parameters.Length > 0)
            //    {
            //        {
            //            string param0 = parameters[0].ToString();

            //            view.ViewData["UserID"] = param0;
            //        }
            //    }
            //}

            //base.OnInitialize(parameters);
        }
    }
}
