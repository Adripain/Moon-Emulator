using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.HabboHotel.Navigator
{
    public class LoteSalas
    {
        public int roomId { get; set; }
        public string Image { get; set; }

        public LoteSalas(int roomId, string image)
        {
            this.roomId = roomId;
            this.Image = image;
        }
    }
}
