using Moon.HabboHotel.Users;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Moderation;
using Moon.Database.Interfaces;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class BanPubliCommand : IChatCommand
    {

        public string PermissionRequired => "user_4";
        public string Parameters => "[USUARIO]";
        public string Description => "Banir publicista.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, digite o nome de usuário do usuário que você deseja Ban IP e conta da proibição.", 34);
                return;
            }

            Habbo Habbo = MoonEnvironment.GetHabboByUsername(Params[1]);
            if (Habbo == null)
            {
                Session.SendWhisper("Ocorreu um erro, O usuário não foi encontrado na base de dados.", 34); //BPU PROGRAMADO POR JOSEMY.
                return;
            }

            if (Habbo.GetPermissions().HasRight("mod_soft_ban") && !Session.GetHabbo().GetPermissions().HasRight("mod_ban_any"))
            {
                Session.SendWhisper("Oops, você não pode banir este usuário.", 34);
                return;
            }
            int time = 1576108800;
            string Reason = "Você não pode publicar outros hotéis no" + MoonEnvironment.HotelName;
            string Username = Habbo.Username;
            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.runFastQuery("UPDATE `user_info` SET `bans` = `bans` + '1' WHERE `user_id` = '" + Habbo.Id + "' LIMIT 1");
            }

            MoonEnvironment.GetGame().GetModerationManager().BanUser(Session.GetHabbo().Username, ModerationBanType.USERNAME, Habbo.Username, Reason, time);

            GameClient TargetClient = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient != null)
                TargetClient.Disconnect();

            Session.SendWhisper("Você baniu '" + Username + "'  por publicista", 34);
        }
    }
}