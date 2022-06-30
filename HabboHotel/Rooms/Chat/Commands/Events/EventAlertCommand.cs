using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.Communication.Packets.Outgoing.Rooms.Chat;
using System;
using Moon.Database.Interfaces;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Events
{
    internal class EventAlertCommand : IChatCommand
    {
        public string PermissionRequired => "user_12";
        public string Parameters => "[MENSAJE]";
        public string Description => "Envia alerta de evento ao Hotel!";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {

            string Message = CommandManager.MergeParams(Params, 1);

            Session.GetHabbo()._eventsopened++;

            MoonEnvironment.GetGame().GetClientManager().SendEventType1(new RoomNotificationComposer("Novo evento no " + MoonEnvironment.HotelName + "!",
                             "<font color=\"#00adff\"><b>" + Session.GetHabbo().Username + "</b></font> está organizando um novo evento neste momento! Se quer ganhar  <font color=\"#235F8C\"><b>Diamantes</b></font> participe agora mesmo.\n\n" +
                             "Você quer participar desse evento? Clique no botão inferior  <b> Ir para o evento</b>, e você poderá participar.\n\n" +
                             "<b>Do que se trata este evento?<b>\n\n <font color=\"#f11648\"><b>" + Message +
                             "</b></font> <br><br>Te esperamos! :)<b>",
                             "events", "Ir para o evento", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));

            MoonEnvironment.GetGame().GetClientManager().SendEventType2(new WhisperComposer(-1, "Novo evento <font color=\"#2E9AFE\"><a href='event:navigator/goto/" + Session.GetHabbo().CurrentRoomId + "'><b>clique aqui</b></a></font> para ir para evento.", 0, 33));
            MoonEnvironment.GetGame().GetClientManager().SendEventType2(new WhisperComposer(-1, Message, 0, 33));
            MoonEnvironment.GetGame().GetClientManager().SendEventType2(new WhisperComposer(-1, "Evento organizado por: " + Session.GetHabbo().Username + ".", 0, 33));

            LogEvent(Session.GetHabbo().Id, Room.Id, Message);
        }

        public void LogEvent(int MasterID, int RoomID, string Message)
        {
            DateTime Now = DateTime.Now;
            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO event_logs VALUES (NULL, " + MasterID + ", " + RoomID + ", @message, UNIX_TIMESTAMP())");
                dbClient.AddParameter("message", Message);
                dbClient.RunQuery();
            }
        }


    }
}

