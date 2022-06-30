using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class RoomKickCommand : IChatCommand
    {
        public string PermissionRequired => "user_10";
        public string Parameters => "[MENSAGEM]";
        public string Description => "Expulsar todos os usuários do quarto.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, dê um motivo para os usuários.");
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);
            foreach (RoomUser RoomUser in Room.GetRoomUserManager().GetUserList().ToList())
            {
                if (RoomUser == null || RoomUser.IsBot || RoomUser.GetClient() == null || RoomUser.GetClient().GetHabbo() == null || RoomUser.GetClient().GetHabbo().GetPermissions().HasRight("mod_tool") || RoomUser.GetClient().GetHabbo().Id == Session.GetHabbo().Id)
                    continue;

                RoomUser.GetClient().SendNotification("Você foi expulso por um moderador pelo seguinte motivo: " + Message);

                Room.GetRoomUserManager().RemoveUserFromRoom(RoomUser.GetClient(), true, false);
            }

            Session.SendWhisper("Expulsou todos corretamente", 34);
        }
    }
}
