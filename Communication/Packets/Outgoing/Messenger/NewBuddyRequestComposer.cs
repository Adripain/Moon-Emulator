using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Users;
using Moon.HabboHotel.Cache;

namespace Moon.Communication.Packets.Outgoing.Messenger
{
    class NewBuddyRequestComposer : ServerPacket
    {
        public NewBuddyRequestComposer(UserCache Habbo)
            : base(ServerPacketHeader.NewBuddyRequestMessageComposer)
        {
           base.WriteInteger(Habbo.Id);
           base.WriteString(Habbo.Username);
           base.WriteString(Habbo.Look);
        }
    }
}
