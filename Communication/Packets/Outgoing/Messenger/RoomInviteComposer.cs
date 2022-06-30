﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Moon.Communication.Packets.Outgoing.Messenger
{
    class RoomInviteComposer : ServerPacket
    {
        public RoomInviteComposer(int SenderId, string Text)
            : base(ServerPacketHeader.RoomInviteMessageComposer)
        {
            base.WriteInteger(SenderId);
           base.WriteString(Text);
        }
    }
}
