using Moon.HabboHotel.GameClients;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class AlertCommand : IChatCommand
    {
        public string PermissionRequired => "user_10";
        public string Parameters => "[USUARIO] [MENSAGEM]";
        public string Description => "Enviar alerta a um usuário.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {

            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, digite o nome do usuário que você deseja alertar.", 34);
                return;
            }

            GameClient TargetClient = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Ocorreu um erro durante a pesquisa do usuário, talvez ele não esteja online.", 34);
                return;
            }

            if (TargetClient.GetHabbo() == null)
            {
                Session.SendWhisper("Ocorreu um erro durante a pesquisa do usuário, talvez ele não esteja online.", 34);
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("Pegue um usuário.");
                return;
            }

            string Message = CommandManager.MergeParams(Params, 2);

            TargetClient.SendNotification(Session.GetHabbo().Username + " alertou você com a seguinte mensagem:\n\n" + Message);
            Session.SendWhisper("Alerta enviado com sucesso para " + TargetClient.GetHabbo().Username, 34);

        }
    }
}
