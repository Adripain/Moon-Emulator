using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Items;
using Moon.HabboHotel.Users;
using Moon.HabboHotel.GameClients;

using Moon.Communication.Packets.Outgoing.Rooms.Engine;


using Moon.Communication.Packets.Outgoing.Rooms.Furni;

using Moon.Database.Interfaces;
using Moon.HabboHotel.Cache;

namespace Moon.Communication.Packets.Incoming.Rooms.Furni
{
    class OpenGiftEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            int PresentId = Packet.PopInt();
            Item Present = Room.GetRoomItemHandler().GetItem(PresentId);
            if (Present == null)
                return;

            if (Present.UserID != Session.GetHabbo().Id)
                return;
            
            DataRow Data = null;
            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `base_id`,`extra_data` FROM `user_presents` WHERE `item_id` = '" + Present.Id + "' LIMIT 1");
                Data = dbClient.getRow();
            }

            if (Data == null)
            {
                Session.SendNotification("Oops! al parecer hay un error con este regalo.\nNosotros nos encargaremos de deshacernos de este regalo.");
                Room.GetRoomItemHandler().RemoveFurniture(null, Present.Id, false);

                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + Present.Id + "' LIMIT 1");
                    dbClient.RunQuery("DELETE FROM `user_presents` WHERE `item_id` = '" + Present.Id + "' LIMIT 1");
                }

                Session.GetHabbo().GetInventoryComponent().RemoveItem(Present.Id);
                return;
            }
            
            int PurchaserId = 0;
            if (Present.GetBaseItem().ItemName == "matic_box") PurchaserId = Session.GetHabbo().Id;
            else
            {
                if (!int.TryParse(Present.ExtraData.Split(Convert.ToChar(5))[2], out PurchaserId))
                {
                    Session.SendNotification("Oops! al parecer hay un error con este regalo.\nNosotros nos encargaremos de deshacernos de este regalo.");
                    Room.GetRoomItemHandler().RemoveFurniture(null, Present.Id, false);

                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + Present.Id + "' LIMIT 1");
                        dbClient.RunQuery("DELETE FROM `user_presents` WHERE `item_id` = '" + Present.Id + "' LIMIT 1");
                    }
                    Session.GetHabbo().GetInventoryComponent().RemoveItem(Present.Id);

                    return;
                }
            }

            UserCache Purchaser = MoonEnvironment.GetGame().GetCacheManager().GenerateUser(PurchaserId);
            if (Purchaser == null)
            {
                Session.SendNotification("Oops! al parecer hay un error con este regalo.\nNosotros nos encargaremos de deshacernos de este regalo..");
                Room.GetRoomItemHandler().RemoveFurniture(null, Present.Id, false);

                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + Present.Id + "' LIMIT 1");
                    dbClient.RunQuery("DELETE FROM `user_presents` WHERE `item_id` = '" + Present.Id + "' LIMIT 1");
                }

                Session.GetHabbo().GetInventoryComponent().RemoveItem(Present.Id);
                return;
            }

            ItemData BaseItem = null;
            if (!MoonEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(Data["base_id"]), out BaseItem))
            {
                Session.SendNotification("Oops, al parecer este regalo ya no existe!");
                Room.GetRoomItemHandler().RemoveFurniture(null, Present.Id, false);

                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + Present.Id + "' LIMIT 1");
                    dbClient.RunQuery("DELETE FROM `user_presents` WHERE `item_id` = '" + Present.Id + "' LIMIT 1");
                }

                Session.GetHabbo().GetInventoryComponent().RemoveItem(Present.Id);
                return;
            }


            Present.MagicRemove = true;
            Room.SendMessage(new ObjectUpdateComposer(Present, Convert.ToInt32(Session.GetHabbo().Id)));

            Thread thread = new Thread(() => FinishOpenGift(Session, BaseItem, Present, Room, Data));
            thread.Start();


        }

        private void FinishOpenGift(GameClient Session, ItemData BaseItem, Item Present, Room Room, DataRow Row)
        {
            try
            {
                if (BaseItem == null || Present == null || Room == null || Row == null)
                    return;


                Thread.Sleep(1500);

                bool ItemIsInRoom = true;

                Room.GetRoomItemHandler().RemoveFurniture(Session, Present.Id);

                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `items` SET `base_item` = @BaseItem, `extra_data` = @edata WHERE `id` = " + Present.Id);
                    dbClient.AddParameter("BaseItem", Row["base_id"]);
                    dbClient.AddParameter("edata", Row["extra_data"]);
                    dbClient.RunQuery();

                    dbClient.RunQuery("DELETE FROM `user_presents` WHERE `item_id` = " + Present.Id);
                }

                Present.BaseItem = Convert.ToInt32(Row["base_id"]);
                Present.ResetBaseItem();
                Present.ExtraData = (!string.IsNullOrEmpty(Convert.ToString(Row["extra_data"])) ? Convert.ToString(Row["extra_data"]) : "");

                if (Present.Data.Type == 's')
                {
                    if (!Room.GetRoomItemHandler().SetFloorItem(Session, Present, Present.GetX, Present.GetY, Present.Rotation, true, false, true))
                    {
                        using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunQuery("UPDATE `items` SET `room_id` = '0' WHERE `id` = " + Present.Id + " LIMIT 1");
                        }

                        ItemIsInRoom = false;
                    }
                }
                else
                {
                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE `items` SET `room_id` = '0' WHERE `id` = " + Present.Id + " LIMIT 1");
                    }

                    ItemIsInRoom = false;
                }

                Session.SendMessage(new OpenGiftComposer(Present.Data, Present.ExtraData, Present, ItemIsInRoom));

                Session.GetHabbo().GetInventoryComponent().UpdateItems(true);
            }
            catch
            {
            }
        }
    }
}