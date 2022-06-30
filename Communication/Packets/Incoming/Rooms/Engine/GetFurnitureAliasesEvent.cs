using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Moon.Communication.Packets.Incoming;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Rooms.Engine;

namespace Moon.Communication.Packets.Incoming.Rooms.Engine
{
    class GetFurnitureAliasesEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new FurnitureAliasesComposer());
        }
    }
}
