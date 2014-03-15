using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.Properties;
using Galssoft.VKontakteWM.Components.GDI;
using System.Drawing;
using System;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Components.Common.Configuration;

namespace Galssoft.VKontakteWM.Forms
{
    partial class UploadPhotoView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolBar = new BottomToolBar();

            this.btnLoadPhotoFromDisk = new UIButton(ButtonStyle.AlphaChannel);
            this.btnSnapPhotoWithinCamera = new UIButton(ButtonStyle.AlphaChannel);
            this.btnRotareImageCounterclockwise = new UIButton(ButtonStyle.AlphaChannel);
            this.btnRotareImageClockwise = new UIButton(ButtonStyle.AlphaChannel);

            this.upcUploadPhotoMobile = new UploadPhotoControl();
            this.upcUploadPhotoMain = new UploadPhotoControl();
            this.cmcPhotoDescription = new CommentControl();

            this.giPhotoPreview = new GraphicsImage();
            this.giPhotoBedding = new GraphicsImage();
            this.lblVoidPhoto = new MLLabel();

            this.giHeader = new GraphicsImage();
                        
            this.headerText = new UILabel();
            this.headerShadowText = new UILabel();            

            this.SuspendLayout();

            //
            // MainForm
            //
            this.Size = new Size(240, 268);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = Color.White;
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Resize += new EventHandler(FormResize);

            toolBar.ToolbarButtonNews.Click += new EventHandler(ToolbarButtonNewsClick);
            toolBar.ToolbarButtonMessages.Click += new EventHandler(ToolbarButtonMessagesClick);
            toolBar.ToolbarButtonFriends.Click += new EventHandler(ToolbarButtonFriendsClick);
            toolBar.ToolbarButtonExtras.Click += new EventHandler(ToolbarButtonExtrasClick);

            //
            //photoPreview // наша превьюха
            //
            giPhotoPreview.Location = new System.Drawing.Point(47, 50);
            giPhotoPreview.Name = "giPhotoPreview";
            giPhotoPreview.Size = new System.Drawing.Size(146, 74);

            //
            //photoBedding // ее фон
            //
            giPhotoBedding.Location = new System.Drawing.Point(42, 45);
            giPhotoBedding.Name = "giPhotoBedding";
            giPhotoBedding.Size = new System.Drawing.Size(156, 84);

            //
            // lblVoidPhoto // пустышка-текст
            //
            lblVoidPhoto.Location = new System.Drawing.Point(42, 45);
            lblVoidPhoto.Size = new System.Drawing.Size(156, 84);
            lblVoidPhoto.Font = FontCache.CreateFont("Tahoma", 14, FontStyle.Regular);
            lblVoidPhoto.FontColor = Color.FromArgb(129, 129, 129);
            lblVoidPhoto.FontColorShadow = Color.FromArgb(223, 223, 223);
            lblVoidPhoto.Text = Resources.UploadPhotoView_VoidPhotoLabel;
            lblVoidPhoto.HorizontalTextAlignment = HorizontalAlignment.Center;
            lblVoidPhoto.VerticalTextAlignment = VerticalAlignment.Center;
            lblVoidPhoto.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            lblVoidPhoto.DropShadow = true;

            // 
            // btnLoadFromDisk 
            //
            btnLoadPhotoFromDisk.Location = new System.Drawing.Point(0, 0);
            btnLoadPhotoFromDisk.Size = new Size(50, 37);
            btnLoadPhotoFromDisk.Name = "btnLoadPhotoFromDisk";
            btnLoadPhotoFromDisk.Anchor = AnchorStyles.Top | Components.UI.AnchorStyles.Left;
            btnLoadPhotoFromDisk.Text = "Диск";
            btnLoadPhotoFromDisk.Click += new EventHandler(btnLoadPhotoFromDisk_Click);

            // 
            // btnSnapPhotoWithinCamera
            //
            btnSnapPhotoWithinCamera.Location = new System.Drawing.Point(190, 0);
            btnSnapPhotoWithinCamera.Size = new Size(50, 37);
            btnSnapPhotoWithinCamera.Name = "btnSnapPhotoWithinCamera";
            btnSnapPhotoWithinCamera.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSnapPhotoWithinCamera.Text = "Камера";
            btnSnapPhotoWithinCamera.Click += new EventHandler(btnSnapPhotoWithinCamera_Click);

            // 
            // btnRotareImageCounterclockwise
            //
            btnRotareImageCounterclockwise.Location = new System.Drawing.Point(5, 45);
            btnRotareImageCounterclockwise.Size = new Size(32, 32);
            btnRotareImageCounterclockwise.Name = "btnRotareImageCounterclockwise";
            btnRotareImageCounterclockwise.Anchor = AnchorStyles.Left;
            btnRotareImageCounterclockwise.Text = "CCW";
            btnRotareImageCounterclockwise.Click += new EventHandler(btnRotareImageCounterclockwise_Click);

            // 
            // btnRotareImageClockwise
            //
            btnRotareImageClockwise.Location = new System.Drawing.Point(203, 45);
            btnRotareImageClockwise.Size = new Size(32, 32);
            btnRotareImageClockwise.Name = "btnRotareImageClockwise";
            btnRotareImageClockwise.Anchor = AnchorStyles.Right;
            btnRotareImageClockwise.Text = "CW";
            btnRotareImageClockwise.Click += new EventHandler(btnRotareImageClockwise_Click);

            //
            //header
            //
            giHeader.Name = "PhotoHeader";
            giHeader.Location = new Point(0, 0);
            giHeader.Stretch = true;
            giHeader.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            //
            //HeaderText
            //
            this.headerText.Name = "headerText";
            this.headerText.Location = new Point(50, 0);
            this.headerText.Size = new Size(140, 37);
            this.headerText.Font = FontCache.CreateFont("Tahoma", 19, FontStyle.Regular);
            this.headerText.ForeColor = Color.White;
            this.headerText.VerticalTextAlignment = VerticalAlignment.Center;
            this.headerText.HorizontalTextAlignment = HorizontalAlignment.Center;
            this.headerText.Text = Resources.UploadPhotoView_Header;
            this.headerText.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            //
            //HeaderShadowText
            //
            this.headerShadowText.Name = "headerShadowText";
            this.headerShadowText.Location = new Point(50, 0);
            this.headerShadowText.Size = new Size(140, 37);
            this.headerShadowText.Font = FontCache.CreateFont("Tahoma", 19, FontStyle.Regular);
            this.headerShadowText.ForeColor = Color.Gray;
            this.headerShadowText.VerticalTextAlignment = VerticalAlignment.Center;
            this.headerShadowText.HorizontalTextAlignment = HorizontalAlignment.Center;
            this.headerShadowText.Text = Resources.UploadPhotoView_Header;
            this.headerShadowText.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            //
            //cmcPhotoDescription
            //
            this.cmcPhotoDescription.Location = new Point(5, 144);
            this.cmcPhotoDescription.Text = string.Empty;
            this.cmcPhotoDescription.TextSubstitute = Resources.CommentControl_TextSubstitute;
            this.cmcPhotoDescription.Size = new Size(230, 32);
            this.cmcPhotoDescription.Font = FontCache.CreateFont("Tahoma", 14, FontStyle.Regular, true);
            this.cmcPhotoDescription.FontColor = Color.FromArgb(119, 126, 93);
            this.cmcPhotoDescription.FontColorUnactive = Color.FromArgb(187, 191, 174);
            this.cmcPhotoDescription.FontColorInvert = Color.White;
            this.cmcPhotoDescription.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
            this.cmcPhotoDescription.Click += new EventHandler(cmcPhotoDescription_Click);            

            // 
            //upcUploadPhoto
            //
            this.upcUploadPhotoMobile.Location = new System.Drawing.Point(5, 186);
            this.upcUploadPhotoMobile.Size = new System.Drawing.Size(110, 32);
            this.upcUploadPhotoMobile.Text = Resources.UploadPhoto_Designer_rbMobileAlbum_Text;
            this.upcUploadPhotoMobile.Font = FontCache.CreateFont("Tahoma", 14, FontStyle.Regular, true);
            this.upcUploadPhotoMobile.PressedFont = FontCache.CreateFont("Tahoma", 14, FontStyle.Regular, true);
            this.upcUploadPhotoMobile.FontColor = Color.FromArgb(129, 129, 129);
            this.upcUploadPhotoMobile.FontColorShadow = Color.FromArgb(223, 223, 223);
            this.upcUploadPhotoMobile.DropShadow = true;
            this.upcUploadPhotoMobile.Name = "upcUploadPhoto";
            this.upcUploadPhotoMobile.Anchor = AnchorStyles.Bottom;
            this.upcUploadPhotoMobile.Click += new EventHandler(upcUploadPhotoMobile_Click);

            // 
            //upcUploadPhotoMain
            //
            this.upcUploadPhotoMain.Location = new System.Drawing.Point(125, 186);
            this.upcUploadPhotoMain.Size = new System.Drawing.Size(110, 32);
            this.upcUploadPhotoMain.Text = Resources.UploadPhoto_Designer_rbPrivateAlbum_Text;
            this.upcUploadPhotoMain.Font = FontCache.CreateFont("Tahoma", 14, FontStyle.Regular, true);
            this.upcUploadPhotoMain.PressedFont = FontCache.CreateFont("Tahoma", 14, FontStyle.Regular, true);
            this.upcUploadPhotoMain.FontColor = Color.FromArgb(129, 129, 129);
            this.upcUploadPhotoMain.FontColorShadow = Color.FromArgb(223, 223, 223);
            this.upcUploadPhotoMain.DropShadow = true;
            this.upcUploadPhotoMain.Name = "upcUploadPhotoMain";
            this.upcUploadPhotoMain.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
            this.upcUploadPhotoMain.Click += new EventHandler(upcUploadPhotoMain_Click);                        

            //
            //this
            //
            this.Canvas.Children.Add(giHeader);
            this.Canvas.Children.Add(headerShadowText);
            this.Canvas.Children.Add(headerText);
            this.Canvas.Children.Add(giPhotoBedding);            
            this.Canvas.Children.Add(giPhotoPreview);
            this.Canvas.Children.Add(btnLoadPhotoFromDisk);
            this.Canvas.Children.Add(btnSnapPhotoWithinCamera);
            this.Canvas.Children.Add(btnRotareImageClockwise);
            this.Canvas.Children.Add(btnRotareImageCounterclockwise);
            this.Canvas.Children.Add(upcUploadPhotoMobile);
            this.Canvas.Children.Add(upcUploadPhotoMain);
            this.Canvas.Children.Add(cmcPhotoDescription);
            this.Canvas.Children.Add(lblVoidPhoto);
            this.Canvas.Children.Add(toolBar);

            this.Canvas.RecalcDPIScaling();

            this.ResumeLayout(false);           

            this.headerShadowText.Location = new Point(headerText.Left - UISettings.CalcPix(1), headerText.Top - UISettings.CalcPix(1));

            giHeader.AlphaChannelImage = MasterForm.SkinManager.GetImage("bdTop");

            btnLoadPhotoFromDisk.TransparentButton = MasterForm.SkinManager.GetImage("ButtonOther");
            btnLoadPhotoFromDisk.TransparentButtonPressed = MasterForm.SkinManager.GetImage("ButtonOtherPressed");
            btnLoadPhotoFromDisk.Size = new Size(UISettings.CalcPix(50), UISettings.CalcPix(37)); // del

            btnSnapPhotoWithinCamera.TransparentButton = MasterForm.SkinManager.GetImage("ButtonOther");
            btnSnapPhotoWithinCamera.TransparentButtonPressed = MasterForm.SkinManager.GetImage("ButtonOtherPressed");
            btnSnapPhotoWithinCamera.Size = new Size(UISettings.CalcPix(50), UISettings.CalcPix(37)); // del

            btnRotareImageCounterclockwise.TransparentButton = MasterForm.SkinManager.GetImage("ButtonOther");
            btnRotareImageCounterclockwise.TransparentButtonPressed = MasterForm.SkinManager.GetImage("ButtonOtherPressed");
            btnRotareImageCounterclockwise.Size = new Size(UISettings.CalcPix(32), UISettings.CalcPix(32)); // del

            btnRotareImageClockwise.TransparentButton = MasterForm.SkinManager.GetImage("ButtonOther");
            btnRotareImageClockwise.TransparentButtonPressed = MasterForm.SkinManager.GetImage("ButtonOtherPressed");
            btnRotareImageClockwise.Size = new Size(UISettings.CalcPix(32), UISettings.CalcPix(32)); // del       
        }

        #endregion     

        private UIButton btnLoadPhotoFromDisk;
        private UIButton btnSnapPhotoWithinCamera;
        private UIButton btnRotareImageCounterclockwise;
        private UIButton btnRotareImageClockwise;

        private UploadPhotoControl upcUploadPhotoMobile;
        private UploadPhotoControl upcUploadPhotoMain;
        private CommentControl cmcPhotoDescription;        

        private GraphicsImage giPhotoPreview;
        private GraphicsImage giHeader;
        private GraphicsImage giPhotoBedding;
        private UILabel headerText;
        private UILabel headerShadowText;
        private MLLabel lblVoidPhoto;

        private BottomToolBar toolBar;
    }
}
