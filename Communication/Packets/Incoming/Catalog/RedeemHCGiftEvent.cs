using Moon.Communication.Packets.Outgoing.Catalog;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Rooms.AI;
using Moon.Communication.Packets.Incoming;
using Moon.HabboHotel.Items;
using Moon.Communication.Packets.Outgoing.Inventory.Furni;
using Moon.Database.Interfaces;

namespace Moon.Communication.Packets.Incoming.Catalog
{
    public class RedeemHCGiftEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string item = Packet.PopString();

            ItemData gift = MoonEnvironment.GetGame().GetItemManager().GetItemByName(item);

            Session.GetHabbo().GetInventoryComponent().AddNewItem(0, gift.Id, "", 0, true, false, 0, 0);
            Session.SendMessage(new FurniListUpdateComposer());
            Session.GetHabbo().GetInventoryComponent().UpdateItems(true);

            Session.GetHabbo().GetStats().vipGifts--;

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `user_stats` SET `vip_gifts` = '" + Session.GetHabbo().GetStats().vipGifts + "' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
            }
        }
    }
}