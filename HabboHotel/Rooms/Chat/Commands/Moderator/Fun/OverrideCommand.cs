using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class OverrideCommand : IChatCommand
    {
        public string PermissionRequired => "user_6";
        public string Parameters => "";
        public string Description => "Ande sobre qualquer coisa.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            User.AllowOverride = !User.AllowOverride;
            if (User.AllowOverride)
                Session.SendWhisper("Override Ativado!", 34);
            else
                Session.SendWhisper("Override Desativado!", 34);
        }
    }
}
