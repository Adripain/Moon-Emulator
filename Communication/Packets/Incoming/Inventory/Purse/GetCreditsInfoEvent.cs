using System;
using System.Linq;
using System.Text;

using Moon.Communication.Packets.Incoming;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Inventory.Purse;

namespace Moon.Communication.Packets.Incoming.Inventory.Purse
{
    class GetCreditsInfoEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
            Session.SendMessage(new ActivityPointsComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Diamonds, Session.GetHabbo().GOTWPoints));
        }
    }
}
