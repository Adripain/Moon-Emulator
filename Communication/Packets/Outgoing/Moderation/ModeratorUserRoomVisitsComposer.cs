using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.Utilities;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Users;

namespace Moon.Communication.Packets.Outgoing.Moderation
{
    class ModeratorUserRoomVisitsComposer : ServerPacket
    {
        public ModeratorUserRoomVisitsComposer(Habbo Data, Dictionary<double, RoomData> Visits)
            : base(ServerPacketHeader.ModeratorUserRoomVisitsMessageComposer)
        {
            base.WriteInteger(Data.Id);
           base.WriteString(Data.Username);
            base.WriteInteger(Visits.Count);

            foreach (KeyValuePair<double, RoomData> Visit in Visits)
            {
                base.WriteInteger(Visit.Value.Id);
               base.WriteString(Visit.Value.Name);
                base.WriteInteger(UnixTimestamp.FromUnixTimestamp(Visit.Key).Hour);
                base.WriteInteger(UnixTimestamp.FromUnixTimestamp(Visit.Key).Minute);
            }
        }
    }
}
