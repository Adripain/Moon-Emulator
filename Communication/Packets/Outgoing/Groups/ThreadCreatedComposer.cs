using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Groups.Forums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Groups
{
    public class ThreadCreatedComposer : ServerPacket
    {
        public ThreadCreatedComposer(GameClient Session, GroupForumThread Thread)
            : base(ServerPacketHeader.ThreadCreatedMessageComposer)
        {

            base.WriteInteger(Thread.ParentForum.Id); //Thread ID
            Thread.SerializeData(Session, this);
            /*
            base.WriteInteger(Thread.Id); //Thread ID
            base.WriteInteger(Thread.GetAuthor().Id);
            base.WriteString(Thread.GetAuthor().Username); //Thread Author
            base.WriteString(Thread.Caption); //Thread Title
            base.WriteBoolean(false); //Pinned
            base.WriteBoolean(false); //Locked
            base.WriteInteger((int)(MoonEnvironment.GetUnixTimestamp() - Thread.Timestamp)); //Created Secs Ago
            base.WriteInteger(Thread.Posts.Count); //Message count
            base.WriteInteger(Thread.GetUnreadMessages(Session.GetHabbo().Id)); //Unread message count
            base.WriteInteger(0); // idk
            base.WriteInteger(0); // idk

            base.WriteString("Unknown");// Last User Post Username
            base.WriteInteger(65); // Last User Post time ago [Sec]

            base.WriteByte(0); //idk
            base.WriteInteger(10);// idk
            base.WriteString("Str4"); //idk
            base.WriteInteger(11);//idk*/
        }
    }
}
