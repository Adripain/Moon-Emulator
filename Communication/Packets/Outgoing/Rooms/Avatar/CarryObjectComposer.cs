using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Rooms.Avatar
{
    class CarryObjectComposer : ServerPacket
    {
        public CarryObjectComposer(int virtualID, int itemID)
            : base(ServerPacketHeader.CarryObjectMessageComposer)
        {
            base.WriteInteger(virtualID);
            base.WriteInteger(itemID);
        }
    }
}
