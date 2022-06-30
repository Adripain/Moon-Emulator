using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Moon.Communication.Packets.Outgoing.Marketplace
{
    class MarketplaceMakeOfferResultComposer : ServerPacket
    {
        public MarketplaceMakeOfferResultComposer(int Success)
            : base(ServerPacketHeader.MarketplaceMakeOfferResultMessageComposer)
        {
            base.WriteInteger(Success);
        }
    }
}
