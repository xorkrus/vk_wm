using System;
using System.Collections.Generic;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.Components.MVC;
//using Galssoft.VKontakteWM.Notification.ServiceClasses;
using Galssoft.VKontakteWM.Properties;
using Galssoft.VKontakteWM.Common;
using Galssoft.VKontakteWM.Components.UI.Wrappers;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.UI.Controls;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    class SettingsController : Controller
    {
        #region Constructors

        public SettingsController()
            : base(new SettingsView())
        {
            Name = "SettingsController";
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

            //При переходе на MultiValueSettingsController надо сохранить настройки,
            //чтобы при возвращении они не сбросились
            var settings = (List<SettingsListViewItems>)ViewData["Settings"];

            foreach (var settingsItem in settings)
            {
                #region AutoUpdateAtStart
                if (settingsItem.OptionName == Resources.AutoUpdateAtStart)
                {
                    if ((bool)settingsItem.OptionValue)
                    {
                        Configuration.AutoUpdateAtStart = true;
                    }
                    else
                    {
                        Configuration.AutoUpdateAtStart = false;
                    }
                }
                #endregion

                #region UpdateFriendsStatus
                if (settingsItem.OptionName == Resources.UpdateFriendsStatus)
                {
                    if ((bool)settingsItem.OptionValue)
                    {
                        Configuration.UpdateFriendsStatus = true;
                    }
                    else
                    {
                        Configuration.UpdateFriendsStatus = false;
                    }
                }
                #endregion

                #region RenewInRoam
                if (settingsItem.OptionName == Resources.RenewInRoam)
                {
                    if ((bool)settingsItem.OptionValue)
                    {
                        Configuration.RenewInRoam = true;
                    }
                    else
                    {
                        Configuration.RenewInRoam = false;
                    }
                }
                #endregion

                #region WiFi
                if (settingsItem.OptionName == Resources.SettingsView_OnlyWiFi)
                {
                    if ((bool)settingsItem.OptionValue)
                    {
                        Configuration.OnlyWiFi = true;
                    }
                    else
                    {
                        Configuration.OnlyWiFi = false;
                    }
                }
                #endregion

                #region TraceMessages
                if (settingsItem.OptionName == Resources.Settings_Messages)
                {
                    if ((bool)settingsItem.OptionValue)
                    {
                        Configuration.TraceMessagess = true;
                    }
                    else
                    {
                        Configuration.TraceMessagess = false;
                    }
                }
                #endregion

                #region TraceComments
                if (settingsItem.OptionName == Resources.Settings_Comments)
                {
                    if ((bool)settingsItem.OptionValue)
                    {
                        Configuration.TraceComments = true;
                    }
                    else
                    {
                        Configuration.TraceComments = false;
                    }
                }
                #endregion

                #region TraceFriends
                if (settingsItem.OptionName == Resources.Settings_Friends)
                {
                    if ((bool)settingsItem.OptionValue)
                    {
                        Configuration.TraceFriends = true;
                    }
                    else
                    {
                        Configuration.TraceFriends = false;
                    }
                }
                #endregion

                #region TraceFriendsNews
                if (settingsItem.OptionName == Resources.Settings_FriendsNews)
                {
                    if ((bool)settingsItem.OptionValue)
                    {
                        Configuration.TraceFriendsNews = true;
                    }
                    else
                    {
                        Configuration.TraceFriendsNews = false;
                    }
                }
                #endregion

                #region TraceFriendsPhotos
                if (settingsItem.OptionName == Resources.Settings_FriendsPhotos)
                {
                    if ((bool)settingsItem.OptionValue)
                    {
                        Configuration.TraceFriendsPhotos = true;
                    }
                    else
                    {
                        Configuration.TraceFriendsPhotos = false;
                    }
                }
                #endregion

                #region TraceWallMessages
                if (settingsItem.OptionName == Resources.Settings_WallMessages)
                {
                    if ((bool)settingsItem.OptionValue)
                    {
                        Configuration.TraceWallMessages = true;
                    }
                    else
                    {
                        Configuration.TraceWallMessages = false;
                    }
                }
                #endregion
            }

        }

        /// <summary>
        /// This method indicates that something has been changed in the view.
        /// </summary>
        /// <param name="key">The string key to identify what has been changed.</param>
        protected override void OnViewStateChanged(string key)
        {
            if (key == "LoadSettings")
            {
                Configuration.LoadConfigSettings();
            }

            #region RefreshSettings
            if (key == "RefreshSettings")
            {
                var settings = new List<SettingsListViewItems>();

                #region Автообновление
                /*
                settings.Add(new SettingsListViewItems
                {
                    GroupName = Resources.SettingsView_GroupHeader,
                    OptionName = Resources.AutoUpdateAtStart,
                    OptionType = SettingsKineticControlOptionType.CheckBox,
                    OptionValue = Configuration.AutoUpdateAtStart,
                });
                 */
                #endregion

                //Загрузка данных
                #region Обновление данных
                string dataRenew = "";
                switch (Configuration.DataRenewType)
                {
                    case DataRenewVariants.DontUpdate:
                        dataRenew = Resources.DontUpdate;
                        break;
                    case DataRenewVariants.AutoUpdateAtStart:
                        dataRenew = Resources.AutoUpdateAtStart;
                        break;
                    case DataRenewVariants.UpdateAlways:
                        dataRenew = Resources.UpdateAlways;
                        break;
                }
                settings.Add(new SettingsListViewItems
                                 {
                                     GroupName = Resources.SettingsView_GroupHeader,
                                     OptionName = Resources.DataRefreshing,
                                     OptionType = SettingsKineticControlOptionType.MultiValue,
                                     OptionValue = dataRenew,
                                     Tag =
                                         (EventHandler)
                                         (delegate
                                              {
                                                  using (new WaitWrapper(false))
                                                  {
                                                      MasterForm.Navigate<MultiValueSettingsController>(
                                                          "DataRefreshing");
                                                  }
                                              })
                                 }
                    );
                #endregion
                #region Только Wi-Fi

                settings.Add(new SettingsListViewItems
                {
                    GroupName = Resources.SettingsView_GroupHeader,
                    OptionName = Resources.SettingsView_OnlyWiFi,
                    OptionType = SettingsKineticControlOptionType.CheckBox,
                    OptionValue = Configuration.OnlyWiFi
                }
                    );

                #endregion
                #region Обновлять в роуминге

                settings.Add(new SettingsListViewItems
                {
                    GroupName = Resources.SettingsView_GroupHeader,
                    OptionName = Resources.RenewInRoam,
                    OptionType = SettingsKineticControlOptionType.CheckBox,
                    OptionValue = Configuration.RenewInRoam
                }
                    );

                #endregion

                //Отправка фотографий
                #region Разрешение изображений

                string imageMaxSize = "";
                switch (Configuration.ImageMaxSize)
                {
                    case ImageMaxSizeTypes.DoNotChange:
                        imageMaxSize = Resources.Settings_DoNotChange;
                        break;
                    case ImageMaxSizeTypes.Res640X480:
                        imageMaxSize = Resources.Settings_Res640X480;
                        break;
                }
                settings.Add(new SettingsListViewItems
                {
                    GroupName = Resources.SettingsView_GroupHeader_Photos,
                    OptionName = Resources.ImageMaxSize,
                    OptionType = SettingsKineticControlOptionType.MultiValue,
                    OptionValue = imageMaxSize,
                    Tag =
                        (EventHandler)
                        (delegate
                        {
                            using (new WaitWrapper(false))
                            {
                                MasterForm.Navigate<MultiValueSettingsController>("ImageMaxSize");
                            }
                        }),
                }
                    );

                #endregion

                //Уведомления
                #region Частота обновления
                // Нотификатор отключён по просьбе пользователей - сильно садит батарейку и кушает много траффика
                /*
                string backgroundNotification = "";
                switch (Configuration.BackgroundNotification)
                {
                    //case BackgroundNotificationTypes.OnStart:
                    //  backgroundNotification = Resources.SettingsView_WhileStatrted;
                    //break;
                    case BackgroundNotificationTypes.Off:
                        backgroundNotification = Resources.Settings_Off;
                        break;
                    case BackgroundNotificationTypes.In5Min:
                        backgroundNotification = Resources.Settings_In5Min;
                        break;
                    case BackgroundNotificationTypes.In10Min:
                        backgroundNotification = Resources.Settings_In10Min;
                        break;
                    case BackgroundNotificationTypes.InHour:
                        backgroundNotification = Resources.Settings_InHour;
                        break;
                }
                settings.Add(new SettingsListViewItems
                {
                    GroupName = Resources.SettingsView_GroupHeader_Notifications,
                    OptionName = Resources.BackgroundNotification,
                    OptionType = SettingsKineticControlOptionType.MultiValue,
                    OptionValue = backgroundNotification,
                    Tag =
                        (EventHandler)
                        (delegate
                        {
                            using (new WaitWrapper(false))
                            {
                                MasterForm.Navigate<MultiValueSettingsController>(
                                    "BackgroundNotification");
                            }
                        }),
                }
                    );
                */
                #endregion

                ViewData["Settings"] = settings;
                view.UpdateView("SettingsLoaded");
            }
            #endregion

            #region ApplyAndSaveSettings
            if (key == "ApplyAndSaveSettings")
            {
                var settings = (List<SettingsListViewItems>)ViewData["Settings"];

                foreach (var settingsItem in settings)
                {
                    #region AutoUpdateAtStart
                    /*
                    if (settingsItem.OptionName == Resources.AutoUpdateAtStart)
                    {
                        if ((bool)settingsItem.OptionValue)
                        {
                            Configuration.AutoUpdateAtStart = true;
                        }
                        else
                        {
                            Configuration.AutoUpdateAtStart = false;
                        }
                    }
                    */
                    #endregion

                    #region UpdateFriendsStatus
                    if (settingsItem.OptionName == Resources.UpdateFriendsStatus)
                    {
                        if ((bool)settingsItem.OptionValue)
                        {
                            Configuration.UpdateFriendsStatus = true;
                        }
                        else
                        {
                            Configuration.UpdateFriendsStatus = false;
                        }
                    }
                    #endregion

                    #region RenewInRoam
                    if (settingsItem.OptionName == Resources.RenewInRoam)
                    {
                        if ((bool)settingsItem.OptionValue)
                        {
                            Configuration.RenewInRoam = true;
                        }
                        else
                        {
                            Configuration.RenewInRoam = false;
                        }
                    }
                    #endregion

                    #region WiFi
                    if (settingsItem.OptionName == Resources.SettingsView_OnlyWiFi)
                    {
                        if ((bool)settingsItem.OptionValue)
                        {
                            Configuration.OnlyWiFi = true;
                        }
                        else
                        {
                            Configuration.OnlyWiFi = false;
                        }
                    }
                    #endregion

                    /*#region NotificateFriendsBirthdays
                    if (settingsItem.OptionName == Resources.NotificateFriendsBirthdays)
                    {
                        if ((bool)settingsItem.OptionValue)
                        {
                            Configuration.NotificateFriendsBirthdays = true;
                        }
                        else
                        {
                            Configuration.NotificateFriendsBirthdays = false;
                        }
                    }
                    #endregion*/

                    #region TraceMessagess
                    if (settingsItem.OptionName == Resources.Settings_Messages)
                    {
                        if ((bool)settingsItem.OptionValue)
                        {
                            Configuration.TraceMessagess = true;
                        }
                        else
                        {
                            Configuration.TraceMessagess = false;
                        }
                    }
                    #endregion

                    #region TraceComments
                    if (settingsItem.OptionName == Resources.Settings_Comments)
                    {
                        if ((bool)settingsItem.OptionValue)
                        {
                            Configuration.TraceComments = true;
                        }
                        else
                        {
                            Configuration.TraceComments= false;
                        }
                    }
                    #endregion

                    #region TraceFriends
                    if (settingsItem.OptionName == Resources.Settings_Friends)
                    {
                        if ((bool)settingsItem.OptionValue)
                        {
                            Configuration.TraceFriends= true;
                        }
                        else
                        {
                            Configuration.TraceFriends = false;
                        }
                    }
                    #endregion

                    #region TraceFriendsNews
                    if (settingsItem.OptionName == Resources.Settings_FriendsNews)
                    {
                        if ((bool)settingsItem.OptionValue)
                        {
                            Configuration.TraceFriendsNews = true;
                        }
                        else
                        {
                            Configuration.TraceFriendsNews = false;
                        }
                    }
                    #endregion

                    #region TraceFriendsPhotos
                    if (settingsItem.OptionName == Resources.Settings_FriendsPhotos)
                    {
                        if ((bool)settingsItem.OptionValue)
                        {
                            Configuration.TraceFriendsPhotos = true;
                        }
                        else
                        {
                            Configuration.TraceFriendsPhotos = false;
                        }
                    }
                    #endregion

                    #region TraceWallMessages
                    if (settingsItem.OptionName == Resources.Settings_WallMessages)
                    {
                        if ((bool)settingsItem.OptionValue)
                        {
                            Configuration.TraceWallMessages = true;
                        }
                        else
                        {
                            Configuration.TraceWallMessages = false;
                        }
                    }
                    #endregion
                }

                #region Старт/стоп нотификатора

                if (Configuration.BackgroundNotification != BackgroundNotificationTypes.Off)
                {
                    Globals.BaseLogic.IDataLogic.SetNtfAutorun();
                    //проверить изменился ли NotificationTimer, если да, то перезапустить нотификатор
                    string timer = Globals.BaseLogic.IDataLogic.GetNotificationTimer();
                    if ((Configuration.BackgroundNotification == BackgroundNotificationTypes.In5Min &&
                        timer != "300000") ||
                        (Configuration.BackgroundNotification == BackgroundNotificationTypes.In10Min &&
                        timer != "600000") ||
                        (Configuration.BackgroundNotification == BackgroundNotificationTypes.InHour &&
                        timer != "3600000"))
                    {
                        OnViewStateChanged("StopNotificator");
                    }

                    OnViewStateChanged("StartNotificator");
                }
                else
                {
                    OnViewStateChanged("StopNotificator");
                    Globals.BaseLogic.IDataLogic.DelNtfAutorun();
                }

                #endregion

                Configuration.SaveConfigSettings();
                NavigationService.GoBack();
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

            #region StartNotificator
            if (key == "StartNotificator")
            {
                if (Globals.BaseLogic.IDataLogic.GetToken() != "")
                {
                    using (new WaitWrapper(false))
                    {
                        ////Запуск службы нотификатора
                        //try
                        //{
                        //    if (!Interprocess.IsServiceRunning) Interprocess.StartService();
                        //}
                        //catch (Exception ex)
                        //{
                        //    //Ошибка при запуске нотификатора
                        //    DebugHelper.WriteLogEntry(ex, "Notificator start error");
                        //}
                    }
                }
                else
                    DialogControl.ShowQuery(Resources.MainView_Button_NotificatorCantStart, DialogButtons.OK);
                //MessageBox.Show(Resources.MainView_Button_NotificatorCantStart);
            }
            #endregion

            #region StopNotificator
            if (key == "StopNotificator")
            {
                using (new WaitWrapper())
                {
                    try
                    {
                        //Interprocess.StopService();
                        //int cnt = 0;
                        //while (Interprocess.IsServiceRunning && cnt < 6)
                        //{
                        //    System.Threading.Thread.Sleep(500);
                        //    cnt++;
                        //}
                    }
                    catch (Exception ex)
                    {
                        //Ошибка при остановке нотификатора
                        DebugHelper.WriteLogEntry(ex, "Notificator stop error");
                    }
                }
            }
            #endregion
        }

        #endregion
    }
}
