using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Notifications
{
    class HotelClosedAndOpensComposer : ServerPacket
    {
        public HotelClosedAndOpensComposer(int Hour, int Minute)
            : base(ServerPacketHeader.HotelClosedAndOpensComposer)
        {
            base.WriteInteger(Hour);
            base.WriteInteger(Minute);
        }
    }
}