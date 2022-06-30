using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class EndPollCommand : IChatCommand
    {

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
           
                    Room.endQuestion();
        }

        public string Description =>
            "Terminar enquete.";

        public string Parameters =>
            "";

        public string PermissionRequired =>
            "user_6";
    }
}