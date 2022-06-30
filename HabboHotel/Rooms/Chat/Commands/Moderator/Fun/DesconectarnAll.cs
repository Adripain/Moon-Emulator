using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Rooms.Session;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class DesconectarnAll : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_16"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Reinicia (deconecta todos)."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {

            foreach (GameClient Client in MoonEnvironment.GetGame().GetClientManager().GetClients.ToList())
            {
                if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().Username == Session.GetHabbo().Username)
                    continue;


                if (!Client.GetHabbo().InRoom)
                    Client.GetConnection().Dispose();
                else if (Client.GetHabbo().InRoom)
                    Client.GetConnection().Dispose();
                Client.SendNotification("O hotel dará um pequeno reinicio, para aplicar todas as alterações dentro do Hotel. \n\nVoltaremos em seguida:)\n\n\n- " + Session.GetHabbo().Username + "");
            }



        }
    }
}
