using System;

using Moon.Utilities;
using Moon.HabboHotel.Moderation;

namespace Moon.Communication.Packets.Outgoing.Moderation
{
    class ModeratorSupportTicketComposer : ServerPacket
    {
        public ModeratorSupportTicketComposer(int Id, ModerationTicket Ticket)
          : base(ServerPacketHeader.ModeratorSupportTicketMessageComposer)
        {
            base.WriteInteger(Ticket.Id); // Id
            base.WriteInteger(Ticket.GetStatus(Id)); // Tab ID
            base.WriteInteger(Ticket.Type); // Type
            base.WriteInteger(Ticket.Category); // Category 
            base.WriteInteger(Convert.ToInt32((DateTime.Now - UnixTimestamp.FromUnixTimestamp(Ticket.Timestamp)).TotalMilliseconds)); // Timestamp
            base.WriteInteger(Ticket.Priority); // Priority
            base.WriteInteger(0); //
            base.WriteInteger(Ticket.Sender == null ? 0 : Ticket.Sender.Id); // Sender ID
                                                                             //base.WriteInteger(1);
            base.WriteString(Ticket.Sender == null ? string.Empty : Ticket.Sender.Username); // Sender Name
            base.WriteInteger(Ticket.Reported == null ? 0 : Ticket.Reported.Id); // Reported ID
            base.WriteString(Ticket.Reported == null ? string.Empty : Ticket.Reported.Username); // Reported Name
            base.WriteInteger(Ticket.Moderator == null ? 0 : Ticket.Moderator.Id); // Moderator ID
            base.WriteString(Ticket.Moderator == null ? string.Empty : Ticket.Moderator.Username); // Mod Name
            base.WriteString(Ticket.Issue); // Issue
            base.WriteInteger(Ticket.Room == null ? 0 : Ticket.Room.Id); // Room Id
            base.WriteInteger(0);
            {
            }
        }
    }
}