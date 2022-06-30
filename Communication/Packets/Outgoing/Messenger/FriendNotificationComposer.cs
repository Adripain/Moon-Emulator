using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Users;
using Moon.HabboHotel.Users.Messenger;

namespace Moon.Communication.Packets.Outgoing.Messenger
{
    class FriendNotificationComposer : ServerPacket
    {
        public FriendNotificationComposer(int UserId, MessengerEventTypes type, string data)
            : base(ServerPacketHeader.FriendNotificationMessageComposer)
        {
            base.WriteString(UserId.ToString());
            base.WriteInteger(MessengerEventTypesUtility.GetEventTypePacketNum(type));
            base.WriteString(data);
        }
    }
}
