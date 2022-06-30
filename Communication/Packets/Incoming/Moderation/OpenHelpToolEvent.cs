using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.Communication.Packets.Incoming;
using Moon.Communication.Packets.Outgoing.Moderation;

namespace Moon.Communication.Packets.Incoming.Moderation
{
    class OpenHelpToolEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new OpenHelpToolComposer());
        }
    }
}
