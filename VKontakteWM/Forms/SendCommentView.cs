using System;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.Server;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.Transitions;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Properties;
using Microsoft.WindowsCE.Forms;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;

namespace Galssoft.VKontakteWM.Forms
{
    public partial class SendCommentView : UIViewBase, IView
    {
        public SendCommentView()
        {
            InitializeComponent();
        }

        public void Load()
        {

        }

        #region IView Members

        public string Title
        {
            get { return Resources.SendCommentTitle; }
        }

        public MainMenu ViewMenu
        {
            get { return mainMenu; }
        }

        public void OnBeforeActivate()
        {
            tbxComment.Visible = true;
        }

        public void OnAfterDeactivate()
        {
            (new InputPanel()).Enabled = false;
        }

        public void OnAfterActivate()
        {
            (new InputPanel()).Enabled = true;
        }

        public Bitmap CreateScreenShot()
        {
            var bitmap = new Bitmap(Width, Height);
            var rect = new Rectangle(0, 0, Width, Height);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                IntPtr gMemPtr = g.GetHdc();

                using (Gdi gMem = Gdi.FromHdc(gMemPtr, Rectangle.Empty))
                {
                    DrawBackground(gMem, rect);

                    if (Canvas != null)
                    {
                        Canvas.Render(gMem, rect);
                    }

                    foreach (Control control in Controls)
                    {
                        if (control is KineticControlBase)
                        {
                            ((KineticControlBase)control).DrawRender(gMem);
                        }
                    }
                }

                g.ReleaseHdc(gMemPtr);
            }

            return bitmap;
        }

        public TransitionType GetTransition(IView from)
        {
            return TransitionType.Basic;
        }

        #endregion

        #region Обработчики нажатий на кнопки формы

        #endregion

        protected override void OnUpdateView(string key)
        {
            #region ClearTextBox

            if (key == "ClearTextBox")
            {
                tbxComment.Text = string.Empty;
            }

            #endregion

            #region SendCommentSuccess

            if (key == "SendCommentSuccess")
            {
                DialogControl.ShowQuery(Resources.NewCommentUploaded, Components.UI.Controls.DialogButtons.OK);
            }

            #endregion

            #region SendCommentFail

            if (key == "SendCommentFail")
            {
                DialogControl.ShowQuery((string) ViewData["SendCommentError"], Components.UI.Controls.DialogButtons.OK);
                (new InputPanel()).Enabled = true;
            }

            #endregion

            (new InputPanel()).Enabled = true;

            if (key == "GoToLogin")
            {
                OnViewStateChanged("GoGoToLogin");
            }
        }

        void MenuItemBackClick(object sender, EventArgs e)
        {
            OnViewStateChanged("GoBack");
        }

        void MenuItemSendClick(object sender, EventArgs e)
        {
            if (tbxComment.Text.Trim() == string.Empty)
            {
                DialogControl.ShowQuery(Resources.SendComment_DontEmpty,
                                        Components.UI.Controls.DialogButtons.OK);
                (new InputPanel()).Enabled = true;
            }
            else
            {
                tbxComment.Visible = false;
                ViewData["Message"] = ParsingHelper.GetActualString(tbxComment.Text.Trim());
                OnViewStateChanged("SendMessage");
                tbxComment.Visible = true;
            }
        }

        void BtnClearClick(object sender, EventArgs e)
        {
            tbxComment.Text = string.Empty;
        }

        #region Контекстное меню

        void ContextMenuPopup(object sender, EventArgs e)
        {
            miPaste.Enabled = Clipboard.GetDataObject().GetDataPresent(DataFormats.Text);
            miCopy.Enabled = miCut.Enabled = tbxComment.SelectedText != string.Empty;
        }

        void MiCopyClick(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(tbxComment.SelectedText);
        }

        void MiPasteClick(object sender, EventArgs e)
        {
            int startPosition = tbxComment.SelectionStart;
            var forPaste = (string)Clipboard.GetDataObject().GetData(DataFormats.Text);
            tbxComment.Text = tbxComment.Text.Substring(0, startPosition) + forPaste +
                                  tbxComment.Text.Substring(startPosition, tbxComment.Text.Length - startPosition);
            tbxComment.SelectionStart = startPosition + forPaste.Length;
        }

        void MiCutClick(object sender, EventArgs e)
        {
            int startPosition = tbxComment.SelectionStart;
            Clipboard.SetDataObject(tbxComment.SelectedText);
            tbxComment.Text = tbxComment.Text.Remove(tbxComment.SelectionStart, tbxComment.SelectionLength);
            tbxComment.SelectionStart = startPosition;
        }

        #endregion

    }
}
