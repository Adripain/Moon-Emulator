using System;
using System.Linq;
using System.Collections.Generic;

using Moon.Core;
using Moon.HabboHotel.Items;
using Moon.Communication.Packets.Outgoing.Inventory.Trading;
using Moon.Communication.Packets.Outgoing;
using Moon.Database.Interfaces;
using Moon.Communication.Packets.Outgoing.Inventory.Furni;

namespace Moon.HabboHotel.Rooms.Trading
{
    public class Trade
    {
        private readonly int RoomId;
        public TradeUser[] Users;

        private readonly int oneId;
        private readonly int twoId;
        private int TradeStage;

        public Trade(int UserOneId, int UserTwoId, int RoomId)
        {
            oneId = UserOneId;
            twoId = UserTwoId;

            Users = new TradeUser[2];
            Users[0] = new TradeUser(UserOneId, RoomId);
            Users[1] = new TradeUser(UserTwoId, RoomId);
            TradeStage = 1;
            this.RoomId = RoomId;

            foreach (TradeUser User in Users.ToList())
            {
                if (!User.GetRoomUser().Statusses.ContainsKey("trd"))
                {
                    User.GetRoomUser().AddStatus("trd", "");
                    User.GetRoomUser().UpdateNeeded = true;
                }
            }

            SendMessageToUsers(new TradingStartComposer(UserOneId, UserTwoId));
        }

        public bool AllUsersAccepted
        {
            get
            {
                for (int i = 0; i < Users.Length; i++)
                {
                    if (Users[i] == null)
                        continue;
                    if (!Users[i].HasAccepted)
                        return false;
                }

                return true;
            }
        }

        public bool ContainsUser(int Id)
        {
            for (int i = 0; i < Users.Length; i++)
            {
                if (Users[i] == null)
                    continue;
                if (Users[i].UserId == Id)
                    return true;
            }

            return false;
        }

        public TradeUser GetTradeUser(int Id)
        {
            for (int i = 0; i < Users.Length; i++)
            {
                if (Users[i] == null)
                    continue;
                if (Users[i].UserId == Id)
                    return Users[i];
            }

            return null;
        }

        public void OfferItem(int UserId, Item Item)
        {
            TradeUser User = GetTradeUser(UserId);

            if (User == null || Item == null || !Item.GetBaseItem().AllowTrade || User.HasAccepted || TradeStage != 1)
            {
                return;
            }

            ClearAccepted();

            if (!User.OfferedItems.Contains(Item))
                User.OfferedItems.Add(Item);

            UpdateTradeWindow();
        }

        public void TakeBackItem(int UserId, Item Item)
        {
            TradeUser User = GetTradeUser(UserId);

            if (User == null || Item == null || User.HasAccepted || TradeStage != 1)
            {
                return;
            }

            ClearAccepted();

            User.OfferedItems.Remove(Item);
            UpdateTradeWindow();
        }

        public void Accept(int UserId)
        {
            TradeUser User = GetTradeUser(UserId);

            if (User == null || TradeStage != 1)
            {
                return;
            }

            User.HasAccepted = true;

            SendMessageToUsers(new TradingAcceptComposer(UserId, true));


            if (AllUsersAccepted)
            {
                SendMessageToUsers(new TradingCompleteComposer());
                TradeStage++;
                ClearAccepted();
            }
        }

        public void Unaccept(int UserId)
        {
            TradeUser User = GetTradeUser(UserId);

            if (User == null || TradeStage != 1 || AllUsersAccepted)
            {
                return;
            }

            User.HasAccepted = false;

            SendMessageToUsers(new TradingAcceptComposer(UserId, false));
        }

        public void CompleteTrade(int UserId)
        {
            TradeUser User = GetTradeUser(UserId);

            if (User == null || TradeStage != 2)
            {
                return;
            }

            User.HasAccepted = true;

            SendMessageToUsers(new TradingConfirmedComposer(UserId, true));

            if (AllUsersAccepted)
            {
                TradeStage = 999;
                Finnito();
            }
        }

        private void Finnito()
        {
            try
            {
                DeliverItems();
                CloseTradeClean();
            }
            catch (Exception e)
            {
                Logging.LogThreadException(e.ToString(), "Trade task");
            }
        }

        public void ClearAccepted()
        {
            foreach (TradeUser User in Users.ToList())
            {
                User.HasAccepted = false;
            }
        }

        public void UpdateTradeWindow()
        {
            foreach (TradeUser User in this.Users.ToList())
            {
                if (User == null)
                    continue;

                SendMessageToUsers(new TradingUpdateComposer(this));
            }
        }

        public void DeliverItems()
        {
            // List items
            List<Item> ItemsOne = GetTradeUser(oneId).OfferedItems;
            List<Item> ItemsTwo = GetTradeUser(twoId).OfferedItems;

            string User1 = "";
            string User2 = "";

            // Verify they are still in user inventory
            foreach (Item I in ItemsOne.ToList())
            {
                if (I == null)
                    continue;

                if (GetTradeUser(oneId).GetClient().GetHabbo().GetInventoryComponent().GetItem(I.Id) == null)
                {
                    GetTradeUser(oneId).GetClient().SendNotification(MoonEnvironment.GetGame().GetLanguageLocale().TryGetValue("trade_failed"));
                    GetTradeUser(twoId).GetClient().SendNotification(MoonEnvironment.GetGame().GetLanguageLocale().TryGetValue("trade_failed"));
                    return;
                }
                User1 += I.Id + ";";
            }

            foreach (Item I in ItemsTwo.ToList())
            {
                if (I == null)
                    continue;

                if (GetTradeUser(twoId).GetClient().GetHabbo().GetInventoryComponent().GetItem(I.Id) == null)
                {
                    GetTradeUser(oneId).GetClient().SendNotification(MoonEnvironment.GetGame().GetLanguageLocale().TryGetValue("trade_failed"));
                    GetTradeUser(twoId).GetClient().SendNotification(MoonEnvironment.GetGame().GetLanguageLocale().TryGetValue("trade_failed"));

                    return;
                }
                User2 += I.Id + ";";
            }

            if (GetTradeUser(oneId).GetClient().GetHabbo().SecureTradeEnabled == false && GetTradeUser(twoId).GetClient().GetHabbo().SecureTradeEnabled == false)
            {
                // Deliver them
                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    foreach (Item I in ItemsOne.ToList())
                    {
                        if (I == null)
                            continue;

                        GetTradeUser(oneId).GetClient().GetHabbo().GetInventoryComponent().RemoveItem(I.Id);

                        dbClient.SetQuery("UPDATE `items` SET `user_id` = @user WHERE `id` = @id LIMIT 1");
                        dbClient.AddParameter("user", twoId);
                        dbClient.AddParameter("id", I.Id);
                        dbClient.RunQuery();

                        GetTradeUser(twoId).GetClient().GetHabbo().GetInventoryComponent().AddNewItem(I.Id, I.BaseItem, I.ExtraData, I.GroupId, false, false, I.LimitedNo, I.LimitedTot);
                        GetTradeUser(twoId).GetClient().SendMessage(new FurniListAddComposer(I));
                        GetTradeUser(twoId).GetClient().SendMessage(new FurniListNotificationComposer(I.Id, 1));
                    }

                    foreach (Item I in ItemsTwo.ToList())
                    {
                        if (I == null)
                            continue;

                        GetTradeUser(twoId).GetClient().GetHabbo().GetInventoryComponent().RemoveItem(I.Id);

                        dbClient.SetQuery("UPDATE `items` SET `user_id` = @user WHERE `id` = @id LIMIT 1");
                        dbClient.AddParameter("user", oneId);
                        dbClient.AddParameter("id", I.Id);
                        dbClient.RunQuery();

                        GetTradeUser(oneId).GetClient().GetHabbo().GetInventoryComponent().AddNewItem(I.Id, I.BaseItem, I.ExtraData, I.GroupId, false, false, I.LimitedNo, I.LimitedTot);
                        GetTradeUser(oneId).GetClient().SendMessage(new FurniListAddComposer(I));
                        GetTradeUser(oneId).GetClient().SendMessage(new FurniListNotificationComposer(I.Id, 1));
                    }
                }

                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("INSERT INTO `logs_client_trade` VALUES(null, @1id, @2id, @1items, @2items, UNIX_TIMESTAMP())");
                    dbClient.AddParameter("1id", oneId);
                    dbClient.AddParameter("2id", twoId);
                    dbClient.AddParameter("1items", User1);
                    dbClient.AddParameter("2items", User2);
                    dbClient.RunQuery();
                }
            }

            GetTradeUser(oneId).GetClient().GetHabbo().GetInventoryComponent().UpdateItems(true);
            GetTradeUser(oneId).GetClient().SendMessage(new FurniListUpdateComposer());
            GetTradeUser(twoId).GetClient().GetHabbo().GetInventoryComponent().UpdateItems(true);
            GetTradeUser(twoId).GetClient().SendMessage(new FurniListUpdateComposer());

            //    else if(GetTradeUser(oneId).GetClient().GetHabbo().SecureTradeEnabled == true && GetTradeUser(twoId).GetClient().GetHabbo().SecureTradeEnabled == true)// SISTEMA DE DADOS
            //    {
            //        using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            //        {
            //            foreach (Item I in ItemsOne.ToList())
            //            {
            //                if (I == null)
            //                    continue;

            //                GetTradeUser(oneId).GetClient().GetHabbo().GetInventoryComponent().RemoveItem(I.Id);

            //                dbClient.SetQuery("UPDATE `items` SET `user_id` = @user WHERE `id` = @id LIMIT 1");
            //                dbClient.AddParameter("user", 3); // @ Security Account Dices
            //                dbClient.AddParameter("id", I.Id);
            //                dbClient.RunQuery();
            //            }

            //            foreach (Item I in ItemsTwo.ToList())
            //            {
            //                if (I == null)
            //                    continue;

            //                GetTradeUser(twoId).GetClient().GetHabbo().GetInventoryComponent().RemoveItem(I.Id);

            //                dbClient.SetQuery("UPDATE `items` SET `user_id` = @user WHERE `id` = @id LIMIT 1");
            //                dbClient.AddParameter("user", 3); // @ Security Account Dices
            //                dbClient.AddParameter("id", I.Id);
            //                dbClient.RunQuery();
            //            }
            //        }

            //        using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            //        {
            //            dbClient.SetQuery("INSERT INTO `logs_client_trade` VALUES(null, @1id, @2id, @1items, @2items, UNIX_TIMESTAMP())");
            //            dbClient.AddParameter("1id", oneId);
            //            dbClient.AddParameter("2id", twoId);
            //            dbClient.AddParameter("1items", User1);
            //            dbClient.AddParameter("2items", User2);
            //            dbClient.RunQuery();
            //        }

            //        GetTradeUser(oneId).GetClient().GetHabbo().PlayingDice = true;
            //        GetTradeUser(twoId).GetClient().GetHabbo().PlayingDice = true;

            //        // Get each user - DICE SYSTEM BY CUSTOM
            //        Room Room = GetRoom();
            //        RoomUser user1 = Room.GetRoomUserManager().GetRoomUserByHabbo(GetTradeUser(oneId).GetClient().GetHabbo().Id);
            //        RoomUser user2 = Room.GetRoomUserManager().GetRoomUserByHabbo(GetTradeUser(twoId).GetClient().GetHabbo().Id);

            //        // Notify rare count from each user - DICE SYSTEM BY CUSTOM
            //        user1.OnChat(33, "Acabo de apostar un total de " + GetTradeUser(oneId).GetClient().GetHabbo().TradeItems.Count() + " rares en dados.", false);
            //        user2.OnChat(33, "Acabo de apostar un total de " + GetTradeUser(twoId).GetClient().GetHabbo().TradeItems.Count() + " rares en dados.", false);

            //        // Assign item list for each user - DICE SYSTEM BY CUSTOM
            //        GetTradeUser(oneId).GetClient().GetHabbo().TradeItems = GetTradeUser(oneId).OfferedItems;
            //        GetTradeUser(twoId).GetClient().GetHabbo().TradeItems = GetTradeUser(twoId).OfferedItems;

            //        // Assign opponent username for the future foreach - DICE SYSTEM BY CUSTOM
            //        GetTradeUser(oneId).GetClient().GetHabbo().Opponent = GetTradeUser(twoId).GetClient().GetHabbo().Username;
            //        GetTradeUser(twoId).GetClient().GetHabbo().Opponent = GetTradeUser(oneId).GetClient().GetHabbo().Username;
            //    }

            //    else
            //    {
            //        GetTradeUser(oneId).GetClient().SendChat("Algo salió mal durante el tradeo, tienes el modo de seguridad" + (GetTradeUser(oneId).GetClient().GetHabbo().SecureTradeEnabled == true ? " activado." : " desactivado.") + "", 33);
            //        GetTradeUser(twoId).GetClient().SendChat("Algo salió mal durante el tradeo, tienes el modo de seguridad" + (GetTradeUser(twoId).GetClient().GetHabbo().SecureTradeEnabled == true ? " activado." : " desactivado.") + "", 33);
            //    }
            //}
        }

        public void CloseTradeClean()
        {
            foreach (TradeUser User in this.Users.ToList())
            {
                if (User == null || User.GetRoomUser() == null)
                    continue;

                if (User.GetRoomUser().Statusses.ContainsKey("trd"))
                {
                    User.GetRoomUser().RemoveStatus("trd");
                    User.GetRoomUser().UpdateNeeded = true;
                }
            }

            SendMessageToUsers(new TradingFinishComposer());
            GetRoom().ActiveTrades.Remove(this);
        }

        public void CloseTrade(int UserId)
        {
            foreach (TradeUser User in this.Users.ToList())
            {
                if (User == null || User.GetRoomUser() == null)
                    continue;

                if (User.GetRoomUser().Statusses.ContainsKey("trd"))
                {
                    User.GetRoomUser().RemoveStatus("trd");
                    User.GetRoomUser().UpdateNeeded = true;
                }
            }

            SendMessageToUsers(new TradingClosedComposer(UserId));
        }

        public void SendMessageToUsers(ServerPacket Message)
        {
            foreach (TradeUser User in this.Users.ToList())
            {
                if (User == null || User.GetClient() == null)
                    continue;

                User.GetClient().SendMessage(Message);
            }
        }

        private Room GetRoom()
        {
            Room Room;
            if (MoonEnvironment.GetGame().GetRoomManager().TryGetRoom(RoomId, out Room))
                return Room;

            return null;
        }
    }
}