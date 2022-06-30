using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Moon.Communication.Packets.Outgoing.GameCenter
{
    class GameUnknownComposer2 : ServerPacket
    {
        public GameUnknownComposer2()
            : base(ServerPacketHeader.GameUnknownComposer1)
        {
        }
    }
}
