﻿using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Galssoft.VKontakteWM.ApplicationLogic;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.Properties;
using Galssoft.VKontakteWM.Components.ImageClass;
using System.Runtime.InteropServices;
using System.IO;

namespace Galssoft.VKontakteWM.CustomControlls
{
    class StatusHistoryKineticList : KineticListView<StatusUpdatesListViewItem>
    {
        #region Members

        //

        #endregion

        #region Constructors

        public StatusHistoryKineticList()
            : base(KineticControlScrollType.Vertical, new KineticControlsSetting
            {
                PrimaryTextFontGdi = FontCache.CreateFont("Tahoma", 12, FontStyle.Bold, true),
                SecondaryTextFontGdi = FontCache.CreateFont("Tahoma", 11, FontStyle.Regular, true),
                ThirdTextFontGdi = FontCache.CreateFont("Tahoma", 11, FontStyle.Regular, true),
                GroupFont = FontCache.CreateFont("Tahoma", 12, FontStyle.Regular, true),

                TextPixMargin = 0,

                ListItemPixSize = 0,

                GroupPixTopMargin = 0,

                GroupPixHeight = 16,

                SecondaryTextLinesCount = 0
            })
        {
            ShowGroupHeader = true;

            EnableLongPress = true;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Источник данных для списка
        /// </summary>
        public List<StatusUpdatesListViewItem> DataSource
        {
            get;
            set;
        }

        #endregion

        #region Protected Methods

        protected override string IfEmptyText
        {
            get
            {
                return Resources.StatusUpdatesListKineticListView_IfEmptyText;
            }
        }

        protected override List<NativeItemData> BuildNativeControlItems()
        {
            var newListData = new List<NativeItemData>(Items.Count);

            if (Items != null)
            {
                IImage newIImage = MasterForm.SkinManager.GetImage("StatusIcon");

                for (int i = 0; i < Items.Count; i++)
                {
                    //bool showGroupHeader = false;

                    //try
                    //{
                    //    showGroupHeader = IsItemNewGroup(i);
                    //}
                    //catch
                    //{
                    //    //
                    //}

                    NativeItemData newNativeItemData = new KineticListView<StatusUpdatesListViewItem>.NativeItemData
                    {
                        //Tag = showGroupHeader,
                        Uid = Items[i].Uid,
                        Group = Items[i].Group,

                        PrimaryText = Items[i].UserName,
                        SecondaryText = Items[i].UserStatus,
                        TertiaryText = Items[i].StatusSetDate.ToString("HH:mm"),

                        PrimaryImage = newIImage,
                        PrimaryImageURL = string.Empty,

                        InfoLeftIndents = new int[3],
                        InfoTopIndents = new int[2]
                    };

                    newListData.Add(newNativeItemData);
                }
            }

            return newListData;
        }

        protected override void GenerateListItems()
        {
            Items.Clear();

            if (DataSource != null)
            {
                Items.AddRange(DataSource);
            }
        }

        protected override int GetTextareaWidthWithOption(int i)
        {
            return GetTextareaWidth(i) - Settings.TextPixMargin * 4 - Settings.SecondaryIconPixWidth * 2;
        }

        protected override void CalculateTextWidthsCustom(NativeItemData nativeItem, Graphics g, int textareaWidth, out int maxTextLines)
        {
            maxTextLines = 2;

            nativeItem.InfoLeftIndents[0] = UISettings.CalcPix(5);//иконка
            nativeItem.InfoLeftIndents[1] = UISettings.CalcPix(10);//Имя и статус
            nativeItem.InfoLeftIndents[2] = UISettings.CalcPix(10);//Дата, отступ справа

            nativeItem.InfoTopIndents[0] = UISettings.CalcPix(5);//иконка, дата и имя
            nativeItem.InfoTopIndents[1] = UISettings.CalcPix(15);//статус
        }

        protected override void DrawGroupHeaderOn(Gdi gMem, int item, Rectangle rHeaderArea)
        {
            //// Draw Group Header Text

            //gMem.Font = Settings.GroupFont;
            //gMem.TextAlign = Win32.TextAlign.TA_RIGHT;
            ////gMem.TextColor = Color.FromArgb(150, 150, 150);
            ////gMem.ExtTextOut(rHeaderArea.Right - UISettings.CalcPix(10) + 1,
            ////                rHeaderArea.Top + Settings.GroupPixTopMargin * 2 + 1,
            ////                GetItemAtIndex(item).Group, true);

            //gMem.TextColor = Settings.ListGroupTextColor;
            //gMem.ExtTextOut(rHeaderArea.Right - UISettings.CalcPix(10),
            //                rHeaderArea.Top + Settings.GroupPixTopMargin * 2,
            //                GetItemAtIndex(item).Group, true);

            //gMem.Font = Settings.GroupFont;
            //gMem.TextAlign = Win32.TextAlign.TA_RIGHT;
            //gMem.TextColor = Settings.ListGroupTextColor;

            //Size newSize = gMem.GetTextExtent(GetItemAtIndex(item).Group);

            //gMem.ExtTextOut(rHeaderArea.Right - UISettings.CalcPix(10), rHeaderArea.Top + (rHeaderArea.Height - newSize.Height) / 2, GetItemAtIndex(item).Group, true);

            gMem.Font = Settings.GroupFont;
            gMem.TextAlign = Win32.TextAlign.TA_RIGHT;

            Size newSize = gMem.GetTextExtent(GetItemAtIndex(item).Group);

            gMem.TextColor = Color.FromArgb(150, 150, 150);
            gMem.ExtTextOut(rHeaderArea.Right - UISettings.CalcPix(10) + UISettings.CalcPix(1), rHeaderArea.Top + (rHeaderArea.Height - newSize.Height) / 2 + UISettings.CalcPix(1), GetItemAtIndex(item).Group);

            gMem.TextColor = Settings.ListGroupTextColor;
            gMem.ExtTextOut(rHeaderArea.Right - UISettings.CalcPix(10), rHeaderArea.Top + (rHeaderArea.Height - newSize.Height) / 2, GetItemAtIndex(item).Group);
        }

        protected override void DrawItemBackgroundOn(Gdi gMem, NativeItemData nativeItem, Rectangle rItem, int nItem, bool isSelected)
        {
            NativeItemData item = (NativeItemData)nativeItem;

            Color baseColor;
            baseColor = Color.FromArgb(255, 255, 255);

            if (isSelected)
            {
                gMem.GradientFill(rItem, Color.FromArgb(69, 137, 219), Color.FromArgb(50, 106, 173), FillDirection.TopToBottom);
            }
            else
            {
                BrushGdi backGround = Gdi.CreateSolidBrush(baseColor);

                if (backGround != BrushGdi.Empty)
                {
                    gMem.FillRect(rItem, backGround);
                }
            }
        }

        protected override void DrawItemOn(Gdi g, NativeItemData nativeItem, Rectangle rItem, int nItem)
        {
            int topIndent;
            int leftIndent;

            NativeItemData item = nativeItem;
            bool isSelected = ShowSelectedItem ? nItem == _SelectedIndex : false;

            //фон
            DrawItemBackgroundOn(g, item, rItem, nItem, isSelected);

            //полоска
            var rSep = new Rectangle(rItem.Left, rItem.Top, rItem.Width, 1);
            g.FillRect(rSep, Settings.ListItemSeparator);
                   
            // дата
            topIndent = rItem.Top + item.InfoTopIndents[0];
            leftIndent = rItem.Right - item.InfoLeftIndents[1];

            if (IsItemNewGroup(nItem))
            {
                topIndent += Settings.GroupPixHeight;
            }    

            if (!string.IsNullOrEmpty(item.TertiaryText))
            {
                g.Font = Settings.ThirdTextFontGdi;
                g.TextAlign = Win32.TextAlign.TA_RIGHT;
                g.TextColor = Color.FromArgb(51, 153, 255);

                if (isSelected)
                {
                    g.TextColor = Color.White;
                }                

                g.ExtTextOut(leftIndent, topIndent, item.TertiaryText);
            }            

            // текст сообщения
            leftIndent = rItem.Left + item.InfoLeftIndents[0];
            topIndent = rItem.Top + item.InfoTopIndents[0];

            if (IsItemNewGroup(nItem))
            {
                topIndent += Settings.GroupPixHeight;
            }

            if (item.SecondaryTextLines.Count > 0)
            {
                g.Font = Settings.SecondaryTextFontGdi;
                g.TextAlign = Win32.TextAlign.TA_LEFT;
                g.TextColor = Color.FromArgb(102, 102, 102);

                if (isSelected)
                {
                    g.TextColor = Color.White;
                }

                foreach (string line in item.SecondaryTextLines)
                {
                    g.ExtTextOut(leftIndent, topIndent, line);

                    topIndent += UISettings.CalcPix(11);
                }
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

            var hdcMem = OffScreenGraphics.GetHdc();

            try
            {
                using (Gdi g = Gdi.FromHdc(hdcMem, Rectangle.Empty))
                {
                    g.Font = Settings.SecondaryTextFontGdi;
                    g.TextAlign = Win32.TextAlign.TA_LEFT;

                    for (int i = 0; i < ItemCount; i++)
                    {
                        StartPositions.Add(c);

                        //nid[i].SecondaryTextLines = TextFormatHelper.CutTextToLines(nid[i].SecondaryText, Width - UISettings.CalcPix(40), 0, g);
                        nid[i].SecondaryTextLines = TextFormatHelper.CutTextToLines(nid[i].SecondaryText, Width - UISettings.CalcPix(45), Settings.SecondaryTextLinesCount, g);

                        //SecondaryTextLinesCount

                        c += (Settings.ListItemPixSize + (nid[i].SecondaryTextLines.Count + 1) * UISettings.CalcPix(11));

                        if (ShowGroupHeader)
                        {
                            //if (IsItemNewGroup(nItem) && item.Group != null && rItem.Top >= rListRect.Top)
                            if (IsItemNewGroup(i) && nid[i].Group != null)
                            {
                                c += Settings.GroupPixHeight;
                            }
                        }

                        //c += Settings.ListItemPixSize;
                    }
                }
            }
            finally
            {
                OffScreenGraphics.ReleaseHdc(hdcMem);
            }

            StartPositions.Add(c); //The end of tha last item

            _ActiveListHeight = c;

            ScrollTo(0);
        }

        #endregion
    }
}
