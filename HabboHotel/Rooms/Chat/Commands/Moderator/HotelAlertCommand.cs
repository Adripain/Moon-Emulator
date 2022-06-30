using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.Communication.Packets.Outgoing.Moderation;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class HotelAlertCommand : IChatCommand
    {
        public string PermissionRequired => "user_11";
        public string Parameters => "[MENSAGEM]";
        public string Description => "Enviar alerta para o hotel.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, escreva a mensagem para enviar", 34);
                return;
            }
            string Message = CommandManager.MergeParams(Params, 1);
            MoonEnvironment.GetGame().GetClientManager().SendMessage(new RoomNotificationComposer("Mensagem de " + Session.GetHabbo().Username + ":", "<font size =\"11\">Querido usuário do " + MoonEnvironment.HotelName + ", o staff <b>" + Session.GetHabbo().Username + "</b> tem uma mensagem para todo o hotel:</font><br><br><font size =\"16\" color=\"#B40404\">" + Message + "</font>", "habboarte", ""));
            return;
        }
    }
}
