using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moon.HabboHotel.GameClients;

namespace Moon.Communication.Packets.Outgoing.Rooms.Session
{
    class CloseConnectionComposer : ServerPacket
    {
        public CloseConnectionComposer(GameClient Session)
            : base(ServerPacketHeader.CloseConnectionMessageComposer)
        {
            Session.GetHabbo().IsTeleporting = false;
            Session.GetHabbo().TeleportingRoomID = 0;
            Session.GetHabbo().TeleporterId = 0;
            Session.GetHabbo().CurrentRoomId = 0;
        }

        public CloseConnectionComposer()
            : base(ServerPacketHeader.CloseConnectionMessageComposer)
        {

        }
    }
}
