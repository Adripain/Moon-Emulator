using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.Database.Interfaces;



namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class DisableEfectoStaff : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_4"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Ativar ou desativar a opção de quem copia seu visual."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().AllowMimic = !Session.GetHabbo().AllowMimic;
            Session.SendWhisper("Você " + (Session.GetHabbo().AllowMimic == true ? "agora" : "agora não") + " tem o efeito staff.", 34);

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `allow_mimic` = @AllowMimic WHERE `id` = '" + Session.GetHabbo().Id + "'");
                dbClient.AddParameter("AllowMimic", MoonEnvironment.BoolToEnum(Session.GetHabbo().AllowMimic));
                dbClient.RunQuery();
            }
        }
    }
}