using System;
using System.Data;

using Moon.Communication.Packets.Incoming;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Catalog.Vouchers;



using Moon.Communication.Packets.Outgoing.Catalog;
using Moon.Communication.Packets.Outgoing.Inventory.Purse;

using Moon.Database.Interfaces;
using Moon.HabboHotel.Items;
using Moon.Communication.Packets.Outgoing.Inventory.Furni;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.Communication.Packets.Incoming.Catalog
{
    public class RedeemVoucherEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string VoucherCode = Packet.PopString().Replace("\r", "");

            Voucher Voucher = null;
            if (!MoonEnvironment.GetGame().GetCatalog().GetVoucherManager().TryGetVoucher(VoucherCode, out Voucher))
            {
                Session.SendMessage(new VoucherRedeemErrorComposer(0));
                return;
            }

            if (Voucher.CurrentUses >= Voucher.MaxUses)
            {
                Session.SendNotification("¡Este Código voucher se ha usado en su maximo de veces Permitidas!");
                return;
            }

            DataRow GetRow = null;
            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `user_vouchers` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND `voucher` = @Voucher LIMIT 1");
                dbClient.AddParameter("Voucher", VoucherCode);
                GetRow = dbClient.getRow();
            }

            if (GetRow != null)
            {
                Session.SendNotification("¡Ya usted ha usado este código Voucher!!");
                return;
            }
            else
            {
                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("INSERT INTO `user_vouchers` (`user_id`,`voucher`) VALUES ('" + Session.GetHabbo().Id + "', @Voucher)");
                    dbClient.AddParameter("Voucher", VoucherCode);
                    dbClient.RunQuery();
                }
            }

            Voucher.UpdateUses();

            if (Voucher.Type == VoucherType.CREDIT)
            {
                Session.GetHabbo().Credits += Voucher.Value;
                Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                Session.SendMessage(RoomNotificationComposer.SendBubble("voucher", "Acabas de recibir un premio voucher por el valor de "+ Voucher.Value +" créditos. ¡Úsalos con cabeza, " + Session.GetHabbo().Username +".", ""));
            }
            else if (Voucher.Type == VoucherType.DUCKET)
            {
                Session.GetHabbo().Duckets += Voucher.Value;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Voucher.Value));
                Session.SendMessage(RoomNotificationComposer.SendBubble("voucher", "Acabas de recibir un premio voucher por el valor de " + Voucher.Value + " duckets. ¡Úsalos con cabeza, " + Session.GetHabbo().Username + ".", ""));
            }
            else if (Voucher.Type == VoucherType.DIAMOND)
            {
                Session.GetHabbo().Diamonds += Voucher.Value;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, Voucher.Value, 5));
                Session.SendMessage(RoomNotificationComposer.SendBubble("voucher", "Acabas de recibir un premio voucher por el valor de " + Voucher.Value + " diamantes. ¡Úsalos con cabeza, " + Session.GetHabbo().Username + ".", ""));
            }
            else if (Voucher.Type == VoucherType.HONOR)
            {
                Session.GetHabbo().GOTWPoints += Voucher.Value;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, Voucher.Value, 103));
                Session.SendMessage(RoomNotificationComposer.SendBubble("voucher", "Acabas de recibir un premio voucher por el valor de " + Voucher.Value + " " + MoonEnvironment.GetDBConfig().DBData["seasonal.currency.name"] + ". ¡Úsalos con cabeza, " + Session.GetHabbo().Username + ".", ""));
            }
            else if (Voucher.Type == VoucherType.ITEM)
            {

                ItemData Item = null;
                if (!MoonEnvironment.GetGame().GetItemManager().GetItem((Voucher.Value), out Item))
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
                    Session.SendMessage(RoomNotificationComposer.SendBubble("voucher", "Acabas de recibir un objeto raro desde un voucher. ¡Corre, " + Session.GetHabbo().Username + ", revisa tu inventario, hay algo nuevo al parecer!", ""));
                }

                Session.GetHabbo().GetInventoryComponent().UpdateItems(false);
            }

            Session.SendMessage(new VoucherRedeemOkComposer());
        }
    }
}