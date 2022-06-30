using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using System;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class AlertSwapCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_15"; }
        }

        public string Parameters
        {
            get { return "%type% %id%"; }
        }

        public string Description
        {
            get { return "Troca o estilo dos alertas."; }
        }

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (Params.Length == 1)
            {
                Session.SendMessage(new MassEventComposer("habbopages/alertexplanations.txt"));
                return;
            }

            string Type = Params[1];
            string AlertType = Params[2];

            if (Params.Length == 3)
            {
                switch (Type)
                {
                    case "staff":
                        Session.GetHabbo()._alerttype = AlertType;
                        Session.SendWhisper("Escolheu seu estilo de alerta de Staff como " + AlertType + ".", 34);
                        break;
                    case "events":
                        Session.GetHabbo()._eventtype = AlertType;
                        Session.SendWhisper("Escolheu seu estilo de alerta de Eventos como " + AlertType + ".", 34);
                        break;
                }
            }
        }
    }
}