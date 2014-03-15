using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Galssoft.VKontakteWM.Components.ResponseClasses
{
    public class AuthLoginResponse
    {
        public string uid { get; set; }

        public string session_key { get; set; }

        public string auth_token { get; set; }

        public AuthLoginResponse()
        {
            uid = string.Empty;

            session_key = string.Empty;

            auth_token = string.Empty;
        }
    }
}
