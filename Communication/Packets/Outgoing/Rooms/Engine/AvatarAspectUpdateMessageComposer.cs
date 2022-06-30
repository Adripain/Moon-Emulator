using System;
using System.Linq;
using System.Text;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.GameClients;

namespace Moon.Communication.Packets.Outgoing.Rooms.Engine
{
    class AvatarAspectUpdateMessageComposer : ServerPacket
    {
        public AvatarAspectUpdateMessageComposer(string Figure, string Gender)
            : base(ServerPacketHeader.AvatarAspectUpdateMessageComposer)
        {
            base.WriteString(Figure);
            base.WriteString(Gender);

        }
    }
}