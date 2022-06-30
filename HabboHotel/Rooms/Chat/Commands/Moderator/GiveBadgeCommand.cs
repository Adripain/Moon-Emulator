using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.HabboHotel.GameClients;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class GiveBadgeCommand : IChatCommand
    {
        public string PermissionRequired => "user_12";
        public string Parameters => "[USUARIO] [IDEMBLEMA]";
        public string Description => "Dar emblema a um usuário.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {

            if (Params.Length != 3)
            {
                Session.SendWhisper("Por favor, insira um nome de usuário e o código ID do emblema que você gostaria de dar!");
                return;
            }

            GameClient TargetClient = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient != null)
            {
                if (!TargetClient.GetHabbo().GetBadgeComponent().HasBadge(Params[2]))
                {
                    TargetClient.GetHabbo().GetBadgeComponent().GiveBadge(Params[2], true, TargetClient);
                    if (TargetClient.GetHabbo().Id != Session.GetHabbo().Id)
                        TargetClient.SendMessage(RoomNotificationComposer.SendBubble("badge/" + Params[2], "Você acaba de receber um emblema!", "/inventory/open/badge"));

                    else
                        Session.SendWhisper("Você enviou o emblema com sucesso  " + Params[2] + "!", 34);

                }
                else
                    Session.SendWhisper("Oops, esse usuário já tem esse emblema (" + Params[2] + ") !");
                return;
            }
            else
            {
                Session.SendWhisper("Oops, não conseguimos encontrar o usuário, talvez não esteja online!");
                return;
            }
        }
    }
}
