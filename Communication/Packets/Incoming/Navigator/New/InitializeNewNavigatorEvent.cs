using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Navigator;
using Moon.Communication.Packets.Outgoing.Navigator;

namespace Moon.Communication.Packets.Incoming.Navigator
{
    class InitializeNewNavigatorEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            ICollection<TopLevelItem> TopLevelItems = MoonEnvironment.GetGame().GetNavigator().GetTopLevelItems();
            ICollection<SearchResultList> SearchResultLists = MoonEnvironment.GetGame().GetNavigator().GetSearchResultLists();

            Session.SendMessage(new NavigatorMetaDataParserComposer(TopLevelItems));
            Session.SendMessage(new NavigatorLiftedRoomsComposer());
            Session.SendMessage(new NavigatorCollapsedCategoriesComposer());
            Session.SendMessage(new NavigatorPreferencesComposer());
        }
    }
}
