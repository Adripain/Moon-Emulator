using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Incoming;
using Moon.Communication.Packets.Outgoing.Navigator;
using Moon.HabboHotel.Navigator;

namespace Moon.Communication.Packets.Incoming.Navigator
{
    class GetNavigatorFlatsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            ICollection<SearchResultList> Categories = MoonEnvironment.GetGame().GetNavigator().GetEventCategories();

            Session.SendMessage(new NavigatorFlatCatsComposer(Categories, Session.GetHabbo().Rank));
        }
    }
}