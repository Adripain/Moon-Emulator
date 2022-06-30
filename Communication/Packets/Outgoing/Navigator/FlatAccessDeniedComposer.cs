using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Navigator
{
    class FlatAccessDeniedComposer : ServerPacket
    {
        public FlatAccessDeniedComposer(string Username)
            : base(ServerPacketHeader.FlatAccessDeniedMessageComposer)
        {
           base.WriteString(Username);
        }
    }
}
