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
    class DeleteGroupThreadEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var int1 = Packet.PopInt();
            var int2 = Packet.PopInt();
            var int3 = Packet.PopInt();

            var forum = MoonEnvironment.GetGame().GetGroupForumManager().GetForum(int1);

            if (forum == null)
            {
                Session.SendNotification(("forums.thread.delete.error.forumnotfound"));
                return;
            }

            if (forum.Settings.GetReasonForNot(Session, forum.Settings.WhoCanModerate) != "")
            {
                Session.SendNotification(("forums.thread.delete.error.rights"));
                return;
            }

            var thread = forum.GetThread(int2);
            if (thread == null)
            {
                Session.SendNotification(("forums.thread.delete.error.threadnotfound"));
                return;
            }

            thread.DeletedLevel = int3 / 10;

            thread.DeleterUserId = thread.DeletedLevel != 0 ? Session.GetHabbo().Id : 0;

            thread.Save();

            Session.SendMessage(new ThreadsListDataComposer(forum, Session));

            if (thread.DeletedLevel != 0)
                Session.SendMessage(new RoomNotificationComposer("forums.thread.hidden"));
            else
                Session.SendMessage(new RoomNotificationComposer("forums.thread.restored"));
        }
    }
}
