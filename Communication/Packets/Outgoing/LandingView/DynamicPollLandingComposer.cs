using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.LandingView
{
    class DynamicPollLandingComposer : ServerPacket
    {
        public DynamicPollLandingComposer(bool HasDone)
            : base(ServerPacketHeader.DynamicPollLandingComposer)
        {
            base.WriteBoolean(HasDone);
        }
    }
}
