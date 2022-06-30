using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Events
{
    internal class HelpCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get
            {
                return "user_normal";
            }
        }
        public string Parameters
        {
            get { return "%message%"; }
        }
        public string Description
        {
            get
            {
                return "Envie um pedido de ajuda, descrevendo brevemente seu problema.";
            }
        }
        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            long nowTime = MoonEnvironment.CurrentTimeMillis();
            long timeBetween = nowTime - Session.GetHabbo()._lastTimeUsedHelpCommand;
            if (timeBetween < 60000)
            {
                Session.SendMessage(RoomNotificationComposer.SendBubble("abuse", "Espere ao menos 1 minuto para voltar a usar o sistema de suporte.", ""));
                return;
            }

            Session.GetHabbo()._lastTimeUsedHelpCommand = nowTime;

            //MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_GuideEnrollmentLifetime", 1);
            Session.SendMessage(new MassEventComposer("help/tour"));
            Session.SendMessage(RoomNotificationComposer.SendBubble("ambassador", "Seu pedido de ajuda foi enviado corretamente, por favor espere.", ""));
        }
    }
}



