using System;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Rooms.Chat;
using System.Threading;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class
        WelcomeCommand : IChatCommand
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
            get { return "Dar Bem-vindo a um usuário."; }
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
                    Session.SendWhisper("Esse usuário não pode ser encontrado, talvez ele não esteja online ou não esteja na sala.", 34);
                    return;
                }

                RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
                if (User == null)
                {
                    Session.SendWhisper("O usuário não pode ser encontrado, talvez ele não esteja online ou não esteja na sala.", 34);
                    return;
                }
                RoomUser Self = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                if (User == Self)
                {
                    Session.SendWhisper("Você não pode dar Bem-Vindo!", 34);
                    return;
                }
                RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                if (ThisUser == null)
                    return;

                if (Math.Abs(User.X - ThisUser.X) < 2000 && Math.Abs(User.Y - ThisUser.Y) < 2000)
                {
                    Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "Olá " + TargetClient.GetHabbo().Username + ", Bem-vindo a comunidade " + MoonEnvironment.HotelName+"!", 0, 4));
                    Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "Aqui você poderá se divertir, conversar e fazer novos amigos.", 0, 4));
                    Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "Espero que você se divirta :D.", 0, 4));
                ThisUser.ApplyEffect(0);
                    System.Threading.Thread thrd = new System.Threading.Thread(delegate ()
                    {
                        Thread.Sleep(4000);
                        ThisUser.ApplyEffect(0);
                    });
                    thrd.Start();
                }
            }
        }
    }