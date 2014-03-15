using System.Collections.Generic;
using System.Drawing;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;
//using Galssoft.VKontakteWM.ApplicationLogic;
//using Galssoft.VKontakteWM.Forms;

namespace Galssoft.VKontakteWM.Controls
{
    /*
    class TestKineticControl : KineticListView<TestDataClass>
    {
        
        #region Members


        #endregion

        #region Constructors

        public TestKineticControl()
            : base(KineticControlScrollType.Vertical, new KineticControlsSetting
                  {
                      PrimaryTextFontGdi = FontCache.CreateFont("Calibri", 18, FontStyle.Regular, true)
                  })
        {

        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Источник данных для списка
        /// </summary>
        public List<TestDataClass> DataSource
        {
            get;
            set;
        }

        #endregion

        #region Protected Methods

        protected override List<NativeItemData> BuildNativeControlItems()
        {
            var retVal = new List<NativeItemData>(Items.Count);

            if (Items != null)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    NativeItemData data = new NativeItemData
                          {
                              PrimaryText = Items[i].AnyData,
                              Tag = Items[i].AnyData2,
                              PrimaryImage = Items[i].MyImage,
                              SecondaryImage = i%2 == 0
                                      ? MasterForm.SkinManager.GetImage("MyCitiesUpButton")
                                      : MasterForm.SkinManager.GetImage("MyCitiesDownButton"),
                              InfoLeftIndents = new int[2],
                              InfoTopIndents = new int[1]
                          };  
                    retVal.Add(data);
                }
            }
            return retVal;
        }

        protected override void GenerateListItems()
        {
            //Здесь можно что либо сделать с источником данных
            //Или как ваориант откуда нибудь его загрузить самому
            //

            Items.Clear();
            if (DataSource != null)
                Items.AddRange(DataSource);
        }

        protected override int GetTextareaWidthWithOption(int i)
        {
            return GetTextareaWidth(i) - Settings.TextPixMargin * 4 - Settings.SecondaryIconPixWidth * 2;
        }

        protected override void CalculateTextWidthsCustom(NativeItemData nativeItem, Graphics g, int textareaWidth, out int maxTextLines)
        {
            maxTextLines = 1;
            nativeItem.InfoLeftIndents[0] = UISettings.CalcPix(30);
            nativeItem.InfoTopIndents[0] = UISettings.CalcPix(14);
        }

        protected override void DrawItemOn(Gdi g, NativeItemData nativeItem, Rectangle rItem, int nItem)
        {
            NativeItemData item = nativeItem;

            if (rItem.Height > Settings.ListItemPixSize)
                rItem = new Rectangle(rItem.Left,
                                      rItem.Top + rItem.Height - Settings.ListItemPixSize,
                                      rItem.Width,
                                      Settings.ListItemPixSize);

            DrawItemBackgroundOn(g, item, rItem, nItem, false);

            var rSep = new Rectangle(rItem.Left, rItem.Bottom - 1, rItem.Width, 1);
            g.FillRect(rSep, Settings.ListItemSeparator);

            int leftIndent = rItem.Left + Settings.TextPixMargin;
            int topIndent = rItem.Bottom - rItem.Height + item.InfoTopIndents[0];

            // write name

            if(item.PrimaryImage != null)
            {
                g.DrawImageAlphaChannel(item.PrimaryImage, leftIndent, topIndent);
            }

            leftIndent += Settings.SecondaryIconPixWidth + Settings.TextPixMargin;

            if (!string.IsNullOrEmpty(item.PrimaryText))
            {
                g.Font = Settings.PrimaryTextFontGdi;
                g.TextAlign = Win32.TextAlign.TA_LEFT;
                g.TextColor = Settings.ListItemTextColor;
                g.ExtTextOut(leftIndent,
                    topIndent,
                    item.PrimaryText);
            }

            if(item.SecondaryImage != null)
            {
                g.DrawImageAlphaChannel(item.SecondaryImage, rItem.Right - Settings.SecondaryIconPixWidth + Settings.TextPixMargin, topIndent);
            }
        }

        #endregion
        
    }
    */
}
