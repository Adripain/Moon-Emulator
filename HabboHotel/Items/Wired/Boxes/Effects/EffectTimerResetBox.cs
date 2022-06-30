using System;
using System.Collections.Concurrent;
using Moon.Communication.Packets.Incoming;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Users;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using System.Collections;

namespace Moon.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class EffectTimerResetBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectTimerReset; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }
        public int Delay { get { return this._delay; } set { this._delay = value; this.TickCount = value + 1; } }
        public int TickCount { get; set; }
        private int _delay = 0;
        private Queue _queue;

        public EffectTimerResetBox(Room Instance, Item Item)
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
            string Mode = Packet.PopString();
            int Unused = Packet.PopInt();
            this.Delay = Packet.PopInt();

            this.StringData = Mode;
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

                //this.SendFreezeToUser(Player);
            }

            this.TickCount = Delay;
            return true;
        }

        public bool Execute(params object[] Params)
        {
            if (Params == null || Params.Length == 0)
                return false;

                Instance.GetWired().TriggerEvent(WiredBoxType.TriggerAtGivenTime, true);

            Habbo Player = (Habbo)Params[0];
            if (Player == null || Player.GetClient() == null)
                return false;

            if (String.IsNullOrEmpty(StringData))
                return false;

            this._queue.Enqueue(Player);

            return true;
        }

        private void SendMessageToUser(Habbo Player)
        {
            RoomUser User = Player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Player.Username);
            if (User == null)
                return;
        }
    }
}