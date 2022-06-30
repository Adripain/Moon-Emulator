using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Moon.Communication.Packets.Outgoing.Quests
{
    class QuestAbortedComposer : ServerPacket
    {
        public QuestAbortedComposer()
            : base(ServerPacketHeader.QuestAbortedMessageComposer)
        {
            base.WriteBoolean(false);
        }
    }
}
