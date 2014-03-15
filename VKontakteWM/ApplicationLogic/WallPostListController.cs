using System;

using System.Collections.Generic;
using System.Text;

using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Forms;
using Galssoft.VKontakteWM.Common;
using Galssoft.VKontakteWM.Components.Common.ResponseClasses;
using Galssoft.VKontakteWM.Components.Configuration;
using Galssoft.VKontakteWM.Components.Server;
using System.Windows.Forms;

using Galssoft.VKontakteWM.Components;
using Galssoft.VKontakteWM.Components.Cache;
using Galssoft.VKontakteWM.Components.ImageClass;
using Galssoft.VKontakteWM.Components.Common.Configuration;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    internal class WallPostListController : Controller<List<WallPostListViewItem>>
    {
        #region Constructors

        public WallPostListController() : base(new WallPostListView())
        {
            Name = "WallPostListController";

            view.Model = new List<WallPostListViewItem>();

            _afterLoadImageEventHandler += OnAfterLoadImage;
        }

        #endregion

        [PublishEvent("OnFriendAvatarLoad")]
        public event EventHandler FriendAvatarLoad;

        #region Controller implementations

        public override void Activate()
        {
            view.Activate();
        }

        public override void Deactivate()
        {
            view.Deactivate();
        }

        private AfterLoadImageEventHandler _afterLoadImageEventHandler;

        /// <summary>
        /// Вызывается по завершении загрузки изображения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAfterLoadImage(object sender, AfterLoadImageEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.ImageFilename))
            {
                string fileName = SystemConfiguration.AppInstallPath + @"\Cache\Files\" + e.ImageFilename;

                ImageClass.ShrinkToMaxLinearSize(fileName, 50);

                if (FriendAvatarLoad != null)
                {
                    FriendAvatarLoad(this, new AfterLoadFriendAvatarEventArgs(fileName, e.ImageLast));
                }
            }
        }

        protected override void OnViewStateChanged(string key)
        {
            if (key == "LoadList")
            {
                try
                {
                    //ActivityResponse newActivityResponse = Globals.BaseLogic.LoadActivityDataList(25, true, false);
                    PhotosCommentsResponse newPhotosCommentsRespounse = Globals.BaseLogic.LoadPhotosComments(20, true, false);

                    view.Model.Clear();

                    foreach (CommentPost newCommentPost in newPhotosCommentsRespounse.pcrComments)
                    {
                        WallPostListViewItem newWallPostListViewItem = new WallPostListViewItem();

                        newWallPostListViewItem.UserName = newCommentPost.cpPostSender.psUserName; //newActivityData.adDataSender.psUserName;
                        newWallPostListViewItem.Status = newCommentPost.cpWallData.wdText; //newActivityData.adText;
                        newWallPostListViewItem.StatusChangeDate = newCommentPost.cpTime.ToString(); //newActivityData.adTime.ToString();

                        newWallPostListViewItem.Avatar = SystemConfiguration.AppInstallPath + @"\Cache\Files\Thumb\" + HttpUtility.GetMd5Hash(newCommentPost.cpPhotoData.pdPhotoURL130px);

                        bool res;

                        //загрузка превьюшки
                        res = Globals.BaseLogic.ICommunicationLogic.LoadImage(newCommentPost.cpPhotoData.pdPhotoURL130px, @"Thumb\" + HttpUtility.GetMd5Hash(newCommentPost.cpPhotoData.pdPhotoURL130px), false, _afterLoadImageEventHandler, UISettings.CalcPix(50), 0, "int");
                        
                        if (res)
                        {
                            newWallPostListViewItem.IsAvatarLoaded = true;
                        }
                        else
                        {
                            newWallPostListViewItem.IsAvatarLoaded = false;
                        }

                        view.Model.Add(newWallPostListViewItem);
                    }

                    //foreach (ActivityData newActivityData in newActivityResponse.arActivityDatas)
                    //{
                    //    WallPostListViewItem newWallPostListViewItem = new WallPostListViewItem();

                    //    newWallPostListViewItem.UserName = newActivityData.adDataSender.psUserName;
                    //    newWallPostListViewItem.Status = newActivityData.adText;
                    //    newWallPostListViewItem.StatusChangeDate = newActivityData.adTime.ToString();

                    //    newWallPostListViewItem.Avatar = SystemConfiguration.AppInstallPath + @"\Cache\Files\Thumb\" + HttpUtility.GetMd5Hash(newActivityData.adDataSender.psUserPhotoURL);

                    //    bool res;

                    //    if (newActivityData.adDataSender.psUserPhotoURL == "0")
                    //    {
                    //        res = false;
                    //    }
                    //    else
                    //    {
                    //        //загрузка аватара
                    //        res = Globals.BaseLogic.ICommunicationLogic.LoadImage(newActivityData.adDataSender.psUserPhotoURL, @"Thumb\" + HttpUtility.GetMd5Hash(newActivityData.adDataSender.psUserPhotoURL), false, _afterLoadImageEventHandler);
                    //    }

                    //    if (res)
                    //    {
                    //        newWallPostListViewItem.IsAvatarLoaded = true;
                    //    }
                    //    else
                    //    {
                    //        newWallPostListViewItem.IsAvatarLoaded = false;
                    //    }

                    //    view.Model.Add(newWallPostListViewItem);
                    //}

                    //view.Model.Sort();

                    view.UpdateView("LoadListResponse");
                }
                catch (Exception newException)
                {
                    ViewData["LoadListResponseMessage"] = newException.Message;

                    view.UpdateView("LoadListResponseNegative");
                }

                #region оригинальный код
                //try
                //{
                //    WallResponse newWallResponse = Globals.BaseLogic.LoadWallPostList(15, true, false);

                //    //WallResponse newWallResponse = new WallResponse();

                //    #region Тестовые данные
                //    //WallPost WallPost1 = new WallPost();
                //    //newWallResponse.wrWallPosts.Add(WallPost1);

                //    //WallData WallData1 = new WallData();
                //    //WallPost1.wpWallData = WallData1;
                //    //WallData1.wdText = "Сообщение со стены!!!";

                //    //PostSender PostSender1 = new PostSender();
                //    //WallPost1.wpPostSender = PostSender1;
                //    //PostSender1.psUserName = "Какой-то чувак";

                //    //WallPost WallPost2 = new WallPost();
                //    //newWallResponse.wrWallPosts.Add(WallPost2);

                //    //WallData WallData2 = new WallData();
                //    //WallPost2.wpWallData = WallData2;
                //    //WallData2.wdText = "Привет мир! Hello World!";

                //    //PostSender PostSender2 = new PostSender();
                //    //WallPost2.wpPostSender = PostSender2;
                //    //PostSender2.psUserName = "Я любимый";

                //    //newWallResponse.wrWallPosts.Add(WallPost1);
                //    //newWallResponse.wrWallPosts.Add(WallPost2);
                //    #endregion

                //    view.Model.Clear();

                //    foreach (WallPost newWallPost in newWallResponse.wrWallPosts)
                //    {
                //        WallPostListViewItem newWallPostListViewItem = new WallPostListViewItem();

                //        newWallPostListViewItem.Name = newWallPost.wpPostSender.psUserName;
                //        newWallPostListViewItem.Text = newWallPost.wpWallData.wdText;

                //        view.Model.Add(newWallPostListViewItem);
                //    }

                //    view.Model.Sort();

                //    view.UpdateView("LoadListResponse");
                //}
                //catch (Exception newException)
                //{
                //    ViewData["LoadListResponseMessage"] = newException.Message;

                //    view.UpdateView("LoadListResponseNegative");
                //}
                #endregion
                //
            }

            if (key == "ReloadList")
            {
                view.UpdateView("ReloadListResponse");
            }

            if (key == "GoToMain")
            {
                MasterForm.Navigate<MainController>();
            }

            if (key == "CloseApplication")
            {
                //TODO Закрыть приложение
                Application.Exit();
            }
        }

        #endregion
    }
}
