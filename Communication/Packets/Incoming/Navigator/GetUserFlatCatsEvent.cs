using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.Communication.Packets.Incoming;
using Moon.HabboHotel.Users;
using Moon.HabboHotel.Navigator;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Navigator;

namespace Moon.Communication.Packets.Incoming.Navigator
{
    public class GetUserFlatCatsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null)
                return;

            ICollection<SearchResultList> Categories = MoonEnvironment.GetGame().GetNavigator().GetFlatCategories();

            Session.SendMessage(new UserFlatCatsComposer(Categories, Session.GetHabbo().Rank));
        }
    }
}