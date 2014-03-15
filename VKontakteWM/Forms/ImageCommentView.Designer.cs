using System;

using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Properties;

namespace Galssoft.VKontakteWM.Forms
{
    partial class ImageCommentView
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
            this.menuItemRefresh = new MenuItem();
            this.menuItemSendComment = new MenuItem();
            this.menuItemPutInAlbum = new MenuItem();
            this.menuItemSaveOnDisk = new MenuItem();
            this.klvDetailed = new ImageDetailedKineticListView();
            this.contexMenu = new ContextMenu();
            this.menuItemSendMessage = new MenuItem();

            this.SuspendLayout();
            //
            // mainMenu
            //
            mainMenu.MenuItems.Add(menuItemCancel);
            mainMenu.MenuItems.Add(menuItemAction);
            
            //
            // menuItemAction
            //
            menuItemAction.Text = Resources.ImageCommentView_MenuItem_Action;
            //menuItemAction.MenuItems.Add(menuItemRefresh);
            //menuItemAction.Click += new EventHandler(MenuItemChangeClick);
            menuItemAction.MenuItems.Add(menuItemSendComment);
            menuItemAction.MenuItems.Add(menuItemPutInAlbum);
            menuItemAction.MenuItems.Add(menuItemSaveOnDisk);
            menuItemAction.MenuItems.Add(menuItemRefresh);

            //
            // menuItemCancel
            //
            menuItemCancel.Text = Resources.ImageCommentView_MenuItem_Back;
            menuItemCancel.Click += new EventHandler(MenuItemCancelClick);

            //
            // contextMenu
            //
            contexMenu.MenuItems.Add(this.menuItemSendMessage);

            //
            // menuItemSendMessage
            //
            menuItemSendMessage.Text = Resources.ContextMenu_SendMessade;
            menuItemSendMessage.Click += new EventHandler(menuItemSendMessage_Click);
            //menuItemSendMessage.Click += new EventHandler(menuItemSendMessage_Click);

            //
            // menuItemRefresh
            //
            menuItemRefresh.Text = Resources.ImageCommentView_MenuItem_Refresh;
            menuItemRefresh.Click += new EventHandler(menuItemRefresh_Click);
            //menuItemRefresh.Click += new EventHandler(MenuItemCancelClick);

            //
            // menuItemSendComment
            //
            menuItemSendComment.Text = Resources.ImageCommentView_MenuItem_SendComment;
            menuItemSendComment.Click += new EventHandler(MenuItemSendCommentClick);

            //
            // menuItemPutInAlbum
            //
            menuItemPutInAlbum.Text = Resources.ImageCommentView_MenuItem_PutInAlbum;
            menuItemPutInAlbum.Click += new EventHandler(MenuItemPutInAlbumClick);

            //
            // menuItemSaveOnDisk
            //
            menuItemSaveOnDisk.Text = Resources.ImageCommentView_MenuItem_SavePhoto;
            menuItemSaveOnDisk.Click += new EventHandler(MenuItemSaveOnDiskClick);

            //
            //klvFriendsList
            //
            klvDetailed.Anchor = System.Windows.Forms.AnchorStyles.Left
                                    | System.Windows.Forms.AnchorStyles.Right
                                    | System.Windows.Forms.AnchorStyles.Top
                                    | System.Windows.Forms.AnchorStyles.Bottom;
            klvDetailed.Location = new System.Drawing.Point(0, 0);
            klvDetailed.BackColor = Color.White;
            klvDetailed.BackgroundIImage = MasterForm.SkinManager.GetImage("List-background");
            klvDetailed.ContentDownShadow = MasterForm.SkinManager.GetImage("ContentDownShadow");
            klvDetailed.ContentUpShadow = MasterForm.SkinManager.GetImage("ContentUpShadow");
            klvDetailed.ShowContentShadows = true;
            klvDetailed.OutsideDownShadow = MasterForm.SkinManager.GetImage("ContentUpShadow");
            klvDetailed.OutsideUpShadow = MasterForm.SkinManager.GetImage("ContentDownShadow");
            klvDetailed.ShowInnerShadows = true;
            klvDetailed.Name = "klvDetailed";
            klvDetailed.Size = new System.Drawing.Size(240, 268);
            klvDetailed.ReturnLongPress += new EventHandler<Galssoft.VKontakteWM.Components.UI.CompoundControls.ListViewLongPressEventArgs>(klvDetailed_ReturnLongPress);
            klvDetailed.MouseUp += new MouseEventHandler(klvDetailed_MouseUp);

            //klvPhotoCommentsUpdatesList.ReturnLongPress += new EventHandler<ListViewLongPressEventArgs>(klvPhotoCommentsUpdatesList_ReturnLongPress);
            //klvPhotoCommentsUpdatesList.MouseUp += new MouseEventHandler(klvPhotoCommentsUpdatesList_MouseUp);

            //
            // ChangeCommentView
            //
            this.Size = new Size(240, 268);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(klvDetailed);
            this.BackColor = Color.White;
            this.AutoScroll = true;

            this.Canvas.RecalcDPIScaling();

            this.ResumeLayout(false);
        }

        #endregion

        private MainMenu mainMenu;

        private MenuItem menuItemCancel;
        private MenuItem menuItemAction;

        private MenuItem menuItemRefresh;
        private MenuItem menuItemSendComment;
        private MenuItem menuItemPutInAlbum;
        private MenuItem menuItemSaveOnDisk;

        private ImageDetailedKineticListView klvDetailed;

        private ContextMenu contexMenu;
        private MenuItem menuItemSendMessage;
    }
}
