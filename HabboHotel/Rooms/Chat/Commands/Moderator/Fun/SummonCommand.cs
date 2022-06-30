using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Rooms.Session;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class SummonCommand : IChatCommand
    {
        public string PermissionRequired => "user_7";
        public string Parameters => "[USUARIO]";
        public string Description => "Trazer um usuário para o quarto atual.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, digite o nome do usuário que você deseja trazer para o quarto", 34);
                return;
            }

            GameClient TargetClient = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Ocorreu um erro, o usuário não foi encontrado ou talvez não esteja online", 34);
                return;
            }

            if (TargetClient.GetHabbo() == null)
            {
                Session.SendWhisper("Ocorreu um erro, o usuário não foi encontrado ou talvez não esteja online", 34);
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("Get a life.", 34);
                return;
            }

            if (TargetClient.GetHabbo().Username == "Forbi" || TargetClient.GetHabbo().Username == "Forb")
            {
                Session.SendWhisper("Você não pode trazer esse usuário!", 34);
                return;
            }

            TargetClient.SendMessage(RoomNotificationComposer.SendBubble("voado", "Você foi chamado por " + Session.GetHabbo().Username + "!", ""));
            if (!TargetClient.GetHabbo().InRoom)
                TargetClient.SendMessage(new RoomForwardComposer(Session.GetHabbo().CurrentRoomId));
            else
                TargetClient.GetHabbo().PrepareRoom(Session.GetHabbo().CurrentRoomId, "");
        }
    }
}