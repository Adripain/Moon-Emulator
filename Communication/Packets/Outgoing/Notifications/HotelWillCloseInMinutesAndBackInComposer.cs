using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Notifications
{
    class HotelWillCloseInMinutesAndBackInComposer : ServerPacket
    {
        public HotelWillCloseInMinutesAndBackInComposer(int CloseIn, int OpenIn)
            : base(ServerPacketHeader.HotelWillCloseInMinutesAndBackInComposer)
        {
            base.WriteBoolean(true);
            base.WriteInteger(CloseIn);
            base.WriteInteger(OpenIn);
        }
    }
}