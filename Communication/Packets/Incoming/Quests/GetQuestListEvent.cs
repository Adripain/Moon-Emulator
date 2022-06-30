using System.Collections.Generic;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Quests;
using Moon.Communication.Packets.Incoming;

namespace Moon.Communication.Packets.Incoming.Quests
{
    public class GetQuestListEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            MoonEnvironment.GetGame().GetQuestManager().GetList(Session, null);
        }
    }
}