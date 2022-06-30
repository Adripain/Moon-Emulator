using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Inventory.Trading
{
    class TradingErrorComposer : ServerPacket
    {
        public TradingErrorComposer(int Error, string Username)
            : base(ServerPacketHeader.TradingErrorMessageComposer)
        {
            base.WriteInteger(Error);
           base.WriteString(Username);
        }
    }
}
