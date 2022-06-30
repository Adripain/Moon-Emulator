using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.Database.Interfaces;
using Moon.Utilities;
using Moon.HabboHotel.GameClients;


namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class UnmuteCommand : IChatCommand
    {
        public string PermissionRequired => "user_10";
        public string Parameters => "[USUARIO]";
        public string Description => "Desmutar usuario.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Digite o nome do usuário que você deseja desmutar..", 34);
                return;
            }

            GameClient TargetClient = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null || TargetClient.GetHabbo() == null)
            {
                Session.SendWhisper("Ocorreu um erro,você não escreveu o nome corretamente ou o usuário não está online.", 34);
                return;
            }

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `users` SET `time_muted` = '0' WHERE `id` = '" + TargetClient.GetHabbo().Id + "' LIMIT 1");
            }

            TargetClient.GetHabbo().TimeMuted = 0;
            TargetClient.SendNotification("Você foi desmutado por " + Session.GetHabbo().Username + "!");
            Session.SendWhisper("Você desmutou  " + TargetClient.GetHabbo().Username + "!", 34);
        }
    }
}