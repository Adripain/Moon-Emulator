using Moon.Database.Interfaces;
using Moon.HabboHotel.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class FilterCommand : IChatCommand
    {
        public string PermissionRequired => "user_15";
        public string Parameters => "[PALAVRA]";
        public string Description => "Adicione palavras ao filtro.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Digite a palavra que você deseja adicionar ao filtro.", 34);
                return;
            }
            string BannedWord = Params[1];
            if (!string.IsNullOrWhiteSpace(BannedWord))
                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("INSERT INTO wordfilter (`word`, `addedby`, `bannable`) VALUES " +
                        "(@ban, '" + Session.GetHabbo().Username + "', '1');");
                    dbClient.AddParameter("ban", BannedWord.ToLower());
                    dbClient.RunQuery();
                    Session.SendWhisper("'" + BannedWord + "' Foi adicionado com sucesso ao filtro", 34);
                }
        }
    }
}