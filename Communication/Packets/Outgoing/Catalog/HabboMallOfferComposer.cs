﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Handshake
{
    class HabboMallOfferComposer : ServerPacket
    {
        public HabboMallOfferComposer()
            : base(ServerPacketHeader.HabboMallOfferComposer)
        {
            base.WriteString("Test");
            base.WriteString("imagen");
        }
    }
}