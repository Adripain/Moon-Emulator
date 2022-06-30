using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.Communication.Packets.Outgoing.Moderation;
using Moon.Communication.Packets.Outgoing.Notifications;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class TrollAlertUser : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_6"; }
        }

        public string Parameters
        {
            get { return "[MENSAGEM]"; }
        }

        public string Description
        {
            get { return "Envie uma mensagem de aviso para todos os usuários online."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Escreva a mensagem que você deseja enviar.", 34);
                return;
            }

            string Image = CommandManager.MergeParams(Params, 3);
            string Message = CommandManager.MergeParams(Params, 1);
            string figure = Session.GetHabbo().Look;

            MoonEnvironment.GetGame().GetClientManager().MsgAlert2(RoomNotificationComposer.SendBubble("fig/" + figure, Message + "", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId + ""));
            return;

        }
    }
}
