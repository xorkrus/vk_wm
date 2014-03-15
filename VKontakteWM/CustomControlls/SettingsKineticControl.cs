using System;
using System.Collections.Generic;
using System.Drawing;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.Properties;

namespace Galssoft.VKontakteWM.CustomControlls
{
    class SettingsKineticControl : SettingsListView
    {
        #region Constructors

        public SettingsKineticControl()
            : base(new KineticControlsSetting
            {
                ListItemPixSize = 32,
                PrimaryTextFontGdi = FontCache.CreateFont("Tahoma", 12, FontStyle.Regular, true),
                SecondaryTextFontGdi = FontCache.CreateFont("Tahoma", 12, FontStyle.Regular, true),
                GroupPixTopMargin = 2,
                GroupPixHeight = 18
            })
        {
            ShowGroupHeader = true;
            //EnableLongPress = true;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Источник данных для списка
        /// </summary>
        public List<SettingsListViewItems> DataSource
        {
            get;
            set;
        }

        #endregion

        #region Protected Methods

        protected override List<NativeItemData> BuildNativeControlItems()
        {
            List<NativeItemData> result = new List<NativeItemData>();

            foreach (SettingsListViewItems element in Items)
            {
                ExNativeItemData item = null;

                if (element.OptionType == SettingsKineticControlOptionType.CheckBox)
                {
                    item = new CheckBoxNativeItemData();
                    ((CheckBoxNativeItemData)item).CheckBoxEmpty = MasterForm.SkinManager.GetImage("SettingsUnchecked");
                    ((CheckBoxNativeItemData)item).CheckBoxSelected = MasterForm.SkinManager.GetImage("SettingsChecked");
                }
                else if (element.OptionType == SettingsKineticControlOptionType.MultiValue)
                {
                    item = new MultiItemsNativeItemData();
                    ((MultiItemsNativeItemData)item).SelectButton = MasterForm.SkinManager.GetImage("SettingsEnum");
                    item.Font = Settings.SecondaryTextFontGdi;
                    ((MultiItemsNativeItemData)item).Select += (EventHandler)element.Tag;
                }
                else if (element.OptionType == SettingsKineticControlOptionType.TwoButtonValue)
                {
                    item = new TwoItemsNativeItemData();
                    ((TwoItemsNativeItemData)item).ItemLeft = MasterForm.SkinManager.GetImage(/*"LeftDeselectedSettingButton"*/"ButtonOther");
                    ((TwoItemsNativeItemData)item).ItemLeftSelected = MasterForm.SkinManager.GetImage(/*"LeftSelectedSettingButton"*/"ButtonOther");
                    ((TwoItemsNativeItemData)item).ItemRight = MasterForm.SkinManager.GetImage(/*"RightDeselectedSettingButton"*/"ButtonOther");
                    ((TwoItemsNativeItemData)item).ItemRightSelected = MasterForm.SkinManager.GetImage(/*"RightSelectedSettingButton"*/"ButtonOther");
                    item.Font = FontCache.CreateFont("Tahoma", 14, FontStyle.Bold, true);
                }

                if (item != null)
                {
                    item.InfoLeftIndents = new int[1];
                    item.InfoTopIndents = new int[1];
                    item.InfoLeftIndents[0] = UISettings.CalcPix(10);
                    item.InfoTopIndents[0] = UISettings.CalcPix(10);

                    item.WorkAreaWidth = UISettings.CalcPix(100);
                    item.PrimaryText = element.OptionName;
                    item.DataSource = element;
                    item.Group = element.GroupName;
                    result.Add(item);
                }
            }

            return result;
        }

        protected override void GenerateListItems()
        {
            Items.Clear();

            if (DataSource != null)
                Items.AddRange(DataSource);
        }

        protected override void OnItemSelected(int index, Point mousePosition)
        {
            if (index >= 0)
            {
                //Реализация disable элементов настроек
                bool isNotifficationOff = false;
                foreach (var item in Items)
                {
                    if (item.OptionName == Resources.BackgroundNotification &&
                        (string) item.OptionValue == Resources.Settings_Off)
                        isNotifficationOff = true;
                }

                if (isNotifficationOff)
                {
                    //Если BackgroundNotification off, то нельзя менять настройки всех событий
                    if (Items[index].OptionName != Resources.Settings_Messages &&
                        Items[index].OptionName != Resources.Settings_Comments &&
                        Items[index].OptionName != Resources.Settings_Friends &&
                        Items[index].OptionName != Resources.Settings_FriendsNews &&
                        Items[index].OptionName != Resources.Settings_FriendsPhotos &&
                        Items[index].OptionName != Resources.Settings_WallMessages)
                        base.OnItemSelected(index, mousePosition);
                }
                else
                    //Сообщения - всегда нельзя менять
                    if (Items[index].OptionName != Resources.Settings_Messages)
                        base.OnItemSelected(index, mousePosition);
            }
        }

        protected override void CalculateItemsSize()
        {
            SetStartPositions(NativeItems);
        }

        private void SetStartPositions(List<NativeItemData> nid)
        {
            int c = 0;

            StartPositions.Clear();

            if (ItemCount == 0)
            {
                _ActiveListHeight = 0;
                _ActiveListWidth = 0;
                return;
            }

            for (int i = 0; i < ItemCount; i++)
            {
                StartPositions.Add(c);
                if (ShowGroupHeader)
                    //if (IsItemNewGroup(nItem) && item.Group != null && rItem.Top >= rListRect.Top)
                    if (IsItemNewGroup(i) && nid[i].Group != null)
                    {
                        c += Settings.GroupPixHeight;
                    }

                c += Settings.ListItemPixSize;
            }

            StartPositions.Add(c); //The end of tha last item

            _ActiveListHeight = c;

            ScrollTo(0);
        }

        #endregion

        #region Drawing

        protected override void DrawItemOn(Gdi g, NativeItemData nativeItem, Rectangle rItem, int nItem)
        {
            //Дефолтный цвет для Enabled элементов
            g.TextColor = Settings.ListItemTextColor;

            #region Отображение disabled элементов
            bool isNotifficationOff = false;
            foreach (var item in Items)
            {
                if (item.OptionName == nativeItem.PrimaryText)
                {
                    //Выясняем стоит ли BackgroundNotification в Off
                    foreach (var item2 in Items)
                    {
                        if (item2.OptionName == Resources.BackgroundNotification &&
                        (string)item2.OptionValue == Resources.Settings_Off)
                            isNotifficationOff = true;
                    }

                    if (item.OptionName == Resources.Settings_Messages)
                    {
                        //((CheckBoxNativeItemData) nativeItem).CheckBoxSelected = MasterForm.SkinManager.GetImage("SettingsUnavailable");
                        g.TextColor = Color.FromArgb(213, 213, 213);
                    }
                    if (item.OptionName == Resources.Settings_Comments && isNotifficationOff)
                    {
                        //((CheckBoxNativeItemData) nativeItem).CheckBoxSelected = MasterForm.SkinManager.GetImage("SettingsUnavailable");
                        g.TextColor = Color.FromArgb(213, 213, 213);
                    }
                    if (item.OptionName == Resources.Settings_Friends && isNotifficationOff)
                    {
                        //((CheckBoxNativeItemData) nativeItem).CheckBoxSelected = MasterForm.SkinManager.GetImage("SettingsUnavailable");
                        g.TextColor = Color.FromArgb(213, 213, 213);
                    }
                    if (item.OptionName == Resources.Settings_FriendsNews && isNotifficationOff)
                    {
                        //((CheckBoxNativeItemData) nativeItem).CheckBoxSelected = MasterForm.SkinManager.GetImage("SettingsUnavailable");
                        g.TextColor = Color.FromArgb(213, 213, 213);
                    }
                    if (item.OptionName == Resources.Settings_FriendsPhotos && isNotifficationOff)
                    {
                        //((CheckBoxNativeItemData) nativeItem).CheckBoxSelected = MasterForm.SkinManager.GetImage("SettingsUnavailable");
                        g.TextColor = Color.FromArgb(213, 213, 213);
                    }
                    if (item.OptionName == Resources.Settings_WallMessages && isNotifficationOff)
                    {
                        //((CheckBoxNativeItemData) nativeItem).CheckBoxSelected = MasterForm.SkinManager.GetImage("SettingsUnavailable");
                        g.TextColor = Color.FromArgb(213, 213, 213);
                    }
                }
            }
            #endregion

            if (rItem.Height > Settings.ListItemPixSize)
                rItem = new Rectangle(rItem.Left,
                                      rItem.Top + rItem.Height - Settings.ListItemPixSize,
                                      rItem.Width,
                                      Settings.ListItemPixSize);
            //Не надо делать выделение
            DrawItemBackgroundOn(g, nativeItem, rItem, nItem, /*nItem == SelectedIndex*/ false);

            var rSep = new Rectangle(rItem.Left, rItem.Bottom - 1, rItem.Width, 1);
            g.FillRect(rSep, Settings.ListItemSeparator);

            // write name
            if (!string.IsNullOrEmpty(nativeItem.PrimaryText))
            {
                g.Font = Settings.PrimaryTextFontGdi;
                g.TextAlign = Win32.TextAlign.TA_LEFT;
                g.ExtTextOut(rItem.Left + nativeItem.InfoLeftIndents[0],
                             rItem.Top + nativeItem.InfoTopIndents[0],
                             nativeItem.PrimaryText);
            }

            base.DrawItemOn(g, nativeItem, rItem, nItem);
        }

        protected override void DrawGroupHeaderOn(Gdi gMem, int item, Rectangle rHeaderArea)
        {
            //// Draw Group Header Text
            //gMem.Font = Settings.GroupFont;
            //gMem.TextAlign = Win32.TextAlign.TA_RIGHT;
            //gMem.TextColor = Settings.ListGroupTextColor;

            //Size newSize = gMem.GetTextExtent(GetItemAtIndex(item).Group);

            //gMem.ExtTextOut(rHeaderArea.Right - UISettings.CalcPix(10), rHeaderArea.Top + (rHeaderArea.Height - newSize.Height) / 2, GetItemAtIndex(item).Group, true);

            //gMem.ExtTextOut(rHeaderArea.Left + Settings.GroupPixTopMargin * 3,
            //                rHeaderArea.Top,
            //                GetItemAtIndex(item).Group, true);

            //gMem.ExtTextOut(rHeaderArea.Right - UISettings.CalcPix(10),
            //    rHeaderArea.Top + Settings.GroupPixTopMargin * 2,
            //    GetItemAtIndex(item).Group, true);

            gMem.Font = Settings.GroupFont;
            gMem.TextAlign = Win32.TextAlign.TA_RIGHT;

            Size newSize = gMem.GetTextExtent(GetItemAtIndex(item).Group);

            gMem.TextColor = Color.FromArgb(150, 150, 150);
            gMem.ExtTextOut(rHeaderArea.Right - UISettings.CalcPix(10) + UISettings.CalcPix(1), rHeaderArea.Top + (rHeaderArea.Height - newSize.Height) / 2 + UISettings.CalcPix(1), GetItemAtIndex(item).Group);

            gMem.TextColor = Settings.ListGroupTextColor;
            gMem.ExtTextOut(rHeaderArea.Right - UISettings.CalcPix(10), rHeaderArea.Top + (rHeaderArea.Height - newSize.Height) / 2, GetItemAtIndex(item).Group);
        }

        #endregion
    }
}
