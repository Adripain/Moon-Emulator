using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Rooms;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class FollowCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";
        public string Parameters => "[USUARIO]";
        public string Description => "Siga um usuário até o quarto onde ele está.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Digite o nome corretamente.", 34);
                return;
            }

            GameClient TargetClient = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Ocorreu um erro, escreva o nome corretamente ou o usuário não está online.", 34);
                return;
            }

            if (TargetClient.GetHabbo().CurrentRoom == Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper("Hey! Abra seus olhos, o usuário " + TargetClient.GetHabbo().Username + " está neste quarto!", 34);
                return;
            }

            if (TargetClient.GetHabbo().Username == "Forbi" || TargetClient.GetHabbo().Username == "Forb")
            {
                Session.SendWhisper("Você não pode seguir esse usuário!", 34);
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("Sadooooooooo!", 34);
                return;
            }

            if (!TargetClient.GetHabbo().InRoom)
            {
                Session.SendWhisper("Ele não está em nenhum quarto", 34);
                return;
            }

            if (TargetClient.GetHabbo().CurrentRoom.Access != RoomAccess.OPEN && !Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                Session.SendWhisper("Oops, o usuário está em um quarto fechado com uma campainha ou senha, você não pode segui-lo!", 34);
                return;
            }

            Session.GetHabbo().PrepareRoom(TargetClient.GetHabbo().CurrentRoom.RoomId, "");
        }
    }
}
