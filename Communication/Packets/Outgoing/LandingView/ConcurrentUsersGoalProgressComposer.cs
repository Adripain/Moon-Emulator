using Moon.Core;
using Moon.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.LandingView
{
    class ConcurrentUsersGoalProgressComposer : ServerPacket
    {
        public ConcurrentUsersGoalProgressComposer(int UsersNow, int type, int goal)
            : base(ServerPacketHeader.ConcurrentUsersGoalProgressMessageComposer)
        {
            base.WriteInteger(type);
            base.WriteInteger(UsersNow);
            base.WriteInteger(goal);
        }
    }
}
