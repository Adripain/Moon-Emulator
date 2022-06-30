using Moon.Communication.Packets.Outgoing.Notifications;
using Moon.Communication.Packets.Outgoing.Rooms.Engine;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.Database.Interfaces;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class DevelopperCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_developer"; }
        }

        public string Parameters
        {
            get { return "%acción%"; }
        }

        public string Description
        {
            get { return "Atualiza uma caracteristica do "+ Convert.ToString(MoonEnvironment.GetConfig().data["hotel.name"]) + "."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (Session.GetHabbo().isDeveloping == false)
            {
                var _cache = new Random().Next(0, 300);
                Session.SendMessage(new MassEventComposer("habbopages/developer.txt?" + _cache));
                Session.GetHabbo().isDeveloping = true;
                return;
            }

            List<Item> ItemsToCopy = new List<Item>();
            if (!ItemsToCopy.Any())
            {
                ItemsToCopy = Room.GetGameMap().GetAllRoomItemForSquare(Session.GetHabbo().lastX, Session.GetHabbo().lastY);
            }

            string action = Params[1];

            switch (action.ToLower())
            {
                case "info":
                    List<Item> Items = Room.GetGameMap().GetAllRoomItemForSquare(Session.GetHabbo().lastX, Session.GetHabbo().lastY);
                    StringBuilder HabboInfo = new StringBuilder();
                    foreach (Item _item in Items)
                    {
                        HabboInfo.Append("Información de " + _item.GetBaseItem().ItemName.ToUpper() + ":\r");
                        HabboInfo.Append("ID: " + _item.Id + " - BaseItem: " + _item.GetBaseItem().Id + "\r");
                        if (!_item.IsWired) { HabboInfo.Append("Extradata: " + _item.ExtraData + " - Modes: " + _item.GetBaseItem().Modes + "\r"); }
                        HabboInfo.Append("SpriteID: " + _item.GetBaseItem().SpriteId + " - Type: " + _item.GetBaseItem().Type + "\r");
                        HabboInfo.Append("Public Name: " + _item.GetBaseItem().PublicName + " - Interaction: " + _item.GetBaseItem().InteractionType + "\r\r");
                        HabboInfo.Append("Height: " + _item.GetBaseItem().Height + " - Length: " + _item.GetBaseItem().Length + " - Width: " + _item.GetBaseItem().Width + "\r");
                        HabboInfo.Append("Coords: - X: " + _item.GetX + " - Y: " + _item.GetY + " - Z: " + _item.GetZ + "\r\r");
                        //HabboInfo.Append("AdjustableHeights: " + int.Parse(_item.GetBaseItem().AdjustableHeights.ToString()) + "\r");
                        HabboInfo.Append("ExtraRots: " + _item.GetBaseItem().ExtraRot + " - Tradable: " + _item.GetBaseItem().AllowTrade + " - Stackable: " + _item.GetBaseItem().Stackable + "\r");
                        HabboInfo.Append("Iventory Stack: " + _item.GetBaseItem().AllowInventoryStack + " - Giftable: " + _item.GetBaseItem().AllowGift + " - Recyclable: " + _item.GetBaseItem().AllowEcotronRecycle + "\r");
                        HabboInfo.Append("Sellable: " + _item.GetBaseItem().AllowMarketplaceSell + " - Rare: " + _item.GetBaseItem().IsRare + " - Chair: " + _item.GetBaseItem().IsSeat + "\r");
                        HabboInfo.Append("Walkable: " + _item.GetBaseItem().Walkable + " - VendingIDs: " + _item.GetBaseItem().VendingIds.ToArray() + "\r");
                        HabboInfo.Append("____________________________________________________\r\r");

                    }
                    Session.SendMessage(new MOTDNotificationComposer(HabboInfo.ToString()));
                    break;

                case "set":
                    string type = Params[2];
                    string itemid = Params[3];
                    string column = Params[4];
                    string value = Params[5];

                    switch (type)
                    {
                        case "baseitem":
                            switch (column)
                            {
                                case "interactiontype":
                                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        dbClient.RunQuery("UPDATE `furniture` SET `interaction_type` = '" + value + "' WHERE `id` = '" + itemid + "' LIMIT 1");
                                    }

                                    Session.SendWhisper("Cambiado: " + value + " en ID: " + itemid, 23);
                                    break;

                                case "multiheight":
                                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        dbClient.RunQuery("UPDATE `furniture` SET `height_adjustable` = '" + value + "' WHERE `id` = '" + itemid + "' LIMIT 1");
                                    }

                                    Session.SendWhisper("Cambiado: " + value + " en ID: " + itemid, 23);
                                    break;

                                case "modes":
                                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        dbClient.RunQuery("UPDATE `furniture` SET `interaction_modes_count` = '" + value + "' WHERE `id` = '" + itemid + "' LIMIT 1");
                                    }

                                    Session.SendWhisper("Cambiados modos: " + value + " en ID: " + itemid, 23);
                                    MoonEnvironment.GetGame().GetItemManager().Init();
                                    break;

                                case "height":
                                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        dbClient.RunQuery("UPDATE `furniture` SET `stack_height` = '" + value + "' WHERE `id` = '" + itemid + "' LIMIT 1");
                                    }

                                    Session.SendWhisper("Cambiada altura: " + value + " en ID: " + itemid, 23);
                                    break;

                                case "length":
                                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        dbClient.RunQuery("UPDATE `furniture` SET `length` = '" + value + "' WHERE `id` = '" + itemid + "' LIMIT 1");
                                    }

                                    Session.SendWhisper("Cambiada longitud: " + value + " en ID: " + itemid, 23);
                                    break;

                                case "width":
                                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        dbClient.RunQuery("UPDATE `furniture` SET `width` = '" + value + "' WHERE `id` = '" + itemid + "' LIMIT 1");
                                    }

                                    Session.SendWhisper("Cambiada anchura: " + value + " en ID: " + itemid, 23);
                                    break;

                                case "extrarot":
                                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        dbClient.RunQuery("UPDATE `furniture` SET `extra_rot` = '" + value + "' WHERE `id` = '" + itemid + "' LIMIT 1");
                                    }

                                    Session.SendWhisper("Cambiada rotación extra: " + value + " en ID: " + itemid, 23);
                                    break;

                                case "tradable":
                                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        dbClient.RunQuery("UPDATE `furniture` SET `allow_trade` = '" + value + "' WHERE `id` = '" + itemid + "' LIMIT 1");
                                    }

                                    Session.SendWhisper("Tradeable: " + value + " en ID: " + itemid, 23);
                                    break;

                                case "stackable":
                                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        dbClient.RunQuery("UPDATE `furniture` SET `can_stack` = '" + value + "' WHERE `id` = '" + itemid + "' LIMIT 1");
                                    }

                                    Session.SendWhisper("Apilable: " + value + " en ID: " + itemid, 23);
                                    break;

                                case "inventorystack":
                                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        dbClient.RunQuery("UPDATE `furniture` SET `allow_inventory_stack` = '" + value + "' WHERE `id` = '" + itemid + "' LIMIT 1");
                                    }

                                    Session.SendWhisper("Stackeable en inventario: " + value + " en ID: " + itemid, 23);
                                    break;

                                case "giftable":
                                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        dbClient.RunQuery("UPDATE `furniture` SET `allow_gift` = '" + value + "' WHERE `id` = '" + itemid + "' LIMIT 1");
                                    }

                                    Session.SendWhisper("Cambiado regalable: " + value + " en ID: " + itemid, 23);
                                    break;

                                case "recyclable":
                                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        dbClient.RunQuery("UPDATE `furniture` SET `allow_recycle` = '" + value + "' WHERE `id` = '" + itemid + "' LIMIT 1");
                                    }

                                    Session.SendWhisper("Cambiado reciclable: " + value + " en ID: " + itemid, 23);
                                    break;

                                case "sellable":
                                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        dbClient.RunQuery("UPDATE `furniture` SET `allow_marketplace_sell` = '" + value + "' WHERE `id` = '" + itemid + "' LIMIT 1");
                                    }

                                    Session.SendWhisper("Cambiado venderse en mercadillo: " + value + " en ID: " + itemid, 23);
                                    break;

                                case "rare":
                                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        dbClient.RunQuery("UPDATE `furniture` SET `is_rare` = '" + value + "' WHERE `id` = '" + itemid + "' LIMIT 1");
                                    }

                                    Session.SendWhisper("Cambiado a rare: " + value + " en ID: " + itemid, 23);
                                    break;

                                case "chair":
                                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        dbClient.RunQuery("UPDATE `furniture` SET `can_sit` = '" + value + "' WHERE `id` = '" + itemid + "' LIMIT 1");
                                    }

                                    Session.SendWhisper("Cambiado a silla: " + value + " en ID: " + itemid, 23);
                                    break;

                                case "walkable":
                                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        dbClient.RunQuery("UPDATE `furniture` SET `is_walkable` = '" + value + "' WHERE `id` = '" + itemid + "' LIMIT 1");
                                    }

                                    Session.SendWhisper("Se puede pisar: " + value + " en ID: " + itemid, 23);
                                    break;

                                case "vendingids":
                                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        dbClient.RunQuery("UPDATE `furniture` SET `vending_ids` = '" + value + "' WHERE `id` = '" + itemid + "' LIMIT 1");
                                    }

                                    Session.SendWhisper("Colocadas vending_ids: " + value + " en ID: " + itemid, 23);
                                    break;

                                case "wiredid":
                                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        dbClient.RunQuery("UPDATE `furniture` SET `wired_id` = '" + value + "' WHERE `id` = '" + itemid + "' LIMIT 1");
                                    }

                                    Session.SendWhisper("Colocada WiredID: " + value + " en ID: " + itemid, 23);
                                    break;
                            }
                            break;

                        case "item":
                            switch (column)
                            {
                                case "extradata":
                                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        dbClient.RunQuery("UPDATE `items` SET `extra_data` = '" + value + "' WHERE `id` = '" + itemid + "' LIMIT 1");
                                    }

                                    Session.SendWhisper("Cambiado extradata: " + value + " en ID: " + itemid, 23);

                                    List<Item> I = Room.GetGameMap().GetAllRoomItemForSquare(Session.GetHabbo().lastX, Session.GetHabbo().lastY);
                                    foreach (Item _item in I)
                                    {
                                        _item.ExtraData = value;
                                        _item.UpdateState();
                                    }

                                    I.Clear();

                                    MoonEnvironment.GetGame().GetItemManager().Init();
                                    break;

                                case "wireddata":
                                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        dbClient.RunQuery("UPDATE `wired_items` SET `string` = '" + value + "' WHERE `id` = '" + itemid + "' LIMIT 1");
                                    }

                                    Session.SendWhisper("Cambiado wireddata: " + value + " en ID: " + itemid, 23);
                                    MoonEnvironment.GetGame().GetItemManager().Init();
                                    break;

                                case "rot":
                                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        dbClient.RunQuery("UPDATE `items` SET `rot` = '" + value + "' WHERE `id` = '" + itemid + "' LIMIT 1");
                                    }


                                    Session.SendWhisper("Cambiada rotación: " + value + " en ID: " + itemid, 23);
                                    List<Item> IZ = Room.GetGameMap().GetAllRoomItemForSquare(Session.GetHabbo().lastX, Session.GetHabbo().lastY);
                                    foreach (Item _item in IZ)
                                    {
                                        _item.Rotation = int.Parse(value);
                                        _item.UpdateState();
                                    }

                                    IZ.Clear();
                                    MoonEnvironment.GetGame().GetItemManager().Init();
                                    break;

                                case "z":
                                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        dbClient.RunQuery("UPDATE `items` SET `z` = '" + value + "' WHERE `id` = '" + itemid + "' LIMIT 1");
                                    }


                                    Session.SendWhisper("Cambiada rotación: " + value + " en ID: " + itemid, 23);
                                    List<Item> IR = Room.GetGameMap().GetAllRoomItemForSquare(Session.GetHabbo().lastX, Session.GetHabbo().lastY);
                                    foreach (Item _item in IR)
                                    {
                                        _item.GetZ = int.Parse(value);
                                        _item.UpdateState();
                                    }

                                    IR.Clear();
                                    break;
                            }
                            break;


                    }
                    break;

                case "copy":
                    Session.SendWhisper("Acabas de copiar una columna de " + ItemsToCopy.Count + " furnis.", 23);
                    Session.GetHabbo().isPasting = true;
                    break;

                case "clear":
                    Session.SendWhisper("Acabas de vaciar tu copia.", 23);
                    ItemsToCopy.Clear();
                    Session.GetHabbo().isPasting = false;
                    break;

                case "paste":
                    foreach (Item _item in ItemsToCopy)
                    {
                        using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            string q = "INSERT INTO `items` (`base_item`,`user_id`,`extra_data`,`room_id`,`x`,`y`,`z`) VALUES ('" + _item.BaseItem + "', '" + Session.GetHabbo().Id + "', '" + _item.ExtraData + "', '" + _item.RoomId + "', '" + Session.GetHabbo().lastX2 + "', '" + Session.GetHabbo().lastY2 + "', '" + _item.GetZ + "')";
                            dbClient.RunQuery(q);
                            Console.WriteLine(q);
                        }

                        Room.SendMessage(new ObjectAddComposer(_item, Room));
                        Room.GetRoomItemHandler().SetFloorItem(Session, _item, Session.GetHabbo().lastX2, Session.GetHabbo().lastY2, _item.Rotation, true, false, true);

                    }
                    break;

                case "update":
                    MoonEnvironment.GetGame().GetItemManager().Init();
                    break;

                default:
                    Session.SendWhisper("'info' - información del furni. 'set' - coloca cambios en el furni.");
                    break;
            }
        }


    }
}


