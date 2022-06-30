using Moon.Communication.Interfaces;
using Moon.Communication.Packets.Outgoing.Rooms.Chat;
using Moon.HabboHotel.GameClients;
using System;
using System.Threading;
using Moon.Communication.Packets.Outgoing.Inventory.Purse;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    internal class FumarCommand : IChatCommand
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
                return "[ACCEPT]";
            }
        }

        public string Description
        {
            get
            {
                return "Fumar um baseado , baseado custa (1000c)..";
            }
        }
        // Coded By Hamada.
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendNotification("Tem certeza de que quer comprar um Baseado ? Ele vai custar 1000créditos\n\n" +
                 "Se você aceitar, escreva \":fumar accept\". \n Ao fazer isso, não há como voltar atrás!\n\n(Se você não quiser comprar o Baseado, ignore essa mensagem!)\n\n");
                return;
            }
            RoomUser roomUserByHabbo = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (roomUserByHabbo == null)
                return;
            if (Params.Length == 2 && Params[1].ToString() == "accept")
            {
                Session.GetHabbo().Credits = Session.GetHabbo().Credits -= 1000;
                Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                roomUserByHabbo.GetClient().SendWhisper("Você comprou com sucesso um baseado por 1000 créditos!");
                Thread.Sleep(1000);
                Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo.VirtualId, "*Eu tiro a maconha e misturo* ", 0, 6), false);
                Thread.Sleep(2000);
                Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo.VirtualId, "*Eu rolo, e fumo*", 0, 6), false);
                Thread.Sleep(2000);
                roomUserByHabbo.ApplyEffect(53);
                Thread.Sleep(2000);
                switch (new Random().Next(1, 4))
                {
                    case 1:
                        Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo.VirtualId, "Ei, por que eu sinto que estou voando? Porque eu voei da casa de caralho e fui pra o céu, hahahaha", 0, 6), false);
                        break;
                    case 2:
                        roomUserByHabbo.ApplyEffect(70);
                        Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo.VirtualId, "YWTF! Eu sinto que meu rosto é grosso, faz sooool bro !!", 0, 6), false);
                        break;
                    default:
                        Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo.VirtualId, "Hehehe voy muy drogado necesito cagar, me gusta es un caramelo :D", 0, 6), false);
                        break;
                }
                Thread.Sleep(2000);
                Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo.VirtualId, "HAHAAHHAHAHAHAAHAHAHHAHAHAHA LOL", 0, 6), false);
                Thread.Sleep(2000);
                roomUserByHabbo.ApplyEffect(0);
                Thread.Sleep(2000);
                Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo.VirtualId, "*Eu fumo o resto e fico na larica*", 0, 6), false);
            }

        }
    }
}