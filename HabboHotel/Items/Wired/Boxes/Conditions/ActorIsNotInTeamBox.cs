using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Users;
using Moon.Communication.Packets.Incoming;
using Moon.HabboHotel.Rooms.Games.Teams;

namespace Moon.HabboHotel.Items.Wired.Boxes.Conditions
{
    class ActorIsNotInTeamBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.ConditionActorIsNotInTeamBox; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public ActorIsNotInTeamBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;

            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            int Unknown2 = Packet.PopInt();

            this.StringData = Unknown2.ToString();
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

            if (int.Parse(this.StringData) == 1 && User.Team != TEAM.RED)
                return true;
            else if (int.Parse(this.StringData) == 2 && User.Team != TEAM.GREEN)
                return true;
            else if (int.Parse(this.StringData) == 3 && User.Team != TEAM.BLUE)
                return true;
            else if (int.Parse(this.StringData) == 4 && User.Team != TEAM.YELLOW)
                return true;
            return false;
        }
    }
}