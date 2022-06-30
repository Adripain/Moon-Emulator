﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Items;

using Moon.Communication.Packets.Outgoing.Rooms.Furni;
using Moon.Communication.Packets.Outgoing.Rooms.Engine;

namespace Moon.Communication.Packets.Incoming.Rooms.Furni
{
    class UpdateMagicTileEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            if (!Room.CheckRights(Session, false, true) && !Session.GetHabbo().GetPermissions().HasRight("room_item_use_any_stack_tile"))
                return;

            int ItemId = Packet.PopInt();
            int DecimalHeight = Packet.PopInt();

            Item Item = Room.GetRoomItemHandler().GetItem(ItemId);
            if (Item == null)
                return;

            Item.GetZ = ((double)DecimalHeight) / 100.0;

            Room.SendMessage(new ObjectUpdateComposer(Item, Convert.ToInt32(Session.GetHabbo().Id)));
            Room.SendMessage(new UpdateMagicTileComposer(ItemId, DecimalHeight));
        }
    }
}
