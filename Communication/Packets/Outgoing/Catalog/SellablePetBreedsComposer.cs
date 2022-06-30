﻿using System;
using System.Linq;
using System.Collections.Generic;

using Moon.HabboHotel.Catalog.Pets;

namespace Moon.Communication.Packets.Outgoing.Catalog
{
    public class SellablePetBreedsComposer : ServerPacket
    {
        public SellablePetBreedsComposer(string PetType, int PetId, ICollection<PetRace> Races)
            : base(ServerPacketHeader.SellablePetBreedsMessageComposer)
        {
           base.WriteString(PetType);

            base.WriteInteger(Races.Count);
            foreach (PetRace Race in Races.ToList())
            {
                base.WriteInteger(PetId);
                base.WriteInteger(Race.PrimaryColour);
                base.WriteInteger(Race.SecondaryColour);
                base.WriteBoolean(Race.HasPrimaryColour);
                base.WriteBoolean(Race.HasSecondaryColour);
            }


        }
    }
}