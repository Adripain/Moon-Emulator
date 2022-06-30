using Moon.Communication.Packets.Outgoing.LandingView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Incoming.LandingView
{
    class CommunityGoalEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new CommunityGoalComposer());
            Session.SendMessage(new DynamicPollLandingComposer(false)); //false pa q pueda votar
        }
    }
}
