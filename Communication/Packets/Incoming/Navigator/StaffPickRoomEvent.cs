using Moon.HabboHotel.Navigator;
using Moon.HabboHotel.GameClients;
using Moon.Database.Interfaces;
using Moon.HabboHotel.Rooms;
using Moon.Communication.Packets.Outgoing.Navigator;
using Moon.Communication.Packets.Outgoing.Rooms.Settings;

namespace Moon.Communication.Packets.Incoming.Navigator
{
    class StaffPickRoomEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            GameClient TargetClient = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(session.GetHabbo().CurrentRoom.OwnerName);
        
            if (!session.GetHabbo().GetPermissions().HasRight("room.staff_picks.management"))
                return;

            Room room = null;
            if (!MoonEnvironment.GetGame().GetRoomManager().TryGetRoom(packet.PopInt(), out room))
                return;

            StaffPick staffPick = null;
            if (!MoonEnvironment.GetGame().GetNavigator().TryGetStaffPickedRoom(room.Id, out staffPick))
            {
                if (MoonEnvironment.GetGame().GetNavigator().TryAddStaffPickedRoom(room.Id))
                {
                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("INSERT INTO `navigator_staff_picks` (`room_id`,`image`) VALUES (@roomId, null)");
                        dbClient.AddParameter("roomId", room.Id);
                        dbClient.RunQuery();
                    }
                    if (TargetClient != null)
                    {
                        MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(TargetClient, "ACH_Spr", 1, false);
                    }
                }
            }

            else
            {
                if (MoonEnvironment.GetGame().GetNavigator().TryRemoveStaffPickedRoom(room.Id))
                {
                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("DELETE FROM `navigator_staff_picks` WHERE `room_id` = @roomId LIMIT 1");
                        dbClient.AddParameter("roomId", room.Id);
                        dbClient.RunQuery();
                    }
                }
            }

            room.SendMessage(new RoomSettingsSavedComposer(room.RoomId));
            room.SendMessage(new RoomInfoUpdatedComposer(room.RoomId));
        }
    }
}