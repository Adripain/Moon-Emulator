using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Items;
using Moon.HabboHotel.Groups;
using Moon.HabboHotel.Users;
using Moon.HabboHotel.Catalog.Utilities;

namespace Moon.Communication.Packets.Outgoing.Inventory.Furni
{
    class FurniListComposer : ServerPacket
    {
        public FurniListComposer(List<Item> Items, ICollection<Item> Walls)
            : base(ServerPacketHeader.FurniListMessageComposer)
        {
            base.WriteInteger(1);
            base.WriteInteger(1);

            base.WriteInteger(Items.Count + Walls.Count);
            foreach (Item Item in Items.ToList())
            {
                WriteItem(Item);
            }

            foreach (Item Item in Walls.ToList())
            {
                WriteItem(Item);
            }
        }

        public FurniListComposer(List<Item> Items, int page, int pages)
            : base(ServerPacketHeader.FurniListMessageComposer)
        {
            base.WriteInteger(pages);
            base.WriteInteger(page);

            base.WriteInteger(Items.Count);
            foreach (Item Item in Items.ToList())
            {
                WriteItem(Item);
            }
        }

        public FurniListComposer()
           : base(ServerPacketHeader.FurniListMessageComposer)
        {
            base.WriteInteger(1);
            base.WriteInteger(1);

            base.WriteInteger(0);
        }

        private void WriteItem(Item Item)
        {
            base.WriteInteger(Item.Id);
            base.WriteString(Item.GetBaseItem().Type.ToString().ToUpper());
            base.WriteInteger(Item.Id);
            base.WriteInteger(Item.GetBaseItem().SpriteId);

            if (Item.LimitedNo > 0)
            {
                base.WriteInteger(1);
                base.WriteInteger(256);
                base.WriteString(Item.ExtraData);
                base.WriteInteger(Item.LimitedNo);
                base.WriteInteger(Item.LimitedTot);
            }

            else
                ItemBehaviourUtility.GenerateExtradata(Item, this);

            base.WriteBoolean(Item.GetBaseItem().AllowEcotronRecycle);
            base.WriteBoolean(Item.GetBaseItem().AllowTrade);
            base.WriteBoolean(Item.LimitedNo == 0 ? Item.GetBaseItem().AllowInventoryStack : false);
            base.WriteBoolean(ItemUtility.IsRare(Item));
            base.WriteInteger(-1);//Seconds to expiration.
            base.WriteBoolean(true);
            base.WriteInteger(-1);//Item RoomId

            if (!Item.IsWallItem)
            {
               base.WriteString(string.Empty);
                base.WriteInteger(0);
            }
        }
    }
}