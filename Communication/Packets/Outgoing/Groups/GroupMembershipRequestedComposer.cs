using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Moon.HabboHotel.Users;

namespace Moon.Communication.Packets.Outgoing.Groups
{
    class GroupMembershipRequestedComposer : ServerPacket
    {
        public GroupMembershipRequestedComposer(int GroupId, Habbo Habbo, int Type) 
            : base(ServerPacketHeader.GroupMembershipRequestedMessageComposer)
        {
            base.WriteInteger(GroupId);//GroupId
            base.WriteInteger(Type);//Type?
            {
                base.WriteInteger(Habbo.Id);//UserId
                base.WriteString(Habbo.Username);
                base.WriteString(Habbo.Look);
                base.WriteString(string.Empty);
            }
        }
    }
}
