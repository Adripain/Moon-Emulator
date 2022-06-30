using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Users;
using Moon.HabboHotel.Users.Messenger;
using Moon.HabboHotel.Users.Relationships;

namespace Moon.Communication.Packets.Outgoing.Messenger
{
    class MessengerInitComposer : ServerPacket
    {
        public MessengerInitComposer()
            : base(ServerPacketHeader.MessengerInitMessageComposer)
        {
            base.WriteInteger(MoonStaticGameSettings.MessengerFriendLimit);//Friends max.
            base.WriteInteger(300);
            base.WriteInteger(800);
            base.WriteInteger(1); // category count
            base.WriteInteger(1);//category id
            base.WriteString("Grupos");//category name
        }
    }
}
