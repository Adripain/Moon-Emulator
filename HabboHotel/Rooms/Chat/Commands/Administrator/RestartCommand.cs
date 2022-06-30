using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Rooms.Chat.Styles;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class RestartCommand : IChatCommand
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
            get { return "Reinicia o Hotel"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            MoonEnvironment.GetGame().GetClientManager().SendMessage(new RoomCustomizedAlertComposer(MoonEnvironment.HotelName+" fará um reinicio rapido, para aplicar todas as atualizações.\n\nVoltaremos em seguida:)\n\n - " + Session.GetHabbo().Username + ""));

            MoonEnvironment.PerformRestart();
        }
    }
}