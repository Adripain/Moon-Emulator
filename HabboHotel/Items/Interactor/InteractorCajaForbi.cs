using Moon.Communication.Packets.Outgoing.Catalog;
using Moon.Communication.Packets.Outgoing.Inventory.Furni;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.Communication.Packets.Outgoing.Users;
using Moon.Database.Interfaces;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using Moon.Communication.Packets.Outgoing.Inventory.Purse;
using Moon.Communication.Packets.Outgoing.Rooms.Nux;

namespace Moon.HabboHotel.Items.Interactor
{
    class InteractorCajaForbi : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {
            Item.ExtraData = "0";
            Item.UpdateNeeded = true;

        }

        public void OnRemove(GameClient Session, Item Item)
        {
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            RoomUser User = null;
            if (Session != null)
                User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            //  +-+-+-+-+-+ +-+-+-+-+-+
            //  |Caja mágica
            //   +-+-+-+-+-+ +-+-+-+-+-+

            if (Item.BaseItem == 12264)
            {
                
                    Random random = new Random();

                    int mobi1 = 2124013;
                    string mobi01 = "un Dragón de Arena";
                    string imagem01 = "qt_sum11_dragon";

                    int mobi2 = 258;
                    string mobi02 = "una Fuente gris";
                    string imagem02 = "rare_fountain_1";

                    int mobi3 = 363;
                    string mobi03 = "una Puerta blanca espacial";
                    string imagem03 = "scifidoor_5";

                    int mobi4 = 342;
                    string mobi04 = "una Puerta láser blanca";
                    string imagem04 = "scifiport_6";

                    int randomNumber = random.Next(1, 8);
                    if (Item.UserID != Session.GetHabbo().Id)
                    {
                    Session.SendMessage(RoomNotificationComposer.SendBubble("furni_placement_error", "Esta caja mágica no te pertenece.", ""));
                        return;
                    }
                    Room Room = Session.GetHabbo().CurrentRoom;
                    Room.GetRoomItemHandler().RemoveFurniture(null, Item.Id);
                    if (randomNumber == 1)
                    {
                        using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunQuery("UPDATE `items` SET `room_id` = '0', `base_item` = '" + mobi1 + "' WHERE `id` = '" + Item.Id + "' LIMIT 1");
                        }
                        Session.GetHabbo().GetInventoryComponent().AddNewItem(Item.Id, mobi1, Item.ExtraData, Item.GroupId, true, true, Item.LimitedNo, Item.LimitedTot);
                        Session.GetHabbo().GetInventoryComponent().UpdateItems(true);
                        Session.SendMessage(RoomNotificationComposer.SendBubble("icons/" + imagem01 + "_icon", "Usted acaba de ganar " + mobi01, "inventory/open/furni"));

                        return;
                    }
                    if (randomNumber == 2)
                    {
                        using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunQuery("UPDATE `items` SET `room_id` = '0', `base_item` = '" + mobi2 + "' WHERE `id` = '" + Item.Id + "' LIMIT 1");
                        }
                        Session.GetHabbo().GetInventoryComponent().AddNewItem(Item.Id, mobi2, Item.ExtraData, Item.GroupId, true, true, Item.LimitedNo, Item.LimitedTot);
                        Session.GetHabbo().GetInventoryComponent().UpdateItems(true);
                        Session.SendMessage(RoomNotificationComposer.SendBubble("icons/" + imagem02 + "_icon", "Usted acaba de ganar " + mobi02, "inventory/open/furni"));

                        return;
                    }
                    if (randomNumber == 3)
                    {
                        using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunQuery("UPDATE `items` SET `room_id` = '0', `base_item` = '" + mobi3 + "' WHERE `id` = '" + Item.Id + "' LIMIT 1");
                        }
                        Session.GetHabbo().GetInventoryComponent().AddNewItem(Item.Id, mobi3, Item.ExtraData, Item.GroupId, true, true, Item.LimitedNo, Item.LimitedTot);
                        Session.GetHabbo().GetInventoryComponent().UpdateItems(true);
                        Session.SendMessage(RoomNotificationComposer.SendBubble("icons/" + imagem03 + "_icon", "Usted acaba de ganar " + mobi03, "inventory/open/furni"));

                        return;
                    }
                    if (randomNumber == 4)
                    {
                        using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunQuery("UPDATE `items` SET `room_id` = '0', `base_item` = '" + mobi4 + "' WHERE `id` = '" + Item.Id + "' LIMIT 1");
                        }
                        Session.GetHabbo().GetInventoryComponent().AddNewItem(Item.Id, mobi4, Item.ExtraData, Item.GroupId, true, true, Item.LimitedNo, Item.LimitedTot);
                        Session.GetHabbo().GetInventoryComponent().UpdateItems(true);
                        Session.SendMessage(RoomNotificationComposer.SendBubble("icons/" + imagem04 + "_icon", "Usted acaba de ganar " + mobi04, "inventory/open/furni"));
                        return;
                    }

                if (randomNumber == 5)
                {
                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + Item.Id + "' LIMIT 1");
                    }

                    int Amountd = random.Next(1, 20);
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, Amountd, 5));
                    return;
                }

                if (randomNumber == 6)
                {
                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + Item.Id + "' LIMIT 1");
                    }
                    Session.SendMessage(new NuxItemListComposer());
                    return;
                }

                if (randomNumber == 7)
                {
                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + Item.Id + "' LIMIT 1");
                    }
                    int Amountduc = random.Next(50, 500);
                    Session.SendMessage(RoomNotificationComposer.SendBubble("duckets", "Acabas de ganar " + Amountduc + " duckets en la caja mágica.", ""));

                    Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Amountduc));
                    return;
                }


                if (randomNumber == 8)
                {
                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE `items` SET `room_id` = '0' WHERE `id` = '" + Item.Id + "' LIMIT 1");
                    }

                    Session.GetHabbo().GetInventoryComponent().AddNewItem(Item.Id, 202, Item.ExtraData, Item.GroupId, true, true, Item.LimitedNo, Item.LimitedTot);
                    Session.GetHabbo().GetInventoryComponent().AddNewItem(Item.Id, 88805298, Item.ExtraData, Item.GroupId, true, true, Item.LimitedNo, Item.LimitedTot);

                    Session.GetHabbo().GetInventoryComponent().UpdateItems(true);
                    Session.SendMessage(RoomNotificationComposer.SendBubble("premio", "Usted acaba de ganar el SUPER PREMIO +1 Caja mágica, +1 Throne, +1 Drako angel", "inventory/open/furni"));
                    return;

                }

            }

            //  +-+-+-+-+-+ +-+-+-+-+-+
            //  |CAJA HC
            //   +-+-+-+-+-+ +-+-+-+-+-+

            /*if (Item.BaseItem == 9324 || Item.BaseItem == 1346781567)
            {
                if (Item.UserID != Session.GetHabbo().Id)
                {
                    Session.SendMessage(RoomNotificationComposer.SendBubble("furni_placement_error", "message", "Esta caixa HC não te pertence."));
                    return;
                }
                Room Room = Session.GetHabbo().CurrentRoom;
                Room.GetRoomItemHandler().RemoveFurniture(null, Item.Id);

                if (Item.BaseItem == 9324)
                {
                    int mobi1 = 756; // 1C
                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE `items` SET `room_id` = '0', `base_item` = '" + mobi1 + "' WHERE `id` = '" + Item.Id + "' LIMIT 1");
                    }
                    Session.GetHabbo().GetInventoryComponent().AddNewItem(Item.Id, mobi1, Item.ExtraData, Item.GroupId, true, true, Item.LimitedNo, Item.LimitedTot);
                    Session.GetHabbo().GetInventoryComponent().UpdateItems(true);
                    int num = num = 31 * 1;
                    Session.GetHabbo().GetClubManager().AddOrExtendSubscription("habbo_vip", num * 24 * 3600, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("HC1", true, Session);
                    MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_BasicClub", 1);
                    MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_VipHC", 1);
                    Session.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));
                    Session.SendMessage(new FurniListUpdateComposer());
                    return;
                }

                if (Item.BaseItem == 12263)
                {
                    int mobi1 = 756; // 1C
                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE `items` SET `room_id` = '0', `base_item` = '" + mobi1 + "' WHERE `id` = '" + Item.Id + "' LIMIT 1");
                    }
                    Session.GetHabbo().GetInventoryComponent().AddNewItem(Item.Id, mobi1, Item.ExtraData, Item.GroupId, true, true, Item.LimitedNo, Item.LimitedTot);
                    Session.GetHabbo().GetInventoryComponent().UpdateItems(true);
                    int num = num = 31 * 1;
                    Session.GetHabbo().GetClubManager().AddOrExtendSubscription("habbo_vip", num * 24 * 3600, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("HC1", true, Session);
                    MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_BasicClub", 1);
                    MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_VipHC", 1);
                    Session.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));
                    Session.SendMessage(new FurniListUpdateComposer());
                    return;
                }

            }*/
           
        }

        public void OnWiredTrigger(Item Item)
        {
            Item.ExtraData = "-1";
            Item.UpdateState(false, true);
            Item.RequestUpdate(4, true);
        }

    }
}
