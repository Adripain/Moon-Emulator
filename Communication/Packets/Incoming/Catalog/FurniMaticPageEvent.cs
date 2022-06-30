using Moon.Communication.Packets.Outgoing;

namespace Moon.Communication.Packets.Incoming.Catalog
{
    class FurniMaticPageEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null) return;
            var response = new ServerPacket(ServerPacketHeader.FurniMaticNoRoomError);
            response.WriteInteger(1);
            response.WriteInteger(0);
            Session.SendMessage(response);
        }
    }
}