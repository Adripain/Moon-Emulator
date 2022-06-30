using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Rooms.Session
{
    class OpenConnectionComposer : ServerPacket
    {
        public OpenConnectionComposer()
            : base(ServerPacketHeader.OpenConnectionMessageComposer)
        {

        }
    }
}
