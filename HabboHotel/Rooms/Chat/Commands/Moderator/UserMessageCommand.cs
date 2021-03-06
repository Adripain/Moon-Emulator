using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.HabboHotel.GameClients;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class UserMessageCommand : IChatCommand
    {
        public string PermissionRequired => "user_10";
        public string Parameters => "[USUARIO] [MENSAJE]";
        public string Description => "Enviar mensaje a un usuario.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {

            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, introduzca el nombre de usuario del usuario.", 34);
                return;
            }

            GameClient TargetClient = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Se produjo un error mientras que la búsqueda de usuario, tal vez no están en línea.", 34);
                return;
            }

            if (TargetClient.GetHabbo() == null)
            {
                Session.SendWhisper("Se produjo un error mientras que la búsqueda de usuario, tal vez no están en línea.", 34);
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("Consigue una vida.", 34);
                return;
            }

            string Message = CommandManager.MergeParams(Params, 2);

            TargetClient.SendMessage(new RoomNotificationComposer("command_gmessage", "message", "" + Message + "!"));
            Session.SendMessage(new RoomNotificationComposer("command_gmessage", "message", "Mensaje enviado con éxito para " + TargetClient.GetHabbo().Username));
        }
    }
}
