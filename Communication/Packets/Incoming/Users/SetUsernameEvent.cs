using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Moon.Communication.Packets.Incoming.Users
{
    class SetUsernameEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            string Username = Packet.PopString();
        }
    }
}
