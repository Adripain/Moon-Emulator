using System;

namespace Moon.Communication.Packets.Outgoing.Rooms.Camera
{
    public class CameraPhotoPreviewComposer : ServerPacket
    {
        public CameraPhotoPreviewComposer(string ImageUrl)
            : base(ServerPacketHeader.CameraToken)
        {
            base.WriteString(ImageUrl);
        }
    }
}