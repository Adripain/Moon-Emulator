using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Groups.Forums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Groups
{
    class GetGroupForumsMessageEvent : ServerPacket
    {
        public GetGroupForumsMessageEvent(GroupForum Forum, GameClient Session)
            : base(ServerPacketHeader.GroupForumDataMessageComposer)
        {
            base.WriteInteger(Forum.Id);
            base.WriteString(Forum.Group.Name); //Group Name
            base.WriteString(Forum.Group.Description); // idk
            base.WriteString(Forum.Group.Badge); //Group Badge code

            base.WriteInteger(Forum.Threads.Count); //Forum Thread Count
            base.WriteInteger(0); //Last Author ID
            base.WriteInteger(0); //Score ?
            base.WriteInteger(0); //Last Thread Mark
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteString("not_member");
            base.WriteInteger(0);
            //end fillFromMEssage func

            base.WriteInteger(Forum.Settings.WhoCanRead); //Who can read 
            base.WriteInteger(Forum.Settings.WhoCanPost); // Who can post
            base.WriteInteger(Forum.Settings.WhoCanInitDiscussions); //Who can make threads
            base.WriteInteger(Forum.Settings.WhoCanModerate); //Who can MOD

            //Permissions i think
            base.WriteString(Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanRead)); //Forum disabled reason//base.WriteString(Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanRead)); 
            base.WriteString(Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanPost)); //Can't reply reason
            base.WriteString(Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanInitDiscussions));// Can't Post reason
            base.WriteString(Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanModerate)); //Can't moderate thread posts reason
            base.WriteString("");

            base.WriteBoolean(Forum.Group.CreatorId == Session.GetHabbo().Id); // Is Owner
            base.WriteBoolean(Forum.Group.IsAdmin(Session.GetHabbo().Id) && Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanModerate) == ""); // Is admin

        }
    }
}
