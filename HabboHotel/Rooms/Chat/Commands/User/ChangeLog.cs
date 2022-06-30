using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.Communication.Packets.Outgoing.Moderation;
using Moon.Communication.Packets.Outgoing.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class ChangeLog : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_staff_alert"; }
        }

        public string Parameters
        {
            get { return "%message%"; }
        }

        public string Description
        {
            get { return "Enviar uma mensagem de alerta a todos staff online."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Escreva a mensagem que deseja enviar.");
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);
            MoonEnvironment.GetGame().GetClientManager().StaffAlert(new MOTDNotificationComposer("[STAFF][" + Session.GetHabbo().Username + "]\r\r" + Message));
            return;

        }
    }
}
