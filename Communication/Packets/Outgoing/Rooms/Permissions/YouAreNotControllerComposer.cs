using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Rooms.Permissions
{
    class YouAreNotControllerComposer : ServerPacket
    {
        public YouAreNotControllerComposer()
            : base(ServerPacketHeader.YouAreNotControllerMessageComposer)
        {
        }
    }
}
