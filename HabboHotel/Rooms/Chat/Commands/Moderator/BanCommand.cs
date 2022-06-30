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
    class BanCommand : IChatCommand
    {

        public string PermissionRequired => "user_11";
        public string Parameters => "[USUARIO] [TEMPO] [RAZÃO]";
        public string Description => "Banir usuario.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor insira o nome do usuário.", 34);
                return;
            }

            Habbo Habbo = MoonEnvironment.GetHabboByUsername(Params[1]);
            if (Habbo == null)
            {
                Session.SendWhisper("O usuário " + Params[1] + " não existe.", 34);
                return;
            }

            if (Habbo.GetPermissions().HasRight("mod_soft_ban") && !Session.GetHabbo().GetPermissions().HasRight("mod_ban_any"))
            {
                Session.SendWhisper("Aparentemente você não pode banir " + Params[1] + ".", 34);
                return;
            }

            Double Expire = 0;
            string Hours = Params[2];
            if (String.IsNullOrEmpty(Hours) || Hours == "perm")
                Expire = MoonEnvironment.GetUnixTimestamp() + 78892200;
            else
                Expire = (MoonEnvironment.GetUnixTimestamp() + (Convert.ToDouble(Hours) * 3600));

            string Reason = null;
            if (Params.Length >= 4)
                Reason = CommandManager.MergeParams(Params, 3);
            else
                Reason = "Sem motivo.";

            string Username = Habbo.Username;
            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `user_info` SET `bans` = `bans` + '1' WHERE `user_id` = '" + Habbo.Id + "' LIMIT 1");
            }

            MoonEnvironment.GetGame().GetModerationManager().BanUser(Session.GetHabbo().Username, ModerationBanType.USERNAME, Habbo.Username, Reason, Expire);

            GameClient TargetClient = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient != null)
                TargetClient.Disconnect();

            Session.SendWhisper("Excelente,'" + Username + "' foi banido por " + Hours + " hora(s) com a razão '" + Reason + "'!", 34);
        }
    }
}