using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.HabboHotel.Items.Crafting;

namespace Moon.Communication.Packets.Outgoing.Rooms.Furni
{
    class MysticBoxCloseComposer : ServerPacket
    {
        public MysticBoxCloseComposer()
            : base(ServerPacketHeader.MysticBoxCloseComposer)
        {
        }
    }
}