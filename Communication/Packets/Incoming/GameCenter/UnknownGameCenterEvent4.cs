using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Games;
using Moon.Communication.Packets.Outgoing.GameCenter;
using System.Data;

using Moon.HabboHotel.Users;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.Communication.Packets.Incoming.GameCenter
{
    class UnknownGameCenterEvent4 : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int pop = Packet.PopInt();
        }
    }
}
