using System;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class ChangelogCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_normal"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Últimas atualizações do "+Convert.ToString(MoonEnvironment.GetConfig().data["hotel.name"])+"."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            var _cache = new Random().Next(0, 300);
            Session.SendMessage(new MassEventComposer("habbopages/changelogs.txt?" + _cache));
        }
    }
}