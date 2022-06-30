using Moon.Communication.Packets.Outgoing.Moderation;
using Moon.HabboHotel.Moderation;

namespace Moon.Communication.Packets.Incoming.Moderation
{
    class ReleaseTicketEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                return;

            int Amount = Packet.PopInt();

            for (int i = 0; i < Amount; i++)
            {
                ModerationTicket Ticket = null;
                if (!MoonEnvironment.GetGame().GetModerationManager().TryGetTicket(Packet.PopInt(), out Ticket))
                    continue;

                Ticket.Moderator = null;
                MoonEnvironment.GetGame().GetClientManager().SendMessage(new ModeratorSupportTicketComposer(Session.GetHabbo().Id, Ticket), "mod_tool");
            }
        }
    }
}