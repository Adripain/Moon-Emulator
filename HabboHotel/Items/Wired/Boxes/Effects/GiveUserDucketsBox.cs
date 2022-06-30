using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Moon.Communication.Packets.Incoming;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Users;
using Moon.Communication.Packets.Outgoing.Rooms.Chat;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.Communication.Packets.Outgoing.Inventory.Purse;

namespace Moon.HabboHotel.Items.Wired.Boxes.Effects
{
    class GiveUserDucketsBox : IWiredItem
    {
        public Room Instance { get; set; }

        public Item Item { get; set; }

        public WiredBoxType Type { get { return WiredBoxType.EffectGiveUserDuckets; } }

        public ConcurrentDictionary<int, Item> SetItems { get; set; }

        public string StringData { get; set; }

        public bool BoolData { get; set; }

        public string ItemsData { get; set; }

        public GiveUserDucketsBox(Room Instance, Item Item)
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
            if (Params == null || Params.Length == 0)
                return false;

            Habbo Owner = MoonEnvironment.GetHabboById(Item.UserID);
            if (Owner == null || !Owner.GetPermissions().HasRight("room_item_wired_rewards"))
                return false;

            Habbo Player = (Habbo)Params[0];
            if (Player == null || Player.GetClient() == null)
                return false;

            RoomUser User = Player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Player.Username);
            if (User == null)
                return false;

            if (String.IsNullOrEmpty(StringData))
                return false;

            int Amount;
            Amount = Convert.ToInt32(StringData);
            if (Amount > 1000)
            {
                Player.GetClient().SendWhisper("La cantidad de duckets pasa de los limites.");
                return false;
            }
            else
            {
                Player.GetClient().GetHabbo().Duckets += Amount;
                Player.GetClient().SendMessage(new HabboActivityPointNotificationComposer(Player.GetClient().GetHabbo().Duckets, Amount));
                Player.GetClient().SendMessage(RoomNotificationComposer.SendBubble("duckets", "" + Player.GetClient().GetHabbo().Username + ", acabas de recibir " + Amount + " Ducket(s).", ""));


            }

            return true;
        }
    }
}