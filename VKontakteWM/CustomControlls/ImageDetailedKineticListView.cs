using System;
using System.Collections.Generic;
using System.Drawing;
using Galssoft.VKontakteWM.ApplicationLogic;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;
using Galssoft.VKontakteWM.Forms;
using System.Runtime.InteropServices;
using System.IO;
using Galssoft.VKontakteWM.Properties;

namespace Galssoft.VKontakteWM.CustomControlls
{
    class ImageDetailedKineticListView : KineticListView<ImageDetailedListViewItem>
    {
        #region Members

        private readonly FontGdi _priFont;
        private readonly FontGdi _secFont;

        #endregion

        #region Constructors

        public ImageDetailedKineticListView()
            : base(KineticControlScrollType.Vertical, new KineticControlsSetting
            {
                //PrimaryTextFontGdi = FontCache.CreateFont("Calibri", 12, FontStyle.Bold, true),
                //SecondaryTextFontGdi = FontCache.CreateFont("Calibri", 11, FontStyle.Regular, true),
                //ThirdTextFontGdi = FontCache.CreateFont("Calibri", 11, FontStyle.Regular, true),

                TextPixMargin = 0,

                ListItemPixSize = 10,
            })
        {
            ShowGroupHeader = true;
            ShowScrollbar = true;

            _priFont = FontCache.CreateFont("Calibri", 12, FontStyle.Bold, true);
            _secFont = FontCache.CreateFont("Calibri", 11, FontStyle.Regular, true);

                      //            PrimaryTextFontGdi = FontCache.CreateFont("Calibri", 12, FontStyle.Bold, true),
                      //SecondaryTextFontGdi = FontCache.CreateFont("Calibri", 11, FontStyle.Regular, true),
                      //ThirdTextFontGdi = FontCache.CreateFont("Calibri", 11, FontStyle.Regular, true),        
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Источник данных для списка
        /// </summary>
        public List<ImageDetailedListViewItem> DataSource
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
                return Resources.ImageDetailedKineticListView_IfEmptyText;
            }
        }

        /// <summary>
        /// Прогружает изображение (группу изображений), проставляя PrimaryImageURL.
        /// Этот метод мы вызываем при прогрузке очередного изображения в кэш
        /// </summary>
        /// <param name="item"></param>
        public void UpdateUserPhoto(ImageDetailedListViewItem item)
        {
            for (int i = 0; i < NativeItems.Count; i++)
            {
                var ni = NativeItems[i];

                if (ni.Uid == item.Uid)
                {
                    ni.PrimaryImageURL = item.Photo;                    

                    break;
                }
            }
        }

        /// <summary>
        /// Освобождает ресурсы IImage при деактивации формы
        /// </summary>
        public void ReleaseUserPhotos()
        {
            IImage image = MasterForm.SkinManager.GetImage("ImageNull");

            for (int i = 0; i < NativeItems.Count; i++)
            {
                var ni = NativeItems[i];

                if ((ni.PrimaryImage != null) && !ni.PrimaryImage.Equals(image))
                {
                    Marshal.ReleaseComObject(ni.PrimaryImage);

                    ni.PrimaryImage = null;

                    ni.PrimaryImageURL = "clear";

                    if (ni.Icon != null)
                    {
                        ni.Icon.Dispose();
                        ni.Icon = null;
                    }
                }
                else
                {
                    ni.PrimaryImageURL = string.Empty;
                }

                //ni.PrimaryImageURL = "clear";
            }
        }

        /// <summary>
        /// Прогружает все имеющиеся в кэше изображения, проставляя PrimaryImageURL
        /// </summary>
        public void ReloadUserPhotos()
        {
            for (int i = 0; i < NativeItems.Count; i++)
            {
                var ni = NativeItems[i];
                var item = Items[i];

                string avatar = string.Empty;

                if (item.IsPhotoLoaded)
                {
                    if (File.Exists(item.Photo))
                    {
                        avatar = item.Photo;
                    }
                    else
                    {
                        avatar = "clear";
                    }
                }
                else
                {
                    avatar = string.Empty;
                }

                ni.PrimaryImageURL = avatar;
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

                return;
            }

            var hdcMem = OffScreenGraphics.GetHdc();

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

                        // есть заголовок группы?
                        if (IsItemNewGroup(i) && !string.IsNullOrEmpty(nid[i].Group))
                        {
                            c += Settings.GroupPixHeight;
                        }
                        
                        // что выводим?
                        switch (Items[i].Type)
                        {
                            case ImageDetailedListViewItemType.Author: // ячейка "автор"
                                c += UISettings.CalcPix(25);                                            
                                break;

                            case ImageDetailedListViewItemType.Comment: // ячейка "комментарий"
                                nid[i].SecondaryTextLines = TextFormatHelper.CutTextToLines(nid[i].SecondaryText, Width - UISettings.CalcPix(10), 0, g);
                                c += (Settings.ListItemPixSize + (nid[i].SecondaryTextLines.Count + 1) * UISettings.CalcPix(11));                                
                                break;

                            case ImageDetailedListViewItemType.Photo:
                                if (Items[i].IsPhotoLoaded)
                                {
                                    c += Items[i].PhotoHeight;
                                }
                                else
                                {
                                    c += UISettings.CalcPix(50);
                                }
                                break;
                        }                                        
                    }
                }
            }
            finally
            {
                OffScreenGraphics.ReleaseHdc(hdcMem);
            }

            StartPositions.Add(c);

            _ActiveListHeight = c;

            ScrollTo(0);
        }

        protected override List<NativeItemData> BuildNativeControlItems()
        {
            var newListData = new List<NativeItemData>(Items.Count);
           
            if (Items != null)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    string photo = string.Empty;

                    if (!string.IsNullOrEmpty(Items[i].Photo))
                    {
                        if (Items[i].IsPhotoLoaded)
                        {
                            photo = Items[i].Photo;
                        }
                        else
                        {
                            photo = "clear";
                        }
                    }

                    //bool groupHeaderIsSet = false;

                    //if (IsItemNewGroup(i) && !string.IsNullOrEmpty(Items[i].Group))
                    //{
                    //    groupHeaderIsSet = true;
                    //}

                    NativeItemData data = new NativeItemData
                          {                              
                              PrimaryImageURL = photo,
                              PrimaryText = Items[i].UserName,
                              SecondaryText = Items[i].UserComment,
                              TertiaryText = Items[i].CommentWroteDateString,
                              Group = Items[i].Group,
                              //Tag = groupHeaderIsSet,

                              InfoLeftIndents = new int[3],
                              InfoTopIndents = new int[5]
                          };

                    newListData.Add(data);
                }
            }

            return newListData;     
        }

        protected override int GetTextareaWidthWithOption(int i)
        {
            return Width - UISettings.CalcPix(10);
        }

        protected override void CalculateTextWidthsCustom(NativeItemData nativeItemData, Graphics groupHeaderGraphics, int textareaWidth, out int maxTextLines)
        {
            maxTextLines = Int32.MaxValue;
            //maxTextLines = 100;
            nativeItemData.InfoLeftIndents[0] = UISettings.CalcPix(5); //от левого края           
            nativeItemData.InfoLeftIndents[1] = UISettings.CalcPix(5); //блок текста (с аватаркой)            
            nativeItemData.InfoLeftIndents[2] = UISettings.CalcPix(10); //Дата, выравнивание по правому краю

            nativeItemData.InfoTopIndents[0] = UISettings.CalcPix(5); //от верх. края
            nativeItemData.InfoTopIndents[1] = UISettings.CalcPix(18); //2 строка текста
            nativeItemData.InfoTopIndents[2] = UISettings.CalcPix(35); //3 строка текста
        }

        protected override void GenerateListItems()
        {
            Items.Clear();

            if (DataSource != null)
            {
                Items.AddRange(DataSource);
            }
        }                

        protected override void DrawGroupHeaderOn(Gdi gMem, int item, Rectangle rHeaderArea)
        {         
            //gMem.Font = Settings.GroupFont;
            //gMem.TextAlign = Win32.TextAlign.TA_LEFT;

            ////gMem.TextColor = Color.FromArgb(150, 150, 150);
            ////gMem.ExtTextOut(rHeaderArea.Left + UISettings.CalcPix(5) + 1, rHeaderArea.Top + Settings.GroupPixTopMargin + 1, GetItemAtIndex(item).Group, true);

            //gMem.TextColor = Settings.ListGroupTextColor;
            //gMem.ExtTextOut(rHeaderArea.Left + UISettings.CalcPix(5), rHeaderArea.Top + Settings.GroupPixTopMargin, GetItemAtIndex(item).Group, true);

            //gMem.Font = Settings.GroupFont;
            //gMem.TextAlign = Win32.TextAlign.TA_LEFT;
            //gMem.TextColor = Settings.ListGroupTextColor;

            //Size newSize = gMem.GetTextExtent(GetItemAtIndex(item).Group);

            //gMem.ExtTextOut(rHeaderArea.Left + UISettings.CalcPix(5), rHeaderArea.Top + (rHeaderArea.Height - newSize.Height) / 2, GetItemAtIndex(item).Group, true);

            gMem.Font = Settings.GroupFont;
            gMem.TextAlign = Win32.TextAlign.TA_LEFT;

            Size newSize = gMem.GetTextExtent(GetItemAtIndex(item).Group);

            gMem.TextColor = Color.FromArgb(150, 150, 150);
            gMem.ExtTextOut(rHeaderArea.Left + UISettings.CalcPix(5) + UISettings.CalcPix(1), rHeaderArea.Top + (rHeaderArea.Height - newSize.Height) / 2 + UISettings.CalcPix(1), GetItemAtIndex(item).Group);

            gMem.TextColor = Settings.ListGroupTextColor;
            gMem.ExtTextOut(rHeaderArea.Left + UISettings.CalcPix(5), rHeaderArea.Top + (rHeaderArea.Height - newSize.Height) / 2, GetItemAtIndex(item).Group);
        }

        protected override void DrawItemBackgroundOn(Gdi gMem, NativeItemData nativeItem, Rectangle rItem, int nItem, bool isSelected)
        {
            NativeItemData item = nativeItem;

            if (nItem == 0) // картинка
            {
                Color baseColor = Color.FromArgb(51, 51, 51);
                BrushGdi backGround = Gdi.CreateSolidBrush(baseColor);
                gMem.FillRect(rItem, backGround);
            }
            else if (!isSelected || nItem == 1)
            {
                Color baseColor = Color.White;
                BrushGdi backGround = Gdi.CreateSolidBrush(baseColor);
                gMem.FillRect(rItem, backGround);
            }            
            else
            {
                gMem.GradientFill(rItem, Color.FromArgb(69, 137, 219), Color.FromArgb(50, 106, 173), FillDirection.TopToBottom);
            }
        }

        protected override void DrawItemOn(Gdi g, NativeItemData nativeItem, Rectangle rItem, int nItem)
        {            
            NativeItemData item = nativeItem;

            bool isSelected = ShowSelectedItem ? nItem == _SelectedIndex : false;

            if (!(nItem > 1))
            {
                isSelected = false;
            }

            //bool isSelected;

            //if (SelectedIndex == nItem)
            //{
                 
            //}

            //if nItem

            //if (SelectedIndex == nItem)
            //{
            //    isSe
            //}

            //    

            //if (rItem.Height > Settings.ListItemPixSize)
            //{
            //    rItem = new Rectangle(rItem.Left, rItem.Top + rItem.Height - Settings.ListItemPixSize, rItem.Width, Settings.ListItemPixSize);
            //}

            // выделение
            DrawItemBackgroundOn(g, item, rItem, nItem, SelectedIndex == nItem);

            //// полоска
            //var rSep = new Rectangle(rItem.Left, rItem.Top, rItem.Width, 1);
            //g.FillRect(rSep, Gdi.CreateSolidBrush(Color.FromArgb(205, 205, 205)));

            // загрузка изображения...
            if (!string.IsNullOrEmpty(item.PrimaryImageURL))
            {
                if (!item.PrimaryImageURL.Equals("clear"))
                {
                    IImage newIImage = null;

                    ImageHelper.LoadImageFromFile(item.PrimaryImageURL, out newIImage);

                    item.PrimaryImage = newIImage;
                }
                else
                {
                    item.PrimaryImage = MasterForm.SkinManager.GetImage("ImageNull");
                }

                if (item.PrimaryImage != null)
                {
                    ImageInfo ii;
                    item.PrimaryImage.GetImageInfo(out ii);
                    Size imageSize = new Size((int) ii.Width, (int) ii.Height);
                    item.Icon = new Bitmap(imageSize.Width, imageSize.Height);
                    //Create cached image
                    using (Graphics gr = Graphics.FromImage(item.Icon))
                    {
                        IntPtr grPtr = gr.GetHdc();
                        Rectangle rect = new Rectangle(0, 0, imageSize.Width, imageSize.Height);
                        item.PrimaryImage.Draw(grPtr, ref rect, IntPtr.Zero);
                        gr.ReleaseHdc(grPtr);
                    }
                }

                item.PrimaryImageURL = string.Empty;
            }

            if (item.Icon != null) // если есть изображение, то айтем первого типа
            {
                g.DrawImage(item.Icon, rItem.Left + (rItem.Width - item.Icon.Width) / 2, rItem.Top + (rItem.Height - item.Icon.Height) / 2);
                //g.DrawImageAlphaChannel(item.PrimaryImage, rItem.Left + (rItem.Width - item.ImageSize.Width) / 2, rItem.Top + (rItem.Height - item.ImageSize.Height) / 2);
            }
            else // 2 или 3
            {
                // полоска
                Rectangle rSep = new Rectangle(rItem.Left, rItem.Top, rItem.Width, 1);
                g.FillRect(rSep, Gdi.CreateSolidBrush(Color.FromArgb(205, 205, 205)));

                int topIndent = rItem.Top + item.InfoTopIndents[0];
                int leftIndent = rItem.Left + item.InfoLeftIndents[0];

                if (nItem == 1 || nItem == 2)
                {
                    topIndent += Settings.GroupPixHeight;
                }

                if (!string.IsNullOrEmpty(item.PrimaryText))
                {
                    g.Font = _priFont;
                    g.TextAlign = Win32.TextAlign.TA_LEFT;
                    g.TextColor = Color.Black;

                    if (isSelected)
                    {
                        g.TextColor = Color.White;
                    }

                    g.ExtTextOut(leftIndent, topIndent, item.PrimaryText);
                }

                leftIndent = rItem.Left + item.InfoLeftIndents[0];
                topIndent = rItem.Top + item.InfoTopIndents[1];

                if (nItem == 2)
                {
                    topIndent += Settings.GroupPixHeight;
                }

                if (item.SecondaryTextLines != null && item.SecondaryTextLines.Count > 0)
                {
                    g.Font = _secFont;
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

                // дата
                topIndent = rItem.Top + item.InfoTopIndents[0];
                leftIndent = rItem.Right - item.InfoLeftIndents[2];

                if (nItem == 2)
                {
                    topIndent += Settings.GroupPixHeight;
                }

                if (!string.IsNullOrEmpty(item.TertiaryText))
                {
                    g.Font = _secFont;
                    g.TextAlign = Win32.TextAlign.TA_RIGHT;
                    g.TextColor = Color.FromArgb(51, 153, 255);

                    if (isSelected)
                    {
                        g.TextColor = Color.White;
                    }

                    g.ExtTextOut(leftIndent, topIndent, item.TertiaryText);
                }
            }
        }       

        #endregion
    }
}
