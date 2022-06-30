using Moon.Communication.Packets.Outgoing.Rooms.Engine;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using System;
using System.Text;
using Moon.Communication.Packets.Outgoing.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class PetCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";
        public string Parameters => "";
        public string Description => "Transformar-se em um PET.";

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {


            RoomUser RoomUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (RoomUser == null)
                return;

            if (!Room.PetMorphsAllowed)
            {
                Session.SendWhisper("O dono da sala não permite que você se torne um PET.");
                if (Session.GetHabbo().PetId > 0)
                {
                    Session.SendWhisper("Vá você ainda tem uma metamorfosis, un-morphing.");
                    Session.GetHabbo().PetId = 0;

                    Room.SendMessage(new UserRemoveComposer(RoomUser.VirtualId));
                    Room.SendMessage(new UsersComposer(RoomUser));
                }
                return;
            }

            if (Params.Length == 1)
            {
                StringBuilder List = new StringBuilder("");
                List.AppendLine("                              - LISTA DE PETS -");
                List.AppendLine("Colocando estes parâmetros usando o comando: pet você pode se transformar em: ");
                List.AppendLine("  l[»]l 1) cachorro - Transforme-se em um cachorro.");
                List.AppendLine("  l[»]l 2) gato - Transforme-se em um gato.");
                List.AppendLine("  l[»]l 3) terrier - Transforme-se em um fox terrier.");
                List.AppendLine("  l[»]l 4) cocodrilo - Transforme-se em um cocodrilo.");
                List.AppendLine("  l[»]l 5) urso - Transforme-se em um urso.");
                List.AppendLine("  l[»]l 6) porco - Transforme-se em um porco.");
                List.AppendLine("  l[»]l 7) leão - Transforme-se em um leão.");
                List.AppendLine("  l[»]l 8) rinoceronte - Transforme-se em um rinoceronte.");
                List.AppendLine("  l[»]l 9) aranha - Transforme-se em uma aranha.");
                List.AppendLine("  l[»]l 10) tartaruga - Transforme-se em uma tartaruga.");
                List.AppendLine("  l[»]l 11) pintinho - Transforme-se em um pintinho.");
                List.AppendLine("  l[»]l 12) sapo - Transforme-se em uma sapo.");
                List.AppendLine("  l[»]l 13) macaco - Transforme-se em um macaco.");
                List.AppendLine("  l[»]l 14) cavalo - Transforme-se em um cavalo.");
                List.AppendLine("  l[»]l 15) coelho - Transforme-se em um coelho.");
                List.AppendLine("  l[»]l 16) pássaro - Transforme-se em uma pássaro.");
                List.AppendLine("  l[»]l 17) demonio - Transforme-se em um demonio.");
                List.AppendLine("  l[»]l 18) gnomo - Transforme-se em um gnomo.");

                List.AppendLine("  l[»]l 19) piedra - Transforme-se em uma piedra.");
                List.AppendLine("  l[»]l 20) bebeoso - Transforme-se em um bebeoso.");
                List.AppendLine("  l[»]l 21) bebeguapo - Transforme-se em um bebeguapo.");
                List.AppendLine("  l[»]l 22) bebefeo - Transforme-se em um bebefeo.");
                List.AppendLine("  l[»]l 23) mario - Transfórmate en mario.");
                List.AppendLine("  l[»]l 24) pikachu - Transfórmate en pikachu.");
                List.AppendLine("  l[»]l 25) lobo - Transforme-se em um lobo.");
                List.AppendLine("  l[»]l 26) elefante - Transforme-se em um elefante.");
                List.AppendLine("  l[»]l 27) pinguino - Transforme-se em um pinguino.");
                List.AppendLine("  l[»]l 28) vaca - Transforme-se em uma vaca.");
                List.AppendLine("  l[»]l 29) velociraptor - Transforme-se em um velociraptor.");
                List.AppendLine("  l[»]l 30) pterosaur - Transforme-se em um pterosaur.");
                List.AppendLine("  l[»]l 31) haloompa - Transforme-se em um haloompa.");
                List.AppendLine("  l[»]l 32) cerdito - Transforme-se em um cerdito.");
                List.AppendLine("  l[»]l 33) perrito - Transforme-se em um perrito.");
                List.AppendLine("  l[»]l 34) gatito - Transforme-se em um gatito.");
                List.AppendLine("  l[»]l 35) bebeterrier - Transforme-se em um bebeterrier.");
                List.AppendLine("  l[»]l 36) bebeoso - Transforme-se em um bebeoso.");

                Session.SendMessage(new MOTDNotificationComposer(List.ToString()));
                return;
            }

            if (Params[1].ToString().ToLower() == "list")
            {
                StringBuilder List = new StringBuilder("");
                List.AppendLine("                              - LISTA DE PETS -");
                List.AppendLine("Colocando estes parâmetros usando o comando: pet você pode se transformar em: ");
                List.AppendLine("  l[»]l 1) cachorro - Transforme-se em um cachorro.");
                List.AppendLine("  l[»]l 2) gato - Transforme-se em um gato.");
                List.AppendLine("  l[»]l 3) terrier - Transforme-se em um fox terrier.");
                List.AppendLine("  l[»]l 4) cocodrilo - Transforme-se em um cocodrilo.");
                List.AppendLine("  l[»]l 5) urso - Transforme-se em um urso.");
                List.AppendLine("  l[»]l 6) porco - Transforme-se em um porco.");
                List.AppendLine("  l[»]l 7) leão - Transforme-se em um leão.");
                List.AppendLine("  l[»]l 8) rinoceronte - Transforme-se em um rinoceronte.");
                List.AppendLine("  l[»]l 9) aranha - Transforme-se em uma aranha.");
                List.AppendLine("  l[»]l 10) tartaruga - Transforme-se em uma tartaruga.");
                List.AppendLine("  l[»]l 11) pintinho - Transforme-se em um pintinho.");
                List.AppendLine("  l[»]l 12) sapo - Transforme-se em uma sapo.");
                List.AppendLine("  l[»]l 13) macaco - Transforme-se em um macaco.");
                List.AppendLine("  l[»]l 14) cavalo - Transforme-se em um cavalo.");
                List.AppendLine("  l[»]l 15) coelho - Transforme-se em um coelho.");
                List.AppendLine("  l[»]l 16) pássaro - Transforme-se em uma pássaro.");
                List.AppendLine("  l[»]l 17) demonio - Transforme-se em um demonio.");
                List.AppendLine("  l[»]l 18) gnomo - Transforme-se em um gnomo.");

                List.AppendLine("  l[»]l 19) piedra - Transforme-se em uma piedra.");
                List.AppendLine("  l[»]l 20) bebeoso - Transforme-se em um bebeoso.");
                List.AppendLine("  l[»]l 21) bebeguapo - Transforme-se em um bebeguapo.");
                List.AppendLine("  l[»]l 22) bebefeo - Transforme-se em um bebefeo.");
                List.AppendLine("  l[»]l 23) mario - Transfórmate en mario.");
                List.AppendLine("  l[»]l 24) pikachu - Transfórmate en pikachu.");
                List.AppendLine("  l[»]l 25) lobo - Transforme-se em um lobo.");
                List.AppendLine("  l[»]l 26) elefante - Transforme-se em um elefante.");
                List.AppendLine("  l[»]l 27) pinguino - Transforme-se em um pinguino.");
                List.AppendLine("  l[»]l 28) vaca - Transforme-se em uma vaca.");
                List.AppendLine("  l[»]l 29) velociraptor - Transforme-se em um velociraptor.");
                List.AppendLine("  l[»]l 30) pterosaur - Transforme-se em um pterosaur.");
                List.AppendLine("  l[»]l 31) haloompa - Transforme-se em um haloompa.");
                List.AppendLine("  l[»]l 32) cerdito - Transforme-se em um cerdito.");
                List.AppendLine("  l[»]l 33) perrito - Transforme-se em um perrito.");
                List.AppendLine("  l[»]l 34) gatito - Transforme-se em um gatito.");
                List.AppendLine("  l[»]l 35) bebeterrier - Transforme-se em um bebeterrier.");
                List.AppendLine("  l[»]l 36) bebeoso - Transforme-se em um bebeoso.");

                Session.SendMessage(new MOTDNotificationComposer(List.ToString()));
                return;
            }

            int TargetPetId = GetPetIdByString(Params[1].ToString());
            if (TargetPetId == 0)
            {
                Session.SendWhisper("Vaya, no hay mascota con ese nombre!");
                return;
            }

            //Change the users Pet Id.
            Session.GetHabbo().PetId = (TargetPetId == -1 ? 0 : TargetPetId);

            //Quickly remove the old user instance.
            Room.SendMessage(new UserRemoveComposer(RoomUser.VirtualId));

            //Add the new one, they won't even notice a thing!!11 8-)
            Room.SendMessage(new UsersComposer(RoomUser));

            //Tell them a quick message.
            if (Session.GetHabbo().PetId > 0)
                Session.SendWhisper("Usa ':pet habbo' para regresarte a tu keko!");
        }

        private int GetPetIdByString(string Pet)
        {
            switch (Pet.ToLower())
            {
                default:
                    return 0;
                case "habbo":
                    return -1;
                case "perro":
                    return 60;//This should be 0.
                case "gato":
                case "1":
                    return 1;
                case "terrier":
                case "2":
                    return 2;
                case "croc":
                case "croco":
                case "cocodrilo":
                case "3":
                    return 3;
                case "oso":
                case "4":
                    return 4;
                case "liz":
                case "cerdo":
                case "kill":
                case "5":
                    return 5;
                case "leon":
                case "rawr":
                case "6":
                    return 6;
                case "rhino":
                case "rinoceronte":
                case "7":
                    return 7;
                case "spider":
                case "arana":
                case "araña":
                case "8":
                    return 8;
                case "tortuga":
                case "9":
                    return 9;
                case "chick":
                case "chicken":
                case "pollo":
                case "10":
                    return 10;
                case "frog":
                case "rana":
                case "11":
                    return 11;
                case "drag":
                case "dragon":
                case "12":
                    return 12;
                case "monkey":
                case "mono":
                case "14":
                    return 14;
                case "horse":
                case "caballo":
                case "15":
                    return 15;
                case "bunny":
                case "conejo":
                case "17":
                    return 17;
                case "pigeon":
                case "pajaro":
                case "21":
                    return 21;
                case "demon":
                case "demonio":
                case "23":
                    return 23;
                case "babybear":
                case "bebeoso":
                case "24":
                    return 24;
                case "babyterrier":
                case "bebeterrier":
                case "25":
                    return 25;
                case "gnome":
                case "gnomo":
                case "26":
                    return 26;
                case "kitten":
                case "gatito":
                case "28":
                    return 28;
                case "puppy":
                case "perrito":
                case "29":
                    return 29;
                case "piglet":
                case "cerdito":
                case "30":
                    return 30;
                case "haloompa":
                case "31":
                    return 31;
                case "rock":
                case "piedra":
                case "32":
                    return 32;
                case "pterosaur":
                case "33":
                    return 33;
                case "velociraptor":
                case "34":
                    return 34;
                case "vaca":
                case "35":
                    return 35;
                case "pinguino":
                case "36":
                    return 36;
                case "elefante":
                case "37":
                    return 37;
                case "bebeguapo":
                case "38":
                    return 38;
                case "bebefeo":
                case "39":
                    return 39;
                case "mario":
                case "40":
                    return 40;
                case "pikachu":
                case "41":
                    return 41;
                case "lobo":
                case "42":
                    return 42;
            }
        }
    }
}