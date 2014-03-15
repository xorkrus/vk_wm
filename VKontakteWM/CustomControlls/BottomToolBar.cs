using System;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Properties;
using AnchorStyles = Galssoft.VKontakteWM.Components.UI.AnchorStyles;

namespace Galssoft.VKontakteWM.CustomControlls
{
    public sealed class BottomToolBar : Components.UI.CompoundControls.ToolBar
    {
        public readonly ToolbarButton ToolbarButtonNews;
        public readonly ToolbarButton ToolbarButtonMessages;
        public readonly ToolbarButton ToolbarButtonFriends;
        public readonly ToolbarButton ToolbarButtonPhotos;
        public readonly ToolbarButton ToolbarButtonExtras;

        public readonly ContextMenu contextMenu;
        public readonly MenuItem miUserData;
        public readonly MenuItem miAbout;
        public readonly MenuItem miSettings;
        public readonly MenuItem miExit;


        private static readonly int _verticalTextMargin = UISettings.CalcPix(4);
        private static readonly FontGdi _font = FontCache.CreateFont("Tahoma", 10, FontStyle.Regular);
        private static readonly FontGdi _pressedFont = FontCache.CreateFont("Tahoma", 10, FontStyle.Regular);
        private static readonly FontGdi _selectedFont = FontCache.CreateFont("Tahoma", 10, FontStyle.Regular);
        private static readonly Color _fontColor = Color.FromArgb(133, 133, 133);
        private static readonly Color _pressedFontColor = Color.FromArgb(89, 203, 241);

        public BottomToolBar()
        {
            Location = new Point(0, 229);
            Size = new Size(240, 39);
            Anchor = AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
            BackgroundImage = new GraphicsImage(MasterForm.SkinManager.GetImage("TBImage"), true);
            Name = "toolBar";

            //FIXME почему он для 128 не пересчитывает понять не могу, пока так
            if (UISettings.ScreenDPI == 128)
                ButtonsSize = new Size(UISettings.CalcPix(45), UISettings.CalcPix(36));
            else
                ButtonsSize = new Size(45, 36);

            Visible = true;

            miAbout = new MenuItem {Text = Resources.ExtraView_AboutButtonTitle};
            miUserData = new MenuItem {Text = Resources.ExtraView_UserDataButtonTitle};
            miSettings = new MenuItem {Text = Resources.ExtraView_SettingsButtonTitle};
            miExit = new MenuItem {Text = Resources.ExtraView_ExitButtonTitle};

            contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add(miUserData);
            contextMenu.MenuItems.Add(miSettings);
            contextMenu.MenuItems.Add(miAbout);
            contextMenu.MenuItems.Add(miExit);
            
            ToolbarButtonNews = AddButton(ButtonStyle.AlphaChannel);
            ToolbarButtonNews.TransparentButton = MasterForm.SkinManager.GetImage("TBButtonNews1");
            ToolbarButtonNews.TransparentButtonPressed = MasterForm.SkinManager.GetImage("TBButtonNews3");
            ToolbarButtonNews.TransparentButtonSelected = MasterForm.SkinManager.GetImage("TBButtonNews2");
            ToolbarButtonNews.Text = Resources.toolBar_News;
            ToolbarButtonNews.Font = _font;
            ToolbarButtonNews.FontColor = _fontColor;
            ToolbarButtonNews.PressedFont = _pressedFont;
            ToolbarButtonNews.PressedFontColor = _pressedFontColor;
            ToolbarButtonNews.SelectedFont = _selectedFont;
            ToolbarButtonNews.SelectedFontColor = _fontColor;
            ToolbarButtonNews.VerticalTextMargin = _verticalTextMargin;

            ToolbarButtonMessages = AddButton(ButtonStyle.AlphaChannel);
            ToolbarButtonMessages.TransparentButton = MasterForm.SkinManager.GetImage("TBButtonMessages1");
            ToolbarButtonMessages.TransparentButtonPressed = MasterForm.SkinManager.GetImage("TBButtonMessages3");
            ToolbarButtonMessages.TransparentButtonSelected = MasterForm.SkinManager.GetImage("TBButtonMessages2");
            ToolbarButtonMessages.Text = Resources.toolBar_Messages;
            ToolbarButtonMessages.Font = _font;
            ToolbarButtonMessages.FontColor = _fontColor;
            ToolbarButtonMessages.PressedFont = _pressedFont;
            ToolbarButtonMessages.PressedFontColor = _pressedFontColor;
            ToolbarButtonMessages.SelectedFont = _selectedFont;
            ToolbarButtonMessages.SelectedFontColor = _fontColor;
            ToolbarButtonMessages.VerticalTextMargin = _verticalTextMargin;

            ToolbarButtonFriends = AddButton(ButtonStyle.AlphaChannel);
            ToolbarButtonFriends.TransparentButton = MasterForm.SkinManager.GetImage("TBButtonFriends1");
            ToolbarButtonFriends.TransparentButtonPressed = MasterForm.SkinManager.GetImage("TBButtonFriends3");
            ToolbarButtonFriends.TransparentButtonSelected = MasterForm.SkinManager.GetImage("TBButtonFriends2");
            ToolbarButtonFriends.Text = Resources.toolBar_Friends;
            ToolbarButtonFriends.Font = _font;
            ToolbarButtonFriends.FontColor = _fontColor;
            ToolbarButtonFriends.PressedFont = _pressedFont;
            ToolbarButtonFriends.PressedFontColor = _pressedFontColor;
            ToolbarButtonFriends.SelectedFont = _selectedFont;
            ToolbarButtonFriends.SelectedFontColor = _fontColor;
            ToolbarButtonFriends.VerticalTextMargin = _verticalTextMargin;

            ToolbarButtonPhotos = AddButton(ButtonStyle.AlphaChannel);
            ToolbarButtonPhotos.TransparentButton = MasterForm.SkinManager.GetImage("TBButtonPhotos1");
            ToolbarButtonPhotos.TransparentButtonPressed = MasterForm.SkinManager.GetImage("TBButtonPhotos3");
            ToolbarButtonPhotos.TransparentButtonSelected = MasterForm.SkinManager.GetImage("TBButtonPhotos2");
            ToolbarButtonPhotos.Text = Resources.toolBar_Photos;
            ToolbarButtonPhotos.Font = _font;
            ToolbarButtonPhotos.FontColor = _fontColor;
            ToolbarButtonPhotos.PressedFont = _pressedFont;
            ToolbarButtonPhotos.PressedFontColor = _pressedFontColor;
            ToolbarButtonPhotos.SelectedFont = _selectedFont;
            ToolbarButtonPhotos.SelectedFontColor = _fontColor;
            ToolbarButtonPhotos.VerticalTextMargin = _verticalTextMargin;

            ToolbarButtonExtras = AddButton(ButtonStyle.AlphaChannel);
            ToolbarButtonExtras.TransparentButton = MasterForm.SkinManager.GetImage("TBButtonOptions1");
            ToolbarButtonExtras.TransparentButtonPressed = MasterForm.SkinManager.GetImage("TBButtonOptions3");
            ToolbarButtonExtras.TransparentButtonSelected = MasterForm.SkinManager.GetImage("TBButtonOptions2");
            ToolbarButtonExtras.Text = Resources.toolBar_Extras;
            ToolbarButtonExtras.Font = _font;
            ToolbarButtonExtras.FontColor = _fontColor;
            ToolbarButtonExtras.PressedFont = _pressedFont;
            ToolbarButtonExtras.PressedFontColor = _pressedFontColor;
            ToolbarButtonExtras.SelectedFont = _selectedFont;
            ToolbarButtonExtras.SelectedFontColor = _fontColor;
            ToolbarButtonExtras.VerticalTextMargin = _verticalTextMargin;
        }

        /// <summary>
        /// Возвращает смещение по вертикали контекстнеого меню в зависимости от разрешения
        /// </summary>
        /// <returns></returns>
        public int GetCurrentShift()
        {
            if (UISettings.ScreenDPI == 96)
                return UISettings.CalcPix(112);
            return UISettings.CalcPix(108);
        }
    }
}