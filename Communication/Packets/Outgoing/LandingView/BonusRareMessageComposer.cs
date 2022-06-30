using Moon.Communication.Packets.Outgoing.Inventory.Furni;
using Moon.Communication.Packets.Outgoing.Inventory.Purse;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Items;
using Moon.HabboHotel.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Moon.Communication.Packets.Outgoing.LandingView
{
    class BonusRareMessageComposer : ServerPacket
    {
        public BonusRareMessageComposer(GameClient Session)
            : base(ServerPacketHeader.BonusRareMessageComposer)
        {
            
            string product = MoonEnvironment.GetDBConfig().DBData["bonus_rare_productdata_name"];
            int baseid = int.Parse(MoonEnvironment.GetDBConfig().DBData["bonus_rare_item_baseid"]);
            int score = Convert.ToInt32(MoonEnvironment.GetDBConfig().DBData["bonus_rare_total_score"]);

            base.WriteString(product);
            base.WriteInteger(baseid);
            base.WriteInteger(score);
            base.WriteInteger(Session.GetHabbo().BonusPoints >= score ? score : score - Session.GetHabbo().BonusPoints); //Total To Gain
            if (Session.GetHabbo().BonusPoints >= score)
            {
                Session.GetHabbo().BonusPoints -= score;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().BonusPoints, score, 101));
                Session.SendMessage(new RoomCustomizedAlertComposer("Has completado tu Bonus Rare ¡ya tienes tu premio en el inventario! Recibirás otro cuando vuelvas a acumular 120 puntos."));
                ItemData Item = null;
                if (!MoonEnvironment.GetGame().GetItemManager().GetItem((baseid), out Item))
                {
                    // No existe este ItemId.
                    return;
                }

                Item GiveItem = ItemFactory.CreateSingleItemNullable(Item, Session.GetHabbo(), "", "");
                if (GiveItem != null)
                {
                    Session.GetHabbo().GetInventoryComponent().TryAddItem(GiveItem);

                    Session.SendMessage(new FurniListNotificationComposer(GiveItem.Id, 1));
                    Session.SendMessage(new FurniListUpdateComposer());
                }

                Session.GetHabbo().GetInventoryComponent().UpdateItems(false);
            }
        }
    }
}
