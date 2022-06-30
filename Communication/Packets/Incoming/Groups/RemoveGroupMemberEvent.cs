﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Groups;
using Moon.Communication.Packets.Outgoing.Groups;
using Moon.Communication.Packets.Outgoing.Rooms.Permissions;
using Moon.Database.Interfaces;
using Moon.HabboHotel.Users;
using Moon.HabboHotel.Cache;
using Moon.Communication.Packets.Outgoing.Messenger;

namespace Moon.Communication.Packets.Incoming.Groups
{
    class RemoveGroupMemberEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            int UserId = Packet.PopInt();

            Group Group = null;
            if (!MoonEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                return;

            if (UserId == Session.GetHabbo().Id)
            {
                if (Group.IsMember(UserId))
                    Group.DeleteMember(UserId);

                if (Group.IsAdmin(UserId))
                {
                    if (Group.IsAdmin(UserId))
                        Group.TakeAdmin(UserId);

                    Room Room;

                    if (!MoonEnvironment.GetGame().GetRoomManager().TryGetRoom(Group.RoomId, out Room))
                        return;

                    RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                    if (User != null)
                    {
                        User.RemoveStatus("flatctrl 1");
                        User.UpdateNeeded = true;

                        if (User.GetClient() != null)
                            User.GetClient().SendMessage(new YouAreControllerComposer(0));
                    }
                }

                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("DELETE FROM `group_memberships` WHERE `group_id` = @GroupId AND `user_id` = @UserId");
                    dbClient.AddParameter("GroupId", GroupId);
                    dbClient.AddParameter("UserId", UserId);
                    dbClient.RunQuery();
                }

                Session.SendMessage(new GroupInfoComposer(Group, Session));
                if (Session.GetHabbo().GetStats().FavouriteGroupId == GroupId)
                {
                    Session.GetHabbo().GetStats().FavouriteGroupId = 0;
                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE `user_stats` SET `groupid` = '0' WHERE `id` = '" + UserId + "' LIMIT 1");
                    }

                    if (Group.AdminOnlyDeco == 0)
                    {
                        Room Room;
                        if (!MoonEnvironment.GetGame().GetRoomManager().TryGetRoom(Group.RoomId, out Room))
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                        if (User != null)
                        {
                            User.RemoveStatus("flatctrl 1");
                            User.UpdateNeeded = true;

                            if (User.GetClient() != null)
                                User.GetClient().SendMessage(new YouAreControllerComposer(0));
                        }
                    }

                    if (Session.GetHabbo().InRoom && Session.GetHabbo().CurrentRoom != null)
                    {
                        RoomUser User = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                        if (User != null)
                            Session.GetHabbo().CurrentRoom.SendMessage(new UpdateFavouriteGroupComposer(Session.GetHabbo().Id, Group, User.VirtualId));
                        Session.GetHabbo().CurrentRoom.SendMessage(new RefreshFavouriteGroupComposer(Session.GetHabbo().Id));
                    }
                    else
                        Session.SendMessage(new RefreshFavouriteGroupComposer(Session.GetHabbo().Id));
                }
                if (Group.HasChat)
                {
                    var Client = MoonEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
                    if (Client != null)
                    {
                        Client.SendMessage(new FriendListUpdateComposer(Group, -1));
                    }
                }
                return;
            }

            else
            {
                if (Group.CreatorId == Session.GetHabbo().Id || Group.IsAdmin(Session.GetHabbo().Id))
                {
                    if (!Group.IsMember(UserId))
                        return;

                    if (Group.IsAdmin(UserId) && Group.CreatorId != Session.GetHabbo().Id)
                    {
                        Session.SendNotification("Lo sentimos, sólo los creadores del grupo pueden eliminar otros administradores del grupo");
                        return;
                    }

                    if (Group.IsAdmin(UserId))
                        Group.TakeAdmin(UserId);

                    if (Group.IsMember(UserId))
                        Group.DeleteMember(UserId);

                    List<UserCache> Members = new List<UserCache>();
                    List<int> MemberIds = Group.GetAllMembers;
                    foreach (int Id in MemberIds.ToList())
                    {
                        UserCache GroupMember = MoonEnvironment.GetGame().GetCacheManager().GenerateUser(Id);
                        if (GroupMember == null)
                            continue;

                        if (!Members.Contains(GroupMember))
                            Members.Add(GroupMember);
                    }


                    int FinishIndex = 14 < Members.Count ? 14 : Members.Count;
                    int MembersCount = Members.Count;

                    Session.SendMessage(new GroupMembersComposer(Group, Members.Take(FinishIndex).ToList(), MembersCount, 1, (Group.CreatorId == Session.GetHabbo().Id || Group.IsAdmin(Session.GetHabbo().Id)), 0, ""));
                    if (Group.HasChat)
                    {
                        var Client = MoonEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
                        if (Client != null)
                        {
                            Client.SendMessage(new FriendListUpdateComposer(Group, -1));
                        }
                    }
                }
            }
        }
    }
}