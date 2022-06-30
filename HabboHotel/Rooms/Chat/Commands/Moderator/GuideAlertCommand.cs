using Moon.Communication.Packets.Outgoing.Moderation;
using Moon.Communication.Packets.Outgoing.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class GuideAlertCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_guide_alert";}
        }

        public string Parameters
        {
            get { return "%message%"; }
        }

        public string Description
        {
            get { return "Enviale un mensaje de alerta a todos los staff online."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Session.GetHabbo()._guidelevel < 1)
            {
                Session.SendWhisper("No puedes enviar alertas para guías si no lo eres.");
                return;
              
            }
            if (Params.Length == 1)
            {
                Session.SendWhisper("Escribe el mensaje que deseas enviar.");
                return;
            }

            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(MoonEnvironment.GetUnixTimestamp()).ToLocalTime();

            string Message = CommandManager.MergeParams(Params, 1);
            MoonEnvironment.GetGame().GetClientManager().GuideAlert(new MOTDNotificationComposer("[GUIDE]\r[" + dtDateTime + "]\r\r" + Message + "\r\r - " + Session.GetHabbo().Username + " [" + Session.GetHabbo()._guidelevel + "]"));
            return;
        }
    }
}