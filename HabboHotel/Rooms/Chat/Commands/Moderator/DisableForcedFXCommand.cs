using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.Database.Interfaces;


namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class DisableForcedFXCommand : IChatCommand
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
            get { return "Possibilidade de ignorar ou permitir efeitos forçados."; }
        }

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            Session.GetHabbo().DisableForcedEffects = !Session.GetHabbo().DisableForcedEffects;
            Session.SendWhisper("Modo FX Forçado está " + (Session.GetHabbo().DisableForcedEffects == true ? "desativado!" : "ativado!"), 34);

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `disable_forced_effects` = @DisableForcedEffects WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                dbClient.AddParameter("DisableForcedEffects", MoonEnvironment.BoolToEnum(Session.GetHabbo().DisableForcedEffects));
                dbClient.RunQuery();
            }

        }
    }
}
