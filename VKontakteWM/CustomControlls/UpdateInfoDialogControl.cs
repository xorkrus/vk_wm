using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.Common.Configuration;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Properties;

namespace Galssoft.VKontakteWM.CustomControlls
{
    public partial class UpdateInfoDialogControl : UIViewBase
    {
        #region Members

        private static Form _masterForm;
        private static int _visibleDialogsCounter;

        private DialogResult _dialogresult;
        private Rectangle _contentRectangle;
        private Rectangle _textRectangle;
        private Rectangle _backgroundRectangle;
        private readonly int _margin;
        private static string _message;
        private static bool _shortview;
        //private static string _messageNew;
        private bool _initialized;
        private MainMenu _menu;
        private readonly Color _gradientColor1 = Color.FromArgb(255, 255, 255);
        private readonly Color _gradientColor2 = Color.White;
        private readonly Color _gradientColor3 = Color.FromArgb(255, 255, 255);
        private const float _gradientCenter = (float)1 / 3;

        private DialogResult _buttonUpdateDialogResult;
        private DialogResult _buttonSkipDialogResult;
        private DialogResult _buttonCancelDialogResult;

        private GraphicsImage _imgBB;
        private GraphicsImage _imgLB;
        private GraphicsImage _imgLL;
        private GraphicsImage _imgLT;
        private GraphicsImage _imgRB;
        private GraphicsImage _imgRR;
        private GraphicsImage _imgRT;
        private GraphicsImage _imgTT;

        private static IImage dlg_BB;
        private static IImage dlg_LB;
        private static IImage dlg_LL;
        private static IImage dlg_LT;
        private static IImage dlg_RB;
        private static IImage dlg_RR;
        private static IImage dlg_RT;
        private static IImage dlg_TT;
        private static IImage dlg_button;
        private static IImage dlg_buttonPressed;

        #endregion

        #region Constructors

        /// <summary>
        /// Инициализация всех диалоговых контролов
        /// </summary>
        /// <param name="masterForm"></param>
        public static void InitializeDialogs(Form masterForm)
        {
            _masterForm = masterForm;
        }

        //Показать сообщение с кнопками
        public static DialogResult ShowQuery(string status, bool shortview)
        {
            DebugHelper.WriteTraceEntry("----------");
            DebugHelper.WriteTraceEntry("ShowQuery called");

            //using (new WaitWrapper(true))
            {
                DebugHelper.WriteTraceEntry("ShowQuery beginning");
                if (_masterForm == null)
                    throw new ApplicationException("UpdateInfoDialogControl not initialized.");

                return showMessage(status, shortview);
            }
        }

        //private delegate DialogResult showMessageDelegate(out string message);
        private static DialogResult showMessage(string status, bool shortview)
        {
            DebugHelper.WriteTraceEntry("PasswordRestoreDialogControl showMessage invoked");
            var control = new UpdateInfoDialogControl { _dialogresult = DialogResult.None };
            AddControlToForm(control);
            DebugHelper.WriteTraceEntry("PasswordRestoreDialogControl control to form added");
            control.Show(status, shortview);

            DebugHelper.WriteTraceEntry("PasswordRestoreDialogControl resume layout");
            DebugHelper.WriteTraceEntry("PasswordRestoreDialogControl MSG cycle beginning");
            DebugHelper.FlushTraceBuffer();

            var msg = new MSG();
            while (GetMessage(out msg, IntPtr.Zero, 0, 0))
            {
                TranslateMessage(ref msg);
                DispatchMessage(ref msg);

                if (control._dialogresult != DialogResult.None)
                    break;
            }

            RemoveControlFromForm(control);
            DialogResult res = control._dialogresult;
            control.Dispose();
            return res;
        }

        //Добавить контрол на форму
        private static void AddControlToForm(UpdateInfoDialogControl control)
        {
            control.Location = new Point(0, 0);
            control.Size = _masterForm.ClientSize;
            control.Dock = DockStyle.Fill;
            control.Visible = false;
            control._initialized = true;
            _masterForm.Controls.Add(control);
            control.BringToFront();
            _visibleDialogsCounter++;
        }

        //Удалить контрол с формы
        private static void RemoveControlFromForm(UpdateInfoDialogControl control)
        {
            control.Hide();
            _masterForm.Controls.Remove(control);
            _masterForm.Menu = control._menu;
            _visibleDialogsCounter--;
        }

        private UpdateInfoDialogControl()
        {
            InitializeComponent();

            _margin = UISettings.CalcPix(10);
        }

        /// <summary>
        /// Показать сообщение с текстом и кнопками
        /// </summary>
        /// <param name="status"></param>
        private void Show(string status, bool shortview)
        {
            DebugHelper.WriteTraceEntry("Menu Hiding");
            _menu = _masterForm.Menu;
            _masterForm.Menu = null;
            DebugHelper.WriteTraceEntry("Menu Hide");
            DebugHelper.WriteTraceEntry("Call overloaded Show()");
            PrepareBackground();
            DebugHelper.WriteTraceEntry("Background prepared");

            DebugHelper.WriteTraceEntry("Canvas pre suspended");
            Canvas.SuspendLayout();

            //textBoxStatus.Text = status;
            _buttonUpdateDialogResult = DialogResult.Yes;
            _buttonSkipDialogResult = DialogResult.No;
            _buttonCancelDialogResult = DialogResult.Cancel;

            DebugHelper.WriteTraceEntry("Canvas pre resumed");
            Canvas.ResumeLayout();
            DebugHelper.WriteTraceEntry("Canvas resumed");

            _message = status;
            _shortview = shortview;

            initImages();
            CalcContentRectangle();

            MessageBeep(32);
            DebugHelper.WriteTraceEntry("Start before Show()");
            Show();
        }

        private void PrepareBackground()
        {
            if (BackgroundImage != null)
            {
                BackgroundImage.Dispose();
            }
            CreateViewsScreenshot();
            if (BackgroundImage != null)
            {
                Gdi.CreateAlphaBitmap(60, BackgroundImage);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (BackgroundImage != null && Width == BackgroundImage.Width && Height == BackgroundImage.Height)
            {
                return;
            }

            if (_initialized)
            {
                CalcContentRectangle();
            }

            if (_initialized)
            {
                PrepareBackground();
            }
        }

        private void SetFont(Gdi g)
        {
            g.Font = FontCache.CreateFont("Calibri", 13, FontStyle.Regular, true);
            g.TextAlign = Win32.TextAlign.TA_LEFT;
            g.TextColor = Color.Black;
        }

        private void CalcContentRectangle()
        {
            int buttonHeight = UISettings.CalcPix(24);
            int buttonWidth = UISettings.CalcPix(75);

            int w = Width / 10;

            int textHeight = 0;

            if (!string.IsNullOrEmpty(_message))
            {
                using (Gdi g = new Gdi(this))
                {
                    SetFont(g);

                    Size textSize = g.GetDrawTextSize(_message, w * 9 - _margin * 2, Win32.DT.WORDBREAK);

                    textHeight = textSize.Height;
                }
            }

            int h = _margin + textHeight + _margin + (buttonHeight * 3 + _margin / 2 * 2) + _margin;

            if (_shortview)
            {
                h -= (buttonHeight + _margin / 2);
            }

            if (h > Height / 10 * 6)
            {
                h = Height / 10 * 6;
            }

            int contentWidth = w * 9;
            int contentHeight = h;

            int vPos = (Height - contentHeight) / 2;
            int hPos = w / 2;

            _contentRectangle = new Rectangle(hPos, vPos, contentWidth, contentHeight);

            _textRectangle = new Rectangle(_contentRectangle.X + _margin,
                                            _contentRectangle.Y + _margin,
                                            _contentRectangle.Width - _margin * 2,
                                            _contentRectangle.Height - _margin - _margin - (buttonHeight * 3 + _margin / 2 * 2) - _margin);

            if (_shortview)
            {
                _textRectangle.Height += (buttonHeight + _margin / 2);
            }

            int buttonXPos = _contentRectangle.Left + (_contentRectangle.Width - buttonWidth) / 2;
            int buttonYPos = _contentRectangle.Y + _margin + _textRectangle.Height + _margin;

            _textRectangle.Height -= _margin;

            buttonUpdate.Location = new Point(buttonXPos, buttonYPos);
            buttonUpdate.Size = new Size(buttonWidth, buttonHeight);

            if (!_shortview)
            {
                buttonYPos += buttonHeight + _margin / 2;

                buttonSkip.Location = new Point(buttonXPos, buttonYPos);
                buttonSkip.Size = new Size(buttonWidth, buttonHeight);

                buttonSkip.Visible = true;
            }
            else
            {
                buttonSkip.Location = new Point(buttonXPos, buttonYPos);
                buttonSkip.Size = new Size(1, 1);

                buttonSkip.Visible = false;
            }

            buttonYPos += buttonHeight + _margin / 2;

            buttonCancel.Location = new Point(buttonXPos, buttonYPos);
            buttonCancel.Size = new Size(buttonWidth, buttonHeight);

            if (_shortview)
            {
                buttonCancel.Text = Resources.UpdateInfoDialogControl_Buttons_Exit;
            }
            else
            {
                buttonCancel.Text = Resources.UpdateInfoDialogControl_Buttons_Cancel;
            }

            //Границы и фон
            _imgLT.Location = new Point(_contentRectangle.X, _contentRectangle.Y);
            _imgTT.Location = new Point(_imgLT.Right + 1, _contentRectangle.Y);
            _imgTT.Width = _contentRectangle.Width - _imgLT.Width - _imgRT.Width;
            _imgRT.Location = new Point(_imgTT.Right + 1, _contentRectangle.Y);

            _imgLL.Location = new Point(_contentRectangle.X, _imgLT.Bottom + 1);
            _imgLL.Height = _contentRectangle.Height - _imgLT.Height - _imgLB.Height;
            _imgRR.Location = new Point(_imgRT.Location.X + _imgRT.Size.Width - _imgRR.Size.Width, _imgRT.Bottom + 1);
            _imgRR.Height = _contentRectangle.Height - _imgRT.Height - _imgRB.Height;

            _imgLB.Location = new Point(_contentRectangle.X, _imgLL.Bottom + 1);
            _imgBB.Location = new Point(_imgLB.Right + 1, _imgLB.Top + 1);
            _imgBB.Width = _contentRectangle.Width - _imgLB.Width - _imgRB.Width;
            _imgRB.Location = new Point(_imgBB.Right + 1, _imgLB.Top);

            _backgroundRectangle = new Rectangle(_imgLL.Right + 1, _imgLL.Top, _imgRR.Left - _imgLL.Right - 1, _imgLL.Height + 1);
        }

        /// <summary>
        /// Инициализация картинок для рамки сообщения
        /// </summary>
        /// <param name="bb">Нижняя полоса</param>
        /// <param name="lb">Левый нижний угол</param>
        /// <param name="ll">Левая полоса</param>
        /// <param name="lt">Левый верхний угол</param>
        /// <param name="rb">Правый нижний угол</param>
        /// <param name="rr">Правая полоса</param>
        /// <param name="rt">Правый верхний угол</param>
        /// <param name="tt">Верхняя полоса</param>
        /// <param name="button">Кнопка</param>
        /// <param name="buttonPressed">Кнопка в нажатом состоянии</param>
        public static void InitImages(IImage bb, IImage lb, IImage ll, IImage lt, IImage rb, IImage rr, IImage rt, IImage tt,
                                      IImage button, IImage buttonPressed)
        {
            if (bb == null || lb == null || ll == null || lt == null || rb == null || rr == null || rt == null || tt == null ||
                button == null || buttonPressed == null)
                throw new ApplicationException("Message dialog images not initialized.");

            dlg_BB = bb;
            dlg_LB = lb;
            dlg_LL = ll;
            dlg_LT = lt;
            dlg_RB = rb;
            dlg_RR = rr;
            dlg_RT = rt;
            dlg_TT = tt;
            dlg_button = button;
            dlg_buttonPressed = buttonPressed;
        }

        private void initImages()
        {
            _imgBB = new GraphicsImage(dlg_BB, true);
            _imgLB = new GraphicsImage(dlg_LB, false);
            _imgLL = new GraphicsImage(dlg_LL, true);
            _imgLT = new GraphicsImage(dlg_LT, false);
            _imgRB = new GraphicsImage(dlg_RB, false);
            _imgRR = new GraphicsImage(dlg_RR, true);
            _imgRT = new GraphicsImage(dlg_RT, false);
            _imgTT = new GraphicsImage(dlg_TT, true);

            buttonUpdate.TransparentButton = dlg_button;
            buttonUpdate.TransparentButtonPressed = dlg_buttonPressed;
            buttonSkip.TransparentButton = dlg_button;
            buttonSkip.TransparentButtonPressed = dlg_buttonPressed;
            buttonCancel.TransparentButton = dlg_button;
            buttonCancel.TransparentButtonPressed = dlg_buttonPressed;

            Canvas.Children.Add(_imgLT);
            Canvas.Children.Add(_imgTT);
            Canvas.Children.Add(_imgRT);
            Canvas.Children.Add(_imgLL);
            Canvas.Children.Add(_imgRR);
            Canvas.Children.Add(_imgLB);
            Canvas.Children.Add(_imgBB);
            Canvas.Children.Add(_imgRB);
        }

        #endregion

        #region Actions

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            _dialogresult = _buttonUpdateDialogResult;
        }

        private void buttonSkip_Click(object sender, EventArgs e)
        {
            _dialogresult = _buttonSkipDialogResult;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            _dialogresult = _buttonCancelDialogResult;
        }

        #endregion

        #region Render

        private void CreateViewsScreenshot()
        {
            Bitmap screen = null;
            try
            {
                screen = NavigationService.Current().View.CreateScreenShot();
            }
            catch (OutOfMemoryException)
            {

            }

            BackgroundImage = screen;
        }

        protected override void OnRender(Gdi graphics, Rectangle clipRect)
        {
            IntPtr ptrSrc = OffscreenBuffer.OffScreenGraphics.GetHdc();

            using (Gdi gr = Gdi.FromHdc(ptrSrc, Rectangle.Empty))
            {
                DrawBackground(gr, clipRect);

                // fill background with double gradient
                var y = (int)(_backgroundRectangle.Height * _gradientCenter);

                gr.GradientFill(new Rectangle(_backgroundRectangle.Left, _backgroundRectangle.Top,
                                                    _backgroundRectangle.Width, y),
                                    _gradientColor1, _gradientColor2, FillDirection.TopToBottom);

                gr.GradientFill(new Rectangle(_backgroundRectangle.Left, _backgroundRectangle.Top + y,
                                                    _backgroundRectangle.Width, _backgroundRectangle.Height - y),
                                    _gradientColor2, _gradientColor3, FillDirection.TopToBottom);

                // Pass the graphics to the canvas to render
                if (Canvas != null)
                {
                    Canvas.Render(gr, clipRect);
                }

                //Draw Text
                SetFont(gr);

                gr.DrawText(_message,
                            new Win32.RECT(_textRectangle.X,
                                           _textRectangle.Y,
                                           _textRectangle.Width,
                                           _textRectangle.Height),
                            Win32.DT.LEFT | Win32.DT.TOP | Win32.DT.WORDBREAK | Win32.DT.NOCLIP);
            }

            graphics.BitBlt(clipRect.Left, clipRect.Top, Width, Height, ptrSrc, clipRect.Left, clipRect.Top, TernaryRasterOperations.SRCCOPY);

            OffscreenBuffer.OffScreenGraphics.ReleaseHdc(ptrSrc);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //
        }

        private void ReleaseResources()
        {
            if (BackgroundImage != null)
            {
                BackgroundImage.Dispose();
            }

            BackgroundImage = null;
        }

        #endregion

        #region External functions

        [DllImport("coredll.dll")]
        private static extern void MessageBeep(int flags);

        [DllImport("coredll.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin,
                                      uint wMsgFilterMax);

        [DllImport("coredll.dll")]
        static extern IntPtr DispatchMessage([In] ref MSG lpmsg);

        [DllImport("coredll.dll")]
        static extern bool TranslateMessage([In] ref MSG lpMsg);

        [StructLayout(LayoutKind.Sequential), CLSCompliant(false)]
        private struct MSG
        {
            public IntPtr HWnd;
            public uint Message;
            public IntPtr WParam;
            public IntPtr LParam;
            public uint Time;
            public Point Point;
        }

        #endregion
    }
}
