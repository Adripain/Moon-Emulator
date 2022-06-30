using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Handshake
{
    class VideoOffersRewardsComposer : ServerPacket
    {
        public VideoOffersRewardsComposer(/*int Id, string Type, string Message*/)
            : base(ServerPacketHeader.VideoOffersRewardsMessageComposer)
        {
            base.WriteString("start_video");
            base.WriteInteger(0);
            base.WriteString("");
            base.WriteString("");
        }
    }
}

 