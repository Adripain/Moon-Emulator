using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Inventory.Furni
{
    class FurniListUpdateComposer : ServerPacket
    {
        public FurniListUpdateComposer()
            : base(ServerPacketHeader.FurniListUpdateMessageComposer)
        {

        }
    }
}
