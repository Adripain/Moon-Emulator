using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Rooms.Session
{
    class RoomReadyComposer : ServerPacket
    {
        public RoomReadyComposer(int RoomId, string Model)
            : base(ServerPacketHeader.RoomReadyMessageComposer)
        {
           base.WriteString(Model);
            base.WriteInteger(RoomId);
        }
    }
}
