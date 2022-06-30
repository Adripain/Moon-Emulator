using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.Database.Interfaces;
using Moon.Utilities;
using Moon.HabboHotel.Users;
using Moon.HabboHotel.GameClients;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class TradeBanCommand : IChatCommand
    {
        public string PermissionRequired => "user_10";
        public string Parameters => "[USUARIO] [TEMPO]";
        public string Description => "Proibir o tradeo de outro usuário.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Digite o nome do usuário e a hora em dias (min 1 dia, max 365 dias).", 34);
                return;
            }

            Habbo Habbo = MoonEnvironment.GetHabboByUsername(Params[1]);
            if (Habbo == null)
            {
                Session.SendWhisper("Ocorreu um erro quando a consulta foi feita no banco de dados.", 34);
                return;
            }

            if (Convert.ToDouble(Params[2]) == 0)
            {
                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `user_info` SET `trading_locked` = '0' WHERE `user_id` = '" + Habbo.Id + "' LIMIT 1");
                }

                if (Habbo.GetClient() != null)
                {
                    Habbo.TradingLockExpiry = 0;
                    Habbo.GetClient().SendNotification("Sua tradeo já foi desbloqueada, você pode continuar a negociar com outros usuários.");
                }

                Session.SendWhisper("Você desbloqueou " + Habbo.Username + " da sua trade Ban.", 34);
                return;
            }

            double Days;
            if (double.TryParse(Params[2], out Days))
            {
                if (Days < 1)
                    Days = 1;

                if (Days > 365)
                    Days = 365;

                double Length = (MoonEnvironment.GetUnixTimestamp() + (Days * 86400));
                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `user_info` SET `trading_locked` = '" + Length + "', `trading_locks_count` = `trading_locks_count` + '1' WHERE `user_id` = '" + Habbo.Id + "' LIMIT 1");
                }

                if (Habbo.GetClient() != null)
                {
                    Habbo.TradingLockExpiry = Length;
                    Habbo.GetClient().SendNotification("Você tem um bloqueio  de tradeos por " + Days + " dia(s).");
                }

                Session.SendWhisper("Você bloqueou as tradeos de  " + Habbo.Username + " por " + Days + " dia(s).", 34);
            }
            else
                Session.SendWhisper("Insira dias válidos, em números inteiros.", 34);
        }
    }
}
