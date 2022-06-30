using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.HabboHotel.GameClients;

namespace Moon.HabboHotel.Rooms.Chat.Commands
{
    internal class MakeVipCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_16"; }
        }

        public string Parameters
        {
            get { return "%username% %days%"; }
        }

        public string Description
        {
            get { return "Dale VIP a un usuario"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor introduce el nombre del usuario al que le enviaras la alerta");
                return;
            }

            GameClient TargetClient = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Ocurrio un error, al parecer no se consigue el usuario o no se encuentra online");
                return;
            }

            int Days = int.Parse(CommandManager.MergeParams(Params, 2));

            if (Days > 31)
            {
                Session.SendWhisper("Ocurrio un error, no puedes entregar tantos días al mismo usuario.");
                return;
            }

            TargetClient.GetHabbo().GetClubManager().AddOrExtendSubscription("club_vip", Days * 24 * 3600, Session);
            TargetClient.SendMessage(new AlertNotificationHCMessageComposer(4));
        }
    }
}