
using Moon.Communication.Packets.Outgoing.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class SendGraphicAlertCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_13"; }
        }

        public string Parameters
        {
            get { return "[IMAGEM]"; }
        }

        public string Description
        {
            get { return "Envie uma mensagem de alerta com imagem para todo o hotel."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, escreva o nome da imagem para enviar.");
                return;
            }

            string image = Params[2];

            MoonEnvironment.GetGame().GetClientManager().SendMessage(new GraphicAlertComposer(image));
            return;
        }
    }
}
