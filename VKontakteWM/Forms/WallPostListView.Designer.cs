using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;
using Galssoft.VKontakteWM.Components.UI.Controls;
using System.Drawing;
using System;
using Galssoft.VKontakteWM.Components.UI;

namespace Galssoft.VKontakteWM.Forms
{
    partial class WallPostListView
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
            this.toolBar = new ToolBar();

            this.klvWallPostList = new WallPostListKineticListView();

            this.SuspendLayout();

            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Dock = System.Windows.Forms.DockStyle.Fill;            

            //
            // toolBar
            //
            toolBar.Size = new Size(240, 40);
            toolBar.Location = new Point(0, 228);
            toolBar.Name = "toolBar";
            toolBar.Visible = true;
            toolBar.ButtonsSize = new Size(34, 34);
            toolBar.Anchor = AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;

            //
            //toolbarButtonMain
            //
            toolbarButtonMain = toolBar.AddButton(ButtonStyle.AlphaChannel);
            toolbarButtonMain.TransparentButton = MasterForm.SkinManager.GetImage("ButtonOther");
            toolbarButtonMain.TransparentButtonPressed = MasterForm.SkinManager.GetImage("ButtonOtherPressed");
            toolbarButtonMain.Text = "Главн";
            //toolbarButtonMain.Click += new EventHandler(buttonMain_Click);
            toolbarButtonMain.Click += new EventHandler(toolbarButtonMain_Click);

            //
            //toolbarButtonFriends
            //
            toolbarButtonFriends = toolBar.AddButton(ButtonStyle.AlphaChannel);
            toolbarButtonFriends.TransparentButton = MasterForm.SkinManager.GetImage("ButtonOther");
            toolbarButtonFriends.TransparentButtonPressed = MasterForm.SkinManager.GetImage("ButtonOtherPressed");
            toolbarButtonFriends.Text = "";

            //
            //toolbarButtonWall
            //
            toolbarButtonWall = toolBar.AddButton(ButtonStyle.AlphaChannel);
            toolbarButtonWall.TransparentButton = MasterForm.SkinManager.GetImage("ButtonOther");
            toolbarButtonWall.TransparentButtonPressed = MasterForm.SkinManager.GetImage("ButtonOtherPressed");
            toolbarButtonWall.Text = "Стена";

            //
            //toolbarButtonAbout
            //
            toolbarButtonAbout = toolBar.AddButton(ButtonStyle.AlphaChannel);
            toolbarButtonAbout.TransparentButton = MasterForm.SkinManager.GetImage("ButtonOther");
            toolbarButtonAbout.TransparentButtonPressed = MasterForm.SkinManager.GetImage("ButtonOtherPressed");
            toolbarButtonAbout.Text = "О";
            //toolbarButtonAbout.Click += new EventHandler(buttonAbout_Click);

            //
            //toolbarButtonExit
            //
            toolbarButtonExit = toolBar.AddButton(ButtonStyle.AlphaChannel);
            toolbarButtonExit.TransparentButton = MasterForm.SkinManager.GetImage("ButtonOther");
            toolbarButtonExit.TransparentButtonPressed = MasterForm.SkinManager.GetImage("ButtonOtherPressed");
            toolbarButtonExit.Text = "Вых";
            //toolbarButtonExit.Click += new EventHandler(buttonExit_Click);
            toolbarButtonExit.Click += new EventHandler(toolbarButtonExit_Click);

            //
            // kinListViewFriends
            //
            this.klvWallPostList.Anchor = System.Windows.Forms.AnchorStyles.Left
                                            | System.Windows.Forms.AnchorStyles.Right
                                            | System.Windows.Forms.AnchorStyles.Top
                                            | System.Windows.Forms.AnchorStyles.Bottom;
            this.klvWallPostList.Location = new System.Drawing.Point(0, 0);
            this.klvWallPostList.BackColor = Color.White;
            this.klvWallPostList.ScrollAction = KineticControlBase.KineticControlScrollAction.ScrollingForTime;
            this.klvWallPostList.Name = "klvWallPostList";
            this.klvWallPostList.Size = new System.Drawing.Size(this.ClientSize.Width, this.ClientSize.Height - 40);

            //this.klvWallPostList.TabIndex = 2;
            //this.klvWallPostList.Select += new ItemSelectedEvent(kinListViewFriends_Select);
            //this.klvWallPostList.ReturnLongPress += new EventHandler<ListViewLongPressEventArgs>(kinListViewFriends_LongPress);

            //
            //this
            //            
            //this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            //this.AutoScroll = true;
            //this.ClientSize = new System.Drawing.Size(240, 268);
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            //this.ClientSize = new System.Drawing.Size(240, 268);
            this.Canvas.Children.Add(toolBar);
            this.Controls.Add(klvWallPostList);
            this.Name = "WallPostListView";
                       
            //components = new System.ComponentModel.Container();

            this.ResumeLayout();
        }

        #endregion

        private ToolBar toolBar;

        private ToolbarButton toolbarButtonMain;
        private ToolbarButton toolbarButtonFriends;
        private ToolbarButton toolbarButtonWall;
        private ToolbarButton toolbarButtonAbout;
        private ToolbarButton toolbarButtonExit;

        private WallPostListKineticListView klvWallPostList;
    }
}
