using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Users;
using Moon.HabboHotel.Users.Messenger;
using Moon.HabboHotel.Cache;

namespace Moon.Communication.Packets.Outgoing.Messenger
{
    class BuddyRequestsComposer : ServerPacket
    {
        public BuddyRequestsComposer(ICollection<MessengerRequest> Requests)
            : base(ServerPacketHeader.BuddyRequestsMessageComposer)
        {
            base.WriteInteger(Requests.Count);
            base.WriteInteger(Requests.Count);

            foreach (MessengerRequest Request in Requests)
            {
                base.WriteInteger(Request.From);
               base.WriteString(Request.Username);

                UserCache User = MoonEnvironment.GetGame().GetCacheManager().GenerateUser(Request.From);
               base.WriteString(User != null ? User.Look : "");
            }
        }
    }
}
