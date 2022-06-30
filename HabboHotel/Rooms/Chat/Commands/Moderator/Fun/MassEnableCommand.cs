using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class MassEnableCommand : IChatCommand
    {
        public string PermissionRequired => "user_12";
        public string Parameters => "[EFEITOID]";
        public string Description => "Efeito a cada usuário no quarto.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor insira o efeito ID.", 34);
                return;
            }

            int EnableId = 0;
            if (int.TryParse(Params[1], out EnableId))
            {
                if (EnableId == 102 || EnableId == 178)
                {
                    Session.SendWhisper("Há usuários que não podem usar esse efeito.", 34);
                    return;
                }

                if (!Session.GetHabbo().GetPermissions().HasCommand("command_override_massenable") && Room.OwnerId != Session.GetHabbo().Id)
                {
                    Session.SendWhisper("Você pode usar este comando somente se você for o dono.", 34);
                    return;
                }

                List<RoomUser> Users = Room.GetRoomUserManager().GetRoomUsers();
                if (Users.Count > 0)
                {
                    foreach (RoomUser U in Users.ToList())
                    {
                        if (U == null || U.RidingHorse)
                            continue;

                        U.ApplyEffect(EnableId);
                    }
                }
            }
            else
            {
                Session.SendWhisper("Por favor insira o efeito ID.", 34);
                return;
            }

        }
    }
}
