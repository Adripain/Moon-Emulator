using Moon.Communication.Packets.Outgoing.Groups;
using Moon.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Incoming.Groups
{
    class GetThreadDataEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var ForumId = Packet.PopInt(); //Maybe Forum ID
            var ThreadId = Packet.PopInt(); //Maybe Thread ID
            var StartIndex = Packet.PopInt(); //Start index
            var length = Packet.PopInt(); //List Length

            var Forum = MoonEnvironment.GetGame().GetGroupForumManager().GetForum(ForumId);

            if (Forum == null)
            {
                Session.SendNotification(";forum.thread.open.error.forumnotfound");
                return;
            }

            var Thread = Forum.GetThread(ThreadId);
            if (Thread == null)
            {
                Session.SendNotification(";forum.thread.open.error.threadnotfound");
                return;
            }

            if (Thread.DeletedLevel > 1 && (Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanModerate) != ""))
            {
                Session.SendNotification((";forum.thread.open.error.deleted"));
                return;
            }


            Session.SendMessage(new ThreadDataComposer(Thread, StartIndex, length));

        }
    }
}
