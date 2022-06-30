using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moon.Communication.Packets.Outgoing.Rooms.Chat;
using Moon.HabboHotel.GameClients;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class OnlineCommand : IChatCommand
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
            get { return "Ver quantidade de usuários online."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            int OnlineUsers = MoonEnvironment.GetGame().GetClientManager().Count;

            Session.SendWhisper("Agora mesmo tem " + OnlineUsers + " usuários conectados no "+MoonEnvironment.HotelName+" :).", 34);
        }
    }
}

