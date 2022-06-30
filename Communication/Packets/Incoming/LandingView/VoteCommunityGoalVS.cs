using Moon.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Incoming.LandingView
{
    class VoteCommunityGoalVS : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int VoteType = Packet.PopInt(); // 1 izq, 2 der

            if (VoteType == 1)
            {
                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.runFastQuery("UPDATE landing_communitygoalvs SET left_votes = left_votes + 1 WHERE id = " + MoonEnvironment.GetGame().GetCommunityGoalVS().GetId());
                }

                MoonEnvironment.GetGame().GetCommunityGoalVS().IncreaseLeftVotes();
            }
            else if (VoteType == 2)
            {
                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.runFastQuery("UPDATE landing_communitygoalvs SET right_votes = right_votes + 1 WHERE id = " + MoonEnvironment.GetGame().GetCommunityGoalVS().GetId());
                }

                MoonEnvironment.GetGame().GetCommunityGoalVS().IncreaseRightVotes();
            }
            MoonEnvironment.GetGame().GetCommunityGoalVS().LoadCommunityGoalVS();
        }
    }
}
