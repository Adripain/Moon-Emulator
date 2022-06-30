using Moon.HabboHotel.Moderation;
using Moon.Communication.Packets.Outgoing.Moderation;

namespace Moon.Communication.Packets.Incoming.Moderation
{
    class PickTicketEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                return;

            int Junk = Packet.PopInt();//??
            int TicketId = Packet.PopInt();

            ModerationTicket Ticket = null;
            if (!MoonEnvironment.GetGame().GetModerationManager().TryGetTicket(TicketId, out Ticket))
                return;

            Ticket.Moderator = Session.GetHabbo();
            MoonEnvironment.GetGame().GetClientManager().SendMessage(new ModeratorSupportTicketComposer(Session.GetHabbo().Id, Ticket), "mod_tool");
        }
    }
}
