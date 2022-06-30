using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Moon.Communication.Packets.Outgoing.Moderation
{
    class BroadcastMessageAlertComposer : ServerPacket
    {
        public BroadcastMessageAlertComposer(string Message, string URL = "")
            : base(ServerPacketHeader.BroadcastMessageAlertMessageComposer)
        {
           base.WriteString(Message);
           base.WriteString(URL);
        }
    }
}

