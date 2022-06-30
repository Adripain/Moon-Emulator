using System;
using System.Linq;
using Moon.Database.Interfaces;
using System.Data;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class ShutdownCommand : IChatCommand
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
            get { return "Fechar o hotel!"; }
        }
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {

            MoonEnvironment.GetGame().GetClientManager().SendMessage(new RoomCustomizedAlertComposer(MoonEnvironment.HotelName + " será cerrado en pocos segundos.\n\n - " + Session.GetHabbo().Username + ""));
            Session.SendWhisper("O hotel estará fechado em um minuto!", 34);
            MoonEnvironment.PerformShutDown();
        }
    }
}