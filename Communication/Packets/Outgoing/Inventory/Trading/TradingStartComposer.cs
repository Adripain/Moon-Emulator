﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Inventory.Trading
{
    class TradingStartComposer : ServerPacket
    {
        public TradingStartComposer(int User1Id, int User2Id)
            : base(ServerPacketHeader.TradingStartMessageComposer)
        {
            base.WriteInteger(User1Id);
            base.WriteInteger(1);
            base.WriteInteger(User2Id);
            base.WriteInteger(1);
        }
    }
}
