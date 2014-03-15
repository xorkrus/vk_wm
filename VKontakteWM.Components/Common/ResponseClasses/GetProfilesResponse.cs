using System;

using System.Collections.Generic;
using System.Text;

//using System.Xml.Serialization;

namespace Galssoft.VKontakteWM.Components.Common.ResponseClasses
{
    public class GetProfilesResponse
    {
        public List<UserVK> users = new List<UserVK>();
    }

    public class UserVK
    {
        public string uid { get; set; }

        public string first_name { get; set; }

        public string last_name { get; set; }


        public string sex { get; set; }

        public string bdate { get; set; }        

        public string foto { get; set; }

        public string mob_phone { get; set; }

        public string status { get; set; }

        public string town { get; set; }
    }
}
