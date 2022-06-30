using System;
using System.Linq;
using Moon.Database.Interfaces;
using System.Data;
using Moon.HabboHotel.GameClients;

using Moon.HabboHotel.Users.Messenger;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class ChatAlertCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_vip"; }
        }
        public string Parameters
        {
            get { return "[USUARIO] [MENSAJE]"; }
        }
        public string Description
        {
            get { return "Enviar mensagem para um usuário."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
                if (Params.Length == 1)
                {
                    Session.SendWhisper("Digite o nome de usuário para o qual você deseja enviar uma mensagem.", 34);
                    return;
                }

                GameClient TargetClient = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
                if (TargetClient == null)
                {
                    Session.SendWhisper("O usuário não está online!", 34);
                    return;
                }

                if (TargetClient.GetHabbo() == null)
                {
                    Session.SendWhisper("O usuário não está online!", 34);
                    return;
                }

                if (TargetClient.GetHabbo().Rank >= 3)
                {
                    Session.SendWhisper("Você não pode enviar mensagens para Staffs!", 34);
                    return;
                }

                if (TargetClient.GetHabbo().AllowFriendRequests)
                {
                    Session.SendWhisper("O usuário desativou as mensagens!", 34);
                    return;
                }

                // Kolla om personerna är vänner!

                if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
                {
                    Session.SendWhisper("Você não pode enviar uma mensagem para si mesmo", 34);
                    return;
                }

                string Message = CommandManager.MergeParams(Params, 2);

                TargetClient.SendWhisper("Você recebeu uma mensagem de " + Session.GetHabbo().Username, 34);
                TargetClient.SendWhisper(Message, 34);
                Session.SendWhisper("Mensagem enviada para " + TargetClient.GetHabbo().Username + "!", 34);
            }

        }
    }