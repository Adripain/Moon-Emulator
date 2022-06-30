using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Users;
using Moon.Communication.Packets.Outgoing.Users;
using Moon.Communication.Packets.Outgoing.Navigator;
using Moon.Communication.Packets.Outgoing.Rooms.Engine;

using Moon.Database.Interfaces;
using Moon.Communication.Packets.Outgoing.Rooms.Session;

namespace Moon.Communication.Packets.Incoming.Users
{
    class ChangeNameEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Username);
            if (User == null)
                return;

            string NewName = Packet.PopString();
            string OldName = Session.GetHabbo().Username;

            if (NewName == OldName)
            {
                Session.GetHabbo().ChangeName(OldName);
                Session.SendMessage(new UpdateUsernameComposer(NewName));
                return;
            }

            

            bool InUse = false;
            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT COUNT(0) FROM `users` WHERE `username` = @name LIMIT 1");
                dbClient.AddParameter("name", NewName);
                InUse = dbClient.getInteger() == 1;
            }

            char[] Letters = NewName.ToLower().ToCharArray();
            string AllowedCharacters = "abcdefghijklmnopqrstuvwxyz.,_-;:?!1234567890";

            foreach (char Chr in Letters)
            {
                if (!AllowedCharacters.Contains(Chr))
                {
                    return;
                }
            }

            if (!Session.GetHabbo().GetPermissions().HasRight("mod_tool") && NewName.ToLower().Contains("mod") || NewName.ToLower().Contains("adm") || NewName.ToLower().Contains("admin")
                || NewName.ToLower().Contains("m0d") || NewName.ToLower().Contains("mob") || NewName.ToLower().Contains("m0b"))
                return;
            else if (NewName.Length > 15)
                return;
            else if (NewName.Length < 3)
                return;
            else if (InUse)
                return;
            else
            {
                if (!MoonEnvironment.GetGame().GetClientManager().UpdateClientUsername(Session, OldName, NewName))
                {
                    Session.SendNotification("Oops! ha ocurrido un problema mientras se actualizaba su nuevo nombre.");
                    return;
                }

                Session.GetHabbo().ChangingName = false;

                Room.GetRoomUserManager().RemoveUserFromRoom(Session, true, false);

                Session.GetHabbo().ChangeName(NewName);
                Session.GetHabbo().GetMessenger().OnStatusChanged(true);

                Session.SendMessage(new UpdateUsernameComposer(NewName));
                Room.SendMessage(new UserNameChangeComposer(Room.Id, User.VirtualId, NewName));

                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("INSERT INTO `logs_client_namechange` (`user_id`,`new_name`,`old_name`,`timestamp`) VALUES ('" + Session.GetHabbo().Id + "', @name, '" + OldName + "', '" + MoonEnvironment.GetUnixTimestamp() + "')");
                    dbClient.AddParameter("name", NewName);
                    dbClient.RunQuery();
                }

                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE users SET username = @name, changename = '1', newname = @name WHERE id = '" + Session.GetHabbo().Id + "' LIMIT 1");
                    dbClient.AddParameter("name", NewName);
                    dbClient.RunQuery();
                }


                ICollection<RoomData> Rooms = Session.GetHabbo().UsersRooms;
                foreach (RoomData Data in Rooms)
                {
                    if (Data == null)
                        continue;

                    Data.OwnerName = NewName;
                }

                foreach (Room UserRoom in MoonEnvironment.GetGame().GetRoomManager().GetRooms().ToList())
                {
                    if (UserRoom == null || UserRoom.RoomData.OwnerName != NewName)
                        continue;

                    UserRoom.OwnerName = NewName;
                    UserRoom.RoomData.OwnerName = NewName;

                    UserRoom.SendMessage(new RoomInfoUpdatedComposer(UserRoom.RoomId));
                }

                MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_Name", 1);

                Session.SendMessage(new RoomForwardComposer(Room.Id));
            }
        }

        private static bool CanChangeName(Habbo Habbo)
        {

            if (Habbo.Rank == 1 && Habbo.VIPRank == 0 && Habbo.LastNameChange == 0)
                return true;
            else if (Habbo.Rank == 2 && Habbo.VIPRank == 1 && (Habbo.LastNameChange == 0 || (MoonEnvironment.GetUnixTimestamp() + 604800) > Habbo.LastNameChange))
                return true;
            else if (Habbo.Rank == 1 && Habbo.VIPRank == 2 && (Habbo.LastNameChange == 0 || (MoonEnvironment.GetUnixTimestamp() + 86400) > Habbo.LastNameChange))
                return true;
            else if (Habbo.Rank == 1 && Habbo.VIPRank == 3)
                return true;
            else if (Habbo.Rank == 1 && Habbo.VIPRank == 1 && (Habbo.LastNameChange == 0 || (MoonEnvironment.GetUnixTimestamp() + 604800) > Habbo.LastNameChange))
                return true;
            else if (Habbo.GetPermissions().HasRight("mod_tool"))
                return true;

            return false;
        }
    }
}