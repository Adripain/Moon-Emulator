using Moon.Communication.Packets.Outgoing.Groups;
using Moon.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Incoming.Groups
{
    class PostGroupContentEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var ForumId = Packet.PopInt();
            var ThreadId = Packet.PopInt();
            var Caption = Packet.PopString();
            var Message = Packet.PopString();

            var Forum = MoonEnvironment.GetGame().GetGroupForumManager().GetForum(ForumId);
            if (Forum == null)
            {
                Session.SendNotification(";forum.thread.post.reply.error.forumnotfound");
                return;
            }
            var e = "";
            var IsNewThread = ThreadId == 0;
            if (IsNewThread)
            {

                if ((e = Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanInitDiscussions)) != "")
                {
                    Session.SendNotification(";forum.thread.create.error." + e);
                    return;
                }

                var Thread = Forum.CreateThread(Session.GetHabbo().Id, Caption);
                var Post = Thread.CreatePost(Session.GetHabbo().Id, Message);

                Session.SendMessage(new ThreadCreatedComposer(Session, Thread));

                Session.SendMessage(new ThreadDataComposer(Thread, 0, 2000));

                //Session.SendMessage(new PostUpdatedComposer(Session, Post));
                //Session.SendMessage(new ThreadReplyComposer(Session, Post));

                Thread.AddView(Session.GetHabbo().Id, 1);

            }
            else
            {
                var Thread = Forum.GetThread(ThreadId);
                if (Thread == null)
                {
                    Session.SendNotification(";forum.thread.post.reply.error.threadnotfound");
                    return;
                }

                if (Thread.Locked && Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanModerate) != "")
                {
                    Session.SendNotification(";forum.thread.post.reply.error.locked");
                    return;
                }

                if ((e = Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanPost)) != "")
                {
                    Session.SendNotification(";forum.thread.post.reply.error." + e);
                    return;
                }

                var Post = Thread.CreatePost(Session.GetHabbo().Id, Message);
                Session.SendMessage(new ThreadReplyComposer(Session, Post));
                MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_Forum", 1);
            }


        }
    }
}
