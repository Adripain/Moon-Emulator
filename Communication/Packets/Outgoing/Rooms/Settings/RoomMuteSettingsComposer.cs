﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Moon.Communication.Packets.Outgoing.Rooms.Settings
{
    class RoomMuteSettingsComposer : ServerPacket
    {
        public RoomMuteSettingsComposer(bool Status)
            : base(ServerPacketHeader.RoomMuteSettingsMessageComposer)
        {
            base.WriteBoolean(Status);
        }
    }
}