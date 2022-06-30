using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Handshake
{
    class GenericErrorComposer : ServerPacket
    {
        public GenericErrorComposer(int errorId)
            : base(ServerPacketHeader.GenericErrorMessageComposer)
        {
            base.WriteInteger(errorId);
        }
    }
}
