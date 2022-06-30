using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Groups;
using Moon.Communication.Packets.Outgoing.Groups;
using Moon.Communication.Packets.Outgoing.Rooms.Permissions;

using Moon.Database.Interfaces;


namespace Moon.Communication.Packets.Incoming.Groups
{
    class UpdateGroupSettingsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();

            Group Group = null;
            if (!MoonEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                return;

            if (Group.CreatorId != Session.GetHabbo().Id)
                return;

            int Type = Packet.PopInt();
            int FurniOptions = Packet.PopInt();

            switch (Type)
            {
                default:
                case 0:
                    Group.GroupType = GroupType.OPEN;
                    break;
                case 1:
                    Group.GroupType = GroupType.LOCKED;
                    break;
                case 2:
                    Group.GroupType = GroupType.PRIVATE;
                    break;
            }

            if (Group.GroupType != GroupType.LOCKED)
            {
                if (Group.GetRequests.Count > 0)
                {
                    foreach (int UserId in Group.GetRequests.ToList())
                    {
                        Group.HandleRequest(UserId, false);
                    }

                    Group.ClearRequests();
                }
            }

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `groups` SET `state` = @GroupState, `admindeco` = @AdminDeco WHERE `id` = " + Group.Id + " LIMIT 1");
                dbClient.AddParameter("GroupState", (Group.GroupType == GroupType.OPEN ? 0 : Group.GroupType == GroupType.LOCKED ? 1 : 2).ToString());
                dbClient.AddParameter("AdminDeco", (FurniOptions == 1 ? 1 : 0).ToString());
                dbClient.RunQuery();
            }

            Group.AdminOnlyDeco = FurniOptions;

            Room Room;
            if (!MoonEnvironment.GetGame().GetRoomManager().TryGetRoom(Group.RoomId, out Room))
                return;

            foreach (RoomUser User in Room.GetRoomUserManager().GetRoomUsers().ToList())
            {
                if (Room.OwnerId == User.UserId || Group.IsAdmin(User.UserId) || !Group.IsMember(User.UserId))
                    continue;

                if (FurniOptions == 1)
                {
                    User.RemoveStatus("flatctrl 1");
                    User.UpdateNeeded = true;

                    User.GetClient().SendMessage(new YouAreControllerComposer(0));
                }
                else if (FurniOptions == 0 && !User.Statusses.ContainsKey("flatctrl 1"))
                {
                    User.AddStatus("flatctrl 1", "");
                    User.UpdateNeeded = true;

                    User.GetClient().SendMessage(new YouAreControllerComposer(1));
                }
            }

            Session.SendMessage(new GroupInfoComposer(Group, Session));
        }
    }
}