using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Help.Helpers
{
    class HelperSessionChatIsTypingComposer : ServerPacket
    {
        public HelperSessionChatIsTypingComposer(bool typing)
            : base(ServerPacketHeader.HelperSessionChatIsTypingMessageComposer)
        {
            base.WriteBoolean(typing);
        }
    }
}
