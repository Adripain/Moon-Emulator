using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Moon.Communication.Packets.Outgoing.Inventory.Trading
{
    class TradingFinishComposer : ServerPacket
    {
        public TradingFinishComposer()
            : base(ServerPacketHeader.TradingFinishMessageComposer)
        {
        }
    }
}
