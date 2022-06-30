using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Users.Inventory.Bots;
using Moon.Communication.Packets.Outgoing.Inventory.Bots;

namespace Moon.Communication.Packets.Incoming.Inventory.Bots
{
    class GetBotInventoryEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().GetInventoryComponent() == null)
                return;

            ICollection<Bot> Bots = Session.GetHabbo().GetInventoryComponent().GetBots();
            Session.SendMessage(new BotInventoryComposer(Bots));
        }
    }
}
