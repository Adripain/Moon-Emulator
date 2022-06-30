using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.BuildersClub
{
    class BCBorrowedItemsComposer : ServerPacket
    {
        public BCBorrowedItemsComposer()
            : base(ServerPacketHeader.BCBorrowedItemsMessageComposer)
        {
            base.WriteInteger(0);
        }
    }
}
