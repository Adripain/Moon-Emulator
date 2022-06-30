using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

using Moon.HabboHotel.Rooms;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class Builder : IChatCommand
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
            get { return "Habilita o teletransporte no seu quarto para construir mais facilmente."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;


            User.TeleportEnabled = !User.TeleportEnabled;
            Room.GetGameMap().GenerateMaps();

            Session.SendMessage(RoomNotificationComposer.SendBubble("builders_club_room_locked_small", "Acabas de activar el modo de constructor.", ""));
        }
    }
}
