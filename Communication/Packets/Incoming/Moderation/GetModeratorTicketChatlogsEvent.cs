using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Moderation;
using Moon.Communication.Packets.Outgoing.Moderation;

namespace Moon.Communication.Packets.Incoming.Moderation
{
    class GetModeratorTicketChatlogsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().GetPermissions().HasRight("mod_tickets"))
                return;

            int TicketId = Packet.PopInt();

            ModerationTicket Ticket = null;
            if (!MoonEnvironment.GetGame().GetModerationManager().TryGetTicket(TicketId, out Ticket) || Ticket.Room == null)
                return;

            RoomData Data = MoonEnvironment.GetGame().GetRoomManager().GenerateRoomData(Ticket.Room.Id);
            if (Data == null)
                return;

            Session.SendMessage(new ModeratorTicketChatlogComposer(Ticket, Data, Ticket.Timestamp));
        }
    }
}
