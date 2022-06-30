using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Incoming;

namespace Moon.Communication.Packets.Incoming.Handshake
{
    public class GetClientVersionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string Build = Packet.PopString();

            if (MoonEnvironment.SWFRevision != Build)
                MoonEnvironment.SWFRevision = Build;
        }
    }
}