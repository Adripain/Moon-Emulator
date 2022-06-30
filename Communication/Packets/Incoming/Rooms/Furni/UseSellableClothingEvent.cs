﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Items;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Catalog.Clothing;



using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Moon.Database.Interfaces;

namespace Moon.Communication.Packets.Incoming.Rooms.Furni
{
    class UseSellableClothingEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            int ItemId = Packet.PopInt();

            Item Item = Room.GetRoomItemHandler().GetItem(ItemId);
            if (Item == null)
                return;

            if (Item.Data == null)
                return;

            if (Item.UserID != Session.GetHabbo().Id)
                return;

            if (Item.Data.InteractionType != InteractionType.PURCHASABLE_CLOTHING)
            {
                Session.SendNotification("Oops, este artículo no se establece como una prenda de vestir vendible");
                return;
            }

            if (Item.Data.ClothingId == 0)
            {
                Session.SendNotification("Oops, este item no tiene la configuracion como una ropa, por favor, reportalo!");
                return;
            }

            ClothingItem Clothing = null;
            if (!MoonEnvironment.GetGame().GetCatalog().GetClothingManager().TryGetClothing(Item.Data.ClothingId, out Clothing))
            {
                Session.SendNotification("Vaya.. no se ha podido encontrar esta parte de la ropa!");
                return;
            }

            //Quickly delete it from the database.
            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM `items` WHERE `id` = @ItemId LIMIT 1");
                dbClient.AddParameter("ItemId", Item.Id);
                dbClient.RunQuery();
            }

            //Remove the item.
            Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id);

            Session.GetHabbo().GetClothing().AddClothing(Clothing.ClothingName, Clothing.PartIds);
            Session.SendMessage(new FigureSetIdsComposer(Session.GetHabbo().GetClothing().GetClothingAllParts));
            Session.SendMessage(new RoomNotificationComposer("figureset.redeemed.success"));
            Session.SendWhisper("Si por alguna razon no ve su nueva ropa, recarga el client y vuelve a ingresar!");
        }
    }
}
