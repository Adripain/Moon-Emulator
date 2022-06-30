﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Moon.Communication.Packets.Outgoing.Groups
{
    class SetGroupIdComposer : ServerPacket
    {
        public SetGroupIdComposer(int Id)
            : base(ServerPacketHeader.SetGroupIdMessageComposer)
        {
            base.WriteInteger(Id);
        }
    }
}
