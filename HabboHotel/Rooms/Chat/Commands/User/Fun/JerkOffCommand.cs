using Moon.Communication.Interfaces;
using Moon.Communication.Packets.Outgoing.Rooms.Chat;
using Moon.HabboHotel.GameClients;
using System;
using System.Threading;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    internal class JerkOffCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get
            {
                return "user_normal";
            }
        }

        public string Parameters
        {
            get
            {
                return "";
            }
        }

        public string Description
        {
            get
            {
                return "Hacerse una paja.";
            }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            long nowTime = MoonEnvironment.CurrentTimeMillis();
            long timeBetween = nowTime - Session.GetHabbo()._lastTimeUsedHelpCommand;
            if (timeBetween < 300000)
            {
                Session.SendMessage(RoomNotificationComposer.SendBubble("advice", "Espera al menos 1 minuto para volver a usar el comando.", ""));
                return;
            }

            RoomUser roomUserByHabbo1 = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (roomUserByHabbo1 == null)
                return;
            {
                {
                    if (Session.GetHabbo().Gender == "m")
                    {
                        Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Se baja los pantalones, y saca su pene*", 0, 0), false);
                        Thread.Sleep(1000);
                        Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Le da a su pene, como si no hubiera mañana*", 0, 0), false);
                        Thread.Sleep(3000);
                        Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*¡Semén POR TODAS PARTES!* Lo siento chicos..", 0, 0), false);
                        Thread.Sleep(1000);
                        roomUserByHabbo1.ApplyEffect(9);
                        Thread.Sleep(3000);
                        roomUserByHabbo1.ApplyEffect(0);
                    }
                    else
                    {
                        Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Se baja los pantalones y empieza a frotar el clítoris*", 0, 0), false);
                        Thread.Sleep(2000);
                        Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Ohhhhh mmmm mmmmmm*", 0, 0), false);
                        Thread.Sleep(2000);
                        Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*¡Chorros por todas partes!* Lo siento chicos..", 0, 0), false);
                        Thread.Sleep(1000);
                        roomUserByHabbo1.ApplyEffect(9);
                        Thread.Sleep(3000);
                        roomUserByHabbo1.ApplyEffect(0);
                    }
                }
            }
        }
    }
}
