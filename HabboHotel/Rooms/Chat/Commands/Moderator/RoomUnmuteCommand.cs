using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class RoomUnmuteCommand : IChatCommand
    {
        public string PermissionRequired => "user_10";
        public string Parameters => "";
        public string Description => "Desmutar quarto.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.RoomMuted)
            {
                Session.SendWhisper("Este quarto não está mudo.", 34);
                return;
            }

            Room.RoomMuted = false;

            List<RoomUser> RoomUsers = Room.GetRoomUserManager().GetRoomUsers();
            if (RoomUsers.Count > 0)
            {
                foreach (RoomUser User in RoomUsers)
                {
                    if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null || User.GetClient().GetHabbo().Username == Session.GetHabbo().Username)
                        continue;

                    User.GetClient().SendWhisper("Este quarto foi desmutado, você pode falar normalmente de novo.", 34);
                }
            }
        }
    }
}