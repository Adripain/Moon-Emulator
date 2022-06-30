using Moon.Communication.Packets.Outgoing.Moderation;
using Moon.Communication.Packets.Outgoing.Notifications;
using Moon.HabboHotel.GameClients;
using System;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Events
{//da2alert
    internal class Da2AlertCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get
            {
                return "user_10";
            }
        }
        public string Parameters
        {
            get { return ""; }
        }
        public string Description
        {
            get
            {
                return "Enviar alerta Da2 para o hotel do evento!";
            }
        }
        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Session == null) return;
            if (Room == null) return;

            MoonEnvironment.GetGame().GetClientManager().SendMessage(new RoomNotificationComposer("¡Se han abierto los dados oficiales en " + MoonEnvironment.HotelName + "!",
                "El inter que abre los dados es: <font color='#FF8000'><b>" + Session.GetHabbo().Username + "</b></font>\n\n" +
                "A diferencia de los dados comunes, es que en estos puedes apostar con total seguridad.\n\n" +
                "¿Quieres participar en este evento de da2? ¡Haz click en el botón inferior de <b> Ir a la sala</b>, y ahí dentro podrás participar.\n\n" +
                "Los inters serán los encargados de supervisar que todo se realiza de manera correcta.\n¡¿A QUE ESPERAS?! ¡Ven ya y gana apostando contra otros usuarios.",
                "da2alert", "¡Ir a la sala!", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
        }
    }
}