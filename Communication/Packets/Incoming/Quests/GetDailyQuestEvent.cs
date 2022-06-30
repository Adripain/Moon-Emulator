//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Moon.HabboHotel.GameClients;
//using Moon.Communication.Packets.Outgoing.LandingView;

//namespace Moon.Communication.Packets.Incoming.Quests
//{
//    class GetDailyQuestEvent : IPacketEvent
//    {
//        public void Parse(GameClient Session, ClientPacket Packet)
//        {
//            int UsersOnline = MoonEnvironment.GetGame().GetClientManager().Count;

//            Session.SendMessage(new ConcurrentUsersGoalProgressComposer(UsersOnline));
//        }
//    }
//}
