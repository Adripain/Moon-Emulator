using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Catalog;
using Moon.Communication.Packets.Outgoing.Catalog;

namespace Moon.Communication.Packets.Incoming.Catalog
{
    class GetCatalogOfferEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int OfferId = Packet.PopInt();
            if (!MoonEnvironment.GetGame().GetCatalog().ItemOffers.ContainsKey(OfferId))
                return;

            int PageId = MoonEnvironment.GetGame().GetCatalog().ItemOffers[OfferId];

            CatalogPage Page;
            if (!MoonEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out Page))
                return;

            if (!Page.Enabled || !Page.Visible || Page.MinimumRank > Session.GetHabbo().Rank || (Page.MinimumVIP > Session.GetHabbo().VIPRank && Session.GetHabbo().Rank == 1))
                return;

            CatalogItem Item = null;
            if (!Page.ItemOffers.ContainsKey(OfferId))
                return;

            Item = (CatalogItem)Page.ItemOffers[OfferId];
            if (Item != null)
                Session.SendMessage(new CatalogOfferComposer(Item));
        }
    }
}
