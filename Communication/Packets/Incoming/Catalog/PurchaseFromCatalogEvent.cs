using System;
using System.Linq;
using System.Collections.Generic;

using Moon.Core;
using Moon.Communication.Packets.Incoming;
using Moon.HabboHotel.Catalog;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Items;
using Moon.HabboHotel.Groups;
using Moon.HabboHotel.Users.Effects;
using Moon.HabboHotel.Items.Utilities;
using Moon.HabboHotel.Users.Inventory.Bots;
using Moon.Communication.Packets.Outgoing.Handshake;
using Moon.HabboHotel.Rooms.AI;
using Moon.Communication.Packets.Outgoing.Catalog;
using Moon.Communication.Packets.Outgoing.Inventory.Bots;
using Moon.Communication.Packets.Outgoing.Inventory.Pets;
using Moon.Communication.Packets.Outgoing.Inventory.Purse;
using Moon.Communication.Packets.Outgoing.Inventory.Furni;
using Moon.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Moon.Database.Interfaces;
using Moon.Communication.Packets.Outgoing.Moderation;
using Moon.Utilities;
using Moon.Communication.Packets.Outgoing.Navigator;
using Moon.Communication.Packets.Outgoing.Users;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using System.Text;
using Moon.HabboHotel.Groups.Forums;

namespace Moon.Communication.Packets.Incoming.Catalog
{
    public class PurchaseFromCatalogEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (MoonEnvironment.GetDBConfig().DBData["catalogue_enabled"] != "1")
            {
                Session.SendNotification("The hotel managers have disabled the catalogue");
                return;
            }
            
            int PageId = Packet.PopInt();
            int ItemId = Packet.PopInt();
            string ExtraData = Packet.PopString();
            int Amount = Packet.PopInt();

            CatalogPage Page = null;
            if (!MoonEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out Page))
                return;
            if (Session.GetHabbo().Rank > 3 && !Session.GetHabbo().StaffOk || MoonStaticGameSettings.IsGoingToBeClose)
                return;
            if (!Page.Enabled || !Page.Visible || Page.MinimumRank > Session.GetHabbo().Rank || (Page.MinimumVIP > Session.GetHabbo().VIPRank && Session.GetHabbo().Rank == 1))
                return;
            
            CatalogItem Item = null; 
            bool ValidItem = true;

            if (!Page.Items.TryGetValue(ItemId, out Item))
            {
                if (Page.ItemOffers.ContainsKey(ItemId))
                {
                    Item = (CatalogItem)Page.ItemOffers[ItemId];
                    if (Item == null)
                        ValidItem = false;
                }
                else
                    ValidItem = false;
            }

            if (!ValidItem)
            {
                Console.WriteLine("Catalog cant load item:" + Item.Data.PublicName);
                return;
            }

            ItemData baseItem = Item.GetBaseItem(Item.ItemId);
            if (baseItem != null)
            {
                if (baseItem.InteractionType == InteractionType.club_1_month || baseItem.InteractionType == InteractionType.club_3_month || baseItem.InteractionType == InteractionType.club_6_month)
                {
                    if (Item.CostCredits > Session.GetHabbo().Credits)
                        return;

                    int Months = 0;

                    switch (baseItem.InteractionType)
                    {
                        case InteractionType.club_1_month:
                            Months = 1;
                            break;

                        case InteractionType.club_3_month:
                            Months = 3;
                            break;

                        case InteractionType.club_6_month:
                            Months = 6;
                            break;
                    }

                    int num = num = 31 * Months;

                    if (Item.CostCredits > 0)
                    {
                        Session.GetHabbo().Credits -= Item.CostCredits;
                        Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));

                    }

                    Session.GetHabbo().GetClubManager().AddOrExtendSubscription("habbo_vip", num * 24 * 3600, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("HC1", true, Session);

                    Session.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));
                    Session.SendMessage(new PurchaseOKComposer(Item, Item.Data));
                    Session.SendMessage(new FurniListUpdateComposer());
                    return;
                }

                if (baseItem.InteractionType == InteractionType.namecolor)
                {
                    if (Item.CostGOTWPoints > Session.GetHabbo().GOTWPoints)
                        return;

                    if (Item.CostGOTWPoints > 0)
                    {
                        Session.GetHabbo().GOTWPoints -= Item.CostGOTWPoints;
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, 0, 103));
                    }

                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE `users` SET `namecolor` = '" + Item.Name + "' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                    }

                    Session.GetHabbo()._nameColor = Item.Name;
                    Session.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));
                    Session.SendMessage(new PurchaseOKComposer(Item, Item.Data));
                    Session.SendMessage(new FurniListUpdateComposer());
                    return;
                }

                if (baseItem.InteractionType == InteractionType.changename)
                {
                    if (Item.CostDiamonds > Session.GetHabbo().Diamonds)
                        return;

                    if (Item.CostDiamonds > 0)
                    {
                        Session.GetHabbo().Diamonds -= Item.CostDiamonds;
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 5));
                    }

                    Session.GetHabbo().LastNameChange = 0;
                    Session.GetHabbo().ChangingName = true;
                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE users SET changename = '1' WHERE id = " + Session.GetHabbo().Id + "");
                    }

                    Session.GetHabbo()._changename = 1;
                    Session.SendMessage(new UserObjectComposer(Session.GetHabbo()));

                    Session.SendWhisper("Acabas de adquirir un cambio de nombre, haz click encima tuyo y cámbialo.", 34);

                    Session.SendMessage(new FurniListUpdateComposer());
                    Session.SendMessage(new PurchaseOKComposer(Item, Item.Data));
                    return;
                }



                if (baseItem.InteractionType == InteractionType.tag)
                {
                    if (ExtraData.Length == 0 || ExtraData.Length > 8 || ExtraData.Length < 0)
                    {
                        Session.SendMessage(RoomNotificationComposer.SendBubble("advice", "Oops, o seu prefixo só pode conter de 1 a 7 caracteres.", ""));
                        return;
                    }
                    else if (ExtraData.Contains("<br>") || ExtraData.Contains("<b>") || ExtraData.Contains("<i>") || ExtraData.Contains("<Br>") || ExtraData.Contains("<BR>") ||
                        ExtraData.Contains("<bR>") || ExtraData.Contains("<B>") || ExtraData.Contains("<I>"))
                    {
                        Session.SendMessage(RoomNotificationComposer.SendBubble("advice", "Oops, não pode usar TAG HTML nos prefixos.", ""));
                        return;
                    }

                    else if (ExtraData.ToUpper().Contains("MNG") || ExtraData.ToUpper().Contains("BOT") || ExtraData.ToUpper().Contains("B0T") || ExtraData.ToUpper().Contains("BAW") || ExtraData.ToUpper().Contains("CLB") || ExtraData.ToUpper().Contains("GUIA") || ExtraData.ToUpper().Contains("INTER") ||
                        ExtraData.ToUpper().Contains("INT") || ExtraData.ToUpper().Contains("EDC") || ExtraData.ToUpper().Contains("VIP") || ExtraData.ToUpper().Contains("EDP") ||
                        ExtraData.ToUpper().Contains("ADM") || ExtraData.ToUpper().Contains("MOD") || ExtraData.ToUpper().Contains("M0D") || ExtraData.ToUpper().Contains("STAFF") ||
                        ExtraData.ToUpper().Contains("0WNER") || ExtraData.ToUpper().Contains("OWNER") || ExtraData.ToUpper().Contains("GM") || ExtraData.ToUpper().Contains("EDM") ||
                        ExtraData.ToUpper().Contains("ROOKIE") || ExtraData.ToUpper().Contains("R00KIE") || ExtraData.ToUpper().Contains("BAW") || ExtraData.ToUpper().Contains("HFM") || ExtraData.ToUpper().Contains("\r"))
                    {
                        Session.SendNotification("Oops, não pode colocar tag staff!");
                        return;
                    }

                    if (!MoonEnvironment.IsValidAlphaNumeric(ExtraData))
                    {
                        Session.SendMessage(RoomNotificationComposer.SendBubble("advice", "Recuerda, que tu prefijo que tu prefijo no puede tener caracteres molestos.", ""));
                        return;
                    }

                    string character;
                    ExtraData = MoonEnvironment.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(ExtraData, out character) ? "" : ExtraData;

                    if (string.IsNullOrEmpty(ExtraData))
                    {
                        Session.SendNotification("Lo sentimos, pero la plabara " + character + " no es una palabra permitida dentro de los filtros de " + MoonEnvironment.GetDBConfig().DBData["hotel.name"] + ", por ello no puedes ponerlo");
                        return;
                    }


                    if (Item.CostCredits > Session.GetHabbo().Credits || Item.CostPixels > Session.GetHabbo().Duckets || Item.CostDiamonds > Session.GetHabbo().Diamonds || Item.CostGOTWPoints > Session.GetHabbo().GOTWPoints)
                        return;

                    if (Item.CostCredits > 0)
                    {
                        Session.GetHabbo().Credits -= Item.CostCredits;
                        Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                    }

                    if (Item.CostPixels > 0)
                    {
                        Session.GetHabbo().Duckets -= Item.CostPixels;
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Duckets));//Love you, Tom.
                    }

                    if (Item.CostDiamonds > 0)
                    {
                        Session.GetHabbo().Diamonds -= Item.CostDiamonds;
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 5));
                    }

                    if (Item.CostGOTWPoints > 0)
                    {
                        Session.GetHabbo().GOTWPoints -= Item.CostGOTWPoints;
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, 0, 103));
                    }


                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE `users` SET `tag` = '[" + ExtraData + "]' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                    }

                    Session.GetHabbo()._tag = "[" + ExtraData + "]";
                    Session.SendMessage(new AlertNotificationHCMessageComposer(4));
                    Session.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));
                    Session.SendMessage(new PurchaseOKComposer(Item, Item.Data));
                    Session.SendMessage(new FurniListUpdateComposer());
                    return;
                }

                if (baseItem.InteractionType == InteractionType.tag_vip)
                {

                    if (Item.CostCredits > Session.GetHabbo().Credits || Item.CostPixels > Session.GetHabbo().Duckets || Item.CostDiamonds > Session.GetHabbo().Diamonds || Item.CostGOTWPoints > Session.GetHabbo().GOTWPoints)
                        return;

                    if (Item.CostCredits > 0)
                    {
                        Session.GetHabbo().Credits -= Item.CostCredits;
                        Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                    }

                    if (Item.CostPixels > 0)
                    {
                        Session.GetHabbo().Duckets -= Item.CostPixels;
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Duckets));//Love you, Tom.
                    }

                    if (Item.CostDiamonds > 0)
                    {
                        Session.GetHabbo().Diamonds -= Item.CostDiamonds;
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 5));
                    }

                    if (Item.CostGOTWPoints > 0)
                    {
                        Session.GetHabbo().GOTWPoints -= Item.CostGOTWPoints;
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, 0, 103));
                    }


                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE `users` SET `tag` = '" + Item.ExtraData + "' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                    }

                    Session.GetHabbo()._tag = Item.ExtraData;
                    Session.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));
                    Session.SendMessage(new PurchaseOKComposer(Item, Item.Data));
                    Session.SendMessage(new FurniListUpdateComposer());
                    return;
                }


                if (baseItem.InteractionType == InteractionType.tagcolor)
                    {
                    if (Item.CostGOTWPoints > Session.GetHabbo().GOTWPoints)
                        return;

                    if (Item.CostGOTWPoints > 0)
                    {
                        Session.GetHabbo().GOTWPoints -= Item.CostGOTWPoints;
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, 0, 103));
                    }

                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE `users` SET `tagcolor` = '" + Item.Name + "' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                    }

                    Session.GetHabbo()._tagcolor = Item.Name;
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, 0, 103));
                    Session.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));
                    Session.SendMessage(new PurchaseOKComposer(Item, Item.Data));
                    return;
                }

                if (baseItem.InteractionType == InteractionType.tagrainbow)
                {
                    if (Item.CostDiamonds > Session.GetHabbo().Diamonds)
                        return;

                    if (Item.CostDiamonds > 0)
                    {
                        Session.GetHabbo().Diamonds -= Item.CostDiamonds;
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 103));
                    }

                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE `users` SET `tagcolor` = '" + Item.Name + "' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                    }

                    Session.GetHabbo()._tagcolor = Item.Name;
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 5));
                    Session.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));
                    Session.SendMessage(new PurchaseOKComposer(Item, Item.Data));
                    Session.SendMessage(new FurniListUpdateComposer());
                    return;
                }


                if (baseItem.InteractionType == InteractionType.club_vip || baseItem.InteractionType == InteractionType.club_vip2)
                {
                    if (Item.CostDiamonds > Session.GetHabbo().Diamonds)
                        return;

                    int Months = 0;

                    switch (baseItem.InteractionType)
                    {
                        case InteractionType.club_vip:
                            Months = 1;
                            break;

                        case InteractionType.club_vip2:
                            Months = 3;
                            break;
                    }

                    Session.GetHabbo().Diamonds -= Item.CostDiamonds;
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 5));

                    var IsVIP = Session.GetHabbo().GetClubManager().HasSubscription("club_vip");
                    if (IsVIP)
                    {
                        Session.SendMessage(new AlertNotificationHCMessageComposer(4));
                    }
                    else
                    {
                        Session.SendMessage(new AlertNotificationHCMessageComposer(5));
                    }

                    Session.GetHabbo().GetClubManager().AddOrExtendSubscription("club_vip", 1 * 24 * 3600, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("DVIP", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("ACH_VipClub12", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("ES28A", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("ES551", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("BR967", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("DE720", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("BR415", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("shop", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("PT054", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("PX4", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("PX3", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("UK277", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("THI95", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("NL185", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("NL537", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("ES720", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("ES78A", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("HST27", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("ROOMP", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("ES800", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("ROOMP", true, Session);
                    Session.GetHabbo().GetBadgeComponent().GiveBadge("ES679", true, Session);

                    Session.SendMessage(RoomNotificationComposer.SendBubble("shop", "" + Session.GetHabbo().Username + ", usted ha recibido un combo de placa por unirse al Club VIP de Jabboz.", ""));


                    MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_VipClub", 1);
                    Session.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));
                    Session.SendMessage(new PurchaseOKComposer(Item, Item.Data));
                    Session.SendMessage(new FurniListUpdateComposer());

                    if (Session.GetHabbo().Rank > 2)
                    {
                        Session.SendWhisper("Men, tienes rango, qué coño ases comprando el vip xd.");
                        return;
                    }

                    else if (Session.GetHabbo().Rank < 2)
                    {
                        using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunQuery("UPDATE `users` SET `rank` = '2' WHERE `id` = '" + Session.GetHabbo().Id + "'");
                            dbClient.RunQuery("UPDATE `users` SET `rank_vip` = '1' WHERE `id` = '" + Session.GetHabbo().Id + "'");
                            dbClient.RunQuery("UPDATE `users` SET `respetos` = '5' WHERE `id` = '" + Session.GetHabbo().Id + "'");
                            dbClient.RunQuery("UPDATE `users` SET `loto_clicks` = '2' WHERE `id` = '" + Session.GetHabbo().Id + "'");
                            Session.GetHabbo().Rank = 2;
                            Session.GetHabbo().VIPRank = 1;
                        }
                    }

                    return;
                }
            }

            if (Amount < 1 || Amount > 100)
                Amount = 1;

            int AmountPurchase = Item.Amount > 1 ? Item.Amount : Amount;
            int TotalCreditsCost = Amount > 1 ? ((Item.CostCredits * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostCredits)) : Item.CostCredits;
            int TotalPixelCost = Amount > 1 ? ((Item.CostPixels * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostPixels)) : Item.CostPixels;
            int TotalDiamondCost = Amount > 1 ? ((Item.CostDiamonds * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostDiamonds)) : Item.CostDiamonds;
            int TotalGOTWPointsCost = Amount > 1 ? ((Item.CostGOTWPoints * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostGOTWPoints)) : Item.CostGOTWPoints;
            //int TotalPumpkinsCost = Amount > 1 ? ((Item.CostPumpkins * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostPumpkins)) : Item.CostPumpkins;

            if (Session.GetHabbo().Credits < TotalCreditsCost || Session.GetHabbo().Duckets < TotalPixelCost || Session.GetHabbo().Diamonds < TotalDiamondCost || Session.GetHabbo().GOTWPoints < TotalGOTWPointsCost)
                return;

            int LimitedEditionSells = 0;
            int LimitedEditionStack = 0;


            if (Item.IsLimited)
            {
                if (Item.LimitedEditionStack <= Item.LimitedEditionSells)
                {
                    Session.SendMessage(new LTDSoldAlertComposer());
                    Session.SendMessage(new CatalogUpdatedComposer());
                    Session.SendMessage(new PurchaseOKComposer());
                    return;
                }

                Item.LimitedEditionSells++;
                MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_LTDPurchased", 1);

                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `catalog_items` SET `limited_sells` = '" + Item.LimitedEditionSells + "' WHERE `id` = '" + Item.Id + "' LIMIT 1");
                    LimitedEditionSells = Item.LimitedEditionSells;
                    LimitedEditionStack = Item.LimitedEditionStack;
                }
            }



            if (Item.CostCredits > 0)
            {
                Session.GetHabbo().Credits -= TotalCreditsCost;
                Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
            }

            if (Item.CostPixels > 0)
            {
                Session.GetHabbo().Duckets -= TotalPixelCost;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Duckets));//Love you, Tom.
            }

            if (Item.CostDiamonds > 0)
            {
                Session.GetHabbo().Diamonds -= TotalDiamondCost;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 5));
            }

            if (Item.CostGOTWPoints > 0)
            {
                Session.GetHabbo().GOTWPoints -= TotalGOTWPointsCost;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, 0, 103));
            }


            #region PREDESIGNED_ROOM BY KOMOK
            if (Item.PredesignedId > 0 && MoonEnvironment.GetGame().GetCatalog().GetPredesignedRooms().predesignedRoom.ContainsKey((uint)Item.PredesignedId))
            {
                #region SELECT ROOM AND CREATE NEW
                var predesigned = MoonEnvironment.GetGame().GetCatalog().GetPredesignedRooms().predesignedRoom[(uint)Item.PredesignedId];
                var decoration = predesigned.RoomDecoration;

                var createRoom = MoonEnvironment.GetGame().GetRoomManager().CreateRoom(Session, Session.GetHabbo().Username + "'s room", "¡Una Sala pre-decorada!", predesigned.RoomModel, 1, 25, 1);

                createRoom.FloorThickness = int.Parse(decoration[0]);
                createRoom.WallThickness = int.Parse(decoration[1]);
                createRoom.Model.WallHeight = int.Parse(decoration[2]);
                createRoom.Hidewall = ((decoration[3] == "True") ? 1 : 0);
                createRoom.Wallpaper = decoration[4];
                createRoom.Landscape = decoration[5];
                createRoom.Floor = decoration[6];
                var newRoom = MoonEnvironment.GetGame().GetRoomManager().LoadRoom(createRoom.Id);
                #endregion

                #region CREATE FLOOR ITEMS
                if (predesigned.FloorItems != null)
                    foreach (var floorItems in predesigned.FloorItemData)
                        using (var dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                            dbClient.runFastQuery("INSERT INTO items (id, user_id, room_id, base_item, extra_data, x, y, z, rot, wall_pos, limited_number, limited_stack) VALUES (null, " + Session.GetHabbo().Id + ", " + newRoom.RoomId + ", " + floorItems.BaseItem + ", '" + floorItems.ExtraData + "', " +
                                floorItems.X + ", " + floorItems.Y + ", " + TextHandling.GetString(floorItems.Z) + ", " + floorItems.Rot + ", '', 0, 0);");
                #endregion

                #region CREATE WALL ITEMS
                if (predesigned.WallItems != null)
                    foreach (var wallItems in predesigned.WallItemData)
                        using (var dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                            dbClient.runFastQuery("INSERT INTO items (id, user_id, room_id, base_item, extra_data, x, y, z, rot, wall_pos, limited_number, limited_stack) VALUES (null, " + Session.GetHabbo().Id + ", " + newRoom.RoomId + ", " + wallItems.BaseItem + ", '" + wallItems.ExtraData +
                                "', 0, 0, 0, 0, '" + wallItems.WallCoord + "', 0, 0);");
                #endregion

                #region VERIFY IF CONTAINS BADGE AND GIVE
                if (Item.Badge != string.Empty) Session.GetHabbo().GetBadgeComponent().GiveBadge(Item.Badge, true, Session);
                #endregion

                #region GENERATE ROOM AND SEND PACKET
                Session.SendMessage(new PurchaseOKComposer());
                Session.GetHabbo().GetInventoryComponent().UpdateItems(false);
                MoonEnvironment.GetGame().GetRoomManager().LoadRoom(newRoom.Id).GetRoomItemHandler().LoadFurniture();
                var newFloorItems = newRoom.GetRoomItemHandler().GetFloor;
                foreach (var roomItem in newFloorItems) newRoom.GetRoomItemHandler().SetFloorItem(roomItem, roomItem.GetX, roomItem.GetY, roomItem.GetZ);
                var newWallItems = newRoom.GetRoomItemHandler().GetWall;
                foreach (var roomItem in newWallItems) newRoom.GetRoomItemHandler().SetWallItem(Session, roomItem);
                Session.SendMessage(new FlatCreatedComposer(newRoom.Id, newRoom.Name));
                #endregion
                return;
            }
            #endregion

            #region Create the extradata
            switch (Item.Data.InteractionType)
            {
                case InteractionType.NONE:
                    ExtraData = "";
                    break;



                case InteractionType.GUILD_CHAT:
                    Group thegroup;
                    if (!MoonEnvironment.GetGame().GetGroupManager().TryGetGroup(Convert.ToInt32(ExtraData), out thegroup))
                        return;
                    if (!(MoonEnvironment.GetGame().GetGroupManager().GetGroupsForUser(Session.GetHabbo().Id).Contains(thegroup)))
                    {
                        return;
                    }

                    int groupID = Convert.ToInt32(ExtraData);
                    if (thegroup.CreatorId == Session.GetHabbo().Id)
                    {
                        thegroup.CreateGroupChat(thegroup);

                    }
                    else if (thegroup.CreatorId != Session.GetHabbo().Id)
                    {
                        Session.SendNotification("Solo el dueño del grupo puede comprar esto");
                        return;
                    }
                    ExtraData = "" + groupID;


                    break;

                case InteractionType.GUILD_FORUM:
                    Group Gp;
                    GroupForum Gf;
                    int GpId;
                    if (!int.TryParse(ExtraData, out GpId))
                    {
                        Session.SendNotification("Oopss! Some error when getting the group ID...");
                        Session.SendMessage(new PurchaseOKComposer());
                        return;
                    }
                    if (!MoonEnvironment.GetGame().GetGroupManager().TryGetGroup(GpId, out Gp))
                    {
                        Session.SendNotification("Error! this group doesn't exists");
                        Session.SendMessage(new PurchaseOKComposer());
                        return;
                    }

                    if (Gp.CreatorId != Session.GetHabbo().Id)
                    {
                        Session.SendNotification("¡Error! No eres el dueño del grupo así que no puedes crear el foro.\n\nPrimero el foro debe ser creado por el dueño del grupo...");
                        Session.SendMessage(new PurchaseOKComposer());
                        return;
                    }
                    Gf = MoonEnvironment.GetGame().GetGroupForumManager().CreateGroupForum(Gp);
                    Session.SendMessage(new RoomNotificationComposer("forums.delivered", new Dictionary<string, string>
                            { { "groupId", Gp.Id.ToString() },  { "groupName", Gp.Name } }));
                    break;

                case InteractionType.GUILD_ITEM:
                case InteractionType.GUILD_GATE:
                case InteractionType.HCGATE:
                case InteractionType.VIPGATE:
                    break;



                case InteractionType.PINATA:
                case InteractionType.PLANT_SEED:
                case InteractionType.PINATATRIGGERED:
                case InteractionType.MAGICEGG:
                    ExtraData = "0";
                    break;

                case InteractionType.FOOTBALL_GATE:
                    ExtraData = "hd-180-14.ch-210-1408.lg-270-1408,hd-600-14.ch-630-1408.lg-695-1408";
                    break;


                #region Pet handling

                case InteractionType.pet0:
                case InteractionType.pet1:
                case InteractionType.pet2:
                case InteractionType.pet3:
                case InteractionType.pet4:
                case InteractionType.pet5:
                case InteractionType.pet6:
                case InteractionType.pet7:
                case InteractionType.pet8:
                case InteractionType.pet9:
                case InteractionType.pet10:
                case InteractionType.pet11:
                case InteractionType.pet12:
                case InteractionType.pet13: //Caballo
                case InteractionType.pet14:
                case InteractionType.pet15:
                case InteractionType.pet16: //Mascota agregada
                case InteractionType.pet17: //Mascota agregada
                case InteractionType.pet18: //Mascota agregada
                case InteractionType.pet19: //Mascota agregada
                case InteractionType.pet20: //Mascota agregada
                case InteractionType.pet21: //Mascota agregada
                case InteractionType.pet22: //Mascota agregada
                case InteractionType.pet23:
                case InteractionType.pet24:
                case InteractionType.pet25:
                case InteractionType.pet26:
                case InteractionType.pet28:
                case InteractionType.pet29:
                case InteractionType.pet30:
                case InteractionType.pet31:
                case InteractionType.pet32:
                case InteractionType.pet33:
                case InteractionType.pet34:
                case InteractionType.pet35:
                case InteractionType.pet36:
                case InteractionType.pet37:
                case InteractionType.pet38:
                case InteractionType.pet39:
                case InteractionType.pet40:
                case InteractionType.pet41:
                case InteractionType.pet42:
                case InteractionType.pet43:
                    try
                    {
                        string[] Bits = ExtraData.Split('\n');
                        string PetName = Bits[0];
                        string Race = Bits[1];
                        string Color = Bits[2];

                        int.Parse(Race); // to trigger any possible errors

                        if (!PetUtility.CheckPetName(PetName))
                            return;

                        if (Race.Length > 2)
                            return;

                        if (Color.Length != 6)
                            return;

                        MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_PetLover", 1);
                    }
                    catch (Exception e)
                    {
                        Logging.LogException(e.ToString());
                        return;
                    }

                    break;

                #endregion

                case InteractionType.FLOOR:
                case InteractionType.WALLPAPER:
                case InteractionType.LANDSCAPE:

                    Double Number = 0;

                    try
                    {
                        if (string.IsNullOrEmpty(ExtraData))
                            Number = 0;
                        else
                            Number = Double.Parse(ExtraData, MoonEnvironment.CultureInfo);
                    }
                    catch (Exception e)
                    {
                        Logging.HandleException(e, "Catalog.HandlePurchase: " + ExtraData);
                    }

                    ExtraData = Number.ToString().Replace(',', '.');
                    break; // maintain extra data // todo: validate

                case InteractionType.POSTIT:
                    ExtraData = "FFFF33";
                    break;

                case InteractionType.MOODLIGHT:
                    ExtraData = "1,1,1,#000000,255";
                    break;

                case InteractionType.TROPHY:
                    ExtraData = Session.GetHabbo().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + Convert.ToChar(9) + ExtraData;
                    break;

                case InteractionType.MANNEQUIN:
                    ExtraData = "m" + Convert.ToChar(5) + ".ch-210-1321.lg-285-92" + Convert.ToChar(5) + "Default Maniqui";
                    break;

                case InteractionType.MUSIC_DISC:
                    ExtraData = Item.ExtraData;
                    break;

                case InteractionType.BADGE_DISPLAY:
                    if (!Session.GetHabbo().GetBadgeComponent().HasBadge(ExtraData))
                    {
                        Session.SendMessage(new BroadcastMessageAlertComposer("Oops, al parecer no tienes esta placa"));
                        return;
                    }

                    ExtraData = ExtraData + Convert.ToChar(9) + Session.GetHabbo().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
                    break;

                case InteractionType.BADGE:
                    {
                        if (Session.GetHabbo().GetBadgeComponent().HasBadge(Item.Data.ItemName))
                        {
                            Session.SendMessage(new PurchaseErrorComposer(1));
                            return;
                        }
                        break;
                    }
                default:
                    ExtraData = "";
                    break;
            }
            #endregion

            Item NewItem = null;
            switch (Item.Data.Type.ToString().ToLower())
            {
                default:
                    List<Item> GeneratedGenericItems = new List<Item>();

                    switch (Item.Data.InteractionType)
                    {
                        default:
                            if (AmountPurchase > 1)
                            {
                                List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, Session.GetHabbo(), ExtraData, AmountPurchase);

                                if (Items != null)
                                {
                                    GeneratedGenericItems.AddRange(Items);
                                    Session.SendMessage(RoomNotificationComposer.SendBubble("icon/" + Item.Data.ItemName + "_icon", "Você acaba de comprar " + Item.Data.PublicName + "", "inventory/open/furni"));
                                }
                            }
                            else
                            {
                                NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, Session.GetHabbo(), ExtraData, ExtraData, 0, LimitedEditionSells, LimitedEditionStack);

                                if (NewItem != null)
                                {
                                    GeneratedGenericItems.Add(NewItem);
                                    Session.SendMessage(RoomNotificationComposer.SendBubble("icon/" + Item.Data.ItemName + "_icon", "Você acaba de comprar " + Item.Data.PublicName + "", "inventory/open/furni"));
                                }
                            }
                            break;

                        case InteractionType.GUILD_GATE:
                        case InteractionType.GUILD_ITEM:
                        case InteractionType.GUILD_FORUM:
                            if (AmountPurchase > 1)
                            {

                                List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, Session.GetHabbo(), ExtraData, AmountPurchase, Convert.ToInt32(ExtraData));

                                if (Items != null)
                                {
                                    GeneratedGenericItems.AddRange(Items);
                                }
                            }
                            else
                            {
                                NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, Session.GetHabbo(), ExtraData, ExtraData, Convert.ToInt32(ExtraData));

                                if (NewItem != null)
                                {
                                    GeneratedGenericItems.Add(NewItem);
                                    Session.SendMessage(RoomNotificationComposer.SendBubble("icon/" + Item.Data.ItemName + "_icon", "Você acaba de comprar " + Item.Data.PublicName + "", "inventory/open/furni"));
                                }
                            }
                            break;

                        case InteractionType.ARROW:
                        case InteractionType.TELEPORT:
                            for (int i = 0; i < AmountPurchase; i++)
                            {
                                List<Item> TeleItems = ItemFactory.CreateTeleporterItems(Item.Data, Session.GetHabbo());

                                if (TeleItems != null)
                                {
                                    GeneratedGenericItems.AddRange(TeleItems);
                                }
                            }
                            break;

                        case InteractionType.MOODLIGHT:
                            {
                                if (AmountPurchase > 1)
                                {
                                    List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, Session.GetHabbo(), ExtraData, AmountPurchase);

                                    if (Items != null)
                                    {
                                        GeneratedGenericItems.AddRange(Items);
                                        foreach (Item I in Items)
                                        {
                                            ItemFactory.CreateMoodlightData(I);
                                        }
                                    }
                                }
                                else
                                {
                                    NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, Session.GetHabbo(), ExtraData, ExtraData);

                                    if (NewItem != null)
                                    {
                                        GeneratedGenericItems.Add(NewItem);
                                        ItemFactory.CreateMoodlightData(NewItem);
                                    }
                                }
                            }
                            break;

                        case InteractionType.reward_box:
                            {
                                string ED = Session.GetHabbo().Username + Convert.ToChar(5) + "Has escogido un cofre de Legado de nivel común, puedes obtener rares que ponderan entre los 0 y 12 vips. Recuerda que sólo están disponibles durante cierto tiempo." + Convert.ToChar(5) + Session.GetHabbo().Id + Convert.ToChar(5) + Item.Data.Id + Convert.ToChar(5) + 206 + Convert.ToChar(5) + 1 + Convert.ToChar(5) + 1;
                                ExtraData = ED;
                                int NewItemId = 0;

                                int Reward = RandomNumber.GenerateRandom(1, 10);
                                #region Rewards
                                switch (Reward)
                                {
                                    case 1:
                                        Reward = 9501; // Humadera Azul Colorable - rare_colourable_scifirocket*1
                                        break;
                                    case 2:
                                        Reward = 9510; // Elefante Azul Colorable - rare_colourable_elephant_statue*1
                                        break;
                                    case 3:
                                        Reward = 1587; // Lámpara Calippo - ads_calip_lava
                                        break;
                                    case 4:
                                        Reward = 540004; // Alce Fiel - loyalty_elk
                                        break;
                                    case 5:
                                        Reward = 385; // Toldo Amarillo - marquee*4
                                        break;
                                    case 6:
                                        Reward = 9502; // Fontana Azul - rare_colourable_fountain*1
                                        break;
                                    case 7:
                                        Reward = 212; // VIP - club_sofa
                                        break;
                                    case 8:
                                        Reward = 9506; // Parasol Azul - rare_colourable_parasol*1
                                        break;
                                    case 9:
                                        Reward = 9514; // Puerta Laser Azul - rare_colourable_scifiport*1
                                        break;
                                    case 10:
                                        Reward = 353; // Humadera Rosa - scifirocket*4
                                        break;
                                }
                                #endregion

                                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("INSERT INTO `items` (`base_item`,`user_id`,`extra_data`) VALUES (9377, '" + Session.GetHabbo().Id + "', @extra_data)");
                                    dbClient.AddParameter("extra_data", ED);
                                    NewItemId = Convert.ToInt32(dbClient.InsertQuery());

                                    dbClient.SetQuery("INSERT INTO `user_presents` (`item_id`,`base_id`,`extra_data`) VALUES ('" + NewItemId + "', '" + Reward + "', @extra_data)");
                                    dbClient.AddParameter("extra_data", (string.IsNullOrEmpty(ExtraData) ? "" : ExtraData));
                                    dbClient.RunQuery();
                                }

                                Session.GetHabbo().GetInventoryComponent().UpdateItems(true);

                                break;
                            }

                        case InteractionType.reward_box2:
                            {
                                string ED = Session.GetHabbo().Username + Convert.ToChar(5) + "Has escogido un cofre de Legado de nivel épico, puedes obtener rares que ponderan entre los 0 y 12 vips. Recuerda que sólo están disponibles durante cierto tiempo." + Convert.ToChar(5) + Session.GetHabbo().Id + Convert.ToChar(5) + Item.Data.Id + Convert.ToChar(5) + 206 + Convert.ToChar(5) + 1 + Convert.ToChar(5) + 1;
                                ExtraData = ED;
                                int NewItemId = 0;

                                int Reward = RandomNumber.GenerateRandom(1, 10);
                                #region Rewards
                                switch (Reward)
                                {
                                    case 1:
                                        Reward = 9501; // Humadera Azul Colorable - rare_colourable_scifirocket*1
                                        break;
                                    case 2:
                                        Reward = 9510; // Elefante Azul Colorable - rare_colourable_elephant_statue*1
                                        break;
                                    case 3:
                                        Reward = 1587; // Lámpara Calippo - ads_calip_lava
                                        break;
                                    case 4:
                                        Reward = 540004; // Alce Fiel - loyalty_elk
                                        break;
                                    case 5:
                                        Reward = 385; // Toldo Amarillo - marquee*4
                                        break;
                                    case 6:
                                        Reward = 9502; // Fontana Azul - rare_colourable_fountain*1
                                        break;
                                    case 7:
                                        Reward = 212; // VIP - club_sofa
                                        break;
                                    case 8:
                                        Reward = 9506; // Parasol Azul - rare_colourable_parasol*1
                                        break;
                                    case 9:
                                        Reward = 9514; // Puerta Laser Azul - rare_colourable_scifiport*1
                                        break;
                                    case 10:
                                        Reward = 353; // Humadera Rosa - scifirocket*4
                                        break;
                                }
                                #endregion

                                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("INSERT INTO `items` (`base_item`,`user_id`,`extra_data`) VALUES (9378, '" + Session.GetHabbo().Id + "', @extra_data)");
                                    dbClient.AddParameter("extra_data", ED);
                                    NewItemId = Convert.ToInt32(dbClient.InsertQuery());

                                    dbClient.SetQuery("INSERT INTO `user_presents` (`item_id`,`base_id`,`extra_data`) VALUES ('" + NewItemId + "', '" + Reward + "', @extra_data)");
                                    dbClient.AddParameter("extra_data", (string.IsNullOrEmpty(ExtraData) ? "" : ExtraData));
                                    dbClient.RunQuery();
                                }

                                Session.GetHabbo().GetInventoryComponent().UpdateItems(true);

                                break;
                            }

                        case InteractionType.TONER:
                            {
                                if (AmountPurchase > 1)
                                {
                                    List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, Session.GetHabbo(), ExtraData, AmountPurchase);

                                    if (Items != null)
                                    {
                                        GeneratedGenericItems.AddRange(Items);
                                        Session.SendMessage(RoomNotificationComposer.SendBubble("icon/" + Item.Data.ItemName + "_icon", "Você acaba de comprar " + Item.Data.PublicName + "", "inventory/open/furni"));
                                        foreach (Item I in Items)
                                        {
                                            ItemFactory.CreateTonerData(I);

                                        }
                                    }
                                }
                                else
                                {
                                    NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, Session.GetHabbo(), ExtraData, ExtraData);

                                    if (NewItem != null)
                                    {
                                        GeneratedGenericItems.Add(NewItem);
                                        Session.SendMessage(RoomNotificationComposer.SendBubble("icon/" + Item.Data.ItemName + "_icon", "Você acaba de comprar " + Item.Data.PublicName + "", "inventory/open/furni"));
                                        ItemFactory.CreateTonerData(NewItem);
                                    }
                                }
                            }
                            break;

                        case InteractionType.DEAL:
                            {
                                var DealItems = (from d in Page.Deals.Values.ToList() where d.Id == Item.Id select d);
                                foreach (CatalogDeal DealItem in DealItems.ToList())
                                {
                                    foreach (CatalogItem CatalogItem in DealItem.ItemDataList.ToList())
                                    {
                                        List<Item> Items = ItemFactory.CreateMultipleItems(CatalogItem.Data, Session.GetHabbo(), "", CatalogItem.Amount);

                                        if (Items != null)
                                        {

                                            GeneratedGenericItems.AddRange(Items);
                                        }
                                    }
                                }
                            }
                            break;
                    }

                    foreach (Item PurchasedItem in GeneratedGenericItems)
                    {
                        if (Session.GetHabbo().GetInventoryComponent().TryAddItem(PurchasedItem))
                        {
                            Session.SendMessage(new FurniListNotificationComposer(PurchasedItem.Id, 1));
                        }
                    }

                    break;

                case "e":
                    AvatarEffect Effect = null;

                    if (Session.GetHabbo().Effects().HasEffect(Item.Data.SpriteId))
                    {
                        Effect = Session.GetHabbo().Effects().GetEffectNullable(Item.Data.SpriteId);

                        if (Effect != null)
                        {
                            Effect.AddToQuantity();
                        }
                    }
                    else
                        Effect = AvatarEffectFactory.CreateNullable(Session.GetHabbo(), Item.Data.SpriteId, 3600);

                    if (Effect != null)// && Session.GetHabbo().Effects().TryAdd(Effect))
                    {
                        Session.SendMessage(new AvatarEffectAddedComposer(Item.Data.SpriteId, 3600));
                    }
                    break;

                case "r":
                    Bot Bot = BotUtility.CreateBot(Item.Data, Session.GetHabbo().Id);
                    if (Bot != null)
                    {
                        Session.GetHabbo().GetInventoryComponent().TryAddBot(Bot);
                        Session.SendMessage(new BotInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetBots()));
                        Session.SendMessage(new FurniListNotificationComposer(Bot.Id, 5));
                    }
                    else
                        Session.SendNotification("Oops! Ha ocurrido un error mientras comprabas el Bot, al parecer no hay datos acerca de el, Reportalo!!");
                    break;

                case "b":
                    {
                        Session.GetHabbo().GetBadgeComponent().GiveBadge(Item.Data.ItemName, true, Session);
                        Session.SendMessage(new FurniListNotificationComposer(0, 4));
                        break;
                    }

                case "p":
                    {
                        switch (Item.Data.InteractionType)
                        {

                            #region Pets
                            #region Pet 0
                            case InteractionType.pet0:
                                string[] PetData = ExtraData.Split('\n');
                                Pet GeneratedPet = PetUtility.CreatePet(Session.GetHabbo().Id, PetData[0], 0, PetData[1], PetData[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet);

                                break;
                            #endregion
                            #region Pet 1
                            case InteractionType.pet1:
                                string[] PetData1 = ExtraData.Split('\n');
                                Pet GeneratedPet1 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData1[0], 1, PetData1[1], PetData1[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet1);

                                break;
                            #endregion
                            #region Pet 2
                            case InteractionType.pet2:
                                string[] PetData5 = ExtraData.Split('\n');
                                Pet GeneratedPet5 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData5[0], 2, PetData5[1], PetData5[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet5);

                                break;
                            #endregion
                            #region Pet 3
                            case InteractionType.pet3:
                                string[] PetData2 = ExtraData.Split('\n');
                                Pet GeneratedPet2 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData2[0], 3, PetData2[1], PetData2[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet2);

                                break;
                            #endregion
                            #region Pet 4
                            case InteractionType.pet4:
                                string[] PetData3 = ExtraData.Split('\n');
                                Pet GeneratedPet3 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData3[0], 4, PetData3[1], PetData3[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet3);

                                break;
                            #endregion
                            #region Pet 5
                            case InteractionType.pet5:
                                string[] PetData7 = ExtraData.Split('\n');
                                Pet GeneratedPet7 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData7[0], 5, PetData7[1], PetData7[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet7);

                                break;
                            #endregion
                            #region Pet 6 (wrong?)
                            case InteractionType.pet6:
                                string[] PetData4 = ExtraData.Split('\n');
                                Pet GeneratedPet4 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData4[0], 6, PetData4[1], PetData4[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet4);

                                break;
                            #endregion
                            #region Pet 7 (wrong?)
                            case InteractionType.pet7:
                                string[] PetData6 = ExtraData.Split('\n');
                                Pet GeneratedPet6 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData6[0], 7, PetData6[1], PetData6[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet6);

                                break;
                            #endregion
                            #region Pet 8
                            case InteractionType.pet8:
                                string[] PetData8 = ExtraData.Split('\n');
                                Pet GeneratedPet8 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData8[0], 8, PetData8[1], PetData8[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet8);

                                break;
                            #endregion
                            #region Pet 8
                            case InteractionType.pet9:
                                string[] PetData9 = ExtraData.Split('\n');
                                Pet GeneratedPet9 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData9[0], 9, PetData9[1], PetData9[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet9);

                                break;
                            #endregion
                            #region Pet 10
                            case InteractionType.pet10:
                                string[] PetData10 = ExtraData.Split('\n');
                                Pet GeneratedPet10 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData10[0], 10, PetData10[1], PetData10[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet10);

                                break;
                            #endregion
                            #region Pet 11
                            case InteractionType.pet11:
                                string[] PetData11 = ExtraData.Split('\n');
                                Pet GeneratedPet11 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData11[0], 11, PetData11[1], PetData11[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet11);

                                break;
                            #endregion
                            #region Pet 12
                            case InteractionType.pet12:
                                string[] PetData12 = ExtraData.Split('\n');
                                Pet GeneratedPet12 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData12[0], 12, PetData12[1], PetData12[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet12);

                                break;
                            #endregion
                            #region Pet 13
                            case InteractionType.pet13: //Caballo - Horse
                                string[] PetData13 = ExtraData.Split('\n');
                                Pet GeneratedPet13 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData13[0], 13, PetData13[1], PetData13[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet13);

                                break;
                            #endregion
                            #region Pet 14
                            case InteractionType.pet14:
                                string[] PetData14 = ExtraData.Split('\n');
                                Pet GeneratedPet14 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData14[0], 14, PetData14[1], PetData14[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet14);

                                break;
                            #endregion
                            #region Pet 15
                            case InteractionType.pet15:
                                string[] PetData15 = ExtraData.Split('\n');
                                Pet GeneratedPet15 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData15[0], 15, PetData15[1], PetData15[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet15);

                                break;
                            #endregion
                            #region Pet 16
                            case InteractionType.pet16: // Mascota Agregada
                                string[] PetData16 = ExtraData.Split('\n');
                                Pet GeneratedPet16 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData16[0], 16, PetData16[1], PetData16[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet16);

                                break;
                            #endregion
                            #region Pet 17
                            case InteractionType.pet17: // Mascota Agregada
                                string[] PetData17 = ExtraData.Split('\n');
                                Pet GeneratedPet17 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData17[0], 17, PetData17[1], PetData17[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet17);

                                break;
                            #endregion
                            #region Pet 18
                            case InteractionType.pet18: // Mascota Agregada
                                string[] PetData18 = ExtraData.Split('\n');
                                Pet GeneratedPet18 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData18[0], 18, PetData18[1], PetData18[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet18);

                                break;
                            #endregion
                            #region Pet 19
                            case InteractionType.pet19: // Mascota Agregada
                                string[] PetData19 = ExtraData.Split('\n');
                                Pet GeneratedPet19 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData19[0], 19, PetData19[1], PetData19[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet19);

                                break;
                            #endregion
                            #region Pet 20
                            case InteractionType.pet20: // Mascota Agregada
                                string[] PetData20 = ExtraData.Split('\n');
                                Pet GeneratedPet20 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData20[0], 20, PetData20[1], PetData20[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet20);

                                break;
                            #endregion
                            #region Pet 21
                            case InteractionType.pet21: // Mascota Agregada
                                string[] PetData21 = ExtraData.Split('\n');
                                Pet GeneratedPet21 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData21[0], 21, PetData21[1], PetData21[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet21);

                                break;
                            #endregion
                            #region Pet 22
                            case InteractionType.pet22: // Mascota Agregada
                                string[] PetData22 = ExtraData.Split('\n');
                                Pet GeneratedPet22 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData22[0], 22, PetData22[1], PetData22[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet22);

                                break;
                            #endregion
                            #region Pet 23
                            case InteractionType.pet23: // Mascota Agregada
                                string[] PetData23 = ExtraData.Split('\n');
                                Pet GeneratedPet23 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData23[0], 23, PetData23[1], PetData23[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet23);

                                break;
                            #endregion
                            #region Pet 24
                            case InteractionType.pet24: // Mascota Agregada
                                string[] PetData24 = ExtraData.Split('\n');
                                Pet GeneratedPet24 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData24[0], 24, PetData24[1], PetData24[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet24);

                                break;
                            #endregion
                            #region Pet 25
                            case InteractionType.pet25: // Mascota Agregada
                                string[] PetData25 = ExtraData.Split('\n');
                                Pet GeneratedPet25 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData25[0], 25, PetData25[1], PetData25[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet25);

                                break;
                            #endregion
                            #region Pet 26
                            case InteractionType.pet26: // Mascota Agregada
                                string[] PetData26 = ExtraData.Split('\n');
                                Pet GeneratedPet26 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData26[0], 26, PetData26[1], PetData26[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet26);

                                break;
                            #endregion
                            #region Pet 28
                            case InteractionType.pet28: // Mascota Agregada
                                string[] PetData28 = ExtraData.Split('\n');
                                Pet GeneratedPet28 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData28[0], 28, PetData28[1], PetData28[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet28);

                                break;
                            #endregion
                            #region Pet 29
                            case InteractionType.pet29:
                                string[] PetData29 = ExtraData.Split('\n');
                                Pet GeneratedPet29 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData29[0], 29, PetData29[1], PetData29[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet29);

                                break;
                            #endregion
                            #region Pet 30
                            case InteractionType.pet30:
                                string[] PetData30 = ExtraData.Split('\n');
                                Pet GeneratedPet30 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData30[0], 30, PetData30[1], PetData30[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet30);

                                break;
                            #endregion
                            #region Pet 31
                            case InteractionType.pet31:
                                string[] PetData31 = ExtraData.Split('\n');
                                Pet GeneratedPet31 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData31[0], 31, PetData31[1], PetData31[2]);
                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet31);
                                break;
                            #endregion
                            #region Pet 32
                            case InteractionType.pet32:
                                string[] PetData32 = ExtraData.Split('\n');
                                Pet GeneratedPet32 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData32[0], 32, PetData32[1], PetData32[2]);
                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet32);
                                break;
                            #endregion
                            #region Pet 33
                            case InteractionType.pet33:
                                string[] PetData33 = ExtraData.Split('\n');
                                Pet GeneratedPet33 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData33[0], 33, PetData33[1], PetData33[2]);
                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet33);
                                break;
                            #endregion
                            #region Pet 34
                            case InteractionType.pet34:
                                string[] PetData34 = ExtraData.Split('\n');
                                Pet GeneratedPet34 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData34[0], 34, PetData34[1], PetData34[2]);
                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet34);
                                break;
                            #endregion
                            #region Pet 35
                            case InteractionType.pet35:
                                string[] PetData35 = ExtraData.Split('\n');
                                Pet GeneratedPet35 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData35[0], 35, PetData35[1], PetData35[2]);
                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet35);
                                break;
                            #endregion
                            #region Pet 36
                            case InteractionType.pet36:
                                string[] PetData36 = ExtraData.Split('\n');
                                Pet GeneratedPet36 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData36[0], 36, PetData36[1], PetData36[2]);
                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet36);
                                break;
                            #endregion
                            #region Pet 37
                            case InteractionType.pet37:
                                string[] PetData37 = ExtraData.Split('\n');
                                Pet GeneratedPet37 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData37[0], 37, PetData37[1], PetData37[2]);
                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet37);
                                break;
                            #endregion
                            #region Pet 38
                            case InteractionType.pet38:
                                string[] PetData38 = ExtraData.Split('\n');
                                Pet GeneratedPet38 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData38[0], 38, PetData38[1], PetData38[2]);
                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet38);
                                break;
                            #endregion
                            #region Pet 39
                            case InteractionType.pet39:
                                string[] PetData39 = ExtraData.Split('\n');
                                Pet GeneratedPet39 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData39[0], 39, PetData39[1], PetData39[2]);
                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet39);
                                break;
                            #endregion
                            #region Pet 40
                            case InteractionType.pet40:
                                string[] PetData40 = ExtraData.Split('\n');
                                Pet GeneratedPet40 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData40[0], 40, PetData40[1], PetData40[2]);
                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet40);
                                break;
                            #endregion
                            #region Pet 41
                            case InteractionType.pet41:
                                string[] PetData41 = ExtraData.Split('\n');
                                Pet GeneratedPet41 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData41[0], 41, PetData41[1], PetData41[2]);
                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet41);
                                break;
                            #endregion
                            #region Pet 42
                            case InteractionType.pet42:
                                string[] PetData42 = ExtraData.Split('\n');
                                Pet GeneratedPet42 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData42[0], 42, PetData42[1], PetData42[2]);
                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet42);
                                break;
                            #endregion
                            #region Pet 43
                            case InteractionType.pet43:
                                string[] PetData43 = ExtraData.Split('\n');
                                Pet GeneratedPet43 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData43[0], 43, PetData43[1], PetData43[2]);
                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet43);
                                break;
                                #endregion
                                #endregion

                        }


                        Session.SendMessage(new FurniListNotificationComposer(0, 3));
                        Session.SendMessage(new PetInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetPets()));

                        ItemData PetFood = null;
                        if (MoonEnvironment.GetGame().GetItemManager().GetItem(320, out PetFood))
                        {
                            Item Food = ItemFactory.CreateSingleItemNullable(PetFood, Session.GetHabbo(), "", "");
                            if (Food != null)
                            {
                                Session.GetHabbo().GetInventoryComponent().TryAddItem(Food);
                                Session.SendMessage(new FurniListNotificationComposer(Food.Id, 1));
                            }
                        }
                        break;
                    }
            }

            if (Item.Badge != string.Empty) Session.GetHabbo().GetBadgeComponent().GiveBadge(Item.Badge, true, Session);
            Session.SendMessage(new PurchaseOKComposer(Item, Item.Data));
            Session.SendMessage(new FurniListUpdateComposer());
        }

        public static string GenerateRainbowText(string Name)
        {
            StringBuilder NewName = new StringBuilder();

            string[] Colours = { "FF0000", "FFA500", "FFFF00", "008000", "0000FF", "800080" };

            int Count = 0;
            int Count2 = 0;
            while (Count < Name.Length)
            {
                NewName.Append("<font color='#" + Colours[Count2] + "'>" + Name[Count] + "</font>");

                Count++;
                Count2++;

                if (Count2 >= 6)
                    Count2 = 0;
            }

            return NewName.ToString();
        }

        public static string GeneratedoscoloresText(string Name)
        {
            StringBuilder NewName = new StringBuilder();

            string[] Colours = { "00527C", "000000", "00527C", "000000", "00527C", "000000" };

            int Count = 0;
            int Count2 = 0;
            while (Count < Name.Length)
            {
                NewName.Append("<font color='#" + Colours[Count2] + "'>" + Name[Count] + "</font>");

                Count++;
                Count2++;

                if (Count2 >= 6)
                    Count2 = 0;
            }

            return NewName.ToString();
        }

        public static string GeneratedoscoloresmasText(string Name)
        {
            StringBuilder NewName = new StringBuilder();

            string[] Colours = { "CF001C", "7B0010", "CF001C", "7B0010", "CF001C", "7B0010" };

            int Count = 0;
            int Count2 = 0;
            while (Count < Name.Length)
            {
                NewName.Append("<font color='#" + Colours[Count2] + "'>" + Name[Count] + "</font>");

                Count++;
                Count2++;

                if (Count2 >= 6)
                    Count2 = 0;
            }

            return NewName.ToString();
        }

        public static string GeneratecolorforbiText(string Name)
        {
            StringBuilder NewName = new StringBuilder();

            string[] Colours = { "0080A6", "00516A", "0080A6", "00516A", "0080A6", "00516A" };

            int Count = 0;
            int Count2 = 0;
            while (Count < Name.Length)
            {
                NewName.Append("<font color='#" + Colours[Count2] + "'>" + Name[Count] + "</font>");

                Count++;
                Count2++;

                if (Count2 >= 6)
                    Count2 = 0;
            }

            return NewName.ToString();
        }

        public static string GeneraterojoynegroText(string Name)
        {
            StringBuilder NewName = new StringBuilder();

            string[] Colours = { "FF0000", "000000", "FF0000", "000000", "FF0000", "000000" };

            int Count = 0;
            int Count2 = 0;
            while (Count < Name.Length)
            {
                NewName.Append("<font color='#" + Colours[Count2] + "'>" + Name[Count] + "</font>");

                Count++;
                Count2++;

                if (Count2 >= 6)
                    Count2 = 0;
            }

            return NewName.ToString();
        }

        public static string GeneratemoradoynegroText(string Name)
        {
            StringBuilder NewName = new StringBuilder();

            string[] Colours = { "65009B", "000000", "65009B", "000000", "65009B", "000000" };

            int Count = 0;
            int Count2 = 0;
            while (Count < Name.Length)
            {
                NewName.Append("<font color='#" + Colours[Count2] + "'>" + Name[Count] + "</font>");

                Count++;
                Count2++;

                if (Count2 >= 6)
                    Count2 = 0;
            }

            return NewName.ToString();
        }

        public static string GenerateverdeynegroText(string Name)
        {
            StringBuilder NewName = new StringBuilder();

            string[] Colours = { "00742C", "000000", "00742C", "000000", "00742C", "000000" };

            int Count = 0;
            int Count2 = 0;
            while (Count < Name.Length)
            {
                NewName.Append("<font color='#" + Colours[Count2] + "'>" + Name[Count] + "</font>");

                Count++;
                Count2++;

                if (Count2 >= 6)
                    Count2 = 0;
            }

            return NewName.ToString();
        }

        public static string GeneraterosadoyrosadoText(string Name)
        {
            StringBuilder NewName = new StringBuilder();

            string[] Colours = { "B70092", "F12AC9", "B70092", "F12AC9", "B70092", "F12AC9" };

            int Count = 0;
            int Count2 = 0;
            while (Count < Name.Length)
            {
                NewName.Append("<font color='#" + Colours[Count2] + "'>" + Name[Count] + "</font>");

                Count++;
                Count2++;

                if (Count2 >= 6)
                    Count2 = 0;
            }

            return NewName.ToString();
        }

    }
}