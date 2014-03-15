using System;
using System.Windows.Forms;
using Galssoft.VKontakteWM.ApplicationLogic;
using Galssoft.VKontakteWM.Common;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.Configuration;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.Process;
using Galssoft.VKontakteWM.Components.Skin;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Properties;
using Galssoft.VKontakteWM.Components.GDI;
using System.Drawing;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Microsoft.WindowsCE.Forms;
using Galssoft.VKontakteWM.Components.Cache;
using Galssoft.VKontakteWM.CustomControls;

namespace Galssoft.VKontakteWM.Forms
{
    public partial class MasterForm : FormBase
    {
        #region Members 

        private static SkinManager _skinManager;
        private static MasterForm _master;
        private static int _startTick;

        #endregion

        #region Constructors

        public MasterForm()
        {
            _master = this;
            _startTick = Environment.TickCount;
            InitializeComponent();
        }

        #endregion

        protected override void OnDeactivate(EventArgs e)
        {
            Text = SystemConfiguration.ApplicationName;

            base.OnDeactivate(e);

            OffscreenBuffer.ReleaseOffscreenBuffer();
        }

        protected override void OnActivated(EventArgs e)
        {
            RestoreText();

            base.OnActivated(e);
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                SetTextInvoke(value);
            }
        }

        private void SetTextInvoke(string text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(SetTextInvoke), text);
            }
            else
            {
                base.Text = text;
            }
        }

        public void RestoreText()
        {
            if (ViewManager.CurrentView != null && !String.IsNullOrEmpty(ViewManager.CurrentView.Title))
            {
                Text = ViewManager.CurrentView.Title;
            }
        }

        private Point _sipPosition = new Point(0, 0);

        private void MoveSIP(bool restore, int screenWidth)
        {
            Win32.RECT rect = new Win32.RECT();

            IntPtr hSip = Win32.FindWindow("MS_SIPBUTTON", "MS_SIPBUTTON");

            if (_sipPosition.X + _sipPosition.Y == 0)
            {
                Win32.GetWindowRect(hSip, ref rect);
                _sipPosition = new Point(rect.X, rect.Y);
            }

            Win32.SetWindowPos(hSip,
                               (IntPtr)(-1),
                               (restore) ? _sipPosition.X : screenWidth + 20,
                               _sipPosition.Y, 0, 0,
                               Win32.SWP.NOSIZE | Win32.SWP.NOACTIVATE | Win32.SWP.NOZORDER);
        }

        protected override void OnLoad(EventArgs e)
        {
            (new InputPanel()).Enabled = false;

            BuildControls();

            ViewManager.Initialize(this);

            StatusDialogControl.InitImages(SkinManager.GetImage("dlg_BB"),
                                           SkinManager.GetImage("dlg_LB"),
                                           SkinManager.GetImage("dlg_LL"),
                                           SkinManager.GetImage("dlg_LT"),
                                           SkinManager.GetImage("dlg_RB"),
                                           SkinManager.GetImage("dlg_RR"),
                                           SkinManager.GetImage("dlg_RT"),
                                           SkinManager.GetImage("dlg_TT"),
                                           SkinManager.GetImage("ButtonOther"),
                                           SkinManager.GetImage("ButtonOtherPressed"));

            LoadingControl.InitImages(SkinManager.GetImage("dlg_BB"),
                                      SkinManager.GetImage("dlg_LB"),
                                      SkinManager.GetImage("dlg_LL"),
                                      SkinManager.GetImage("dlg_LT"),
                                      SkinManager.GetImage("dlg_RB"),
                                      SkinManager.GetImage("dlg_RR"),
                                      SkinManager.GetImage("dlg_RT"),
                                      SkinManager.GetImage("dlg_TT"),
                                      SkinManager.GetImage("ButtonOther"),
                                      SkinManager.GetImage("ButtonOtherPressed"),
                                      SkinManager.GetImage("ProgressFull"),
                                      SkinManager.GetImage("ProgressEmpty"));

            DialogControl.InitImages(SkinManager.GetImage("dlg_BB"),
                                     SkinManager.GetImage("dlg_LB"),
                                     SkinManager.GetImage("dlg_LL"),
                                     SkinManager.GetImage("dlg_LT"),
                                     SkinManager.GetImage("dlg_RB"),
                                     SkinManager.GetImage("dlg_RR"),
                                     SkinManager.GetImage("dlg_RT"),
                                     SkinManager.GetImage("dlg_TT"),
                                     SkinManager.GetImage("ButtonOther"),
                                     SkinManager.GetImage("ButtonOtherPressed"));

            UpdateInfoDialogControl.InitImages(SkinManager.GetImage("dlg_BB"),
                                               SkinManager.GetImage("dlg_LB"),
                                               SkinManager.GetImage("dlg_LL"),
                                               SkinManager.GetImage("dlg_LT"),
                                               SkinManager.GetImage("dlg_RB"),
                                               SkinManager.GetImage("dlg_RR"),
                                               SkinManager.GetImage("dlg_RT"),
                                               SkinManager.GetImage("dlg_TT"),
                                               SkinManager.GetImage("ButtonOther"),
                                               SkinManager.GetImage("ButtonOtherPressed"));

            base.OnLoad(e);

            if (string.IsNullOrEmpty(Globals.BaseLogic.IDataLogic.GetToken()))
            {
                Navigate<LoginController>();
            }
            else
            {
                bool isCacheClear = true;                
                //bool isCacheClear2 = true;

                //foreach (var entry in Cache.GetEntryNames(string.Empty))
                //{
                //    if (entry == "ActivityResponse")
                //    {
                //        if (!isCacheClear2)
                //        {
                //            isCacheClear = false;
                //        }
                //        else
                //        {
                //            isCacheClear2 = false;
                //        }
                //    }

                //    if (entry == "UpdatesPhotosResponse")
                //    {
                //        if (!isCacheClear2)
                //        {
                //            isCacheClear = false;
                //        }
                //        else
                //        {
                //            isCacheClear2 = false;
                //        }
                //    }
                //}

                isCacheClear = false;

                // если в кэше нет ActivityResponse и UpdatesPhotosResponse то считаем, что пользователю нельзя заходить
                if (isCacheClear)
                {
                    Navigate<LoginController>();
                }
                else
                {
                    // надо что-то показать пользователю что б не плакал...
                    Navigate<StatusUpdatesListController>("LoadingPreview");
                }
            }

            (new InputPanel()).Enabled = false;        
        }

        internal void Initialize()
        {
            UISettings.InitSettings();

            try
            {
                DialogControl.InitializeDialogs(this);
                StatusDialogControl.InitializeDialogs(this);
                LoadingControl.InitializeDialogs(this);
                UpdateInfoDialogControl.InitializeDialogs(this);

                DebugHelper.WriteLogEntry("ScreenDPI = " + UISettings.ScreenDPI);

                try
                {
                    if (UISettings.ScreenDPI <= 128)
                    {
                        _skinManager = new SkinManager(SystemConfiguration.AppInstallPath + "\\Skins", 96);
                        _skinManager.LoadSkin("VKSkin96");
                    }
                    else
                    {
                        _skinManager = new SkinManager(SystemConfiguration.AppInstallPath + "\\Skins", 192);
                        _skinManager.LoadSkin("VKSkin192");
                    }
                }
                catch
                {
                    MessageBox.Show(Resources.MasterForm_Initialize_SkinManagerLoadingException);
                    DebugHelper.WriteLogEntry(Resources.MasterForm_Initialize_SkinManagerLoadingException);
                }

                if (!_skinManager.Initialized)
                {
                    _skinManager.LoadSkin("Default");
                }
            }
            catch (Exception)
            {
                MessageBox.Show(Resources.MasterForm_Initialize_SkinManagerDefaultLoadingException);
                DebugHelper.WriteLogEntry(Resources.MasterForm_Initialize_SkinManagerDefaultLoadingException);
                Application.Exit();
            }
        }

        #region Properties

        /// <summary>
        /// Возвращает инициализированный класс управляющий скинами
        /// </summary>
        public static SkinManager SkinManager
        {
            get { return _skinManager; }
        }

        #endregion
        
        public static IImage GetSkinManagerImageInvoked(string imageName)
        {
            if (_master.InvokeRequired)
            {
                return (IImage)_master.Invoke((GetSkinImageDelegate)GetSkinManagerImageInvoked, imageName);
            }
            else
            {
                return SkinManager.GetImage(imageName);
            }
        }
        private delegate IImage GetSkinImageDelegate(string imageName);

        #region Navigation

        /// <summary>
        /// Не понимаю, почему не перенести этот метод в NavigationService? Ведь так для его использования приходится классам контроллеров явно зависеть от модуля MasterForm (Н.Ш.)
        /// PPS в таком виде как он сейчас - да. Но если начинать расширять его то тогда все равно прийдется вынести сюда (зависимости можно будет брать из синглтона мастер формы если таковой будет)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        public static void Navigate<T>(params object[] parameters) where T : Controller, new()
        {
            Type viewType = typeof(T);
            if (NavigationService.Controllers.ContainsKey(viewType.Name))
            {
                NavigationService.Navigate(viewType.Name, parameters);
            }
            else
            {
                T controller = new T();
                controller.View.Load();

                NavigationService.Navigate(controller, parameters);
            }
        }

        #endregion
    }
}