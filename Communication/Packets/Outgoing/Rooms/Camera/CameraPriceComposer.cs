using System;

namespace Moon.Communication.Packets.Outgoing.Rooms.Camera
{
    public class CameraPriceComposer : ServerPacket
    {
        public CameraPriceComposer(int BuyPicCreditCost, int BuyPicDucketCost, int PublishPicDucketCost)
            : base(ServerPacketHeader.CameraPriceComposer)
        {
            base.WriteInteger(BuyPicCreditCost);
            base.WriteInteger(BuyPicDucketCost);
            base.WriteInteger(PublishPicDucketCost);
        }
    }
}