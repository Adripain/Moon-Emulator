using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Groups.Forums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Groups
{
    class PostUpdatedComposer : ServerPacket
    {
        public PostUpdatedComposer(GameClient Session, GroupForumThreadPost Post)
            : base(ServerPacketHeader.PostUpdatedMessageComposer)
        {
            base.WriteInteger(Post.ParentThread.ParentForum.Id);
            base.WriteInteger(Post.ParentThread.Id);

            Post.SerializeData(this);
        }
    }
}
