using System.Collections.Generic;

namespace Galssoft.VKontakteWM.Components.UI.CompoundControls
{
    public enum SettingsKineticControlOptionType
    {
        CheckBox,
        TwoButtonValue,
        MultiValue
    }

    /// <summary>
    /// Описание элементов настроек SettingsList
    /// </summary>
    public class SettingsListViewItems
    {
        public string OptionName { get; set; }
        public SettingsKineticControlOptionType OptionType { get; set; }
        public object OptionValue { get; set; }
        public List<string> OptionValues { get; set; }
        public object Tag { get; set; }
        public string GroupName;
    }
}
