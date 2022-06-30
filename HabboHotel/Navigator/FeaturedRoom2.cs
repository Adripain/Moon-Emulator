using System;

namespace Moon.HabboHotel.Navigator
{
    public class FeaturedRoom2
    {
        public int roomId { get; set; }
        public string Image { get; set; }

        public FeaturedRoom2(int roomId, string image)
        {
            this.roomId = roomId;
            this.Image = image;
        }
    }
}