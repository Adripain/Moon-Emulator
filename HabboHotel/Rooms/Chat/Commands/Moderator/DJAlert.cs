using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.Communication.Packets.Outgoing.Moderation;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class DJAlert : IChatCommand
    {
        public string PermissionRequired => "user_6";
        public string Parameters => "[USUARIO]";
        public string Description => "Enviar alerta para hotel de transmissão.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, escreva a mensagem que você quer enviar");
                return;
            }
            string Message = CommandManager.MergeParams(Params, 1);
            MoonEnvironment.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("DJAlertNEW", "DJ " + Message + " está transmitindo ao vivo! Sintonize "+Convert.ToString(MoonEnvironment.GetConfig().data["hotel.name"])+"FM agora mesmo e disfrute ao máximo.", ""));
            return;
        }
    }
}
