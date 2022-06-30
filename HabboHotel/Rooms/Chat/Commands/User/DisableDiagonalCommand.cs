using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class DisableDiagonalCommand : IChatCommand
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
            get { return "Desativa a opção de andar na diagonal na sua sala."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
            {
                Session.SendWhisper("Oops, só o dono do quarto pode usar este comando!", 34);
                return;
            }

            Room.GetGameMap().DiagonalEnabled = !Room.GetGameMap().DiagonalEnabled;
            Session.SendWhisper("Ninguem pode caminhar na diagonal na sala", 34);
        }
    }
}
