using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Rooms.Poll
{
    class QuickPollResultsMessageComposer : ServerPacket
    {
        public QuickPollResultsMessageComposer(int yesVotesCount, int noVotesCount)
            : base(ServerPacketHeader.QuickPollResultsMessageComposer)
        {
            base.WriteInteger(-1);
            base.WriteInteger(2);
            base.WriteString("1");
            base.WriteInteger(yesVotesCount);

            base.WriteString("0");
            base.WriteInteger(noVotesCount);
        }
    }
}