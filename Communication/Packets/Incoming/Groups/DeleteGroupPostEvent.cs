using Moon.Communication.Packets.Outgoing.Groups;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Incoming.Groups
{
    class DeleteGroupPostEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var forumId = Packet.PopInt();
            var threadId = Packet.PopInt();
            var postId = Packet.PopInt();
            var deleteLevel = Packet.PopInt();

            var forum = MoonEnvironment.GetGame().GetGroupForumManager().GetForum(forumId);

            var thread = forum.GetThread(threadId);

            var post = thread.GetPost(postId);

            post.DeletedLevel = deleteLevel / 10;
            post.DeleterId = Session.GetHabbo().Id;
            post.Save();
            Session.SendMessage(new PostUpdatedComposer(Session, post));

            if (post.DeletedLevel != 0)
                Session.SendMessage(new RoomNotificationComposer("forums.message.hidden"));
            else
                Session.SendMessage(new RoomNotificationComposer("forums.message.restored"));
        }
    }
}
