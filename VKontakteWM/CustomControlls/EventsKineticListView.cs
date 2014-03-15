using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.ApplicationLogic;
using Galssoft.VKontakteWM.Components;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;
using Galssoft.VKontakteWM.Components.UI.Wrappers;
using Galssoft.VKontakteWM.Forms;

namespace Galssoft.VKontakteWM.CustomControlls
{
    class EventsKineticListView : KineticListView<EventButton>
    {
        protected class NativeItemDataMm : NativeItemData
        {
            public IImage Icon { get; set; }
            public new Bitmap PrimaryImage { get; set; }
        }

        #region Constructors

        public EventsKineticListView()
            : base(KineticControlScrollType.Vertical, new KineticControlsSetting
            {
                SecondaryIconPixWidth = 36,
                ListItemPixSize = 37,
                PrimaryIconPixWidth = 24,
                PrimaryIconPixHeight = 24
            })
        {
            ShowScrollbar = true;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Источник данных для списка
        /// </summary>
        public List<EventButton> DataSource
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
                string tag;
                string primaryText;
                if (item.Count > 0)
                {
                    primaryText = item.Text + " (+" + item.Count + ")";
                    tag = "New";
                }
                else
                {
                    tag = "";
                    primaryText = item.Text;
                }
                //item.Button
                var data = new NativeItemDataMm
                {
                    PrimaryText = primaryText,
                    SecondaryText = item.Event.ToString(),
                    Tag = tag,
                    PrimaryImage = null,
                    SecondaryImage = null,
                    Icon = item.Icon
                };
                retVal.Add(data);
            }
            return retVal;
        }

        protected override void GenerateListItems()
        {
            Items.Clear();
            if (DataSource != null)
                Items.AddRange(DataSource);
        }

        private Bitmap _paintedButton;

        protected override void DrawItemOn(Gdi g, NativeItemData nativeitem, Rectangle rItem, int nItem)
        {
            var item = (NativeItemDataMm)nativeitem;

            bool isSelected = ShowSelectedItem ? nItem == _SelectedIndex : false;

            //var primaryRect = new Rectangle(rItem.Left, rItem.Top, rItem.Width, rItem.Height - UISettings.CalcPix(5)); 
            rItem = new Rectangle(rItem.Left, rItem.Top, rItem.Width, rItem.Height - UISettings.CalcPix(5));

            DrawItemBackgroundOn(g, item, rItem, nItem, isSelected);

            int iconMargin = UISettings.CalcPix(15);
            int iconX = rItem.Left + iconMargin;

            // draw icon
            if (item.Icon != null)
            {
                ImageInfo iinfo;
                item.Icon.GetImageInfo(out iinfo);
                int iconY = rItem.Top + (rItem.Height - (int)iinfo.Height) / 2;

                if (item.PrimaryImage == null || isSelected)
                {
                    g.DrawImageAlphaChannel(item.Icon, iconX, iconY);

                    if (item.PrimaryImage == null & iconY > 0)
                    {
                        item.PrimaryImage = new Bitmap((int)iinfo.Width, (int)iinfo.Height);
                        g.CopyImageTo(item.PrimaryImage, 0, 0, Settings.PrimaryIconPixWidth, Settings.PrimaryIconPixHeight, iconX, iconY);
                    }
                }
                else
                {
                    g.DrawImage(item.PrimaryImage, iconX, iconY);
                }
            }

            if (!String.IsNullOrEmpty(item.PrimaryText))
            {

                if (item.Tag.ToString() == "New")
                {
                    g.Font = FontCache.CreateFont("Tahoma", 16, FontStyle.Bold, true);
                    if (isSelected) g.TextColor = Color.FromArgb(255, 255, 255);
                    else g.TextColor = Color.FromArgb(245, 36, 43);
                }
                else
                {
                    g.Font = FontCache.CreateFont("Tahoma", 16, FontStyle.Regular, true);
                    if (isSelected) g.TextColor = Color.FromArgb(255, 255, 255);
                    else g.TextColor = Color.FromArgb(119, 126, 93);
                }
                g.TextAlign = Win32.TextAlign.TA_LEFT;
                Size textSize = g.GetTextExtent(item.PrimaryText);

                g.ExtTextOut(iconX + Settings.PrimaryIconPixWidth + UISettings.CalcPix(10), rItem.Top + (rItem.Height - textSize.Height) / 2, item.PrimaryText);
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
            //const int height = 36;

            /*if (_paintedButton == null)
            {
                _paintedButton = new Bitmap(rItem.Width, rItem.Height-4);

                Graphics graphics = null;
                IntPtr hdc = IntPtr.Zero;
                try
                {
                    graphics = Graphics.FromImage(_paintedButton);
                    hdc = graphics.GetHdc();
                    Rectangle rect = new Rectangle(0, 0, rItem.Width, rItem.Height);
                    //using (Gdi g = Gdi.FromHdc(hdc, rect))
                    using (Gdi g = Gdi.FromHdc(hdc, rItem))
                    {
                        //Белый фон
                        g.GradientFill(rItem, Color.White, Color.White, FillDirection.TopToBottom);

                        //DrawWideButton(new Rectangle(0, 0, rItem.Width, height), g, MasterForm.SkinManager.GetImage("EventList1"), MasterForm.SkinManager.GetImage("EventList3"), MasterForm.SkinManager.GetImage("EventList2"));
                        DrawWideButton(rItem, g, MasterForm.SkinManager.GetImage("EventList1"), MasterForm.SkinManager.GetImage("EventList3"), MasterForm.SkinManager.GetImage("EventList2"));
                    }
                }
                catch(Exception ex)
                {
                    _paintedButton = null;
                    MessageBox.Show(ex.ToString());
                    throw;
                }
                finally
                {
                    if (graphics != null)
                    {
                        graphics.ReleaseHdc(hdc);
                        graphics.Dispose();
                    }
                }
            }*/

            if (_paintedButton == null || isSelected)
            {
                if (isSelected)
                    DrawWideButton(rItem, gMem, MasterForm.SkinManager.GetImage("EventList1Pressed"),
                        MasterForm.SkinManager.GetImage("EventList3Pressed"), MasterForm.SkinManager.GetImage("EventList2Pressed"));
                else
                    DrawWideButton(rItem, gMem, MasterForm.SkinManager.GetImage("EventList1"),
                        MasterForm.SkinManager.GetImage("EventList3"), MasterForm.SkinManager.GetImage("EventList2"));
                //gMem.DrawImageAlphaChannel(item.SecondaryImage, rItem.Left, rItem.Top);

                if (_paintedButton == null & !isSelected & rItem.Top > 0)
                {
                    _paintedButton = new Bitmap(rItem.Width, rItem.Height);
                    gMem.CopyImageTo(_paintedButton, 0, 0, rItem.Width, rItem.Height,
                        rItem.Left, rItem.Top);
                }
            }
            else
                gMem.DrawImage(_paintedButton, rItem.Left, rItem.Top);

            /*if (isSelected)
            {
                DrawWideButton(rItem, gMem, MasterForm.SkinManager.GetImage("EventList1"), MasterForm.SkinManager.GetImage("EventList3"), MasterForm.SkinManager.GetImage("EventList2"));
            }
            else
                gMem.DrawImage(_paintedButton, rItem.Left, rItem.Top);*/
        }

        private static void DrawWideButton(Rectangle rItem, Gdi g, IImage imgL, IImage imgR, IImage imgC)
        {
            ImageInfo iiL;
            ImageInfo iiR;
            imgL.GetImageInfo(out iiL);
            imgR.GetImageInfo(out iiR);

            int x = rItem.Left + 4;
            int y = rItem.Top;

            // draw left part
            g.DrawImageAlphaChannel(imgL, new Rectangle(x, y, (int)iiL.Width, rItem.Height));

            // draw tiled center
            int xR = rItem.Width - 4 - (int)iiR.Width - (int)iiL.Width;
            g.DrawImageAlphaChannel(imgC, new Rectangle(x + (int)iiL.Width, y, xR, rItem.Height));

            // draw right part
            xR = rItem.Width - 4 - (int)iiR.Width;
            g.DrawImageAlphaChannel(imgR, new Rectangle(x + xR, y, (int)iiR.Width, rItem.Height));
        }

        protected override void OnItemSelected(int index, Point mousePosition)
        {
            if (index < 0)
                return;

            //using (new WaitWrapper(false))
            {
                //if (NativeItems[index].SecondaryText == EventType.Messages.ToString())
                //{
                //    NavigationService.Controllers["MainController"].Initialize("GoToMessages");
                //}
                //else if (NativeItems[index].SecondaryText == EventType.Guests.ToString())
                //{
                //    NavigationService.Controllers["MainController"].Initialize("GoToGuests");
                //}
                //else if (NativeItems[index].SecondaryText == EventType.Marks.ToString())
                //{
                //    NavigationService.Controllers["MainController"].Initialize("GoToMarks");
                //}
                //else if (NativeItems[index].SecondaryText == EventType.Notifications.ToString())
                //{
                //    NavigationService.Controllers["MainController"].Initialize("GoToNotifications");
                //}
                //else if (NativeItems[index].SecondaryText == EventType.Activities.ToString())
                //{
                //    NavigationService.Controllers["MainController"].Initialize("GoToActivities");
                //}
                //else if (NativeItems[index].SecondaryText == EventType.Discussions.ToString())
                //{
                //    NavigationService.Controllers["MainController"].Initialize("GoToDiscussions");
                //}

                base.OnItemSelected(index, mousePosition);
            }
        }

        #endregion
    }

}
