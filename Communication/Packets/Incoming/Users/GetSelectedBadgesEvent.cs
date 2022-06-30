using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Users;
using Moon.Communication.Packets.Outgoing.Users;

namespace Moon.Communication.Packets.Incoming.Users
{
    class GetSelectedBadgesEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int UserId = Packet.PopInt();
            Habbo Habbo = MoonEnvironment.GetHabboById(UserId);
            if (Habbo == null)
                return;

            Session.GetHabbo().lastUserId = UserId;
            Session.SendMessage(new HabboUserBadgesComposer(Habbo));
        }
    }
}