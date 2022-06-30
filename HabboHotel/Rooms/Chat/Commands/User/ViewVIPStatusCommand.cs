using Moon.HabboHotel.GameClients;

using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.Database.Interfaces;
using System.Data;
using System;
using Moon.Communication.Packets.Outgoing.Rooms.Engine;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class ViewVIPStatusCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_info"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Informações do seu VIP "+Convert.ToString(MoonEnvironment.GetConfig().data["hotel.name"])+"."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            Session.SendMessage(RoomNotificationComposer.SendBubble("abuse", "Você não é membro VIP do "+Convert.ToString(MoonEnvironment.GetConfig().data["hotel.name"])+", clique aqui para voltar.", ""));
        }
    }
}