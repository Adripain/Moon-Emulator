﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Groups;
using Moon.HabboHotel.Items;
using Moon.Communication.Packets.Outgoing.Groups;
using Moon.Communication.Packets.Outgoing.Rooms.Engine;

using Moon.Database.Interfaces;


namespace Moon.Communication.Packets.Incoming.Groups
{
    class UpdateGroupColoursEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            int Colour1 = Packet.PopInt();
            int Colour2 = Packet.PopInt();

            Group Group = null;
            if (!MoonEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                return;
          
            if (Group.CreatorId != Session.GetHabbo().Id)
                return;

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `groups` SET `colour1` = @colour1, `colour2` = @colour2 WHERE `id` =' " + Group.Id + "' LIMIT 1");
                dbClient.AddParameter("colour1", Colour1);
                dbClient.AddParameter("colour2", Colour2);
                dbClient.RunQuery();
            }

            Group.Colour1 = Colour1;
            Group.Colour2 = Colour2;

            Session.SendMessage(new GroupInfoComposer(Group, Session));
            if (Session.GetHabbo().CurrentRoom != null)
            {
                foreach (Item Item in Session.GetHabbo().CurrentRoom.GetRoomItemHandler().GetFloor.ToList())
                {
                    if (Item == null || Item.GetBaseItem() == null)
                        continue;

                    if (Item.GetBaseItem().InteractionType != InteractionType.GUILD_ITEM && Item.GetBaseItem().InteractionType != InteractionType.GUILD_GATE || Item.GetBaseItem().InteractionType != InteractionType.GUILD_FORUM)
                        continue;

                    Session.GetHabbo().CurrentRoom.SendMessage(new ObjectUpdateComposer(Item, Convert.ToInt32(Item.UserID)));
                }
            }
        }
    }
}
