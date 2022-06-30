using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Moon.Communication.Packets.Incoming;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Pathfinding;
using System.Drawing;

namespace Moon.HabboHotel.Items.Wired.Boxes.Conditions
{
    class TotalUsersCoincidenceBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.TotalUsersCoincidence; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public TotalUsersCoincidenceBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string Unknown2 = Packet.PopString();

            if (this.SetItems.Count > 0)
                this.SetItems.Clear();

            int FurniCount = Packet.PopInt();
            for (int i = 0; i < FurniCount; i++)
            {
                Item SelectedItem = Instance.GetRoomItemHandler().GetItem(Packet.PopInt());
                if (SelectedItem != null)
                    SetItems.TryAdd(SelectedItem.Id, SelectedItem);
            }
        }

        public bool Execute(params object[] Params)
        {
            int desiredusers = 0;
            bool exe = false;
            this.StringData = "0";

            foreach (Item Item in this.SetItems.Values.ToList())
            {
                if (Item == null || !Instance.GetRoomItemHandler().GetFloor.Contains(Item))
                    continue;

                desiredusers = desiredusers + Instance.GetGameMap().GetRoomUsers(new Point(Item.GetX, Item.GetY)).Count();
            }

            if (desiredusers == int.Parse(this.StringData))
                exe = true;

            if (!exe)
                return false;

            return true;
        }
    }
}