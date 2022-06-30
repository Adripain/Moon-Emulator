using Moon.Communication.Packets.Outgoing.Catalog;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Incoming;

namespace Moon.Communication.Packets.Incoming.Catalog
{
    public class GetMarketplaceConfigurationEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new MarketplaceConfigurationComposer());
        }
    }
}