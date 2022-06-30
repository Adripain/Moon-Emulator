﻿using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;


using Moon.Communication.Packets.Outgoing.Marketplace;
using Moon.Database.Interfaces;

namespace Moon.Communication.Packets.Incoming.Marketplace
{
    class GetMarketplaceItemStatsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();
            int SpriteId = Packet.PopInt();

            DataRow Row = null;
            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `avgprice` FROM `catalog_marketplace_data` WHERE `sprite` = @SpriteId LIMIT 1");
                dbClient.AddParameter("SpriteId", SpriteId);
                Row = dbClient.getRow();
            }

            Session.SendMessage(new MarketplaceItemStatsComposer(ItemId, SpriteId, (Row != null ? Convert.ToInt32(Row["avgprice"]) : 0)));
        }
    }
}
