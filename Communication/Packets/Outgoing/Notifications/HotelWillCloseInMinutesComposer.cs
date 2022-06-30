using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Notifications
{
    class HotelWillCloseInMinutesComposer : ServerPacket
    {
        public HotelWillCloseInMinutesComposer(int Minutes)
            : base(ServerPacketHeader.HotelWillCloseInMinutesComposer)
        {
            base.WriteInteger(Minutes);
        }
    }
}