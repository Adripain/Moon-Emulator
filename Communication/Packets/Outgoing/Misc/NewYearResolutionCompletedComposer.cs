using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Handshake
{
    class NewYearResolutionCompletedComposer : ServerPacket
    {
        public NewYearResolutionCompletedComposer(string badge)
            : base(ServerPacketHeader.NewYearResolutionCompletedComposer)
        {
            base.WriteString(badge);
            base.WriteString(badge);
        }
    }
}

