using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Items;

namespace Moon.Communication.Packets.Outgoing.Rooms.FloorPlan
{
    class FloorPlanFloorMapComposer : ServerPacket
    {
        public FloorPlanFloorMapComposer(ICollection<Item> Items)
            : base(ServerPacketHeader.FloorPlanFloorMapMessageComposer)
        {
            base.WriteInteger(Items.Count);//TODO: Figure this out, it pushes the room coords, but it iterates them, x,y|x,y|x,y|and so on.
            foreach (Item Item in Items.ToList())
            {
                base.WriteInteger(Item.GetX);
                base.WriteInteger(Item.GetY);
            }
        }
    }
}
