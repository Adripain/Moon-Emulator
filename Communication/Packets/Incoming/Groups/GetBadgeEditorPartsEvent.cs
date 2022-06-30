using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.Communication.Packets.Outgoing.Groups;

namespace Moon.Communication.Packets.Incoming.Groups
{
    class GetBadgeEditorPartsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new BadgeEditorPartsComposer(
                MoonEnvironment.GetGame().GetGroupManager().Bases,
                MoonEnvironment.GetGame().GetGroupManager().Symbols,
                MoonEnvironment.GetGame().GetGroupManager().BaseColours,
                MoonEnvironment.GetGame().GetGroupManager().SymbolColours,
                MoonEnvironment.GetGame().GetGroupManager().BackGroundColours));

        }
    }
}
