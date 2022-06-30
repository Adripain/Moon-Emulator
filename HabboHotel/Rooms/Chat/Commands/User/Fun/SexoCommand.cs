using Moon.HabboHotel.Rooms;
using Moon.Communication.Interfaces;
using Moon.Communication.Packets.Outgoing.Rooms.Chat;
using Moon.HabboHotel.GameClients;
using System;
using System.Threading;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class
       SexoCommand : IChatCommand
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
                return "Faça sexo com um usuário.";
            }
        }
        // Coded By Hamada.
        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            long nowTime = MoonEnvironment.CurrentTimeMillis();
            long timeBetween = nowTime - Session.GetHabbo()._lastTimeUsedHelpCommand;
            if (timeBetween < 300000)
            {
                Session.SendMessage(RoomNotificationComposer.SendBubble("advice", "Espere pelo menos 1 minuto para reutilizar o comando.", ""));
                return;
            }

            Session.GetHabbo()._lastTimeUsedHelpCommand = nowTime;

            RoomUser roomUserByHabbo1 = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (roomUserByHabbo1 == null)
                return;

            if (Params.Length == 0)
            {
                Session.SendWhisper("Você deve digitar o nome de usuário da pessoa com quem deseja fazer sexo.", 34);
            }
            else
            {
                RoomUser roomUserByHabbo2 = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Params[1]);
                GameClient clientByUsername = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);

                if (clientByUsername.GetHabbo().Username == "Forbi" || clientByUsername.GetHabbo().Username == "Forb")
                {
                    Session.SendWhisper("Você não pode ter relacionamentos com esse usuário!", 34);
                    return;
                }

                if (clientByUsername.GetHabbo().Username == Session.GetHabbo().Username)
                {

                    RoomUser Self = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                    if (roomUserByHabbo1 == Self)
                    {
                        Session.SendWhisper("Você não pode fazer sexo com você mesmo!", 34);
                        return;
                    }
                }
                else if (clientByUsername.GetHabbo().CurrentRoomId == Session.GetHabbo().CurrentRoomId && (Math.Abs(checked(roomUserByHabbo1.X - roomUserByHabbo2.X)) < 2 && Math.Abs(checked(roomUserByHabbo1.Y - roomUserByHabbo2.Y)) < 2))
                {
                    if ((Session.GetHabbo().sexWith == null || Session.GetHabbo().sexWith == "") && (clientByUsername.GetHabbo().Username != Session.GetHabbo().sexWith && Session.GetHabbo().Username != clientByUsername.GetHabbo().sexWith))
                    {
                        Session.GetHabbo().sexWith = clientByUsername.GetHabbo().Username;
                        clientByUsername.SendNotification(Session.GetHabbo().Username + " pediu para ter sexo com você, ter relações com " + Session.GetHabbo().Username + " escreva \":sexo " + Session.GetHabbo().Username + "\"");
                        Session.SendNotification("Você enviou sua solicitação de sexo para " + clientByUsername.GetHabbo().Username + ". Se você responder, você poderá fazer sexo.");
                    }
                    else if (roomUserByHabbo2 != null)
                    {
                        if (clientByUsername.GetHabbo().sexWith == Session.GetHabbo().Username)
                        {
                            if (roomUserByHabbo2.GetClient() != null && roomUserByHabbo2.GetClient().GetHabbo() != null)
                            {
                                if (clientByUsername.GetHabbo().CurrentRoomId == Session.GetHabbo().CurrentRoomId && (Math.Abs(checked(roomUserByHabbo1.X - roomUserByHabbo2.X)) < 2 && Math.Abs(checked(roomUserByHabbo1.Y - roomUserByHabbo2.Y)) < 2))
                                {
                                    clientByUsername.GetHabbo().sexWith = (string)null;
                                    Session.GetHabbo().sexWith = (string)null;
                                    if (Session.GetHabbo().Gender == "m")
                                    {
                                        Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Agarro o" + Params[1] + " ele vira, e começo a meter com força* ", 0, 16), false);
                                        Thread.Sleep(2000);
                                        roomUserByHabbo1.ApplyEffect(9);
                                        roomUserByHabbo2.ApplyEffect(9);
                                        Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "*Olho de forma maliciosa o" + Session.GetHabbo().Username + " * ", 0, 16), false);
                                        Thread.Sleep(2000);
                                        Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Tocando devagar o " + Params[1] + ", coloco meu pau * ", 0, 16), false);
                                        Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Você gosta né " + Params[1] + " ?, enfiando rapido* ", 0, 16), false);
                                        Room.SendMessage((IServerPacket)new ShoutComposer(roomUserByHabbo2.VirtualId, "*Chupar o " + Session.GetHabbo().Username + ", me encanta *", 0, 16), false);
                                        Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "ahhh ahhh, como é bom gozar, ahhhhhhh *", 0, 16), false);
                                        Thread.Sleep(2000);
                                        Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*É gostoso, estou prestes a gozar*", 0, 16), false);
                                        Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "*Toco minha virilha para melhorar o orgasmo " + Session.GetHabbo().Username + " *", 0, 16), false);
                                        Thread.Sleep(2000);
                                        Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Pego meu pau e gozo nela " + Params[1] + " * ", 0, 16), false);
                                        Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "*Olhar malicioso* Espero que isso se repita!", 0, 16), false);
                                        Thread.Sleep(2000);
                                        roomUserByHabbo1.ApplyEffect(9);
                                        roomUserByHabbo2.ApplyEffect(9);
                                    }
                                    else
                                    {
                                        Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "*Toca las entrepiernas de " + Session.GetHabbo().Username + "*", 0, 16), false);
                                        Thread.Sleep(1000);
                                        roomUserByHabbo1.ApplyEffect(9);
                                        roomUserByHabbo2.ApplyEffect(9);
                                        Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Mirada picara a " + Params[1] + "*", 0, 16), false);
                                        Thread.Sleep(2000);
                                        Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "*Golpea suavemente el coño de " + Session.GetHabbo().Username + "*", 0, 16), false);
                                        Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Mmmmmm* Esto se siente bien.. |.|", 0, 16), false);
                                        Thread.Sleep(2000);
                                        Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "*Frota las entrepiertas de " + Session.GetHabbo().Username + "* ", 0, 16), false);
                                        Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*mm, mm, mm, mm, mmmmmm!*", 0, 16), false);
                                        Thread.Sleep(2000);
                                        Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "*Retrocede y le acaba en el cuerpo a " + Session.GetHabbo().Username + "*", 0, 16), false);
                                        Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Morder el labio* ¡Eso fue increíble!", 0, 16), false);
                                        Thread.Sleep(1000);
                                        roomUserByHabbo1.ApplyEffect(0);
                                        roomUserByHabbo2.ApplyEffect(0);
                                    }

                                }
                                else
                                    Session.SendWhisper("Este usuário está muito longe para fazer sexo com você!", 34);
                            }
                            else
                                Session.SendWhisper("Houve um erro ao encontrar o usuário.", 34);
                        }
                        else
                            Session.SendWhisper("Este usuário não aceitou sua solicitação por sexo.", 34);
                    }
                    else
                        Session.SendWhisper("Este usuário não pôde ser encontrado na sala.", 34);
                }
                else
                    Session.SendWhisper("Esse usuário está muito longe para fazer sexo com você!", 34);
            }
        }
    }
}
