using System;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.Server;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Components.UI.Transitions;
using Galssoft.VKontakteWM.Components.UI.Wrappers;
using Galssoft.VKontakteWM.CustomControlls;
using Microsoft.WindowsCE.Forms;
using Galssoft.VKontakteWM.Properties;

namespace Galssoft.VKontakteWM.Forms
{
    public partial class ChangeStatusView : UIViewBase, IView
    {
        public ChangeStatusView()
        {
            InitializeComponent();
        }

        public void Load()
        {
            //
        }

        public override bool HaveWinFormsControls
        {
            get
            {
                return true;
            }
        }

        #region IView Members

        public string Title
        {
            get { return Resources.ChangeStatusView_Title; }
        }

        public MainMenu ViewMenu
        {
            get { return mainMenu; }
        }

        public void OnBeforeActivate()
        {
            tbxStatusText.Enabled = true;
            tbxStatusText.Text = string.Empty;
        }

        public void OnAfterDeactivate()
        {
            //
        }

        public void OnAfterActivate()
        {
            (new InputPanel()).Enabled = true;
        }

        public Bitmap CreateScreenShot()
        {
            Bitmap bitmap = new Bitmap(Width, Height);
            Rectangle rect = new Rectangle(0, 0, Width, Height);

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

        protected override void OnUpdateView(string key)
        {            
            if (key == "SetStatusFail")
            {
                string changeStatusError = (string)ViewData["SetStatusError"];

                DialogControl.ShowQuery(changeStatusError, DialogButtons.OK);                
            }

            if (key == "SetStatusSuccess")
            {
                DialogControl.ShowQuery(Resources.ChangeStatusView_SetStatusSuccess, DialogButtons.OK);
            }         
   
            if (key == "HideCursor")
            {
                tbxStatusText.Enabled = false;
            }

            if (key == "GoToLogin")
            {
                OnViewStateChanged("GoGoToLogin");
            }
        }

        #region кнопочки

        void ButtonClearTextClick(object sender, EventArgs e)
        {
            tbxStatusText.Text = string.Empty;
        }
        
        void MenuItemCancelClick(object sender, EventArgs e)
        {
            OnViewStateChanged("Cancel");
        }

        void MenuItemChangeClick(object sender, EventArgs e)
        {
            ViewData["CurrentStatus"] = ParsingHelper.GetActualString(tbxStatusText.Text.Trim());
            OnViewStateChanged("SetStatus");            
        }

        #endregion

        #region Контекстное меню

        void ContextMenuPopup(object sender, EventArgs e)
        {
            miPaste.Enabled = Clipboard.GetDataObject().GetDataPresent(DataFormats.Text);
            miCopy.Enabled = miCut.Enabled = tbxStatusText.SelectedText != string.Empty;
        }

        void MiCopyClick(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(tbxStatusText.SelectedText);
        }

        void MiPasteClick(object sender, EventArgs e)
        {
            int startPosition = tbxStatusText.SelectionStart;
            var forPaste = (string)Clipboard.GetDataObject().GetData(DataFormats.Text);
            tbxStatusText.Text = tbxStatusText.Text.Substring(0, startPosition) + forPaste +
                                  tbxStatusText.Text.Substring(startPosition, tbxStatusText.Text.Length - startPosition);
            tbxStatusText.SelectionStart = startPosition + forPaste.Length;
        }

        void MiCutClick(object sender, EventArgs e)
        {
            int startPosition = tbxStatusText.SelectionStart;
            Clipboard.SetDataObject(tbxStatusText.SelectedText);
            tbxStatusText.Text = tbxStatusText.Text.Remove(tbxStatusText.SelectionStart, tbxStatusText.SelectionLength);
            tbxStatusText.SelectionStart = startPosition;
        }

        #endregion

    }
}

