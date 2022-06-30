using System.Linq;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

using Moon.HabboHotel.GameClients;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class MassBadgeCommand : IChatCommand
    {
        public string PermissionRequired => "user_13";
        public string Parameters => "[CODIGO]";
        public string Description => "Dar emblemas para todo o hotel.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, digite o código do emblema que você deseja enviar para todos");
                return;
            }

            foreach (GameClient Client in MoonEnvironment.GetGame().GetClientManager().GetClients.ToList())
            {
                if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().Username == Session.GetHabbo().Username)
                    continue;

                if (!Client.GetHabbo().GetBadgeComponent().HasBadge(Params[1]))
                {
                    Client.GetHabbo().GetBadgeComponent().GiveBadge(Params[1], true, Client);
                    Client.SendMessage(RoomNotificationComposer.SendBubble("badge/" + Params[1], Session.GetHabbo().Username + " te enviou o emblema " + Params[1] + ".", "/inventory/open/badge"));
                }
                else
                    Client.SendMessage(RoomNotificationComposer.SendBubble("erro", "" + Session.GetHabbo().Username + " tentou enviar-lhe o emblema " + Params[1] + " mas você já tem.", ""));
            }

            Session.SendWhisper("Você deu a cada um do hotel o emblema com sucesso " + Params[1] + "!", 34);
        }
    }
}
