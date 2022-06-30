using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using System;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class EventSwapCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_normal"; }
        }

        public string Parameters
        {
            get { return "%events% %1ou2%"; }
        }

        public string Description
        {
            get { return "Altera o design do alerta de evento."; }
        }

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (Params.Length == 1)
            {
                Session.SendMessage(new MassEventComposer("habbopages/alertexplanationz.txt"));
                return;
            }

            string Type = Params[1];
            string AlertType = Params[2];

            if (Params.Length == 3)
            {
                switch (Type)
                {
                    case "events":
                        Session.GetHabbo()._eventtype = AlertType;
                        Session.SendWhisper("Você escolheu o tipo de alerta de eventos: " + AlertType + ".", 34);
                        Session.SendWhisper("Você pode usar o alerta anterior escrevendo :eventtype events 1 o 2.", 34);
                        break;
                }
            }
        }
    }
}