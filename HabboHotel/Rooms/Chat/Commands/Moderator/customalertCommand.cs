using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.Communication.Packets.Outgoing.Moderation;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class customalertCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_11"; }
        }

        public string Parameters
        {
            get { return "[MENSAGEM]"; }
        }

        public string Description
        {
            get { return "Envie uma mensagem de aviso para o hotel."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, escreva uma mensagem para enviar.", 34);
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);

            MoonEnvironment.GetGame().GetClientManager().SendMessage(new RoomCustomizedAlertComposer(Message));



            return;
        }
    }
}
