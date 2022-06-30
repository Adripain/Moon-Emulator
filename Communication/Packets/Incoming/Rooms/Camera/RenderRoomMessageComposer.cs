using Moon.Communication.Packets.Outgoing;
using Moon.HabboHotel.GameClients;

namespace Moon.Communication.Packets.Incoming.Rooms.Camera
{
    public class RenderRoomMessageComposer : ServerPacket
    {
        public RenderRoomMessageComposer()
            : base(ServerPacketHeader.TakedRoomPhoto)
        {

        }
    }

    public class RenderRoomMessageComposerEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket paket)
        {
            Session.SendMessage(new RenderRoomMessageComposer());
        }
    }
}