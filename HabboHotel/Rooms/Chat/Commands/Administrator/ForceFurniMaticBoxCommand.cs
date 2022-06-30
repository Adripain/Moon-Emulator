using Moon.HabboHotel.Catalog.PredesignedRooms;
using System.Text;
using System.Linq;
using System.Globalization;
using Moon.HabboHotel.Items;
using System;
using Moon.Communication.Packets.Outgoing.Inventory.Furni;
using Moon.Communication.Packets.Outgoing.Catalog;
using Moon.Communication.Packets.Outgoing;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class ForceFurniMaticBoxCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_16"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "TEST Command - Rewards"; }
        }

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            if(Session.GetHabbo().Username != "Imperial")
            {
                Session.SendWhisper("Uy mijito qué intentas.");
                return;
            }

            int Boxes = int.Parse(Params[1]);

            for (int i = 1; i <= Boxes; i++)
            {
                var reward = MoonEnvironment.GetGame().GetFurniMaticRewardsMnager().GetRandomReward();
                if (reward == null) return;
                int rewardId;
                var furniMaticBoxId = 4692;
                ItemData data = null;
                MoonEnvironment.GetGame().GetItemManager().GetItem(furniMaticBoxId, out data);
                var maticData = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;
                using (var dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("INSERT INTO `items` (`base_item`,`user_id`,`extra_data`) VALUES ('" + data.Id + "', '" + Session.GetHabbo().Id + "', @extra_data)");
                    dbClient.AddParameter("extra_data", maticData);
                    rewardId = Convert.ToInt32(dbClient.InsertQuery());
                    dbClient.runFastQuery("INSERT INTO `user_presents` (`item_id`,`base_id`,`extra_data`) VALUES ('" + rewardId + "', '" + reward.GetBaseItem().Id + "', '')");
                    dbClient.RunQuery("DELETE FROM `items` WHERE `id` = " + rewardId + " LIMIT 1;");
                }

                var GiveItem = ItemFactory.CreateGiftItem(data, Session.GetHabbo(), maticData, maticData, rewardId, 0, 0);
                if (GiveItem != null)
                {
                    Session.GetHabbo().GetInventoryComponent().TryAddItem(GiveItem);
                    Session.SendMessage(new FurniListNotificationComposer(GiveItem.Id, 1));
                    Session.SendMessage(new PurchaseOKComposer());
                    Session.SendMessage(new FurniListAddComposer(GiveItem));
                    Session.SendMessage(new FurniListUpdateComposer());
                }

                var response = new ServerPacket(ServerPacketHeader.FurniMaticReceiveItem);
                response.WriteInteger(1);
                response.WriteInteger(GiveItem.Id); // received item id
                Session.SendMessage(response);
            }
        }
    }
}
