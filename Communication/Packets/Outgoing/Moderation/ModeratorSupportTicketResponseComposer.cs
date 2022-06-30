using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Moderation
{
    class ModeratorSupportTicketResponseComposer : ServerPacket
    {
        public ModeratorSupportTicketResponseComposer(int Result)
            : base(ServerPacketHeader.ModeratorSupportTicketResponseMessageComposer)
        {
            base.WriteInteger(Result);
           base.WriteString("");
        }
    }
}