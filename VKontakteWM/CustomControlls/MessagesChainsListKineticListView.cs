using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Galssoft.VKontakteWM.ApplicationLogic;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;
using Galssoft.VKontakteWM.Components.UI.Wrappers;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.Properties;
using Galssoft.VKontakteWM.Components.ImageClass;
using Galssoft.VKontakteWM.Components.MVC;
using System.Runtime.InteropServices;
using System.IO;

namespace Galssoft.VKontakteWM.CustomControlls
{
    class MessagesChainsListKineticListView : KineticListView<MessagesChainsListViewItem>
    {
        protected class NativeItemDataNew : NativeItemData
        {
            public bool IsItemHighlight { get; set; }
            public bool IsOutboxIconSet { get; set; }
            public new Bitmap OutboxIcon { get; set; }
        }

        #region Members

        private List<Bitmap> _cachedImages; 

        #endregion

        #region Constructors

        public MessagesChainsListKineticListView()
            : base(KineticControlScrollType.Vertical, new KineticControlsSetting
                  {
                      PrimaryTextFontGdi = FontCache.CreateFont("Tahoma", 12, FontStyle.Bold, true),
                      SecondaryTextFontGdi = FontCache.CreateFont("Tahoma", 11, FontStyle.Regular, true),
                      PrimaryText2FontGdi = FontCache.CreateFont("Tahoma", 11, FontStyle.Bold, true),
                      ThirdTextFontGdi = FontCache.CreateFont("Tahoma", 11, FontStyle.Regular, true),

                      TextPixMargin = 0,

                      ListItemPixSize = 42,
                      //ListItemPixSize = 100,
                  })
        {
            CacheStaticImages();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Источник данных для списка
        /// </summary>
        public List<MessagesChainsListViewItem> DataSource
        {
            get;
            set;
        }

        #endregion

        #region Protected Methods

        protected void CacheStaticImages()
        {
            if (_cachedImages != null)
                foreach (Bitmap bitmap in _cachedImages)
                    bitmap.Dispose();

            _cachedImages = new List<Bitmap>();

            _cachedImages.Add(CreateBitmapFromIImage(MasterForm.SkinManager.GetImage("MessagesArrow"), false));
            _cachedImages.Add(CreateBitmapFromIImage(MasterForm.SkinManager.GetImage("MessagesArrowPressed"), true));
        }

        protected Bitmap CreateBitmapFromIImage(IImage img, bool isSelected)
        {
            ImageInfo ii;
            if (img != null)
                img.GetImageInfo(out ii);
            else
                return null;

            int topIndent = - UISettings.CalcPix(15); //Arrow
            int height = Settings.ListItemPixSize;

            Bitmap res = new Bitmap((int)ii.Width, (int)ii.Height);
            using (Graphics gr = Graphics.FromImage(res))
            {
                IntPtr ptr = gr.GetHdc();
                try
                {
                    using (Gdi g = Gdi.FromHdc(ptr, Rectangle.Empty))
                    {
                        DrawItemBackgroundOn(g, null, new Rectangle(0, topIndent, res.Width, height), 0, isSelected);
                        g.DrawImageAlphaChannel(img, 0, 0);
                    }
                }
                finally
                {
                    gr.ReleaseHdc(ptr);
                }
            }
            return res;
        }

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
                    List<string> secondaryTextLines = new List<string>();

                    var hdcMem = OffScreenGraphics.GetHdc();

                    try
                    {                       
                        using (Gdi g = Gdi.FromHdc(hdcMem, Rectangle.Empty))
                        {
                            g.Font = Settings.SecondaryTextFontGdi;
                            g.TextAlign = Win32.TextAlign.TA_LEFT;

                            secondaryTextLines = TextFormatHelper.CutTextToLines(Items[i].MessageText, UISettings.CalcPix(192), 2, g);                                                                                 
                        }
                    }
                    finally
                    {
                        OffScreenGraphics.ReleaseHdc(hdcMem);
                    }

                    NativeItemDataNew newNativeItemData = new NativeItemDataNew
                    {
                        Uid = Items[i].Uid,

                        PrimaryText = Items[i].UserName,
                        SecondaryText = Items[i].MessageText,
                        TertiaryText = Items[i].MessageWroteDateString,

                        SecondaryTextLines = secondaryTextLines,

                        IsItemHighlight = Items[i].IsMessageNew,
                        IsOutboxIconSet = Items[i].IsMessageOutbox,

                        Tag = Items[i].UserID,
                        PrimaryImage = null,//MasterForm.SkinManager.GetImage("IsOutputMessage"),

                        InfoLeftIndents = new int[5],
                        InfoTopIndents = new int[3]
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

            nativeItem.InfoLeftIndents[0] = UISettings.CalcPix(5); //до иконки
            nativeItem.InfoLeftIndents[1] = UISettings.CalcPix(18); //блок текста (без "Я: ")
            nativeItem.InfoLeftIndents[2] = UISettings.CalcPix(30); //блок текста (с "Я: ")
            nativeItem.InfoLeftIndents[3] = UISettings.CalcPix(26); //Дата, выравнивание по правому краю
            nativeItem.InfoLeftIndents[4] = UISettings.CalcPix(18); //Arrow, выравнивание по правому краю

            nativeItem.InfoTopIndents[0] = UISettings.CalcPix(4); //ФИО, иконка и дата
            nativeItem.InfoTopIndents[1] = UISettings.CalcPix(18); //Текст сообщения
            nativeItem.InfoTopIndents[2] = UISettings.CalcPix(15); //Arrow
        }

        protected override void DrawItemBackgroundOn(Gdi gMem, NativeItemData nativeItem, Rectangle rItem, int nItem, bool isSelected)
        {
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
            NativeItemDataNew item = (NativeItemDataNew)nativeItem;

            bool isSelected = ShowSelectedItem ? nItem == _SelectedIndex : false;

            //Подложка айтема
            DrawItemBackgroundOn(g, item, rItem, nItem, isSelected);

            //полоска
            var rSep = new Rectangle(rItem.Left, rItem.Top, rItem.Width, 1);
            g.FillRect(rSep, Gdi.CreateSolidBrush(Color.FromArgb(219, 219, 219)));
            
            //IsItemHighlight icon + кэширование
            int leftIndent = rItem.Left + item.InfoLeftIndents[0] + UISettings.CalcPix(1);
            int topIndent = rItem.Top + item.InfoTopIndents[0] + UISettings.CalcPix(3);
            if (item.IsItemHighlight && item.PrimaryImage != null)
            {
                ImageInfo iinfo;
                item.PrimaryImage.GetImageInfo(out iinfo);

                if (item.OutboxIcon == null || isSelected)
                {
                    //g.DrawImageAlphaChannel(item.PrimaryImage, leftIndent, topIndent);

                    if (item.OutboxIcon == null & topIndent > 0)
                    {
                        item.OutboxIcon = new Bitmap((int)iinfo.Width, (int)iinfo.Height);
                        g.CopyImageTo(item.OutboxIcon, 0, 0, Settings.PrimaryIconPixWidth, Settings.PrimaryIconPixHeight, leftIndent, topIndent);
                    }
                }
                else
                {
                    g.DrawImage(item.OutboxIcon, leftIndent, topIndent);
                }
            }

            //Имя Фамилия
            topIndent = rItem.Top + item.InfoTopIndents[0];
            leftIndent = rItem.Left + item.InfoLeftIndents[1];
            if (!string.IsNullOrEmpty(item.PrimaryText))
            {
                g.Font = Settings.PrimaryTextFontGdi;
                g.TextAlign = Win32.TextAlign.TA_LEFT;
                if (isSelected) g.TextColor = Color.White;
                else g.TextColor = Settings.ListItemTextColor;
                g.ExtTextOut(/*leftIndent*/ UISettings.CalcPix(7), topIndent, item.PrimaryText);
            }

            //Дата
            topIndent = rItem.Top + item.InfoTopIndents[0] + UISettings.CalcPix(1); //из-за разных шрифтов
            leftIndent = rItem.Right - item.InfoLeftIndents[3];
            if (!string.IsNullOrEmpty(item.TertiaryText))
            {
                g.Font = Settings.ThirdTextFontGdi;
                g.TextAlign = Win32.TextAlign.TA_RIGHT;
                if (isSelected) g.TextColor = Color.White;
                else g.TextColor = Color.FromArgb(51, 153, 255);
                g.ExtTextOut(leftIndent, topIndent, item.TertiaryText);
            }

            //bool needTab = false;

            //Я:
            topIndent = rItem.Top + item.InfoTopIndents[1];
            leftIndent = rItem.Left + UISettings.CalcPix(7); //rItem.Left + item.InfoLeftIndents[1];
            if (item.IsOutboxIconSet)
            {
                g.Font = Settings.PrimaryText2FontGdi;
                g.TextAlign = Win32.TextAlign.TA_LEFT;
                if (isSelected) g.TextColor = Color.White;
                else g.TextColor = Settings.ListItemTextColor;
                g.ExtTextOut(leftIndent, topIndent, Resources.OutboxText);

                //Отступ для текста сообщения
                //leftIndent = rItem.Left + item.InfoLeftIndents[2];
                leftIndent = rItem.Left + UISettings.CalcPix(22);
            }

            

            //Текст сообщения
            if (item.SecondaryTextLines.Count > 0)
            {
                g.Font = Settings.SecondaryTextFontGdi;
                g.TextAlign = Win32.TextAlign.TA_LEFT;
                if (isSelected) g.TextColor = Color.White;
                else g.TextColor = Color.FromArgb(102, 102, 102);

                foreach (string line in item.SecondaryTextLines)
                {
                    g.ExtTextOut(leftIndent, topIndent, line);

                    topIndent += UISettings.CalcPix(11);
                }
            }

            //Arrow
            topIndent = rItem.Top + item.InfoTopIndents[2];
            leftIndent = rItem.Right - item.InfoLeftIndents[4];

            //if (isSelected) g.DrawImageAlphaChannel(MasterForm.SkinManager.GetImage("MessagesArrowPressed"), leftIndent, topIndent);
            //else g.DrawImageAlphaChannel(MasterForm.SkinManager.GetImage("MessagesArrow"), leftIndent, topIndent);
            if (_cachedImages != null && _cachedImages.Count > 1)
            {
                if (isSelected) g.DrawImage(_cachedImages[1], leftIndent, topIndent);
                else g.DrawImage(_cachedImages[0], leftIndent, topIndent);
            }

        }

        protected override void OnItemSelected(int index, Point mousePosition)
        {
            if (index < 0)
            {
                return;
            }

            int userID = 0;

            try
            {
                userID = Convert.ToInt32(NativeItems[index].Tag);
            }
            catch
            {
                return;
            }

            string userName = NativeItems[index].PrimaryText;

            if (string.IsNullOrEmpty(userName))
            {
                userName = string.Empty;
            }

            using (new WaitWrapper(false))
            {
                NavigationService.Controllers["MessagesChainsListController"].Initialize("GoToMessagesList", userID, userName);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (_cachedImages != null)
            {
                foreach (Bitmap cachedImage in _cachedImages)
                {
                    cachedImage.Dispose();
                }
                _cachedImages = null;
            }
        }

        #endregion
    }
}
