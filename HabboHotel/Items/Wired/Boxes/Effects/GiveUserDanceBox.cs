using System;
using System.Collections.Concurrent;
using Moon.Communication.Packets.Incoming;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Users;
using Moon.Communication.Packets.Outgoing.Rooms.Avatar;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Items.Wired.Boxes.Effects
{
    class GiveUserDanceBox : IWiredItem
    {
        public Room Instance { get; set; }

        public Item Item { get; set; }

        public WiredBoxType Type { get { return WiredBoxType.EffectGiveUserDance; } }

        public ConcurrentDictionary<int, Item> SetItems { get; set; }

        public string StringData { get; set; }

        public bool BoolData { get; set; }

        public string ItemsData { get; set; }

        public GiveUserDanceBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string Badge = Packet.PopString();

            this.StringData = Badge;
        }

        public bool Execute(params object[] Params)
        {
            if (Params == null || Params.Length == 0)
                return false;

            Habbo Player = (Habbo)Params[0];
            if (Player == null || Player.GetClient() == null)
                return false;

            RoomUser User = Player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Player.Username);
            if (User == null)
                return false;

            if (String.IsNullOrEmpty(StringData))
                return false;

            var dance = int.Parse(StringData);
            User.GetClient().GetHabbo().CurrentRoom.SendMessage(new DanceComposer(User, dance));
            //User.GetClient().SendMessage(RoomNotificationComposer.SendBubble("wfdance", "" + User.GetClient().GetHabbo().Username + ", acabas de activar el baile " + StringData + " mediante Wired.", ""));

            return true;
        }
    }
}