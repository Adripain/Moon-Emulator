using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Users;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Items;
using Moon.HabboHotel.GameClients;

using Moon.Database.Interfaces;


namespace Moon.Communication.Packets.Incoming.Rooms.Settings
{
    class DeleteRoomEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().UsersRooms == null)
                return;

            int RoomId = Packet.PopInt();
            if (RoomId == 0)
                return;
            if (Session.GetHabbo().Rank > 3 && !Session.GetHabbo().StaffOk)
                return;
            Room Room;

            if (!MoonEnvironment.GetGame().GetRoomManager().TryGetRoom(RoomId, out Room))
                return;

            RoomData data = Room.RoomData;
            if (data == null)
                return;

            if (Room.OwnerId != Session.GetHabbo().Id && !Session.GetHabbo().GetPermissions().HasRight("room_delete_any") || MoonStaticGameSettings.IsGoingToBeClose)
                return;

            List<Item> ItemsToRemove = new List<Item>();
            foreach (Item Item in Room.GetRoomItemHandler().GetWallAndFloor.ToList())
            {
                if (Item == null)
                    continue;

                if (Item.GetBaseItem().InteractionType == InteractionType.MOODLIGHT)
                {
                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("DELETE FROM `room_items_moodlight` WHERE `item_id` = '" + Item.Id + "' LIMIT 1");
                    }
                }

                ItemsToRemove.Add(Item);
            }

            foreach (Item Item in ItemsToRemove)
            {
                GameClient targetClient = MoonEnvironment.GetGame().GetClientManager().GetClientByUserID(Item.UserID);
                if (targetClient != null && targetClient.GetHabbo() != null)//Again, do we have an active client?
                {
                    Room.GetRoomItemHandler().RemoveFurniture(targetClient, Item.Id);
                    targetClient.GetHabbo().GetInventoryComponent().AddNewItem(Item.Id, Item.BaseItem, Item.ExtraData, Item.GroupId, true, true, Item.LimitedNo, Item.LimitedTot);
                    targetClient.GetHabbo().GetInventoryComponent().UpdateItems(false);
                }
                else//No, query time.
                {
                    Room.GetRoomItemHandler().RemoveFurniture(null, Item.Id);
                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE `items` SET `room_id` = '0' WHERE `id` = '" + Item.Id + "' LIMIT 1");
                    }
                }
            }

            MoonEnvironment.GetGame().GetRoomManager().UnloadRoom(Room, true);

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("DELETE FROM `user_roomvisits` WHERE `room_id` = '" + RoomId + "'");
                dbClient.RunQuery("DELETE FROM `rooms` WHERE `id` = '" + RoomId + "' LIMIT 1");
                dbClient.RunQuery("DELETE FROM `user_favorites` WHERE `room_id` = '" + RoomId + "'");
                dbClient.RunQuery("DELETE FROM `items` WHERE `room_id` = '" + RoomId + "'");
                dbClient.RunQuery("DELETE FROM `room_rights` WHERE `room_id` = '" + RoomId + "'");
                dbClient.RunQuery("UPDATE `users` SET `home_room` = '0' WHERE `home_room` = '" + RoomId + "'");
            }

            RoomData removedRoom = (from p in Session.GetHabbo().UsersRooms where p.Id == RoomId select p).SingleOrDefault();
            if (removedRoom != null)
                Session.GetHabbo().UsersRooms.Remove(removedRoom);
        }
    }
}