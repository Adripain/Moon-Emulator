using System;

namespace Moon.HabboHotel.Users.UserDataManagement
{
    public class UserDataNotFoundException : Exception
    {
        public UserDataNotFoundException(string reason)
            : base(reason)
        {
        }
    }
}