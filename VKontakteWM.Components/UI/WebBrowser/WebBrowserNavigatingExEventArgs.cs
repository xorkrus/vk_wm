using System;

using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Galssoft.VKontakteWM.Components.UI.WebBrowser
{
    public class WebBrowserNavigatingExEventArgs : WebBrowserNavigatingEventArgs
    {
        public WebBrowserNavigatingExEventArgs(Uri url, string targetFrameName, string postdata)
            : base(url, targetFrameName)
        {
            Data = postdata;
        }

        public string Data { get; set; }
    }
}
