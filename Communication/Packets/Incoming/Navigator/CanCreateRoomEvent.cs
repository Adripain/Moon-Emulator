using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.Communication.Packets.Outgoing.Navigator;

namespace Moon.Communication.Packets.Incoming.Navigator
{
    class CanCreateRoomEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new CanCreateRoomComposer(false, 150));
        }
    }
}
