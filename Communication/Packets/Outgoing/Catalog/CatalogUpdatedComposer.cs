using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Catalog
{
    class CatalogUpdatedComposer : ServerPacket
    {
        public CatalogUpdatedComposer()
            : base(ServerPacketHeader.CatalogUpdatedMessageComposer)
        {
            base.WriteBoolean(false);
        }
    }
}
