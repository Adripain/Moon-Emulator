using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.HabboHotel.Moderation
{
    class ModerationPreset
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }

        public ModerationPreset(int Id, string Type, string Message)
        {
            this.Id = Id;
            this.Type = Type;
            this.Message = Message;
        }
    }
}
