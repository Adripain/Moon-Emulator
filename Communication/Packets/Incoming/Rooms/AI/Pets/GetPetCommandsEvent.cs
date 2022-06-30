using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.Communication.Packets.Outgoing.Rooms.AI.Pets;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Rooms.AI;

namespace Moon.Communication.Packets.Incoming.Rooms.AI.Pets
{
    class GetPetCommandsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            int PetId = Packet.PopInt();

            RoomUser _pet = null;
            if (!Session.GetHabbo().CurrentRoom.GetRoomUserManager().TryGetPet(PetId, out _pet))
            {
                RoomUser User = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(PetId);
                if (User == null)
                    return;

                //Check some values first, please!
                if (User.GetClient() == null || User.GetClient().GetHabbo() == null)
                    return;

                //And boom! Let us send the information composer 8-).
                Session.SendMessage(new PetInformationComposer(User.GetClient().GetHabbo()));
                return;
            }

            //Continue as a regular pet..
            if (_pet.RoomId != Session.GetHabbo().CurrentRoomId || _pet.PetData == null)
                return;

            Pet pet = _pet.PetData;
            Session.SendMessage(new PetTrainingPanelComposer(pet));
        }
    }
}
