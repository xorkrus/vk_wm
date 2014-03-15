using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Galssoft.VKontakteWM.Components.Common.ResponseClasses
{
    [XmlRoot(ElementName = "users_getInfo_response", Namespace = "http://api.forticom.com/1.0/")]
    public class UsersGetInfoResponse
    {
        public List<User> users = new List<User>();
    }

    public class User
    {

        public string Uid { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public DateTime Birthday { get; set; }

        public string Photo100px { get; set; }

        public string Photo200px { get; set; }        
       
        public string Status { get; set; }

        public string Sex { get; set; }

        public string Town { get; set; }

        public string MobilePhone { get; set; }

        public string IsOnline { get; set; }

        public User()
        {
            Uid = FirstName = LastName = FullName = Photo100px = Photo200px = Status = Sex = Town = MobilePhone = IsOnline = string.Empty;
            Birthday = new DateTime(0);
        }
    }

    public class Location
    {
        public string City { get; set; }

        public string Country { get; set; }
        


        //мое

        //public string city { get; set; }

        //public string country { get; set; }
    }
}
