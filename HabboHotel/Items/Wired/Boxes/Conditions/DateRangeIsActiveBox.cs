using Moon.Communication.Packets.Incoming;
using Moon.HabboHotel.Rooms;
using System;
using System.Collections.Concurrent;

namespace Moon.HabboHotel.Items.Wired.Boxes.Conditions
{
    class DateRangeIsActiveBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.ConditionDateRangeActive; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }
        public int StartDate { get; set; }
        public int EndDate { get; set; }

        public DateRangeIsActiveBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;

            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            int Date1 = Packet.PopInt();
            int Date2 = Packet.PopInt();

            this.StringData =  Convert.ToString(Date1 + ";" + Date2);
            this.StartDate = Date1;
            this.EndDate = Date2;
          
        }

        public bool Execute(params object[] Params)
        {
            if (Params.Length == 0 || Instance == null || String.IsNullOrEmpty(this.StringData))
                return false;
            
            int TimeStamp = MoonEnvironment.GetIUnixTimestamp();
            if (TimeStamp < this.StartDate || TimeStamp > this.EndDate)
            { return false; }
            else
            return true;
        }
    }
}