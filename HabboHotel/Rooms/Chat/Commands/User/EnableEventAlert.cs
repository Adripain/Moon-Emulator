using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Yezz.Database.Interfaces;

namespace Yezz.HabboHotel.Rooms.Chat.Commands.User
{
    class EnableEventAlert : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_enable_eventalert"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Activar o desactivar las alertas de eventos."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().AlertaEvent = !Session.GetHabbo().AlertaEvent;
            Session.SendWhisper("Ahora mismo " + (Session.GetHabbo().AlertaEvent == true ? "aceptas" : "no aceptas") + " nuevas peticiones de amistad");

            using (IQueryAdapter dbClient = YezzEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `ignorealertevent` = '1' WHERE `id` = '" + Session.GetHabbo().Id + "'");
                dbClient.RunQuery();
            }
        }
    }
}