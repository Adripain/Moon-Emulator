using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Items;

namespace Moon.Communication.Packets.Outgoing.Rooms.Engine
{
    class ObjectRemoveComposer : ServerPacket
    {
        public ObjectRemoveComposer(Item Item, int UserId)
            : base(ServerPacketHeader.ObjectRemoveMessageComposer)
        {
           base.WriteString(Item.Id.ToString());
            base.WriteBoolean(false);
            base.WriteInteger(UserId);
            base.WriteInteger(0);
        }
    }
}