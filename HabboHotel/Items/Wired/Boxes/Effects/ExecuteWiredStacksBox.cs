﻿using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Moon.Communication.Packets.Incoming;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Users;
using System.Collections;

namespace Moon.HabboHotel.Items.Wired.Boxes.Conditions
{
    class ExecuteWiredStacksBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectExecuteWiredStacks; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }
        public int Delay { get { return this._delay; } set { this._delay = value; this.TickCount = value + 1; } }
        public int TickCount { get; set; }
        private int _delay = 0;
        private Queue _queue;

        public ExecuteWiredStacksBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
            this.TickCount = Delay;
            this._queue = new Queue();
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
            this.Delay = Packet.PopInt();
        }

        public bool OnCycle()
        {
            if (_queue.Count == 0)
            {
                this._queue.Clear();
                this.TickCount = Delay;
                return true;
            }

            while (_queue.Count > 0)
            {
                Habbo Player = (Habbo)_queue.Dequeue();
                if (Player == null || Player.CurrentRoom != Instance)
                    continue;

                this.ExecuteWiredStacks(Player);
            }

            this.TickCount = Delay;
            return true;
        }

        public bool Execute(params object[] Params)
        {
            if (Params.Length != 1)
                return false;

            Habbo Player = (Habbo)Params[0];
            if (Player == null)
                return false;

            this._queue.Enqueue(Player);
            return true;
        }

        private void ExecuteWiredStacks(Habbo Player)
        {
            foreach (Item Item in this.SetItems.Values.ToList())
            {
                if (Item == null || !Instance.GetRoomItemHandler().GetFloor.Contains(Item) || !Item.IsWired)
                    continue;

                IWiredItem WiredItem;
                if (Instance.GetWired().TryGet(Item.Id, out WiredItem))
                {
                    if (WiredItem.Type == WiredBoxType.EffectExecuteWiredStacks)
                        continue;
                    else
                    {
                        ICollection<IWiredItem> Effects = Instance.GetWired().GetEffects(WiredItem);
                        if (Effects.Count > 0)
                        {
                            foreach (IWiredItem EffectItem in Effects.ToList())
                            {
                                if (SetItems.ContainsKey(EffectItem.Item.Id) && EffectItem.Item.Id != Item.Id)
                                    continue;
                                else if (EffectItem.Type == WiredBoxType.EffectExecuteWiredStacks)
                                    continue;
                                else
                                    EffectItem.Execute(Player);
                            }
                        }
                    }
                }
                else continue;
            }
        }
    }
}