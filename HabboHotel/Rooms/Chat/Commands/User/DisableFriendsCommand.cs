using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.Database.Interfaces;



namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class DisableFriendsCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_disable_friends"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Desativa as solicitações de amigos."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().AllowFriendRequests = !Session.GetHabbo().AllowFriendRequests;
            Session.SendWhisper("Você " + (Session.GetHabbo().AllowFriendRequests == true ? "agora" : "agora não") + "é mais adicionavel.");

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `block_newfriends` = '1' WHERE `id` = '" + Session.GetHabbo().Id + "'");
                dbClient.RunQuery();
            }
        }
    }
}