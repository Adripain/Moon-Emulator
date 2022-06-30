using Moon.Communication.Packets.Incoming;
using Moon.HabboHotel.GameClients;

namespace Moon.Communication.Packets
{
    public interface IPacketEvent
    {
        void Parse(GameClient Session, ClientPacket Packet);
    }
}