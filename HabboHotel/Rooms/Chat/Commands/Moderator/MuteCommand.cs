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
    class MuteCommand : IChatCommand
    {
        public string PermissionRequired => "user_10";
        public string Parameters => "[USUARIO] [TEMPO]";
        public string Description => "Mute um usuário.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Digite o nome do usuário para mutar e o tempo expresso em segundos (Maximo 600).", 34);
                return;
            }

            Habbo Habbo = MoonEnvironment.GetHabboByUsername(Params[1]);
            if (Habbo == null)
            {
                Session.SendWhisper("Ocorreu um erro ao procurar o usuário no banco de dados.", 34);
                return;
            }

            if (Habbo.Username == "Forbi" || Habbo.Username == "Forb" || Habbo.Username == "Antoniocrevi")
            {
                Session.SendWhisper("Você não pode mutar esse usuário!", 34);
                return;
            }

            double Time;
            if (double.TryParse(Params[2], out Time))
            {
                if (Time > 600 && !Session.GetHabbo().GetPermissions().HasRight("mod_mute_limit_override"))
                    Time = 600;

                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `users` SET `time_muted` = '" + Time + "' WHERE `id` = '" + Habbo.Id + "' LIMIT 1");
                }

                if (Habbo.GetClient() != null)
                {
                    Habbo.TimeMuted = Time;
                    Habbo.GetClient().SendNotification("Você foi mutado por " + Time + " segundos!");
                }

                Session.SendWhisper("Você mutou  " + Habbo.Username + " por " + Time + " segundos.", 34);
            }
            else
                Session.SendWhisper("Por favor, insira números inteiros.", 34);
        }
    }
}