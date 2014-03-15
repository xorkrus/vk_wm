using System;
using System.Collections.Generic;
using System.Drawing;
using Galssoft.VKontakteWM.ApplicationLogic;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;
using Galssoft.VKontakteWM.Properties;

namespace Galssoft.VKontakteWM.CustomControlls
{
    class MessageListKineticListView : KineticListView<MessagesListViewItem>
    {
        protected class NativeItemDataNew : NativeItemData
        {
            public bool ItemHighlight { get; set; }
        }

        #region Members

        //

        #endregion

        #region Constructors

        public MessageListKineticListView()
            : base(KineticControlScrollType.Vertical, new KineticControlsSetting
                  {
                      PrimaryTextFontGdi = FontCache.CreateFont("Tahoma", 12, FontStyle.Bold, true),
                      SecondaryTextFontGdi = FontCache.CreateFont("Tahoma", 11, FontStyle.Regular, true),
                      ThirdTextFontGdi = FontCache.CreateFont("Tahoma", 11, FontStyle.Regular, true),                      

                      TextPixMargin = 0,

                      ListItemPixSize = 10,                      
                  })
        {
            //
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Источник данных для списка
        /// </summary>
        public List<MessagesListViewItem> DataSource
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
                return Resources.MessagesChainsListKineticListView_IfEmptyText;
            }
        }

        protected override List<NativeItemData> BuildNativeControlItems()
        {
            var newListData = new List<NativeItemData>(Items.Count);

            if (Items != null)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    string primaryText = Resources.MessageI;
                    if (Items[i].IsMessageInbox)
                        primaryText = Items[i].UserName;

                    NativeItemDataNew newNativeItemData = new NativeItemDataNew
                    {
                        Uid = Items[i].Uid,

                        PrimaryText = primaryText,
                        SecondaryText = Items[i].MessageText,
                        TertiaryText = Items[i].MessageWroteDateString,

                        ItemHighlight = Items[i].IsMessageInbox,                        

                        InfoLeftIndents = new int[3],
                        InfoTopIndents = new int[3]
                    };

                    newListData.Add(newNativeItemData);
                }
            }

            return newListData;
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

                return;
            }

            var hdcMem = OffScreenGraphics.GetHdc();
            int lastHeight = 0;

            try
            {
                using (Gdi g = Gdi.FromHdc(hdcMem, Rectangle.Empty))
                {
                    g.Font = Settings.SecondaryTextFontGdi;
                    g.TextAlign = Win32.TextAlign.TA_LEFT;
                   
                    // для текста в каждой ячейке
                    for (int i = 0; i < ItemCount; i++)
                    {
                        StartPositions.Add(c);

                        nid[i].SecondaryTextLines = TextFormatHelper.CutTextToLines(nid[i].SecondaryText, UISettings.CalcPix(200), 0, g);                        

                        c += (Settings.ListItemPixSize + (nid[i].SecondaryTextLines.Count + 1) * UISettings.CalcPix(11));
                    }

                    StartPositions.Add(c);
                }
            }
            finally
            {
                OffScreenGraphics.ReleaseHdc(hdcMem);
            }

            

            _ActiveListHeight = c;
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
            maxTextLines = Int32.MaxValue;

            nativeItem.InfoLeftIndents[0] = UISettings.CalcPix(18); //от левого края           
            nativeItem.InfoLeftIndents[1] = UISettings.CalcPix(18); //блок текста (с аватаркой)            
            nativeItem.InfoLeftIndents[2] = UISettings.CalcPix(10); //Дата, выравнивание по правому краю

            nativeItem.InfoTopIndents[0] = UISettings.CalcPix(5); //от верх. края
            nativeItem.InfoTopIndents[1] = UISettings.CalcPix(18); //2 строка текста
            nativeItem.InfoTopIndents[2] = UISettings.CalcPix(35); //3 строка текста
        }

        protected override void DrawItemBackgroundOn(Gdi gMem, NativeItemData nativeItem, Rectangle rItem, int nItem, bool isSelected)
        {
            //var item = (NativeItemDataNew)nativeItem;

            Color baseColor;

            //if (item.ItemHighlight)
            //{
                //baseColor = Color.FromArgb(230, 230, 230);
            //}
            //else
            {
                baseColor = Color.FromArgb(255, 255, 255);
            }

            //if (isSelected)
            //{
                //gMem.GradientFill(rItem, Color.FromArgb(214, 220, 236), baseColor, FillDirection.TopToBottom);
            //}
            //else
            //{
                BrushGdi backGround = Gdi.CreateSolidBrush(baseColor);

                if (backGround != BrushGdi.Empty)
                {
                    gMem.FillRect(rItem, backGround);
                }
            //}
        }

        protected override void DrawItemOn(Gdi g, NativeItemData nativeItem, Rectangle rItem, int nItem)
        {
            NativeItemDataNew item = (NativeItemDataNew)nativeItem;

            //выделение
            DrawItemBackgroundOn(g, item, rItem, nItem, SelectedIndex == nItem);

            //полоска
            var rSep = new Rectangle(rItem.Left, rItem.Top, rItem.Width, 1);
            g.FillRect(rSep, Gdi.CreateSolidBrush(Color.FromArgb(205, 205, 205)));


            //Имя
            int topIndent = rItem.Top + item.InfoTopIndents[0];
            int leftIndent = rItem.Left + UISettings.CalcPix(7); //rItem.Left + item.InfoLeftIndents[0];
            if (!string.IsNullOrEmpty(item.PrimaryText))
            {
                g.Font = Settings.PrimaryTextFontGdi;
                g.TextAlign = Win32.TextAlign.TA_LEFT;
                g.TextColor = Settings.ListItemTextColor;

                g.ExtTextOut(leftIndent, topIndent, item.PrimaryText);
            }

            //Дата
            topIndent = rItem.Top + item.InfoTopIndents[0] + UISettings.CalcPix(1); //из-за разных шрифтов
            leftIndent = rItem.Right - item.InfoLeftIndents[2];
            if (!string.IsNullOrEmpty(item.TertiaryText))
            {
                g.Font = Settings.ThirdTextFontGdi;
                g.TextAlign = Win32.TextAlign.TA_RIGHT;
                g.TextColor = Color.FromArgb(51, 153, 255);

                g.ExtTextOut(leftIndent, topIndent, item.TertiaryText);
            }

            //Текст сообщения
            leftIndent = rItem.Left + UISettings.CalcPix(7); //rItem.Left + item.InfoLeftIndents[0];
            topIndent = rItem.Top + item.InfoTopIndents[1];
            if (item.SecondaryTextLines.Count > 0)
            {
                g.Font = Settings.SecondaryTextFontGdi;
                g.TextAlign = Win32.TextAlign.TA_LEFT;
                g.TextColor = Color.FromArgb(102, 102, 102);

                foreach (string line in item.SecondaryTextLines)
                {
                    g.ExtTextOut(leftIndent, topIndent, line);

                    topIndent += UISettings.CalcPix(11);
                }
            }            
        }  

        #endregion
    }
}
