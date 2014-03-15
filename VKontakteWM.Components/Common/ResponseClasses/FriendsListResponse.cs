using System;

using System.Collections.Generic;
using System.Text;

namespace Galssoft.VKontakteWM.Components.Common.ResponseClasses
{
    public class FriendsListResponse
    {
        public DateTime LastUpdate;
        public List<User> Users = new List<User>();

        public bool IsUserInList(string uid)
        {            
            foreach (var val in Users)
            {
                if (val.Uid.Equals(uid))
                {
                    return true;
                }
            }

            return false;
        }

        public User GetUserByID(string uid)
        {
            foreach (var val in Users)
            {
                if (val.Uid.Equals(uid))
                {
                    return val;
                }
            }

            return null;
        }
    }
}
