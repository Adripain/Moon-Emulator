using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.Database.Interfaces;



namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class EnableFriendsCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_enable_friends"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Ativa as solicitações de amizade."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().AllowFriendRequests = !Session.GetHabbo().AllowFriendRequests;
            Session.SendWhisper("Você" + (Session.GetHabbo().AllowFriendRequests == true ? "agora" : "agora não ") + " pode ter amigos.");

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `block_newfriends` = '0' WHERE `id` = '" + Session.GetHabbo().Id + "'");

                dbClient.RunQuery();
            }
        }
    }
}