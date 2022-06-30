using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.Communication.Packets.Outgoing.Help;

namespace Moon.Communication.Packets.Incoming.Help
{
    class SendBullyReportEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new SendBullyReportComposer());
        }
    }
}
