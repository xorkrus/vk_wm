using System.Collections.Generic;
using System.Drawing;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;
using Galssoft.VKontakteWM.Forms;

namespace Galssoft.VKontakteWM.CustomControlls
{
    class MultiValueSettingsKineticControl : SettingsListView
    {
        #region Constructors

        public MultiValueSettingsKineticControl()
            : base(new KineticControlsSetting
            {
                ListItemPixSize = 32,
                PrimaryTextFontGdi = FontCache.CreateFont("Tahoma", 12, FontStyle.Regular, true),
                SecondaryTextFontGdi = FontCache.CreateFont("Tahoma", 10, FontStyle.Regular, true),
                GroupPixTopMargin = 2,
                GroupPixHeight = 2
            })
        {
            //ShowGroupHeader = true;
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
                    ((CheckBoxNativeItemData)item).CheckBoxEmpty = MasterForm.SkinManager.GetImage("MultiValueCheckNone");
                    ((CheckBoxNativeItemData)item).CheckBoxSelected = MasterForm.SkinManager.GetImage("MultiValueCheck");
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
                foreach (var item in Items)
                {
                    item.OptionValue = false;
                }

                Items[index].OptionValue = true;
            }

            //base.OnItemSelected(index, mousePosition);
        }

        #endregion

        #region Drawing

        protected override void DrawItemOn(Gdi g, NativeItemData nativeItem, Rectangle rItem, int nItem)
        {
            if (rItem.Height > Settings.ListItemPixSize)
                rItem = new Rectangle(rItem.Left,
                                      rItem.Top + rItem.Height - Settings.ListItemPixSize,
                                      rItem.Width,
                                      Settings.ListItemPixSize);

            DrawItemBackgroundOn(g, nativeItem, rItem, nItem, /*nItem == SelectedIndex*/ false);

            var rSep = new Rectangle(rItem.Left, rItem.Bottom - 1, rItem.Width, 1);
            g.FillRect(rSep, Settings.ListItemSeparator);

            // write name
            if (!string.IsNullOrEmpty(nativeItem.PrimaryText))
            {
                g.Font = Settings.PrimaryTextFontGdi;
                g.TextAlign = Win32.TextAlign.TA_LEFT;
                g.TextColor = Settings.ListItemTextColor;
                g.ExtTextOut(rItem.Left + nativeItem.InfoLeftIndents[0],
                    rItem.Top + nativeItem.InfoTopIndents[0],
                    nativeItem.PrimaryText);
            }

            base.DrawItemOn(g, nativeItem, rItem, nItem);
        }
        #endregion
    }
}