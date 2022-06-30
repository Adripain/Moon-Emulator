using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Groups
{
    class UnreadForumThreadPostsComposer : ServerPacket
    {
        public UnreadForumThreadPostsComposer(int count)
            : base(ServerPacketHeader.UnreadForumThreadPostsMessageComposer)
        {
            base.WriteInteger(count);
        }
    }
}


