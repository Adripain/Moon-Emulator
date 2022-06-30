using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.HabboHotel.Rooms.AI;

namespace Moon.Communication.Packets.Outgoing.Rooms.AI.Pets
{
    class PetTrainingPanelComposer : ServerPacket
    {
        public PetTrainingPanelComposer(Pet pet)
            : base(ServerPacketHeader.PetTrainingPanelMessageComposer)
        {
            base.WriteInteger(pet.PetId);//Pet Id for sure.

            //Commands available to be done.
            base.WriteInteger(7);//Count
            {
                //base.WriteInteger(pet.Type);//Breed?
                base.WriteInteger(1);
                base.WriteInteger(2);
                base.WriteInteger(3);
                base.WriteInteger(4);
                base.WriteInteger(5);
                base.WriteInteger(6);
                base.WriteInteger(7);
            }

            //Commands that can be used NOW. (Level ups give you new commands etc).
            base.WriteInteger(7);//Count
            {
                //base.WriteInteger(pet.Type);//Breed?
                base.WriteInteger(1);
                base.WriteInteger(2);
                base.WriteInteger(3);
                base.WriteInteger(4);
                base.WriteInteger(5);
                base.WriteInteger(6);
                base.WriteInteger(7);
            }
        }

        public int GetCount(int Level)
        {
            switch(Level)
            {
                case 1:
                case 2:
                    return 1;

                case 3:
                case 4:
                    return 2;

                case 5:
                case 6:
                    return 3;

                case 7:
                case 8:
                    return 4;

                case 9:
                case 10:
                    return 5;

                case 11:
                case 12:
                    return 6;

                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                    return 8;

                default:
                    return 1;
            }
        }
    }
}
