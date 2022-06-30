using Moon.HabboHotel.Rooms;
using Moon.Communication.Interfaces;
using Moon.Communication.Packets.Outgoing.Rooms.Chat;
using Moon.HabboHotel.GameClients;
using System;
using System.Threading;


namespace Moon.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class
       SexCommand : IChatCommand
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
                return "Tener sexo sin permiso con un usuario.";
            }
        }
        // Coded By Hamada.
        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            RoomUser roomUserByHabbo1 = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (roomUserByHabbo1 == null)
                return;

            if (Params.Length == 0)
            {
                Session.SendWhisper("Debe introducir el nombre de usuario de la persona con la que desea tener relaciones sexuales.", 34);
            }
            else
            {
                RoomUser roomUserByHabbo2 = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Params[1]);
                GameClient clientByUsername = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
                if (clientByUsername.GetHabbo().Username == "Forbi" || clientByUsername.GetHabbo().Username == "Forb")
                {
                    Session.SendWhisper("¡No puedes tener relaciones con ese usuario!", 34);
                    return;
                }

                if (clientByUsername.GetHabbo().Username == Session.GetHabbo().Username)
                {

                    RoomUser Self = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                    if (roomUserByHabbo1 == Self)
                    {
                        Session.SendWhisper("¡No puedes tener sexo contigo mismo!", 34);
                        return;
                    }
                }
                else if (clientByUsername.GetHabbo().CurrentRoomId == Session.GetHabbo().CurrentRoomId && (Math.Abs(checked(roomUserByHabbo1.X - roomUserByHabbo2.X)) < 2 && Math.Abs(checked(roomUserByHabbo1.Y - roomUserByHabbo2.Y)) < 2))
                {
                    if (roomUserByHabbo2.GetClient() != null && roomUserByHabbo2.GetClient().GetHabbo() != null)
                    {
                        if (clientByUsername.GetHabbo().CurrentRoomId == Session.GetHabbo().CurrentRoomId && (Math.Abs(checked(roomUserByHabbo1.X - roomUserByHabbo2.X)) < 2 && Math.Abs(checked(roomUserByHabbo1.Y - roomUserByHabbo2.Y)) < 2))
                        {
                            if (Session.GetHabbo().Gender == "m")

                            {
                                Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Agarra por detrás a " + Params[1] + " y empiezan a tener sex*", 0, 0), false);
                                Thread.Sleep(2000);
                                roomUserByHabbo1.ApplyEffect(507);
                                roomUserByHabbo2.ApplyEffect(507);
                                Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "*Se pone en cuatro para que " + Session.GetHabbo().Username + " inserte su miembro*", 0, 0), false);
                                Thread.Sleep(2000);
                                Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Empieza empujando fuertemente a " + Params[1] + ", haciéndolo gemir*", 0, 0), false);
                                Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*¡Di mi nombre!, " + Params[1] + " ¡DILO EN VOZ ALTA!*", 0, 0), false);
                                Room.SendMessage((IServerPacket)new ShoutComposer(roomUserByHabbo2.VirtualId, " " + Session.GetHabbo().Username + " ", 0, 0), false);
                                Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "ahhh ahhh, que bien se siente, ahhhhhhh*", 0, 0), false);
                                Thread.Sleep(2000);
                                Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Estoy apunto de acabar*", 0, 0), false);
                                Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "*Toca su entrepierna, para hacer el orgasmo mejor para " + Session.GetHabbo().Username + "*", 0, 0), false);
                                Thread.Sleep(2000);
                                Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Acaba adentro de " + Params[1] + "*", 0, 0), false);
                                Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "*Morder el labio* ¡Eso fue increíble!", 0, 0), false);
                                Thread.Sleep(2000);
                                roomUserByHabbo1.ApplyEffect(9);
                                roomUserByHabbo2.ApplyEffect(9);
                            }
                            else
                            {
                                Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "*Tocas las entrepiernas de " + Session.GetHabbo().Username + "*", 0, 0), false);
                                Thread.Sleep(1000);
                                roomUserByHabbo1.ApplyEffect(507);
                                roomUserByHabbo2.ApplyEffect(507);
                                Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Se quita los pantalones*", 0, 0), false);
                                Thread.Sleep(2000);
                                Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "*Golpea el coño de " + Session.GetHabbo().Username + "*", 0, 0), false);
                                Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Mmmmmm* Esto se siente bien..", 0, 0), false);
                                Thread.Sleep(2000);
                                Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "*Acelera y empuja más profundamente*", 0, 0), false);
                                Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*mm, mm, mm, mm, mmmmmm!*", 0, 0), false);
                                Thread.Sleep(2000);
                                Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "*Retrocede y le acaba adentro a " + Session.GetHabbo().Username + "*", 0, 0), false);
                                Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Morder el labio* ¡Eso fue increíble!", 0, 0), false);
                                Thread.Sleep(1000);
                                roomUserByHabbo1.ApplyEffect(9);
                                roomUserByHabbo2.ApplyEffect(9);
                            }

                        }
                        else
                            Session.SendWhisper("Ese usuario está demasiado lejos para tener sexo contigo!", 34);
                    }
                    else
                        Session.SendWhisper("Se ha producido un error al encontrar a el usuario.", 34);
                }
                else
                    Session.SendWhisper("Ese usuario está demasiado lejos para tener sexo contigo!", 34);
            }
        }

    }
}