using System;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.Common.ResponseClasses;
using Galssoft.VKontakteWM.Components.MVC;
using Galssoft.VKontakteWM.Components.Server;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.Components.UI.Controls;
using Galssoft.VKontakteWM.Components.UI.Transitions;
using Galssoft.VKontakteWM.Components.UI.Wrappers;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Properties;
using Microsoft.WindowsCE.Forms;
using Galssoft.VKontakteWM.Components.UI.CompoundControls;
using Galssoft.VKontakteWM.Components.GDI;
using Galssoft.VKontakteWM.Components.Common.Configuration;

namespace Galssoft.VKontakteWM.Forms
{
    public partial class MessageSendView : UIViewBase, IView
    {
        public MessageSendView()
        {
            InitializeComponent();
        }

        public void Load()
        {
            //
        }

        #region IView Members

        public string Title
        {
            get { return Resources.MessageSend_Title; }
        }

        public MainMenu ViewMenu
        {
            get { return mainMenu; }
        }

        public void OnBeforeActivate()
        {
            tbxMessageData.Visible = true;
            OnViewStateChanged("Activate");
            (new InputPanel()).Enabled = true;
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
            #region ActivateResponse

            if (key == "ActivateResponse")
            {
                headerText.Text = headerShadowText.Text = (string)ViewData["UserName"];

                tbxMessageData.Text = (string)ViewData["MessageDraftInput"];

                if (!string.IsNullOrEmpty((string)ViewData["MsgForChangedReceiver"]))
                {
                    tbxMessageData.Text = (string) ViewData["MsgForChangedReceiver"];
                }

                if ((bool)ViewData["AFRHideButton"])
                {
                    if (this.Canvas.Children.Contains(friendsLogo))
                    {
                        this.friendsLogo.Location = new Point(0, 0);

                        this.Canvas.Children.Remove(friendsLogo);
                    }
                }
                else
                {
                    if (!this.Canvas.Children.Contains(friendsLogo))
                    {
                        this.friendsLogo.Location = new Point(UISettings.CalcPix(210), UISettings.CalcPix(4));

                        this.Canvas.Children.Add(friendsLogo);
                    }
                }                
            }

            #endregion

            #region SentMessageResponse

            if (key == "SentMessageResponse")
            {
                DialogControl.ShowQuery((string)ViewData["SentMessageResponseMessage"], DialogButtons.OK);

                ViewData["SentMessageResponseMessage"] = string.Empty;

                if (Convert.ToBoolean((string)ViewData["MessageIsSent"]))
                {
                    //NavigationService.GoBack();
                    OnViewStateChanged("SendComplete");
                }
            }

            #endregion

            if (key == "GoToLogin")
            {
                OnViewStateChanged("GoGoToLogin");
            }
        }

        private void FriendsLogoClick(object sender, EventArgs e)
        {
            ViewData["MsgForChangedReceiver"] = tbxMessageData.Text;
            OnViewStateChanged("ChangeReceiver");            
        }

        private void ButtonSendClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbxMessageData.Text.Trim()))
            {
                tbxMessageData.Visible = false;
                ViewData["MessageDraftOutput"] = ParsingHelper.GetActualString(tbxMessageData.Text.Trim());
                OnViewStateChanged("SentMessage");
                tbxMessageData.Visible = true;
            }
            else
            {
                DialogControl.ShowQuery(Resources.MessagesList_View_MessageIsEmpty, DialogButtons.OK);
                (new InputPanel()).Enabled = true;
            }
        }

        private void ButtonCancelClick(object sender, EventArgs e)
        {
            ViewData["MessageDraftOutput"] = tbxMessageData.Text;

            using (new WaitWrapper(false))
            {
                (new InputPanel()).Enabled = false;
                OnViewStateChanged("Back");
            }
        }

        private void MessageSendViewGotFocus(object sender, EventArgs e)
        {
            (new InputPanel()).Enabled = true;
        }

        #region Контекстное меню

        void ContextMenuPopup(object sender, EventArgs e)
        {
            miPaste.Enabled = Clipboard.GetDataObject().GetDataPresent(DataFormats.Text);
            miCopy.Enabled = miCut.Enabled = tbxMessageData.SelectedText != string.Empty;
        }

        void MiCopyClick(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(tbxMessageData.SelectedText);
        }

        void MiPasteClick(object sender, EventArgs e)
        {
            int startPosition = tbxMessageData.SelectionStart;
            var forPaste = (string) Clipboard.GetDataObject().GetData(DataFormats.Text);
            tbxMessageData.Text = tbxMessageData.Text.Substring(0, startPosition) + forPaste +
                                  tbxMessageData.Text.Substring(startPosition, tbxMessageData.Text.Length - startPosition);
            tbxMessageData.SelectionStart = startPosition + forPaste.Length;
        }

        void MiCutClick(object sender, EventArgs e)
        {
            int startPosition = tbxMessageData.SelectionStart;
            Clipboard.SetDataObject(tbxMessageData.SelectedText);
            tbxMessageData.Text = tbxMessageData.Text.Remove(tbxMessageData.SelectionStart, tbxMessageData.SelectionLength);
            tbxMessageData.SelectionStart = startPosition;
        }

        #endregion
    }
}
