using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class RoomMuteCommand : IChatCommand
    {
        public string PermissionRequired => "user_10";
        public string Parameters => "[MENSAGEM]";
        public string Description => "Mutar quarto.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, insira uma razão para silenciar o quarto.", 34);
                return;
            }

            if (!Room.RoomMuted)
                Room.RoomMuted = true;

            string Msg = CommandManager.MergeParams(Params, 1);

            List<RoomUser> RoomUsers = Room.GetRoomUserManager().GetRoomUsers();
            if (RoomUsers.Count > 0)
            {
                foreach (RoomUser User in RoomUsers)
                {
                    if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null || User.GetClient().GetHabbo().Username == Session.GetHabbo().Username)
                        continue;

                    User.GetClient().SendMessage(new RoomCustomizedAlertComposer("Este quarto foi silenciado pelo seguinte motivo:\n\n" + Msg + "\n\n - " + Session.GetHabbo().Username + ""));
                }
            }
        }
    }
}
