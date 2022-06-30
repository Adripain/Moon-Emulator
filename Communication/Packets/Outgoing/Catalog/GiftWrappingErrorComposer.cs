using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Catalog
{
    class GiftWrappingErrorComposer : ServerPacket
    {
        public GiftWrappingErrorComposer()
            : base(ServerPacketHeader.GiftWrappingErrorMessageComposer)
        {

        }
    }
}
