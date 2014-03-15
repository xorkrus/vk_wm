using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Components.UI.Transitions;
using Galssoft.VKontakteWM.Components.UI.Wrappers;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Properties;
using Galssoft.VKontakteWM.ApplicationLogic;
using Galssoft.VKontakteWM.Components.ImageClass;
using Microsoft.WindowsCE.Forms;

namespace Galssoft.VKontakteWM.Forms
{
    public partial class WallPostListView : UIViewBase, IView<List<WallPostListViewItem>>
    {
        public WallPostListView()
        {
            InitializeComponent();
        }

        public void Load()
        {
            OnViewStateChanged("LoadList");
        }

        #region IView Members

        public string Title
        {
            get { return Resources.WallPostList_View_Title; }
        }

        public MainMenu ViewMenu
        {
            get { return null; }
        }

        public void OnBeforeActivate()
        {

        }

        public void OnAfterDeactivate()
        {

        }

        public void OnAfterActivate()
        {
            (new InputPanel()).Enabled = false;

            this.toolBar.SelectButton(toolbarButtonWall);
        }

        public Bitmap CreateScreenShot()
        {
            return null;
        }

        public TransitionType GetTransition(IView from)
        {
            return TransitionType.Basic;
        }

        public new ViewDataDictionary<List<WallPostListViewItem>> ViewData { get; set; }

        public new List<WallPostListViewItem> Model { get; set; }

        #endregion

        protected override void OnUpdateView(string key)
        {
            if (key == "LoadListResponse")
            {
                klvWallPostList.Clear();
                klvWallPostList.DataSource = Model;
                klvWallPostList.Reload();
                klvWallPostList.Refresh();
            }

            if (key == "ReloadListResponse")
            {
                klvWallPostList.Reload();
            }

            if (key == "LoadListResponseNegative")
            {
                DialogControl.ShowQuery((string) ViewData["LoadListResponseMessage"], DialogButtons.OK);
                //MessageBox.Show((string)ViewData["LoadListResponseMessage"]);
            }

            //base.OnUpdateView(key);
        }

        void toolbarButtonExit_Click(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("CloseApplication");
            }            
        }

        void toolbarButtonMain_Click(object sender, EventArgs e)
        {
            using (new WaitWrapper(false))
            {
                OnViewStateChanged("GoToMain");
            }   
        }

        private void OnFriendAvatarLoad(object sender, EventArgs e)
        {
            AfterLoadFriendAvatarEventArgs ee = (AfterLoadFriendAvatarEventArgs)e;

            string friendAvatarPath = ee.ImageFileName;

            if (friendAvatarPath != "")
            {
                foreach (WallPostListViewItem item in klvWallPostList.Items)
                {
                    if (item.Avatar == friendAvatarPath)
                    {
                        item.IsAvatarLoaded = true;
                    }
                }

                OnViewStateChanged("ReloadList");
            }
        }
    }
}
