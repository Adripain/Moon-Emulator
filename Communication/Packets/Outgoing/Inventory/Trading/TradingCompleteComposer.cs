using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Inventory.Trading
{
    class TradingCompleteComposer : ServerPacket
    {
        public TradingCompleteComposer()
            : base(ServerPacketHeader.TradingCompleteMessageComposer)
        {
        }
    }
}
