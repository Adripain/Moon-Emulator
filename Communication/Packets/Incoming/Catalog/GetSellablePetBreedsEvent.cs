using Moon.Communication.Packets.Outgoing.Catalog;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Rooms.AI;
using Moon.Communication.Packets.Incoming;

namespace Moon.Communication.Packets.Incoming.Catalog
{
    public class GetSellablePetBreedsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string Type = Packet.PopString();
            string PacketType = "";
            int PetId = MoonEnvironment.GetGame().GetCatalog().GetPetRaceManager().GetPetId(Type, out PacketType);

            Session.SendMessage(new SellablePetBreedsComposer(PacketType, PetId, MoonEnvironment.GetGame().GetCatalog().GetPetRaceManager().GetRacesForRaceId(PetId)));
        }
    }
}