using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.HabboHotel.GameClients;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class TrollAlert : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_6"; }
        }

        public string Parameters
        {
            get { return "[MENSAGEM]"; }
        }

        public string Description
        {
            get { return "Envie uma mensagem de aviso para toda a equipe online."; }
        }

        public void Execute(GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Escreva a mensagem que você deseja enviar.", 34);
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);
            string figure = Session.GetHabbo().Look;
            MoonEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("fig/" + figure, Message + "\n\n- " + Session.GetHabbo().Username + "", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId + ""));
            return;


        }
    }
}
