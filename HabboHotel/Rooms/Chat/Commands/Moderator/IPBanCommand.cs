using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.Utilities;
using Moon.HabboHotel.Users;
using Moon.HabboHotel.GameClients;


using Moon.HabboHotel.Moderation;

using Moon.Database.Interfaces;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class IPBanCommand : IChatCommand
    {
        public string PermissionRequired => "user_11";
        public string Parameters => "[USUARIO]";
        public string Description => "Banir por IP e a conta de outro usuário.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, digite o nome do usuário ou IP para Banir.", 34);
                return;
            }

            Habbo Habbo = MoonEnvironment.GetHabboByUsername(Params[1]);
            if (Habbo == null)
            {
                Session.SendWhisper("Ocorreu um erro na pesquisa pelo banco de dados.", 34);
                return;
            }

            if (Habbo.GetPermissions().HasRight("mod_tool") && !Session.GetHabbo().GetPermissions().HasRight("mod_ban_any"))
            {
                Session.SendWhisper("Oops, você não pode banir esse usuário.", 34);
                return;
            }

            String IPAddress = String.Empty;
            Double Expire = MoonEnvironment.GetUnixTimestamp() + 78892200;
            string Username = Habbo.Username;
            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `user_info` SET `bans` = `bans` + '1' WHERE `user_id` = '" + Habbo.Id + "' LIMIT 1");

                dbClient.SetQuery("SELECT `ip_last` FROM `users` WHERE `id` = '" + Habbo.Id + "' LIMIT 1");
                IPAddress = dbClient.getString();
            }

            string Reason = null;
            if (Params.Length >= 3)
                Reason = CommandManager.MergeParams(Params, 2);
            else
                Reason = "O motivo não foi especificado.";

            if (!string.IsNullOrEmpty(IPAddress))
                MoonEnvironment.GetGame().GetModerationManager().BanUser(Session.GetHabbo().Username, ModerationBanType.IP, IPAddress, Reason, Expire);
            MoonEnvironment.GetGame().GetModerationManager().BanUser(Session.GetHabbo().Username, ModerationBanType.USERNAME, Habbo.Username, Reason, Expire);

            GameClient TargetClient = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient != null)
                TargetClient.Disconnect();


            Session.SendWhisper("O usuário foi banido com sucesso '" + Username + "' pelo seguinte motivo: '" + Reason + "'!", 34);
        }
    }
}