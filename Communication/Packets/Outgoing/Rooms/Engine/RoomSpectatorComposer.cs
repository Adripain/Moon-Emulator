using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Items;

namespace Moon.Communication.Packets.Outgoing.Rooms.Engine
{
    class RoomSpectatorComposer : ServerPacket
    {
        public RoomSpectatorComposer()
            : base(ServerPacketHeader.RoomSpectatorComposer)
        {
        }
    }
}
