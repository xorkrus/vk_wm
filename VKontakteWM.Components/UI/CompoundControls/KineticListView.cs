/*	 This file was created by GALS Soft	company
 *	 http://www.galssoft.com
 *	 
 *	 Project name is Odnoklassniki Mobile
 *	 Platform is .Net Compact Framework 2.0 for Windows Mobile 5.0+ 
 * 
 *	 Copyright (c) 2009-2010 GALS Soft
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.GDI;
using System.Text;

namespace Galssoft.VKontakteWM.Components.UI.CompoundControls
{
    public abstract class KineticListView<TItemData> : KineticControl where TItemData : class
    {
        /// <summary>
        /// Helper class for storing prepared data to display on screen 
        /// </summary>
        protected class NativeItemData
        {
            public string PrimaryText { get; set; }
            public string SecondaryText { get; set; }
            public string TertiaryText { get; set; }
            public new Bitmap Icon { get; set; }

            public List<string> SecondaryTextLines;

            private IImage _primaryImage;
            public IImage PrimaryImage
            {
                get { return _primaryImage; }
                set { _primaryImage = value; }
            }

            private string _primaryImageURL;
            public string PrimaryImageURL
            {
                get { return _primaryImageURL; }
                set { _primaryImageURL = value; }
            }

            private IImage _secondaryImage;
            public IImage SecondaryImage
            {
                get { return _secondaryImage; }
                set { _secondaryImage = value; }
            }

            public string Group { get; set; }
            public object Tag { get; set; }
            public string IsGroupIdentity { get; set; }
            public string Uid { get; set; }

            public int[] InfoLeftIndents { get; set; }
            public int[] InfoTopIndents { get; set; }

            protected internal KineticListView<TItemData> Parent { get; set; }
        }

        protected KineticControlsSetting Settings;
        //public KineticControlsSetting Settings;

        protected virtual void RestoreCache()
        {
        }

        /// <summary>
        /// On item selected
        /// </summary>
        public event ItemSelectedEvent Select;
		
        /// <summary>
        /// On select item changed 
        /// </summary>
        public event EventHandler SelectedIndexChanged;

        /// <summary>
        /// On long click 
        /// </summary>
        public event EventHandler<ListViewLongPressEventArgs> ReturnLongPress;
		
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler BackClick;

        public readonly List<TItemData> Items = new List<TItemData>();
        protected readonly List<NativeItemData> NativeItems = new List<NativeItemData>();
        private Point _enterMousePoint;
        private readonly Timer _enterTimer = new Timer();
        private bool _enableLongPress;
        protected bool IsLocalKeyUp;

        public IImage OutsideUpShadow
        {
            get
            {
                return _outsideUpShadow;
            }
            set
            {
                _outsideUpShadow = value;
                if (_outsideUpShadow != null && _outsideShadowSize == Size.Empty)
                {
                    ImageInfo ii;
                    _outsideUpShadow.GetImageInfo(out ii);
                    _outsideShadowSize = new Size((int)ii.Width, (int)ii.Height);
                }
            }
        }
        private IImage _outsideUpShadow;

        public IImage OutsideDownShadow
        {
            get
            {
                return _outsideDownShadow;
            }
            set
            {
                _outsideDownShadow = value;
                if (_outsideDownShadow != null && _outsideShadowSize == Size.Empty)
                {
                    ImageInfo ii;
                    _outsideDownShadow.GetImageInfo(out ii);
                    _outsideShadowSize = new Size((int)ii.Width, (int)ii.Height);
                }
            }
        }
        private IImage _outsideDownShadow;

        private Size _outsideShadowSize = System.Drawing.Size.Empty;

        public IImage ContentUpShadow
        {
            get
            {
                return _contentUpShadow;
            }
            set
            {
                _contentUpShadow = value;
                if (_contentUpShadow != null && _insideShadowSize == Size.Empty)
                {
                    ImageInfo ii;
                    _contentUpShadow.GetImageInfo(out ii);
                    _insideShadowSize = new Size((int)ii.Width, (int)ii.Height);
                }
            }
        }
        private IImage _contentUpShadow;

        public IImage ContentDownShadow
        {
            get
            {
                return _contentDownShadow;
            }
            set
            {
                _contentDownShadow = value;
                if (_contentDownShadow != null && _insideShadowSize == Size.Empty)
                {
                    ImageInfo ii;
                    _contentDownShadow.GetImageInfo(out ii);
                    _insideShadowSize = new Size((int)ii.Width, (int)ii.Height);
                }
            }
        }
        private IImage _contentDownShadow;

        private Size _insideShadowSize = System.Drawing.Size.Empty;

        private bool _showSelectedItem = true;

        protected KineticListView(KineticControlScrollType scrollType, KineticControlsSetting settings)
            : base(scrollType)
        {
            if (settings != null)
                Settings = settings;
            else
                Settings = new KineticControlsSetting();

            _enterTimer.Tick += OnLongPressReturn;
            _enterTimer.Interval = 700;

            Settings.RecalcDPIScaling();
        }

        protected KineticListView(KineticControlScrollType scrollType)
            : this(scrollType, null)
        {
        }

        #region Nested type: ReloadDelegate

        public delegate void ReloadDelegate();

        #endregion

        #region Event Handlers

        protected override void OnSelectionChanged(int target)
        {
            if (SelectedIndexChanged != null)
                SelectedIndexChanged(this, EventArgs.Empty);
        }

        #endregion


        /// <summary>
        /// Selected item
        /// </summary>
        public TItemData SelectedItem
        {
            get
            {
                if (SelectedIndex >= 0 && SelectedIndex < Items.Count)
                    return Items[SelectedIndex];

                return null;
            }
            set
            {
                SelectedIndex = GetItemIndex(value);
            }
        }

        /// <summary>
        /// Image for group background
        /// </summary>
        public Bitmap GroupBackgroundImage
        {
            get { return _groupBackgroundImage; }
            set
            {
                Bitmap image = PrepareBackground(value);
                if (_groupBackgroundImage != image)
                {
                    ReleaseBitmap(_groupBackgroundImage);
                    _groupBackgroundImage = image;
                }
            }
        }
        private Bitmap _groupBackgroundImage = null;

        /// <summary>
        /// Get index of the specified item in Items list
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private int GetItemIndex(TItemData value)
        {
            if (Items.Count > 0 && value != null)
                return Items.IndexOf(value);

            return -1;
        }

        private static Bitmap _offGroupHeaderBmp;
        private static Graphics _offGroupHeaderGraphics;

        protected Graphics GroupHeaderGraphics
        {
            get
            {
                if (_offGroupHeaderGraphics == null)
                    TryReAllocateHeaderBuffer();

                return _offGroupHeaderGraphics;
            }
        }

        /// <summary>
        /// Index of the selected item in list
        /// </summary>
        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set { SelectItemInternal(value); }
        }

        /// <summary>
        /// Mark selected item
        /// </summary>
        public bool ShowSelectedItem
        {
            get { return _showSelectedItem; }
            set { _showSelectedItem = value; }
        }

        /// <summary>
        /// Count of the items in list
        /// </summary>
        public override int ItemCount
        {
            get
            {
                return Items.Count;
            }
        }

        public bool EnableLongPress
        {
            get
            {
                return _enableLongPress;
            }
            set
            {
                _enableLongPress = value;
            }
        }

        /// <summary>
        /// Title of the list
        /// </summary>
        public override string Title
        {
            get
            {
                return string.Format("{0}{1}",
                                     "Items",
                                     Items == null || Items.Count == 0 ? string.Empty : " (" + Items.Count + ")");
            }
        }


        /// <summary>
        /// Text to display if list is empty
        /// </summary>
        protected virtual string IfEmptyText
        {
            get
            {
                return "There are no items in this view";
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            _recountScrollOffset = true;

            TryReAllocateHeaderBuffer();

            SetItems(BuildNativeControlItems(), _SelectedIndex, true);

            if (SelectedIndex != -1)
                ScrollItemIntoView(SelectedIndex);
        }

        private void TryReAllocateHeaderBuffer()
        {
            OffscreenBuffer.TryReAllocateOffsceenBuffer(
                ref _offGroupHeaderBmp,
                ref _offGroupHeaderGraphics,
                Screen.PrimaryScreen.WorkingArea.Width,
                Settings.GroupPixHeight);
        }

        private static void ReleaseGroupHeaderBuffer()
        {
            OffscreenBuffer.ReleaseOffscreenBuffer(ref _offGroupHeaderBmp, ref _offGroupHeaderGraphics);
            //if (_offGroupHeaderGraphics != null)
            //{
            //    _offGroupHeaderGraphics.Dispose();
            //    _offGroupHeaderGraphics = null;
            //}

            //if (_offGroupHeaderBmp != null)
            //{
            //    _offGroupHeaderBmp.Dispose();
            //    _offGroupHeaderBmp = null;
            //}
        }

        /// <summary>
        /// Make the list of prepared items to display on screen without future modifications 
        /// </summary>
        /// <returns></returns>
        protected abstract List<NativeItemData> BuildNativeControlItems();

        #region Drawing Methods

        protected override void DrawScreenOn(Gdi gMem, Rectangle rClip, int yListOffset)
        {
            if(ScrollType == KineticControlScrollType.Vertical)
            {
                if (rClip.Bottom > ContentRectangle.Top || rClip.Top < ContentRectangle.Bottom)
                    DrawListOn(gMem, ContentRectangle, yListOffset);                
            }
            else
            {
                if (rClip.Right > ContentRectangle.Left || rClip.Left < ContentRectangle.Right)
                    DrawListOn(gMem, ContentRectangle, yListOffset);                
            }
        }

        /// <summary>
        /// Draw list in general
        /// </summary>
        /// <param name="gMem">Graphics</param>
        /// <param name="rListRect">Client rectangle</param>
        /// <param name="yOffset"></param>
        private void DrawListOn(Gdi gMem, Rectangle rListRect, int yOffset)
        {
            DrawBackground(gMem, rListRect, yOffset);

            if (ItemCount == 0)
            {
                DrawIfEmptyList(gMem, rListRect);

                if (ShowInnerShadows)
                {
                    if (ScrollType == KineticControlScrollType.Vertical)
                    {
                        if (OutsideUpShadow != null)
                            gMem.DrawImageAlphaChannelTiled(OutsideUpShadow, rListRect.Left, rListRect.Top, true, rListRect.Width);
                        if (OutsideDownShadow != null)
                            gMem.DrawImageAlphaChannelTiled(OutsideDownShadow, rListRect.Left, rListRect.Bottom - _outsideShadowSize.Height, true, rListRect.Width);
                    }
                    else
                    {
                        if (OutsideUpShadow != null)
                            gMem.DrawImageAlphaChannelTiled(OutsideUpShadow, rListRect.Left, rListRect.Top, false, rListRect.Height);
                        if (OutsideDownShadow != null)
                            gMem.DrawImageAlphaChannelTiled(OutsideDownShadow, rListRect.Right - _outsideShadowSize.Width, rListRect.Top, false, rListRect.Height);
                    }
                }

                return;
            }

            if (ShowInnerShadows)
            {
                if (ScrollType == KineticControlScrollType.Vertical)
                {
                    // draw shadow from outside controls at top
                    if (!ShowInnerTopShadowToplevel && OutsideUpShadow != null)
                        gMem.DrawImageAlphaChannelTiled(OutsideUpShadow, rListRect.Left, rListRect.Top, true, rListRect.Width);
                    // draw shadow from outside controls at bottom
                    if (!ShowInnerBottomShadowToplevel && OutsideDownShadow != null)
                        gMem.DrawImageAlphaChannelTiled(OutsideDownShadow, rListRect.Left, rListRect.Bottom - _outsideShadowSize.Height, true, rListRect.Width);
                }
                else
                {
                    // draw shadow from outside controls at top
                    if (!ShowInnerTopShadowToplevel && OutsideUpShadow != null)
                        gMem.DrawImageAlphaChannelTiled(OutsideUpShadow, rListRect.Left, rListRect.Top, false, rListRect.Height);
                    // draw shadow from outside controls at bottom
                    if (!ShowInnerBottomShadowToplevel && OutsideDownShadow != null)
                        gMem.DrawImageAlphaChannelTiled(OutsideDownShadow, rListRect.Right - _outsideShadowSize.Width, rListRect.Top, false, rListRect.Height);
                }
            }

            int nItem;

            int ps = yOffset < 0 ? 0 : GetPixelToItem(yOffset);
            if (ps == -2)
                ps = ItemCount - 1;

            int nFirstItem = nItem = ps;

            int bot;
            Rectangle rItem;

            if (ScrollType == KineticControlScrollType.Vertical)
            {
                if (nItem < 0)
                    bot = rListRect.Top - yOffset;
                else
                    bot = rListRect.Top + StartPositions[nItem] - yOffset;
                rItem = new Rectangle(rListRect.Left, rListRect.Top, rListRect.Width, bot);
            }
            else
            {
                if (nItem < 0)
                    bot = rListRect.Left - yOffset;
                else
                    bot = rListRect.Left + StartPositions[nItem] - yOffset;
                rItem = new Rectangle(rListRect.Left, rListRect.Top, bot, rListRect.Height);
            }

            // draw shadow on background from list items
            if (ShowContentShadows && nFirstItem == 0 && ContentUpShadow != null)
            {
                if (ScrollType == KineticControlScrollType.Vertical)
                {
                    gMem.DrawImageAlphaChannelTiled(ContentUpShadow, rListRect.Left, rItem.Height - _insideShadowSize.Height, true, rListRect.Width);
                }
                else
                {
                    gMem.DrawImageAlphaChannelTiled(ContentUpShadow, rListRect.Left - _insideShadowSize.Width, rItem.Height, false, rListRect.Height);
                }
            }

            NativeItemData item;

            // Draw all Items
            if (ScrollType == KineticControlScrollType.Vertical)
                while (nItem < ItemCount && rItem.Bottom < rListRect.Bottom)
                {
                    item = GetItemAtIndex(nItem);
                    rItem = new Rectangle(rItem.Left, rItem.Bottom, rItem.Width, StartPositions[nItem + 1] - StartPositions[nItem]);
                    DrawItemOn(gMem, item, rItem, nItem);

                    // ****** Group Header
                    if (ShowGroupHeader)
                        if (IsItemNewGroup(nItem) && item.Group != null && rItem.Top >= rListRect.Top)
                        {
                            int headerHeight = Settings.GroupPixHeight;
                            Rectangle rHeader = DrawGroupBackground(gMem, new Rectangle(rListRect.Left, rItem.Top, rListRect.Width, headerHeight));
                            DrawGroupHeaderOn(gMem, nItem, rHeader);
                        }

                    // Next nItem
                    nItem++;
                }
            else
                while (nItem < ItemCount && rItem.Right < rListRect.Right)
                {
                    item = GetItemAtIndex(nItem);

                    // ****** Group Header
                    int groupOffset = 0;
                    if (ShowGroupHeader)
                    {
                        int headerHeight = Settings.GroupPixHeight;

                        Rectangle rHeaderArea = new Rectangle(rItem.Right, rItem.Top,
                            StartPositions[nItem + 1] - StartPositions[nItem], headerHeight);

                        Rectangle rHead = DrawGroupBackground(gMem, rHeaderArea);

                        if (IsItemNewGroup(nItem) && rHeaderArea.Left >= 0)
                        {
                            DrawGroupHeaderOn(gMem, nItem, rHead);
                        }
                        groupOffset += headerHeight;
                    }

                    rItem = new Rectangle(rItem.Right, rItem.Top, StartPositions[nItem + 1] - StartPositions[nItem], rItem.Height);
                    Rectangle drawRect = new Rectangle(rItem.Left, rItem.Top + groupOffset, rItem.Width, rItem.Height - groupOffset);
                    DrawItemOn(gMem, item, drawRect, nItem);
                    // Next nItem
                    nItem++;
                }


            // Special: Draw the group of the list nItem that's at the top of the list
            if (ShowGroupHeader && ScrollType == KineticControlScrollType.Vertical)
            {
                item = GetItemAtIndex(nFirstItem);

                if (item.Group != null && yOffset > 0 && GroupHeaderGraphics != null)
                {
                    var rTopGroup = new Rectangle(rListRect.Left, 0, rListRect.Width, Settings.GroupPixHeight);

                    int nHeight = Settings.GroupPixHeight;
                    int nBottom = nHeight;

                    if (nFirstItem + 1 < ItemCount && IsItemNewGroup(nFirstItem + 1))
                    {
                        nBottom = Math.Min(nBottom, StartPositions[nFirstItem + 1] - yOffset);
                    }

                    // account for the fact that the list
                    // doesn't start at the top of the screen
                    nBottom += rListRect.Top;
                    int nLeft = rListRect.Left;
                    int nWidth = rListRect.Right - rListRect.Left;
                    int nTop = nBottom - nHeight;

                    rTopGroup = new Rectangle(nLeft, nTop, nWidth, nHeight);
                    Rectangle rHeader = DrawGroupBackground(gMem, rTopGroup);
                    DrawGroupHeaderOn(gMem, nFirstItem, rHeader);
                }
            }
            else if (ShowGroupHeader && ScrollType == KineticControlScrollType.Horizontal)
            {
                item = GetItemAtIndex(nFirstItem);
                int nWidth = StartPositions[nFirstItem + 1] - StartPositions[nFirstItem];

                if (item.Group != null && (yOffset > StartPositions[nFirstItem] || !IsItemNewGroup(nFirstItem)) && GroupHeaderGraphics != null)
                {
                    int nHeight = Settings.GroupPixHeight;
                    int nRight = nWidth;

                    if (nFirstItem + 1 < ItemCount && IsItemNewGroup(nFirstItem + 1))
                    {
                        nRight = Math.Min(nRight, StartPositions[nFirstItem + 1] - yOffset);
                    }

                    int nLeft = nRight - nWidth;

                    var rTopGroup = new Rectangle(nLeft, 0, nWidth - 1, nHeight);
                    DrawGroupBackground(gMem, rTopGroup);
                    DrawGroupHeaderOn(gMem, nFirstItem, rTopGroup);
                }
            }

            if (ShowContentShadows && nItem == ItemCount && rItem.Bottom < rListRect.Bottom && ContentDownShadow != null)
            {
                if (ScrollType == KineticControlScrollType.Vertical)
                {
                    gMem.DrawImageAlphaChannelTiled(ContentDownShadow, rListRect.Left, rItem.Bottom, true, rListRect.Width);
                }
                else
                {
                    gMem.DrawImageAlphaChannelTiled(ContentDownShadow, rListRect.Right, rItem.Top, false, rListRect.Height);
                }
            }

            if (ShowScrollbar)
                DrawScroll(gMem, yOffset, nFirstItem, nItem);

            if (ShowInnerShadows)
            {
                if (ScrollType == KineticControlScrollType.Vertical)
                {
                    // draw shadow from outside controls at top
                    if (OutsideUpShadow != null && ShowInnerTopShadowToplevel)
                        gMem.DrawImageAlphaChannelTiled(OutsideUpShadow, rListRect.Left, rListRect.Top, true, rListRect.Width);
                    // draw shadow from outside controls at bottom
                    if (OutsideDownShadow != null && ShowInnerBottomShadowToplevel)
                        gMem.DrawImageAlphaChannelTiled(OutsideDownShadow, rListRect.Left, rListRect.Bottom - _outsideShadowSize.Height, true, rListRect.Width);
                }
                else
                {
                    if (OutsideUpShadow != null && ShowInnerTopShadowToplevel)
                        gMem.DrawImageAlphaChannelTiled(OutsideUpShadow, rListRect.Left, rListRect.Top, false, rListRect.Height);
                    if (OutsideDownShadow != null && ShowInnerBottomShadowToplevel)
                        gMem.DrawImageAlphaChannelTiled(OutsideDownShadow, rListRect.Right - _outsideShadowSize.Width, rListRect.Top, false, rListRect.Height);
                }
            }
        }

        public bool ShowInnerShadows
        {
            get { return _showInnerShadows; }
            set { _showInnerShadows = value; }
        }
        private bool _showInnerShadows = false;

        public bool ShowContentShadows
        {
            get { return _showContentShadows; }
            set { _showContentShadows = value; }
        }
        private bool _showContentShadows = false;

        public bool ShowInnerTopShadowToplevel
        {
            get { return _showInnerTopShadowToplevel; }
            set { _showInnerTopShadowToplevel = value; }
        }
        private bool _showInnerTopShadowToplevel = false;

        public bool ShowInnerBottomShadowToplevel
        {
            get { return _showInnerBottomShadowToplevel; }
            set { _showInnerBottomShadowToplevel = value; }
        }
        private bool _showInnerBottomShadowToplevel = false;

        protected virtual void DrawGroupHeaderOn(Gdi gMem, int item, Rectangle rHeaderArea)
        {
            if (ScrollType == KineticControlScrollType.Horizontal)
            {
                var rSep = new Rectangle(rHeaderArea.Left - 1, rHeaderArea.Top, 1, rHeaderArea.Height);
                gMem.FillRect(rSep, Settings.ListItemSeparator);
            }

            // Draw Group Header Text
            gMem.Font = Settings.GroupFont;
            gMem.TextAlign = Win32.TextAlign.TA_LEFT;
            gMem.TextColor = Settings.ListGroupTextColor;
            //gMem.ExtTextOut(rHeaderArea.Left + Settings.GroupItemIndentPix,
            //                rHeaderArea.Top - 1 + ((Settings.GroupPixHeight - Settings.GroupFontPixHeight) / 2),
            //                GetItemAtIndex(item).Group,
            //                Settings.ListGroupTextShadow);
            gMem.ExtTextOut(rHeaderArea.Left,
                            rHeaderArea.Top + Settings.GroupPixTopMargin,
                            rHeaderArea.Width,
                            GetItemAtIndex(item).Group);

        }

        private Rectangle DrawGroupBackground(Gdi gMem, Rectangle rHeaderArea)
        {
            Rectangle rHeader = rHeaderArea;
            if (rHeaderArea.Height > Settings.GroupPixHeight)  // drawing the header as a separator between items
            {
                // apih: don't erase bg image
                // gMem.FillRect(rHeaderArea, Settings.ListBackgroundColor);

                int headerTop = rHeaderArea.Top;
                rHeader = new Rectangle(rHeaderArea.Left, headerTop, rHeaderArea.Width, Settings.GroupPixHeight);
            }

            // Group Header background
            if (GroupBackgroundImage != null)
                gMem.DrawImage(GroupBackgroundImage, rHeader.Left, rHeader.Top);
            else
            {
                if (Settings.ListGroupBackground != BrushGdi.Empty)
                {
                    //gMem.FillRect(rHeader, Settings.ListGroupBackground);
                    gMem.GradientFill(rHeader, Color.FromArgb(162, 169, 177), Color.FromArgb(189, 194, 200), FillDirection.TopToBottom);
                }
                // separator
                if (Settings.ListItemSeparator != BrushGdi.Empty)
                {
                    Rectangle rSep = new Rectangle(rHeader.Left, rHeader.Bottom, rHeader.Width, 1);
                    gMem.FillRect(rSep, Settings.ListItemSeparator);
                }
            }

            return rHeader;
        }

        public bool ShowScrollbar
        {
            get { return _showScrollbar; }
            set { _showScrollbar = value; }
        }

        public bool ShowGroupHeader
        {
            get; set;
        }

        private bool _showScrollbar = true;

        private bool _recountScrollOffset = true;
        private int _scrollLength = 1;

        protected override int ShiftForScrollIntoView
        {
            get { return Settings.GroupPixHeight; }
        }

        private void DrawScroll(Gdi gMem, int offset, int nFirstItem, int nItem)
        {
            int itemsVisibleCount = nItem - nFirstItem;
            bool recalc = ScrollType == KineticControlScrollType.Vertical
                              ? StartPositions.Count > 0 && StartPositions[StartPositions.Count - 1] > Height
                              : StartPositions.Count > 0 && StartPositions[StartPositions.Count - 1] > Width;

            if (recalc)
            {
                if (_recountScrollOffset)
                {
                    if(ScrollType == KineticControlScrollType.Vertical)
                        _scrollLength = Height / (ItemCount - itemsVisibleCount + 1);
                    else
                        _scrollLength = Width / (ItemCount - itemsVisibleCount + 1);

                    int minScrollLenght = UISettings.CalcPix(30);

                    if (_scrollLength < minScrollLenght)
                        _scrollLength = minScrollLenght;
//					if (_scrollLength > Height / 2)
//						_scrollLength = Height / 2;
                    _recountScrollOffset = false;
                }
                int scrollTop;

                if (offset <= 0)
                    scrollTop = 0;
                else
                {
                    if (ScrollType == KineticControlScrollType.Vertical)
                    {
                        scrollTop = (Height - _scrollLength) * offset /
                                    (StartPositions[StartPositions.Count - 1] - Height);
                        if (scrollTop > Height - _scrollLength ||
                            StartPositions[StartPositions.Count - 1] - offset <= Height)
                            scrollTop = Height - _scrollLength;                        
                    }
                    else
                    {
                        scrollTop = (Width - _scrollLength) * offset /
                                    (StartPositions[StartPositions.Count - 1] - Width);
                        if (scrollTop > Width - _scrollLength ||
                            StartPositions[StartPositions.Count - 1] - offset <= Width)
                            scrollTop = Width - _scrollLength;                        
                    }
                }

                int scrollWidth = Settings.ScrollPixWidth;
                Color scrollColor = Color.FromArgb(204, 204, 204);
                if (ScrollType == KineticControlScrollType.Vertical)
                    gMem.DrawRectandleAlpha(scrollColor, scrollColor,
                                            new Rectangle(Width - scrollWidth - Settings.ScrollPixMargin, scrollTop, scrollWidth, _scrollLength), 190);
                else
                    gMem.DrawRectandleAlpha(scrollColor, scrollColor,
                                            new Rectangle(scrollTop, Height - scrollWidth - Settings.ScrollPixMargin, _scrollLength, scrollWidth), 190);
            }
            else
                _recountScrollOffset = false;
        }

        /// <summary>
        /// Draw item
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="item">Prepared native item</param>
        /// <param name="rItem">Area to draw</param>
        /// <param name="nItem">Index of the item in the native items list</param>
        protected virtual void DrawItemOn(Gdi g, NativeItemData item, Rectangle rItem, int nItem)
        {
            bool isSelected = ShowSelectedItem ? nItem == _SelectedIndex : false;

            if(ScrollType == KineticControlScrollType.Vertical)
            {
                var rSep = new Rectangle(rItem.Left, rItem.Bottom - 1, rItem.Width, 1);
                g.FillRect(rSep, Settings.ListItemSeparator);                
            }
            else
            {
                var rSep = new Rectangle(rItem.Top, rItem.Right - 1, rItem.Height, 1);
                g.FillRect(rSep, Settings.ListItemSeparator);
            }
            
            DrawItemImagesOn(g, item, rItem);

            // Item Primary Text
            if (!string.IsNullOrEmpty(item.PrimaryText))
            {
                g.Font = Settings.PrimaryTextFontGdi;
                g.TextAlign = Win32.TextAlign.TA_LEFT;
                g.TextColor = isSelected ? Settings.ListItemSelectedTextColor : Settings.ListItemTextColor;
                if (ScrollType == KineticControlScrollType.Vertical)
                    g.ExtTextOut(rItem.Left + Settings.PrimaryIconPixWidth + Settings.PrimaryTextIndentPix,
                                 rItem.Bottom - rItem.Height + 2,
                                 item.PrimaryText);
                else
                    g.ExtTextOut(rItem.Left + Settings.PrimaryTextIndentPix,
                                 rItem.Bottom - rItem.Height + 2,
                                 item.PrimaryText);
            }
        }

        /// <summary>
        /// Draw background
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="item">Prepeared native item</param>
        /// <param name="rItem">Area to draw</param>
        /// <param name="nItem">Index of the item in the native items list</param>
        /// <param name="isSelected">Is item selected</param>
        protected virtual void DrawItemBackgroundOn(Gdi gMem, NativeItemData item, Rectangle rItem, int nItem, bool isSelected)
        {
            if (isSelected)
            {
                gMem.GradientFill(rItem, Color.FromArgb(214, 220, 236), Color.FromArgb(255, 255, 255),
                                  FillDirection.TopToBottom);
            }
            else
            {
                BrushGdi backGround = nItem%2 == 0 ? Settings.ListItemBackgroundEven : Settings.ListItemBackgroundOdd;
                if (backGround != BrushGdi.Empty)
                    gMem.FillRect(rItem, backGround);
            }
        }

        /// <summary>
        /// Draw images
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="item">Prepared native item</param>
        /// <param name="rItem">Area to draw</param>
        protected void DrawItemImagesOn(Gdi g, NativeItemData item, Rectangle rItem)
        {
            if (item.PrimaryImage != null)
            {
                g.DrawImageAlphaChannel(item.PrimaryImage, rItem.Left, rItem.Top);
            }
        }

        protected void DrawIfEmptyList(Gdi g, Rectangle area)
        {
            if (ItemCount == 0)
            {
                //area.Inflate(Settings.BackgroundHeaderPixMargin * -2, Settings.BackgroundHeaderPixMargin * -2);
                g.Font = Settings.PrimaryTextFontGdi;
                g.TextColor =Settings.ListGroupBackgroundColor;
                g.DrawText(IfEmptyText, new Win32.RECT(area.Left, area.Top, area.Width, area.Height), Win32.DT.CENTER);
            }
        }

        #endregion

        protected override void OnItemSelected(int index, Point mousePosition)
        {
            OnItemBodyClicked(_SelectedIndex);
        }

        protected virtual void OnItemBodyClicked(int index)
        {
            if (Select != null)
            {
                Select(SelectedItem, null);
            }
        }

        protected void OnLongPressReturn(object sender, EventArgs e)
        {
            _enterTimer.Enabled = false;
            IsLocalKeyUp = false;

            MouseSelectItemInternal(_enterMousePoint, false);

            if (ReturnLongPress != null)
            {     
                var args = new ListViewLongPressEventArgs(new Point(_enterMousePoint.X, _enterMousePoint.Y), 
                                                          SelectedIndex, SelectedItem);
                ReturnLongPress(this, args);
            }

            _IsDragging = true;
        }

        protected bool IsItemNewGroup(int index)
        {
            if (GetItemAtIndex(index).Group == null)
                return false;

            if (index == 0)
                return true;

            if (GetItemAtIndex(index - 1).Group == null)
                return true;

            return GetItemAtIndex(index - 1).Group != null && GetItemAtIndex(index - 1).Group.CompareTo(GetItemAtIndex(index).Group) != 0;
        }

        protected NativeItemData GetItemAtIndex(int inx)
        {
            return NativeItems[inx];
        }

        #region Public Methods

        public void OnAfterDeactivate()
        {
        }

        public void OnAfterActivate()
        {
            Focus();
        }

        public void RestartPosition()
        {
            if (_IsScrolling || _IsDragging)
                return;
            if (CurrentScrollPosition < 0)
                ScrollToStart();
            if (CurrentScrollPosition > MaxScrollPosition)
                ScrollToEnd(); 
        }

        public virtual void OnBeforeActivate()
        {
            Focus();
        }

        /// <summary>
        /// Clear list 
        /// </summary>
        public virtual void Clear()
        {
            ClearItems();
        }

        /// <summary>
        /// Reload items from the data source
        /// </summary>
        public virtual void Reload()
        {
            _recountScrollOffset = true;
			
            GenerateListItems();
            SetRootItemInternal(SelectedIndex, true);
        }

        /// <summary>
        /// Select items from the data source
        /// </summary>
        protected abstract void GenerateListItems();

        protected virtual void SetRootItemInternal(int selectedIndex, bool preserveScroll)
        {
            int index = -1;

            if (Items.Count > 0)
            {
                if (selectedIndex == -1 || selectedIndex >= 0 && selectedIndex < Items.Count)
                    index = selectedIndex;
                else if (selectedIndex >= Items.Count)
                    index = Items.Count - 1;
            }

            SetItems(BuildNativeControlItems(), index, preserveScroll);
        }

        /// <summary>
        /// Update data for the native item
        /// </summary>
        /// <param name="item">New data for item</param>
        /// <param name="index">Index of the native item to replace</param>
        protected void UpdateItem(NativeItemData item, int index)
        {
            if (index < 0 || index >= Items.Count)
                return;

            NativeItems[index] = item;

            CalculateTextWidths(index);

            if (IsItemVisible(index))
                RefreshControl();
        }

        private void ClearItems()
        {
            Items.Clear();
            CalculateItemsSize();
            ScrollTo(0);
        }

        protected void SetItemsAndShowHeader(List<NativeItemData> items)
        {
            ReleaseNativeItems();
            NativeItems.Clear();
            NativeItems.AddRange(items);
            SetParentLink(NativeItems, this);
            CalculateItemsSize();
            CalculateTextWidths(-1);

            SelectedIndex = -1;

            this.ScrollToStart();
        }

        protected void SetItems(List<NativeItemData> items, int selectedIndex, bool preserveScrollPosition)
        {
            ReleaseNativeItems();
            NativeItems.Clear();
            NativeItems.AddRange(items);
            SetParentLink(NativeItems, this);
            CalculateItemsSize();
            CalculateTextWidths(-1);

            if (!preserveScrollPosition)
                ResetScrollPosition();

            SelectedIndex = selectedIndex;
            RefreshControl();
        }

        #endregion

        ///// <summary>
        ///// Calculate width of the text to display on screen
        ///// </summary>
        ///// <param name="itemIndex">Index of the item from the NativeItems list</param>
        //private void CalculateTextWidths(int itemIndex)
        //{
        //    int min = 0, max = ItemCount;
        //    if (itemIndex >= 0)
        //    {
        //        min = itemIndex;
        //        max = itemIndex + 1;
        //    }

        //    if (GroupHeaderGraphics == null)
        //        return;

        //    for (int i = min; i != max; i++)
        //    {
        //        int textareaPrimaryTextWidth = GetTextareaWidthWithOption(i);

        //        int maxTextLines;

        //        if (!string.IsNullOrEmpty(NativeItems[i].PrimaryText))
        //            using (Font primaryFont = System.Drawing.Font.FromHfont((IntPtr)Settings.PrimaryTextFontGdi))
        //            {
        //                // truncate subject
        //                SizeF size = GroupHeaderGraphics.MeasureString(NativeItems[i].PrimaryText, primaryFont);
        //                float avl = (float) textareaPrimaryTextWidth/size.Width;
        //                if (avl < 1)
        //                {
        //                    double len = Math.Floor(NativeItems[i].PrimaryText.Length*avl);
        //                    NativeItems[i].PrimaryText = NativeItems[i].PrimaryText.Substring(0, (int) len) + "...";
        //                    while (GroupHeaderGraphics.MeasureString(NativeItems[i].PrimaryText, primaryFont).Width > textareaPrimaryTextWidth)
        //                    {
        //                        NativeItems[i].PrimaryText = NativeItems[i].PrimaryText.Remove(NativeItems[i].PrimaryText.Length - 4, 1);
        //                    }
        //                }
        //            }

        //        if (!string.IsNullOrEmpty(NativeItems[i].SecondaryText))
        //            using (Font secondaryFont = System.Drawing.Font.FromHfont((IntPtr)Settings.SecondaryTextFontGdi))
        //            {
        //                // truncate subject
        //                SizeF size = GroupHeaderGraphics.MeasureString(NativeItems[i].SecondaryText, secondaryFont);
        //                float avl = (float)textareaPrimaryTextWidth / size.Width;
        //                if (avl < 1)
        //                {
        //                    double len = Math.Floor(NativeItems[i].SecondaryText.Length * avl);
        //                    NativeItems[i].SecondaryText = NativeItems[i].SecondaryText.Substring(0, (int)len) + "...";
        //                    while (GroupHeaderGraphics.MeasureString(NativeItems[i].SecondaryText, secondaryFont).Width > textareaPrimaryTextWidth)
        //                    {
        //                        NativeItems[i].SecondaryText = NativeItems[i].SecondaryText.Remove(NativeItems[i].SecondaryText.Length - 4, 1);
        //                    }
        //                }
        //            }

        //        CalculateTextWidthsCustom(NativeItems[i], GroupHeaderGraphics, textareaPrimaryTextWidth, out maxTextLines);
        //    }
        //}

        /// <summary>
        /// Calculate width of the text to display on screen
        /// </summary>
        /// <param name="itemIndex">Index of the item from the NativeItems list</param>
        private void CalculateTextWidths(int itemIndex)
        {
            int min = 0, max = ItemCount;
            if (itemIndex >= 0)
            {
                min = itemIndex;
                max = itemIndex + 1;
            }

            if (GroupHeaderGraphics == null)
                return;

            for (int i = min; i != max; i++)
            {
                int textareaPrimaryTextWidth = GetTextareaWidthWithOption(i);

                int maxTextLines;

                CalculateTextWidthsCustom(NativeItems[i], GroupHeaderGraphics, textareaPrimaryTextWidth, out maxTextLines);

                if (!string.IsNullOrEmpty(NativeItems[i].PrimaryText))
                    using (Font primaryFont = System.Drawing.Font.FromHfont((IntPtr)Settings.PrimaryTextFontGdi))
                    {
                        // truncate subject
                        SizeF size = GroupHeaderGraphics.MeasureString(NativeItems[i].PrimaryText, primaryFont);
                        float avl = (float)textareaPrimaryTextWidth / size.Width;
                        if (avl < 1)
                        {
                            double len = Math.Floor(NativeItems[i].PrimaryText.Length * avl);
                            NativeItems[i].PrimaryText = NativeItems[i].PrimaryText.Substring(0, (int)len) + "...";
                            while (GroupHeaderGraphics.MeasureString(NativeItems[i].PrimaryText, primaryFont).Width > textareaPrimaryTextWidth)
                            {
                                NativeItems[i].PrimaryText = NativeItems[i].PrimaryText.Remove(NativeItems[i].PrimaryText.Length - 4, 1);
                            }
                        }
                    }

                // truncate text
                string text = NativeItems[i].SecondaryText;
                string resultText = "";

                using (Font secondaryFont = System.Drawing.Font.FromHfont((IntPtr)Settings.SecondaryTextFontGdi))
                {
                    for (int itextlines = 0; itextlines < maxTextLines; itextlines++)
                    {
                        SizeF size = GroupHeaderGraphics.MeasureString(text, secondaryFont);
                        float avl = (float)textareaPrimaryTextWidth / size.Width;
                        if (avl < 1)
                        {
                            //for (int i = 1; i < _Items[i].SecondaryText.Length; i)
                            bool foundWhitespace = false;
                            int len = (int)Math.Floor(text.Length * avl);
                            for (int ipos = len; ipos > 0 && ipos > len - 5; ipos--)
                            {
                                char ch = text[ipos];
                                if (ch == ' ')
                                {
                                    resultText += text.Substring(0, ipos);
                                    text = text.Remove(0, ipos + 1);
                                    foundWhitespace = true;
                                    break;
                                }
                            }

                            if (!foundWhitespace)
                            {
                                resultText += text.Substring(0, len);
                                text = text.Remove(0, len);
                            }

                            if (itextlines + 1 == maxTextLines)
                            {
                                if (text.TrimEnd().Length > 0)
                                {
                                    resultText += "...";
                                }
                                else
                                {
                                    resultText += "\n";                                
                                }
                            }
                            else
                            {
                                resultText += "\n";                                
                            }
                        }
                        else
                        {
                            resultText += text;
                            text = string.Empty;
                            break;
                        }
                    }
                    NativeItems[i].SecondaryText = resultText;
                }
            }
        }

        /// <summary>
        /// Calulate width for text drawing	according to using Primary, Secondary icons and so on
        /// </summary>
        /// <param name="i">Index of the native item</param>
        /// <returns>Width in pixels</returns>
        protected int GetTextareaWidth(int i)
        {
            int textareaWidth = ClientRectangle.Width;
            return textareaWidth;
        }

        /// <summary>
        /// Calulate width for text in header include security option
        /// </summary>
        /// <param name="i">Index of the native item</param>
        /// <returns>Width in pixels</returns>
        protected virtual int GetTextareaWidthWithOption(int i)
        {
            return GetTextareaWidth(i);
        }

        /// <summary>
        /// vVirtual method for child classes to calculate width of the text to display on screen 
        /// </summary>
        /// <param name="nativeItemData">Item from the NativeItems list</param>
        /// <param name="groupHeaderGraphics">Graphics object</param>
        /// <param name="textareaWidth">Width of the allowed area to write text</param>
        /// <param name="fb">Font primary bold</param>
        /// <param name="maxTextLines">Calculated maximum lines of the secondary text to display in area</param>
        protected virtual void CalculateTextWidthsCustom(NativeItemData nativeItemData, Graphics groupHeaderGraphics, int textareaWidth, out int maxTextLines)
        {
            maxTextLines = Settings.SecondaryTextLinesCount;
        }

        /// <summary>
        /// Get text of the item to search
        /// </summary>
        protected virtual string GetItemText(int itemIndex)
        {
            var item = GetItemAtIndex(itemIndex);
            return item != null ? item.PrimaryText : string.Empty;
        }

        protected override void OnBackKeyClicked()
        {
            if (BackClick != null)
                BackClick(this, null);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            RestartPosition();
            base.OnGotFocus(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ReleaseGroupHeaderBuffer();
            }

            base.Dispose(disposing);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            _enterTimer.Enabled = false;

            try
            {
                switch (e.KeyCode)
                {
                    case Keys.Enter:
                        if (Select != null && SelectedIndex != -1 && IsLocalKeyUp)
                        {
                            if (_enableLongPress)
                                _enterTimer.Enabled = true;
                            Select(SelectedItem, null);
                            _enterTimer.Enabled = false;
                        }
                        e.Handled = true;
                        break;
                }

                if (!e.Handled)
                {
                    base.OnKeyUp(e);
                }
            }
            finally
            {
                IsLocalKeyUp = false;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    IsLocalKeyUp = true;
                    if (SelectedItem != null)
                    {
                        _enterMousePoint = new Point(1, StartPositions[_SelectedIndex] - CurrentScrollPosition + 1);
                        if (_enableLongPress)
                            _enterTimer.Enabled = true;
                    }
                    e.Handled = true;
                    break;

                    //Left Right
                case Keys.Left:
                    e.Handled = true;
                    break;

                    //Left Right
                case Keys.Right:
                    e.Handled = true;
                    break;

                case Keys.Up:
                    e.Handled = true;
                    break;

                case Keys.Down:
                    e.Handled = true;
                    break;
            }

            //Patch the * and # symbols
            if ((int)e.KeyData == 120)
            {
                FindNextItemStartingWith('A');
                e.Handled = true;
            }
            else if ((int)e.KeyData == 119)
            {
                FindNextItemStartingWith('Z');
                e.Handled = true;
            }

            if (!e.Handled)
            {
                base.OnKeyDown(e);
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            FindNextItemStartingWith(e.KeyChar);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (LastMouseX > (ContentRectangle.Right) && !_IsScrolling && !_IsDragging)
            {
                RefreshControl();
            }

            this.Focus();

            if (e.Button == MouseButtons.Left)
            {
                _enterMousePoint = new Point(e.X, e.Y);
                if (_enableLongPress)
                    _enterTimer.Enabled = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (Math.Abs(e.X - _enterMousePoint.X) > SCROLL_THRESHOLD || Math.Abs(e.Y - _enterMousePoint.Y) > SCROLL_THRESHOLD)
                if (_enableLongPress)
                    _enterTimer.Enabled = false;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _enterTimer.Enabled = false;
            base.OnMouseUp(e);
        }

        protected bool FindNextItemStartingWith(char keyChar)
        {
            if (ItemCount == 0)
                return false;

            int curr;

            if (_SelectedIndex > 0 && _SelectedIndex < ItemCount)
                curr = _SelectedIndex;
            else
                curr = 0;

            var start = curr;

            do
            {
                curr++;

                if (curr > ItemCount - 1)
                    curr = 0;

                if (GetItemText(curr).StartsWith(keyChar.ToString(), StringComparison.InvariantCultureIgnoreCase)
                    ||
                    GetItemText(curr).StartsWith(GetAlternativeKey(keyChar).ToString(),
                                                 StringComparison.InvariantCultureIgnoreCase))
                {
                    SelectItemInternal(curr);
                    return true;
                }
            } while (curr != start);

            return false;
        }

        protected override void CalculateItemsSize()
        {
            CalculateItemSizeForListSize(Settings.ListItemPixSize, null);
        }

        protected void CalculateItemSizeForListSize(int defaultListItemHeight, List<int> listItemHeights)
        {
            int c = 0;

            StartPositions.Clear();

            if (ItemCount == 0)
            {
                _ActiveListHeight = 0;
                _ActiveListWidth = 0;
                return;
            }

            for (int i = 0; i < ItemCount; i++)
            {
                StartPositions.Add(c);
                if (listItemHeights != null && listItemHeights.Count > i)
                    c += listItemHeights[i];
                else
                    c += defaultListItemHeight;

                if (ScrollType == KineticControlScrollType.Vertical && ShowGroupHeader)
                {
                    if (IsItemNewGroup(i))
                    {
                        c += Settings.GroupPixHeight;
                    }
                }
            }

            StartPositions.Add(c); //The end of tha last item

            if (ScrollType == KineticControlScrollType.Vertical)
            {
                _ActiveListHeight = c;
            }
            else
            {
                _ActiveListWidth = c;
            }

        }

        private void ReleaseNativeItems()
        {
            SetParentLink(NativeItems, null);
            foreach (NativeItemData nativeItemData in NativeItems)
            {
                if (nativeItemData != null && nativeItemData is IDisposable)
                    ((IDisposable) nativeItemData).Dispose();
            }
        }

        private static void SetParentLink(IEnumerable<NativeItemData> items, KineticListView<TItemData> listView)
        {
            foreach (NativeItemData item in items)
            {
                item.Parent = listView;
            }
        }

    }
}