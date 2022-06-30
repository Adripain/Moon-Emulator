﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;

namespace Moon.Communication.Packets.Outgoing.Moderation
{
    class ModeratorRoomInfoComposer : ServerPacket
    {
        public ModeratorRoomInfoComposer(RoomData Data, bool OwnerInRoom)
            : base(ServerPacketHeader.ModeratorRoomInfoMessageComposer)
        {
            base.WriteInteger(Data.Id);
            base.WriteInteger(Data.UsersNow);
            base.WriteBoolean(OwnerInRoom); // owner in room
            base.WriteInteger(Data.OwnerId);
            base.WriteString(Data.OwnerName);
            base.WriteBoolean(Data != null);
            base.WriteString(Data.Name);
            base.WriteString(Data.Description);
           
            base.WriteInteger(Data.Tags.Count);
            foreach (string Tag in Data.Tags)
            {
               base.WriteString(Tag);
            }

            base.WriteBoolean(false);
        }
    }
}
