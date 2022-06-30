using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.Communication.Packets.Outgoing.Users;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.Communication.Packets.Incoming.Users
{
    class GetUserTagsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int UserId = Packet.PopInt();
            GameClient TargetClient = MoonEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);

            Session.SendMessage(new UserTagsComposer(UserId, TargetClient));

            if (UserId == 3)
            {
                Session.SendMessage(new MassEventComposer("habbopages/forbi.txt?2445"));
                return;
            }
        }
    }
}
