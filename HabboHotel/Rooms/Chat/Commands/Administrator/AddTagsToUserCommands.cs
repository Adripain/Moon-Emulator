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
    class AddTagsToUserCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_11"; }
        }

        public string Parameters
        {
            get { return "<usuario> <tag>"; }
        }

        public string Description
        {
            get { return "Adiciona TAGS a um usuário."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length != 3)
            {
                Session.SendWhisper("Insira o nome de usuário a quem deseja enviar!");
                return;
            }

            GameClient TargetClient = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient != null)
            {
                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.runFastQuery("INSERT INTO `user_tags` (user_id, tag_name) VALUES(" + TargetClient.GetHabbo().Id + ", '" + Params[2] + "')");

                    TargetClient.GetHabbo().Tags.Add(Params[2]);
                }

                Session.SendMessage(RoomNotificationComposer.SendBubble("definitions", "Adicionou \"" + Params[2] + "\" a " + TargetClient.GetHabbo().Username + " satisfatoriamente.", ""));
                TargetClient.SendMessage(RoomNotificationComposer.SendBubble("definitions", Session.GetHabbo().Username + " te deu a TAG " + Params[2] + ".", ""));

                foreach (RoomUser RoomUser in Room.GetRoomUserManager().GetRoomUsers())
                {

                    RoomUser.GetClient().SendMessage(new UserTagsComposer(TargetClient.GetHabbo().Id, TargetClient));
                }
            }
        }
    }
}
