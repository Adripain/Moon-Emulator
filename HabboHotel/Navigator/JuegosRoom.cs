using System;

namespace Moon.HabboHotel.Navigator
{
    public class JuegosRoom
    {
        public int roomId { get; set; }
        public string Image { get; set; }

        public JuegosRoom(int roomId, string image)
        {
            this.roomId = roomId;
            this.Image = image;
        }
    }
}