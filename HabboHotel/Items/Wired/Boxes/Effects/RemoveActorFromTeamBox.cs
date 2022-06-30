﻿using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Users;
using Moon.HabboHotel.Rooms.Games.Teams;
using Moon.Communication.Packets.Incoming;
using Moon.Communication.Packets.Outgoing.Rooms.Settings;

namespace Moon.HabboHotel.Items.Wired.Boxes.Effects
{
    class RemoveActorFromTeamBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectRemoveActorFromTeam; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public RemoveActorFromTeamBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;

            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
        }

        public bool Execute(params object[] Params)
        {
            if (Params.Length == 0 || Instance == null)
                return false;

            Habbo Player = (Habbo)Params[0];
            if (Player == null)
                return false;

            RoomUser User = Instance.GetRoomUserManager().GetRoomUserByHabbo(Player.Id);
            if (User == null)
                return false;

            if (User.Team != TEAM.NONE)
            {
                TeamManager Team = Instance.GetTeamManagerForFreeze();
                if (Team != null)
                {
                    Team.OnUserLeave(User);

                    User.Team = TEAM.NONE;

                    if (User.GetClient().GetHabbo().Effects().CurrentEffect != 0)
                        User.GetClient().GetHabbo().Effects().ApplyEffect(0);

                    User.GetClient().SendMessage(new HideUserOnPlaying(false));
                }
            }
            return true;
        }
    }
}