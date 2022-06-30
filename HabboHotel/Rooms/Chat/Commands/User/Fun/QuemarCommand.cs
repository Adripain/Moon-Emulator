using System;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Rooms.Chat;
using System.Threading;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class
        QuemarCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_normal"; }
        }
        public string Parameters
        {
            get { return "%usuario%"; }
        }
        public string Description
        {
            get { return "Quema a un usuario."; }
        }
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Session.GetHabbo().GetPermissions().HasCommand("user_vip"))
            {
                Session.SendMessage(RoomNotificationComposer.SendBubble("erro", "Ese comando es solo para usuarios VIP.", ""));
            }
            else
            {
                if (Params.Length == 1)
                {
                    Session.SendWhisper("¡Debe ingresar un nombre de usuario!", 34);
                    return;
                }

                GameClient TargetClient = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
                if (TargetClient == null)
                {
                    Session.SendWhisper("Ese usuario no se puede encontrar, tal vez no está en línea o no esta en la sala.", 34);
                    return;
                }

                RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
                if (User == null)
                {
                    Session.SendWhisper("Ese usuario no se puede encontrar, tal vez no está en línea o no esta en la sala.", 34);
                    return;
                }
                RoomUser Self = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                if (User == Self)
                {
                    Session.SendWhisper("¡No puedes quemarte!", 34);
                    return;
                }
                if (TargetClient.GetHabbo().Username == "Forbi" || TargetClient.GetHabbo().Username == "Forb")
                {
                    Session.SendWhisper("¡No puedes quemar a ese usuario!", 34);
                    return;
                }
                RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                if (ThisUser == null)
                    return;

                if (Math.Abs(User.X - ThisUser.X) < 2 && Math.Abs(User.Y - ThisUser.Y) < 2)
                {
                    Room.SendMessage(new ShoutComposer(ThisUser.VirtualId, "*He quemado a " + TargetClient.GetHabbo().Username + "*", 0, ThisUser.LastBubble));
                    Room.SendMessage(new ChatComposer(User.VirtualId, "*Se prende en fuego*", 0, User.LastBubble));

                    ThisUser.ApplyEffect(5);
                    User.ApplyEffect(25);
                    TargetClient.SendMessage(new FloodControlComposer(3));
                    if (User != null)
                        User.Frozen = true;
                    System.Threading.Thread thrd = new System.Threading.Thread(delegate ()
                    {
                        Thread.Sleep(4000);
                        if (User != null)
                            User.Frozen = false;
                        User.ApplyEffect(25);
                        Thread.Sleep(2000);
                        ThisUser.ApplyEffect(5);
                        User.ApplyEffect(25);
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
}