namespace Galssoft.VKontakteWM.Notification.ServiceClasses
{
    /// <summary>
    /// Actions that could be taken on a notification event
    /// </summary>
    public enum NotificationOptions
    {
        /// <summary>
        /// Play a sound
        /// </summary>
        Sound = 1,
        /// <summary>
        /// Vibrate
        /// </summary>
        Vibrate = 2,
        /// <summary>
        /// Flash the device's LED
        /// </summary>
        Flash = 4,
        /// <summary>
        /// Display a message on the screen
        /// </summary>
        Message = 8,
        /// <summary>
        /// Repeat the played sound
        /// <remarks>Undocumented.</remarks>
        /// </summary>
        RepeatSound = 0x10,
        /// <summary>
        /// Enable the "repeat sound" check box in notification settings dialog
        /// <remarks>Undocumented.</remarks>
        /// </summary>
        EnableRepeatSoundCheckBox = 0x40000000,
        // 3407931 = 0x34003B, seen somewhere but does nothing obvious
        // 3276841 = 0x320029, same
        // 0x80000000 - seen for a phone call, does nothing
    }
}
