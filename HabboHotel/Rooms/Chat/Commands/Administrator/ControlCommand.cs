using System;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.Communication.Packets.Outgoing.Nux;
using Moon.Communication.Packets.Outgoing.Rooms.Furni.RentableSpaces;
using Moon.Communication.Packets.Outgoing.Moderation;
using Moon.Communication.Packets.Outgoing.Catalog;
using Moon.Communication.Packets.Outgoing.Users;
using Moon.Database.Interfaces;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class ControlCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_16"; }
        }

        public string Parameters
        {
            get { return "<usuario>"; }
        }

        public string Description
        {
            get { return "Controla o usuário que escolher."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length != 2)
            {
                Session.SendWhisper("Introduza o nome do usuário que deseja controlar!");
                return;
            }

            if (Params.Length == 2 && Params[1] == "end")
            {
                Session.SendWhisper("Deixou de controlar o " + Session.GetHabbo().Opponent +".");
                Session.GetHabbo().isControlling = false;
                return;
            }

            GameClient TargetClient = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient != null)
            {
                Session.GetHabbo().Opponent = TargetClient.GetHabbo().Username;
                Session.GetHabbo().isControlling = true;
                Session.SendMessage(RoomNotificationComposer.SendBubble("definitions", "Você agora controla o/a " + TargetClient.GetHabbo().Username + ". Para parar diga :control end."));
                return;
            }

            else Session.SendMessage(RoomNotificationComposer.SendBubble("definitions", "Não encontramos o usuário " + Params[1] + ".", ""));
        }
    }
}
