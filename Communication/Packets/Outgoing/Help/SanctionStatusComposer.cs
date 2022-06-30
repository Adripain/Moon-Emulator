using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Help
{
    class SanctionStatusComposer : ServerPacket
    {
        public SanctionStatusComposer()
            : base(ServerPacketHeader.SanctionStatusMessageComposer)
        {
            base.WriteBoolean(true);
            base.WriteBoolean(false);
            base.WriteString("ALERT");
            base.WriteInteger(0);
            base.WriteInteger(30);
            base.WriteString("cfh.reason.EMPTY");
            base.WriteString("2016-07-17 16:33 (GMT +0000)");
            base.WriteInteger(720);
            base.WriteString("ALERT");
            base.WriteInteger(0);
            base.WriteInteger(30);
            base.WriteString("");
            base.WriteBoolean(false);
        }
    }
}