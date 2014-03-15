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
using Microsoft.WindowsCE.Forms;

namespace Galssoft.VKontakteWM.CustomControlls
{
    /// <summary>
    /// Класс формы диалога
    /// </summary>
    public partial class StatusDialogControl : UIViewBase
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
        private static string _messageNew;
        private bool _initialized;
        private MainMenu _menu;
        private readonly Color _gradientColor1 = Color.FromArgb(229, 235, 250);
        private readonly Color _gradientColor2 = Color.White;
        private readonly Color _gradientColor3 = Color.FromArgb(216, 222, 239);
        private const float _gradientCenter = (float)1 / 3;
        private DialogResult _buttonOkDialogResult;
        private DialogResult _buttonResetDialogResult;
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

        //Показать сообщение
        /*
        public static DialogResult ShowMessage(string message)
        {
            //return ShowQuery(message);
        }
        */

        //Показать сообщение с кнопками
        public static DialogResult ShowQuery(string status, out string message)
        {
            DebugHelper.WriteTraceEntry("----------");
            DebugHelper.WriteTraceEntry("ShowQuery called");
            (new InputPanel()).Enabled = true;

            //using (new WaitWrapper(true))
            {
                DebugHelper.WriteTraceEntry("ShowQuery beginning");
                if (_masterForm == null)
                    throw new ApplicationException("StatusDialogControl not initialized.");

                /*
                if (_masterForm.InvokeRequired)
                {
                    return
                        (DialogResult)
                        _masterForm.Invoke((showMessageDelegate)showMessage);
                }
                */
                return showMessage(status, out message);
            }
        }

        //private delegate DialogResult showMessageDelegate(out string message);
        private static DialogResult showMessage(string status, out string message)
        {
            DebugHelper.WriteTraceEntry("StatusDialogControl showMessage invoked");
            var control = new StatusDialogControl { _dialogresult = DialogResult.None };
            AddControlToForm(control);
            DebugHelper.WriteTraceEntry("StatusDialogControl control to form added");
            control.Show(status);

            DebugHelper.WriteTraceEntry("StatusDialogControl resume layout");
            DebugHelper.WriteTraceEntry("StatusDialogControl MSG cycle beginning");
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
            message = _messageNew;
            return res;
        }

        //Добавить контрол на форму
        private static void AddControlToForm(StatusDialogControl control)
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
        private static void RemoveControlFromForm(StatusDialogControl control)
        {
            control.Hide();
            _masterForm.Controls.Remove(control);
            _masterForm.Menu = control._menu;
            _visibleDialogsCounter--;
        }

        private StatusDialogControl()
        {
            InitializeComponent();
            _margin = UISettings.CalcPix(10);
        }

        /// <summary>
        /// Показать сообщение с текстом и кнопками
        /// </summary>
        /// <param name="status"></param>
        private void Show(string status)
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

            textBoxStatus.Text = status;
            _buttonOkDialogResult = DialogResult.Yes;
            _buttonResetDialogResult = DialogResult.No;
            _buttonCancelDialogResult = DialogResult.Cancel;

            DebugHelper.WriteTraceEntry("Canvas pre resumed");
            Canvas.ResumeLayout();
            DebugHelper.WriteTraceEntry("Canvas resumed");

            _message = status;

            initImages();
            CalcContentRectangle();

            MessageBeep(32);
            DebugHelper.WriteTraceEntry("Start before Show()");
            Show();
        }

        private void PrepareBackground()
        {
            if (BackgroundImage != null)
                BackgroundImage.Dispose();

            CreateViewsScreenshot();
            Gdi.AdjustBrightness(BackgroundImage, 80);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (BackgroundImage != null && Width == BackgroundImage.Width && Height == BackgroundImage.Height)
                return;

            if (_initialized)
                CalcContentRectangle();
            if (_initialized)
                PrepareBackground();
        }

        private void SetFont(Gdi g)
        {
            g.Font = FontCache.CreateFont("Calibri", 13, FontStyle.Regular, true);
            g.TextAlign = Win32.TextAlign.TA_LEFT;
            g.TextColor = Color.Black;
        }

        private void CalcContentRectangle()
        {
            int w = Width / 8;

            int textHeight = 0;
            if (!string.IsNullOrEmpty(_message))
                using (Gdi g = new Gdi(this))
                {
                    SetFont(g);
                    Size textSize = g.GetDrawTextSize(_message, w * 6 - _margin * 2, Win32.DT.WORDBREAK);
                    textHeight = textSize.Height;
                }

            int h = textBoxStatus.Height + /*buttonOk.Height*/30 * 3 + _margin * 10;
            if (h > Height / 8 * 6)
                h = Height / 8 * 6;

            int contentWidth = w * 6;
            int contentHeight = h;
            int vPos = (Height - contentHeight) / 2;
            int hPos = w;

            _contentRectangle = new Rectangle(hPos, vPos, contentWidth, contentHeight);
            /*
            _textRectangle = new Rectangle(_contentRectangle.X + _margin, _contentRectangle.Y + _margin,
                                           _contentRectangle.Width - _margin * 2,
                                           _contentRectangle.Height - _margin * 3 - buttonOk.Height);
            */


            int workWidth = _contentRectangle.Width - _margin * 3;
            int left = hPos + _margin;

            textBoxStatus.Location = new Point(left + _contentRectangle.Left,
                                         _contentRectangle.Y + _margin * 3);

            textBoxStatus.Size = new Size(workWidth - _margin * 4, 30);

            buttonOk.Location = new Point(left + _contentRectangle.Left,
                                         textBoxStatus.Location.Y + textBoxStatus.Height + _margin);

            buttonOk.Size = new Size(workWidth - _margin * 4, 30);

            buttonReset.Location = new Point(left + _contentRectangle.Left,
                                         buttonOk.Location.Y + buttonOk.Height + _margin);

            buttonReset.Size = new Size(workWidth - _margin * 4, 30);

            buttonCancel.Location = new Point(left + _contentRectangle.Left,
                                         buttonReset.Location.Y + buttonReset.Height + _margin);

            buttonCancel.Size = new Size(workWidth - _margin * 4, 30);


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
            _imgBB.Location = new Point(_imgLB.Right + 1, _imgLB.Top);
            _imgBB.Width = _contentRectangle.Width - _imgLB.Width - _imgRB.Width;
            _imgRB.Location = new Point(_imgBB.Right + 1, _imgLB.Top);

            _backgroundRectangle = new Rectangle(_imgLL.Right + 1, _imgLL.Top, _imgRR.Left - _imgLL.Right - 1,
                                                 _imgLL.Height);
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

            buttonOk.TransparentButton = dlg_button;
            buttonOk.TransparentButtonPressed = dlg_buttonPressed;
            buttonReset.TransparentButton = dlg_button;
            buttonReset.TransparentButtonPressed = dlg_buttonPressed;
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

        private void buttonOk_Click(object sender, EventArgs e)
        {
            _dialogresult = _buttonOkDialogResult;
            _messageNew = textBoxStatus.Text;
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            _dialogresult = _buttonResetDialogResult;
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
                            Win32.DT.LEFT | Win32.DT.TOP | Win32.DT.WORDBREAK);
            }

            graphics.BitBlt(clipRect.Left, clipRect.Top, Width, Height, ptrSrc, clipRect.Left, clipRect.Top, TernaryRasterOperations.SRCCOPY);

            OffscreenBuffer.OffScreenGraphics.ReleaseHdc(ptrSrc);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {

        }

        private void ReleaseResources()
        {
            if (BackgroundImage != null)
                BackgroundImage.Dispose();
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
