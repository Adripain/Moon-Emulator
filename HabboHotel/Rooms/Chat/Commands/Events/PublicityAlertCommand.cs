using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.HabboHotel.GameClients;
using Moon.Core;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Events
{
    class PublicityAlertCommand : IChatCommand
    {
        public string PermissionRequired => "user_7";
        public string Parameters => "";
        public string Description => "Enviar um alerta de hotel para o seu evento!";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Session == null) return;
            if (Room == null) return;

            MoonEnvironment.GetGame().GetClientManager().SendMessage(new RoomNotificationComposer("Há uma nova publicidade em " + MoonEnvironment.HotelName + "!",
                             "A publicidade foi realizada por: <font color=\"#00adff\"><b>" + Session.GetHabbo().Username + "</b></font>\n\n" +
                "Quer participar da publicade? Clique no botão abaixo <b> Ir ao quarto</b>, e participe.\n\n" +
                "<b>O que é uma publicidade?</b>\n" +
                "A publicidade se baseia em ir a outros hoteis e anunciar " + MoonEnvironment.HotelName + " e conivdar amigos para jogar com nós, para nos divertimos todos juntos.\n\n\n" +
                "<b>Que beneficio tenho em participar de uma publicade?</b>\n" +
                "Poderá ir subindo de rank e mais adiante participar da nossa Equipe Staff <b>Está animado?</b>\n\n",
                             "oleadas", "Ir divulgar!", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
        }
    }
}