using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Moon.Communication.Packets.Incoming;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Users;
using Moon.Communication.Packets.Outgoing.Rooms.Chat;
using Moon.Communication.Packets.Outgoing.Inventory.Furni;
using Moon.Communication.Packets.Outgoing.Catalog;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.Communication.Packets.Outgoing.Inventory.Purse;
using Moon.Communication.Packets.Outgoing.Rooms.Engine;

namespace Moon.HabboHotel.Items.Wired.Boxes.Effects
{
    class GiveRewardBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectGiveReward; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public GiveRewardBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            int Often = Packet.PopInt();
            bool Unique = (Packet.PopInt() == 1);
            int Limit = Packet.PopInt();
            int Often_No = Packet.PopInt();
            string Reward = Packet.PopString();

            this.BoolData = Unique;
            this.StringData = Reward + "-" + Often + "-" + Limit + "-" + Often_No;
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

            Room Room = Player.CurrentRoom;

            if (String.IsNullOrEmpty(StringData))
                return false;

            int SplitNumber = -1;
            int oftenforuser = int.Parse(this.StringData.Split('-')[3]);
            int amountLeft = int.Parse(this.StringData.Split('-')[2]);
            int often = int.Parse(this.StringData.Split('-')[1]);
            string Reward = this.StringData.Split('-')[0];
            bool unique = this.BoolData;
            int totalrewards = (this.StringData.Split('-')[0]).Split(';').Count();
            bool premied = false;

            /*
             * Often numbers
             * 3- 1/min
             * 2- 1/Hour
             * 1- 1/Day
             * 0- Once
             * 
             */

            if (amountLeft == 1)
            {
                Player.GetClient().SendMessage(new WiredRewardAlertComposer(0));
                return true;
            }

            if (!unique)
            {
                int percentage1 = 0;
                int percentage2 = 0;
                int percentage3 = 0;
                int percentage4 = 0;
                int percentage5 = 0;
                int totalpercentage = 0;

                if (totalrewards > 0)
                    percentage1 = int.Parse(((this.StringData.Split('-')[0]).Split(';')[0]).Split(',')[2]);
                else if (totalrewards > 1)
                    percentage2 = int.Parse(((this.StringData.Split('-')[0]).Split(';')[1]).Split(',')[2]) + percentage1;
                else if (totalrewards > 2)
                    percentage3 = int.Parse(((this.StringData.Split('-')[0]).Split(';')[2]).Split(',')[2]) + percentage2;
                else if (totalrewards > 3)
                    percentage4 = int.Parse(((this.StringData.Split('-')[0]).Split(';')[3]).Split(',')[2]) + percentage3;
                else if (totalrewards > 4)
                    percentage5 = int.Parse(((this.StringData.Split('-')[0]).Split(';')[4]).Split(',')[2]) + percentage4;

                totalpercentage = percentage5 + percentage4 + percentage3 + percentage2 + percentage1;

                var random = MoonEnvironment.GetRandomNumber(0, 100);

                if(random > totalpercentage)
                {
                    Player.GetClient().SendMessage(new WiredRewardAlertComposer(4));
                    return true;
                }
                    

                if (percentage1 >= random)
                    SplitNumber = 0;
                else if (percentage1 <= random && random <= percentage2)
                    SplitNumber = 1;
                else if (percentage2 <= random && random <= percentage3)
                    SplitNumber = 2;
                else if (percentage3 <= random && random <= percentage4)
                    SplitNumber = 3;
                else if (percentage4 <= random && random <= percentage5 || random >= percentage5)
                    SplitNumber = 4;

                Player.GetClient().SendWhisper(random + " | " + SplitNumber + " | " + totalpercentage);               

                var dataArray = ((this.StringData.Split('-')[0]).Split(';')[SplitNumber]).Split(',');

                var isbadge = dataArray[0] == "0";
                var code = dataArray[1];
                var percentage = int.Parse(dataArray[2]);

                premied = true;

                if (isbadge)
                {
                    if (code.StartsWith("diamonds:"))
                    {
                        foreach (var reward in code.Split('-'))
                        {
                            var dataArray2 = code.Split(':');
                            var diamonds = int.Parse(dataArray2[1]);
                            if (diamonds > 100)
                            {
                                Player.GetClient().SendMessage(new RoomCustomizedAlertComposer("¡Ha ocurrido un error! Avisa a un miembro del equipo Staff."));
                                MoonEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("advice", "" + Player.GetClient().GetHabbo().Username + " ha usado un wired que no sigue las normas del hotel.\n             Click aquí.", "event:navigator/goto/" + Player.GetClient().GetHabbo().CurrentRoomId));
                            }
                            else
                            {
                                Player.GetClient().GetHabbo().Diamonds += diamonds;
                                Player.GetClient().SendMessage(new HabboActivityPointNotificationComposer(Player.GetClient().GetHabbo().Diamonds, diamonds, 5));
                            }
                        }
                    }
                    else if (Player.GetBadgeComponent().HasBadge(code))
                        Player.GetClient().SendMessage(new WiredRewardAlertComposer(5));
                    else
                    {
                        Player.GetBadgeComponent().GiveBadge(code, true, Player.GetClient());
                        Player.GetClient().SendMessage(new WiredRewardAlertComposer(7));
                    }
                }
                else
                {
                    ItemData ItemData = null;

                    if (!MoonEnvironment.GetGame().GetItemManager().GetItem(int.Parse(code), out ItemData))
                    {
                        Player.GetClient().SendMessage(new WhisperComposer(User.VirtualId, "No se pudo obtener Item ID: " + code, 0, User.LastBubble));
                        return false;
                    }

                    Item Item = ItemFactory.CreateSingleItemNullable(ItemData, Player.GetClient().GetHabbo(), "", "", 0, 0, 0);


                    if (Item != null)
                    {
                        Player.GetClient().GetHabbo().GetInventoryComponent().TryAddItem(Item);
                        Player.GetClient().SendMessage(new FurniListNotificationComposer(Item.Id, 1));
                        Player.GetClient().SendMessage(new PurchaseOKComposer());
                        Player.GetClient().SendMessage(new FurniListAddComposer(Item));
                        Player.GetClient().SendMessage(new FurniListUpdateComposer());
                        Player.GetClient().SendMessage(new WiredRewardAlertComposer(6));
                    }
                }
            }
           else
            {
                foreach (var dataStr in (this.StringData.Split('-')[0]).Split(';'))
                {
                    var dataArray = dataStr.Split(',');

                    var isbadge = dataArray[0] == "0";
                    var code = dataArray[1];
                    var percentage = int.Parse(dataArray[2]);

                    var random = MoonEnvironment.GetRandomNumber(0, 100);

                    premied = true;

                    if (isbadge)
                    {
                        if (code.StartsWith("diamonds:"))
                        {
                            foreach (var reward in code.Split('-'))
                            {
                                var dataArray2 = code.Split(':');
                                var diamonds = int.Parse(dataArray2[1]);
                                if (diamonds > 100)
                                {
                                    Player.GetClient().SendMessage(new RoomCustomizedAlertComposer("¡Ha ocurrido un error! Avisa a un miembro del equipo Staff."));
                                    MoonEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("advice", "" + Player.GetClient().GetHabbo().Username + " ha usado un wired que no sigue las normas del hotel.\n             Click aquí.", "event:navigator/goto/" + Player.GetClient().GetHabbo().CurrentRoomId));
                                }
                                else
                                {
                                    Player.GetClient().GetHabbo().Diamonds += diamonds;
                                    Player.GetClient().SendMessage(new HabboActivityPointNotificationComposer(Player.GetClient().GetHabbo().Diamonds, diamonds, 5));
                                }
                            }
                        }
                        if (Player.GetBadgeComponent().HasBadge(code))
                            Player.GetClient().SendMessage(new WiredRewardAlertComposer(5));
                        else
                        {
                            Player.GetBadgeComponent().GiveBadge(code, true, Player.GetClient());
                            Player.GetClient().SendMessage(new WiredRewardAlertComposer(7));
                        }
                    }
                    else
                    {
                        ItemData ItemData = null;

                        if (!MoonEnvironment.GetGame().GetItemManager().GetItem(int.Parse(code), out ItemData))
                        {
                            Player.GetClient().SendMessage(new WhisperComposer(User.VirtualId, "No se pudo obtener Item ID: " + code, 0, User.LastBubble));
                            return false;
                        }

                        Item Item = ItemFactory.CreateSingleItemNullable(ItemData, Player.GetClient().GetHabbo(), "", "", 0, 0, 0);


                        if (Item != null)
                        {
                            Player.GetClient().GetHabbo().GetInventoryComponent().TryAddItem(Item);
                            Player.GetClient().SendMessage(new FurniListNotificationComposer(Item.Id, 1));
                            Player.GetClient().SendMessage(new PurchaseOKComposer());
                            Player.GetClient().SendMessage(new FurniListAddComposer(Item));
                            Player.GetClient().SendMessage(new FurniListUpdateComposer());
                            Player.GetClient().SendMessage(new WiredRewardAlertComposer(6));
                        }
                    }
                }
            }

            if(premied)
            {
                
            }
            else if (!premied)
            {
                Player.GetClient().SendMessage(new WiredRewardAlertComposer(4));
            }
            else if (amountLeft > 1)
            {
                amountLeft--;
                this.StringData = Reward + "-" + often + "-" + amountLeft + "-" + oftenforuser;              
            }

            return true;
        }
    }
}