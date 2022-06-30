﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.Database.Interfaces;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Items;


namespace Moon.Communication.Packets.Incoming.Rooms.Furni
{
    class SetMannequinNameEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null || !Room.CheckRights(Session, true))
                return;

            int ItemId = Packet.PopInt();
            string Name = Packet.PopString();

            Item Item = Session.GetHabbo().CurrentRoom.GetRoomItemHandler().GetItem(ItemId);
            if (Item == null)
                return;

            if (Item.ExtraData.Contains(Convert.ToChar(5)))
            {
                string[] Flags = Item.ExtraData.Split(Convert.ToChar(5));
                Item.ExtraData = Flags[0] + Convert.ToChar(5) + Flags[1] + Convert.ToChar(5) + Name;
            }
            else
                Item.ExtraData = "m" + Convert.ToChar(5) + ".ch-210-1321.lg-285-92" + Convert.ToChar(5) + "Default Maniqui";

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `items` SET `extra_data` = @Ed WHERE id = '" + Item.Id + "' LIMIT 1");
                dbClient.AddParameter("Ed", Item.ExtraData);
                dbClient.RunQuery();
            }

            Item.UpdateState(true, true);
        }
    }
}
