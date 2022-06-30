using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.Communication.Packets.Outgoing.Moderation;
using Moon.Communication.Packets.Outgoing.Notifications;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Events
{
    class EventoExpress : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_6"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Cria um evento relampago."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            MoonEnvironment.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("eventos", "Acaba de começar um evento Relampago, para mais informações clique aqui.", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId + ""));
            return;

        }
    }
}
