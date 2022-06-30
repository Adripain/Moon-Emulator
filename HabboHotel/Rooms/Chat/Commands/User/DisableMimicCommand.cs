using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.Database.Interfaces;



namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class DisableMimicCommand : IChatCommand
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
            get { return "Ativa ou desativa a opção de copiar seu visual."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().AllowMimic = !Session.GetHabbo().AllowMimic;
            Session.SendWhisper("Você " + (Session.GetHabbo().AllowMimic == true ? "agora" : "agora não") + " protege seu visual.", 34);

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `allow_mimic` = @AllowMimic WHERE `id` = '" + Session.GetHabbo().Id + "'");
                dbClient.AddParameter("AllowMimic", MoonEnvironment.BoolToEnum(Session.GetHabbo().AllowMimic));
                dbClient.RunQuery();
            }
        }
    }
}