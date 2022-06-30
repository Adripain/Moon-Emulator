using Moon.Communication.Interfaces;
using Moon.Communication.Packets.Outgoing.Rooms.Chat;
using Moon.HabboHotel.GameClients;
using System;
using System.Threading;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

// Coded for HabboPlux.com  //
namespace Moon.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    internal class hacerpaja : IChatCommand
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
                return "[USUARIO]";
            }
        }

        public string Description
        {
            get
            {
                return "Hacer una paja a un usuario.";
            }
        }
        // Coded for HabboPlux.com  //
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



            GameClient TargetClient = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Error, No se ha encontrado a ese usuario.");
                return;
            }

            if (TargetClient.GetHabbo().Username == "Forbi" || TargetClient.GetHabbo().Username == "Forb")
            {
                Session.SendWhisper("¡No puedes hacerle la paja a ese usuario!", 34);
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            if (User == null)
            {
                Session.SendWhisper("Error, No se ha encontrado a ese usuario.");
                return;
            }

            if (Params.Length == 0)
            {
                Session.SendWhisper("Debe introducir el nombre del usuario con la que desea hacerle la paja.", 0);
            }
            else
            {
                RoomUser roomUserByHabbo2 = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Params[1]);
                GameClient clientByUsername = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
                if (clientByUsername.GetHabbo().Username == Session.GetHabbo().Username)
                {

                    RoomUser Self = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                    if (roomUserByHabbo1 == Self)
                    {
                        Session.SendWhisper("¡Debes de introducir el nombre del usuario a quien quieres hacerle la paja!");
                        return;
                    }
                }
                else if (clientByUsername.GetHabbo().CurrentRoomId == Session.GetHabbo().CurrentRoomId && (Math.Abs(checked(roomUserByHabbo1.X - roomUserByHabbo2.X)) < 2 && Math.Abs(checked(roomUserByHabbo1.Y - roomUserByHabbo2.Y)) < 2))
                {
                    if (roomUserByHabbo2.GetClient() != null && roomUserByHabbo2.GetClient().GetHabbo() != null)
                    {
                        if (clientByUsername.GetHabbo().CurrentRoomId == Session.GetHabbo().CurrentRoomId && (Math.Abs(checked(roomUserByHabbo1.X - roomUserByHabbo2.X)) < 2 && Math.Abs(checked(roomUserByHabbo1.Y - roomUserByHabbo2.Y)) < 2))
                        {
                            if (roomUserByHabbo2.GetClient().GetHabbo().Gender == "m")
                            {
                                Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Agarra a " + Params[1] + " y le baja los pantalones*", 0, 0), false);
                                Thread.Sleep(2000);
                                roomUserByHabbo1.ApplyEffect(507);
                                Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Le agarra la poronga a " + Session.GetHabbo().Username + " y comienza a frotarla*", 0, 0), false);
                                Thread.Sleep(2000);
                                Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Le frota tanto la porogna a  " + Params[1] + ", que lo hace gemir*", 0, 0), false);
                                Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "*¡Estoy a punto de correrme, ahhhhh!", 0, 0), false);
                                Thread.Sleep(2000);
                                roomUserByHabbo1.ApplyEffect(9);
                                roomUserByHabbo2.ApplyEffect(9);
                            }
                            else
                            {
                                Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*le baja los pantalones y empieza a frotarle el clítoris a *" + Params[1], 0, 0), false);
                                Thread.Sleep(2000);
                                roomUserByHabbo1.ApplyEffect(507);
                                Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "*Ohhhhh mmmm mmmmmm*", 0, 0), false);
                                Thread.Sleep(2000);
                                Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "*" + Params[1] + "*" + "*¡Ha tirado chorros por todas partes!,", 0, 0), false);
                                Thread.Sleep(1000);
                                roomUserByHabbo1.ApplyEffect(9);
                                Thread.Sleep(3000);
                                roomUserByHabbo1.ApplyEffect(0);
                            }

                        }
                        else
                            Session.SendWhisper("Ese usuario está demasiado lejos para hacerle una paja.", 0);
                    }
                    else
                        Session.SendWhisper("Se ha producido un error al encontrar a el usuario.", 0);
                }
                else
                    Session.SendWhisper("Ese usuario está demasiado lejos para hacerle una paja.", 0);
            }
        }

    }
}