using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Items;
using Moon.Communication.Packets.Outgoing.Inventory.Furni;



namespace Moon.Communication.Packets.Incoming.Inventory.Furni
{
    class RequestFurniInventoryEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            ICollection<Item> FloorItems = Session.GetHabbo().GetInventoryComponent().GetFloorItems();
            ICollection<Item> WallItems = Session.GetHabbo().GetInventoryComponent().GetWallItems();

            List<Item> allItems = new List<Item>();
            allItems.AddRange(FloorItems);
            allItems.AddRange(WallItems);

            if(allItems.Count == 0)
            {
                Session.SendMessage(new FurniListComposer());
                return;
            }

            Dictionary<int, List<Item>> slots = new Dictionary<int, List<Item>>();
            int currentSlot = 0;
            foreach (Item item in allItems)
            {
                if (!slots.ContainsKey(currentSlot))
                    slots.Add(currentSlot, new List<Item>());

                List<Item> items = slots[currentSlot];
                if (items.Count > 700)
                {
                    currentSlot++;
                    slots.Add(currentSlot, new List<Item>());
                    items = slots[currentSlot];
                }

                items.Add(item);


            }
            int i = 0;

            foreach(List<Item> items in slots.Values)
            {
                Session.SendMessage(new FurniListComposer(items, i, slots.Count));
                i++;
            }
        }

    }
}