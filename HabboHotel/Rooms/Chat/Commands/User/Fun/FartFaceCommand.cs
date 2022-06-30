using System;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Rooms.Chat;
using System.Threading;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class
        FartFaceCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_vip"; }
        }
        public string Parameters
        {
            get { return "[USUARIO]"; }
        }
        public string Description
        {
            get { return "Jogue peido na cara do usuário."; }
        }
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
                if (Params.Length == 1)
                {
                    Session.SendWhisper("Você deve digitar um nome de usuário!", 34);
                    return;
                }

                GameClient TargetClient = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
                if (TargetClient == null)
                {
                    Session.SendWhisper("Este usuário não pode ser encontrado, talvez ele não esteja online ou não esteja no quarto.", 34);
                    return;
                }

                RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
                if (User == null)
                {
                    Session.SendWhisper("Este usuário não pode ser encontrado, talvez ele não esteja online ou não esteja no quarto.", 34);
                    return;
                }
                RoomUser Self = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                if (User == Self)
                {
                    Session.SendWhisper("Você não pode peidar em seu próprio rosto!", 34);
                    return;
                }
            if (TargetClient.GetHabbo().Username == "Forbi" || TargetClient.GetHabbo().Username == "Forb")
            {
                Session.SendWhisper("¡El es tu patón!", 34);
                return;
            }
            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                if (ThisUser == null)
                    return;

                if (Math.Abs(User.X - ThisUser.X) < 2 && Math.Abs(User.Y - ThisUser.Y) < 2)
                {
                    Room.SendMessage(new ShoutComposer(ThisUser.VirtualId, "*Jogar peido na cara de " + TargetClient.GetHabbo().Username + "'*", 0, User.LastBubble));
                    User.ApplyEffect(95);
                    ThisUser.ApplyEffect(96);
                    System.Threading.Thread thrd = new System.Threading.Thread(delegate ()
                    {
                        Thread.Sleep(2000);
                        User.ApplyEffect(0);
                        ThisUser.ApplyEffect(0);
                    });
                    thrd.Start();
                }
                else
                {
                    Session.SendWhisper("Ese usuario está demasiado lejos, trata de acercarte.", 34);
                    return;
                }
            }
        }
    }
