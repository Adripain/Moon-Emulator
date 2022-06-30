using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Help;

namespace Moon.Communication.Packets.Incoming.Help
{
    class GetSanctionStatusEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new SanctionStatusComposer());
        }
    }
}
