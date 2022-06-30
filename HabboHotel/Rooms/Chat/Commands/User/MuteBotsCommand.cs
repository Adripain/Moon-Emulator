using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.Database.Interfaces;


namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class MuteBotsCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_normal"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Silencia todos os BOTs."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().AllowBotSpeech = !Session.GetHabbo().AllowBotSpeech;
            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `users` SET `bots_muted` = '" + ((Session.GetHabbo().AllowBotSpeech) ? 1 : 0) + "' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
            }

            if (Session.GetHabbo().AllowBotSpeech)
                Session.SendWhisper("Pronto, agora você não pode ouvir mais os BOTS", 34);
            else
                Session.SendWhisper("Pronto, agora você pode ouvir os BOTS.", 34);
        }
    }
}
