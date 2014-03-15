using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.ImageClass;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.ApplicationLogic;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using System.IO;

namespace Galssoft.VKontakteWM.CustomControlls
{
    class ImagesKineticListView : KineticListView<PhotosUpdatesListViewItem>
    {
        ///// <summary>
        ///// Helper class for storing prepared data to display on screen 
        ///// </summary>
        //protected class ImageNativeItemData : NativeItemData, IDisposable
        //{
        //    public int ItemWidth { get; set; }

        //    public void Dispose()
        //    {
        //        if (SecondaryImage != null)
        //        {
        //            Marshal.ReleaseComObject(SecondaryImage);
        //            SecondaryImage = null;
        //        }
        //    }
        //}

        #region Members

        private readonly int _imageMargin;

        private readonly IImage _leftButton;
        private readonly IImage _rightButton;

        private Size _leftButtonSize;
        private Size _rightButtonSize;

        #endregion

        #region Constructors

        public ImagesKineticListView()
            : base(KineticControlScrollType.Horizontal, new KineticControlsSetting())
        {
            Settings.ListItemPixSize = Height;
            _imageMargin = UISettings.CalcPix(5);
            ShowGroupHeader = false;
            ShowScrollbar = false;

            _leftButton = MasterForm.SkinManager.GetImage("KineticControlLeft");
            _rightButton = MasterForm.SkinManager.GetImage("KineticControlRight");

            if (_leftButton != null)
            {
                ImageInfo ii;
                _leftButton.GetImageInfo(out ii);
                _leftButtonSize = new Size((int) ii.Width, (int) ii.Height);
            }
            else
            {
                _leftButtonSize = Size.Empty;
            }

            if (_rightButton != null)
            {
                ImageInfo ii;
                _rightButton.GetImageInfo(out ii);
                _rightButtonSize = new Size((int)ii.Width, (int)ii.Height);
            }
            else
            {
                _rightButtonSize = Size.Empty;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Источник данных для списка
        /// </summary>
        public List<PhotosUpdatesListViewItem> DataSource
        {
            get;
            set;
        }

        #endregion

        #region Protected Methods

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            Settings.ListItemPixSize = Height;
        }

        protected override string IfEmptyText
        {
            get
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Прогружает изображение (группу изображений), проставляя PrimaryImageURL.
        /// Этот метод мы вызываем при прогрузке очередного изображения в кэш
        /// </summary>
        /// <param name="item"></param>
        public void UpdateUserPhoto(PhotosUpdatesListViewItem item)
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

                    NativeItemData newNativeItemData = new KineticListView<PhotosUpdatesListViewItem>.NativeItemData
                    {
                        Uid = Items[i].Uid,                                                
                        PrimaryImageURL = photo,
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

        protected override void CalculateItemsSize()
        {
            if (ItemCount > 0)
            {
                List<int> sizes = new List<int>();

                for (int i = 0; i < ItemCount; i++)
                {
                    sizes.Add(UISettings.CalcPix(50) + _imageMargin);
                }

                CalculateItemSizeForListSize(Settings.ListItemPixSize, sizes);
            }
            else
            {
                CalculateItemSizeForListSize(Settings.ListItemPixSize, null);
            }
        }

        protected override void DrawScreenOn(Gdi gMem, Rectangle rClip, int yListOffset)
        {
            base.DrawScreenOn(gMem, rClip, yListOffset);

            if (yListOffset > 0)
            {
                if (_leftButton != null)
                {
                    gMem.DrawImageAlphaChannel(_leftButton, rClip.Left + _leftButtonSize.Width, rClip.Top + (Height - _leftButtonSize.Height) / 2);
                }
            }

            if (yListOffset + rClip.Right - rClip.Left < _ActiveListWidth)
            {
                if (_rightButton != null)
                {
                    gMem.DrawImageAlphaChannel(_rightButton, rClip.Right - _rightButtonSize.Width, rClip.Top + (Height - _rightButtonSize.Height) / 2);
                }
            }
        }

        /// <summary>
        /// Draw background
        /// </summary>
        /// <param name="gMem">Graphics</param>
        /// <param name="item">Prepeared native item</param>
        /// <param name="rItem">Area to draw</param>
        /// <param name="nItem">Index of the item in the native items list</param>
        /// <param name="isSelected">Is item selected</param>
        protected override void DrawItemBackgroundOn(Gdi gMem, NativeItemData item, Rectangle rItem, int nItem, bool isSelected)
        {
            if (isSelected)
            {
                gMem.GradientFill(rItem, Color.FromArgb(69, 137, 219), Color.FromArgb(50, 106, 173),
                                  FillDirection.TopToBottom);
            }
                /*
            else
            {
                BrushGdi backGround = nItem % 2 == 0 ? Settings.ListItemBackgroundEven : Settings.ListItemBackgroundOdd;
                if (backGround != BrushGdi.Empty)
                    gMem.FillRect(rItem, backGround);
            }
                 * */
        }

        protected override void DrawItemOn(Gdi g, NativeItemData nativeItem, Rectangle rItem, int nItem)
        {
            //ImageNativeItemData item = (ImageNativeItemData) nativeItem;

            NativeItemData item = nativeItem;

            if (rItem.Height > Settings.ListItemPixSize)
            {
                rItem = new Rectangle(rItem.Left, rItem.Top + rItem.Height - Settings.ListItemPixSize, rItem.Width, Settings.ListItemPixSize);
            }

            //выделение
            DrawItemBackgroundOn(g, item, rItem, nItem, SelectedIndex == nItem);

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

                    imageMarginLeft = (rItem.Width - (int)newImageInfo.Width) / 2;
                    imageMarginTop = (rItem.Height - (int)newImageInfo.Height) / 2;

                    g.DrawImageAlphaChannel(item.PrimaryImage, rItem.Left + imageMarginLeft, rItem.Top + imageMarginTop);
                }
            }

            #endregion

            ////изображение
            //IImage img = item.SecondaryImage ?? item.PrimaryImage;

            //if (img != null)
            //{
            //    g.DrawImageAlphaChannel(img, rItem.Left + _imageMargin, rItem.Top + _imageMargin);
            //}
        }

        #endregion
    }
}
