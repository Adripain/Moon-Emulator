using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Moon.Communication.Packets.Outgoing.Moderation
{
    class OpenHelpToolComposer : ServerPacket
    {
        public OpenHelpToolComposer()
            : base(ServerPacketHeader.OpenHelpToolMessageComposer)
        {
            base.WriteInteger(0);
        }
    }
}
