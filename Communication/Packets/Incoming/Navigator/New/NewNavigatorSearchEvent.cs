using System.Collections.Generic;

using Moon.HabboHotel.Navigator;
using Moon.Communication.Packets.Outgoing.Navigator;

namespace Moon.Communication.Packets.Incoming.Navigator
{
    class NewNavigatorSearchEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            string Category = packet.PopString();
            string Search = packet.PopString();

            ICollection<SearchResultList> Categories = new List<SearchResultList>();

            if (!string.IsNullOrEmpty(Search))
            {
                SearchResultList QueryResult = null;
                if (MoonEnvironment.GetGame().GetNavigator().TryGetSearchResultList(0, out QueryResult))
                {
                    Categories.Add(QueryResult);
                }
            }
            else
            {
                Categories = MoonEnvironment.GetGame().GetNavigator().GetCategorysForSearch(Category);
                if (Categories.Count == 0)
                {
                    //Are we going in deep?!
                    Categories = MoonEnvironment.GetGame().GetNavigator().GetResultByIdentifier(Category);
                    if (Categories.Count > 0)
                    {
                        session.SendMessage(new NavigatorSearchResultSetComposer(Category, Search, Categories, session, 2, 100));
                        return;
                    }
                }
            }

            session.SendMessage(new NavigatorSearchResultSetComposer(Category, Search, Categories, session));
        }
    }
}
