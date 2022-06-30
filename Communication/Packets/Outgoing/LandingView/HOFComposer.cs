using Moon.Communication.Packets.Incoming.LandingView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.LandingView
{
    class HOFComposer : ServerPacket
    {
        public HOFComposer()
            : base(ServerPacketHeader.HOFComposer)
        {
            base.WriteString("haddoRanks");
            GetHallOfFame.getInstance().Serialize(this);
            return;
        }
    }
}
