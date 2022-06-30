using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.Database.Interfaces;
using Moon.HabboHotel.GameClients;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class PrefixCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_prefix"; }
        }

        public string Parameters
        {
            get { return "%prefix%"; }
        }

        public string Description
        {
            get { return "Exclui suas Tags."; }
        }

        public void Execute(GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, escreva \":prefix off\" para desativar suas tags.");
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);

            if (Message == "off")
            {
                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `users` SET `tag` = NULL WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                }
                Session.GetHabbo()._tag = string.Empty;
                Session.SendWhisper("Tag excluída corretamente.", 34);
            }
        }
    }
}
