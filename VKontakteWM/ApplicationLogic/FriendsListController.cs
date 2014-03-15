using System;
using System.Windows.Forms;
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
using Galssoft.VKontakteWM.Components.Cache;
using Microsoft.WindowsMobile.PocketOutlook;
using Microsoft.WindowsMobile.Telephony;
using System.Text;
using System.IO;
using Galssoft.VKontakteWM.Components.Data;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    class FriendsListController : Controller<List<FriendListViewItem>>
    {
        #region Constructors

        public FriendsListController()
            : base(new FriendsListView())
        {
            Name = "FriendsListController";

            view.Model = new List<FriendListViewItem>();

            _afterLoadImageEventHandler += OnAfterLoadImage;

            timerKeepAwake = new System.Windows.Forms.Timer();
            timerKeepAwake.Tick += TimerKeepAwakeTick;
            timerKeepAwake.Interval = 10000;
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

        private AfterLoadImageEventHandler _afterLoadImageEventHandler;

        private void OnAfterLoadImage(object sender, AfterLoadImageEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.ImageFilename))
            {
                string fileName = SystemConfiguration.AppInstallPath + @"\Cache\Files\" + e.ImageFilename;

                try
                {
                    //ImageHelper.SaveScaledImage(fileName, fileName, e.ImageLinearSize, OpenNETCF.Drawing.RotationAngle.Zero);

                    ImageHelper.SaveSquareImage(fileName, fileName, e.ImageLinearSize);

                    if (FriendAvatarLoad != null)
                    {
                        FriendAvatarLoad(this, new AfterLoadFriendAvatarEventArgs(fileName, e.ImageLast));
                    }
                }
                catch
                {
                    File.Delete(fileName);
                }
            }
        }

        protected override void OnViewStateChanged(string key)
        {
            #region LoadList

            if (key == "LoadList")
            {
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
                            FriendsListResponse additionalFrendListResponse = LoadAdditionalDataFromCache();

                            ViewData["OriginalFrendListResponse"] = newFriendsListResponse;
                            ViewData["AdditionalFrendListResponse"] = additionalFrendListResponse;

                            FillListModel(newFriendsListResponse, additionalFrendListResponse, string.Empty);

                            Globals.BaseLogic.ICommunicationLogic.ClearImagesInDictionary();
                        }
                        else
                        {
                            view.Model.Clear();
                        }

                        ViewData["ListID"] = Globals.BaseLogic.IDataLogic.GetUid(); // сохраняем ID пользователя для которого был построен список

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

            #region GoToSendMessage

            if (key == "GoToSendMessage")
            {
                MasterForm.Navigate<SendMessageController>("FriendList", ViewData["Uid"]);
            }

            #endregion

            #region FilterFriendList

            if (key == "FilterFriendList")
            {
                using (new WaitWrapper(false))
                {
                    if ((string)ViewData["FilterString"] == string.Empty)
                    {
                        ViewData["OriginalModel"] = OriginalModel;
                        view.UpdateView("ResetState");
                    }
                    else
                    {
                        FillListModel((FriendsListResponse)ViewData["OriginalFrendListResponse"], (FriendsListResponse)ViewData["AdditionalFrendListResponse"], (string)ViewData["FilterString"]);
                        ViewData["OriginalModel"] = view.Model;
                        view.UpdateView("ResetState");
                    }
                }
            }

            #endregion

            #region CallNumber

            if (key == "CallNumber")
            {
                try
                {
                    if (ViewData["ABTelephone"] != null)
                    {
                        Phone phone = new Phone();

                        phone.Talk((string)ViewData["ABTelephone"], true);
                    }
                }
                catch
                {
                    // CallNumberResponseMessage_Unsuccess
                    ViewData["CallNumberResponseMessage"] = Resources.CallNumberResponseMessage_Unsuccess;
                    view.UpdateView("CallNumberResponse");
                }
            }

            #endregion

            #region SaveNumber

            if (key == "SaveNumber")
            {
                using (OutlookSession os = new OutlookSession())
                {
                    try
                    {
                        if (ViewData["ABFirstName"] != null && ViewData["ABLastName"] != null && ViewData["ABTelephone"] != null)
                        {
                            Contact contact = null;

                            foreach (var val in os.Contacts.Items)
                            {
                                if (val.FirstName == (string)ViewData["ABFirstName"] && val.LastName == (string)ViewData["ABLastName"])
                                {
                                    contact = val;
                                }
                            }

                            if (contact == null)
                            {
                                contact = new Contact();

                                contact.FirstName = (string)ViewData["ABFirstName"];
                                contact.LastName = (string)ViewData["ABLastName"];

                                os.Contacts.Items.Add(contact);
                            }

                            contact.MobileTelephoneNumber = (string)ViewData["ABTelephone"];

                            if (ViewData["ABBD"] != null)
                            {
                                contact.Birthday = (DateTime)ViewData["ABBD"];
                            }

                            if (ViewData["ImagePath"] != null)
                            {
                                contact.SetPicture((string)ViewData["ImagePath"]);
                            }

                            contact.Update();

                            ViewData["SaveNumberResponseMessage"] = Resources.SaveNumberResponseMessage_Succsess;
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                    catch (Exception)
                    {
                        ViewData["SaveNumberResponseMessage"] = Resources.SaveNumberResponseMessage_Unsuccess;
                    }
                }

                view.UpdateView("SaveNumberResponse");

                //contact.B

                //contact.MobileTelephoneNumber

                //using (new WaitWrapper(false))
                //{
                //    if ((string)ViewData["FilterString"] == string.Empty)
                //    {
                //        ViewData["OriginalModel"] = OriginalModel;
                //        view.UpdateView("ResetState");
                //    }
                //    else
                //    {
                //        FillListModel((FriendsListResponse) ViewData["OriginalFrendListResponse"], null, (string) ViewData["FilterString"]);
                //        ViewData["OriginalModel"] = view.Model;
                //        view.UpdateView("ResetState");
                //    }
                //}
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

            #region GoToPhotos

            if (key == "GoToPhotos")
            {
                MasterForm.Navigate<ShareController>();
            }

            #endregion

            #region GoToExtras

            if (key == "GoToExtras")
            {
                MasterForm.Navigate<ExtraController>();
            }

            #endregion



            #region ListActualization

            if (key == "ListActualization")
            {
                string currentID = Globals.BaseLogic.IDataLogic.GetUid();
                string listID = (string)ViewData["ListID"];

                if (currentID != listID)
                {
                    ViewData["IsRefresh"] = false;

                    OnViewStateChanged("LoadList");
                }
            }

            #endregion

            #region GoToSettings

            if (key == "GoToSettings")
            {
                using (new WaitWrapper(false))
                {
                    Configuration.LoadConfigSettings();
                    MasterForm.Navigate<SettingsController>();
                }
            }

            #endregion

            #region GoToUserData

            if (key == "GoToUserData")
            {
                using (new WaitWrapper(false))
                {
                    MasterForm.Navigate<UserDataController>();
                }
            }

            #endregion

            #region GoToAbout

            if (key == "GoToAbout")
            {
                using (new WaitWrapper(false))
                {
                    MasterForm.Navigate<AboutController>();
                }
            }

            #endregion

            #region GoToExit

            if (key == "GoToExit")
            {
                using (new WaitWrapper(false))
                {
                    if (SystemConfiguration.DeleteOnExit)
                        Globals.BaseLogic.IDataLogic.ClearPass();
                    Application.Exit();
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
                    int uid = 0;

                    try
                    {
                        uid = Convert.ToInt32(parameters[0].ToString());
                    }
                    catch
                    {
                        uid = -1;
                    }

                    // передаем только корректный id
                    if (uid > 0)
                    {
                        MasterForm.Navigate<SendMessageController>("FriendList", uid.ToString());
                    }
                }
            }

            base.OnInitialize(parameters);
        }

        #endregion

        private void FillListModel(FriendsListResponse newFriendsListResponse, FriendsListResponse additionalFriendsListResponse, string filter)
        {
            if (additionalFriendsListResponse == null)
            {
                additionalFriendsListResponse = new FriendsListResponse();
            }

            view.Model.Clear();

            FriendListViewItem newFriendListViewItem = null;

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
                                                    Group = string.IsNullOrEmpty(newUser.LastName) ? 
                                                        Resources.FriendList_Noname : SetGroup(newUser.LastName[0].ToString()),
                                                    Avatar =
                                                        SystemConfiguration.AppInstallPath + @"\Cache\Files\Thumb\" +
                                                        HttpUtility.GetMd5Hash(newUser.Photo100px)
                                                };

                    bool result = Globals.BaseLogic.ICommunicationLogic.LoadImage(newUser.Photo100px,
                                                                                  @"Thumb\" +
                                                                                  HttpUtility.GetMd5Hash(
                                                                                      newUser.Photo100px), false,
                                                                                  _afterLoadImageEventHandler,
                                                                                  UISettings.CalcPix(50),
                                                                                  newUser.LastName, "string");

                    newFriendListViewItem.IsAvatarLoaded = result;

                    // доп. дата
                    string telephone = string.Empty;
                    DateTime bd = new DateTime(0);

                    User additionalUserData = additionalFriendsListResponse.GetUserByID(newUser.Uid);

                    if (additionalUserData != null)
                    {
                        if (!string.IsNullOrEmpty(additionalUserData.MobilePhone))
                        {
                            newFriendListViewItem.Telephone = GetValidTelephoneNumber(additionalUserData.MobilePhone);
                        }

                        if (!additionalUserData.Birthday.Equals(bd))
                        {
                            newFriendListViewItem.Birthday = additionalUserData.Birthday;
                        }
                    }

                    view.Model.Add(newFriendListViewItem);

                }
                view.Model.Sort();

                OriginalModel = new List<FriendListViewItem>();

                foreach (FriendListViewItem item in view.Model)
                {
                    OriginalModel.Add(item);
                }
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
                        newFriendListViewItem.Group = SetGroup(string.IsNullOrEmpty(newUser.LastName) ?
                                                        Resources.FriendList_Noname : SetGroup(newUser.LastName[0].ToString()));

                        newFriendListViewItem.Avatar = SystemConfiguration.AppInstallPath + @"\Cache\Files\Thumb\" + HttpUtility.GetMd5Hash(newUser.Photo100px);

                        bool result = Globals.BaseLogic.ICommunicationLogic.LoadImage(newUser.Photo100px,
                                                                                      @"Thumb\" +
                                                                                      HttpUtility.GetMd5Hash(
                                                                                          newUser.Photo100px), false,
                                                                                      _afterLoadImageEventHandler,
                                                                                      UISettings.CalcPix(50),
                                                                                      newUser.LastName, "string");

                        newFriendListViewItem.IsAvatarLoaded = result;

                        // доп. дата
                        string telephone = string.Empty;
                        DateTime bd = new DateTime(0);

                        User additionalUserData = additionalFriendsListResponse.GetUserByID(newUser.Uid);

                        if (additionalUserData != null)
                        {
                            if (!string.IsNullOrEmpty(additionalUserData.MobilePhone))
                            {
                                newFriendListViewItem.Telephone = GetValidTelephoneNumber(additionalUserData.MobilePhone);
                            }

                            if (!additionalUserData.Birthday.Equals(bd))
                            {
                                bd = additionalUserData.Birthday;

                                newFriendListViewItem.Birthday = bd;
                            }
                        }

                        view.Model.Add(newFriendListViewItem);
                    }
                }

                view.Model.Sort();
            }
        }

        private System.Windows.Forms.Timer timerKeepAwake;

        private static void TimerKeepAwakeTick(object sender, EventArgs eventArgs)
        {
            CoreHelper.KeepDeviceAwake();
            DebugHelper.WriteLogEntry("TimerKeepAwakeTick");
        }

        private void AsyncGetViewData(LoadingControlInterface lc)
        {
            try
            {
                timerKeepAwake.Enabled = true;

                lc.Current = 0;

                FriendsListResponse newFriendsListResponse = null;

                try
                {
                    lc.Current = 5;

                    newFriendsListResponse = Globals.BaseLogic.LoadFriendsList(true, false);
                    lc.Current = 95;
                }
                catch (VKException ex)
                {
                    timerKeepAwake.Enabled = false;
                    string error = ExceptionTranslation.TranslateException(ex);

                    if (!string.IsNullOrEmpty(error))
                    {
                        ViewData["LoadListResponseMessage"] = error;
                        view.UpdateView("LoadListResponseNegative");

                        //newFriendsListResponse = LoadDataFromCache();

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
                    FriendsListResponse additionalFrendListResponse = LoadAdditionalDataFromCache();

                    ViewData["OriginalFrendListResponse"] = newFriendsListResponse;
                    ViewData["AdditionalFrendListResponse"] = additionalFrendListResponse;

                    FillListModel(newFriendsListResponse, additionalFrendListResponse, string.Empty);
                }
                else
                {
                    //view.Model.Clear();
                }

                ViewData["ListID"] = Globals.BaseLogic.IDataLogic.GetUid(); // сохраняем ID пользователя для которого был построен список

                view.UpdateView("LoadListResponse");

                // запускаем поток || прогрузки фотографий
                var t = new Thread(delegate { Globals.BaseLogic.ICommunicationLogic.LoadImagesInDictionary(); })
                {
                    IsBackground = true
                };

                t.Start();

                lc.Current = 100;
            }
            finally
            {
                timerKeepAwake.Enabled = false;
            }
        }

        private string SetGroup(string firstLetter)
        {
            string result = firstLetter.ToUpper();

            if (!(result.CompareTo("А") >= 0 && result.CompareTo("Я") <= 0) && !(result.CompareTo("A") >= 0 && result.CompareTo("Z") <= 0))
            {
                return "#";
            }          

            return result;
        }

        //private FriendsListResponse LoadDataFromCache()
        //{
        //    try
        //    {
        //        return Cache.LoadFromCache<FriendsListResponse>(string.Empty, "FriendsListResponse");
        //    }
        //    catch
        //    {
        //        //
        //    }

        //    return null;
        //}

        private FriendsListResponse LoadAdditionalDataFromCache()
        {
            //try
            //{
            //    return Cache.LoadFromCache<FriendsListResponse>(string.Empty, "FriendsListAdditionalResponse");
            //}
            //catch
            //{
            //    //
            //}

            //return null;

            return DataModel.Data.FriendsListAdditionalResponseData;
        }

        private string GetValidTelephoneNumber(string tel)
        {
            StringBuilder result = new StringBuilder();

            // вырезаем все симовлы не цифры...
            for (int i = 0; i < tel.Length; i++)
            {
                if (tel[i] >= '0' && tel[i] <= '9')
                {
                    result.Append(tel[i]);
                }
            }

            // выравниваем длинну
            if (result.Length < 11)
            {
                return string.Empty;
            }
            else if (result.Length > 11)
            {
                result = result.Remove(11, result.Length - 11);
            }

            // проверяем первую цифру
            if (result[0] == '8')
            {
                result[0] = '7';
            }
            else
            {
                if (result[0] != '7')
                {
                    return string.Empty;
                }
            }

            // добавляем "+"
            return "+" + result.ToString();
        }
    }
}
