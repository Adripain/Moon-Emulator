using System.Linq;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.HabboHotel.GameClients;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class GlobalMessageCommand : IChatCommand
    {
        public string PermissionRequired => "user_12";
        public string Parameters => "[MENSAJE]";
        public string Description => "Enviar alerta 'BUBBLE' global";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {


            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, introduce el mensaje.");
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);
            foreach (GameClient client in MoonEnvironment.GetGame().GetClientManager().GetClients.ToList())
            {
                client.SendMessage(new RoomNotificationComposer("command_gmessage", "message", "" + Message + "!"));
            }
        }
    }
}
