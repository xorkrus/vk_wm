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

namespace Galssoft.VKontakteWM.CustomControlls
{
    class WallPostListKineticListView : KineticListView<WallPostListViewItem>
    {
        #region Members

        //

        #endregion

        #region Constructors

        public WallPostListKineticListView()
            : base(KineticControlScrollType.Vertical, new KineticControlsSetting
                  {
                      //PrimaryTextFontGdi = FontCache.CreateFont("Calibri", 18, FontStyle.Regular, true)
                  })
        {
            EnableLongPress = true;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Источник данных для списка
        /// </summary>
        public List<WallPostListViewItem> DataSource
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
                return Resources.WallPostListKineticListView_IfEmptyText;
            }
        }

        protected override List<NativeItemData> BuildNativeControlItems()
        {
            var retVal = new List<NativeItemData>(Items.Count);

            if (Items != null)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    IImagingFactory newImagingFactory = ImagingFactory.GetImaging();

                    IImage newIImage;

                    if (Items[i].IsAvatarLoaded)
                    {
                        newImagingFactory.CreateImageFromFile(Items[i].Avatar, out newIImage);
                    }
                    else
                    {
                        newIImage = MasterForm.SkinManager.GetImage("AvatarStub");
                    }

                    NativeItemData newNativeItemData = new KineticListView<WallPostListViewItem>.NativeItemData
                    {
                        PrimaryText = Items[i].UserName,
                        SecondaryText = Items[i].Status,
                        TertiaryText = Items[i].StatusChangeDate,

                        PrimaryImage = newIImage,

                        InfoLeftIndents = new int[2],
                        InfoTopIndents = new int[1]

                    };

                    retVal.Add(newNativeItemData);
                }

            }

            #region старая версия
            //if (Items != null)
            //{
            //    for (int i = 0; i < Items.Count; i++)
            //    {
            //        IImagingFactory iImagingFactory = ImagingFactory.GetImaging();

            //        //Привязка аватарки пользователя, если она еще не загружена, выводится заглушка
            //        IImage primaryImage;
            //        if (Items[i].IsAvatarLoaded)
            //            iImagingFactory.CreateImageFromFile(Items[i].Avatar, out primaryImage);
            //        else
            //            primaryImage = MasterForm.SkinManager.GetImage("AvatarStub");

            //        //Значек подарка или online
            //        /*
            //        IImage secondaryImage;
            //        if (Items[i].IsBirthday)
            //            secondaryImage = MasterForm.SkinManager.GetImage("Birthday");
            //        else if (Items[i].StatusOnline)
            //            secondaryImage = MasterForm.SkinManager.GetImage("Online");
            //        else secondaryImage = null;
            //        */

            //        //Статус пользователя
            //        string status = Items[i].Status;
            //        NativeItemData data = new NativeItemData
            //        {
            //            PrimaryText = Items[i].Name,
            //            SecondaryText = status,
            //            Tag = Items[i].Name,
            //            PrimaryImage = primaryImage,
            //            //SecondaryImage = secondaryImage,
            //            InfoLeftIndents = new int[2],
            //            InfoTopIndents = new int[1]
            //        };
            //        retVal.Add(data);
            //    }
            //}
            #endregion

            return retVal;
        }

        protected override void GenerateListItems()
        {
            //Здесь можно что либо сделать с источником данных
            //Или как ваориант откуда нибудь его загрузить самому
            //

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

            //Если выделен
            DrawItemBackgroundOn(g, item, rItem, nItem, SelectedIndex == nItem);

            var rSep = new Rectangle(rItem.Left, rItem.Bottom - 1, rItem.Width, 1);

            //полоска
            g.FillRect(rSep, Settings.ListItemSeparator);

            //
            int leftIndent = rItem.Left + Settings.TextPixMargin;
            int topIndent = rItem.Bottom - rItem.Height + item.InfoTopIndents[0];

            // write name

            //Отрисовка изображения
            if (item.PrimaryImage != null)
            {
                g.DrawImageAlphaChannel(item.PrimaryImage, leftIndent, topIndent);
            }

            leftIndent += Settings.SecondaryIconPixWidth + Settings.TextPixMargin;

            //определение размеров текста g.GetTextExtent
            /*
            Size textSize = g.GetTextExtent(vals[0] + " ");
            g.ExtTextOut(rItem.Left + rItem.Width / 2 - textSize.Width,
                         rItem.Bottom - item.InfoTopIndents[0],
                         textSize.Width,
                         vals[0]);
            */

            if (!string.IsNullOrEmpty(item.PrimaryText))
            {
                g.Font = Settings.PrimaryTextFontGdi;
                g.TextAlign = Win32.TextAlign.TA_LEFT;
                g.TextColor = Settings.ListItemTextColor;
                g.ExtTextOut(leftIndent, topIndent, item.PrimaryText);
            }

            if (!string.IsNullOrEmpty(item.SecondaryText))
            {
                g.Font = Settings.SecondaryTextFontGdi;
                g.TextAlign = Win32.TextAlign.TA_LEFT;
                g.TextColor = Settings.ListItemTextColor;
                g.ExtTextOut(leftIndent, topIndent + 10,item.SecondaryText);
            }

            if (!string.IsNullOrEmpty(item.SecondaryText))
            {
                g.Font = Settings.SecondaryTextFontGdi;
                g.TextAlign = Win32.TextAlign.TA_LEFT;
                g.TextColor = Settings.ListItemTextColor;
                g.ExtTextOut(leftIndent, topIndent + 20, item.TertiaryText);
            }

            /*
            if(item.SecondaryImage != null)
            {
                g.DrawImageAlphaChannel(item.SecondaryImage, rItem.Right - Settings.SecondaryIconPixWidth + Settings.TextPixMargin, topIndent);
            }
            */
        }

//        /*
//protected override void OnItemSelected(int index, Point mousePosition)
//{
            
//    if (index < 0)
//        return;

//    //if (((FriendListViewItem)NativeItems[index]).Name)
//        //return;
            
//    if (mousePosition.X >= NativeItems[index].InfoLeftIndents[3] && mousePosition.X < NativeItems[index].InfoLeftIndents[4])
//    {
//        //MoveUp(index);
//    }
//    else if (mousePosition.X >= NativeItems[index].InfoLeftIndents[4])
//    {
//        //MoveDown(index);
//    }
//    else if (mousePosition.X > NativeItems[index].InfoLeftIndents[0] && mousePosition.X < NativeItems[index].InfoLeftIndents[1])
//    {
//        //Rename(index);
//    }
//    else
//        OnItemBodyClicked(index);
            
//}
//*/
        #endregion
    }
}
