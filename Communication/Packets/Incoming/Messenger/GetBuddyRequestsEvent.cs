using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Users;
using Moon.HabboHotel.Users.Messenger;
using Moon.Communication.Packets.Outgoing.Messenger;

namespace Moon.Communication.Packets.Incoming.Messenger
{
    class GetBuddyRequestsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            ICollection<MessengerRequest> Requests = Session.GetHabbo().GetMessenger().GetRequests().ToList();

            Session.SendMessage(new BuddyRequestsComposer(Requests));
        }
    }
}
