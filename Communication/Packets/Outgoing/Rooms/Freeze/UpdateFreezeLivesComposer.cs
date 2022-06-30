using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Moon.Communication.Packets.Outgoing.Rooms.Freeze
{
    class UpdateFreezeLivesComposer : ServerPacket
    {
        public UpdateFreezeLivesComposer(int UserId, int FreezeLives)
            : base(ServerPacketHeader.UpdateFreezeLivesMessageComposer)
        {
            base.WriteInteger(UserId);
            base.WriteInteger(FreezeLives);
        }
    }
}
