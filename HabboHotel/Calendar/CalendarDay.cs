using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.HabboHotel.Calendar
{
    public class CalendarDay
    {
        public int Day;
        public string Gift;
        public string ProductName;
        public string ImageLink;
        public string ItemName;

        public CalendarDay(int day, string gift, string productname, string imagelink, string itemname)
        {
            this.Day = day;
            this.Gift = gift;
            this.ProductName = productname;
            this.ImageLink = imagelink;
            this.ItemName = itemname;
        }
    }
}
