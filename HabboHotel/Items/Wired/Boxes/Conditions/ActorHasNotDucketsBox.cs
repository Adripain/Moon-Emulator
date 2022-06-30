﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Users;
using Moon.Communication.Packets.Incoming;

namespace Moon.HabboHotel.Items.Wired.Boxes.Conditions
{
    class ActorHasNotDucketsBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.ConditionActorHasNotDuckets; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public ActorHasNotDucketsBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;

            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string Duckets = Packet.PopString();

            this.StringData = Duckets;
        }

        public bool Execute(params object[] Params)
        {
            if (Params.Length == 0 || Instance == null || String.IsNullOrEmpty(this.StringData))
                return false;

            Habbo Player = (Habbo)Params[0];
            if (Player == null)
                return false;

            RoomUser User = Instance.GetRoomUserManager().GetRoomUserByHabbo(Player.Id);
            if (User == null)
                return false;

            if (User.GetClient().GetHabbo().Duckets > int.Parse(this.StringData))
                return false;

            return true;
        }
    }
}