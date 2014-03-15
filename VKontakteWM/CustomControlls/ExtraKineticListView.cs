using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.Properties;

namespace Galssoft.VKontakteWM.CustomControlls
{
    class ExtraKineticListView : KineticListView<ExtraButton>
    {
        protected class NativeItemDataMm : NativeItemData
        {
            public IImage Icon { get; set; }
            public new Bitmap PrimaryImage { get; set; }
        }

        #region Constructors

        public ExtraKineticListView()
            : base(KineticControlScrollType.Vertical, new KineticControlsSetting
            {
                SecondaryIconPixWidth = 36,
                ListItemPixSize = 37,
                PrimaryIconPixWidth = 24,
                PrimaryIconPixHeight = 24
            })
        {
            ShowScrollbar = false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Источник данных для списка
        /// </summary>
        public List<ExtraButton> DataSource
        {
            get;
            set;
        }

        protected override string IfEmptyText
        {
            get
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// The menu
        /// </summary>
        public MainMenu ViewMenu
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region Protected Methods

        protected override List<NativeItemData> BuildNativeControlItems()
        {
            var retVal = new List<NativeItemData>();
            foreach (var item in Items)
            {
                var data = new NativeItemDataMm
                {
                    PrimaryText = item.Text,
                    SecondaryText = item.Action.ToString(),
                    Icon = item.Icon
                };
                retVal.Add(data);
            }
            return retVal;
        }

        protected override void GenerateListItems()
        {
            Items.Add(new ExtraButton(ExtraActions.UserData, Resources.ExtraView_UserDataButtonTitle, MasterForm.SkinManager.GetImage("OptionsProfile")));
            Items.Add(new ExtraButton(ExtraActions.Settings, Resources.ExtraView_SettingsButtonTitle, MasterForm.SkinManager.GetImage("OptionsApp")));
            Items.Add(new ExtraButton(ExtraActions.About, Resources.ExtraView_AboutButtonTitle, MasterForm.SkinManager.GetImage("OptionAbout")));
            Items.Add(new ExtraButton(ExtraActions.Help, Resources.ExtraView_HelpButtonTitle, MasterForm.SkinManager.GetImage("OptionsHelp")));
            Items.Add(new ExtraButton(ExtraActions.Exit, Resources.ExtraView_ExitButtonTitle, MasterForm.SkinManager.GetImage("OptionsHelp")));
        }

        private Bitmap _paintedButton;

        protected override void CalculateItemsSize()
        {
            int offset = UISettings.CalcPix(5);
            CalculateItemSizeForListSize(Settings.ListItemPixSize, null);
            if (StartPositions.Count > 0)
                for (int i = 1; i < StartPositions.Count; i++)
                {
                    StartPositions[i] = StartPositions[i] + offset;
                }
        }

        protected override void DrawItemOn(Gdi g, NativeItemData nativeitem, Rectangle rItem, int nItem)
        {
            var item = (NativeItemDataMm)nativeitem;

            //if (nItem == ItemCount - 1)
            //    rItem = new Rectangle(rItem.Left, rItem.Top, rItem.Width, rItem.Height);
            //else


            bool isSelected = ShowSelectedItem ? nItem == _SelectedIndex : false;

            DrawItemBackgroundOn(g, item, rItem, nItem, isSelected);

            if (nItem == 0)
                rItem = new Rectangle(rItem.Left, rItem.Top + UISettings.CalcPix(5), rItem.Width, rItem.Height - UISettings.CalcPix(10));
            else
                rItem = new Rectangle(rItem.Left, rItem.Top, rItem.Width, rItem.Height - UISettings.CalcPix(5));

            int iconMargin = UISettings.CalcPix(15);
            int iconX = rItem.Left + iconMargin;

            //Для кнопки выход не рисуем иконку
            /*
            if (item.PrimaryText != Resources.ExtraView_ExitButtonTitle)
            {
                // draw icon
                if (item.Icon != null)
                {
                    ImageInfo iinfo;
                    item.Icon.GetImageInfo(out iinfo);
                    int iconY = rItem.Top + (rItem.Height - (int) iinfo.Height)/2;

                    if (item.PrimaryImage == null || isSelected)
                    {
                        g.DrawImageAlphaChannel(item.Icon, iconX, iconY);

                        if (item.PrimaryImage == null & iconY > 0)
                        {
                            item.PrimaryImage = new Bitmap((int) iinfo.Width, (int) iinfo.Height);
                            g.CopyImageTo(item.PrimaryImage, 0, 0, Settings.PrimaryIconPixWidth,
                                          Settings.PrimaryIconPixHeight, iconX, iconY);
                        }
                    }
                    else
                    {
                        g.DrawImage(item.PrimaryImage, iconX, iconY);
                    }
                }
            }
             */

            if (!String.IsNullOrEmpty(item.PrimaryText))
            {
                g.Font = FontCache.CreateFont("Tahoma", 16, FontStyle.Regular, true);


                g.TextAlign = Win32.TextAlign.TA_LEFT;
                Size textSize = g.GetTextExtent(item.PrimaryText);

                //Для кнопки выход текст по центру
                if (item.PrimaryText == Resources.ExtraView_ExitButtonTitle)
                {
                    if (isSelected)
                        g.TextColor = Color.FromArgb(255, 255, 255);
                    else
                        g.TextColor = Color.FromArgb(145, 145, 145);
                    g.ExtTextOut(rItem.Left + (rItem.Width - textSize.Width) / 2, rItem.Top + (rItem.Height - textSize.Height) / 2,
                                 item.PrimaryText);
                }
                else
                {
                    if (isSelected)
                        g.TextColor = Color.FromArgb(255, 255, 255);
                    else
                        g.TextColor = Color.FromArgb(119, 126, 93);
                    g.ExtTextOut(UISettings.CalcPix(20),
                                 rItem.Top + (rItem.Height - textSize.Height) / 2, item.PrimaryText);
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            if (_paintedButton != null)
            {
                _paintedButton.Dispose();
                _paintedButton = null;
            }

            base.OnResize(e);
        }

        protected override void DrawItemBackgroundOn(Gdi gMem, NativeItemData item, Rectangle rItem, int nItem, bool isSelected)
        {
            gMem.FillRect(rItem, Color.White);

            if (nItem == 0)
                rItem = new Rectangle(rItem.Left, rItem.Top + UISettings.CalcPix(5), rItem.Width, rItem.Height - UISettings.CalcPix(10));
            else
                rItem = new Rectangle(rItem.Left, rItem.Top, rItem.Width, rItem.Height - UISettings.CalcPix(5));

            if (_paintedButton == null || isSelected || item.PrimaryText == Resources.ExtraView_ExitButtonTitle)
            {
                if (item.PrimaryText == Resources.ExtraView_ExitButtonTitle)
                {
                    if (isSelected)
                        DrawWideButton(rItem, gMem, MasterForm.SkinManager.GetImage("PhotoUpload1Pressed"),
                                       MasterForm.SkinManager.GetImage("PhotoUpload3Pressed"),
                                       MasterForm.SkinManager.GetImage("PhotoUpload2Pressed"));
                    else
                        DrawWideButton(rItem, gMem, MasterForm.SkinManager.GetImage("PhotoUpload1"),
                                       MasterForm.SkinManager.GetImage("PhotoUpload3"),
                                       MasterForm.SkinManager.GetImage("PhotoUpload2"));
                }
                else
                {
                    if (isSelected)
                        DrawWideButton(rItem, gMem, MasterForm.SkinManager.GetImage("EventList1Pressed"),
                                       MasterForm.SkinManager.GetImage("EventList3Pressed"),
                                       MasterForm.SkinManager.GetImage("EventList2Pressed"));
                    else
                        DrawWideButton(rItem, gMem, MasterForm.SkinManager.GetImage("EventList1"),
                                       MasterForm.SkinManager.GetImage("EventList3"),
                                       MasterForm.SkinManager.GetImage("EventList2"));
                }

                if (_paintedButton == null & !isSelected & rItem.Top > 0)
                {
                    _paintedButton = new Bitmap(rItem.Width, rItem.Height);
                    gMem.CopyImageTo(_paintedButton, 0, 0, rItem.Width, rItem.Height,
                        rItem.Left, rItem.Top);
                }
            }
            else
                gMem.DrawImage(_paintedButton, rItem.Left, rItem.Top);
        }

        private static void DrawWideButton(Rectangle rItem, Gdi g, IImage imgL, IImage imgR, IImage imgC)
        {
            ImageInfo iiL;
            ImageInfo iiR;
            imgL.GetImageInfo(out iiL);
            imgR.GetImageInfo(out iiR);

            int x = rItem.Left + UISettings.CalcPix(4);
            int y = rItem.Top;

            // draw left part
            g.DrawImageAlphaChannel(imgL, new Rectangle(x, y, (int)iiL.Width, rItem.Height));

            // draw tiled center
            int xR = rItem.Width - UISettings.CalcPix(4) - (int)iiR.Width - (int)iiL.Width;
            g.DrawImageAlphaChannel(imgC, new Rectangle(x + (int)iiL.Width, y, xR, rItem.Height));

            // draw right part
            xR = rItem.Width - UISettings.CalcPix(8) - (int)iiR.Width;
            g.DrawImageAlphaChannel(imgR, new Rectangle(x + xR, y, (int)iiR.Width, rItem.Height));
        }

        protected override void OnItemSelected(int index, Point mousePosition)
        {
            if (index < 0)
                return;

            if (NativeItems[index].SecondaryText == ExtraActions.UserData.ToString())
            {
                NavigationService.Controllers["ExtraController"].Initialize("GoToUserData");
            }
            else if (NativeItems[index].SecondaryText == ExtraActions.Settings.ToString())
            {
                NavigationService.Controllers["ExtraController"].Initialize("GoToSettings");
            }
            else if (NativeItems[index].SecondaryText == ExtraActions.About.ToString())
            {
                NavigationService.Controllers["ExtraController"].Initialize("GoToAbout");
            }
            else if (NativeItems[index].SecondaryText == ExtraActions.Help.ToString())
            {
                NavigationService.Controllers["ExtraController"].Initialize("GoToHelp");
            }
            else if (NativeItems[index].SecondaryText == ExtraActions.Exit.ToString())
            {
                NavigationService.Controllers["ExtraController"].Initialize("GoToExit");
            }

            base.OnItemSelected(index, mousePosition);
        }

        #endregion
    }

}
