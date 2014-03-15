using System;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Common;
using Galssoft.VKontakteWM.Components;
using Galssoft.VKontakteWM.Components.Configuration;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.Server;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.Components.Common.ResponseClasses;
//using Galssoft.VKontakteWM.Notification.ServiceClasses;
using Galssoft.VKontakteWM.Properties;
using System.Collections.Generic;
using Galssoft.VKontakteWM.Components.ImageClass;
using Galssoft.VKontakteWM.Components.Common.Localization;
using System.Threading;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.Common.Configuration;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    class FriendsOnlineListController : Controller<List<FriendListViewItem>>
    {
        #region Constructors

        public FriendsOnlineListController()
            : base(new FriendsOnlineListView())
        {
            Name = "FriendsListController";

            view.Model = new List<FriendListViewItem>();

            _afterLoadImageEventHandler += OnAfterLoadImage;
        }

        #endregion

        #region Events

        [PublishEvent("OnFriendAvatarLoad")]
        public event EventHandler FriendAvatarLoad;

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

        private AfterLoadImageEventHandler _afterLoadImageEventHandler;

        private void OnAfterLoadImage(object sender, AfterLoadImageEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.ImageFilename))
            {
                string fileName = SystemConfiguration.AppInstallPath + @"\Cache\Files\" + e.ImageFilename;

                ImageHelper.SaveScaledImage(fileName, fileName, UISettings.CalcPix(50), OpenNETCF.Drawing.RotationAngle.Zero);

                if (FriendAvatarLoad != null)
                {
                    FriendAvatarLoad(this, new AfterLoadFriendAvatarEventArgs(fileName, e.ImageLast));
                }
            }
        }

        protected override void OnViewStateChanged(string key)
        {
            #region LoadList

            if (key == "LoadList")
            {
                FriendsListResponse newFriendsListResponse = null;

                try
                {
                    newFriendsListResponse = Globals.BaseLogic.LoadFriendsOnlineList(false);
                }
                catch (VKException ex)
                {
                    switch (ex.LocalizedMessage)
                    {
                        case ExceptionMessage.UnknownError:
                            ViewData["LoadListResponseMessage"] = Resources.VK_ERRORS_UnknownError;
                            break;

                        case ExceptionMessage.ServerUnavalible:
                            ViewData["LoadListResponseMessage"] = Resources.VK_ERRORS_ServerUnavalible;
                            break;

                        case ExceptionMessage.NoConnection:
                            ViewData["LoadListResponseMessage"] = Resources.VK_ERRORS_NoConnection;
                            break;
                    }

                    view.UpdateView("LoadListResponseNegative");
                    return;
                }
                catch (OutOfMemoryException)
                {
                    ViewData["LoadListResponseMessage"] = Resources.OutOfMemory;

                    view.UpdateView("LoadListResponseNegative");
                    return;
                }

                if (newFriendsListResponse != null)
                {
                    view.Model.Clear();

                    foreach (User newUser in newFriendsListResponse.Users)
                    {
                        FriendListViewItem newFriendListViewItem = new FriendListViewItem();

                        newFriendListViewItem.Uid = newUser.Uid;
                        //newFriendListViewItem.UserName = newUser.FullName;

                        newFriendListViewItem.Avatar = SystemConfiguration.AppInstallPath + @"\Cache\Files\Thumb\" + HttpUtility.GetMd5Hash(newUser.Photo100px);

                        bool result = Globals.BaseLogic.ICommunicationLogic.LoadImage(newUser.Photo100px, @"Thumb\" + HttpUtility.GetMd5Hash(newUser.Photo100px), false, _afterLoadImageEventHandler, 100, 0, "int");

                        newFriendListViewItem.IsAvatarLoaded = result;

                        view.Model.Add(newFriendListViewItem);
                    }

                    view.Model.Sort();

                    view.UpdateView("LoadListResponse");

                    // запускаем поток || прогрузки фотографий
                    var t = new Thread(() => Globals.BaseLogic.ICommunicationLogic.LoadImagesInDictionary())
                    {
                        IsBackground = true
                    };

                    t.Start();
                }
                else
                {
                    ViewData["LoadListResponseMessage"] = Resources.FriendsList_Messages_OperationDoneUnsuccsessfully;

                    view.UpdateView("LoadListResponseNegative");
                }
            }

            #endregion

            #region ReloadList

            if (key == "ReloadList")
            {
                view.UpdateView("ReloadListResponse");
            }

            #endregion



            #region GoToMain

            if (key == "GoToMain")
            {
                MasterForm.Navigate<MainController>();
            }

            #endregion

            #region GoToNews

            if (key == "GoToNews")
            {
                MasterForm.Navigate<StatusUpdatesListController>();
            }

            #endregion

            #region GoToMessages

            if (key == "GoToMessages")
            {
                MasterForm.Navigate<MessagesChainsListController>();
            }

            #endregion

            #region GoToExtras

            if (key == "GoToExtras")
            {
                MasterForm.Navigate<ExtraController>();
            }

            #endregion

            #region GoToFriends

            if (key == "GoToFriends")
            {
                MasterForm.Navigate<FriendsListController>();
            }

            #endregion
        }

        protected override void OnInitialize(params object[] parameters)
        {
            if (parameters != null)
            {
                if (parameters.Length > 0)
                {
                    int uid = 0;

                    try
                    {
                        uid = Convert.ToInt32(parameters[0].ToString());
                    }
                    catch
                    {
                        return;
                    }

                    if (uid > 0)
                    {
                        MasterForm.Navigate<SendMessageController>(uid.ToString());
                    }
                }
            }

            base.OnInitialize(parameters);
        }

        #endregion
    }
}
