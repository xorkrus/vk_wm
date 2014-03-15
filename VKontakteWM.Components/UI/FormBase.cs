/*	 This file was created by GALS Soft	company
 *	 http://www.galssoft.com
 *	 
 *	 Project name is Odnoklassniki Mobile
 *	 Platform is .Net Compact Framework 2.0 for Windows Mobile 5.0+ 
 * 
 *	 Copyright (c) 2009-2010 GALS Soft
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using Galssoft.VKontakteWM.Components.Configuration;
using Galssoft.VKontakteWM.Components.UI.Transitions;
using Microsoft.WindowsCE.Forms;

namespace Galssoft.VKontakteWM.Components.UI
{
    public class FormBase : Form
    {
        #region Constants


        #endregion

        #region Fields

        private bool _correctClose = false;
        private string _storedText;
        protected bool UserClose = false;
        protected Form ReturnForm = null;
        private Timer timer;

        private TransitionControl _transitionControl;
        protected InputPanel InputPanel;

        #endregion

        #region Constructors

        public FormBase() : this(null)
        {
        }

        public FormBase(Form form)
            //: base()
        {
            ReturnForm = form;
           
            InputPanel = new InputPanel();
            InputPanel.EnabledChanged += new EventHandler(InputPanel_EnabledChanged2);
            
            timer = new Timer();
            timer.Interval = 1;
            timer.Tick += new System.EventHandler(this.timer_Tick);
        }

        #endregion

        #region Properties

        public new DialogResult DialogResult
        {
            get { return base.DialogResult; }
            set
            {
                _correctClose = true;
                base.DialogResult = value;
            }
        }

        /// <summary>
        /// Stored caption for the windows before hide
        /// </summary>
        public string StoredText
        {
            get { return _storedText ?? Text; }
            set { _storedText = value; }
        }

        /// <summary>
        /// Установка в true вызывает сокрытие формы
        /// </summary>
        public bool NeedHide
        {
            get
            {
                return timer.Enabled;
            }
            set
            {
                timer.Enabled = value;
            }
        }

        /// <summary>
        /// Windows' caption
        /// </summary>
        public override string Text
        {
            get { return base.Text; }
            set { SetTextInvoke(value); }
        }

        internal TransitionControl TransitionControl
        {
            get { return _transitionControl; }
        }

        #endregion

        #region Methods

        protected virtual void BuildControls()
        {
            _transitionControl = new TransitionControl();
            _transitionControl.Dock = DockStyle.Fill;
            _transitionControl.Location = new Point(0, 0);
            _transitionControl.Name = "Transition";
            _transitionControl.Size = ClientSize;
            _transitionControl.TabIndex = 0;
            _transitionControl.Visible = false;

            Controls.Add(_transitionControl);
        }

        /// <summary>
        /// Close the form
        /// </summary>
        public new void Close()
        {
            Application.DoEvents();
            _correctClose = true;
            base.Close();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (InputPanel != null))
            {
                InputPanel.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Use for thread safe update property Text
        /// </summary>
        /// <param name="text">New caption</param>
        private void SetTextInvoke(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(SetTextInvoke), text);
            }
            else
                base.Text = text;
        }

        private void InputPanel_EnabledChanged2(object sender, EventArgs e)
        {
            HideInputPanel();
        }

        /// <summary>
        /// Сокрытие окна в Form.OnLoad невозможно, поэтому оно происходит по таймеру с минимальной задержкой
        /// </summary>
        private void timer_Tick(object sender, EventArgs e)
        {
            NeedHide = false;
            Hide();
        }

        //TODO: write a description	and how to use
        private void InputPaneltypeChanged()
        {
            //TODO: всегда ли высота панели задач совпадает с высотой главного меню?
            Height = InputPanel.VisibleDesktop.Height;

            //// TODO: Хак для перерисовки кинетического списка при закрытии SIP
            //if (this is InboxDialogForm && (this as InboxDialogForm).dialog != null)
            //    (this as InboxDialogForm).dialog.Refresh();
            //if (this is TagForm && (this as TagForm).tags != null)
            //    (this as TagForm).tags.Refresh();
            //if (this is FriendForm && (this as FriendForm).friends != null)
            //    (this as FriendForm).friends.Refresh();

            Refresh();
        }

        //TODO: write a description	and how to use
        private void HideInputPanel()
        {
            //if (InputPanel.Enabled)
            //{
            //    FormBorderStyle = FormBorderStyle.None;
            //    Height = InputPanel.VisibleDesktop.Height;

            //    // Так как свойства ActiveControl в CF нет
            //    if (Visible)
            //    {
            //        Control activeControl = GetFocusedControl(this);
            //        if (activeControl != null)
            //        {
            //            var newPosition = new Point(0, GetAbsoluteTop(activeControl, 0));

            //            if (this is NewMessageForm)
            //                // TODO: Хак для случая, когда скролл не у формы, а у таба -
            //                // некрасиво, но быстрее чем проходить по всей иерархии и 
            //                // опрашивать у кого есть автоскролл.
            //                (this as NewMessageForm).SelectedPanel.AutoScrollPosition = newPosition;
            //            else
            //            {
            //                AutoScrollPosition = newPosition;
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    //TODO: всегда ли высота панели задач совпадает с высотой главного меню?
            //    Height = InputPanel.VisibleDesktop.Height - Screen.PrimaryScreen.WorkingArea.Y;
            //    //Refresh();
            //}

            ////// TODO: Хак для перерисовки кинетического списка при открытии и закрытии SIP
            //if (this is InboxDialogForm && (this as InboxDialogForm).dialog != null)
            //    (this as InboxDialogForm).dialog.Refresh();
            //if (this is TagForm && (this as TagForm).tags != null)
            //    (this as TagForm).tags.Refresh();
            //if (this is FriendForm && (this as FriendForm).friends != null)
            //    (this as FriendForm).friends.Refresh();
            //if (this is CheckListForm)
            //     (this as CheckListForm).RefreshList();
        }

        private Control GetFocusedControl(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (!c.Visible)
                    continue;

                if (c.Focused)
                {
                    return c;
                }
                else
                {
                    Control c1 = GetFocusedControl(c);
                    if (c1 != null)
                        return c1;
                }
            }

            return null;
        }

        //TODO: write a description	and how to use
        private int GetAbsoluteTop(Control c, int currentTop)
        {
            if (c.Parent == null)
                return currentTop;
            else
                return GetAbsoluteTop(c.Parent, currentTop + c.Top);
        }

        protected void InitializeComponent()
        {
            var resources = new ComponentResourceManager(typeof (FormBase));
            SuspendLayout();
            // 
            // FormBase
            // 
            AutoScaleMode = AutoScaleMode.Inherit;
            //Была ошибка изменения размеров экрана. Необходимо убирать размеры в ресурсах
            resources.ApplyResources(this, "$this");
            Name = "FormBase";
            Deactivate += new EventHandler(FormBase_Deactivate);
            Width = Screen.PrimaryScreen.WorkingArea.Width;
            ResumeLayout(false);
        }

        private void FormBase_Deactivate(object sender, EventArgs e)
        {
            PerformDeactivate();
        }

        protected void PerformDeactivate()
        {
            //FormBorderStyle = FormBorderStyle.FixedSingle;
            InputPanel.Enabled = false;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!_correctClose)
            {
                e.Cancel = true;
#if DEBUG
                DebugHelper.WriteLogEntry(String.Format("Try to close form {0} '{1}' was declined", Name, Text));
#endif
                return;
            }

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            CheckRestoreForm();
            ReturnForm = null;
        }

        private void CheckRestoreForm()
        {
            if (!UserClose)
            {
#if DEBUG
                DebugHelper.WriteLogEntry(String.Format("Form {0} is closed. Show RestoreForm '{1}'", Name, ReturnForm != null ? ReturnForm.Name : "<null>"));
#endif
                if (ReturnForm != null)
                    ReturnForm.Show();
            }
        }

        //protected void Minimize()
        //{
        //  Galssoft.LiveJournalWM.Components.Helpers.CoreHelper.ShowWindow(this.Handle,
        //    Galssoft.LiveJournalWM.Components.Helpers.CoreHelper.SW_MINIMIZED);
        //}

        protected override void OnActivated(EventArgs e)
        {
            RestoreText();
            base.OnActivated(e);
        }

        /// <summary>
        /// Restore windows' caption after Show
        /// </summary>
        public void RestoreText()
        {
            Text = StoredText;
            StoredText = null;
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            StoredText = Text;
            Text = SystemConfiguration.ApplicationName;
        }

        //protected void SetReturnFormTextToStoredText()
        //{
        //    var fb = ReturnForm as FormBase;
        //    if (fb != null)
        //    {
        //        fb.Text = !string.IsNullOrEmpty(fb.StoredText) ? fb.StoredText : ApplicationName;
        //    }
        //}

        //protected void SetReturnFormTextEmpty()
        //{
        //    if (ReturnForm != null)
        //        ReturnForm.Text = string.Empty;
        //}

        protected void ActivateForm()
        {
            if (!Focused)
                Activate();
            BringToFront();
        }

        protected override void OnLoad(EventArgs e)
        {
            //HideInputPanel(); // Первоначальная установка высоты формы
            base.OnLoad(e);
        }

        #endregion

    }
}