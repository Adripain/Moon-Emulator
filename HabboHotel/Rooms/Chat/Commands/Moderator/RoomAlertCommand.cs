using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class RoomAlertCommand : IChatCommand
    {
        public string PermissionRequired => "user_10";
        public string Parameters => "[MENSAGEM]";
        public string Description => "Enviar mensagem para todos no quarto.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor insira a mensagem que deseja enviar no quarto");
                return;
            }

            if (!Session.GetHabbo().GetPermissions().HasRight("mod_alert") && Room.OwnerId != Session.GetHabbo().Id)
            {
                Session.SendWhisper("Você só pode fazer isso no seu próprio quarto..", 34);
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);
            foreach (RoomUser RoomUser in Room.GetRoomUserManager().GetRoomUsers())
            {
                if (RoomUser == null || RoomUser.GetClient() == null || Session.GetHabbo().Id == RoomUser.UserId)
                    continue;

                MoonEnvironment.GetGame().GetClientManager().SendMessage(new RoomCustomizedAlertComposer(Message + "\n\n- " + Session.GetHabbo().Username));
            }
            Session.SendWhisper("Mensagem enviada corretamente no quarto.", 34);
        }
    }
}
