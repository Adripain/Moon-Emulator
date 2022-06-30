using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.Core;
using Moon.HabboHotel.Rooms;

using Moon.Communication.Packets.Outgoing.Rooms.Permissions;
using Moon.Communication.Packets.Outgoing.Rooms.Settings;
using Moon.HabboHotel.Users;

using Moon.Database.Interfaces;
using Moon.HabboHotel.Cache;

namespace Moon.Communication.Packets.Incoming.Rooms.Action
{
    class AssignRightsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            int UserId = Packet.PopInt();

            Room Room = null;
            if (!MoonEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            if (!Room.CheckRights(Session, true))
                return;

            if (Room.UsersWithRights.Contains(UserId))
            {
                Session.SendNotification(MoonEnvironment.GetGame().GetLanguageLocale().TryGetValue("room_rights_has_rights_error"));
                return;
            }

            Room.UsersWithRights.Add(UserId);

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("INSERT INTO `room_rights` (`room_id`,`user_id`) VALUES ('" + Room.RoomId + "','" + UserId + "')");
            }

            RoomUser RoomUser = Room.GetRoomUserManager().GetRoomUserByHabbo(UserId);
            if (RoomUser != null && !RoomUser.IsBot)
            {
                RoomUser.SetStatus("flatctrl", "1");
                RoomUser.UpdateNeeded = true;
                if (RoomUser.GetClient() != null)
                    RoomUser.GetClient().SendMessage(new YouAreControllerComposer(1));

                Session.SendMessage(new FlatControllerAddedComposer(Room.RoomId, RoomUser.GetClient().GetHabbo().Id, RoomUser.GetClient().GetHabbo().Username));
            }
            else
            {
                UserCache User =  MoonEnvironment.GetGame().GetCacheManager().GenerateUser(UserId);
                if (User != null)
                    Session.SendMessage(new FlatControllerAddedComposer(Room.RoomId, User.Id, User.Username));
            }
        }
    }
}
