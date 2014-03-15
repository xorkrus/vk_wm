using System;

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
using System.Runtime.InteropServices;
using System.IO;

namespace Galssoft.VKontakteWM.CustomControlls
{
    class PhotoCommentsUpdatesListKineticListView : KineticListView<PhotoCommentsUpdatesViewItem>
    {
        #region Members

        //

        #endregion

        #region Constructors

        public PhotoCommentsUpdatesListKineticListView()
            : base(KineticControlScrollType.Vertical, new KineticControlsSetting
                  {
                      PrimaryTextFontGdi = FontCache.CreateFont("Tahoma", 12, FontStyle.Bold, true),
                      SecondaryTextFontGdi = FontCache.CreateFont("Tahoma", 11, FontStyle.Regular, true),
                      ThirdTextFontGdi = FontCache.CreateFont("Tahoma", 11, FontStyle.Regular, true),
                      GroupFont = FontCache.CreateFont("Tahoma", 12, FontStyle.Regular, true),

                      TextPixMargin = 0,

                      ListItemPixSize = 50,

                      GroupPixTopMargin = 0,

                      GroupPixHeight = 16,

                      SecondaryTextLinesCount = 2
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
        public List<PhotoCommentsUpdatesViewItem> DataSource
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
                return Resources.PhotoCommentsUpdatesListKineticListView_IfEmptyText;
            }
        }

        /// <summary>
        /// Прогружает изображение (группу изображений), проставляя PrimaryImageURL.
        /// Этот метод мы вызываем при прогрузке очередного изображения в кэш
        /// </summary>
        /// <param name="item"></param>
        public void UpdateUserPhoto(PhotoCommentsUpdatesViewItem item)
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
                }

                ni.PrimaryImageURL = "clear";
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

                string photo = string.Empty;

                if (item.IsPhotoLoaded)
                {
                    if (File.Exists(item.Photo))
                    {
                        photo = item.Photo;
                    }
                    else
                    {
                        photo = "clear";
                    }
                }
                else
                {
                    photo = "clear";
                }

                ni.PrimaryImageURL = photo;
            }
        }

        protected override List<NativeItemData> BuildNativeControlItems()
        {
            var newListData = new List<NativeItemData>(Items.Count);
           
            if (Items != null)
            {
                var hdcMem = OffScreenGraphics.GetHdc();

                try
                {
                    using (Gdi g = Gdi.FromHdc(hdcMem, Rectangle.Empty))
                    {
                        g.Font = Settings.SecondaryTextFontGdi;
                        g.TextAlign = Win32.TextAlign.TA_LEFT;

                        for (int i = 0; i < Items.Count; i++)
                        {
                            string photo = string.Empty;

                            if (Items[i].IsPhotoLoaded)
                            {
                                photo = Items[i].Photo;
                            }
                                else
                            {
                                photo = "clear";
                            }

                            NativeItemData newNativeItemData = new KineticListView<PhotoCommentsUpdatesViewItem>.NativeItemData
                            {
                                Uid = Items[i].Uid,
                                Group = Items[i].Group,

                                PrimaryText = Items[i].SenderName,
                                SecondaryText = Items[i].Comment,
                                TertiaryText = Items[i].CommentSetDate.ToString("HH:mm"),

                                SecondaryTextLines = TextFormatHelper.CutTextToLines(Items[i].Comment, UISettings.CalcPix(170), 2, g),

                                PrimaryImageURL = photo,

                                InfoLeftIndents = new int[3],
                                InfoTopIndents = new int[2]
                            };

                            newListData.Add(newNativeItemData);
                        }
                    }
                }
                finally
                {
                    OffScreenGraphics.ReleaseHdc(hdcMem);
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

            nativeItem.InfoLeftIndents[0] = UISettings.CalcPix(6);//Картинка
            nativeItem.InfoLeftIndents[1] = UISettings.CalcPix(60);//Имя и Комментарий
            nativeItem.InfoLeftIndents[2] = UISettings.CalcPix(10);//Дата, отступ справа

            nativeItem.InfoTopIndents[0] = UISettings.CalcPix(5);//Картинка, дата и имя
            nativeItem.InfoTopIndents[1] = UISettings.CalcPix(20);//Комментарий
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
            NativeItemData item = nativeItem;
            bool isSelected = ShowSelectedItem ? nItem == _SelectedIndex : false;

            if (rItem.Height > Settings.ListItemPixSize)
            {
                rItem = new Rectangle(rItem.Left,
                                        rItem.Top + rItem.Height - Settings.ListItemPixSize,
                                        rItem.Width,
                                        Settings.ListItemPixSize);
            }

            //фон
            DrawItemBackgroundOn(g, item, rItem, nItem, isSelected);

            //Картинка
            int topIndent = rItem.Top;
            int leftIndent = rItem.Left;

            #region выводим изображения
            int index = -1;

            for (int i = 0; i < NativeItems.Count; i++)
            {
                var ni = NativeItems[i];

                if (ni.Uid == item.Uid)
                {
                    index = i;

                    break;
                }
            }

            if (index > -1)
            {
                if (IsItemVisible(index))
                {
                    if (!string.IsNullOrEmpty(item.PrimaryImageURL))
                    {
                        if (!item.PrimaryImageURL.Equals("clear"))
                        {
                            IImage newIImage = null;

                            //ImageHelper.LoadImageFromFile(item.PrimaryImageURL, out newIImage);

                            //item.PrimaryImage = newIImage;

                            if (File.Exists(item.PrimaryImageURL))
                            {
                                ImageHelper.LoadImageFromFile(item.PrimaryImageURL, out newIImage);

                                item.PrimaryImage = newIImage;
                            }
                            else
                            {
                                item.PrimaryImage = MasterForm.SkinManager.GetImage("ImageNull");
                            }
                        }
                        else
                        {
                            item.PrimaryImage = MasterForm.SkinManager.GetImage("ImageNull");
                        }

                        item.PrimaryImageURL = string.Empty;
                    }

                    int imageMarginLeft = 0;
                    int imageMarginTop = 0;

                    ImageInfo newImageInfo;

                    item.PrimaryImage.GetImageInfo(out newImageInfo);

                    imageMarginLeft = 0;
                    imageMarginTop = 0;

                    g.DrawImageAlphaChannel(item.PrimaryImage, leftIndent + imageMarginLeft, topIndent + imageMarginTop);
                }
            }
            #endregion

            //полоска
            var rSep = new Rectangle(rItem.Left, rItem.Top, rItem.Width, 1);
            g.FillRect(rSep, Settings.ListItemSeparator);

            //имя
            topIndent = rItem.Top + item.InfoTopIndents[0];
            leftIndent = rItem.Left + item.InfoLeftIndents[1];
            if (!string.IsNullOrEmpty(item.PrimaryText))
            {
                g.Font = Settings.PrimaryTextFontGdi;
                g.TextAlign = Win32.TextAlign.TA_LEFT;
                if (isSelected) g.TextColor = Color.White;
                else g.TextColor = Settings.ListItemTextColor;
                g.ExtTextOut(leftIndent, topIndent, item.PrimaryText);
            }

            //дата
            topIndent = rItem.Top + item.InfoTopIndents[0];
            leftIndent = rItem.Right - item.InfoLeftIndents[2];
            if (!string.IsNullOrEmpty(item.TertiaryText))
            {
                g.Font = Settings.ThirdTextFontGdi;
                g.TextAlign = Win32.TextAlign.TA_RIGHT;
                if (isSelected) g.TextColor = Color.White;
                else g.TextColor = Color.FromArgb(51, 153, 255);
                g.ExtTextOut(leftIndent, topIndent, item.TertiaryText);
            }

            //Комментарий
            topIndent = rItem.Top + item.InfoTopIndents[1];
            leftIndent = rItem.Left + item.InfoLeftIndents[1];
            if (!string.IsNullOrEmpty(item.SecondaryText))
            {
                //g.Font = Settings.SecondaryTextFontGdi;
                //g.TextAlign = Win32.TextAlign.TA_LEFT;
                //if (isSelected) g.TextColor = Color.White;
                //else g.TextColor = Color.FromArgb(102, 102, 102);

                //int colPrev = 0;
                //int line = Settings.SecondaryTextLinesCount;

                //do
                //{
                //    int col = item.SecondaryText.IndexOf('\n', colPrev);

                //    string text;

                //    if (col > -1)
                //    {
                //        text = item.SecondaryText.Substring(colPrev, col - colPrev);
                //    }
                //    else
                //    {
                //        text = item.SecondaryText.Substring(colPrev);
                //    }

                //    g.ExtTextOut(leftIndent, topIndent, text);

                //    colPrev = col + 1;

                //    line--;

                //    topIndent += UISettings.CalcPix(11);
                //}
                //while (colPrev > 0 && line > 0);

                if (item.SecondaryTextLines.Count > 0)
                {
                    g.Font = Settings.SecondaryTextFontGdi;
                    g.TextAlign = Win32.TextAlign.TA_LEFT;
                    g.TextColor = Color.FromArgb(102, 102, 102);

                    if (isSelected) g.TextColor = Color.White;

                    foreach (string line in item.SecondaryTextLines)
                    {
                        g.ExtTextOut(leftIndent, topIndent, line);

                        topIndent += UISettings.CalcPix(11);
                    }
                }  
            }
        }

        #endregion
    }
}
