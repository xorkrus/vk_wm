/*	 This file was created by GALS Soft	company
 *	 http://www.galssoft.com
 *	 
 *	 Project name is Odnoklassniki Mobile
 *	 Platform is .Net Compact Framework 2.0 for Windows Mobile 5.0+ 
 * 
 *	 Copyright (c) 2009-2010 GALS Soft
 */

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.ComponentModel;
using System.Text;
using System.Diagnostics;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;

namespace Galssoft.VKontakteWM.Components.UI.WebBrowser
{
    public partial class WebBrowserEx : UserControl
    {
        #region Fields

        private IntPtr htmlViewHandle = IntPtr.Zero;
        private IntPtr oldWndProc = IntPtr.Zero;
        private WndProcDelegate newWndProc = null;
        private IntPtr hwnd = IntPtr.Zero;

        private Uri _url = null;

        #endregion

        #region Constructors

        public WebBrowserEx()
        {
            InitializeComponent();
            WebBrowserAPI.InitializeHtmlView();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                base.Dispose(disposing);
            }
            catch (ObjectDisposedException)
            { }
        }

        #endregion

        #region Events

        public event WebBrowserEventHandler BeforeNavigate;
        public event WebBrowserEventHandler NavigateComplete;

        /// <summary>
        /// Событие срабатывает на окончание загрузки всего документа.
        /// </summary>
        public event WebBrowserEventHandler DocumentComplete;
        public event EventHandler<WebBrowserNavigatingExEventArgs> Navigating;

        /// <summary>
        /// Метод для альтернативного закрытия окна. либо подменяет картинки из браузера на пустые
        /// либо говорит оставшимся о невозможности загрузки
        /// </summary>
        public void DoSomeThing()
        {
            //Stop();

            //return;
            //foreach (UInt32 inlineImage in _inlineImages)
            //    unsafe
            //    {
            //        IntPtr pObj = Marshal.AllocHGlobal(sizeof(WebBrowserAPI.INLINEIMAGEINFO));

            //        try
            //        {
            //            WebBrowserAPI.INLINEIMAGEINFO imInfo = new WebBrowserAPI.INLINEIMAGEINFO();
            //            imInfo.hbm = IntPtr.Zero;
            //            imInfo.bOwnBitmap = false;
            //            imInfo.iOrigWidth = 0;
            //            imInfo.iOrigHeight = 0;
            //            imInfo.dwCookie = inlineImage;

            //            Marshal.StructureToPtr(imInfo, pObj, false);

            //            WebBrowserAPI.SendMessage(htmlViewHandle, WebBrowserAPI.DTM_SETIMAGE, 0,  (int) pObj);
            //        }
            //        finally
            //        {
            //            // Free the unmanaged memory.
            //            //Marshal.FreeHGlobal(pObj);
            //        }
            //    }

            //foreach (UInt32 inlineImage in _inlineImages)
            //    WebBrowserAPI.SendMessage(htmlViewHandle, WebBrowserAPI.DTM_IMAGEFAIL, 0, (int)inlineImage);

        }

        #endregion

        #region Properties

        /// <summary>
        /// Special params which must be added to the url
        /// </summary>
        public string SpecialUrlParams { get; set; }

        [DefaultValue(true)]
        public bool ShowWaitCursorWhileLoading { get; set; }

        public Uri Url
        {
            get { return _url; }
            set { Navigate(value.ToString()); }
        }

        #endregion

        #region Methods

        public void Navigate(string url)
        {
            Stop();

            if (url == null || url.Trim().Length == 0)
                return;

            //if (!String.IsNullOrEmpty(SpecialUrlParams))
            //{
            //    if (url.IndexOf('?') > 0)
            //        url += "&" + SpecialUrlParams;
            //    else
            //        url += "?" + SpecialUrlParams;
            //}

            _url = new Uri(url);
            WebBrowserAPI.SendMessage(htmlViewHandle, WebBrowserAPI.DTM_NAVIGATE, WebBrowserAPI.NAVIGATEFLAG_NOCACHE, url);
        }

        public int ZoomLevel
        {
            set
            {
                int level = value;
                if (level < 0)
                {
                    level = 0;
                }
                else if (level > 4)
                {
                    level = 4;
                }
                WebBrowserAPI.SendMessage(htmlViewHandle, WebBrowserAPI.DTM_ZOOMLEVEL, 0, level);
            }
        }

        public int ScrollPosition
        {
            get
            {
                WebBrowserAPI.SCROLLINFO lpScrollInfo = new WebBrowserAPI.SCROLLINFO();
                lpScrollInfo.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lpScrollInfo);
                lpScrollInfo.fMask = WebBrowserAPI.SIF_ALL;
                WebBrowserAPI.GetScrollInfo(htmlViewHandle, WebBrowserAPI.SB_VERT, ref lpScrollInfo);
                return lpScrollInfo.nPos;
            }
            set
            {
                WebBrowserAPI.SCROLLINFO lpScrollInfo = new WebBrowserAPI.SCROLLINFO();
                lpScrollInfo.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lpScrollInfo);
                lpScrollInfo.fMask = WebBrowserAPI.SIF_POS;
                lpScrollInfo.nPos = value;
                WebBrowserAPI.SetScrollInfo(htmlViewHandle, WebBrowserAPI.SB_VERT, ref lpScrollInfo, true);
            }
        }

        public void SetScrollEnd()
        {
            WebBrowserAPI.SendMessage(htmlViewHandle, WebBrowserAPI.WM_VSCROLL, WebBrowserAPI.SB_BOTTOM, 0);
        }

        public void SetScrolPosition(int value)
        {
            WebBrowserAPI.SCROLLINFO lpScrollInfo = new WebBrowserAPI.SCROLLINFO();
            lpScrollInfo.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lpScrollInfo);
            lpScrollInfo.fMask = WebBrowserAPI.SIF_POS;
            lpScrollInfo.nPos = value;
            WebBrowserAPI.SetScrollInfo(htmlViewHandle, WebBrowserAPI.SB_VERT, ref lpScrollInfo, true);

            Rectangle update = new Rectangle();
            WebBrowserAPI.GetClientRect(htmlViewHandle, ref update);
            WebBrowserAPI.ScrollWindowEx(htmlViewHandle, 0, -(value), 0, 0, IntPtr.Zero, update, WebBrowserAPI.SW_ERASE | WebBrowserAPI.SW_INVALIDATE);
        }

        public int GetScrollMax
        {
            get
            {
                WebBrowserAPI.SCROLLINFO lpScrollInfo = new WebBrowserAPI.SCROLLINFO();
                lpScrollInfo.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lpScrollInfo);
                lpScrollInfo.fMask = WebBrowserAPI.SIF_ALL;
                WebBrowserAPI.GetScrollInfo(htmlViewHandle, WebBrowserAPI.SB_VERT, ref lpScrollInfo);
                return lpScrollInfo.nMax;
            }
        }

        public void SetScrollAnchor(string anchor)
        {
            WebBrowserAPI.SendMessage(htmlViewHandle, WebBrowserAPI.DTM_ANCHORW, 0, anchor);
        }

        public void Stop()
        {
            if (htmlViewHandle != IntPtr.Zero)
                WebBrowserAPI.SendMessage(htmlViewHandle, WebBrowserAPI.DTM_STOP, 0, 0);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            var style = (int)(WebBrowserAPI.WS_VISIBLE | WebBrowserAPI.WS_TABSTOP | WebBrowserAPI.WS_CHILD | WebBrowserAPI.HS_NOSELECTION);
            htmlViewHandle = WebBrowserAPI.CreateWindowEx(0, "DISPLAYCLASS", null, style, 0, 0, Width, Height, Handle,
                                                  IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            //WmApi.SendMessage(htmlViewHandle, WmApi.DTM_ENABLECONTEXTMENU, 0, 1);

            hwnd = Handle;
            newWndProc = new WndProcDelegate(NewWndProc);
            oldWndProc = WebBrowserAPI.GetWindowLong(hwnd, WebBrowserAPI.GWL_WNDPROC);
            int success = WebBrowserAPI.SetWindowLong(hwnd, WebBrowserAPI.GWL_WNDPROC, newWndProc);
        }

        private IntPtr NewWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case WebBrowserAPI.WM_NOTIFY:
                    WebBrowserAPI.NmHdr n = WebBrowserAPI.NmHdr.GetStruct(lParam);
                    switch (n.code)
                    {
                        case WebBrowserAPI.NM_INLINE_IMAGE:
                            OnInlineObject(WebBrowserAPI.NM_INLINE_IMAGE, WebBrowserAPI.HtmlViewMessage.GetStruct(lParam));
                            break;
                        case WebBrowserAPI.NM_BEFORENAVIGATE:
                            OnBeforeNavigate(WebBrowserAPI.HtmlViewMessage.GetStruct(lParam));
                            break;
                        case WebBrowserAPI.NM_NAVIGATECOMPLETE:
                            OnNavigateComplete(WebBrowserAPI.HtmlViewMessage.GetStruct(lParam));
                            break;
                        case WebBrowserAPI.NM_DOCUMENTCOMPLETE:
                            OnDocumentComplete(WebBrowserAPI.HtmlViewMessage.GetStruct(lParam));
                            break;
                        //case WebBrowserAPI.NM_CONTEXTMENU:
                        //    WebBrowserAPI.HtmlContextMessage hcm = WebBrowserAPI.HtmlContextMessage.GetStruct(lParam);
                        //    string lnk = hcm.linkHREF;
                        //    string tit = hcm.linkName;
                        //    System.Drawing.Point pt = hcm.pt;
                        //    OnContextMenu(hcm);
                        //    break;
                        case WebBrowserAPI.NM_HOTSPOT:
                            if (OnHotSpotClicked(WebBrowserAPI.HtmlViewMessage.GetStruct(lParam)))
                            {
                                // return not zero value to not let pass message to the parent control 
                                return new IntPtr(1);
                            }
                            break;
                    }
                    break;
            }

            return WebBrowserAPI.CallWindowProc(oldWndProc, hWnd, msg, wParam, lParam);
        }

        private void OnInlineObject(int notifyCode, WebBrowserAPI.HtmlViewMessage htmlViewMessage)
        {
            if (ShowWaitCursorWhileLoading)
                Cursor.Current = Cursors.WaitCursor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns>true if message was processed</returns>
        private bool OnHotSpotClicked(WebBrowserAPI.HtmlViewMessage msg)
        {
            Stop();

            if (msg.target == null)
                return true;

            try
            {
                string escapedUrl = Uri.EscapeUriString(PrepareUrl(UnicodeToUtf8(msg.target)));
                string target = null;

                int idig = escapedUrl.IndexOf('#');
                if (idig > -1)
                {
                    target = escapedUrl.Substring(idig, escapedUrl.Length - idig);
                    escapedUrl = escapedUrl.Substring(0, idig);
                }

                Uri uri = null;
                try
                {
                    uri = new Uri(escapedUrl);
                }
                catch (UriFormatException)
                { }

                if (uri == null || String.IsNullOrEmpty(uri.Host))
                {
                    try
                    {
                        uri = new Uri(_url, escapedUrl);
                    }
                    catch (UriFormatException)
                    { }
                }

                string postdata = null;
                if (msg.data != null && msg.data.Length > 0)
                    postdata = UnicodeToUtf8(msg.data);
                WebBrowserNavigatingExEventArgs e = new WebBrowserNavigatingExEventArgs(uri, target, postdata);
                if (Navigating != null)
                {
                    Navigating(this, e);

                    if (!e.Cancel)
                        return false;
                }
            }
            catch (UriFormatException)
            {
                return true;
            }
            catch (Exception)
            {
                return true;
            }
            return true;
        }

        //private void OnContextMenu(WebBrowserAPI.HtmlContextMessage message)
        //{
        //    string lnk = message.linkHREF;
        //    string tit = message.linkName;
        //}

        private void OnBeforeNavigate(WebBrowserAPI.HtmlViewMessage message)
        {
            if (ShowWaitCursorWhileLoading)
                Cursor.Current = Cursors.WaitCursor;

            if (BeforeNavigate != null)
            {
                BeforeNavigate(message);
            }
        }

        private static string PrepareUrl(string url)
        {
            int posEmptyByte = url.IndexOf("\0");
            return posEmptyByte != -1
                       ? url.Substring(0, posEmptyByte)
                       : url;
        }

        private void OnNavigateComplete(WebBrowserAPI.HtmlViewMessage message)
        {
            //string url = UnicodeToUtf8(message.target);
            //if (url.Substring(0, 5) == "http:")
            //{
            //    Stop();
            //    Navigate(_url.ToString());
            //    return;
            //}

            if (NavigateComplete != null)
            {
                NavigateComplete(message);
            }
        }

        private string UnicodeToUtf8(string textUnicode)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            UnicodeEncoding unicode = new UnicodeEncoding();

            byte[] buff = unicode.GetBytes(textUnicode);
            string s = utf8.GetString(buff, 0, buff.Length);
            //(new .UTF8Encoding()).GetString((new System.Text.UnicodeEncoding()).GetBytes(message.target), 0, 10)
            return s;
        }

        private void OnDocumentComplete(WebBrowserAPI.HtmlViewMessage message)
        {
            Debug.WriteLine("WebBrowser.OnDocumentComplete");
            if (ShowWaitCursorWhileLoading)
                Cursor.Current = Cursors.Default;

            if (DocumentComplete != null)
            {
                DocumentComplete(message);

                //Из за увеличения размеров при VGA в браузере вручную ставим в 0
                //if(ListViewSettings.Current.ScreenDPI >= 192)
                //    ZoomLevel = 0;
            }
        }

        //private void KillWindow(string className, string title)
        //{
        //    IntPtr handle = Win32.FindWindow(className, title);
        //    Win32.SendMessage(handle, Win32.WM_DESTROY, 0, 0);
        //}

        private Timer _oncloseTimer = null;

        protected override void OnHandleDestroyed(EventArgs e)
        {
            Stop();

            base.OnHandleDestroyed(e);

            if (oldWndProc != IntPtr.Zero && hwnd != IntPtr.Zero)
                WebBrowserAPI.SetWindowLong(hwnd, WebBrowserAPI.GWL_WNDPROC, oldWndProc);

            Marshal.Release(htmlViewHandle);
            if (htmlViewHandle != IntPtr.Zero)
            {
                if (WebBrowserAPI.DestroyWindow(htmlViewHandle))
                {
                    htmlViewHandle = IntPtr.Zero;
                    GC.Collect();
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Win32.SetWindowPos(htmlViewHandle, IntPtr.Zero, this.Left, this.Top, this.Width, this.Height, 0);
        }

        #endregion

    }
}
