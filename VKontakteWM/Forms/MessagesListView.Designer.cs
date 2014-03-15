using System;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Properties;
using Microsoft.WindowsCE.Forms;
using AnchorStyles = Galssoft.VKontakteWM.Components.UI.AnchorStyles;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.Common.Configuration;

namespace Galssoft.VKontakteWM.Forms
{
    partial class MessagesListView
    {
        /// <summary> 
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс  следует удалить; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Обязательный метод для поддержки конструктора - не изменяйте 
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            mainMenu = new System.Windows.Forms.MainMenu();

            menuItemActions = new System.Windows.Forms.MenuItem();
            //menuItemWrite = new System.Windows.Forms.MenuItem();
            menuItemBack = new System.Windows.Forms.MenuItem();

            this.headerText = new UILabel();
            this.headerShadowText = new UILabel();

            klvMessagesList = new MessageListKineticListView();

            tbxMessageData = new TextBox();

            btnRefresh = new UIButton(ButtonStyle.AlphaChannel);
            header = new GraphicsImage();
            logo = new GraphicsImage();

            this.SuspendLayout();

            this.Size = new Size(240, 268);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.Name = "MessagesListView";
            this.BackColor = Color.White;

            this.header.Name = "Head";
            this.header.Location = new Point(0, 0);
            this.header.Stretch = true;
            this.header.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            this.logo.Name = "Logo";
            this.logo.Location = new Point(6, 7);
            this.logo.Stretch = false;
            this.logo.Anchor = AnchorStyles.Left | AnchorStyles.Top;

            //
            // headerText
            //
            this.headerText.Name = "headerText";
            this.headerText.Location = new Point(0, 0);
            this.headerText.Size = new Size(240, 36);
            this.headerText.Font = FontCache.CreateFont("Tahoma", 14, FontStyle.Bold);
            this.headerText.ForeColor = Color.White;
            this.headerText.VerticalTextAlignment = VerticalAlignment.Center;
            this.headerText.HorizontalTextAlignment = Galssoft.VKontakteWM.Components.UI.HorizontalAlignment.Center;
            this.headerText.Text = Resources.ChangeCommentView_Title;
            this.headerText.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            //
            // headerShadowText
            //
            this.headerShadowText.Name = "headerShadowText";
            this.headerShadowText.Location = new Point(0, 0);
            this.headerShadowText.Size = new Size(240, 36);
            this.headerShadowText.Font = FontCache.CreateFont("Tahoma", 14, FontStyle.Bold);
            this.headerShadowText.ForeColor = Color.Black;
            this.headerShadowText.VerticalTextAlignment = VerticalAlignment.Center;
            this.headerShadowText.HorizontalTextAlignment = Galssoft.VKontakteWM.Components.UI.HorizontalAlignment.Center;
            this.headerShadowText.Text = Resources.ChangeCommentView_Title;
            this.headerShadowText.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            //
            //btnRefresh
            //
            btnRefresh.TransparentButton = MasterForm.SkinManager.GetImage("RefreshButton");
            btnRefresh.TransparentButtonPressed = MasterForm.SkinManager.GetImage("RefreshButtonPressed");
            btnRefresh.Location = new Point(211, 5);
            btnRefresh.Size = new Size(24, 24);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefresh.Click += new EventHandler(BtnRefreshClick);

            //
            // inputPanel
            //
            this.inputPanel = new Microsoft.WindowsCE.Forms.InputPanel();

            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.Add(this.menuItemBack);
            this.mainMenu.MenuItems.Add(this.menuItemActions);
            //menuItemActions.MenuItems.Add(menuItemWrite);


            // 
            // menuItemBack
            // 
            this.menuItemBack.Text = Resources.MessagesList_Designer_menuItemBack_Text;
            this.menuItemBack.Click += new EventHandler(menuItemBack_Click);

            // 
            // menuItemActions
            // 
            this.menuItemActions.Text = Resources.MessagesList_Designer_menuItemSent_Text;
            this.menuItemActions.Click += new EventHandler(menuItemSent_Click);

            // 
            // menuItemWrite
            // 
            //this.menuItemWrite.Text = Resources.MessagesList_Designer_menuItemSent_Text;
            //this.menuItemWrite.Click += new EventHandler(menuItemSent_Click);

            //
            //tbxMessageData
            //
            tbxMessageData.Location = new Point(5, 5);
            tbxMessageData.Size = new Size(230, 90);
            tbxMessageData.Name = "tbxMessageData";
            tbxMessageData.Multiline = true;
            tbxMessageData.WordWrap = true;
            tbxMessageData.ScrollBars = ScrollBars.None;
            tbxMessageData.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            tbxMessageData.GotFocus += new EventHandler(tbxMessageDataGotFocus);
            tbxMessageData.LostFocus += new EventHandler(tbxMessageDataLostFocus);
            tbxMessageData.BorderStyle = BorderStyle.None;

            //
            //klvMessagesList
            //
            klvMessagesList.Anchor =    System.Windows.Forms.AnchorStyles.Left
                                        | System.Windows.Forms.AnchorStyles.Right
                                        | System.Windows.Forms.AnchorStyles.Top
                                        | System.Windows.Forms.AnchorStyles.Bottom;
            klvMessagesList.Location = new System.Drawing.Point(0, 36);
            klvMessagesList.Size = new System.Drawing.Size(240, 234);
            klvMessagesList.BackColor = Color.White;
            klvMessagesList.BackgroundIImage = MasterForm.SkinManager.GetImage("List-background");
            klvMessagesList.ContentUpShadow = MasterForm.SkinManager.GetImage("ContentUpShadow");
            klvMessagesList.ContentDownShadow = MasterForm.SkinManager.GetImage("ContentDownShadow");
            klvMessagesList.ShowContentShadows = true;
            klvMessagesList.OutsideDownShadow = MasterForm.SkinManager.GetImage("ContentUpShadow");
            klvMessagesList.OutsideUpShadow = MasterForm.SkinManager.GetImage("ContentDownShadow");
            klvMessagesList.ShowInnerShadows = true;
            klvMessagesList.ScrollAction = KineticControlBase.KineticControlScrollAction.ScrollingForTime;
            klvMessagesList.Name = "klvMessagesList";


            this.Controls.Add(klvMessagesList);
            this.Canvas.Children.Add(header);
            this.Canvas.Children.Add(logo);
            this.Canvas.Children.Add(btnRefresh);
            this.Canvas.Children.Add(headerShadowText);
            this.Canvas.Children.Add(headerText);
            //this.Controls.Add(tbxMessageData);

            this.Canvas.RecalcDPIScaling();
            this.ResumeLayout(false);

            this.header.AlphaChannelImage = MasterForm.SkinManager.GetImage("Header");
            this.logo.AlphaChannelImage = MasterForm.SkinManager.GetImage("HeaderLogo");

            this.headerShadowText.Top -= UISettings.CalcPix(1);
        }

        #endregion

        private System.Windows.Forms.MenuItem menuItemActions;
        //private System.Windows.Forms.MenuItem menuItemWrite;
        private System.Windows.Forms.MenuItem menuItemBack;
        private Microsoft.WindowsCE.Forms.InputPanel inputPanel;

        private TextBox tbxMessageData;

        private UIButton btnRefresh;
        private GraphicsImage header;
        private GraphicsImage logo;

        private UILabel headerText;
        private UILabel headerShadowText;

        private MessageListKineticListView klvMessagesList;
    }
}
