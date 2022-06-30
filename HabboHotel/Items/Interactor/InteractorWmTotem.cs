using System;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.GameClients;
using Moon.Database.Interfaces;
using System.Data;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Items.Interactor
{
    public class InteractorWmTotem : IFurniInteractor
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

            if (Item.BaseItem == 1592)
            {
                if (Session.GetHabbo().Rank > 0)
                {

                    if (Item.UserID != Session.GetHabbo().Id)
                    {
                        if (Item.GetZ < 2.5)
                        {
                            Session.SendMessage(RoomNotificationComposer.SendBubble("furni_placement_error", "El planeta tota sólo puede ser activado si está encima de un tótem.", ""));
                            return;
                        }
                        if (Item.UserID != Session.GetHabbo().Id)
                        {
                            Session.SendMessage(RoomNotificationComposer.SendBubble("furni_placement_error", "Este tótem no te pertenece.", ""));
                            return;
                        }
                        if (Session.GetHabbo().Rank > 0)
                        {

                            if (Item.ExtraData == "0")
                            {
                                User.ApplyEffect(24);

                                Session.SendWhisper("Efecto de lluvia agregado con éxito.");
                                return;
                            }
                            if (Item.ExtraData == "1")
                            {
                                User.ApplyEffect(25);

                                Session.SendWhisper("Efecto de fuego agregado con éxito.");

                                return;
                            }
                            if (Item.ExtraData == "2")
                            {
                                User.ApplyEffect(26);

                                Session.SendWhisper("Efecto Totem agregado con éxito.");

                                return;
                            }
                            if (Item.ExtraData == "3")
                            {
                                User.ApplyEffect(23);
                                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("INSERT INTO `user_effects` (`user_id`,`effect_id`) VALUES ('" + Session.GetHabbo().Id + "', '23')");
                                    dbClient.RunQuery();
                                }
                                Session.SendWhisper("Efecto levitación agregado con éxito.");

                                return;
                            }
                        }
                    }
                    else
                    {
                        if (Item.GetZ < 2.5)
                        {
                            Session.SendMessage(RoomNotificationComposer.SendBubble("furni_placement_error", "El planeta tota sólo puede ser activado si está encima de un tótem.", ""));
                            return;
                        }
                        if (Item.UserID != Session.GetHabbo().Id)
                        {
                            Session.SendMessage(RoomNotificationComposer.SendBubble("furni_placement_error", "Esta caja de la suerte no te pertenece.", ""));
                            return;
                        }
                        if (Item.ExtraData == "0")
                        {
                            User.ApplyEffect(24);
                            Item.ExtraData = "1";
                            Item.UpdateState(true, true);
                            Item.RequestUpdate(0, true);
 
                            Session.SendWhisper("Efecto de lluvia agregado con éxito.");
                            return;
                        }
                        if (Item.ExtraData == "1")
                        {
                            User.ApplyEffect(25);
                            Item.ExtraData = "2";
                            Item.UpdateState(true, true);
                            Item.RequestUpdate(0, true);

                            Session.SendWhisper("Efecto de fuego agregado con éxito.");

                            return;
                        }
                        if (Item.ExtraData == "2")
                        {
                            User.ApplyEffect(26);
                            Item.ExtraData = "3";
                            Item.UpdateState(true, true);
                            Item.RequestUpdate(0, true);
  
                            Session.SendWhisper("Efecto Totem agregado con éxito.");

                            return;
                        }
                        if (Item.ExtraData == "3")
                        {
                            User.ApplyEffect(23);
                            Item.ExtraData = "0";
                            Item.UpdateState(true, true);
                            Item.RequestUpdate(0, true);
 
                            Session.SendWhisper("Efecto levitación agregado con éxito.");

                            return;
                        }
                    }
                }
                else
                {
                    Session.SendMessage(RoomNotificationComposer.SendBubble("furni_placement_error", "Sólo miembros VIP tienen acceso al Efecto Totem.", ""));
                    return;
                }
            }

            if (Item.BaseItem == 2536366)
            {
                if (Session.GetHabbo().Rank > 0)
                {

                    if (Item.UserID != Session.GetHabbo().Id)
                    {
                        if (Item.GetZ < 2.5)
                        {
                            Session.SendMessage(RoomNotificationComposer.SendBubble("furni_placement_error", "El planeta tota sólo puede ser activado si está encima de un tótem.", ""));
                            return;
                        }
                        if (Item.UserID != Session.GetHabbo().Id)
                        {
                            Session.SendMessage(RoomNotificationComposer.SendBubble("furni_placement_error", "Este tótem no te pertenece.", ""));
                            return;
                        }
                        if (Item.ExtraData == "0")
                        {
                            User.ApplyEffect(548);

                            Session.SendWhisper("Efecto fuego agregado con éxito.");
                            return;
                        }
                        if (Item.ExtraData == "1")
                        {
                            User.ApplyEffect(531);

                            Session.SendWhisper("Efecto fuego agregado con éxito.");

                            return;
                        }
                        if (Item.ExtraData == "2")
                        {
                            User.ApplyEffect(26);

                            Session.SendWhisper("Efecto Totem agregado con éxito.");

                            return;
                        }
                        if (Item.ExtraData == "3")
                        {
                            User.ApplyEffect(23);

                            Session.SendWhisper("Efecto levitación agregado con éxito.");

                            return;
                        }
                    }
                    else
                    {
                        if (Item.GetZ < 2.5)
                        {
                            Session.SendMessage(RoomNotificationComposer.SendBubble("furni_placement_error", "El planeta tota sólo puede ser activado si está encima de un tótem.", ""));
                            return;
                        }
                        if (Item.UserID != Session.GetHabbo().Id)
                        {
                            Session.SendMessage(RoomNotificationComposer.SendBubble("furni_placement_error", "Esta caja de la suerte no te pertenece.", ""));
                            return;
                        }
                        if (Item.ExtraData == "0")
                        {
                            User.ApplyEffect(548);
                            Item.ExtraData = "1";
                            Item.UpdateState(true, true);
                            Item.RequestUpdate(0, true);

                            Session.SendWhisper("Efecto fuego agregado con éxito.");
                            return;
                        }
                        if (Item.ExtraData == "1")
                        {
                            User.ApplyEffect(531);
                            Item.ExtraData = "2";
                            Item.UpdateState(true, true);
                            Item.RequestUpdate(0, true);

                            Session.SendWhisper("Efecto fuego agregado con éxito.");

                            return;
                        }
                        if (Item.ExtraData == "2")
                        {
                            User.ApplyEffect(26);
                            Item.ExtraData = "3";
                            Item.UpdateState(true, true);
                            Item.RequestUpdate(0, true);

                            Session.SendWhisper("Efecto Totem agregado con éxito.");

                            return;
                        }
                        if (Item.ExtraData == "3")
                        {
                            User.ApplyEffect(23);
                            Item.ExtraData = "0";
                            Item.UpdateState(true, true);
                            Item.RequestUpdate(0, true);

                            Session.SendWhisper("Efecto levitación agregado con éxito.");

                            return;
                        }
                    }
                }
                else
                {
                    Session.SendMessage(RoomNotificationComposer.SendBubble("advice", "Sólo miembros VIP tienen acceso al Efecto Totem.", ""));
                    return;
                }
            }

            

            if (Item.BaseItem == 42636366)
            {
                Random random = new Random();
                int mobi1 = 1145;
                string mobi01 = "una Barra de Oro (500C)";
                string imagem01 = "CFC_500_goldbar";

                int mobi2 = 1145;
                string mobi02 = "una Barra de Oro (500C)";
                string imagem02 = "CFC_500_goldbar";

                int mobi3 = 1145;
                string mobi03 = "una Barra de Oro (500C)";
                string imagem03 = "CFC_500_goldbar";

                int mobi4 = 1145;
                string mobi04 = "una Barra de Oro (500C)";
                string imagem04 = "CFC_500_goldbar";

                int mobi5 = 10342383;
                string mobi05 = "una Piedra de Diamantes (5D)";
                string imagem05 = "DIA_5_diamonds_habbis";

                int mobi6 = 13420382;
                string mobi06 = "una Piedra de Diamantes (1D)";
                string imagem06 = "DIA_1_diamonds_habbis";

                int mobi7 = 540019;
                string mobi07 = "un Pato de Oro 250c";
                string imagem07 = "CFC_500_goldbar";

                int mobi8 = 540019;
                string mobi08 = "un Pato de Oro 250c";
                string imagem08 = "CFC_500_goldbar";

                int mobi9 = 4468633;
                string mobi09 = "una Moneda de Diamante (10D)";
                string imagem09 = "DI_10_diam_coin";

                int randomNumber = random.Next(1, 9);
                if (Item.UserID != Session.GetHabbo().Id)
                {
                    Session.SendMessage(RoomNotificationComposer.SendBubble("furni_placement_error", "Esta caja de la suerte no te pertenece.", ""));
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
                    Session.SendMessage(RoomNotificationComposer.SendBubble("icon/" + imagem01 + "_icon", "Usted acaba de ganar " + mobi01, "inventory/open/furni"));
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
                    Session.SendMessage(RoomNotificationComposer.SendBubble("icon/" + imagem02 + "_icon", "Usted acaba de ganar " + mobi02, "inventory/open/furni"));
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
                    Session.SendMessage(RoomNotificationComposer.SendBubble("icon/" + imagem03 + "_icon", "Usted acaba de ganar " + mobi03, "inventory/open/furni"));
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
                    Session.SendMessage(RoomNotificationComposer.SendBubble("icon/" + imagem04 + "_icon", "Usted acaba de ganar " + mobi04, "inventory/open/furni"));
                    return;
                }
                if (randomNumber == 5)
                {
                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE `items` SET `room_id` = '0', `base_item` = '" + mobi5 + "' WHERE `id` = '" + Item.Id + "' LIMIT 1");
                    }
                    Session.GetHabbo().GetInventoryComponent().AddNewItem(Item.Id, mobi5, Item.ExtraData, Item.GroupId, true, true, Item.LimitedNo, Item.LimitedTot);
                    Session.GetHabbo().GetInventoryComponent().UpdateItems(true);
                    Session.SendMessage(RoomNotificationComposer.SendBubble("icon/" + imagem05 + "_icon", "Usted acaba de ganar " + mobi05, "inventory/open/furni"));
                    return;
                }
                if (randomNumber == 6)
                {
                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE `items` SET `room_id` = '0', `base_item` = '" + mobi6 + "' WHERE `id` = '" + Item.Id + "' LIMIT 1");
                    }
                    Session.GetHabbo().GetInventoryComponent().AddNewItem(Item.Id, mobi6, Item.ExtraData, Item.GroupId, true, true, Item.LimitedNo, Item.LimitedTot);
                    Session.GetHabbo().GetInventoryComponent().UpdateItems(true);
                    Session.SendMessage(RoomNotificationComposer.SendBubble("icon/" + imagem06 + "_icon", "Usted acaba de ganar " + mobi06, "inventory/open/furni"));
                    return;
                }
                if (randomNumber == 7)
                {
                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE `items` SET `room_id` = '0', `base_item` = '" + mobi7 + "' WHERE `id` = '" + Item.Id + "' LIMIT 1");
                    }
                    Session.GetHabbo().GetInventoryComponent().AddNewItem(Item.Id, mobi7, Item.ExtraData, Item.GroupId, true, true, Item.LimitedNo, Item.LimitedTot);
                    Session.GetHabbo().GetInventoryComponent().UpdateItems(true);
                    Session.SendMessage(RoomNotificationComposer.SendBubble("icon/" + imagem07 + "_icon", "Usted acaba de ganar " + mobi07, "inventory/open/furni"));
                    return;
                }
                if (randomNumber == 8)
                {
                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE `items` SET `room_id` = '0', `base_item` = '" + mobi8 + "' WHERE `id` = '" + Item.Id + "' LIMIT 1");
                    }
                    Session.GetHabbo().GetInventoryComponent().AddNewItem(Item.Id, mobi8, Item.ExtraData, Item.GroupId, true, true, Item.LimitedNo, Item.LimitedTot);
                    Session.GetHabbo().GetInventoryComponent().UpdateItems(true);
                    Session.SendMessage(RoomNotificationComposer.SendBubble("icon/" + imagem08 + "_icon", "Usted acaba de ganar " + mobi08, "inventory/open/furni"));
                    return;
                }
                if (randomNumber == 9)
                {
                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE `items` SET `room_id` = '0', `base_item` = '" + mobi9 + "' WHERE `id` = '" + Item.Id + "' LIMIT 1");
                    }
                    Session.GetHabbo().GetInventoryComponent().AddNewItem(Item.Id, mobi9, Item.ExtraData, Item.GroupId, true, true, Item.LimitedNo, Item.LimitedTot);
                    Session.GetHabbo().GetInventoryComponent().UpdateItems(true);
                    Session.SendMessage(RoomNotificationComposer.SendBubble("icon/" + imagem09 + "_icon", "Usted acaba de ganar " + mobi09, "inventory/open/furni"));
                    return;
                }
            }
        }

        public void OnWiredTrigger(Item Item)
        {
            Item.ExtraData = "-1";
            Item.UpdateState(false, true);
            Item.RequestUpdate(4, true);
        }
    }
}