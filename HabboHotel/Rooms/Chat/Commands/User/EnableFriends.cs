using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.Database.Interfaces;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class EnableFriends : IChatCommand
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
            get { return "Ativa ou desativa as solicitações de amizade."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().AllowFriendRequests = !Session.GetHabbo().AllowFriendRequests;
            Session.SendWhisper("Agora você póde aceitar novas solicitações de amizade", 34);

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `block_newfriends` = '0' WHERE `id` = '" + Session.GetHabbo().Id + "'");
                dbClient.RunQuery();
            }
        }
    }
}