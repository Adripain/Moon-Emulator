using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.GameClients;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class DisconnectCommand : IChatCommand
    {
        public string PermissionRequired => "user_10";
        public string Parameters => "[USUARIO]";
        public string Description => "Desconectar um usuário.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, digite o nome do usuário que você deseja desconectar.", 34);
                return;
            }

            GameClient TargetClient = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Ocorreu um erro, aparentemente, o usuário não existe ou não está online", 34);
                return;
            }

            if (TargetClient.GetHabbo().Username == "Forbi" || TargetClient.GetHabbo().Username == "Forb" || TargetClient.GetHabbo().Username == "Antoniocrevi")
            {
                Session.SendWhisper("Você não pode desconectar esse usuário!", 34);
                return;
            }

            TargetClient.GetConnection().Dispose();
        }
    }
}
