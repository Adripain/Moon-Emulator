using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Groups.Forums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Groups
{
    class ForumsListDataComposer : ServerPacket
    {
        public ForumsListDataComposer(ICollection<GroupForum> Forums, GameClient Session, int ViewOrder = 0, int StartIndex = 0, int MaxLength = 20)
            : base(ServerPacketHeader.ForumsListDataMessageComposer)
        {
            base.WriteInteger(ViewOrder);
            base.WriteInteger(StartIndex);
            base.WriteInteger(StartIndex);

            base.WriteInteger(Forums.Count); // Forum List Count

            foreach (var Forum in Forums)
            {
                var lastpost = Forum.GetLastPost();
                var isn = lastpost == null;
                base.WriteInteger(Forum.Id); //Maybe ID
                base.WriteString(Forum.Name); //Forum name
                base.WriteString(Forum.Description); //idk
                base.WriteString(Forum.Group.Badge); // Group Badge
                base.WriteInteger(0);//Idk
                base.WriteInteger(0);// Score
                base.WriteInteger(Forum.MessagesCount);//Message count
                base.WriteInteger(Forum.UnreadMessages(Session.GetHabbo().Id));//unread message count
                base.WriteInteger(0);//Idk
                base.WriteInteger(!isn ? lastpost.GetAuthor().Id : 0);// Las user to message id
                base.WriteString(!isn ? lastpost.GetAuthor().Username : ""); //Last user to message name
                base.WriteInteger(!isn ? (int)MoonEnvironment.GetUnixTimestamp() - lastpost.Timestamp : 0); //Last message timestamp
            }
        }
    }
}
