using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Help.Helpers
{
    class GuardianHandleReportComposer : ServerPacket
    {
        public GuardianHandleReportComposer(int seconds)
            : base(ServerPacketHeader.GuardianHandleReportMessageComposer)
        {
            base.WriteInteger(seconds);
        }
    }
}
