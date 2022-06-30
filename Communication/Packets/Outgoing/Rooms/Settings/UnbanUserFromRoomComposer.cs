using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Rooms.Settings
{
    class UnbanUserFromRoomComposer : ServerPacket
    {
        public UnbanUserFromRoomComposer(int RoomId, int UserId)
            : base(ServerPacketHeader.UnbanUserFromRoomMessageComposer)
        {
            base.WriteInteger(RoomId);
            base.WriteInteger(UserId);
        }
    }
}
