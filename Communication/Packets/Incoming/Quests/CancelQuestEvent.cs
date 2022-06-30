using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Moon.Communication.Packets.Incoming.Quests
{
    class CancelQuestEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            MoonEnvironment.GetGame().GetQuestManager().CancelQuest(Session, Packet);
        }
    }
}
