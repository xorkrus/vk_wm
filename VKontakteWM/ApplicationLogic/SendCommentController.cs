using System;
using System.IO;
using Galssoft.VKontakteWM.Common;
using Galssoft.VKontakteWM.Components.Cache;
using Galssoft.VKontakteWM.Components.Common.Localization;
using Galssoft.VKontakteWM.Components.Common.ResponseClasses;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.UI.Wrappers;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.Properties;
using Galssoft.VKontakteWM.Components.Data;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    class SendCommentController : Controller
    {
        #region Constructors

        public SendCommentController()
            : base(new SendCommentView())
        {
            Name = "SendCommentController";
        }

        #endregion

        #region Events

        #endregion

        #region Controller implementations

        public override void Activate()
        {
            view.Activate();
            view.UpdateView("ClearTextBox");
        }

        public override void Deactivate()
        {
            view.Deactivate();
        }

        protected override void OnViewStateChanged(string key)
        {
            #region SendMessage

            if (key == "SendMessage")
            {

                try
                {
                    using (new WaitWrapper(false))
                    {
                        Globals.BaseLogic.AddComments((string) ViewData["Message"],
                                                      (string) ViewData["UserID"] + "_" +
                                                      (string) ViewData["PhotoID"],
                                                      false);
                    }
                    view.UpdateView("SendCommentSuccess");

                    using (new WaitWrapper(false))
                    {
                        PhotosCommentsResponseHistory pchResponse = null;
                        PhotosCommentsResponse pcResponse = null;

                        try
                        {
                            //pchResponse = Cache.LoadFromCache<PhotosCommentsResponseHistory>(string.Empty, "PhotosCommentsResponseHistory");

                            pchResponse = DataModel.Data.PhotosCommentsResponseHistoryData;

                            if (pchResponse == null)
                            {
                                throw new Exception();
                            }
                        }
                        catch (Exception)
                        {
                            pchResponse = new PhotosCommentsResponseHistory();
                        }

                        pcResponse = pchResponse.GetItem(Convert.ToInt32((string) ViewData["PhotoID"]));

                        if (pcResponse == null)
                        {
                            pcResponse = new PhotosCommentsResponse();
                            pchResponse.AddItem(pcResponse);
                        }

                        var newPhotoData = new PhotoData
                                               {
                                                   pdPhotoID = Convert.ToInt32((string) ViewData["PhotoID"]),
                                                   pdUserID = Convert.ToInt32((string) ViewData["UserID"]),
                                                   pdPhotoURL604px = (string) ViewData["LargePhotoUrl"]
                                               };

                        var newPostReceiver = new PostReceiver
                                                  {
                                                      prUserID = Convert.ToInt32((string) ViewData["UserID"])
                                                  };

                        var newPostSender = new PostSender
                                                {
                                                    psUserID =
                                                        Convert.ToInt32(Globals.BaseLogic.IDataLogic.GetUid()),
                                                    psUserName = Resources.MessageI
                                                };

                        var newWallData = new WallData
                                              {
                                                  wdWallDataType = WallDataType.PlainText,
                                                  wdText = (string) ViewData["Message"]
                                              };

                        pcResponse.pcrPhotoID = Convert.ToInt32((string) ViewData["PhotoID"]);
                        pcResponse.pcrComments.Insert(0, new CommentPost
                                                             {
                                                                 cpID = 0,
                                                                 cpTime = DateTime.Now,
                                                                 cpPhotoData = newPhotoData,
                                                                 cpPostReceiver = newPostReceiver,
                                                                 cpPostSender = newPostSender,
                                                                 cpWallData = newWallData
                                                             }
                            );

                        //try
                        //{
                        //    bool result = Cache.SaveToCache(pchResponse, string.Empty,
                        //                                    "PhotosCommentsResponseHistory");
                        //    DebugHelper.WriteLogEntry(result
                        //                                  ? "Новый комментарий сохранен в кэш."
                        //                                  : "Новый комментарий не сохранены в кэш.");
                        //}
                        //catch (IOException newException)
                        //{
                        //    DebugHelper.WriteLogEntry("Ошибка сохранения комментария к фото в кэш: " +
                        //                              newException.Message);
                        //}

                        DataModel.Data.PhotosCommentsResponseHistoryData = pchResponse;

                        if ((string)ViewData["UserID"] == Globals.BaseLogic.IDataLogic.GetUid())
                        {
                            try
                            {
                                //pcResponse = Cache.LoadFromCache<PhotosCommentsResponse>(string.Empty,
                                //                                                         "PhotosCommentsResponse");

                                pcResponse = DataModel.Data.PhotosCommentsResponseData;

                                if (pcResponse == null)
                                    throw new Exception();
                            }
                            catch (Exception)
                            {
                                pcResponse = new PhotosCommentsResponse();
                            }

                            pcResponse.pcrPhotoID = Convert.ToInt32((string)ViewData["PhotoID"]);
                            pcResponse.pcrComments.Insert(0, new CommentPost
                                                                 {
                                                                     cpID = 0,
                                                                     cpTime = DateTime.Now,
                                                                     cpPhotoData = newPhotoData,
                                                                     cpPostReceiver = newPostReceiver,
                                                                     cpPostSender = newPostSender,
                                                                     cpWallData = newWallData
                                                                 }
                                );

                            //try
                            //{
                            //    bool result = Cache.SaveToCache(pcResponse, string.Empty, "PhotosCommentsResponse");
                            //    DebugHelper.WriteLogEntry(result
                            //                                  ? "Новый комментарий сохранен в кэш комментариев к своим фото."
                            //                                  : "Новый комментарий не сохранен в кэш комментариев к своим фото.");
                            //}
                            //catch (IOException newException)
                            //{
                            //    DebugHelper.WriteLogEntry("Ошибка сохранения комментария к своему фото в кэш: " +
                            //                              newException.Message);
                            //}

                            DataModel.Data.PhotosCommentsResponseData = pcResponse;
                        }
                        NavigationService.GoBack();

                    }
                }
                catch (VKException ex)
                {
                    //timerKeepAwake.Enabled = false;
                    string err = ExceptionTranslation.TranslateException(ex);
                    if (!string.IsNullOrEmpty(err))
                    {
                        ViewData["SendCommentError"] = err;
                        view.UpdateView("SendCommentFail");

                        if (ex.LocalizedMessage == ExceptionMessage.IncorrectLoginOrPassword)
                        {
                            Globals.BaseLogic.IDataLogic.SetToken(string.Empty);

                            view.UpdateView("GoToLogin");
                        }
                    }
                }
                catch (OutOfMemoryException)
                {
                    ViewData["SendCommentError"] = Resources.OutOfMemory;
                    view.UpdateView("SendCommentFail");
                }

            }

            #endregion

            #region GoBack

            if (key == "GoBack")
            {
                using (new WaitWrapper(false))
                {
                    NavigationService.GoBack();
                }
            }

            #endregion

            if (key == "GoGoToLogin")
            {
                MasterForm.Navigate<LoginController>();
            }
        }

        /// <summary>
        /// Notifies the overriding class of the Initialize method been called.
        /// </summary>
        /// <param name="parameters">An array of parameters</param>
        protected override void OnInitialize(params object[] parameters)
        {
            if ((parameters != null) && (parameters.Length == 3))
            {
                ViewData["UserID"] = parameters[0];
                ViewData["PhotoID"] = parameters[1];
                ViewData["LargePhotoUrl"] = parameters[2];
            }

            base.OnInitialize(parameters);
        }

        #endregion
    }
}
