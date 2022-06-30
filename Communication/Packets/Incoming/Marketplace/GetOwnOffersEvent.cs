using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.Communication.Packets.Outgoing.Marketplace;

namespace Moon.Communication.Packets.Incoming.Marketplace
{
    class GetOwnOffersEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new MarketPlaceOwnOffersComposer(Session.GetHabbo().Id));
        }
    }
}
