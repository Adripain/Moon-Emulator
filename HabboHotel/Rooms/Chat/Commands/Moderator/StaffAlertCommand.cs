using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.Communication.Packets.Outgoing.Moderation;
using Moon.Communication.Packets.Outgoing.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class StaffAlertCommand : IChatCommand
    {
        public string PermissionRequired => "user_4";
        public string Parameters => "[MENSAGEM]";
        public string Description => "Enviar mensagem aos staff.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Escreva a mensagem que você deseja enviar.", 34);
                return;
            }

            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(MoonEnvironment.GetUnixTimestamp()).ToLocalTime();

            string Message = CommandManager.MergeParams(Params, 1);
            MoonEnvironment.GetGame().GetClientManager().StaffAlert(new MOTDNotificationComposer("[STAFF]\r[" + dtDateTime + "]\r\r" + Message + "\r\r - " + Session.GetHabbo().Username + " [" + Session.GetHabbo().Rank + "]"));
            return;

        }
    }
}
