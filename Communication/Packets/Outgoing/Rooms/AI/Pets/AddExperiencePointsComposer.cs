using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Rooms.AI.Pets
{
    class AddExperiencePointsComposer : ServerPacket
    {
        public AddExperiencePointsComposer(int PetId, int VirtualId, int Amount)
            : base(ServerPacketHeader.AddExperiencePointsMessageComposer)
        {
            base.WriteInteger(PetId);
            base.WriteInteger(VirtualId);
            base.WriteInteger(Amount);
        }
    }
}