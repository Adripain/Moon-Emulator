using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.LandingView;
using Moon.HabboHotel.LandingView.Promotions;
using Moon.Communication.Packets.Outgoing.LandingView;

namespace Moon.Communication.Packets.Incoming.LandingView
{
    class GetPromoArticlesEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            ICollection<Promotion> LandingPromotions = MoonEnvironment.GetGame().GetLandingManager().GetPromotionItems();

            Session.SendMessage(new PromoArticlesComposer(LandingPromotions));
        }
    }
}
