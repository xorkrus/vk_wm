using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace Galssoft.VKontakteWM.Notification.ServiceClasses
{
    /// <summary>
    /// Controls the behaviour of a specific notification class, e.g. new SMS or IM
    /// </summary>
    public class NotificationClass
    {
        private const string NotificationsKeyName = "ControlPanel\\Notifications";

        /// <summary>
        /// User-visible notification class name. 
        /// If set to null, the notification class will not appear in Settings.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Actions to take on notification event.
        /// </summary>
        public NotificationOptions Options { get; set; }
        /// <summary>
        /// Message display duration
        /// </summary>
        public int Duration { get; set; }
        /// <summary>
        /// Wave file to be played upon the notification event
        /// </summary>
        public string WaveFile { get; set; }

        /// <summary>
        /// Get the notification class options
        /// </summary>
        /// <param name="clsid">The notification class ID</param>
        public static NotificationClass Get(Guid clsid)
        {
            return Get(clsid.ToString("B"));
        }

        /// <summary>
        /// Get the default notification class options
        /// </summary>
        public static NotificationClass Default
        {
            get { return Get("Default"); }
            set { Set("Default", value); }
        }

        public static IEnumerable<Guid> Enumerate()
        {
            RegistryKey notificationsKey = Registry.CurrentUser.OpenSubKey(NotificationsKeyName);
            foreach (string subKeyName in notificationsKey.GetSubKeyNames())
            {
                Guid notificationID;
                try
                {
                    notificationID = new Guid(subKeyName);
                }
                catch (FormatException) { continue; }
                catch (OverflowException) { continue; }
                yield return notificationID;
            }
        }

        /// <summary>
        /// Modify the notification class options
        /// </summary>
        /// <param name="clsid">The notification class ID</param>
        /// <param name="cls">Options to set</param>
        public static void Set(Guid clsid, NotificationClass cls)
        {
            string clsidStr = clsid.ToString("B");
            Set(clsidStr, cls);
        }

        /// <summary>
        /// Delete a notification class
        /// </summary>
        /// <param name="clsid">The notification class ID</param>
        public static void Delete(Guid clsid)
        {
            Registry.CurrentUser.DeleteSubKey(ClsidKey(clsid.ToString("B")));
        }

        private static NotificationClass Get(string clsidStr)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(ClsidKey(clsidStr), false);
            if (key == null)
                return null;
            NotificationClass result = new NotificationClass();
            result.Name = key.GetValue(null) as string;
            object optionsValue = key.GetValue("Options");
            if (optionsValue == null)
                result.Options = 0;
            else
                result.Options = (NotificationOptions)int.Parse(optionsValue.ToString());
            result.Duration = (int)(key.GetValue("Duration") ?? 0);
            result.WaveFile = key.GetValue("Wave") as string;
            return result;
        }

        private static void Set(string clsidStr, NotificationClass cls)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(ClsidKey(clsidStr), true);
            if (key == null)
                key = Registry.CurrentUser.CreateSubKey(ClsidKey(clsidStr));
            SetDeleteValue(key, null, cls.Name);
            key.SetValue("Options", (int)cls.Options);
            key.SetValue("Duration", cls.Duration);
            SetDeleteValue(key, "Wave", cls.WaveFile);
        }

        private static void SetDeleteValue(RegistryKey key, string valueName, string value)
        {
            if (value == null)
            {
                if (key.GetValue(valueName) != null)
                    key.DeleteValue(valueName);
            }
            else
                key.SetValue(valueName, value);
        }

        private static string ClsidKey(string clsidStr)
        {
            return string.Format("{1}\\{0}", clsidStr, NotificationsKeyName);
        }
    }
}
