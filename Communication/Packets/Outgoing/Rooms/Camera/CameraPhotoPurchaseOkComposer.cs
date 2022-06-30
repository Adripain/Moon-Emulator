using System;

namespace Moon.Communication.Packets.Outgoing.Rooms.Camera
{
    public class CameraPhotoPurchaseOkComposer : ServerPacket
    {
        public CameraPhotoPurchaseOkComposer()
            : base(ServerPacketHeader.CameraPhotoPurchaseOkComposer)
        {
        }
    }
}