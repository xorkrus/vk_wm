using System.Windows.Forms;
using System.Drawing;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Properties;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using AnchorStyles = Galssoft.VKontakteWM.Components.UI.AnchorStyles;

namespace Galssoft.VKontakteWM.Forms
{
    partial class SharePhotoView
    {
        /// <summary> 
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.mainMenu = new MainMenu();

            this.menuItemCancel = new MenuItem();
            this.menuItemAction = new MenuItem();
            this.menuItemRC = new MenuItem();
            this.menuItemRCC = new MenuItem();
            this.menuItemSend = new MenuItem();

            this.btnSentMessage = new UIButton(ButtonStyle.AlphaChannel);
            this.btnSentMessageText = new UILabel();
            this.btnSentMessageShadowText = new UILabel();

            //filter = new FilterControl(230, 25);

            header = new GraphicsImage();
            logo = new GraphicsImage();

            this.giPhotoPreview = new GraphicsImage();            
            this.lblVoidPhoto = new MLLabel();

            this.SuspendLayout();

            //
            // header
            //
            this.header.Name = "header";
            this.header.Location = new Point(0, 0);
            this.header.Stretch = true;
            this.header.Anchor = Galssoft.VKontakteWM.Components.UI.AnchorStyles.Left | Galssoft.VKontakteWM.Components.UI.AnchorStyles.Top | Galssoft.VKontakteWM.Components.UI.AnchorStyles.Right;

            //
            // logo
            //
            this.logo.Name = "Logo";
            this.logo.Location = new Point(6, 7);
            this.logo.Stretch = false;
            this.logo.Anchor = AnchorStyles.Left | AnchorStyles.Top;

            //
            // mainMenu
            //
            mainMenu.MenuItems.Add(menuItemCancel);
            mainMenu.MenuItems.Add(menuItemAction);
            
            //
            // menuItemChange
            //
            menuItemAction.Text = Resources.SharePhotoView_MenuItem_Action;
            menuItemAction.MenuItems.Add(menuItemSend);
            menuItemAction.MenuItems.Add(menuItemRC);
            menuItemAction.MenuItems.Add(menuItemRCC);
            //menuItemAction.Click += new EventHandler(MenuItemChangeClick);

            //
            // menuItemCancel
            //
            menuItemCancel.Text = Resources.SharePhotoView_MenuItem_Cancel;
            menuItemCancel.Click += new System.EventHandler(menuItemCancel_Click);

            //
            // menuItemRC
            //
            menuItemRC.Text = Resources.SharePhotoView_MenuItem_RC;
            menuItemRC.Click += new System.EventHandler(menuItemRC_Click);
            //menuItemRC.Click += new System.EventHandler(menuItemCancel_Click);

            //
            // menuItemRCC
            //
            menuItemRCC.Text = Resources.SharePhotoView_MenuItem_RCC;
            menuItemRCC.Click += new System.EventHandler(menuItemRCC_Click);
            //menuItemRCC.Click += new System.EventHandler(menuItemCancel_Click);

            //
            // menuItemCancel
            //
            menuItemSend.Text = Resources.SharePhotoView_MenuItem_Send;
            menuItemSend.Click += new System.EventHandler(menuItemSend_Click);
            //menuItemSend.Click += new System.EventHandler(menuItemCancel_Click);

            //
            //btnSentMessage
            //
            //btnSentMessage.TransparentButton = MasterForm.SkinManager.GetImage("MessagesNew");
            //btnSentMessage.TransparentButtonPressed = MasterForm.SkinManager.GetImage("MessagesNewPressed");
            btnSentMessage.Location = new Point(60, 5);
            btnSentMessage.Size = new Size(121, 25);
            btnSentMessage.Name = "btnSentMessage";
            btnSentMessage.Click += new System.EventHandler(btnSentMessage_Click);
            //btnSentMessage.Click += new EventHandler(BtnSentMessageClick);

            //
            // btnSentMessageText
            //
            this.btnSentMessageText.Name = "btnSentMessageText";
            this.btnSentMessageText.Location = new Point(0, 0);
            this.btnSentMessageText.Size = new Size(240, 36);
            this.btnSentMessageText.Font = FontCache.CreateFont("Tahoma", 14, FontStyle.Bold);
            this.btnSentMessageText.ForeColor = Color.White;
            this.btnSentMessageText.VerticalTextAlignment = Galssoft.VKontakteWM.Components.UI.VerticalAlignment.Center;
            this.btnSentMessageText.HorizontalTextAlignment = Galssoft.VKontakteWM.Components.UI.HorizontalAlignment.Center;
            this.btnSentMessageText.Text = Resources.SharePhotoView_MenuItem_Send;
            this.btnSentMessageText.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            //
            // btnSentMessageShadowText
            //
            this.btnSentMessageShadowText.Name = "btnSentMessageShadowText";
            this.btnSentMessageShadowText.Location = new Point(0, 0);
            this.btnSentMessageShadowText.Size = new Size(240, 36);
            this.btnSentMessageShadowText.Font = FontCache.CreateFont("Tahoma", 14, FontStyle.Bold);
            this.btnSentMessageShadowText.ForeColor = Color.Black;
            this.btnSentMessageShadowText.VerticalTextAlignment = Galssoft.VKontakteWM.Components.UI.VerticalAlignment.Center;
            this.btnSentMessageShadowText.HorizontalTextAlignment = Galssoft.VKontakteWM.Components.UI.HorizontalAlignment.Center;
            this.btnSentMessageShadowText.Text = Resources.SharePhotoView_MenuItem_Send;
            this.btnSentMessageShadowText.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            //
            // lblVoidPhoto // пустышка-текст
            //
            lblVoidPhoto.Location = new System.Drawing.Point(0, 36);
            lblVoidPhoto.Size = new System.Drawing.Size(240, 232);
            lblVoidPhoto.Font = FontCache.CreateFont("Tahoma", 14, FontStyle.Regular);
            lblVoidPhoto.FontColor = Color.White;
            lblVoidPhoto.FontColorShadow = Color.Gray;
            lblVoidPhoto.Text = Resources.UploadPhotoView_VoidPhotoLabel;
            lblVoidPhoto.HorizontalTextAlignment = Galssoft.VKontakteWM.Components.UI.HorizontalAlignment.Center;
            lblVoidPhoto.VerticalTextAlignment = Galssoft.VKontakteWM.Components.UI.VerticalAlignment.Center;
            lblVoidPhoto.Anchor = Galssoft.VKontakteWM.Components.UI.AnchorStyles.Bottom | Galssoft.VKontakteWM.Components.UI.AnchorStyles.Left | Galssoft.VKontakteWM.Components.UI.AnchorStyles.Right | Galssoft.VKontakteWM.Components.UI.AnchorStyles.Top;
            lblVoidPhoto.DropShadow = true;
            //lblVoidPhoto.ForeColor = Color.Magenta;
            lblVoidPhoto.Visible = false;

            ////
            //// filter
            ////
            //this.filter.Location = new Point(5, 5);
            ////this.filter._filter.TextBox.TextChanged += new EventHandler(TextChange);
            ////this.filter._filter.TextBox.GotFocus += new EventHandler(TextBoxGotFocus);
            ////this.filter._filter.TextBox.LostFocus += new EventHandler(TextBoxLostFocus);

            //
            // this
            //
            this.Size = new Size(240, 268);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = Color.FromArgb(51, 51, 51);
            this.AutoScroll = true;
            this.Resize += new System.EventHandler(FormResize);

            this.Canvas.Children.Add(header);
            this.Canvas.Children.Add(logo);
            this.Canvas.Children.Add(lblVoidPhoto);
            this.Canvas.Children.Add(giPhotoPreview);
            //this.Controls.Add(filter);

            //this.Canvas.Children.Add(btnSentMessage);
            this.Canvas.Children.Add(btnSentMessageShadowText);
            this.Canvas.Children.Add(btnSentMessageText);

            this.Canvas.RecalcDPIScaling();
            this.ResumeLayout(false);

            this.header.AlphaChannelImage = MasterForm.SkinManager.GetImage("Header");
            this.logo.AlphaChannelImage = MasterForm.SkinManager.GetImage("HeaderLogo");
            btnSentMessage.TransparentButton = MasterForm.SkinManager.GetImage("MessagesNew");
            btnSentMessage.TransparentButtonPressed = MasterForm.SkinManager.GetImage("MessagesNewPressed");

            btnSentMessageShadowText.Location = new Point(btnSentMessageShadowText.Location.X, btnSentMessageShadowText.Location.Y - UISettings.CalcPix(1));
        }

        #endregion

        private MainMenu mainMenu;

        private MenuItem menuItemAction;
        private MenuItem menuItemCancel;
        private MenuItem menuItemRC;
        private MenuItem menuItemRCC;
        private MenuItem menuItemSend;

        //private FilterControl filter;

        private UIButton btnSentMessage;
        private UILabel btnSentMessageText;
        private UILabel btnSentMessageShadowText;

        private MLLabel lblVoidPhoto;
        private GraphicsImage giPhotoPreview;

        private GraphicsImage header;
        private GraphicsImage logo;
    }
}
