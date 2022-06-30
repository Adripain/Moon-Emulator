using System;
using Moon.Communication.Packets.Incoming;

using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Catalog;
using Moon.Communication.Packets.Outgoing.BuildersClub;

namespace Moon.Communication.Packets.Incoming.Catalog
{
    public class GetCatalogIndexEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {

            Session.SendMessage(new CatalogIndexComposer(Session, MoonEnvironment.GetGame().GetCatalog().GetPages(), "NORMAL"));
            Session.SendMessage(new CatalogIndexComposer(Session, MoonEnvironment.GetGame().GetCatalog().GetBCPages(), "BUILDERS_CLUB"));

            Session.SendMessage(new CatalogItemDiscountComposer());
            Session.SendMessage(new BCBorrowedItemsComposer());
        }
    }
}