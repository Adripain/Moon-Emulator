using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.Communication.Packets.Outgoing.Catalog;

namespace Moon.Communication.Packets.Incoming.Catalog
{
    class GetGroupFurniConfigEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new GroupFurniConfigComposer(MoonEnvironment.GetGame().GetGroupManager().GetGroupsForUser(Session.GetHabbo().Id)));
        }
    }
}
