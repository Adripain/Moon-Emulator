using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.GameClients;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class KickCommand : IChatCommand
    {
        public string PermissionRequired => "user_10";
        public string Parameters => "[USUARIO] [MENSAGEM]";
        public string Description => "Expulse o usuário e envie-lhe o motivo.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Digite o nome do usuário.", 34);
                return;
            }

            GameClient TargetClient = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Ocorreu um erro, aparentemente o usuário não existe ou não está online.", 34);
                return;
            }

            if (TargetClient.GetHabbo() == null)
            {
                Session.SendWhisper("Ocorreu um erro, aparentemente o usuário não existe ou não está online.", 34);
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("Get a life.", 34);
                return;
            }

            if (!TargetClient.GetHabbo().InRoom)
            {
                Session.SendWhisper("O usuário não está no quarto", 34);
                return;
            }

            Room TargetRoom;
            if (!MoonEnvironment.GetGame().GetRoomManager().TryGetRoom(TargetClient.GetHabbo().CurrentRoomId, out TargetRoom))
                return;

            if (Params.Length > 2)
                TargetClient.SendNotification("Um moderador expulsou você do quarto pelo seguinte motivo: " + CommandManager.MergeParams(Params, 2));
            else
                TargetClient.SendNotification("Um moderador expulsou você do quarto.");

            TargetRoom.GetRoomUserManager().RemoveUserFromRoom(TargetClient, true, false);
        }
    }
}
