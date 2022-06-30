using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Games;
using Moon.Communication.Packets.Outgoing.GameCenter;

namespace Moon.Communication.Packets.Incoming.GameCenter
{
    class GetGameListingEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            ICollection<GameData> Games = MoonEnvironment.GetGame().GetGameDataManager().GameData;

            Session.SendMessage(new GameListComposer(Games));
        }
    }
}
