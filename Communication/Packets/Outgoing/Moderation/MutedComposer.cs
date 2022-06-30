using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Moon.Communication.Packets.Outgoing.Moderation
{
    class MutedComposer : ServerPacket
    {
        public MutedComposer(Double TimeMuted)
            : base(ServerPacketHeader.MutedMessageComposer)
        {
            base.WriteInteger(Convert.ToInt32(TimeMuted));
        }
    }
}
