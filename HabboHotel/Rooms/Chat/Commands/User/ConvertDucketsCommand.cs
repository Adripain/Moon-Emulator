using System;
using System.Data;
using Moon.HabboHotel.Items;
using Moon.Communication.Packets.Outgoing.Inventory.Purse;
using Moon.Database.Interfaces;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class ConvertDucketsCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";
        public string Parameters => "";
        public string Description => "Transforma suas moedas em duckets reais.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
                int TotalDuckets = 0;

                try
                {
                    DataTable Table = null;
                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("SELECT `id` FROM `items` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND (`room_id`=  '0' OR `room_id` = '')");
                        Table = dbClient.getTable();
                    }

                    if (Table == null)
                    {
                        Session.SendWhisper("Você não tem moeda alguma no seu inventario!", 34);
                        return;
                    }

                    foreach (DataRow Row in Table.Rows)
                    {
                        Item Item = Session.GetHabbo().GetInventoryComponent().GetItem(Convert.ToInt32(Row[0]));
                        if (Item == null)
                            continue;

                        if (!Item.GetBaseItem().ItemName.StartsWith("DU_") && !Item.GetBaseItem().ItemName.StartsWith("DUC_"))
                            continue;

                        if (Item.RoomId > 0)
                            continue;

                        string[] Split = Item.GetBaseItem().ItemName.Split('_');
                        int Value = int.Parse(Split[1]);

                        using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.runFastQuery("DELETE FROM `items` WHERE `id` = '" + Item.Id + "' LIMIT 1");
                        }

                        Session.GetHabbo().GetInventoryComponent().RemoveItem(Item.Id);

                        TotalDuckets += Value;

                        if (Value > 0)
                        {
                            Session.GetHabbo().Duckets += Value;
                            Session.SendMessage(new ActivityPointsComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Diamonds, Session.GetHabbo().GOTWPoints));
                        }
                    }

                    if (TotalDuckets > 0)
                        Session.SendWhisper("Transformou corretamente " + TotalDuckets + " duckets do seu inventario!", 34);
                    else
                        Session.SendWhisper("Lamentamos, ocorreu um erro!", 34);
                }
                catch
                {
                    Session.SendNotification("Lamentamos, ocorreu um erro!");
                }
            }
        }
    }