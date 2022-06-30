﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Groups;
using Moon.Communication.Packets.Outgoing.Groups;
using Moon.Database.Interfaces;
using Moon.Communication.Packets.Outgoing.Users;
using Moon.HabboHotel.Rooms;

namespace Moon.Communication.Packets.Incoming.Groups
{
    class SetGroupFavouriteEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null)
                return;

            int GroupId = Packet.PopInt();
            if (GroupId == 0)
                return;

            Group Group = null;
            if (!MoonEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                return;

            Session.GetHabbo().GetStats().FavouriteGroupId = Group.Id;
            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `user_stats` SET `groupid` = " + Session.GetHabbo().GetStats().FavouriteGroupId + " WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
            }

            if (Session.GetHabbo().InRoom && Session.GetHabbo().CurrentRoom != null)
            {
                Session.GetHabbo().CurrentRoom.SendMessage(new RefreshFavouriteGroupComposer(Session.GetHabbo().Id));
                if (Group != null)
                {
                    Session.GetHabbo().CurrentRoom.SendMessage(new HabboGroupBadgesComposer(Group));

                    RoomUser User = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                    if (User != null)
                    Session.GetHabbo().CurrentRoom.SendMessage(new UpdateFavouriteGroupComposer(Session.GetHabbo().Id, Group, User.VirtualId));
                }
            }
            else
                Session.SendMessage(new RefreshFavouriteGroupComposer(Session.GetHabbo().Id));
        }
    }
}
