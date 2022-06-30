using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class MoonwalkCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_vip"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Faça o passo do Michael Jackson."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            User.MoonwalkEnabled = !User.MoonwalkEnabled;

            if (User.MoonwalkEnabled)
                Session.SendWhisper("Moonwalk Ativado!");
            else
                Session.SendWhisper("Moonwalk Desativado!");
        }
    }
}
