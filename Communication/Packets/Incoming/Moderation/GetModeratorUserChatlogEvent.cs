using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Users;
using Moon.HabboHotel.Support;
using Moon.Communication.Packets.Outgoing.Moderation;

namespace Moon.Communication.Packets.Incoming.Moderation
{
    class GetModeratorUserChatlogEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            if (!Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                return;

            int UserId = Packet.PopInt();
            Habbo Habbo = MoonEnvironment.GetHabboById(UserId);

            if (Habbo == null)
            {
                Session.SendNotification("Oops, no se consigue este usuario");
                return;
            }

            try
            {
                Session.SendMessage(new ModeratorUserChatlogComposer(UserId));
            }
            catch { Session.SendNotification("Overflow :/"); }
        }
    }
}
