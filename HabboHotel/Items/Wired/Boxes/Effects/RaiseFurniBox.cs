using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Moon.Communication.Packets.Incoming;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Users;
using System.Drawing;
using System.Security.Cryptography;
using Moon.Communication.Packets.Outgoing.Rooms.Engine;
using Moon.Utilities;


namespace Moon.HabboHotel.Items.Wired.Boxes.Effects
{
    class RaiseFurniBox : IWiredItem, IWiredCycle
    { 
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectRaiseFurni; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public int TickCount { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public int Delay { get { return this._delay; } set { this._delay = value; this.TickCount = value; } }
        public string ItemsData { get; set; }

        private long _next;
        private int _delay = 0;
        private bool Requested = false;

        public RaiseFurniBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            this.SetItems.Clear();
            int Unknown = Packet.PopInt();
            string Unknown2 = Packet.PopString();

            int FurniCount = Packet.PopInt();
            for (int i = 0; i < FurniCount; i++)
            {
                Item SelectedItem = Instance.GetRoomItemHandler().GetItem(Packet.PopInt());
                if (SelectedItem != null)
                    SetItems.TryAdd(SelectedItem.Id, SelectedItem);
            }

            int Delay = Packet.PopInt();
            this.Delay = Delay;
        }

        public bool Execute(params object[] Params)
        {
            if (this.SetItems.Count == 0)
                return false;

            if (this._next == 0 || this._next < MoonEnvironment.Now())
                this._next = MoonEnvironment.Now() + this.Delay;

            if (!Requested)
            {
                this.TickCount = this.Delay;
                this.Requested = true;
            }
            return true;
        }

        public bool OnCycle()
        {
            if (Instance == null || !Requested || _next == 0)
                return false;

            long Now = MoonEnvironment.Now();
            if (_next < Now)
            {
                foreach (Item Item in this.SetItems.Values.ToList())
                {
                    if (Item == null)
                        continue;

                    if (!Instance.GetRoomItemHandler().GetFloor.Contains(Item))
                        continue;

                    if (Item.GetZ > 80.00)
                        continue;

                    Item.GetZ++;
                    Item.UpdateState();
                }

                _next = 0;
                return true;
            }
            return false;
        }
    }
}