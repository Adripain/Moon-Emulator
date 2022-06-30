using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class RegenMaps : IChatCommand
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
            get { return "Regenera o mapa da sala em que está."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
            {
                Session.SendWhisper("Oops, somente o dono da sala pode usar este comando!", 34);
                return;
            }

            Room.GetGameMap().GenerateMaps();
            Session.SendWhisper("Excelente, o mapa do jogo foi recarregado.", 34);
        }
    }
}
