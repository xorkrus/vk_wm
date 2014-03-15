using System.Collections.Generic;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Properties;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    class MultiValueSettingsController : Controller
    {
        #region Constructors

        public MultiValueSettingsController()
            : base(new MultiValueSettingsView())
        {
            Name = "MultiValueSettingsController";
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

        /// <summary>
        /// This method indicates that something has been changed in the view.
        /// </summary>
        /// <param name="key">The string key to identify what has been changed.</param>
        protected override void OnViewStateChanged(string key)
        {
            #region LoadBackgroundNotificationSettings
            if (key == "LoadBackgroundNotificationSettings")
            {
                //Изменить текст
                ViewData["SettingsText"] = Resources.BackgroundNotification;
                view.UpdateView("ChangeText");

                var settings = new List<SettingsListViewItems>();

                settings.Add(new SettingsListViewItems
                {
                    OptionName = Resources.Settings_Off,
                    OptionType = SettingsKineticControlOptionType.CheckBox,
                    OptionValue =
                        (Configuration.BackgroundNotification == BackgroundNotificationTypes.Off),
                });
                /*
                settings.Add(new SettingsListViewItems
                {
                    OptionName = Resources.SettingsView_WhileStatrted,
                    OptionType = SettingsKineticControlOptionType.CheckBox,
                    OptionValue =
                        (Configuration.BackgroundNotification == BackgroundNotificationTypes.OnStart),
                });
                */
                settings.Add(new SettingsListViewItems
                {
                    OptionName = Resources.Settings_In5Min,
                    OptionType = SettingsKineticControlOptionType.CheckBox,
                    OptionValue =
                        (Configuration.BackgroundNotification == BackgroundNotificationTypes.In5Min),
                });
                settings.Add(new SettingsListViewItems
                {
                    OptionName = Resources.Settings_In10Min,
                    OptionType = SettingsKineticControlOptionType.CheckBox,
                    OptionValue =
                        (Configuration.BackgroundNotification == BackgroundNotificationTypes.In10Min),
                });
                settings.Add(new SettingsListViewItems
                {
                    OptionName = Resources.Settings_InHour,
                    OptionType = SettingsKineticControlOptionType.CheckBox,
                    OptionValue =
                        (Configuration.BackgroundNotification == BackgroundNotificationTypes.InHour),
                });

                ViewData["Settings"] = settings;
                view.UpdateView("SettingsLoaded");
            }

            #endregion

            #region LoadImageMaxSizeSettings
            if (key == "LoadImageMaxSizeSettings")
            {
                //Изменить текст
                ViewData["SettingsText"] = Resources.ImageMaxSize;
                view.UpdateView("ChangeText");

                var settings = new List<SettingsListViewItems>();

                settings.Add(new SettingsListViewItems
                {
                    OptionName = Resources.Settings_DoNotChange,
                    OptionType = SettingsKineticControlOptionType.CheckBox,
                    OptionValue =
                        (Configuration.ImageMaxSize == ImageMaxSizeTypes.DoNotChange),
                });
                settings.Add(new SettingsListViewItems
                {
                    OptionName = Resources.Settings_Res640X480,
                    OptionType = SettingsKineticControlOptionType.CheckBox,
                    OptionValue =
                        (Configuration.ImageMaxSize == ImageMaxSizeTypes.Res640X480),
                });

                ViewData["Settings"] = settings;
                view.UpdateView("SettingsLoaded");
            }
            #endregion

            #region LoadDataRenewTypeSettings
            if (key == "LoadDataRenewTypeSettings")
            {
                ViewData["SettingsText"] = Resources.DataRefreshing;
                view.UpdateView("ChangeText");

                var settings = new List<SettingsListViewItems>
                                   {
                                       new SettingsListViewItems
                                           {
                                               OptionName = Resources.DontUpdate,
                                               OptionType = SettingsKineticControlOptionType.CheckBox,
                                               OptionValue =
                                                   (Configuration.DataRenewType == DataRenewVariants.DontUpdate)
                                           },
                                       new SettingsListViewItems
                                           {
                                               OptionName = Resources.AutoUpdateAtStart,
                                               OptionType = SettingsKineticControlOptionType.CheckBox,
                                               OptionValue =
                                                   (Configuration.DataRenewType == DataRenewVariants.AutoUpdateAtStart)
                                           },
                                       new SettingsListViewItems
                                           {
                                               OptionName = Resources.UpdateAlways,
                                               OptionType = SettingsKineticControlOptionType.CheckBox,
                                               OptionValue =
                                                   (Configuration.DataRenewType == DataRenewVariants.UpdateAlways)
                                           }
                                   };
                ViewData["Settings"] = settings;
                view.UpdateView("SettingsLoaded");
            }
            #endregion

            #region ApplyAndSaveSettings
            if (key == "SaveSettings")
            {
                var settings = (List<SettingsListViewItems>)ViewData["Settings"];
                foreach (var settingsItem in settings)
                {
                    #region BackgroundNotification
                    if (settingsItem.OptionName == Resources.Settings_Off)
                    {
                        if ((bool)settingsItem.OptionValue)
                        {
                            Configuration.BackgroundNotification = BackgroundNotificationTypes.Off;
                        }
                    }
                    else if (settingsItem.OptionName == Resources.Settings_In5Min)
                    {
                        if ((bool)settingsItem.OptionValue)
                        {
                            Configuration.BackgroundNotification = BackgroundNotificationTypes.In5Min;
                        }
                    }
                    else if (settingsItem.OptionName == Resources.Settings_In10Min)
                    {
                        if ((bool)settingsItem.OptionValue)
                        {
                            Configuration.BackgroundNotification = BackgroundNotificationTypes.In10Min;
                        }
                    }
                    else if (settingsItem.OptionName == Resources.Settings_InHour)
                    {
                        if ((bool)settingsItem.OptionValue)
                        {
                            Configuration.BackgroundNotification = BackgroundNotificationTypes.InHour;
                        }
                    }
                    #endregion

                    #region ImageMaxSize
                    if (settingsItem.OptionName == Resources.Settings_DoNotChange)
                    {
                        if ((bool)settingsItem.OptionValue)
                        {
                            Configuration.ImageMaxSize = ImageMaxSizeTypes.DoNotChange;
                        }
                    }
                    else if (settingsItem.OptionName == Resources.Settings_Res640X480)
                    {
                        if ((bool)settingsItem.OptionValue)
                        {
                            Configuration.ImageMaxSize = ImageMaxSizeTypes.Res640X480;
                        }
                    }
                    #endregion

                    #region DataRenewType
                        if (settingsItem.OptionName == Resources.DontUpdate)
                        {
                            if ((bool) settingsItem.OptionValue)
                                Configuration.DataRenewType = DataRenewVariants.DontUpdate;
                        }
                        else if (settingsItem.OptionName == Resources.AutoUpdateAtStart)
                        {
                            if ((bool) settingsItem.OptionValue)
                                Configuration.DataRenewType = DataRenewVariants.AutoUpdateAtStart;
                        }
                        else if (settingsItem.OptionName == Resources.UpdateAlways)
                        {
                            if ((bool) settingsItem.OptionValue)
                                Configuration.DataRenewType = DataRenewVariants.UpdateAlways;
                        }
                    #endregion
                }

                NavigationService.GoBack();
            }
            #endregion

            #region GoBack
            if (key == "GoBack")
            {
                NavigationService.GoBack();
            }
            #endregion
        }
        protected override void OnInitialize(params object[] parameters)
        {
            if ((parameters != null) && (parameters.Length > 0))
            {
                string param = parameters[0] as string;

                if ((param != null) && (param == "BackgroundNotification"))
                {
                    OnViewStateChanged("LoadBackgroundNotificationSettings");
                }
                if ((param != null) && (param == "ImageMaxSize"))
                {
                    OnViewStateChanged("LoadImageMaxSizeSettings");
                }
                if ((param != null) && (param == "DataRefreshing"))
                {
                    OnViewStateChanged("LoadDataRenewTypeSettings");
                }
            }
        }

        #endregion
    }
}