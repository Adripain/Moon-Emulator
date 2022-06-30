using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Catalog;
using Moon.Communication.Packets.Outgoing.Catalog;
using Moon.Communication.Packets.Outgoing.BuildersClub;

namespace Moon.Communication.Packets.Incoming.Catalog
{
    class GetCatalogModeEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            string PageMode = Packet.PopString();

            if (PageMode == "NORMAL")
                Session.SendMessage(new CatalogIndexComposer(Session, MoonEnvironment.GetGame().GetCatalog().GetPages(), PageMode));//, Sub));
            else if (PageMode == "BUILDERS_CLUB")
                Session.SendMessage(new CatalogIndexComposer(Session, MoonEnvironment.GetGame().GetCatalog().GetBCPages(), PageMode));
        }
    }
}
