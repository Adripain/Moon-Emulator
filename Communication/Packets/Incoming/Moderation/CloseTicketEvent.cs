using Moon.HabboHotel.Users;
using Moon.HabboHotel.Moderation;
using Moon.Communication.Packets.Outgoing.Moderation;
using Moon.HabboHotel.GameClients;
using Moon.Database.Interfaces;

namespace Moon.Communication.Packets.Incoming.Moderation
{
    class CloseTicketEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                return;

            int Result = Packet.PopInt(); // 1 = useless, 2 = abusive, 3 = resolved
            int Junk = Packet.PopInt();
            int TicketId = Packet.PopInt();

            ModerationTicket Ticket = null;
            if (!MoonEnvironment.GetGame().GetModerationManager().TryGetTicket(TicketId, out Ticket))
                return;

            if (Ticket.Moderator.Id != Session.GetHabbo().Id)
                return;

            GameClient Client = MoonEnvironment.GetGame().GetClientManager().GetClientByUserID(Ticket.Sender.Id);
            if (Client != null)
            {
                Client.SendMessage(new ModeratorSupportTicketResponseComposer(Result));
            }

            if (Result == 2)
            {
                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `user_info` SET `cfhs_abusive` = `cfhs_abusive` + 1 WHERE `user_id` = '" + Ticket.Sender.Id + "' LIMIT 1");
                }
            }

            Ticket.Answered = true;
            MoonEnvironment.GetGame().GetClientManager().SendMessage(new ModeratorSupportTicketComposer(Session.GetHabbo().Id, Ticket), "mod_tool");
        }
    }
}