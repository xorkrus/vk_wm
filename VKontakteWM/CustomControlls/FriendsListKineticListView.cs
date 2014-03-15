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
using Galssoft.VKontakteWM.Components.ImageClass;
using Galssoft.VKontakteWM.Components.MVC;
using System.Runtime.InteropServices;
using System.IO;

namespace Galssoft.VKontakteWM.CustomControlls
{
    class FriendsListKineticListView : KineticListView<FriendListViewItem>
    {
        #region Members

        //

        #endregion

        #region Constructors

        public FriendsListKineticListView()
            : base(KineticControlScrollType.Vertical, new KineticControlsSetting
        {
            PrimaryTextFontGdi = FontCache.CreateFont("Tahoma", 13, FontStyle.Bold, true),
            SecondaryTextFontGdi = FontCache.CreateFont("Tahoma", 13, FontStyle.Regular, true),
            ThirdTextFontGdi = FontCache.CreateFont("Tahoma", 11, FontStyle.Regular, true),
            GroupFont = FontCache.CreateFont("Tahoma", 14, FontStyle.Regular, true),

            TextPixMargin = 0,

            ListItemPixSize = 50,

            GroupPixTopMargin = 0,

            GroupPixHeight = 16
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
        public List<FriendListViewItem> DataSource
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
                return Resources.FriendsListKineticListView_IfEmptyText;
            }
        }

        /// <summary>
        /// Прогружает изображение (группу изображений), проставляя PrimaryImageURL.
        /// Этот метод мы вызываем при прогрузке очередного изображения в кэш
        /// </summary>
        /// <param name="item"></param>
        public void UpdateUserPhoto(FriendListViewItem item)
        {
            for (int i = 0; i < NativeItems.Count; i++)
            {
                var ni = NativeItems[i];

                if (ni.Uid == item.Uid)
                {
                    ni.PrimaryImageURL = item.Avatar;

                    //this.Refresh();

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

                string avatar = string.Empty;

                if (item.IsAvatarLoaded)
                {
                    if (File.Exists(item.Avatar))
                    {
                        avatar = item.Avatar;
                    }
                    else
                    {
                        avatar = "clear";
                    }
                }
                else
                {
                    avatar = "clear";
                }

                ni.PrimaryImageURL = avatar;
            }
        }

        protected override List<NativeItemData> BuildNativeControlItems()
        {
            var newListData = new List<NativeItemData>(Items.Count);
           
            if (Items != null)
            {
                IImage secondaryIImage = MasterForm.SkinManager.GetImage("SendMessage");

                for (int i = 0; i < Items.Count; i++)
                {
                    string avatar = string.Empty;

                    if (Items[i].IsAvatarLoaded)
                    {
                        avatar = Items[i].Avatar;
                    }
                    else
                    {
                        avatar = "clear";
                    }

                    string tertiaryText = "";
                    if (Items[i].IsOnline) tertiaryText = "online";

                    NativeItemData newNativeItemData = new KineticListView<FriendListViewItem>.NativeItemData
                    {
                        Group = Items[i].Group,

                        PrimaryText = Items[i].LastName + " ",
                        SecondaryText = Items[i].FirstName,
                        TertiaryText = tertiaryText,

                        Uid = Items[i].Uid,
                        //Tag = Items[i].IsDataForAddressBook,
                        Tag = Items[i].Uid,

                        PrimaryImageURL = avatar,
                        SecondaryImage = secondaryIImage,

                        InfoLeftIndents = new int[3],
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

            nativeItem.InfoLeftIndents[0] = UISettings.CalcPix(0); //Аватарка
            nativeItem.InfoLeftIndents[1] = UISettings.CalcPix(60); //Имя
            nativeItem.InfoLeftIndents[2] = UISettings.CalcPix(60); //online

            nativeItem.InfoTopIndents[0] = UISettings.CalcPix(0); //Автарка
            nativeItem.InfoTopIndents[1] = UISettings.CalcPix(15); //Имя
            nativeItem.InfoTopIndents[2] = UISettings.CalcPix(25 + 3); //online
        }

        protected override void DrawGroupHeaderOn(Gdi gMem, int item, Rectangle rHeaderArea)
        {
            //// Draw Group Header Text
            //gMem.Font = Settings.GroupFont;
            //gMem.TextAlign = Win32.TextAlign.TA_LEFT;
            ////gMem.TextColor = Color.FromArgb(150,150,150);
            ////gMem.ExtTextOut(rHeaderArea.Left + UISettings.CalcPix(5) + 1,
            ////                rHeaderArea.Top + Settings.GroupPixTopMargin + 1,
            ////                GetItemAtIndex(item).Group, true);

            //gMem.TextColor = Settings.ListGroupTextColor;
            //gMem.ExtTextOut(rHeaderArea.Left + UISettings.CalcPix(5),
            //                rHeaderArea.Top + Settings.GroupPixTopMargin,
            //                GetItemAtIndex(item).Group, true);

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
                rItem = new Rectangle(rItem.Left, rItem.Top + rItem.Height - Settings.ListItemPixSize, rItem.Width, Settings.ListItemPixSize);
            }

            //Фон
            DrawItemBackgroundOn(g, item, rItem, nItem, isSelected);

            //Аватарка
            int leftIndent = rItem.Left + item.InfoLeftIndents[0];
            int topIndent = rItem.Top + item.InfoTopIndents[0];

            #region выводим изображение

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

            //Имя
            leftIndent = rItem.Left + item.InfoLeftIndents[1];
            topIndent = rItem.Top + item.InfoTopIndents[1];
            if (!string.IsNullOrEmpty(item.PrimaryText) && !string.IsNullOrEmpty(item.SecondaryText))
            {                
                g.TextAlign = Win32.TextAlign.TA_LEFT;
                if (isSelected) g.TextColor = Color.White;
                else g.TextColor = Settings.ListItemTextColor;
                g.Font = Settings.PrimaryTextFontGdi;
                Size size = g.GetTextExtent(item.PrimaryText);
                g.ExtTextOut(leftIndent, topIndent, item.PrimaryText);
                leftIndent += size.Width;
                
                g.Font = Settings.SecondaryTextFontGdi;
                g.ExtTextOut(leftIndent, topIndent, item.SecondaryText);
            }

            //Online
            leftIndent = rItem.Left + item.InfoLeftIndents[2];
            topIndent = rItem.Top + item.InfoTopIndents[2];
            if (!string.IsNullOrEmpty(item.TertiaryText))
            {
                g.Font = Settings.ThirdTextFontGdi;
                g.TextAlign = Win32.TextAlign.TA_LEFT;
                if (isSelected) g.TextColor = Color.White;
                else g.TextColor = Color.FromArgb(150, 150, 150);
                g.ExtTextOut(leftIndent, topIndent, item.TertiaryText);
            }
        }

        protected override void OnItemSelected(int index, Point mousePosition)
        {
            //if (index < 0)
            //{
            //    return;
            //}

            //int UserID = 0; ;

            //try
            //{
            //    UserID = Convert.ToInt32(NativeItems[index].Tag);
            //}
            //catch
            //{
            //    return;
            //}

            //NavigationService.Controllers["FriendsListController"].Initialize(UserID);
            base.OnItemSelected(index, mousePosition);
        }

        #endregion
    }
}
