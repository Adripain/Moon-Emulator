using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Games;
using Moon.HabboHotel.Users;

namespace Moon.Communication.Packets.Outgoing.GameCenter
{
    public class Game1WeeklyLeaderboardComposer : ServerPacket
    {
        public Game1WeeklyLeaderboardComposer(int GameId/*, ICollection<Habbo> Habbos*/)
            : base(ServerPacketHeader.Game1WeeklyLeaderboardMessageComposer)
        {
            base.WriteInteger(2014);
            base.WriteInteger(41);
            base.WriteInteger(0);
            base.WriteInteger(1);
            base.WriteInteger(1581);

            //Used to generate the ranking numbers.
            //int num = 0;

            //base.WriteInteger(Habbos.Count);//Count
            //foreach (Habbo Habbo in Habbos.ToList())
            //{
            //    num++;
            //    base.WriteInteger(Habbo.Id);//Id
            //    base.WriteInteger(Habbo.FastfoodScore);//Score
            //    base.WriteInteger(num);//Rank
            //   base.WriteString(Habbo.Username);//Username
            //   base.WriteString(Habbo.Look);//Figure
            //   base.WriteString(Habbo.Gender.ToLower());//Gender .ToLower()
            //}

            //base.WriteInteger(0);//
            //base.WriteInteger(GameData.GameId);//Game Id?

            base.WriteInteger(10);//Count

            base.WriteInteger(1);//Id
            base.WriteInteger(99999999);//Score
            base.WriteInteger(1);//Rank
            base.WriteString("Custom");//Username
            base.WriteString("ch-235-1408.hd-3095-14.lg-3116-85-1408.sh-3115-1408-1408.ca-1805-64.ha-1002-1408");//Figure
            base.WriteString("m");//Gender .ToLower()

            base.WriteInteger(1);//Id
            base.WriteInteger(1000);//Score
            base.WriteInteger(2);//Rank
            base.WriteString("Custom");//Username
            base.WriteString("hd-180-11.hr-828-55.ch-804-96.sh-3089-1186.lg-281-110");//Figure
            base.WriteString("m");//Gender .ToLower()

            base.WriteInteger(1);//Id
            base.WriteInteger(1000);//Score
            base.WriteInteger(3);//Rank
            base.WriteString("Custom");//Username
            base.WriteString("wa-2009-64.ch-3030-1408.hr-3090-45.hd-208-8.lg-3023-64.sh-3068-64-64");//Figure
            base.WriteString("m");//Gender .ToLower()

            base.WriteInteger(1);//Id
            base.WriteInteger(1000);//Score
            base.WriteInteger(4);//Rank
            base.WriteString("Custom");//Username
            base.WriteString("fa-1201-62.sh-6102459-96-62.hr-831-1031.ch-804-1201.lg-281-110.ha-1012-78.hd-180-11");//Figure
            base.WriteString("m");//Gender .ToLower()

            base.WriteInteger(1);//Id
            base.WriteInteger(1000);//Score
            base.WriteInteger(5);//Rank
            base.WriteString("Custom");//Username
            base.WriteString("ch-235-1408.hd-3095-14.lg-3116-85-1408.sh-3115-1408-1408.ca-1805-64.ha-1002-1408");//Figure
            base.WriteString("m");//Gender .ToLower()

            base.WriteInteger(1);//Id
            base.WriteInteger(1000);//Score
            base.WriteInteger(6);//Rank
            base.WriteString("Custom");//Username
            base.WriteString("ch-235-1408.hd-3095-14.lg-3116-85-1408.sh-3115-1408-1408.ca-1805-64.ha-1002-1408");//Figure
            base.WriteString("m");//Gender .ToLower()

            base.WriteInteger(1);//Id
            base.WriteInteger(1000);//Score
            base.WriteInteger(7);//Rank
            base.WriteString("Custom");//Username
            base.WriteString("ch-235-1408.hd-3095-14.lg-3116-85-1408.sh-3115-1408-1408.ca-1805-64.ha-1002-1408");//Figure
            base.WriteString("m");//Gender .ToLower()

            base.WriteInteger(1);//Id
            base.WriteInteger(1000);//Score
            base.WriteInteger(8);//Rank
            base.WriteString("Custom");//Username
            base.WriteString("ch-235-1408.hd-3095-14.lg-3116-85-1408.sh-3115-1408-1408.ca-1805-64.ha-1002-1408");//Figure
            base.WriteString("m");//Gender .ToLower()

            base.WriteInteger(1);//Id
            base.WriteInteger(1000);//Score
            base.WriteInteger(9);//Rank
            base.WriteString("Custom");//Username
            base.WriteString("ch-235-1408.hd-3095-14.lg-3116-85-1408.sh-3115-1408-1408.ca-1805-64.ha-1002-1408");//Figure
            base.WriteString("m");//Gender .ToLower()

            base.WriteInteger(1);//Id
            base.WriteInteger(1000);//Score
            base.WriteInteger(10);//Rank
            base.WriteString("Custom");//Username
            base.WriteString("ch-235-1408.hd-3095-14.lg-3116-85-1408.sh-3115-1408-1408.ca-1805-64.ha-1002-1408");//Figure
            base.WriteString("m");//Gender .ToLower()



            base.WriteInteger(0);//
            base.WriteInteger(GameId);//Game Id?
        }
    }
}
