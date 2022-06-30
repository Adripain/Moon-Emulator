using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Groups.Forums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Groups
{
    class ThreadReplyComposer : ServerPacket
    {
        public ThreadReplyComposer(GameClient Session, GroupForumThreadPost Post)
            : base(ServerPacketHeader.ThreadReplyMessageComposer)
        {
            var User = Post.GetAuthor();
            base.WriteInteger(Post.ParentThread.ParentForum.Id);
            base.WriteInteger(Post.ParentThread.Id);

            base.WriteInteger(Post.Id); //Post Id
            base.WriteInteger(Post.ParentThread.Posts.IndexOf(Post)); //Post Index

            base.WriteInteger(User.Id); //User id
            base.WriteString(User.Username); //Username
            base.WriteString(User.Look); //User look

            base.WriteInteger((int)(MoonEnvironment.GetUnixTimestamp() - Post.Timestamp)); //User message timestamp
            base.WriteString(Post.Message); // Message text
            base.WriteByte(0); // User message oculted by - level
            base.WriteInteger(0); // User that oculted message ID
            base.WriteString(""); //Oculted message user name
            base.WriteInteger(10);
            base.WriteInteger(Post.ParentThread.GetUserPosts(User.Id).Count); //User messages count
        }
    }
}
