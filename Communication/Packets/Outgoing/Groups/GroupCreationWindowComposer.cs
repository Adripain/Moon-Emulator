﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;

namespace Moon.Communication.Packets.Outgoing.Groups
{
    class GroupCreationWindowComposer : ServerPacket
    {
        public GroupCreationWindowComposer(ICollection<RoomData> Rooms)
            : base(ServerPacketHeader.GroupCreationWindowMessageComposer)
        {
            base.WriteInteger(MoonStaticGameSettings.GroupPurchaseAmount);//Price

            base.WriteInteger(Rooms.Count);//Room count that the user has.
            foreach (RoomData Room in Rooms)
            {
                base.WriteInteger(Room.Id);//Room Id
               base.WriteString(Room.Name);//Room Name
                base.WriteBoolean(false);//What?
            }

            base.WriteInteger(5);
            base.WriteInteger(5);
            base.WriteInteger(11);
            base.WriteInteger(4);

            base.WriteInteger(6);
            base.WriteInteger(11);
            base.WriteInteger(4);

            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(0);

            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(0);

            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(0);
        }
    }
}
