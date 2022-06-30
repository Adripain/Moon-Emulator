using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.Communication.Packets.Outgoing.Rooms.Avatar;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class MassDanceCommand : IChatCommand
    {
        public string PermissionRequired => "user_12";
        public string Parameters => "[DANCEID]";
        public string Description => "Fazer todo mundo dançar.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, insira um ID de dança. (1-4)", 34);
                return;
            }

            int DanceId = Convert.ToInt32(Params[1]);
            if (DanceId < 0 || DanceId > 4)
            {
                Session.SendWhisper("Por favor, insira um ID de dança. (1-4)", 34);
                return;
            }

            List<RoomUser> Users = Room.GetRoomUserManager().GetRoomUsers();
            if (Users.Count > 0)
            {
                foreach (RoomUser U in Users.ToList())
                {
                    if (U == null)
                        continue;

                    if (U.CarryItemID > 0)
                        U.CarryItemID = 0;

                    U.DanceId = DanceId;
                    Room.SendMessage(new DanceComposer(U, DanceId));
                }
            }
        }
    }
}
