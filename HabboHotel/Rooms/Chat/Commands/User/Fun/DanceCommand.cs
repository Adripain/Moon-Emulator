using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.Communication.Packets.Outgoing.Rooms.Avatar;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class DanceCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_normal"; }
        }

        public string Parameters
        {
            get { return "[BAILE]"; }
        }

        public string Description
        {
            get { return "Ativar uma dança em seu personagem, de 0 a 4."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser ThisUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
                return;

            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, digite un ID de uma dança.", 34);
                return;
            }

            int DanceId;
            if (int.TryParse(Params[1], out DanceId))
            {
                if (DanceId > 4 || DanceId < 0)
                {
                    Session.SendWhisper("O ID da dança deve estar entre 0 e 4!", 34);
                    return;
                }

                Session.GetHabbo().CurrentRoom.SendMessage(new DanceComposer(ThisUser, DanceId));
            }
            else
                Session.SendWhisper("Por favor, introduzca un ID válido de baile.", 34);
        }
    }
}
