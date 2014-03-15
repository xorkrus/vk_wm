using System;
using Galssoft.VKontakteWM.Common;
using Galssoft.VKontakteWM.Components.Configuration;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.Server;
using Galssoft.VKontakteWM.Components.UI.Wrappers;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.Components.Common.ResponseClasses;
using Galssoft.VKontakteWM.Properties;
using System.Collections.Generic;
using Galssoft.VKontakteWM.Components.Common.Localization;
using System.Threading;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.CustomControls;
using System.IO;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    class FriendsSearchListController : Controller<List<FriendListViewItem>>
    {
        #region Constructors

        public FriendsSearchListController()
            : base(new FriendsSearchListView())
        {
            Name = "FriendsSearchListController";

            view.Model = new List<FriendListViewItem>();

            _afterLoadImageEventHandler += OnAfterLoadImage;
        }

        #endregion

        #region Events

        [PublishEvent("OnFriendAvatarLoad")]
        public event EventHandler FriendAvatarLoad;

        #endregion

        public static List<FriendListViewItem> OriginalModel;

        #region Controller implementations

        public override void Activate()
        {
            view.Activate();
        }

        public override void Deactivate()
        {
            view.Deactivate();
        }

        private readonly AfterLoadImageEventHandler _afterLoadImageEventHandler;

        private void OnAfterLoadImage(object sender, AfterLoadImageEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.ImageFilename))
            {
                string fileName = SystemConfiguration.AppInstallPath + @"\Cache\Files\" + e.ImageFilename;

                ImageHelper.SaveScaledImage(fileName, fileName, e.ImageLinearSize, OpenNETCF.Drawing.RotationAngle.Zero);

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
                //LoadingControlInterface lc = LoadingControl.CreateLoading(Resources.DataLoading);

                //var asyncDataThread = new Thread(() => AsyncGetViewData(lc)) {IsBackground = true};
                //asyncDataThread.Start();

                //lc.ShowLoading(true);

                //if (lc.Abort)
                //{
                //    asyncDataThread.Abort();
                //}

                bool isRefrsh = Convert.ToBoolean(ViewData["IsRefresh"]);

                if (isRefrsh)
                {
                    LoadingControlInterface lc = LoadingControl.CreateLoading(Resources.DataLoading);

                    Thread asyncDataThread = new Thread(delegate { AsyncGetViewData(lc); });
                    asyncDataThread.IsBackground = true;
                    asyncDataThread.Start();

                    lc.ShowLoading(true);

                    if (lc.Abort)
                    {
                        asyncDataThread.Abort();
                    }
                }
                else
                {
                    try
                    {
                        FriendsListResponse newFriendsListResponse = Globals.BaseLogic.LoadFriendsList(false, false);

                        if (newFriendsListResponse != null)
                        {
                            //FriendsListResponse additionalFrendListResponse = LoadAdditionalDataFromCache();

                            ViewData["OriginalFrendListResponse"] = newFriendsListResponse;
                            //ViewData["AdditionalFrendListResponse"] = additionalFrendListResponse;

                            FillListModel(newFriendsListResponse, string.Empty);

                            Globals.BaseLogic.ICommunicationLogic.ClearImagesInDictionary();
                        }
                        else
                        {
                            //view.Model.Clear();
                        }

                        //ViewData["ListID"] = Globals.BaseLogic.IDataLogic.GetUid(); // сохраняем ID пользователя для которого был построен список

                        view.UpdateView("LoadListResponse");
                    }
                    catch
                    {
                        //
                    }
                }
            }

            #endregion

            #region ReloadList

            if (key == "ReloadList")
            {
                view.UpdateView("ReloadListResponse");
            }

            #endregion

            #region RefreshList

            if (key == "RefreshList")
            {
                view.UpdateView("RefreshListResponse");
            }

            #endregion

            #region UserChoise

            if (key == "UserChoise")
            {
                if ((string)ViewData["Uid"] != string.Empty)
                {
                    using (new WaitWrapper(false))
                    {
                        //var uid = (string) ViewData["Addressee"];
                        MasterForm.Navigate<SendMessageController>((string)ViewData["BackLink"], ViewData["Uid"]);
                    }
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

            #region FilterFriendList

            if (key == "FilterFriendList")
            {
                using (new WaitWrapper(false))
                {
                    if ((string) ViewData["FilterString"] == string.Empty)
                    {
                        ViewData["OriginalModel"] = OriginalModel;
                        view.UpdateView("ResetState");
                    }
                    else
                    {
                        FillListModel((FriendsListResponse) ViewData["OriginalFrendListResponse"],
                                      (string) ViewData["FilterString"]);
                        ViewData["OriginalModel"] = view.Model;
                        view.UpdateView("ResetState");
                    }
                }
            }

            #endregion

            if (key == "GoGoToLogin")
            {
                MasterForm.Navigate<LoginController>();
            }
        }

        protected override void OnInitialize(params object[] parameters)
        {
            if (parameters != null)
            {
                if (parameters.Length > 0)
                {
                    try
                    {
                        ViewData["BackLink"] = parameters[0].ToString();
                    }
                    catch
                    {
                        NavigationService.GoBack();
                    }

                    // передаем только корректный id
                    /*
                    if (uid > 0)
                    {
                        MasterForm.Navigate<SendMessageController>("FriendSearch", uid.ToString());
                    }
                    */
                }
            }

            base.OnInitialize(parameters);
        }

        #endregion

        private void FillListModel(FriendsListResponse newFriendsListResponse, string filter)
        {
            view.Model.Clear();
            FriendListViewItem newFriendListViewItem;
            if (filter == string.Empty)
            {
                foreach (User newUser in newFriendsListResponse.Users)
                {
                    newFriendListViewItem = new FriendListViewItem
                    {
                        Uid = newUser.Uid,
                        FirstName = newUser.FirstName,
                        LastName = newUser.LastName,
                        IsOnline = newUser.IsOnline == "1",
                        Group = SetGroup(newUser.LastName[0].ToString()),
                        Avatar =
                            SystemConfiguration.AppInstallPath + @"\Cache\Files\Thumb\" +
                            HttpUtility.GetMd5Hash(newUser.Photo100px)
                    };

                    //bool result = Globals.BaseLogic.ICommunicationLogic.LoadImage(newUser.Photo100px,
                    //                                                              @"Thumb\" +
                    //                                                              HttpUtility.GetMd5Hash(
                    //                                                                  newUser.Photo100px), false,
                    //                                                              _afterLoadImageEventHandler,
                    //                                                              UISettings.CalcPix(32),
                    //                                                              newUser.LastName, "string");

                    bool result = File.Exists(newFriendListViewItem.Avatar);

                    newFriendListViewItem.IsAvatarLoaded = result;
                    view.Model.Add(newFriendListViewItem);

                }
                view.Model.Sort();

                OriginalModel = new List<FriendListViewItem>();
                foreach (FriendListViewItem item in view.Model)
                    OriginalModel.Add(item);
            }
            else
            {
                foreach (User newUser in newFriendsListResponse.Users)
                {
                    newFriendListViewItem = new FriendListViewItem();
                    if ((newUser.FirstName + newUser.LastName).ToLower().IndexOf(filter.ToLower()) != -1)
                    {

                        newFriendListViewItem.Uid = newUser.Uid;
                        newFriendListViewItem.FirstName = newUser.FirstName;
                        newFriendListViewItem.LastName = newUser.LastName;
                        newFriendListViewItem.IsOnline = newUser.IsOnline == "1";
                        newFriendListViewItem.Group = SetGroup(newUser.LastName[0].ToString());

                        newFriendListViewItem.Avatar = SystemConfiguration.AppInstallPath + @"\Cache\Files\Thumb\" +
                                                       HttpUtility.GetMd5Hash(newUser.Photo100px);

                        //bool result = Globals.BaseLogic.ICommunicationLogic.LoadImage(newUser.Photo100px,
                        //                                                              @"Thumb\" +
                        //                                                              HttpUtility.GetMd5Hash(
                        //                                                                  newUser.Photo100px), false,
                        //                                                              _afterLoadImageEventHandler,
                        //                                                              UISettings.CalcPix(32),
                        //                                                              newUser.LastName, "string");

                        bool result = File.Exists(newFriendListViewItem.Avatar);

                        newFriendListViewItem.IsAvatarLoaded = result;
                        view.Model.Add(newFriendListViewItem);
                    }
                }

                view.Model.Sort();
            }
        }

        private void AsyncGetViewData(LoadingControlInterface lc)
        {
            lc.Current = 0;

            FriendsListResponse newFriendsListResponse = null;

            //bool isRefresh = Convert.ToBoolean((string)ViewData["IsRefresh"]);

            try
            {
                lc.Current = 5;

                //if (lc.Abort)
                //{
                //    isRefresh = false;
                //}

                newFriendsListResponse = Globals.BaseLogic.LoadFriendsList(true, false);
                lc.Current = 95;
            }
            catch (VKException ex)
            {
                string error = ExceptionTranslation.TranslateException(ex);

                if (!string.IsNullOrEmpty(error))
                {
                    ViewData["LoadListResponseMessage"] = error;
                    view.UpdateView("LoadListResponseNegative");

                    if (ex.LocalizedMessage.Equals(ExceptionMessage.IncorrectLoginOrPassword))
                    {
                        Globals.BaseLogic.IDataLogic.SetToken(string.Empty);

                        view.UpdateView("GoToLogin");
                    }
                }
            }
            catch (OutOfMemoryException)
            {
                ViewData["LoadListResponseMessage"] = Resources.OutOfMemory;
                view.UpdateView("LoadListResponseNegative");
            }

            if (newFriendsListResponse != null)
            {
                ViewData["OriginalFrendListResponse"] = newFriendsListResponse;

                FillListModel(newFriendsListResponse, string.Empty);
            }

            view.UpdateView("LoadListResponse");

            //// запускаем поток || прогрузки фотографий
            //var t = new Thread(() => Globals.BaseLogic.ICommunicationLogic.LoadImagesInDictionary())
            //{
            //    IsBackground = true
            //};

            //t.Start();

            lc.Current = 100;
        }

        private static string SetGroup(string firstLetter)
        {
            string result = firstLetter.ToUpper();

            if (!(result.CompareTo("А") >= 0 && result.CompareTo("Я") <= 0) && !(result.CompareTo("A") >= 0 && result.CompareTo("Z") <= 0))
            {
                return "#";
            }

            return result;
        }
    }
}
