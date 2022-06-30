using System;

using Moon.Communication.Packets.Outgoing.Catalog;
using Moon.HabboHotel.Catalog;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Incoming;

namespace Moon.Communication.Packets.Incoming.Catalog
{
    public class GetCatalogPageEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int PageId = Packet.PopInt();
            int Something = Packet.PopInt();
            string CataMode = Packet.PopString();            

            CatalogPage Page = null;
            BCCatalogPage BCPage = null;

            if (CataMode == "NORMAL")
            {
                if (!MoonEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out Page))
                    return;

                if (!Page.Enabled || !Page.Visible || Page.MinimumRank > Session.GetHabbo().Rank || (Page.MinimumVIP > Session.GetHabbo().VIPRank && Session.GetHabbo().Rank == 1))
                    return;

                Session.SendMessage(new CatalogPageComposer(Page, CataMode, Session));
            }

            if (CataMode == "BUILDERS_CLUB")
            {
                if (!MoonEnvironment.GetGame().GetCatalog().TryGetBCPage(PageId, out BCPage))
                    return;

                if (!BCPage.Enabled || !BCPage.Visible || BCPage.MinimumRank > Session.GetHabbo().Rank || (BCPage.MinimumVIP > Session.GetHabbo().VIPRank && Session.GetHabbo().Rank == 1))
                    return;

                Session.SendMessage(new BCCatalogPageComposer(BCPage, CataMode));
            }

            Session.GetHabbo().lastLayout = Page.Template;
           
        }
    }
}