using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.Database.Interfaces;



namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class DisableGiftsCommand : IChatCommand
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
            get { return "Ativar ou desativar presentes."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().AllowGifts = !Session.GetHabbo().AllowGifts;
            Session.SendWhisper("Você " + (Session.GetHabbo().AllowGifts == true ? "agora" : "agora não") + " pode receber mais presentes.", 34);

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `allow_gifts` = @AllowGifts WHERE `id` = '" + Session.GetHabbo().Id + "'");
                dbClient.AddParameter("AllowGifts", MoonEnvironment.BoolToEnum(Session.GetHabbo().AllowGifts));
                dbClient.RunQuery();
            }
        }
    }
}