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
using Microsoft.WindowsCE.Forms;

namespace Galssoft.VKontakteWM.CustomControlls
{
    /// <summary>
    /// Класс формы диалога
    /// </summary>
    public partial class DialogControl : UIViewBase
    {
        #region Members

        private static Form _masterForm;
        private static int _visibleDialogsCounter = 0;

        private DialogResult _dialogresult;
        private Rectangle _contentRectangle;
        private Rectangle _textRectangle;
        private Rectangle _backgroundRectangle;
        private int _margin;
        private string _message;
        private bool _initialized;
        private MainMenu _menu;
        private Color _gradientColor1 = Color.White;
        private Color _gradientColor2 = Color.White;
        private Color _gradientColor3 = Color.White;
        private float _gradientCenter = (float)1 / 3;
        private DialogResult _button1DialogResult;
        private DialogResult _button2DialogResult;
        private DialogResult _button3DialogResult;

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

        //Показать простое сообщение с кнопкой OK
        public static DialogResult ShowMessage(string message)
        {
            return ShowQuery(message, DialogButtons.OK);
        }

        //Показать сообщение с кнопками
        public static DialogResult ShowQuery(string message, DialogButtons buttons)
        {
            DebugHelper.WriteTraceEntry("----------");
            DebugHelper.WriteTraceEntry("ShowQuery called");

            (new InputPanel()).Enabled = false;
            //using (new WaitWrapper(true))
            {
                DebugHelper.WriteTraceEntry("ShowQuery beginning");
                if (_masterForm == null)
                    throw new ApplicationException("DialogControl not initialized.");

                if (_masterForm.InvokeRequired)
                {
                    return
                        (DialogResult)
                        _masterForm.Invoke((showMessageDelegate)showMessage, new object[] { message, buttons });
                }
                else
                {
                    return showMessage(message, buttons);
                }
            }
        }

        private delegate DialogResult showMessageDelegate(string message, DialogButtons buttons);
        private static DialogResult showMessage(string message, DialogButtons buttons)
        {
            DebugHelper.WriteTraceEntry("DialogControl showMessage invoked");
            DialogControl control = new DialogControl();
            control._dialogresult = DialogResult.None;
            AddControlToForm(control);
            DebugHelper.WriteTraceEntry("DialogControl control to form added");
            control.Show(message, buttons);

            DebugHelper.WriteTraceEntry("DialogControl resume layout");
            DebugHelper.WriteTraceEntry("DialogControl MSG cycle beginning");
            DebugHelper.FlushTraceBuffer();

            MSG msg = new MSG();
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
        private static void AddControlToForm(DialogControl control)
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
        private static void RemoveControlFromForm(DialogControl control)
        {
            control.Hide();
            _masterForm.Controls.Remove(control);
            _masterForm.Menu = control._menu;
            _visibleDialogsCounter--;
        }

        private DialogControl()
        {
            InitializeComponent();
            _margin = UISettings.CalcPix(20);
        }

        /// <summary>
        /// Показать сообщение с текстом и кнопками
        /// </summary>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        private void Show(string message, DialogButtons buttons)
        {
            DebugHelper.WriteTraceEntry("Menu Hiding");
            /*
            button1.Size = new Size(UISettings.CalcPix(button1.Width), UISettings.CalcPix(button1.Height));
            button2.Size = new Size(UISettings.CalcPix(button2.Width), UISettings.CalcPix(button2.Height));
            button3.Size = new Size(UISettings.CalcPix(button3.Width), UISettings.CalcPix(button3.Height));
            */
            if (((UIViewBase)ViewManager.CurrentView).HaveWinFormsControls)
                CreateScreenshot();
            else
                PrepareBackground();
            _menu = _masterForm.Menu;
            _masterForm.Menu = null;
            DebugHelper.WriteTraceEntry("Menu Hide");
            DebugHelper.WriteTraceEntry("Call overloaded Show()");
            //PrepareBackground();
            DebugHelper.WriteTraceEntry("Background prepared");

            DebugHelper.WriteTraceEntry("Canvas pre suspended");
            Canvas.SuspendLayout();
            switch (buttons)
            {
                case DialogButtons.OK:
                    button1.Visible = true;
                    button2.Visible = false;
                    button3.Visible = false;
                    button1.Text = "OK";
                    _button1DialogResult = DialogResult.OK;
                    break;
                case DialogButtons.YesNo:
                    button1.Visible = true;
                    button2.Visible = true;
                    button3.Visible = false;
                    button1.Text = Resources.Yes;
                    button2.Text = Resources.No;
                    _button1DialogResult = DialogResult.Yes;
                    _button2DialogResult = DialogResult.No;
                    break;
            }
            DebugHelper.WriteTraceEntry("Canvas pre resumed");
            //this.Canvas.RecalcDPIScaling();
            Canvas.ResumeLayout();
            DebugHelper.WriteTraceEntry("Canvas resumed");

            _message = message;
            initImages();
            CalcContentRectangle();
            //button1.Location = new Point((ClientRectangle.Width - button1.Width)/2, ClientRectangle.Height/10*7);

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

        // При нашей прорисовке Windows.Forms.Control не рисуется. Поэтому при первом показе 
        // делается скриншот экрана что бы были видны вс контролы
        private void CreateScreenshot()
        {
            if (BackgroundImage != null)
            {
                BackgroundImage.Dispose();
            }
            BackgroundImage = Gdi.CreateSnapshot(60, _masterForm.Location);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            //Скрытие клавиатуры
            (new InputPanel()).Enabled = false;

            if (BackgroundImage != null && Width == BackgroundImage.Width && Height == BackgroundImage.Height)
                return;

            if (_initialized)
                CalcContentRectangle();
            if (_initialized)
                PrepareBackground();
        }

        private void SetFont(Gdi g)
        {
            g.Font = FontCache.CreateFont("Tahoma", 14, FontStyle.Regular, true);
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
                    Size textSize = g.GetDrawTextSize(_message, w * 6 - _margin * 2, Win32.DT.CENTER | Win32.DT.WORDBREAK);
                    textHeight = textSize.Height;
                }

            int h = textHeight + _margin * 3 + button1.Height;
            if (h > Height / 8 * 6)
                h = Height / 8 * 6;

            int contentWidth = w * 6;
            int contentHeight = h;
            int vPos = (Height - contentHeight) / 2;
            int hPos = w;

            _contentRectangle = new Rectangle(hPos, vPos, contentWidth, contentHeight);
            _textRectangle = new Rectangle(_contentRectangle.X + _margin, _contentRectangle.Y + _margin,
                                           _contentRectangle.Width - _margin * 2,
                                           _contentRectangle.Height - _margin * 3 - button1.Height);

            if (button3.Visible)
            {

            }
            else if (button2.Visible)
            {
                int workWidth = _contentRectangle.Width - _margin * 3;
                int left = (workWidth / 2 - button1.Width) / 2;
                left = _margin + left;
                button1.Location = new Point(left + _contentRectangle.Left,
                                             _textRectangle.Y + _textRectangle.Height + _margin);
                left = left + workWidth / 2 + _margin;
                button2.Location = new Point(left + _contentRectangle.Left,
                                             _textRectangle.Y + _textRectangle.Height + _margin);
            }
            else
            {
                button1.Location = new Point((ClientRectangle.Width - button1.Width) / 2,
                                             _textRectangle.Y + _textRectangle.Height + _margin);
            }

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

            _backgroundRectangle = new Rectangle(_imgLL.Right + 1, _imgLL.Top, _imgRR.Left - _imgLL.Right - 1,
                                                 _imgLL.Height + 1);
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

            button1.TransparentButton = dlg_button;
            button1.TransparentButtonPressed = dlg_buttonPressed;
            button1.FontColor = Color.FromArgb(145, 145, 145);
            button1.PressedFontColor = Color.FromArgb(255, 255, 255);
            button1.Font = FontCache.CreateFont("Tahoma", 12, FontStyle.Regular, true);

            button2.TransparentButton = dlg_button;
            button2.TransparentButtonPressed = dlg_buttonPressed;
            button2.FontColor = Color.FromArgb(145, 145, 145);
            button2.PressedFontColor = Color.FromArgb(255, 255, 255);
            button2.Font = FontCache.CreateFont("Tahoma", 12, FontStyle.Regular, true);

            button3.TransparentButton = dlg_button;
            button3.TransparentButtonPressed = dlg_buttonPressed;
            button3.FontColor = Color.FromArgb(145, 145, 145);
            button3.PressedFontColor = Color.FromArgb(255, 255, 255);
            button3.Font = FontCache.CreateFont("Tahoma", 12, FontStyle.Regular, true);

            //button1.Size = new Size(UISettings.CalcPix(button1.Width), UISettings.CalcPix(button1.Height));
            //button2.Size = new Size(UISettings.CalcPix(button2.Width), UISettings.CalcPix(button2.Height));
            //button3.Size = new Size(UISettings.CalcPix(button3.Width), UISettings.CalcPix(button3.Height));

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

        private void button1_Click(object sender, EventArgs e)
        {
            _dialogresult = _button1DialogResult;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _dialogresult = _button2DialogResult;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _dialogresult = _button3DialogResult;
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