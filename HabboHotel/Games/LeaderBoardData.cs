using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.HabboHotel.Games
{
    public class LeaderBoardData
    {
        public int GameId { get; set; }
        public int UserId { get; set; }
        public int Points { get; set; }
        public int Record { get; set; }
        public int Week { get; set; }
        public int Year { get; set; }

        public LeaderBoardData(int GameId, int UserId, int Points, int Record, int Week, int Year)
        {
            this.GameId = GameId;
            this.UserId = UserId;
            this.Points = Points;
            this.Record = Record;
            this.Week = Week;
            this.Year = Year;
        }
    }
}
