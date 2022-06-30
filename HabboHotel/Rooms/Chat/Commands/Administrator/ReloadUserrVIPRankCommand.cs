using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.Communication.Packets.Outgoing.Users;
using Moon.Database.Interfaces;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Rooms.Chat.Commands;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class ReloadUserrVIPRankCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_15"; }
        }
        public string Parameters
        {
            get { return "[USUARIO]"; }
        }
        public string Description
        {
            get { return "Dar rank VIP a um usuário."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            GameClient TargetClient = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.runFastQuery("UPDATE `users` SET `rank` = '2' WHERE `id` = '" + TargetClient.GetHabbo().Id + "'");
                dbClient.runFastQuery("UPDATE `users` SET `rank_vip` = '1' WHERE `id` = '" + TargetClient.GetHabbo().Id + "'");
                TargetClient.GetHabbo().Rank = 2;
                TargetClient.GetHabbo().VIPRank = 1;
            }

            TargetClient.GetHabbo().GetClubManager().AddOrExtendSubscription("club_vip", 1 * 24 * 3600, Session);
            TargetClient.GetHabbo().GetBadgeComponent().GiveBadge("DVIP", true, Session);

            MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_VipClub", 1);
            TargetClient.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));

            string figure = TargetClient.GetHabbo().Look;
            MoonEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("fig/" + figure, Params[1] + " agora você é um usuário VIP!", ""));
            Session.SendWhisper("VIP dado com exito!");
        }
    }
}